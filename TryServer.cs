using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerSide
{
    class SockServer
    {
        static void Main(string[] args)
        {
            Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7029);
            listenerSocket.Bind(ipEnd);
            listenerSocket.Listen(int.MaxValue);
            Console.WriteLine($"Waiting for a connection....");
            
            Socket clientSocket = listenerSocket.Accept();
            byte[] buffer = new byte[clientSocket.SendBufferSize];
            Console.WriteLine($"Client Connected");
            
            int readByte;
            do
            {
                readByte = clientSocket.Receive(buffer);
                byte[] recvData = new byte[readByte];
                Array.Copy(buffer, recvData, readByte);
                Console.WriteLine($"We got: " + recvData.ToString());
            } while (readByte > 0);
            
            Console.WriteLine($"Client Disconnected");
            Console.ReadKey();

        }
    }
}