using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Serwer_Echo_Lib
{

    public class ServerLoginAPM : ServerLogin
    {

        public delegate void TransmissionDataDelegate(NetworkStream stream);
        public ServerLoginAPM(IPAddress IP, int port) : base(IP, port)
        {

        }

        protected override void AcceptClient()
        {
            while (true)
            {

                TcpClient tcpClient = TcpListener.AcceptTcpClient();

                Stream = tcpClient.GetStream();

                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                //callback style
                transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, tcpClient);
                // async result style
                //IAsyncResult result = transmissionDelegate.BeginInvoke(Stream, null, null);
                ////operacje......


                //while (!result.IsCompleted) ;
                ////sprzątanie

            }

        }


      

        private void TransmissionCallback(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;
            client.Close();
        }

       

        protected override void BeginDataTransmission(NetworkStream stream)
        {

            while (true)
            {
                try
                {
                    LoginSystem(stream);
                }
                catch (IOException e)
                {
                    break;
                }

            }

        }

        public override void Start()
        {

            StartListening();
            //transmission starts within the accept function
            AcceptClient();

        }



    }

}


