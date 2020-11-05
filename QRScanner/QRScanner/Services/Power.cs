using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace QRScanner.Services
{
    public class Power
    {
        public const int PPN_UNATTENDEDMODE = 0x0003;

        [DllImport("coredll.dll")]
        public static extern void SystemIdleTimerReset();

        [DllImport("coredll.dll")]
        private static extern bool PowerPolicyNotify(int dwMessage, bool dwData);

        private static object ppnLock = new object();
        private static volatile int ppnCount = 0;

        public static bool IsUnattendedMode()
        {
            lock (ppnLock)
            {
                return ppnCount > 0;
            }
        }

        public static void EnterUnattendedMode()
        {
            lock (ppnLock)
            {
                if (ppnCount == 0)
                {
                    Power.PowerPolicyNotify(Power.PPN_UNATTENDEDMODE, true);
                }
                ppnCount++;
            }
        }

        public static void ForceExitUnattendedMode()
        {
            lock (ppnLock)
            {
                if (ppnCount > 0)
                {
                    Power.PowerPolicyNotify(Power.PPN_UNATTENDEDMODE, false);
                    ppnCount = 0;
                }
            }
        }

        public static void ExitUnattendedMode()
        {
            var t = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(30000);
                lock (ppnLock)
                {
                    ppnCount--;
                    if (ppnCount == 0)
                    {
                        Power.PowerPolicyNotify(Power.PPN_UNATTENDEDMODE, false);
                    }
                    else if (ppnCount < 0)
                    {
                        Services.ReportData("Exception: ExitUnattendedMode results in negative count!");
                        ppnCount = 0;
                    }
                }
            }));
            t.Start();
        }
    }
}
