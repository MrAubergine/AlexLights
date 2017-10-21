using Android.Bluetooth;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlexLights.Comms
{
    class LightDriver
    {
        private static volatile LightDriver instance;
        private static object syncRoot = new Object();

        private BluetoothAdapter btAdapter = null;
        private BluetoothDevice btDevice = null;
        private UUID sppUUID = null;
        private BluetoothSocket btSocket = null;
        private bool btConnected = false;

        private bool connected;

        public static LightDriver Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new LightDriver();
                    }
                }

                return instance;
            }
        }

        private LightDriver() { }

        private bool Connect()
        {
            if (btAdapter == null)
            {
                btAdapter = BluetoothAdapter.DefaultAdapter;
                if (btAdapter == null)
                    return false;
            }

            if (btDevice == null)
            {
                var devices = btAdapter.BondedDevices;
                foreach (BluetoothDevice device in devices)
                {
                    if (device.Name == "Adafruit EZ-Link a131")
                    {
                        btDevice = device;
                        break;
                    }
                }
                if (btDevice == null)
                    return false;
            }

            if (sppUUID == null)
            {
                sppUUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
            }

            if (btSocket == null)
            {
                btSocket = btDevice.CreateInsecureRfcommSocketToServiceRecord(sppUUID);
            }
            if (btSocket == null)
                return false;

            try
            {
                btSocket.Connect();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void Close()
        {
            if (btSocket != null)
            {
                btSocket.Close();
            }
        }

        public bool SendString(string str)
        {
            if (!Connect())
                return false;

            btSocket.OutputStream.WriteByte(42);
            Close();

            return true;
        }
    }
}
