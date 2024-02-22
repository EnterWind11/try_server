/*
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

                // Sending the received data back
                clientSocket.Send(receivedData.ToArray());
                Console.WriteLine($"Number of send bytes: {receivedData.Count}");

                //Console.WriteLine($"Received Data: " + Encoding.UTF8.GetString(receivedData.ToArray()));
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
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class NetworkServer
{
    private static long totalBytesSent = 0;
    private static long totalBytesReceived = 0;

    public static void Main(string[] args)
    {
        // Define port number and IP address (you can choose any available port)
        int port = 8080;
        IPAddress address = IPAddress.Any;

        // Create a TCP socket
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the specified port and address
        listener.Bind(new IPEndPoint(address, port));

        // Start listening for incoming connections
        listener.Listen(5);

        Console.WriteLine("Server started, listening on port {0}", port);

        while (true)
        {
            // Accept a connection from a client
            Socket clientSocket = listener.Accept();

            // Start a new thread to handle the client connection
            Thread clientThread = new Thread(() => HandleClient(clientSocket));
            clientThread.Start();
        }

        // Close the listener socket (unreachable in this example)
        listener.Close();

        Console.WriteLine("Server closed");
        Console.WriteLine("Total bytes sent: {0}", totalBytesSent);
        Console.WriteLine("Total bytes received: {0}", totalBytesReceived);
    }

    private static void HandleClient(Socket clientSocket)
    {
        try
        {
            // Receive data from the client (length + data)
            byte[] buffer = new byte[4]; // 4 bytes for length
            clientSocket.Receive(buffer);
            int length = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[length];
            clientSocket.Receive(buffer);
            string receivedData = Encoding.UTF8.GetString(buffer);

            // Calculate and display received bytes for this connection
            long connectionBytesReceived = length + 4; // 4 for length header
            Console.WriteLine("Received {0} bytes from client, data length: {1}", connectionBytesReceived, length);

            // Update total statistics
            totalBytesReceived += connectionBytesReceived;

            // Send a response to the client
            string responseData = "Server received your data: " + receivedData;
            SendData(clientSocket, responseData);

            // Close the client socket
            clientSocket.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error handling client: {0}", ex.Message);
        }
    }

    private static void SendData(Socket clientSocket, string data)
    {
        byte[] lengthBytes = BitConverter.GetBytes(data.Length);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] combinedBytes = new byte[lengthBytes.Length + dataBytes.Length];
        Array.Copy(lengthBytes, 0, combinedBytes, 0, lengthBytes.Length);
        Array.Copy(dataBytes, 0, combinedBytes, lengthBytes.Length, dataBytes.Length);
        clientSocket.Send(combinedBytes);
        totalBytesSent += combinedBytes.Length;
    }
}
