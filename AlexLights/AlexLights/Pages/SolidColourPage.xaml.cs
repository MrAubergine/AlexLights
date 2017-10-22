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
            brightnessVal.Value = 160;
            brightnessVal.ValueChanged += OnBrightnessChanged;
            Color buttonCol;
            double hue = 0;
            double hueStep = 1.0 / 33.0;
            for( int row =0; row<10; row++ )
            {
                for (int col = 0; col < 3; col++)
                {
                    buttonCol = Color.FromHsla(hue, 1, .5);
                    Button buttonColor = new Button { BackgroundColor = buttonCol, BorderRadius = 0 };
                    buttonColor.Clicked += OnColorClicked;
                    colourGrid.Children.Add(buttonColor, col, row);
                    hue += hueStep;
                }
            }

        }

        private void OnColorClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Color rgb = button.BackgroundColor;
            string strColor = "S" + 
                ((int)(rgb.R*255.0)).ToString("X2") +
                ((int)(rgb.G*255.0)).ToString("X2") +
                ((int)(rgb.B*255.0)).ToString("X2");
            LightDriver.Instance.SendString(strColor);
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