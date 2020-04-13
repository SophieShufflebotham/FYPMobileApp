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

            LoginResponse response = await rest.PostRequest<LoginResponse>(loginParams);

            if(response.UserId > 0)
            {
                navigator.navigateToFingerprintPage();
            }
        }
    }
}
