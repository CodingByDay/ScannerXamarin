﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace QRScanner.App
{
    public class Keyboard
    {
        public static bool IsIntKey(char KeyChar)
        {
            return !('0' <= KeyChar && KeyChar <= '9' || KeyChar == (char)Keys.Back || KeyChar == (char)Keys.Delete);
        }
    }
}