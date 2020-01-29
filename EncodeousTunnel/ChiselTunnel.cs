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
    class ChiselTunnel
    {
        private Process chiselProcess;
        private Thread processParentThread;
        bool stop = false;
        public void Start()
        {
            processParentThread = new Thread(ProgramStarter);
            processParentThread.Start();
        }

        public void Stop()
        {
            stop = true;
            if(!chiselProcess.HasExited) chiselProcess?.Kill();
        }
        public void ProgramStarter()
        {
            if (!File.Exists(Path.GetTempPath() + "chisel.temp"))
            {
                Utils.WriteResourceToFile( "chisel", "chisel.temp");
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
                    Arguments = "server --port 27463"
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
                StatusUpdate.SetStatus("Status: Chisel Server Stopped!");
                processParentThread = null;
                StatusUpdate.ErrorState();
            }
        }
        private void StandardUpdate(object sender, DataReceivedEventArgs e)
        {
            StatusUpdate.UpdateConsole(e.Data + "\n");
        }

    }
}
