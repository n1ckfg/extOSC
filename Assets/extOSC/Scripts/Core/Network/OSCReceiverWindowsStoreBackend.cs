﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

#if NETFX_CORE

using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace extOSC.Core.Network
{
    internal class OSCReceiverWindowsStoreBackend : OSCReceiverBackend
    {
        #region Public Vars

        public override OSCReceivedCallback ReceivedCallback 
        {
            get { return _receivedCallback; }
            set { _receivedCallback = value; }
        }

        public override bool IsStarted
        {
            get { return _datagramSocket != null; }
        }

        public override bool IsRunning
        {
            get { return _isRunning; }
        }

        #endregion

        #region Private Vars

        private bool _isRunning;

        private DatagramSocket _datagramSocket;

        private HostName _localHost;

        private string _localPort;

        private OSCReceivedCallback _receivedCallback;

        #endregion

        #region Public Methods

        public override void Connect(string localHost, int localPort)
        {
            ConnectAsync(localPort);

            _isRunning = true;
        }

        public override void Close()
        {
            if (_datagramSocket != null)
                _datagramSocket.Dispose();

            _datagramSocket = null;

            _isRunning = false;
        }

        #endregion

        #region Private Methods

        private async void ConnectAsync(int localPort)
        {
            if (_datagramSocket != null)
                Close();

            _localPort = localPort.ToString();
          
			try
			{
                _datagramSocket = new DatagramSocket();
			    _datagramSocket.MessageReceived += Receive;
                _datagramSocket.Control.DontFragment = true;
				await _datagramSocket.BindEndpointAsync(null, _localPort);

                InitMessage();
			}
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogErrorFormat("[OSCReceiver] Invalid port: {0}", localPort);

                Close();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[OSCReceiver] Error: {0}", e);

                Close();
            }
        }

        private async void InitMessage()
        {
            using (var dataWriter = new DataWriter(await _datagramSocket.GetOutputStreamAsync(new HostName("255.255.255.255"), _localPort)))
            {
                try
                {
                    var length = 0;
                    var buffer = OSCConverter.Pack(new OSCMessage("/wsainit"), out length);

                    dataWriter.WriteBuffer(buffer.AsBuffer(0, length));
                    await dataWriter.StoreAsync();
                }
                catch (Exception exception)
                {
                    Debug.LogWarningFormat("[OSCReceiver] Error: {0}", exception);
                }
            }
        }

        private void Receive(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
            try
            {
                using (var dataReader = args.GetDataReader())
                {
                    var data = new byte[dataReader.UnconsumedBufferLength];
                    dataReader.ReadBytes(data);

                    var ioscPacket = OSCConverter.Unpack(data);
                    ioscPacket.Ip = IPAddress.Parse(args.RemoteAddress.ToString());
                    ioscPacket.Port = int.Parse(args.RemotePort);

                    if (_receivedCallback != null)
                        _receivedCallback.Invoke(ioscPacket);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[OSCReceiver] Receive error: " + e);
            }
		}

        #endregion
    }
}

#endif