﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRScanner.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaitForm : ContentPage
    {
        public WaitForm()
        {
            InitializeComponent();
        }
    }
}