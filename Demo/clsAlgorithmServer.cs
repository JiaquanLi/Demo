using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Routeguide;
using Grpc.Core;
using Google.Protobuf;

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
            strRpcMessage = "";
            try
            {
                Log("*** RouteChat");
                using (var call = client.RouteChat())
                {
                    var responseReaderTask = Task.Run(async () =>
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            var note = call.ResponseStream.Current;
                            Log("Got Message:" + note.Datasend.ToStringUtf8());
                            strRpcMessage = note.Datasend.ToStringUtf8();
                        }
                    });

                    string strTemp = "";
                    FileStream fs = new FileStream("test.txt", FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    RouteNote noteSend = new RouteNote();
                    //while (() != null)
                    //{
                    strTemp = sr.ReadToEnd();
                    noteSend.Datasend = ByteString.CopyFromUtf8(strTemp);
                    noteSend.Size = strTemp.Length;
                    await call.RequestStream.WriteAsync(noteSend);
                    await call.RequestStream.CompleteAsync();
                    await responseReaderTask;

                    Log("Finished RouteChat");
                }
            }
            catch (RpcException e)
            {
                Log("RPC failed");
                throw;
            }
        }



        private void Log(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

    }
    class clsAlgorithm_Server
    {
        private string rpcMsg;
        public void StartOneRpc(ref string GetMsg)
        {
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

            var channel = new Channel("192.168.50.24:50051", ChannelCredentials.Insecure);
            var client = new RouteGuideClient(new RouteGuide.RouteGuideClient(channel));
            // Send and receive some notes.
            client.RouteChat().Wait();

            channel.ShutdownAsync().Wait();

            rpcMsg = client.RPC_Message;
        }
    }
}
