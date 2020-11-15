using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlertDialog = Android.App.AlertDialog;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRScanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registration : ContentPage
    {
        public Registration()
        {
            InitializeComponent();
        }

        private async void ProcessRegistration()
        {
            if (string.IsNullOrEmpty(Password.Text.Trim())) { return; }

            Services.Services.ClearUserInfo();

            string error;
            bool valid = false;

            try
            {
             
                valid = Services.Services.IsValidUser(Password.Text.Trim(), out error);
            }
            finally
            {
                
            }

            if (valid)
            {
                if (Services.Services.HasPermission("TNET_WMS", "R"))
                {
                    await DisplayAlert("Success", "Bravo", "Cancel");


                    Password.Text = "";
                    Password.Focus();
                }
                else
                {
                    //MessageForm.Show("Prijava ni uspela! Nimate dovoljena za uporabo aplikacije (TNET_WMS)!");
                    Password.Text = "";
                    Password.Focus();
                }
            }
            else
            {
                // Prijava ni uspela! Napaka: " + error);
                Password.Text = "";
                Password.Focus();
            }
        }


        private void btnRegistration(object sender, EventArgs e)
        {
            ProcessRegistration();
        }
    }
}