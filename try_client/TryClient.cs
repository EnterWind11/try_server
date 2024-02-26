using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class NetworkClient
{
    public static void Main(string[] args)
    {
        // Define server IP address and port number
        string serverAddress = "127.0.0.1";
        int port = 7029;

        // Create a TCP socket
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Connect to the server
        socket.Connect(IPAddress.Parse(serverAddress), port);

        // Get user input for data to send
        Console.WriteLine("Enter the data you want to send to the server:");
        string data = Console.ReadLine();

        // Send the data to the server (prepend length)
        byte[] lengthBytes = BitConverter.GetBytes(data.Length);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] combinedBytes = new byte[lengthBytes.Length + dataBytes.Length];
        Array.Copy(lengthBytes, 0, combinedBytes, 0, lengthBytes.Length);
        Array.Copy(dataBytes, 0, combinedBytes, lengthBytes.Length, dataBytes.Length);
        socket.Send(combinedBytes);

        // Receive data from the server (optional)
        // ... (add code for receiving data if needed)

        // Close the socket
        socket.Close();
    }
}