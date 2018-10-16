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
        private long FileSize;
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

        public long RPC_FileSize
        {
            get
            {
                return FileSize;
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
                    int size = 4096 * 10;

                    //byte[] buffer = new byte[1024];
                    //fs.Read(buffer, index, 4096);
                    byte[] buffer = System.IO.File.ReadAllBytes("CloudPoint.txt");
                    total = buffer.Length;

                    while (total > 0)
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
                        Array.Copy(buffer, index, bufWrite, 0, iCpyCunt);
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

        public async Task SendTempletFile()
        {
            try
            {
                using (var call = client.SetIcpTemplet())
                {

                    string strTemp = "";
                    TempletFileRequest noteSend = new TempletFileRequest();

                    int total = 0;
                    int iCpyCunt = 0;
                    int index = 0;
                    int size = 4096 * 10;

                    byte[] buffer = System.IO.File.ReadAllBytes("CloudPoint.txt");
                    
                    total = buffer.Length;
                    FileSize = total;
                    while (total > 0)
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
                        Array.Copy(buffer, index, bufWrite, 0, iCpyCunt);
                        strTemp = Encoding.UTF8.GetString(bufWrite);
                        noteSend.Datasend = ByteString.CopyFromUtf8(strTemp);
                        noteSend.Size = strTemp.Length;
                        await call.RequestStream.WriteAsync(noteSend);
                        // A bit of delay before sending the next one.
                        await Task.Delay(100);
                        total -= iCpyCunt;
                        index += iCpyCunt;
                    }

                    await call.RequestStream.CompleteAsync();
                    TempletFileReply rply    = await call.ResponseAsync;
                    if (rply.Retsts == false)
                    {
                        System.Windows.Forms.MessageBox.Show("TempletFileReply false");
                    }
                }
            }
            catch (RpcException e)
            {
                //Log("RPC failed", e);
                throw;
            }
        }

    }
    public class clsAlgorithm_Server
    {
        public delegate void OnMessageCallback(string message);
        public event OnMessageCallback Callback;
        private string rpcMsg;
        private long rpcFileSize;


        public void SetIcpMaxIte(int maxIterations)
        {
            string strConString;
            strConString = string.Format("{0}:{1}", ReadIniSettings.ReadIni.objIniValue.iniServer.Ip, ReadIniSettings.ReadIni.objIniValue.iniServer.Port);
            Channel channel = new Channel(strConString, ChannelCredentials.Insecure);

            var client = new RouteGuide.RouteGuideClient(channel);

            var reply = client.SetIcpMaxIterations(new IcpMaxIterationsRequest { Maxiterations = maxIterations });

            channel.ShutdownAsync().Wait();

            if (reply.Retsts == false)
            {
                //System.Windows.Forms.MessageBox.Show("rpc return fail");
            }
        }
        public void SetIcpFilter(double filter)
        {
            string strConString;
            strConString = string.Format("{0}:{1}", ReadIniSettings.ReadIni.objIniValue.iniServer.Ip, ReadIniSettings.ReadIni.objIniValue.iniServer.Port);
            Channel channel = new Channel(strConString, ChannelCredentials.Insecure);

            var client = new RouteGuide.RouteGuideClient(channel);

            var reply = client.SetIcpFilter(new IcpFilterRequest {Filter= (float)filter });

            channel.ShutdownAsync().Wait();

            if (reply.Retsts == false)
            {
                //System.Windows.Forms.MessageBox.Show("rpc return fail");
            }
        }

        public void StartOneRpc(ref string GetMsg)
        {
            if (Ping() == false)
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

        public void SetIcpTempletFile()
        {
            if (Ping() == false)
            {
                Callback("can't connect server");
                return;
            }
            Callback("Ping Server: Statue ok ...");
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(RpcSetIcpTempletFile));
            th.IsBackground = true;
            th.Start();

            while (th.IsAlive)
            {
                System.Threading.Thread.Sleep(500);
            }

            Callback("Send File Size:" + rpcFileSize.ToString());



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

        private void RpcSetIcpTempletFile()
        {
            string strConString;

            strConString = string.Format("{0}:{1}", ReadIniSettings.ReadIni.objIniValue.iniServer.Ip, ReadIniSettings.ReadIni.objIniValue.iniServer.Port);

            var channel = new Channel(strConString, ChannelCredentials.Insecure);
            var objclient = new RouteGuideClient(new RouteGuide.RouteGuideClient(channel));

            objclient.SendTempletFile().Wait();
            channel.ShutdownAsync().Wait();
            rpcFileSize = objclient.RPC_FileSize;
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
