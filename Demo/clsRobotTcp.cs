using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net;
using System.Xml;

namespace Demo
{
    public struct RobotTcpInfo
    {
        public string ipAddress;
        public int port;
    }
    class clsRobotTcp
    {
        public delegate void OnMessageCallback(string message);
        public event OnMessageCallback Callback;
        bool bExit = false;
        Thread thTcpServer = null;
        private TcpListener objTcpServer = null;
        private RobotTcpInfo objTcpInfo;

        public RobotTcpInfo TcpConnectInfo
        {
            get
            {
                return objTcpInfo;
            }
            set
            {
                objTcpInfo = value;
            }
        }

        public clsRobotTcp(string IpAddress,int Port)
        {
            objTcpInfo.ipAddress = IpAddress;
            objTcpInfo.port = Port;
            thTcpServer = new Thread(new ThreadStart(TcpServer));
            thTcpServer.IsBackground = true;

        }

        public clsRobotTcp(RobotTcpInfo tcpInfo)
        {
            objTcpInfo = tcpInfo;

            thTcpServer = new Thread(new ThreadStart(TcpServer));
            thTcpServer.IsBackground = true;

        }

        ~clsRobotTcp()
        {
            bExit = true;
            if (objTcpServer != null) objTcpServer.Stop();
        }


        public bool StartServer()
        {
            if (PingClient() == false) return false;
            if (thTcpServer == null) return false;
            if (objTcpInfo.ipAddress.Length < 7 || objTcpInfo.port < 1) return false;

            thTcpServer.Start();
            return true;
        }
        private void TcpServer()
        {

            string strSend = "";
            string strReceive = "";
            IPAddress IP = IPAddress.Parse(objTcpInfo.ipAddress);
            objTcpServer = new TcpListener(IP, objTcpInfo.port);
            objTcpServer.Start();

            byte[] bytes = new Byte[1024];
            
            while (bExit != true)
            {
                Socket handler;
                handler = objTcpServer.AcceptSocket();
                XmlDocument SendXML = new XmlDocument();
                SendXML.PreserveWhitespace = true;
                SendXML.Load("points.xml");
                strSend = SendXML.InnerXml;
                int bytesRec = handler.Receive(bytes);
                strReceive = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec);
                Console.WriteLine(strReceive);
                if (strReceive != null)
                {
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(strSend);
                    Console.WriteLine(strSend);
                    Console.WriteLine("\n");
                    handler.Send(msg, 0, msg.Length, System.Net.Sockets.SocketFlags.None);
                }

                handler.Close();
            }
        }

        bool PingClient()

        {
            bool bStatue;
            Ping ping = new Ping();
            Callback("Ping ip:" + objTcpInfo.ipAddress);
            PingReply pingReply = ping.Send(objTcpInfo.ipAddress);
            if (pingReply.Status == IPStatus.Success)
            {
                bStatue = true;
                Callback("Ping ip:" + objTcpInfo.ipAddress + "OK");
            }
            else
            {
                bStatue = false;
                Callback("Ping ip:" + objTcpInfo.ipAddress + "NG");
            }

            return bStatue;
        }

    }
}
