using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using TrendNET.WMS.Device.Components;

namespace QRScanner.App
{
    public class WindowDispatcher
    {
        private static Func<WMSForm> nextFormFunc;

        public static void SetNextWindow(Func<WMSForm> formFunc)
        {
            nextFormFunc = formFunc;
        }

        public static Form GetNextForm()
        {
            var startedAt = DateTime.Now;
            try
            {
                var formFunc = nextFormFunc;
                nextFormFunc = null;
                return formFunc == null ? null : formFunc();
            }
            finally
            {
                Log.Write(new LogEntry("END REQUEST: [NextForm];" + (DateTime.Now - startedAt).TotalMilliseconds.ToString()));
            }
        }
    }
}
