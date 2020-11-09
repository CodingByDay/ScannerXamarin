using Android.App;
using QRscanner.App.ScannerAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace QRScanner
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(OnUnhandledException);

            var ldt = new Thread(new ThreadStart(Log.Dumper));
            ldt.IsBackground = true;
            ldt.Start();

            Application.Run(new Registration());
            Log.DumpEntries();
        }

        public static void Exit(Action a)
        {
            try
            {
                try
                {
                    ScannerFactory.ReaderInstance.DisableScanner();
                    a();
                }
                catch (Exception exHandler)
                {
                    MessageBox.Show(exHandler.Message);
                }
            }
            finally
            {
                try
                {
                    Appclases.Log.DumpEntries();
                    Appclases.Power.ForceExitUnattendedMode();
                    Process.GetCurrentProcess().Kill();
                }
                catch (Exception exKill)
                {
                    MessageBox.Show(exKill.Message);
                }
            }
        }

        private static void OnUnhandledException(Object sender,
            UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                Exit(() =>
                {
                    Services.Services.ReportException(ex);
                    Appclases.Log.DumpEntries();
                    MessageBox.Show("Pri delovanju programa WMS " + Appclases.CommonData.Version + " je prišlo do kritične napake: " + ex.Message);
                });
            }
        }
    
}
}
