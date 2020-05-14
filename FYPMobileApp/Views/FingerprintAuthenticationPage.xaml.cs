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

        //Prompt the user to authenticate their fingerprint in response to the button;
        protected async void AuthenticateFingerprint(object sender, System.EventArgs e)
        {
            var availableResult = await Plugin.Fingerprint.CrossFingerprint.Current.GetAvailabilityAsync();

            var request = new AuthenticationRequestConfiguration("Please validate your fingerprint");
            var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(request);

            if (result.Authenticated)
            {
                App.FINGERPRINT_TIMEOUT.Restart(); //If we're authenticated, start the 30 second countdown timer
                Application.Current.Properties["AuthStatus"] = result; //Write the authentication result to a global property to allow the HCE process to see the validation status
                navigator.navigateToNfcPage();
                
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Not authenticated", "OK");
            }
        }

        //Mark the user as safe in response to the button
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