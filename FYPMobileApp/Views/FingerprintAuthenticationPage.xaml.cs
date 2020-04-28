using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Fingerprint.Abstractions;
using FYPMobileApp.Services;
using FYPMobileApp.Models;
using FYPMobileApp.Responses;

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

        protected async void AuthenticateFingerprint(object sender, System.EventArgs e)
        {
            var availableResult = await Plugin.Fingerprint.CrossFingerprint.Current.GetAvailabilityAsync();

            var request = new AuthenticationRequestConfiguration("Prove you have fingers!");
            var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(request);

            if (result.Authenticated)
            {
                App.FINGERPRINT_TIMEOUT.Restart();
                Application.Current.Properties["AuthStatus"] = result;
                navigator.navigateToNfcPage();
                
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Not authenticated", "OK");
            }
        }

        protected void MarkAsSafe(object sender, System.EventArgs e)
        {
            RestService service = new RestService();
            string userId = Application.Current.Properties["UserId"] as string;
            Response param = new Response();
            param.userId = userId;
            service.PostSafetyStatus(param);
        }
    }
}