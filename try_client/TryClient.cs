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

        try
        {
            // Connect to the server
            socket.Connect(IPAddress.Parse(serverAddress), port);
            Console.WriteLine("Connected to server successfully.");

            // Get user input for data to send
            Console.Write("Enter the data you want to send to the server: ");
            string data = Console.ReadLine();

            // Send the data to the server (prepend length)
            byte[] lengthBytes = BitConverter.GetBytes(data.Length);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] combinedBytes = new byte[lengthBytes.Length + dataBytes.Length];
            Array.Copy(lengthBytes, 0, combinedBytes, 0, lengthBytes.Length);
            Array.Copy(dataBytes, 0, combinedBytes, lengthBytes.Length, dataBytes.Length);

            // Print combined data (length + data) in hex
            Console.WriteLine("");
            Console.WriteLine("Sending data (hex):");
            PrintByteArrayToHex(combinedBytes);
            
            Console.WriteLine("");
            socket.Send(combinedBytes);

            // Receive data from the server
            List<byte> receivedData = new List<byte>();
            byte[] buffer = new byte[1024]; // Adjust buffer size based on expected data size
            int receivedBytes;

            do
            {
                receivedBytes = socket.Receive(buffer);
                receivedData.AddRange(buffer.Take(receivedBytes));
            } while (receivedBytes > 0);

            // Extract data length from the received data (assuming server sends length first)
            int dataLength = BitConverter.ToInt32(receivedData.ToArray(), 0);

            // Extract and display the actual data
            string receivedString = Encoding.UTF8.GetString(receivedData.Skip(4).ToArray());

            //Console.WriteLine("** Server Response **");
            Console.WriteLine(receivedString); // Display received data
            Console.WriteLine("Data Length: {0} bytes", dataLength); // Display data length

            // Print received data (hex)
            Console.WriteLine("");
            Console.WriteLine("Received data (hex):");
            PrintByteArrayToHex(receivedData.ToArray());

            // Close the socket
            socket.Close();
            Console.WriteLine("Connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
        }
    }

    private static void PrintByteArrayToHex(byte[] data)
    {
        foreach (byte b in data)
        {
            Console.Write("{0:X2} ", b); // Format byte as uppercase hex with 2 digits
        }
        Console.WriteLine();
    }
}
