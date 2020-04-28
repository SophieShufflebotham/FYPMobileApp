using System;
using System.Collections.Generic;
using System.Text;
using FYPMobileApp.Views;
using Xamarin.Forms;

namespace FYPMobileApp.Services
{
    class NavigationService
    {

        public async void navigateToFingerprintPage()
        {

            FingerprintAuthenticationPage fingerprintPage = new FingerprintAuthenticationPage();
            NavigationPage navPage = new NavigationPage(fingerprintPage);

            await Application.Current.MainPage.Navigation.PushModalAsync(navPage);
        }

        public async void navigateToNfcPage()
        {
            //await Application.Current.MainPage.DisplayAlert("Navigator", $"This is where navigation would be, if I had one", "OK");
        }
    }
}
