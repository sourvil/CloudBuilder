﻿using Microsoft.AspNetCore.WebSockets.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Waiting to start");
                Console.ReadKey();
                SimpleWork().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task SimpleWork()
        {
            WebSocketClient client = new WebSocketClient();
            WebSocket webSocket = await client.ConnectAsync(new Uri("ws://localhost:5005/"), CancellationToken.None);
            while (webSocket.State == WebSocketState.Open)
            {
                var token = CancellationToken.None;
                var buffer = new ArraySegment<byte>(new byte[4096]);

                // Below will wait for a request message.
                var received = await webSocket.ReceiveAsync(buffer, token);

                switch (received.MessageType)
                {
                    case WebSocketMessageType.Text:
                        //var request = Encoding.UTF8.GetString(buffer.Array,
                        //                                      buffer.Offset,
                        //                                      buffer.Count);

                        var pushedData = Encoding.UTF8.GetString(buffer.Array);
                        // Handle request here.
                        Console.WriteLine("Server pushed data: " + pushedData);
                        break;
                }
            }
        }

        private static async Task RunTestAsync()
        {
            WebSocketClient client = new WebSocketClient();
            WebSocket socket = await client.ConnectAsync(new Uri("ws://localhost:5005/"), CancellationToken.None);
            byte[] data = new byte[100]; 
            while (true)
            {
                // await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                WebSocketReceiveResult result;
                do
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(data), CancellationToken.None);
                    Console.WriteLine("Received: " + result.MessageType + ", " + result.Count + ", " + result.EndOfMessage);
                } while (!result.EndOfMessage);
                Console.WriteLine(Encoding.UTF8.GetString(data, 0, result.Count));
            }
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }
}
