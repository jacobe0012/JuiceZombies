using System;

namespace XFramework
{
    /// <summary>
    /// �¼�(��Ϣ)����
    /// </summary>
    public class EventManager : Singleton<EventManager>, IDisposable
    {
        private UnOrderMapList<Type, object> allEvents = new UnOrderMapList<Type, object>();

        /// <summary>
        /// ��Ӽ���
        /// </summary>
        /// <param name="target"></param>
        public void AddTarget(object target)
        {
            if (!(target is IEvent))
                return;

            Type targetType = target.GetType();
            if (TypesManager.Instance.TryGetEvents(targetType, out var types))
            {
                foreach (var argType in types)
                {
                    allEvents.TryAdd(argType, target);
                }
            }
        }

        /// <summary>
        /// ��Ӽ���
        /// </summary>
        /// <param name="target"></param>
        /// <param name="argType"></param>
        public void AddListener(object target, Type argType)
        {
            if (!(target is IEvent))
                return;

            Type targetType = target.GetType();
            if (!TypesManager.Instance.TryGetEvents(targetType, out var types))
                return;

            if (types.Contains(argType))
            {
                allEvents.TryAdd(argType, target);
            }
        }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="target"></param>
        /// <param name="argType"></param>
        /// <returns></returns>
        public bool RemoveListener(object target, Type argType)
        {
            if (!(target is IEvent))
                return false;

            Type targetType = target.GetType();
            if (!TypesManager.Instance.TryGetEvents(targetType, out _))
                return false;

            return allEvents.Remove(argType, targetType);
        }

        /// <summary>
        /// �Ƴ�Ŀ�����
        /// </summary>
        /// <param name="target"></param>
        public void RemoveTarget(object target)
        {
            if (!(target is IEvent))
                return;

            Type targetType = target.GetType();
            if (TypesManager.Instance.TryGetEvents(targetType, out var types))
            {
                foreach (var type in types)
                {
                    allEvents.Remove(type, target);
                }
            }
        }

        /// <summary>
        /// �Ƴ����м���
        /// </summary>
        public void RemoveAllListeners()
        {
            allEvents.Clear();
        }

        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="args"></param>
        public void Publish<T>(T args) where T : struct
        {
            Type type = typeof(T);
            if (!allEvents.TryGetValue(type, out var list))
                return;

            using var handleList = XList<object>.Create();
            handleList.AddRange(list);

            foreach (var o in handleList)
            {
                try
                {
                    // ��ֹ��ִ���¼���ʱ��ͬʱ����Ƴ��¼������±���
                    // �����ִ�е�1���¼���ʱ��ɾ���˵�3���¼�����ô��3���¼��Ͳ��ܱ�ִ����
                    if (!list.Contains(o))
                        continue;

                    (o as IEvent<T>).HandleEvent(args);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        #region System

        /// <summary>
        /// ���ٷ���
        /// </summary>
        /// <param name="obj"></param>
        public void Destroy(IDestroy obj)
        {
            using var list = XList<object>.Create();
            if (TypesManager.Instance.TryGetSystems(typeof(IDestroySystem), obj.GetType(), list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var o = list[i];
                    try
                    {
                        ((IDestroySystem)o).Run(obj);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }

        #endregion

        public void Dispose()
        {
            RemoveAllListeners();
            Instance = null;
        }
    }
}