using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QRScanner.Views;

namespace QRScanner
{
    public partial class Appclases : Application
    {
        public Appclases()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Registration());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
