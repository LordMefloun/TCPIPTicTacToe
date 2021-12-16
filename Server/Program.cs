﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Manager;
using Server.Objects;

namespace Server
{
    class Program
    {
        public static List<TcpClient> Client { get; private set; }
        private static NetworkStream _stream { get; set; }
        private static Task _Task { get; set; }

        static void Main(string[] args)
        {
            Client = new List<TcpClient>();
            
            TcpListener server = new TcpListener(8888);
            TcpClient client = default(TcpClient);
            int counter = 0;
            while (Client.Count != 2)
            {
                server.Start();
                Console.WriteLine(" >>" + "Server started");
                client = server.AcceptTcpClient();
                Client.Add(client);
                Console.WriteLine($" {client.Client.RemoteEndPoint} >>" + "connected");
                _stream = client.GetStream();
                _Task = Task.Run(async ()
                    =>
                {
                    while (Client.Count <= 1)
                    {
                        var paket = Encoding.Default.GetBytes("Waiting for players");
                        await Task.Delay(10000);
                        Console.WriteLine("sent packet");
                        _stream.Write(paket, 0, paket.Length);
                    }
                });
            }
            
            if (Client.Count == 2)
            {
                //TODO: Sort Name Packet
                
                Player player1 = new Player("player1" , 0 , Client[0].Client.RemoteEndPoint.ToString() , 8888);
                Player player2 = new Player("player2" , 0 , Client[1].Client.RemoteEndPoint.ToString() , 8888);
                GameManager gameManager = new GameManager(player1 , player2 , _stream);
                Client.Clear();
                gameManager.StartGame();
                
            }
        }

        

}
}