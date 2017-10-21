using Android.Bluetooth;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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

        private Thread rdThread = null;
        private object rdLock = new Object();
        private int rdBufferCount = 0;
        private static int rdBufferMax = 32;
        private byte[] rdBuffer = new byte[rdBufferMax];

        System.Threading.Timer autoCloseTimer = null;

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

        private LightDriver()
        {
            // Get the adapter, device, and SPP UUID
            btAdapter = BluetoothAdapter.DefaultAdapter;
            if (btAdapter == null)
                return;
 
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
                return;

            sppUUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

            autoCloseTimer = new System.Threading.Timer(new TimerCallback(AutoDisconnect),this,-1,-1);
        }

        private bool Connect()
        {
            // Early out if we already have a socket and a reader thread
            if (btSocket != null && rdThread != null && rdThread.IsAlive)
                return true;

            // Tidy up if necessary
            if (btSocket != null)
            {
                btSocket.Close();
                btSocket = null;
            }
            if(rdThread != null)
            {
                if (rdThread.IsAlive)
                    rdThread.Abort();
                rdThread = null;
            }

            // If we don't have an SPP UID then initialisation failed
            if (sppUUID == null)
                return false;

            // Create the socket and try to connect
            btSocket = btDevice.CreateInsecureRfcommSocketToServiceRecord(sppUUID);
            if (btSocket == null)
                return false;

            try
            {
                btSocket.Connect();
            }
            catch (Exception)
            {
                btSocket = null;
                return false;
            }

            // Start the receive thread
            rdThread = new Thread(SocketReaderWorker);
            rdThread.Start();
            while (!rdThread.IsAlive) ;
            
            return true;
        }

        private void SocketReaderWorker()
        {
            rdBufferCount = 0;
            byte[] buffer = new byte[rdBufferMax];
            while (true && btSocket != null)
            {
                try
                {
                    int read = btSocket.InputStream.Read(buffer, 0, rdBufferMax - rdBufferCount);
                    if(read==0)
                    {
                        break;
                    }
                    else
                    {
                        lock (rdLock)
                        {
                            Array.Copy(buffer, 0, rdBuffer, rdBufferCount, read);
                            rdBufferCount += read;
                        }
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private int AvailableInput()
        {
            return rdBufferCount;
        }

        private string InputString()
        {
            string retstr;

            lock(rdLock)
            {
                retstr = Encoding.ASCII.GetString(rdBuffer, 0, rdBufferCount);
                rdBufferCount = 0;
            }

            return retstr;
        }

        private bool WriteString(string str)
        {
            if (btSocket == null)
                return false;

            btSocket.OutputStream.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
            return true;
        }

        private void AutoDisconnect(object state)
        {
            ((LightDriver)state).Disconnect();
        }

        private void Disconnect()
        {
            if (btSocket != null)
            {
                btSocket.Close();
                btSocket = null;
            }
        }

        public bool SendString(string str)
        {
            if (!Connect())
                return false;

            autoCloseTimer.Change(10000, -1);

            for(int i=0; i<32; i++)
            {
                WriteString("*");
                if (AvailableInput() != 0)
                    break;
            }

            if( InputString()=="R")
            {
                WriteString(str);
            }

            return true;
        }
    }
}
