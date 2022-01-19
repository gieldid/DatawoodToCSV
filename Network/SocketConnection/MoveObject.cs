﻿using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace DatawoodGH.Network.SocketConnection
{
    public abstract class MoveObject
    {
        public string Name { get; set; }    
        public string Speed { get; set; }
        public string Zone { get; set; }
        public string ExternalJoint { get; set; }

        public MoveObject(string line, Dictionary<string , string> speeds, Dictionary<string, string> zones) {
            ReadExternalJoint(line);
            ReadSpeed(line, speeds);
            ReadZone(line, zones);
        }

        private void ReadExternalJoint(string line) {
            int openBracket = line.LastIndexOf('[');
            int closeBracket = line.LastIndexOf(']');
            ExternalJoint = line.Substring(openBracket, closeBracket - openBracket);
        }

        private void ReadSpeed(string line, Dictionary<string, string> speeds) {
            string[] values = line.Split(',');
            string speedKey = null;
            foreach (var value in values)
            {
                if (value.Contains("Speed"))
                {
                    speedKey = value;
                }
            }

            speeds.TryGetValue(speedKey, out string speedValue);

            Speed = speedValue;
        }

        private void ReadZone(string line, Dictionary<string, string> zones) {
            string[] values = line.Split(',');
            string zoneKey = null;
            foreach (var value in values)
            {
                if (value.Contains("Zone"))
                {
                    zoneKey = value;
                }
            }

            zones.TryGetValue(zoneKey, out string zoneValue);

            Zone = zoneValue;
        }

        public abstract void SendOverSocket(Socket client);

        protected void SendOverSocketBase(Socket client) {
            byte[] payload = Encoding.UTF8.GetBytes(Name);
            client.Send(payload);
            System.Threading.Thread.Sleep(500);

            payload = Encoding.UTF8.GetBytes(Speed);
            client.Send(payload);
            System.Threading.Thread.Sleep(500);

            payload = Encoding.UTF8.GetBytes(Zone);
            client.Send(payload);
            System.Threading.Thread.Sleep(500);

            payload = Encoding.UTF8.GetBytes(ExternalJoint);
            client.Send(payload);
            System.Threading.Thread.Sleep(500);
        }
    }
}