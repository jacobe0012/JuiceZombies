//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-08-31 10:47:42
//---------------------------------------------------------------------

using System;
using System.IO;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using MessagePack;
using Newtonsoft.Json;
using UnityEngine;
using UnityWebSocket;
using XFramework;
using ErrorEventArgs = UnityWebSocket.ErrorEventArgs;


namespace HotFix_UI
{
    public sealed class NetWorkManager : Singleton<NetWorkManager>, IDisposable
    {
        public static int wsUrl0 = 29;
        public static int wsUrl1 = 22;
        public static string emptyUrl = "ws://192.168.2.{0}:10100/websocket";

        private WebSocket socket;

        private Color debugColor;

#if UNITY_EDITOR
        public static string savePath = "Assets/Resources/WsUrl.json";
#else
        public static string savePath = Application.persistentDataPath + "/WsUrl.json";
#endif
        private long timerId;

        private const float HeartbeatInterval = 10; // 心跳间隔时间

        private float timer; // 计时器

        // 定义数据结构
        [Serializable]
        public class WebUrlData
        {
            [JsonProperty("webUrl")] public string webUrl;
        }

        // 断线重连间隔（单位：秒）
        private const float reconnectInterval = 2f;

        // 最大重连次数
        private const int maxReconnectAttempts = 20;

        private int curReconnectAttempts = 0;

        public static string url;

        private static readonly MessagePackSerializerOptions options =
            MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

        public void Init()
        {
            // if (!File.Exists(savePath))
            // {
            //     // 如果 JSON 文件不存在，创建一个示例 JSON 数据
            //     CreateSampleData();
            // }
            //
            // // 读取数据
            // string json = File.ReadAllText(savePath);
            // WebUrlData data = JsonConvert.DeserializeObject<WebUrlData>(json);
            // string url = data.webUrl;
            //
            // //Log.Error($"{url}");
            // var sharedData = await JsonManager.Instance.LoadSharedData();
            // if (sharedData.lastLoginUserId > 0)
            // {
            //     var playerData = await JsonManager.Instance.LoadPlayerData(sharedData.lastLoginUserId);
            //     url += $"?token={playerData.privateKey}&udId={SystemInfo.deviceUniqueIdentifier}";
            // }

            url = $"ws://{MyUrl.urlipv4}/ws";

            debugColor = Color.cyan;
            socket = new WebSocket(url);
            Log.Debug($"准备建立链接,websocketURL:{url}", Color.cyan);
            socket.OnOpen += OnOpen;
            socket.OnClose += OnClose;
            socket.OnError += OnError;
            socket.OnMessage += OnMessage;
            socket.ConnectAsync();
            //curReconnectAttempts = 0;
        }


        // 尝试重连
        async UniTaskVoid AttemptReconnect(bool isNew = false)
        {
            if (curReconnectAttempts < maxReconnectAttempts)
            {
                Debug.Log($"Reconnecting... Attempt {curReconnectAttempts}");
                await UniTask.Delay((int)(curReconnectAttempts * reconnectInterval * 1000f));
                curReconnectAttempts++;
                if (isNew)
                {
                    Close();
                    Init();
                }
                else
                {
                    socket.ConnectAsync();
                }
            }
            else
            {
                curReconnectAttempts = 0;
                Debug.LogError("断线重连超过最大次数");
#if UNITY_EDITOR
                Application.Quit();
#endif

                //TODO:断线重连超过最大次数
            }

            // Debug.Log($"{socket.ReadyState}");
            // if (socket.ReadyState != WebSocketState.Open)
            // {
            //     if (curReconnectAttempts < maxReconnectAttempts)
            //     {
            //         Debug.Log($"Reconnecting... Attempt {curReconnectAttempts}");
            //         await UniTask.Delay((int)((curReconnectAttempts + 1) * reconnectInterval * 1000f));
            //         curReconnectAttempts++;
            //     }
            //     else
            //     {
            //         curReconnectAttempts = 0;
            //         Debug.LogError("Max reconnect attempts reached.");
            //         //TODO:断线重连超过最大次数
            //     }
            // }
        }


        private void CreateSampleData()
        {
            // 存储数据
            string wsUrl = string.Format(emptyUrl, wsUrl0);
            WebUrlData data = new WebUrlData { webUrl = wsUrl };
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(savePath, json);
        }

        public void ReInit()
        {
            if (!File.Exists(savePath))
            {
                // 如果 JSON 文件不存在，创建一个示例 JSON 数据
                CreateSampleData();
            }

            // 读取数据
            string json = File.ReadAllText(savePath);
            WebUrlData data = JsonConvert.DeserializeObject<WebUrlData>(json);
            string url = data.webUrl;

            //Log.Error($"{url}");

            debugColor = Color.cyan;
            socket = new WebSocket(url);

            // 注册回调
            socket.OnOpen += OnOpen;
            socket.OnClose += OnClose;
            socket.OnMessage += OnMessage;

            socket.OnError += OnError;
            socket.ConnectAsync();

            //curReconnectAttempts = 0;
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.RepeatedFrameTimer(this.Update);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }

        /// <summary>
        /// 关闭链接
        /// </summary>
        public void Close()
        {
            RemoveTimer();
            socket.OnOpen -= OnOpen;
            socket.OnClose -= OnClose;
            socket.OnMessage -= OnMessage;
            socket.OnError -= OnError;
            socket.CloseAsync();
        }

        public void OnOpen(object o, OpenEventArgs args)
        {
            Log.Debug($"OnOpen", debugColor);
            curReconnectAttempts = 0;
            StartTimer();
            ResourcesSingleton.Instance.isConnectSuccess = true;
        }

        public void OnClose(object o, CloseEventArgs args)
        {
            Log.Debug($"OnClose", debugColor);
            RemoveTimer();
            //AttemptReconnect(true);
            ResourcesSingleton.Instance.isConnectSuccess = false;
        }

        public void OnError(object o, ErrorEventArgs args)
        {
            Log.Debug($"OnError: ", debugColor);
            ResourcesSingleton.Instance.isConnectSuccess = false;
            // AttemptReconnect();
            // RemoveTimer();
        }

        void SendHeartbeat()
        {
            //Log.Debug($"SendHeartbeat()", debugColor);
            return;
            var myExternalMessage = new MyExternalMessage
            {
                CmdCode = 0,
                ProtocolSwitch = 0,
                CmdMerge = 0,
                ResponseStatus = 0,
                ValidMsg = "0"
            };

            socket.SendAsync(myExternalMessage.ToByteArray());
        }

        void Update()
        {
            timer += Time.unscaledDeltaTime;
            // 当计时器超过心跳间隔时发送心跳消息
            if (timer >= HeartbeatInterval)
            {
                timer = 0f; // 重置计时器
                SendHeartbeat();
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        public void OnMessage(object o, MessageEventArgs args)
        {
            //将字节数组转换为
            //IMessage message = new MyExternalMessage();

            //var mySelf = (MyExternalMessage)message.Descriptor.Parser.ParseFrom(args.RawData);
            // if (mySelf.ResponseStatus != 0)
            // {
            //     ErrorMsg.LogErrorMsg(mySelf.ResponseStatus);
            // }

            //Log.Debug($"ResponseStatus:{mySelf.ResponseStatus}", debugColor);

            // byte[] byteArray = mySelf.DataContent.ToByteArray();
            // string content = System.Text.Encoding.Default.GetString(byteArray);

            //Log.Debug($"OnMessage:{mySelf}", debugColor);

            if (args.RawData == null)
            {
                Log.Debug($"empty message", debugColor);
                return;
            }

            var message = MessagePackSerializer.Deserialize<MyMessage>(args.RawData, options);
            if (message.ErrorCode != 0)
            {
                ErrorMsg.LogErrorMsg(message.ErrorCode);
            }

            WebMsgHandler.Instance.PackageHandler(message.Cmd, message.Content);
            //var playerData = MessagePackSerializer.Deserialize<PlayerData>(message.Content);

            //UniTask.Delay(111);
        }

        /// <summary>
        /// 向服务器发送proto消息
        /// </summary>
        /// <param name="cmd">业务主路由</param>
        /// <param name="subCmd">业务子路由</param>
        /// <param name="protoMessage">发送的proto消息类</param>
        /// <typeparam name="T"></typeparam>
        public void SendMsg(int cmd, string args = "")
        {
            var myExternalMessage = new MyMessage
            {
                Cmd = cmd,
                Content = new byte[]
                {
                },
                ErrorCode = 0,
                Args = args
            };
            socket.SendAsync(MessagePackSerializer.Serialize(myExternalMessage, options));
        }

        /// <summary>
        /// 向服务器发送proto消息
        /// </summary>
        /// <param name="cmd">业务主路由</param>
        /// <param name="subCmd">业务子路由</param>
        /// <param name="protoMessage">发送的proto消息类</param>
        /// <typeparam name="T"></typeparam>
        public void SendMsg<T>(int cmd, T protoMessage, string args = "") where T : IMessagePack
        {
            var myExternalMessage = new MyMessage
            {
                Cmd = cmd,
                Content = MessagePackSerializer.Serialize(protoMessage,
                    options),
                ErrorCode = 0,
                Args = args,
            };

            socket.SendAsync(MessagePackSerializer.Serialize(myExternalMessage, options));
        }


        /// <summary>
        /// 向服务器发送proto消息
        /// </summary>
        /// <param name="cmd">业务主路由</param>
        /// <param name="subCmd">业务子路由</param>
        /// <param name="protoMessage">发送的proto消息类</param>
        /// <typeparam name="T"></typeparam>
        public void SendMessage<T>(int cmd, int subCmd, T protoMessage) where T : IMessage<T>, IBufferMessage
        {
            return;
            var myExternalMessage = new MyExternalMessage
            {
                CmdMerge = CmdHelper.GetMergeCmd(cmd, subCmd),
                DataContent = protoMessage.ToByteString(),
                ProtocolSwitch = 0,
                CmdCode = 1
            };

            socket.SendAsync(myExternalMessage.ToByteArray());
        }

        /// <summary>
        /// 向服务器发送proto消息
        /// </summary>
        /// <param name="cmd">业务主路由</param>
        /// <param name="subCmd">业务子路由</param>
        /// <param name="protoMessage">发送的proto消息类</param>
        /// <typeparam name="T"></typeparam>
        public void SendMessage(int cmd, int subCmd, ByteString byteString)
        {
            return;
            var myExternalMessage = new MyExternalMessage
            {
                CmdMerge = CmdHelper.GetMergeCmd(cmd, subCmd),
                DataContent = byteString,
                ProtocolSwitch = 0,
                CmdCode = 1
            };

            socket.SendAsync(myExternalMessage.ToByteArray());
        }

        /// <summary>
        /// 向服务器发送proto消息
        /// </summary>
        /// <param name="cmd">业务主路由</param>
        /// <param name="subCmd">业务子路由</param>
        /// <param name="protoMessage">发送的proto消息类</param>
        /// <typeparam name="T"></typeparam>
        public void SendMessage<T>(int mergeCmd, T protoMessage) where T : IMessage<T>, IBufferMessage
        {
            return;
            var myExternalMessage = new MyExternalMessage
            {
                CmdMerge = mergeCmd,
                DataContent = protoMessage.ToByteString(),
                ProtocolSwitch = 0,
                CmdCode = 1
            };

            socket.SendAsync(myExternalMessage.ToByteArray());
        }

        /// <summary>
        /// 向服务器发送路由消息
        /// </summary>
        /// <param name="cmd">业务主路由</param>
        /// <param name="subCmd">业务子路由</param>
        public void SendMessage(int cmd, int subCmd)
        {
            return;
            var myExternalMessage = new MyExternalMessage
            {
                CmdMerge = CmdHelper.GetMergeCmd(cmd, subCmd),
                ProtocolSwitch = 0,
                CmdCode = 1
            };

            socket.SendAsync(myExternalMessage.ToByteArray());
        }

        /// <summary>
        /// 向服务器发送路由消息
        /// </summary>
        /// <param name="mergeCmd">合并路由</param>
        public void SendMessage(int mergeCmd)
        {
            return;
            //Log.Debug($"SendMessageSendMessageSendMessageSendMessage",Color.green);
            var myExternalMessage = new MyExternalMessage
            {
                CmdMerge = mergeCmd,
                ProtocolSwitch = 0,
                CmdCode = 1
            };
            //myExternalMessage.
            socket.SendAsync(myExternalMessage.ToByteArray());
        }

        public void Dispose()
        {
            RemoveTimer();
            socket.CloseAsync();
            socket = null;
            Instance = null;
        }
    }
}