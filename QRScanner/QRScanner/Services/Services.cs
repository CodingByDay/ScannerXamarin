using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml.Serialization;

using TrendNET.WMS.Core.Data;
using TrendNET.WMS.Device.App;
using TrendNET.WMS.Device.Components;
using QRScanner.App;

namespace TrendNET.WMS.Device.Services
{
    public class Services
    {
        public static List<NameValue> UserInfo = new List<NameValue>();

        public static void ClearUserInfo()
        {
            UserInfo.Clear();
        }

        public static bool HasPermission(string perm, string minLevel)
        {
            var usePerm = CommonData.GetSetting("UsePermissions");
            if (string.IsNullOrEmpty(usePerm) || usePerm == "1")
            {
                var item = UserInfo.FirstOrDefault(x => x.Name == "Perm" + perm);
                if (item == null)
                {
                    return false;
                }
                else
                {
                    if (string.IsNullOrEmpty(item.StringValue) || (item.StringValue == "0")) { return false; }
                    if ((minLevel == "R") && (item.StringValue == "R" || item.StringValue == "W" || item.StringValue == "D")) { return true; }
                    if ((minLevel == "W") && (item.StringValue == "W" || item.StringValue == "D")) { return true; }
                    if ((minLevel == "D") && (item.StringValue == "D")) { return true; }
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public static bool IsDeviceActive(out string error)
        {
            string result;
            if (WebApp.Get("mode=deviceActive", out result))
            {
                if (result.Contains("Active!"))
                {
                    error = "";
                    return true;
                }
                else
                {
                    error = result;
                    return false;
                }
            }
            else
            {
                error = "Napaka pri dostopu do web aplikacije: " + result;
                return false;
            }
        }

        public static int UserID () { return (int) UserInfo.First(x => x.Name == "UserID").IntValue; }
        public static string UserName() { return (string) UserInfo.First(x => x.Name == "FullName").StringValue; }
        public static string DeviceUser() { return QRScanner.App.WMSDeviceConfig.GetString("ID", "") + "|" + UserID ().ToString(); }

        private static List<string> obtainedLocks = new List<string>();

        public static void ReleaseObtainedLocks()
        {
            if (obtainedLocks.Count > 0)
            {
                var wf = new WaitForm();
                try
                {
                    wf.Start("Sproščam dostope");

                    string error;
                    obtainedLocks.ForEach(l => WebApp.Get("mode=releaseLock&lockID=" + l, out error));
                    obtainedLocks.Clear();
                }
                finally
                {
                    wf.Stop();
                }
            }
        }

        public static bool TryLock (string lockID, out string error) {
            if (obtainedLocks.FirstOrDefault(x => x == lockID) != null) { error = "OK!"; return true; }

            var wf = new WaitForm();
            try
            {
                wf.Start("Pridobivam dostop");

                var obj = new QRScanner.App.NameValueObject("Lock");
                obj.SetString("LockID", lockID);
                obj.SetString("LockInfo", UserName());
                obj.SetInt("Locker", UserID());
                var serObj = CompactSerializer.Serialize<NameValueObject>(obj);
                if (WebApp.Post("mode=tryLock", serObj, out error))
                {
                    if (error == "OK!")
                    {
                        obtainedLocks.Add(lockID);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                wf.Stop();
            }
        }

        public static bool IsValidUser(string password, out string error)
        {
            string result;
            if (WebApp.Get("mode=loginUser&password=" + password, out result))
            {
                try
                {
                    var nvl = CompactSerializer.Deserialize<NameValueList> (result);
                    if (nvl.Get("Success").BoolValue == true)
                    {
                        nvl.Items.ForEach(nv => UserInfo.Add(nv));
                        error = "";
                        return true;
                    }
                    else
                    {
                        error = nvl.Get("Error").StringValue;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    error = "Napaka pri tolmačenju odziva web strežnika: " + ex.Message;
                    return false;
                }
            }
            else
            {
                error = "Napaka pri klicu web strežnika: " + result;
                return false;
            }
        }

        public static NameValueObjectList GetObjectList(string table, out string error, string pars)
        {
            string result;
            if (WebApp.Get("mode=list&table=" + table + "&pars=" + pars, out result))
            {
                try
                {
                    var startedAt = DateTime.Now;
                    var nvol = CompactSerializer.Deserialize<NameValueObjectList>(result);
                    Log.Write(new LogEntry("END REQUEST: [Device/DeserializeList];" + (DateTime.Now - startedAt).TotalMilliseconds.ToString()));
                    error = "";
                    return nvol;
                }
                catch (Exception ex)
                {
                    error = "Napaka pri tolmačenju odziva web strežnika: " + ex.Message;
                    return null;
                }
            }
            else
            {
                error = "Napaka pri klicu web strežnika: " + result;
                return null;
            }
        }

        public static NameValueObject GetObject(string table, string id, out string error)
        {
            string result;
            if (WebApp.Get("mode=getObj&table=" + table + "&id=" + id, out result))
            {
                try
                {
                    var startedAt = DateTime.Now;
                    var nvo = CompactSerializer.Deserialize<NameValueObject>(result);
                    Log.Write(new LogEntry("END REQUEST: [Device/DeserializeObject];" + (DateTime.Now - startedAt).TotalMilliseconds.ToString()));
                    error = nvo == null ? "Ne obstaja (" + table + "; " + id + ")!" : "";
                    return nvo;
                }
                catch (Exception ex)
                {
                    error = "Napaka pri tolmačenju odziva web strežnika: " + ex.Message;
                    return null;
                }
            }
            else
            {
                error = "Napaka pri klicu web strežnika: " + result;
                return null;
            }
        }

        public static NameValueObject SetObject(string table, NameValueObject data, out string error)
        {
            string result;
            var startedAt = DateTime.Now;
            var serData = CompactSerializer.Serialize <NameValueObject> (data);
            Log.Write(new LogEntry("END REQUEST: [Device/SerializeObject];" + (DateTime.Now - startedAt).TotalMilliseconds.ToString()));
            if (WebApp.Post("mode=setObj&table=" + table, serData, out result))
            {
                try
                {
                    startedAt = DateTime.Now;
                    var nvo = CompactSerializer.Deserialize<NameValueObject>(result);
                    Log.Write(new LogEntry("END REQUEST: [Device/DeserializeObject];" + (DateTime.Now - startedAt).TotalMilliseconds.ToString()));
                    error = nvo == null ? "Zapis objekta ni uspel!" : "";
                    return nvo;
                }
                catch (Exception ex)
                {
                    error = "Napaka pri tolmačenju odziva web strežnika: " + ex.Message;
                    return null;
                }
            }
            else
            {
                error = "Napaka pri klicu web strežnika: " + result;
                return null;
            }
        }

        public static void ReportException(Exception ex)
        {
            var data = ex.ToString();
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                data += " --- " + ex.ToString();
            }

            var guid = Guid.NewGuid().ToString();
            using (var sw = new StreamWriter(Path.Combine(WMSDeviceConfig.ExePath(), "UnhandledExceptions!" + guid + ".txt"), true, Encoding.UTF8))
            {
                sw.WriteLine("Version: " + CommonData.Version);
                sw.WriteLine(data);
            }

            ReportData(data);
        }

        public static object reportLock = new object();
        public static string instanceInfo = Guid.NewGuid().ToString().Split('-')[0];
        public static void ReportData(string data)
        {
            lock (reportLock)
            {
                using (var sw = new StreamWriter(Path.Combine(WMSDeviceConfig.ExePath(), "UnhandledExceptions-" + instanceInfo + ".txt"), true, Encoding.UTF8))
                {
                    sw.WriteLine();
                    sw.WriteLine(DateTime.Now.ToString("s") + " " + instanceInfo + " " + CommonData.Version);
                    sw.WriteLine(data);
                }
            }
        }

        private static bool pdRunning = false;
        private static DateTime lastCall = DateTime.MinValue;
        private static string lastEventName = null;
        public static void PreventDups(string eventName, Action a)
        {
            var block = false;
            if (eventName == lastEventName)
            {
                if (pdRunning || lastCall >= DateTime.UtcNow.AddSeconds(-3))
                {
                    block = true;
                    lastCall = DateTime.UtcNow;
                }
            }

            if (block)
            {
                Log.Write(new LogEntry("PreventDups: prevented duplicate event for " + eventName + " @ " + DateTime.UtcNow.ToString()));
            }
            else
            {
                Log.Write(new LogEntry("PreventDups: event executed " + eventName + " @ " + DateTime.UtcNow.ToString()));
                pdRunning = true;
                try
                {
                    lastEventName = eventName;
                    lastCall = DateTime.UtcNow;
                    a();
                    lastCall = DateTime.UtcNow;
                }
                finally
                {
                    pdRunning = false;
                }
            }
        }
    }
}
