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

        public long monCardTime = 0;
        public PlayerResource playerResource;

        public void Init()
        {
        }


        public void Clear()
        {
        }

        public void Dispose()
        {
            Clear();
            Instance = null;
        }
    }
    public struct PlayerResource
    {
        public long diamond;

        public long gold;
    }
}