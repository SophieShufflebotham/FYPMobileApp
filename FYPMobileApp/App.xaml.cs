using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Fingerprint.Abstractions;
using FYPMobileApp.Services;

namespace FYPMobileApp
{
    public partial class App : Application
    {
        public static Stopwatch FINGERPRINT_TIMEOUT = new Stopwatch();
        private static int defaultTimespan = 30;
        public static bool AlarmTriggered = false;
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            if (!FINGERPRINT_TIMEOUT.IsRunning)
            {
                FINGERPRINT_TIMEOUT.Start();
            }
            //Start a timer that 'ticks' every second
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                if (FINGERPRINT_TIMEOUT.IsRunning && FINGERPRINT_TIMEOUT.Elapsed.Seconds >= defaultTimespan)
                {
                    if (Application.Current.Properties.ContainsKey("AuthStatus"))
                    {
                        FingerprintAuthenticationResult result = Application.Current.Properties["AuthStatus"] as FingerprintAuthenticationResult;

                        if (result.Authenticated) //The fingerprint can't timeout if it was never authenticated in the first place
                        {
                            bool removalStatus = Application.Current.Properties.Remove("AuthStatus");
                            AlarmTriggered = true;
                            NavigationService service = new NavigationService();

                            //Due to hardware 'back' lockout on NFC page, user can't be anywhere else, so it's safe to use returnToPrevious and pop from the modal stack
                            Device.BeginInvokeOnMainThread(async () => {
                                service.returnToPrevious();
                                await Application.Current.MainPage.DisplayAlert("Timeout", "Fingerprint timed out, please re-authenticate", "OK");

                            });
                        }
                    }
                    FINGERPRINT_TIMEOUT.Restart();
                }
                // Always return true as to keep our device timer running.
                return true;
            });
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
