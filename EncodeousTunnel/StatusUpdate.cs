using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodeousTunnel
{
    class StatusUpdate
    {
        public delegate void Update(string line);

        public static Update DebugConsoleUpdater;

        public static Update StatusUpdater;

        public static Update Error;

        public static Update Address;

        public static void SetStatus(string s)
        {
            try
            { 
                Form1.instance.Invoke(StatusUpdater, s);
            }
            catch
            {

            }
            UpdateConsole(s);
        }

        public static void UpdateConsole(string s)
        {
            try
            {
                Form1.instance.Invoke(DebugConsoleUpdater, s+"\n");
            }
            catch
            {

            }
        }

        public static void ErrorState()
        {
            try
            {
                Form1.instance.Invoke(Error, "");
            }
            catch
            {

            }
        }

        public static void AddressData(string s)
        {
            try
            {
                Form1.instance.Invoke(Address, s);
            }
            catch
            {

            }
        }
    }
}
