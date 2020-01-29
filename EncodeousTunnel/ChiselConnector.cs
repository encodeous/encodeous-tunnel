using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncodeousTunnel
{
    class ChiselConnector
    {
        private Process chiselProcess;
        private Thread processParentThread;
        bool stop = false;
        private string _ip, _port;
        public void Start(string ip, string port)
        {
            _ip = ip;
            _port = port;
            processParentThread = new Thread(ProgramStarter);
            processParentThread.Start();
        }

        public void Stop()
        {
            stop = true;
            if (!chiselProcess.HasExited) chiselProcess?.Kill();
        }
        public void ProgramStarter()
        {
            if (!File.Exists(Path.GetTempPath() + "chisel.temp"))
            {
                Utils.WriteResourceToFile("chisel", "chisel.temp");
            }
            chiselProcess = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.GetTempPath() + "chisel.temp",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    Arguments = "client https://" + _ip + ".trycloudflare.com " + _port,
                }
            };
            chiselProcess.OutputDataReceived += StandardUpdate;
            chiselProcess.ErrorDataReceived += StandardUpdate;
            chiselProcess.Start();
            chiselProcess.BeginOutputReadLine();
            chiselProcess.BeginErrorReadLine();
            chiselProcess.WaitForExit();
            if (!stop)
            {
                StatusUpdate.SetStatus("Status: Chisel Client Stopped!");
                processParentThread = null;
                StatusUpdate.ErrorState();
            }
        }
        private void StandardUpdate(object sender, DataReceivedEventArgs e)
        {
            StatusUpdate.UpdateConsole(e.Data + "\n");
            ConsoleChecker(e.Data);
        }
        public void ConsoleChecker(string s)
        {
            if (s == null)
            {
                return;
            }

            if (s.Contains("Connected (Latency "))
            {
                string tem = s.Substring(s.IndexOf("Connected (Latency "));
                tem = tem.Substring(0, tem.IndexOf("ms)")).Replace("Connected (Latency ","");
                StatusUpdate.SetStatus("Status: Connected to tunnel! Latency "+tem+" miliseconds. Specified ports mapped to localhost:<port>");
            }

            if (s.Contains("Retrying in"))
            {
                StatusUpdate.SetStatus("Status: Disconnected from server! Attempting to reconnect!");
            }
        }
    }
}
