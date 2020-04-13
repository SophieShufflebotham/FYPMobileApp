using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Fingerprint.Abstractions;
using FYPMobileApp.Services;

namespace FYPMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FingerprintAuthenticationPage : ContentPage
    {
        NavigationService navigator = new NavigationService();
        public FingerprintAuthenticationPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            var availableResult = await Plugin.Fingerprint.CrossFingerprint.Current.GetAvailabilityAsync();

            var request = new AuthenticationRequestConfiguration("Prove you have fingers!");
            var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(request);


            if (result.Authenticated)
            {
                // do secret stuff :)
                //await Application.Current.MainPage.DisplayAlert("Error", $"Verified", "OK");
                navigator.navigateToNfcPage();
                
            }
            else
            {
                // not allowed to do secret stuff :(
                await Application.Current.MainPage.DisplayAlert("Error", $"Not authenticated", "OK");
            }
        }
    }
}