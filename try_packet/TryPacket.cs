using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class GamePacket
{
    private const int DEFAULT_BUFFER_LENGTH = 1024;

    public static void Main(string[] args)
    {
        // Handshake initiation request packets
        byte[] handshakePacket1 = {
            0x11, 0x00, 0xff, 0xff, 0x92, 0xdf, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x2d, 0x1a
        };

        byte[] handshakePacket2 = {
            0x1a, 0x00, 0xff, 0xff, 0x00, 0x50, 0x56, 0xc0, 0x00, 0x01,
            0x00, 0x00, 0x7f, 0x4b, 0x4e, 0x06, 0x25, 0xc7, 0x8e, 0x93, 
            0x4e, 0x06, 0x95, 0xd8, 0x26, 0x1a
        };

        // Initialize Winsock
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        try
        {
            
            // Connect to the server
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 7029;
            socket.Connect(new IPEndPoint(ipAddress, port));

            // Send handshake packet 1
            int sentBytes = socket.Send(handshakePacket1);
            Console.WriteLine($"Sent handshake packet 1: {sentBytes} bytes");

            // Receive server response
            byte[] receiveBuffer = new byte[DEFAULT_BUFFER_LENGTH];
            int receivedBytes = socket.Receive(receiveBuffer);
            Console.WriteLine($"Received server response: {receivedBytes} bytes");

            // Send handshake packet 2
            sentBytes = socket.Send(handshakePacket2);
            Console.WriteLine($"Sent handshake packet 2: {sentBytes} bytes");

            // Receive server response
            receivedBytes = socket.Receive(receiveBuffer);
            Console.WriteLine($"Received server response: {receivedBytes} bytes");

            // Receive data from the server
            do
            {
                receivedBytes = socket.Receive(receiveBuffer);
                if (receivedBytes > 0)
                {
                    // Process received data (implement logic to handle the data)
                    Console.WriteLine($"Received data: {receivedBytes} bytes");
                }
                else if (receivedBytes == 0)
                {
                    Console.WriteLine("Connection closed");
                }
            } while (receivedBytes > 0);
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
            // Ensure socket is closed regardless of exceptions
            socket.Close();
        }
    }

    private static string GetSocketErrorDescription(SocketError errorCode)
    {
        // Map error codes to descriptions based on your chosen resource
        // (e.g., https://learn.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2)
        switch (errorCode)
        {
            case SocketError.AccessDenied:
                return "Access denied";
            case SocketError.AddressAlreadyInUse:
                return "Address already in use";
            case SocketError.ConnectionAborted:
                return "Connection aborted";
            // ... (Add more mappings for relevant error codes)
            default:
                return "Unknown error";
        }
    }
}
