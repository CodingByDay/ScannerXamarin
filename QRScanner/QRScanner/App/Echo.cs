using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QRScanner.App
{
    public class Echo
    {
        public static bool IsWebAppReady(out string result)
        {
            if (WebApp.Get ("Echo.aspx", out result)) {
                if (result.Contains("OK!"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } else {
                return false;
            }
        }
    }
}
