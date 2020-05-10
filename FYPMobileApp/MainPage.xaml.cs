using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using FYPMobileApp.Models;
using FYPMobileApp.Responses;
using FYPMobileApp.Services;

namespace FYPMobileApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        NavigationService navigator = new NavigationService();
        public MainPage()
        {                 
            InitializeComponent();
        }

        async void loginRequest(object sender, System.EventArgs e)
        {
            RestService rest = new RestService();
            UserLogin loginParams = new UserLogin();

            loginParams.username = inputUsername.Text;
            loginParams.password = inputPassword.Text;

            if (inputUsername.Text.Length == 0 || inputPassword.Text.Length == 0)
            {
                await DisplayAlert("Error", "Please ensure username and password are entered", "OK");
            }
            else
            {
                LoginResponse response = await rest.PostLoginRequest<LoginResponse>(loginParams);

                if (response.UserId > 0)
                {
                    Application.Current.Properties["UserId"] = response.UserId.ToString();
                    navigator.navigateToFingerprintPage();
                }
                else
                {
                    await DisplayAlert("Error", "Invalid username or password", "OK");
                    inputUsername.Text = "";
                    inputPassword.Text = "";
                }
            }
        }

        protected override void OnAppearing()
        {
            inputUsername.Text = "";
            inputPassword.Text = "";
            base.OnAppearing();
        }
    }
}
