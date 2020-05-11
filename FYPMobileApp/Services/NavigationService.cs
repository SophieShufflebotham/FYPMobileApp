using System;
using System.Collections.Generic;
using System.Text;
using FYPMobileApp.Views;
using Xamarin.Forms;

namespace FYPMobileApp.Services
{
    public class NavigationService
    {

        public async void navigateToFingerprintPage()
        {

            FingerprintAuthenticationPage fingerprintPage = new FingerprintAuthenticationPage();
            NavigationPage navPage = new NavigationPage(fingerprintPage);

            await Application.Current.MainPage.Navigation.PushModalAsync(navPage);
        }

        public async void navigateToNfcPage()
        {

            NFCActivePage nfcPage = new NFCActivePage();
            NavigationPage navPage = new NavigationPage(nfcPage);

            await Application.Current.MainPage.Navigation.PushModalAsync(navPage);
        }

        public async void returnToPrevious()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
