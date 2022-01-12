﻿using System;
using System.Collections.Generic;
using DatawoodGH.Network;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Net.Sockets;
using System.Net;
using System.Text;
using DatawoodGH.Utils;

namespace CSVModule.Network
{
    public class SocketClient : NetworkComponent
    {
        private const string MOVE_L = "MoveL";
        private const string MOVE_ABSJ = "MoveAbsJ";

        /// <summary>
        /// Initializes a new instance of the WebSocketComponent class.
        /// </summary>
        public SocketClient()
          : base("Socket Client", "Socket",
              "Makes a socket client and connects to given server.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Ip", "IP", "IP to make a web socket connection to", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Port","P","Port for the socket connection",GH_ParamAccess.item);
            pManager.AddTextParameter("Targets", "T", "Robottargets", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Run", "R", "When to run", GH_ParamAccess.item, true);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Message","m","Message received",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string ip = null;
            int port = 0;
            bool run = true;
            List<string> targets = new List<string>();

            if (!DA.GetData("Ip", ref ip)) {
                return;
            }

            if (!DA.GetData("Port", ref port)) {
                return;
            }

            if (!DA.GetDataList("Targets", targets)) {
                return;
            }

            DA.GetData("Run", ref run);
            
            if (run) {
                Socket client = SocketConnection(ip, port);
                SendTargets(client, targets);
                CloseConnection(client);
            }
        }

		private void CloseConnection(Socket client)
		{
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

		private Socket SocketConnection(string ip, int port) {        
            IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client.Connect(remoteEP);
            this.Message = "Socket connected to "+ client.RemoteEndPoint.ToString();
     
            byte[] payload = Encoding.UTF8.GetBytes("listening?");
            client.Send(payload);

            byte[] bytes = new byte[1024];
            int bytesRec = client.Receive(bytes);
            string answer = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            this.Message = "Received the following: " + answer;
            if (answer != "Yes") {
                CloseConnection(client);
            }
            return client;
        }

        private void SendTargets(Socket client, List<string> targets) {
            foreach (var target in targets) {

                List<string> messages = RAPIDToTargets(target);

                foreach(var message in messages) {
                    byte[] payload = Encoding.UTF8.GetBytes(message);
                    client.Send(payload);
                    System.Threading.Thread.Sleep(500);
                }


                //Reply from server
                byte[] bytes = new byte[1024];
                int bytesRec = client.Receive(bytes);
                string answer = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                if (answer != "ready") { 
                    return; 
                }
            }
            //byte[] end_payload = Encoding.UTF8.GetBytes("No more targets");
            //client.Send(end_payload);
        }

        /// <summary>
        /// Splits the received RAPID string into following targets
        /// 1 Type of move
        /// 2 robjoint
        /// 3 Speed
        /// </summary>
        /// <param name="RAPID"></param>
        /// <returns></returns>
        private List<string> RAPIDToTargets(string RAPID) {
            List<string> targets = new List<string>();
            targets.Add(GetSpeed(RAPID));
            targets.Add(GetExtJoint(RAPID));
            targets.Add(GetPosOrRobJoint(RAPID));


            if (RAPID.Contains(MOVE_L)) {
                targets.Insert(0, MOVE_L);
                targets.Add(GetOrient(RAPID));
            } else if (RAPID.Contains(MOVE_ABSJ)) {
                targets.Insert(0,MOVE_ABSJ);
            }
            
            return targets;
        }

        private string GetSpeed(string RAPID) {
            string[] values = RAPID.Split(',');
            string speed = null;
            foreach (var value in values)
            {
                if (value.Contains("Speed"))
                {
                    speed = value;
                }
            }
            return speed;
        }

        private string GetPosOrRobJoint(string RAPID) {
            int openBracket = Utils.GetNthIndex(RAPID, '[', 2) ;
            int closeBracket = RAPID.IndexOf(']') + 1;

            return RAPID.Substring(openBracket, closeBracket - openBracket);
        }

        private string GetOrient(string RAPID) {
            int openBracket = Utils.GetNthIndex(RAPID, '[', 3);
            int closeBracket = Utils.GetNthIndex(RAPID, ']', 2) + 1;

            return RAPID.Substring(openBracket, closeBracket - openBracket);
        }

        private string GetExtJoint(string RAPID) {
            int openBracket = RAPID.LastIndexOf('[');
            int closeBracket = RAPID.LastIndexOf(']');

            return RAPID.Substring(openBracket, closeBracket - openBracket);
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("50843146-2808-4E45-8501-6716BDACB6EC"); }
        }
    }
}