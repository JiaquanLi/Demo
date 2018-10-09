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

        public clsRobotTcp(string IpAddress, int Port)
        {
            objTcpInfo.ipAddress = IpAddress;
            objTcpInfo.port = Port;
        }

        public clsRobotTcp(RobotTcpInfo tcpInfo)
        {
            //objTcpInfo = tcpInfo;
        }

        ~clsRobotTcp()
        {
            bExit = true;
            if (objTcpServer != null) objTcpServer.Stop();
        }

        public bool StartServer()
        {
            if (PingClient() == false) return false;
            thTcpServer = new Thread(new ThreadStart(TcpServer));
            thTcpServer.IsBackground = true;

            if (thTcpServer == null) return false;
            //if (objTcpInfo.ipAddress.Length < 7 || objTcpInfo.port < 1) return false;

            thTcpServer.Start();
            return true;
        }
        private void TcpServer()
        {

            int a = 0;

            TcpListener server = null;
            IPAddress IP = IPAddress.Parse("192.168.11.5");
            server = new TcpListener(IP, 49152);
            server.Start();
            String strSend = null;

            byte[] bytes = new Byte[1024];
            Socket handler;
            while (true)
            {
                String strReceive = null;
                handler = server.AcceptSocket();
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
            }
        }

        bool PingClient()
        {
            string ip;
            bool bStatue = true;
            Ping ping = new Ping();
            if (ReadIniSettings.ReadIni.objIniValue.iniRobot.Ip == null)
            {
                ip = objTcpInfo.ipAddress;
            }
            else
            {
                ip = ReadIniSettings.ReadIni.objIniValue.iniRobot.Ip;
            }

            Callback("start Ping robot IP:" + ip + "   5s");
            DateTime dt = DateTime.Now.AddSeconds(1);
            while (dt > DateTime.Now)
            {
                try
                {
                    PingReply pingReply = ping.Send(ip);
                    if (pingReply.Status == IPStatus.Success)
                    {
                        bStatue = true;
                        Callback("Ping ip:" + objTcpInfo.ipAddress + " OK");
                        break;
                    }
                    else
                    {
                        bStatue = false;
                        Callback("Ping ip:" + objTcpInfo.ipAddress + " NG");
                    }
                }
                catch (Exception ex)
                {
                    Callback(ex.Message);
                    bStatue = false;
                    break;
                }
            }
            return bStatue;
        }
    }
}