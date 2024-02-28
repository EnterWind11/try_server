using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class NetworkServer
{
    private static long totalBytesSent = 0;
    private static long totalBytesReceived = 0;

    public static async Task Main(string[] args)
    {
        // Define port number and IP address (you can choose any available port)
        int port = 7029;
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
            // Accept a connection from a client asynchronously
            Socket clientSocket = await listener.AcceptAsync();

            // Start a new task to handle the client connection asynchronously
            _ = HandleClientAsync(clientSocket);
        }

        // Close the listener socket (unreachable in this example)
        listener.Close();

        Console.WriteLine("Server closed");
        Console.WriteLine("Total bytes sent: {0}", totalBytesSent);
        Console.WriteLine("Total bytes received: {0}", totalBytesReceived);
    }

    private static async Task HandleClientAsync(Socket clientSocket)
    {
        try
        {
            // Receive data length (fixed 4 bytes)
            byte[] lengthBuffer = await ReceiveAsync(clientSocket, 4);

            // Convert the received bytes to an integer
            int length = BitConverter.ToInt32(lengthBuffer, 0);

            // Receive the actual data
            byte[] dataBuffer = await ReceiveAsync(clientSocket, length);

            // Display received data in hex
            Console.WriteLine($"Received {dataBuffer.Length} bytes from client, data: {Encoding.UTF8.GetString(dataBuffer)}");

            // Echo the received data back to the client
            await SendDataAsync(clientSocket, dataBuffer);

            // Close the client socket
            clientSocket.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
    }


    private static async Task<byte[]> ReceiveAsync(Socket socket, int size)
    {
        using (var memoryStream = new MemoryStream())
        {
            byte[] buffer = new byte[1024];
            int remainingBytes = size;

            while (remainingBytes > 0)
            {
                int bytesRead = await socket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, Math.Min(buffer.Length, remainingBytes)), SocketFlags.None);

                if (bytesRead == 0)
                {
                    Console.WriteLine("Error: Connection closed before receiving full data.");
                    return new byte[0];
                }

                memoryStream.Write(buffer, 0, bytesRead);
                remainingBytes -= bytesRead;
            }

            return memoryStream.ToArray();
        }
    }

    private static async Task SendDataAsync(Socket clientSocket, byte[] data)
    {
        byte[] lengthBytes = BitConverter.GetBytes(data.Length);
        byte[] combinedBytes = new byte[lengthBytes.Length + data.Length];
        Array.Copy(lengthBytes, 0, combinedBytes, 0, lengthBytes.Length);
        Array.Copy(data, 0, combinedBytes, lengthBytes.Length, data.Length);

        // Print combined data (length + data) in hex
        Console.WriteLine("");
        Console.WriteLine("Sending data (hex):");
        PrintByteArrayToHex(combinedBytes);

        await clientSocket.SendAsync(new ArraySegment<byte>(combinedBytes), SocketFlags.None);
        totalBytesSent += combinedBytes.Length;
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
