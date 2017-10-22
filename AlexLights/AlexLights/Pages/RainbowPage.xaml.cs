using AlexLights.Comms;
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
	public partial class RainbowPage : ContentPage
	{
		public RainbowPage ()
		{
			InitializeComponent ();
            brightnessVal.Value = 160;
            brightnessVal.ValueChanged += OnBrightnessChanged;
        }

        private void OnSmallRainbow(object sender, EventArgs e)
        {
            LightDriver.Instance.SendString("R0a0000");
        }

        private void OnMediumRainbow(object sender, EventArgs e)
        {
            LightDriver.Instance.SendString("R070000");
        }

        private void OnBigRainbow(object sender, EventArgs e)
        {
            LightDriver.Instance.SendString("R020000");
        }

        private void OnBrightnessChanged(object sender, EventArgs e)
        {
            string strBrightness = "B" +
                ((int)brightnessVal.Value).ToString("X2") +
                "0000";
            LightDriver.Instance.SendString(strBrightness);
        }
    }
}