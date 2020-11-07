using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QRScanner.Appclasses
{
    public class Echo
    {
        public static bool IsWebAppclassesReady(out string result)
        {
            if (WebAppclasses.Get ("Echo.aspx", out result)) {
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
