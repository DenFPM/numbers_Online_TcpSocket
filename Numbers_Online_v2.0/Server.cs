using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Numbers_Online_v2._0
{
    class Server
    {
        static Random random = new Random();
        static List<Socket> clients = new List<Socket>()
            ;
        static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            Console.Title="Server";

            socket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            socket.Listen(0);

            socket.BeginAccept(AcceptCallback, null);
        }

        static void AcceptCallback(IAsyncResult ar)
        {
            Socket client = socket.EndAccept(ar);
            Thread thread = new Thread(HandleClient);
            thread.Start(client);

            clients.Add(client);
            socket.BeginAccept(AcceptCallback, null);
        }
        static void HandleClient(object o)
        {
            Socket client = (Socket)o;
            MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
            BinaryReader reader = new BinaryReader(ms);
            BinaryWriter writer = new BinaryWriter(ms);
            while (true)
            {
                client.Receive(ms.GetBuffer());
                int code = reader.ReadInt32();
                switch(code)
                {
                    case 0:
                        int id = random.Next(0, 1001);
                        writer.Write(id);
                        client.Send(ms.GetBuffer());
                        break;
                    case 1:
                        foreach(var c in clients)
                        {
                            if (c != client)
                            {
                                c.Send(ms.GetBuffer());
                            }
                        }
                        break;
                }
            }
        }
    }
}
