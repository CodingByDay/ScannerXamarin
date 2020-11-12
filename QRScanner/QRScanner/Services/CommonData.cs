using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using QRScanner.Appclasses.Core.Data;
using QRScanner.Components;

namespace QRScanner.Services
{
    public class CommonData
    {
        public static string Version = "1.0.71l";
        /// <summary>
        ///  Import statements are called like this...
        /// </summary>
        private static NameValueObjectList warehouses = null;
        private static NameValueObjectList shifts = null;
        private static NameValueObjectList subjects = null;
        private static Dictionary<string, bool> locations = new Dictionary<string, bool>();
        private static Dictionary<string, NameValueObjectList> docTypes = new Dictionary<string, NameValueObjectList>();
        private static Dictionary<string, NameValueObject> idents = new Dictionary<string, NameValueObject>();
        private static Dictionary<string, string> settings = new Dictionary<string, string>();

        private static string qtyPicture = null;
        public static string GetQtyPicture () {
            if (qtyPicture == null) {
                var digStr = GetSetting ("QtyDigits");
                if (string.IsNullOrEmpty(digStr)) { digStr = "2"; }
                var digits = Convert.ToInt32 (digStr);
                qtyPicture = "###,###,##0.";
                for (int i = 1; i <= digits; i++) { qtyPicture += "0"; }
            }
            return qtyPicture;
        }

        public static string GetSetting(string name)
        {
            if (settings.ContainsKey(name))
            {
                return settings[name];
            }

            var wf = new WaitForm();
   //         wf.Start("Preverjam nastavitve.");
            try
            {
                string error;
                var value = Services.GetObject("sg", name, out error);
                if (value == null)
                {
                    throw new ApplicationException("Napaka pri pridobivanju vrednosti nastavitve: " + name + " / " + error);
                }
                else
                {
                    var val = value.GetString("Value");
                    settings.Add(name, val);
                    return val == null ? "" : val;
                }
            }
            finally
            {
  //              wf.Stop();
            }
        }

        public static string GetNextSSCC()
        {
            var wf = new WaitForm();
  //          wf.Start("Pridobivam SSCC kodo.");
            try
            {
                string error;
                var value = Services.GetObject("ns", "", out error);
                if (value == null)
                {
                    throw new ApplicationException("Napaka pri pridobivanju SSCC kode: " + error);
                }
                else
                {
                    var sscc = value.GetString("SSCC");
                    if (string.IsNullOrEmpty(sscc))
                    {
                        throw new ApplicationException("Napaka pri pridobivanju SSCC kode: SSCC koda je prazna");

                    }
                    return sscc;
                }
            }
            finally
            {
   //             wf.Stop();
            }
        }

   //     public static NameValueObject GetWarehouse(string warehouse)
   //     {
   //         var wh = ListWarehouses()
   //             .Items
   //             .FirstOrDefault(x => x.GetString ("Subject") == warehouse);
   //         if (wh == null)
   //         {
   //             Program.Exit(() =>
   //             {
   ////                 MessageBox.Show("Kritična napaka: Skladišča '" + (warehouse ?? "null").Trim () + "' ni mogoče najti, oz. za dostop do tega skladišča nimate nastavljenih pravic v PA!");
   //             });
   //         }
   //         return wh;
   //     }

        public static NameValueObjectList ListWarehouses()
        {
            if (warehouses == null)
            {
                var wf = new WaitForm();
                try
                {
  //                  wf.Start("Nalagam seznam skladišč iz strežnika...");
                    string error;
                    warehouses = Services.GetObjectList("wh", out error, Services.UserID().ToString());
                    if (warehouses == null)
                    {
             //           Program.Exit(() => { MessageForm.Show("Napaka pri dostopu do web aplikacije: " + error); });
                        return null;
                    }
                }
                finally
                {
  //                  wf.Stop();
                }
            }
            return warehouses;
        }

        public static NameValueObjectList ListShifts()
        {
            if (shifts == null)
            {
                var wf = new WaitForm();
                try
                {
 //                   wf.Start("Nalagam seznam izmen iz strežnika...");
                    string error;
                    shifts = Services.GetObjectList("sh", out error, "");
                    if (shifts == null)
                    {
          //              Program.Exit(() => { MessageForm.Show("Napaka pri dostopu do web aplikacije: " + error); });
                        return null;
                    }
                }
                finally
                {
  //                  wf.Stop();
                }
            }
            return shifts;
        }

        public static NameValueObjectList ListSubjects()
        {
            var wf = new WaitForm();
            try
            {
    //            wf.Start("Nalagam subjekte...");
                string error;
                subjects = Services.GetObjectList("su", out error, "");
                if (subjects == null)
                {
      //              Program.Exit(() => { MessageForm.Show("Napaka pri dostopu do web aplikacije: " + error); });
                    return null;
                }
            }
            finally
            {
//                wf.Stop();
            }
            return subjects;
        }

        public static NameValueObjectList ListReprintSubjects()
        {
            var wf = new WaitForm();
            try
            {
    //            wf.Start("Nalagam subjekte...");
                string error;
                subjects = Services.GetObjectList("surl", out error, "");
                if (subjects == null)
                {
   //                 Program.Exit(() => { MessageForm.Show("Napaka pri dostopu do web aplikacije: " + error); });
                    return null;
                }
            }
            finally
            {
 //               wf.Stop();
            }
            return subjects;
        }

        public static bool IsValidLocation(string warehouse, string location)
        {
            var key = warehouse + "|" + location;
            if (locations.ContainsKey(key))
            {
                return locations [key];
            }

            var wf = new WaitForm();
            try
            {
       //         wf.Start("Preverjam lokacijo...");

                string error;
                var loc = Services.GetObject("lo", key, out error);
                if (loc != null) { locations.Add(key, true); }
                return loc != null;
            }
            finally
            {
   //             wf.Stop();
            }
        }

        public static NameValueObjectList ListDocTypes(string pars)
        {
            if (docTypes.ContainsKey(pars))
            {
                return docTypes[pars];
            }

            var wf = new WaitForm();
            try
            {
              //  wf.Start("Nalagam seznam tipov dokumentov...");

                string error;
                var dts = Services.GetObjectList("dt", out error, pars);
                if (dts == null)
                {
//                    Program.Exit(() => { MessageForm.Show("Napaka pri dostopu do web aplikacije: " + error); });
                    return null;
                }

                docTypes.Add(pars, dts);
                return dts;
            }
            finally
            {
              //  wf.Stop();
            }
        }

        public static NameValueObject LoadIdent(string ident)
        {
            if (idents.ContainsKey(ident))
            {
                return idents[ident];
            }

            var wf = new WaitForm();
            try
            {
              //  wf.Start("Preverjam ident...");

                string error;
                var openIdent = Services.GetObject("id", ident, out error);
                if (openIdent == null)
                {
                //    MessageForm.Show("Napaka pri preverjanju ident-a: " + error);
                    return null;
                }
                else
                {
                    var code = openIdent.GetString("Code");
                    var secCode = openIdent.GetString("SecondaryCode");
                    if (!string.IsNullOrEmpty(code) && !idents.ContainsKey(code))
                    {
                        idents.Add(code, openIdent);
                    }
                    if (!string.IsNullOrEmpty(secCode) && !idents.ContainsKey(secCode))
                    {
                        idents.Add(secCode, openIdent);
                    }
                    return openIdent;
                }
            }
            finally
            {
             //   wf.Stop();
            }
        }
    }
}
