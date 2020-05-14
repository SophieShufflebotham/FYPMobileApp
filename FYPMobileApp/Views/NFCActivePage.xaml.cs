using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FYPMobileApp.Services;

namespace FYPMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NFCActivePage : ContentPage
    {
        NavigationService Service { get; set; }
        public NFCActivePage()
        {
            Service = new NavigationService();
            InitializeComponent();
        }

        // Override hardware back button - lock user out of returning before timeout finishes
        // This is the easiest way to avoid navigation issues whilst still allowing us to pop the page afterwards
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}