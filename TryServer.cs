using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace ServerSide
{
    class SockServer
    {
        static void Main(string[] args)
        {
            Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddr = IPAddress.Any;
            IPEndPoint ipEnd = new IPEndPoint(ipaddr, 7029);
            
            try
            {
                listenerSocket.Bind(ipEnd);
                listenerSocket.Listen(int.MaxValue);
                Console.WriteLine($"Waiting for a connection....");

                Socket clientSocket = listenerSocket.Accept();
                Console.WriteLine($"Client Connected. " + clientSocket.ToString() + " - IP End Point: " + clientSocket.RemoteEndPoint.ToString());

                List<byte> receivedData = new List<byte>();
                byte[] buffer = new byte[clientSocket.ReceiveBufferSize];

                int readBytes;
                do
                {
                    readBytes = clientSocket.Receive(buffer);
                    Console.WriteLine($"Number of received bytes: {readBytes}");
                    receivedData.AddRange(buffer.Take(readBytes));
                } while (readBytes > 0);

                Console.WriteLine($"Received Data: " + Encoding.UTF8.GetString(receivedData.ToArray()));
                Console.WriteLine($"Client Disconnected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                listenerSocket.Close();
                Console.ReadKey();
            }
        }
    }
}