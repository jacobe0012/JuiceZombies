//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2023-08-31 14:13:13
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using MessagePack;
using UnityEngine;
using XFramework;

namespace HotFix_UI
{
    public sealed class WebMsgHandler : Singleton<WebMsgHandler>, IDisposable
    {
        private readonly Dictionary<string, MethodInfo> _handlers = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// 注册所有处理 S2C 消息的方法
        /// </summary>
        /// <param name="handlerObject">包含处理方法的对象实例</param>
        public void AddHandler(object handlerObject)
        {
            var methods = handlerObject.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<MsgHandlerAttribute>();
                if (attr != null)
                {
                    // 使用消息类型的名称作为字典键
                    _handlers.Add(attr.MsgType.Name, method);
                }
            }
        }

        /// <summary>
        /// 处理从服务器接收到的消息
        /// </summary>
        public void PackageHandler(string msgTypeName, byte[] msgBodyBytes)
        {
            if (_handlers.TryGetValue(msgTypeName, out var method))
            {
                // 获取处理方法的参数类型 (即消息体类型)
                var msgType = method.GetParameters()[0].ParameterType;

                // 反序列化消息体
                var msgBody = MessagePackSerializer.Deserialize(msgType, msgBodyBytes);

                // 调用处理方法
                method.Invoke(this, new[] { msgBody });
            }
            else
            {
                Console.WriteLine($"Unknown message type received: {msgTypeName}");
            }
        }

        /// <summary>
        /// 注册消息回调
        /// </summary>
        /// <param name="mergeCmd">合并路由</param>
        /// <param name="IHandler">回调事件</param>
        // public void AddHandler(int mergeCmd, EventHandler<Execute> IHandler, int tagId = 0)
        // {
        //     var name = IHandler.Method.Name;
        //     if (tagId != 0)
        //     {
        //         if (tagPoolsNum.ContainsKey(tagId))
        //         {
        //             tagPoolsNum[tagId]++;
        //         }
        //         else
        //         {
        //             tagPoolsNum.Add(tagId, 1);
        //         }
        //
        //         IHandler += (sender, e) =>
        //         {
        //             //Log.Debug($"{name}11111111111", Color.green);
        //             if (tagPoolsNum.ContainsKey(tagId))
        //             {
        //                 tagPoolsNum[tagId]--;
        //                 if (tagId == 2)
        //                 {
        //                     Log.Debug(
        //                         $"收到上层数据 CMDOld: {mergeCmd}",
        //                         Color.green);
        //                 }
        //
        //                 if (tagPoolsNum[tagId] <= 0)
        //                 {
        //                     Log.Debug($"完成了初始数据层:{tagId}", Color.green);
        //                     if (tagEvent.TryGetValue(tagId, out var eventHandler))
        //                     {
        //                         eventHandler.Invoke(eventHandler, EventArgs.Empty);
        //                         tagEvent.TryRemove(tagId, out var f1);
        //
        //                         tagPoolsNum.TryRemove(tagId, out var f);
        //                         //eventHandler = null;
        //                         // tagPoolsNum.Clear();
        //                         // //eventHandler = null;
        //                         // tagEvent.Clear();
        //                     }
        //                 }
        //             }
        //         };
        //     }
        //
        //     if (handlers.ContainsKey(mergeCmd))
        //     {
        //         if (handlers[mergeCmd].ContainsKey(name))
        //         {
        //             Log.Debug($"IHandler:{name} is already exists!", Color.red);
        //             return;
        //         }
        //
        //         handlers[mergeCmd].Add(name, IHandler);
        //         return;
        //     }
        //
        //     handlers[mergeCmd] = new Dictionary<string, EventHandler<Execute>>();
        //     handlers[mergeCmd].Add(name, IHandler);
        // }
        //
        //
        // //消息分发
        // public void PackageHandler(int mergeCmd, byte[] data, string args = "")
        // {
        //     // int cmd = CmdHelper.GetCmd(mergeCmd);
        //     // int subCmd = CmdHelper.GetSubCmd(mergeCmd);
        //     if (!handlers.ContainsKey(mergeCmd))
        //     {
        //         //TODO:
        //         if (mergeCmd != 0)
        //         {
        //             Log.Debug($"cmd:{mergeCmd} is not exists,please check out!", Color.red);
        //         }
        //         else
        //         {
        //             Log.Debug($"ReceiveHeartbeat", Color.cyan);
        //         }
        //
        //         return;
        //     }
        //
        //     if (data.Length <= 0)
        //     {
        //         Log.Debug($"cmd:{mergeCmd} 接收到空消息", Color.red);
        //     }
        //
        //     var handlerDic = handlers[mergeCmd];
        //     var tempList = new List<EventHandler<Execute>>(handlerDic.Values);
        //
        //     foreach (var handler in tempList)
        //     {
        //         handler.Invoke(handler, new Execute(data, args));
        //     }
        //
        //     tempList.Clear();
        //     tempList = null;
        // }

        public void Clear()
        {
          
        }

        public void Dispose()
        {
            Clear();
            Instance = null;
        }
    }
}