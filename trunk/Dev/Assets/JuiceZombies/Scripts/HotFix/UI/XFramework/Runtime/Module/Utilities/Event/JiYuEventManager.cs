using System;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    /// <summary>
    /// 事件(消息)管理
    /// </summary>
    public class JiYuEventManager : Singleton<JiYuEventManager>, IDisposable
    {
        // 创建一个字典，键是事件名，值是带有可空类型参数的 Action
        private Dictionary<string, Action<string>> eventDictionary = new Dictionary<string, Action<string>>();

        // 注册事件
        public void RegisterEvent(string eventName, Action<string> listener)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                // 如果事件已存在，添加监听器
                eventDictionary[eventName] += listener;
            }
            else
            {
                // 如果事件不存在，创建新的事件并添加监听器
                eventDictionary.Add(eventName, listener);
            }
        }

        // 注销事件
        public void UnregisterEvent(string eventName)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                // 如果事件存在，移除监听器
                //eventDictionary[eventName] -= listener;

                // 如果事件没有监听器了，则删除该事件
                // if (eventDictionary[eventName] == null)
                // {
                eventDictionary.Remove(eventName);
                //}
            }
        }

        // 触发事件
        public void TriggerEvent(string eventName, string data)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                // 如果事件存在，调用所有注册的监听器，传递可空类型参数
                eventDictionary[eventName]?.Invoke(data);
            }
            else
            {
                Log.Error($"事件未注册：{eventName}");
            }
        }

        public void RemoveAllListeners()
        {
            eventDictionary.Clear();
        }

        public void Dispose()
        {
            RemoveAllListeners();
            Instance = null;
        }
    }
}