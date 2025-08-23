using System;
using HotFix_UI;

namespace XFramework
{
    public class Game : Singleton<Game>, IDisposable
    {
        private IEntry entry;

        public void Start()
        {
            entry = new DemoEntry(); //可以实现其他entry
            entry?.Start();
        }

        public void Update()
        {
            entry?.Update();
        }

        public void LateUpdate()
        {
            entry?.LateUpdate();
        }

        public void FixedUpdate()
        {
            entry?.FixedUpdate();
        }

        public void Restart()
        {
            // entry?.Dispose();
            // Start();
            ResourcesSingleton.Instance.FromRunTimeScene = false;

            var global = Common.Instance.Get<Global>();
            global.SetResolution();

            NetWorkManager.Instance.Close();
            NetWorkManager.Instance.Init();

            ResourcesSingleton.Instance.Clear();
            ResourcesSingleton.Instance.Init();

            RedDotManager.Instance.Clear();
            WebMessageHandlerOld.Instance.Clear();
            WebMsgHandler.Instance.Clear();
        }

        public void Dispose()
        {
            entry?.Dispose();
            Instance = null;
        }
    }
}