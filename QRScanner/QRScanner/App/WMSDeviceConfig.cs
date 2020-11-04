using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using TrendNET.WMS.Core.Data;
using TrendNET.WMS.Device.Services;

namespace QRScanner.App
{
    public class WMSDeviceConfigItem
    {
        public string Key;
        public string Value;
    }

    public class WMSDeviceConfig
    {
        private static List<WMSDeviceConfigItem> config = null;

        public static string ExePath()
        {
            string exeFile = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            if (exeFile.StartsWith("file:///"))
                exeFile = exeFile.Substring("file:///".Length);
            var exePath = Path.GetDirectoryName(exeFile);
            return exePath;
        }

        public static void LoadConfig()
        {
            if (config == null)
            {
                config = new List<WMSDeviceConfigItem>();

                using (var sr = new StreamReader(Path.Combine (ExePath (), "WMS.config.txt")))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (line != null) {
                            line = line.Trim ();
                            if (!string.IsNullOrEmpty (line)) {
                                var colonPos = line.IndexOf(':');
                                if (colonPos >= 0)
                                {
                                    var key = line.Substring(0, colonPos).Trim ();
                                    var value = line.Substring(colonPos + 1).Trim ();
                                    config.Add (new WMSDeviceConfigItem { Key = key, Value = value });
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ReconfigureViaAzureIfNeeded()
        {
            try
            {
                var data = string.Join(";", config.Select(c => c.Key + "=" + c.Value).ToArray());
                string result;
                if (WebApp.PostAzure(data, out result, 60000))
                {
                    if (result.StartsWith("OK:"))
                    {
                        var newValsStr = result.Substring(3);
                        if (!string.IsNullOrEmpty(newValsStr))
                        {
                            var newVals = newValsStr.Split(';').ToList();
                            if (newVals.Count > 0)
                            {
                                newVals.ForEach(nv =>
                                {
                                    if (!string.IsNullOrEmpty(nv))
                                    {
                                        var pair = nv.Split('=').ToList();
                                        var c = config.FirstOrDefault(x => x.Key == pair[0]);
                                        if (c == null)
                                        {
                                            config.Add(new WMSDeviceConfigItem { Key = pair[0], Value = pair[1] });
                                        }
                                        else
                                        {
                                            c.Value = pair[1];
                                        }
                                    }
                                });

                                using (var sw = new StreamWriter(Path.Combine(ExePath(), "WMS.config.new"), false))
                                {
                                    config.ForEach(c =>
                                    {
                                        sw.WriteLine(c.Key + ": " + c.Value);
                                    });
                                }

                                if (File.Exists(Path.Combine(ExePath(), "WMS.config.old"))) { File.Delete(Path.Combine(ExePath(), "WMS.config.old")); }
                                File.Move(Path.Combine(ExePath(), "WMS.config.txt"), Path.Combine(ExePath(), "WMS.config.old"));
                                File.Move(Path.Combine(ExePath(), "WMS.config.new"), Path.Combine(ExePath(), "WMS.config.txt"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Services.Services.ReportException(ex);
            }
        }

        public static string GetString(string key, string def)
        {
            LoadConfig();
            var el = config.FirstOrDefault(x => x.Key == key);
            if (el == null)
            {
                return def;
            }
            else
            {
                return el.Value;
            }
        }
    }
}
