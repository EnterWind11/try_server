using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
                
            //this is not the correct way to do it but it should work for now    
            byte[] firstPacket = new byte[]
            {
                (byte)0x11, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0x08, (byte)0xC2,
                (byte)0x02, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00,
                (byte)0x00, (byte)0x00, (byte)0x01, (byte)0x2D, (byte)0x1A
            };

             //SocketAsyncEventArgs asyncEventArgs = new SocketAsyncEventArgs()

            // Now both server and client can send packets
            while (true)
            {
                // byte[] sendBuffer = new byte[DEFAULT_BUFFER_LENGTH];
                // int bytesRead = memoryStream.Read(sendBuffer, 0, DEFAULT_BUFFER_LENGTH);
                //
                // if (bytesRead > 0)
                // {
                //     // Send the packet to the client
                //     await clientSocket.SendAsync(new ArraySegment<byte>(sendBuffer, 0, bytesRead), SocketFlags.None);
                // }
                // else
                // {
                //     // End of data in memory stream, close the connection
                //     Console.WriteLine("End of data reached. Closing connection...");
                //     clientSocket.Close();
                //     //break;
                // }
                await clientSocket.SendAsync(new ArraySegment<byte>(firstPacket, 0, firstPacket.Length), SocketFlags.None);

                byte[] receiveBuffer = new byte[1024];
                var receivedLength = await clientSocket.ReceiveAsync(receiveBuffer, SocketFlags.None);

                var test = Encoding.UTF8.GetString(receiveBuffer,0, receivedLength);
                
                
                var hexString = BitConverter.ToString(receiveBuffer,0, receivedLength);
                
                
                Console.WriteLine($"received {receivedLength} bytes data: {hexString} as string {test}");
                
                //we need logic now: 
                //if client connects: send the first packet (firstPacket), then receive data we have that above
                
                //get the op code from the client response packet (extract n bytes from receiveBuffer)
                
                //switch (opCode)
                //case 1A-00-FF-FF-00-50-56-C0-00-01-...
                //send ..
                
                //case loginData:
                // send : 0b00e50c0000000001371a
                
                //you have to decode the packet's bytes into their meaning: 
                // e.g. figure out this packet: it is sent as a response to the client sending its login data
                //

                //Decode these 4 packets
                //firstPacket S2C
                
                /*
                 * 0000   7c c2 c6 1b 45 86 5c e7 47 9f 9d 80 08 00 45 b8   |...E.\.G.....E.
0010   00 39 fd 4a 40 00 2d 06 69 ab 33 d2 df 92 c0 a8   .9.J@.-.i.3.....
0020   12 04 1b 75 22 cb 91 09 19 26 d4 24 4f 1a 50 18   ...u"....&.$O.P.
0030   01 f6 85 16 00 00 11 00 ff ff 08 c2 02 00 00 00   ................
0040   00 00 00 00 01 2d 1a                              .....-.

                 */
                //firstPacket from Client to server C2S
                0000   5c e7 47 9f 9d 80 7c c2 c6 1b 45 86 08 00 45 00   \.G...|...E...E.
                0010   00 42 18 93 40 00 80 06 fc 11 c0 a8 12 04 33 d2   .B..@.........3.
                0020   df 92 22 cb 1b 75 d4 24 4f 1a 91 09 19 37 50 18   .."..u.$O....7P.
                0030   01 03 e6 e0 00 00 1a 00 ff ff 00 50 56 c0 00 01   ...........PV...
                0040   00 00 00 b5 75 01 bf da 0b fd 75 01 88 42 26 1a   ....u.....u..B&.

                //second packet from S2C
                0000   7c c2 c6 1b 45 86 5c e7 47 9f 9d 80 08 00 45 b8   |...E.\.G.....E.
                0010   00 34 fd 4c 40 00 2d 06 69 ae 33 d2 df 92 c0 a8   .4.L@.-.i.3.....
                0020   12 04 1b 75 22 cb 91 09 19 37 d4 24 4f 34 50 18   ...u"....7.$O4P.
                0030   01 f6 50 e1 00 00 0c 00 fe ff 49 bc 09 c2 dc 65   ..P.......I....e
                0040   30 1a                                             0.

                //second packet from C2S
                0000   5c e7 47 9f 9d 80 7c c2 c6 1b 45 86 08 00 45 00   \.G...|...E...E.
                0010   00 f8 18 94 40 00 80 06 fb 5a c0 a8 12 04 33 d2   ....@....Z....3.
                0020   df 92 22 cb 1b 75 d4 24 4f 34 91 09 19 43 50 18   .."..u.$O4...CP.
                0030   01 03 3c ac 00 00 d0 00 e5 0c dd dc 4f 01 00 07   ..<.........O...
                0040   74 6f 65 65 6e 6b 32 00 20 66 34 37 64 61 37 36   toeenk2. f47da76
                0050   33 31 39 32 61 31 65 36 33 36 37 64 31 38 30 64   3192a1e6367d180d
                0060   37 65 35 62 37 35 36 35 36 00 2f 41 4d 44 20 52   7e5b75656./AMD R
                0070   79 7a 65 6e 20 35 20 33 36 30 30 20 36 2d 43 6f   yzen 5 3600 6-Co
                0080   72 65 20 50 72 6f 63 65 73 73 6f 72 20 20 20 20   re Processor    
                0090   20 20 20 20 20 20 20 20 20 20 00 17 4e 56 49 44             ..NVID
                00a0   49 41 20 47 65 46 6f 72 63 65 20 52 54 58 20 34   IA GeForce RTX 4
                00b0   30 36 30 00 88 34 ff 01 38 57 69 6e 64 6f 77 73   060..4..8Windows
                00c0   20 31 30 20 45 6e 74 65 72 70 72 69 73 65 20 45    10 Enterprise E
                00d0   64 69 74 69 6f 6e 55 6e 6b 6e 6f 77 6e 20 20 28   ditionUnknown  (
                    00e0   42 75 69 6c 64 3a 39 32 30 30 29 20 36 34 42 69   Build:9200) 64Bi
                00f0   74 00 0c 34 2e 30 39 2e 30 30 2e 30 39 30 34 00   t..4.09.00.0904.
                0100   e4 04 00 00 ec 1a                                 ......


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
