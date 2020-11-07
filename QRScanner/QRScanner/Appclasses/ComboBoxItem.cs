using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace QRScanner.Device.App
{
    public class ComboBoxItem
    {
        public string ID { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public static void Select (ComboBox cb, string id) {
            for (int i = 0; i < cb.Items.Count; i++) {
                if (((ComboBoxItem) cb.Items [i]).ID == id) {
                    if (cb.SelectedIndex != i)
                    {
                        cb.SelectedIndex = i;
                    }
                    return;
                }
            }
            if (cb.SelectedIndex != -1)
            {
                cb.SelectedIndex = -1;
            }
        }
    }
}
