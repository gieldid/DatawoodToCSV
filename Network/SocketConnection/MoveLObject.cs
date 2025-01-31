﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DatawoodGH.Utility;

namespace DatawoodGH.Network.SocketConnection
{
    public class MoveLObject : MoveObject
    {
        public string Pos { get; set; }
        public string Orient { get; set; }

        public MoveLObject(string line, Dictionary<string, string> speeds, Dictionary<string, string> zones) : base(line, speeds, zones)
        {
            Name = "MoveL";
            ReadPos(line);
            ReadOrient(line);
        }

        private void ReadPos(string line) {
            int openBracket = Utils.GetNthIndex(line, '[', 2);
            int closeBracket = line.IndexOf(']') + 1;

            Pos = line.Substring(openBracket, closeBracket - openBracket);
        }

        private void ReadOrient(string line) {
            int openBracket = Utils.GetNthIndex(line, '[', 3);
            int closeBracket = Utils.GetNthIndex(line, ']', 2) + 1;

            Orient = line.Substring(openBracket, closeBracket - openBracket);
        }

        public override async Task SendOverSocket(Socket client)
        {
            await SendOverSocketMoveBase(client);
            byte[] payload = Encoding.UTF8.GetBytes(Pos);
            client.Send(payload);
            await Task.Delay(WaitTime);

            payload = Encoding.UTF8.GetBytes(Orient);
            client.Send(payload);
            await Task.Delay(WaitTime);
        }

    }
}
