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
            string dane;
            StringBuilder readable = new StringBuilder();
            byte[] readbuffer = new byte[Buffer_size];
            byte[] writebuffer = new byte[Buffer_size];
            int message_size;

            while (true)
            {
                try
                {
                    dane = "Wybierz: Logoawanie [1], Rejestracja [2] lub Wyjscie [3]: ";
                    writebuffer = Encoding.ASCII.GetBytes(dane);
                    stream.Write(writebuffer, 0, writebuffer.Length);
                    do
                    {
                        message_size = stream.Read(readbuffer, 0, readbuffer.Length);
                        readable.Append(Encoding.ASCII.GetString(readbuffer, 0, message_size));
                    } while (stream.DataAvailable);
                    if (readable[0] == '1')
                    {
                        dane = "Podaj dane do logowania: ";
                        writebuffer = Encoding.ASCII.GetBytes(dane);
                        stream.Write(writebuffer, 0, writebuffer.Length);
                        readable.Clear();
                        do
                        {
                            message_size = stream.Read(readbuffer, 0, readbuffer.Length);
                            readable.Append(Encoding.ASCII.GetString(readbuffer, 0, message_size));
                        } while (stream.DataAvailable);

                        dane = readable.ToString().Replace('\n', ' ');
                        dane = dane.Replace('\r', ' ');
                        string[] logs = dane.Split(' ');
                        dane = Operations.Login(logs[0], logs[1]);
                        writebuffer = Encoding.ASCII.GetBytes(dane);
                        stream.Write(writebuffer, 0, writebuffer.Length);
                    }
                    else if (readable[0] == '2')
                    {
                        dane = "Podaj dane do rejestracji: ";
                        writebuffer = Encoding.ASCII.GetBytes(dane);
                        stream.Write(writebuffer, 0, writebuffer.Length);
                        readable.Clear();
                        do
                        {
                            message_size = stream.Read(readbuffer, 0, readbuffer.Length);
                            readable.Append(Encoding.ASCII.GetString(readbuffer, 0, message_size));
                        } while (stream.DataAvailable);

                        dane = readable.ToString().Replace('\n', ' ');
                        dane = dane.Replace('\r', ' ');
                        string[] logs = dane.Split(' ');
                        dane = Operations.Register(logs[0], logs[1]);
                        writebuffer = Encoding.ASCII.GetBytes(dane);
                        stream.Write(writebuffer, 0, writebuffer.Length);

                    }
                    else if (readable[0] == '3') 
                    {
                        return;
                    }
                    else
                    {
                        dane = "Wybrano nieprawidlowa opcje\n";
                        writebuffer = Encoding.ASCII.GetBytes(dane);
                        stream.Write(writebuffer, 0, writebuffer.Length);
                    }
                    readable.Clear();
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


