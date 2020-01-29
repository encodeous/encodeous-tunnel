using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EncodeousTunnel
{
    class CloudflareManager
    {
        private Process cloudflaredProcess;
        private Thread processParentThread;
        private bool grabbedIP = false;
        bool stop = false;
        private int clientsconnected = 0;
        public void Start()
        {
            processParentThread = new Thread(ProgramStarter);
            processParentThread.Start();
        }

        public void Stop()
        {
            stop = true;
            if(!cloudflaredProcess.HasExited) cloudflaredProcess?.Kill();
        }
        public void ProgramStarter()
        {
            if (!File.Exists(Path.GetTempPath() + "cloudflared.temp"))
            {
                Utils.WriteResourceToFile("cloudflared", "cloudflared.temp");
            }
            cloudflaredProcess = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.GetTempPath() + "cloudflared.temp",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    Arguments = "tunnel --url localhost:27463 --proxy-keepalive-timeout 24h" +
                            " --proxy-tcp-keepalive 24h --proxy-connection-timeout 24h --proxy-expect-continue-timeout 24h" +
                            " --compression-quality 10 --loglevel debug"
                }
            };
            cloudflaredProcess.OutputDataReceived += StandardUpdate;
            cloudflaredProcess.ErrorDataReceived += StandardUpdate;
            cloudflaredProcess.Start();
            cloudflaredProcess.BeginOutputReadLine();
            cloudflaredProcess.BeginErrorReadLine();
            cloudflaredProcess.WaitForExit();
            if (!stop)
            {
                StatusUpdate.SetStatus("Status: Error, Cloudflare Service stopped! Check Debug Console.");
                processParentThread = null;
                StatusUpdate.ErrorState();
            }
        }
        private void StandardUpdate(object sender, DataReceivedEventArgs e)
        {
            StatusUpdate.UpdateConsole(e.Data+"\n");
            ConsoleChecker(e.Data);
        }

        public void ConsoleChecker(string s)
        {
            if (s == null)
            {
                return;
            }
            if (!grabbedIP)
            {
                if (s.Contains("] |    https://") || s.Contains(".trycloudflare.com"))
                {
                    int ind = s.IndexOf("] |    https://");
                    string str2 = s.Substring(ind);
                    str2 = str2.Replace("] |    https://", "");
                    int ind2 = str2.IndexOf(".trycloudflare.com");
                    string ipString = str2.Substring(0, ind2);
                    StatusUpdate.AddressData(ipString);
                    StatusUpdate.SetStatus("Connected to Cloudflare network! Clients can now connect to the generated id.");
                    grabbedIP = true;
                }
            }

            if (s.Contains("Sec-Websocket-Protocol:[chisel-v3]"))
            {
                clientsconnected++;
                StatusUpdate.SetStatus("Status: "+clientsconnected+" clients are now connected!");
            }
        }
    }
}
