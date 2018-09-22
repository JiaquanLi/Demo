using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Routeguide;
using Grpc.Core;
using Google.Protobuf;
using System.Net.NetworkInformation;

namespace Demo
{
    public class RouteGuideClient
    {
        private string strRpcMessage;
        readonly RouteGuide.RouteGuideClient client;

        public RouteGuideClient(RouteGuide.RouteGuideClient client)
        {
            this.client = client;
        }

        public string RPC_Message
        {
            get
            {
                return strRpcMessage;
            }
        }

        public async Task RouteChat()
        {
            try
            {

                using (var call = client.RouteChat())
                {
                    var responseReaderTask = Task.Run(async () =>
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            var note = call.ResponseStream.Current;
                            //Log("Got message {0}", note.Datasend.ToStringUtf8());
                            strRpcMessage = string.Format("{0}", note.Datasend.ToStringUtf8());
                        }
                    });

                    string strTemp = "";
                    //FileStream fs = new FileStream("CloudPoint.txt", FileMode.Open);
                    //StreamReader sr = new StreamReader(fs);
                    RouteNote noteSend = new RouteNote();

                    int total = 0;
                    int iCpyCunt = 0;
                    int index = 0;
                    int size = 4096*10;

                    //byte[] buffer = new byte[1024];
                    //fs.Read(buffer, index, 4096);
                    byte[] buffer = System.IO.File.ReadAllBytes("CloudPoint.txt");
                    total = buffer.Length;

                    while (total >0)
                    {
                        byte[] bufWrite;
                        if (total > size)
                        {
                            iCpyCunt = size;
       
                        }
                        else
                        {
                            iCpyCunt = total;
                            
                        }

                        bufWrite = new byte[iCpyCunt];
                        Array.Copy(buffer, index,bufWrite,0,iCpyCunt);
                        strTemp = Encoding.UTF8.GetString(bufWrite);
                        noteSend.Datasend = ByteString.CopyFromUtf8(strTemp);
                        noteSend.Size = strTemp.Length;
                        await call.RequestStream.WriteAsync(noteSend);
                        total -= iCpyCunt;
                        index += iCpyCunt;
                    }



                    //while ((strTemp = sr.ReadLine()) != null)
                    //{
                    //    noteSend.Datasend = ByteString.CopyFromUtf8(strTemp);
                    //    noteSend.Size = strTemp.Length;
                    //    await call.RequestStream.WriteAsync(noteSend);
                    //}
                    await call.RequestStream.CompleteAsync();
                    await responseReaderTask;
                }

                
            }
            catch (RpcException e)
            {
                //Log("RPC failed", e);
                throw;
            }
        }

    }
    class clsAlgorithm_Server
    {
        public delegate void OnMessageCallback(string message);
        public event OnMessageCallback Callback;
        private string rpcMsg;

        public void StartOneRpc(ref string GetMsg)
        {
            if(Ping() == false)
            {
                Callback("can't connect server");
                return;
            }
            Callback("Ping Server: Statue ok ...");
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(RpcRun));
            th.IsBackground = true;
            th.Start();

            while (th.IsAlive)
            {
                System.Threading.Thread.Sleep(500);
            }

            GetMsg = rpcMsg;
        }

        private void RpcRun()
        {
            string strConString;
            
            strConString = string.Format("{0}:{1}", ReadIniSettings.ReadIni.objIniValue.iniServer.Ip, ReadIniSettings.ReadIni.objIniValue.iniServer.Port);
            //var channel = new Channel("192.168.50.30:50051", ChannelCredentials.Insecure);
            var channel = new Channel(strConString, ChannelCredentials.Insecure);
             var objclient = new RouteGuideClient(new RouteGuide.RouteGuideClient(channel));
            // Send and receive some notes.
            objclient.RouteChat().Wait();

            channel.ShutdownAsync().Wait();

            rpcMsg = objclient.RPC_Message;
        }

        private bool Ping()
        {
            bool bStatue = false;
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(ReadIniSettings.ReadIni.objIniValue.iniServer.Ip);
            if (pingReply.Status == IPStatus.Success)
            {
                bStatue = true;
            }
            else
            {
                bStatue = false;
            }

            return bStatue;
        }
    }
}
