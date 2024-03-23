using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class GameServer
{
    private const int DEFAULT_BUFFER_LENGTH = 1024;

    public static async Task Main(string[] args)
    {
        // Initialize Winsock
        IPHostEntry ipEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());
        IPAddress ipAddress = IPAddress.Any;
        IPEndPoint ipEndPoint = new(ipAddress, 7029);

        Socket listenerSocket = null;
        try
        {
            // Bind the listener socket to the local endpoint
            listenerSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(ipEndPoint);
            listenerSocket.Listen(10);
            Console.WriteLine($"Server is listening for incoming connections from port: {ipEndPoint.Port}");

            // Accept incoming connection
            using Socket clientSocket = await listenerSocket.AcceptAsync();
            Console.WriteLine("Client connected");

            // Create a memory stream to store packets
            MemoryStream memoryStream = new MemoryStream();

            // Simulate packets being received and stored in the memory stream
            for (int i = 0; i < 10; i++)
            {
                byte[] packet = GeneratePacket(i); // Generate a packet
                memoryStream.Write(packet, 0, packet.Length); // Write the packet to the memory stream
            }

            // Reset the memory stream position to the beginning
            memoryStream.Position = 0;

            // Now both server and client can send packets
            while (true)
            {
                byte[] sendBuffer = new byte[DEFAULT_BUFFER_LENGTH];
                int bytesRead = memoryStream.Read(sendBuffer, 0, DEFAULT_BUFFER_LENGTH);

                if (bytesRead > 0)
                {
                    // Send the packet to the client
                    await clientSocket.SendAsync(new ArraySegment<byte>(sendBuffer, 0, bytesRead), SocketFlags.None);
                }
                else
                {
                    // End of data in memory stream, close the connection
                    Console.WriteLine("End of data reached. Closing connection...");
                    clientSocket.Close();
                    //break;
                }
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Socket exception: {ex.Message}");

            if (ex.SocketErrorCode > 0)
            {
                Console.WriteLine($"Error code: {(int)ex.SocketErrorCode} ({GetSocketErrorDescription(ex.SocketErrorCode)})");
            }
        }
        finally
        {
            // Ensure the listener socket is closed regardless of exceptions
            listenerSocket?.Close();
        }
    }

    private static byte[] GeneratePacket(int index)
    {
        // Simulate generating a packet based on some index
        return BitConverter.GetBytes(index);
    }

    private static string GetSocketErrorDescription(SocketError errorCode)
    {
        switch (errorCode)
        {
            case SocketError.AccessDenied:
                return "Access denied";
            case SocketError.AddressAlreadyInUse:
                return "Address already in use";
            case SocketError.ConnectionAborted:
                return "Connection aborted";
            default:
                return "Unknown error";
        }
    }
}
