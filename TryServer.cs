using System;
using System.Net;
using System.Net.Sockets;

public class GameServer
{
    private const int DEFAULT_BUFFER_LENGTH = 1024;

    public static void Main(string[] args)
    {
        // Initialize Winsock
        Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        try
        {
            // Bind the listener socket to the local endpoint
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 7029;
            listenerSocket.Bind(new IPEndPoint(ipAddress, port));

            // Start listening for incoming connections
            listenerSocket.Listen(10);
            Console.WriteLine("Server is listening for incoming connections...");

            // Accept incoming connection
            Socket clientSocket = listenerSocket.Accept();
            Console.WriteLine("Client connected");

            // Process handshake packets until the client disconnects
            while (clientSocket.Connected)
            {
                byte[] receiveBuffer = new byte[DEFAULT_BUFFER_LENGTH];
                int receivedBytes = clientSocket.Receive(receiveBuffer);
                
                if (receivedBytes > 0)
                {
                    // Display the received handshake packet
                    Console.WriteLine($"Received handshake packet: {receivedBytes} bytes");
                    Console.WriteLine(BitConverter.ToString(receiveBuffer, 0, receivedBytes));
                    
                    // Send a response (echo the packet back)
                    clientSocket.Send(receiveBuffer, receivedBytes, SocketFlags.None);
                }
            }

            Console.WriteLine("Client disconnected");
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
            listenerSocket.Close();
        }
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
