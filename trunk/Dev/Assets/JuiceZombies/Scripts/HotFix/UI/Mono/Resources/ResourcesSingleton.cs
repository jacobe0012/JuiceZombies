using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using cfg.config;
using Common;
using Google.Protobuf.Collections;
using Main;
using UnityEngine;
using XFramework;


namespace HotFix_UI
{
    /// <summary>
    /// 前端缓存的所有数据单例
    /// </summary>
    public sealed class ResourcesSingleton : Singleton<ResourcesSingleton>, IDisposable
    {
        public Dictionary<Type, IMessagePack> MessagePacks = new Dictionary<Type, IMessagePack>();

        public void Init()
        {
            
        }

        public bool TryGetData<T>(out T messagePack) where T : IMessagePack
        {
            var type = typeof(T);
            messagePack = default(T);
            if (MessagePacks.TryGetValue(type, out var mess))
            {
                messagePack = (T)mess;
                return true;
            }

            return false;
        }

        public void AddOrSetData<T>(T messagePack) where T : IMessagePack
        {
            var type = typeof(T);
            if (MessagePacks.ContainsKey(type))
            {
                MessagePacks[type] = messagePack;
            }
            else
            {
                MessagePacks.Add(type, messagePack);
            }
        }

        public void Clear()
        {
            MessagePacks.Clear();
        }

        public void Dispose()
        {
            Clear();
            Instance = null;
        }
    }
}