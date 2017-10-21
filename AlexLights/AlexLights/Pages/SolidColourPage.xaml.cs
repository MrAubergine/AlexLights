using AlexLights.Comms;
using Android.Bluetooth;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AlexLights.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SolidColourPage : ContentPage
	{
		public SolidColourPage ()
		{
			InitializeComponent ();
		}

        private void OnBoopClicked(object sender, EventArgs e)
        {
            LightDriver.Instance.SendString("");
        }
    }
}