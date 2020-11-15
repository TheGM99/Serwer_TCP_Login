﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Serwer_Echo_Lib
{
    /// <summary>

    /// This is an abstract class for Servers of Echo type.

    /// </summary>

    public abstract class ServerLogin

    {

        #region Fields

        IPAddress iPAddress;
        int port;
        int buffer_size = 1024;
        bool running;
        TcpListener tcpListener;
        TcpClient tcpClient;
        NetworkStream stream;

        #endregion

        #region Properties

        /// <summary>
        /// This property gives access to the IP address of a server instance. Property can't be changed when the Server is running. 
        /// </summary>
        public IPAddress IPAddress { get => iPAddress; set { if (!running) iPAddress = value; else throw new Exception("nie można zmienić adresu IP kiedy serwer jest uruchomiony"); } }

        /// <summary>
        /// This property gives access to the port of a server instance. Property can't be changed when the Server is running. Setting invalid port numbers will cause an exception. 
        /// </summary>

        public int Port
        {
            get => port; set
            {
                int tmp = port;
                if (!running) port = value; else throw new Exception("nie można zmienić portu kiedy serwer jest uruchomiony");
                if (!checkPort())
                {
                    port = tmp;
                    throw new Exception("błędna wartość portu");
                }

            }

        }

        /// <summary>
        /// This property gives access to the buffer size of a server instance. Property can't be changed when the Server is running. Setting invalid size numbers will cause an exception. 
        /// </summary>
        public int Buffer_size
        {
            get => buffer_size; set

            {

                if (value < 0 || value > 1024 * 1024 * 64) throw new Exception("błędny rozmiar pakietu");

                if (!running) buffer_size = value; else throw new Exception("nie można zmienić rozmiaru pakietu kiedy serwer jest uruchomiony");

            }

        }



        protected TcpListener TcpListener { get => tcpListener; set => tcpListener = value; }

        protected TcpClient TcpClient { get => tcpClient; set => tcpClient = value; }

        protected NetworkStream Stream { get => stream; set => stream = value; }

        #endregion

        #region Constructors

        /// <summary>
        /// A default constructor. It doesn't start the server. Invalid port numbers will thrown an exception.
        /// </summary>
        /// <param name="IP">IP address of the server instance.</param>
        /// <param name="port">Port number of the server instance.</param>

        public ServerLogin(IPAddress IP, int port)
        {



            running = false;
            IPAddress = IP;
            Port = port;

            if (!checkPort())

            {

                Port = 8000;
                throw new Exception("błędna wartość portu, ustawiam port na 8000");

            }

        }

        #endregion
        #region Functions

        /// <summary>
        /// This function will return false if Port is set to a value lower than 1024 or higher than 49151.
        /// </summary>
        /// <returns>An information wether the set Port value is valid.</returns>

        protected bool checkPort()
        {

            if (port < 1024 || port > 49151) return false;
            return true;

        }

        /// <summary>
        /// This function starts the listener.
        /// </summary>

        protected void StartListening()
        {

            TcpListener = new TcpListener(IPAddress, Port);
            TcpListener.Start();

        }

        /// <summary>
        /// This function waits for the Client connection.
        /// </summary>

        protected abstract void AcceptClient();
        
        /// <summary>
        /// This function reads infromation sent by a client
        /// </summary>
        /// <param name="stream">Connection with Client</param>
        /// <returns>all signs read from client</returns>
        private StringBuilder ReadFromStream(NetworkStream stream)
        {
            StringBuilder readable = new StringBuilder();
            byte[] readbuffer = new byte[Buffer_size];
            do
            {
                int message_size = stream.Read(readbuffer, 0, readbuffer.Length);
                readable.Append(Encoding.ASCII.GetString(readbuffer, 0, message_size));
            } while (stream.DataAvailable);

            return readable;

        }
        /// <summary>
        /// This function sends data to the client
        /// </summary>
        /// <param name="stream">Conecction with the client</param>
        /// <param name="dane">data sent to the client</param>
        private void WriteToStream(NetworkStream stream, string dane)
        {
            byte[] writebuffer = new byte[Buffer_size];
            writebuffer = Encoding.ASCII.GetBytes(dane);
            stream.Write(writebuffer, 0, writebuffer.Length);
        }

        /// <summary>
        /// Function responsible for communication with the client and letting him register/login .
        /// </summary>
        /// <param name="stream"></param>
        protected void LoginSystem(NetworkStream stream)
        {

            string dane;
            StringBuilder readable = new StringBuilder();
            byte[] readbuffer = new byte[Buffer_size];
            byte[] writebuffer = new byte[Buffer_size];
            int message_size;

            WriteToStream(stream, "Wybierz: Logoawanie [1], Rejestracja [2] lub Wyjscie [3]: ");
            readable = ReadFromStream(stream);
            if (readable[0] == '1')
            {
                WriteToStream(stream, "Podaj dane do logowania: ");
                readable = ReadFromStream(stream);

                dane = readable.ToString().Replace('\n', ' ');
                dane = dane.Replace('\r', ' ');
                string[] logs = dane.Split(' ');

                dane = Operations.Login(logs[0], logs[1]);
                WriteToStream(stream, dane);
            }
            else if (readable[0] == '2')
            {
                WriteToStream(stream, "Podaj dane do rejestracji: ");
                readable = ReadFromStream(stream);

                dane = readable.ToString().Replace('\n', ' ');
                dane = dane.Replace('\r', ' ');
                string[] logs = dane.Split(' ');

                dane = Operations.Register(logs[0], logs[1]);
                WriteToStream(stream, dane);

            }
            else if (readable[0] == '3')
            {
                return;
            }
            else
            {
                WriteToStream(stream, "Wybrano nieprawidlowa opcje\r\n");
            }
        }

        /// <summary>
        /// This function implements Echo and transmits the data between server and client.
        /// </summary>

        protected abstract void BeginDataTransmission(NetworkStream stream);

        /// <summary>
        /// This function fires off the default server behaviour. It interrupts the program.
        /// </summary>

        public abstract void Start();

        #endregion

    }
}
