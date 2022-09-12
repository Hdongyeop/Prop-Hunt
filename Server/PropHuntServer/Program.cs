using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using PropHuntServer.Data;
using PropHuntServer.GameContent;
using PropHuntServer.GameContent.Room;
using ServerCore;

namespace PropHuntServer
{
    class Program
    {
        static Listener _listener = new Listener();
        static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();

        static void TickRoom(GameRoom room, int tick = 100)
        {
            var timer = new System.Timers.Timer();
            timer.Interval = tick;
            timer.Elapsed += ((s, e) => { room.Update(); });
            timer.AutoReset = true;
            timer.Enabled = true;

            _timers.Add(timer);
        }

        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.LoadData();

            GameRoom room = RoomManager.Instance.Add(1);
            TickRoom(room, 50);

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine($"Server IP : {ipAddr}");
            Console.WriteLine("Listening...");

            //FlushRoom();
            //JobTimer.Instance.Push(FlushRoom);

            // TODO
            while (true)
            {
                //JobTimer.Instance.Flush();
                Thread.Sleep(100);
            }
        }
	}
}
