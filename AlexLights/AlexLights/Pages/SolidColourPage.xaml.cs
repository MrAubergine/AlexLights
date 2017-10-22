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

        private void OnRedClicked(object sender, EventArgs e)
        {
            LightDriver.Instance.SendString("SC880000");
        }
        private void OnGreenClicked(object sender, EventArgs e)
        {
            LightDriver.Instance.SendString("SC008800");
        }
        private void OnBlueClicked(object sender, EventArgs e)
        {
            LightDriver.Instance.SendString("SC000088");
        }
        private void OnWhiteClicked(object sender, EventArgs e)
        {
            LightDriver.Instance.SendString("SC888888");
        }
    }
}