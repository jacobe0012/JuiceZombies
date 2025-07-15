using System;
using Newtonsoft.Json;

namespace XFramework
{
    public class XObject : IDisposable
    {
        /// <summary>
        /// ���Id���ͷź��0��ÿ�γ�ʼ����Ҳ��ı�
        /// <para>�첽����ʱ�������</para>
        /// </summary>
        [JsonIgnore]
        public long TagId { get; private set; }

        /// <summary>
        /// �Ƿ��Ѿ������ˣ�������ߵĶ��������Ͳ���׼ȷ
        /// </summary>
        [JsonIgnore]
        public bool IsDisposed => TagId == 0;

        /// <summary>
        /// �Ƿ��Ѿ���ʼ������
        /// </summary>
        [JsonIgnore] private bool isAwake = false;

        /// <summary>
        /// �Ƿ����Զ����
        /// </summary>
        [JsonIgnore] private bool isFromPool = false;

        /// <summary>
        /// �Ƿ����ù�����
        /// </summary>
        [JsonIgnore] private bool setFromPool = false;

        /// <summary>
        /// ֮ǰ�ı��Id
        /// </summary>
        [JsonIgnore] private long beforeTagId = 0;

        /// <summary>
        /// �����ͷ�ʱ
        /// </summary>
        [JsonIgnore] private Action _onDisposed;

        protected XObject()
        {
        }

        protected virtual void OnStart()
        {
            AddEvent(); //��仰д�����ԭ���ǲ�ǿ��Ԥ��ע���¼�
        }

        protected virtual void OnDestroy()
        {
        }

        internal void Awake()
        {
            if (isAwake)
                return;

            isAwake = true;
            TagId = beforeTagId + 1;
            if (TagId == 0)
                ++TagId;

            OnStart();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            isAwake = false;
            beforeTagId = TagId;
            TagId = 0;

            if (this is IEvent)
                EventManager.Instance?.RemoveTarget(this);

            var cb = _onDisposed;
            ClearOnDisposed(); //���Ƴ�����ִ��cb
            try
            {
                cb?.Invoke();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            try
            {
                OnDestroy();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (this is IDestroy o)
                EventManager.Instance?.Destroy(o);

            if (isFromPool)
            {
                ObjectPool.Instance.Recycle(this);
            }

            setFromPool = false;
        }

        /// <summary>
        /// �����Ƿ����Գ���
        /// </summary>
        /// <param name="fromPool"></param>
        internal void SetFromPool(bool fromPool)
        {
            if (setFromPool)
                return;

            setFromPool = true;
            isFromPool = fromPool;
        }

        /// <summary>
        /// ��ӱ�������м���
        /// </summary>
        protected void AddEvent()
        {
            if (this is IEvent)
                EventManager.Instance?.AddTarget(this);
        }

        /// <summary>
        /// ����ͷ�ʱ�ļ�������������<see cref="OnDestroy"/>��ִ��
        /// </summary>
        /// <param name="action"></param>
        protected void AddOnDisposed(Action action)
        {
            if (action != null)
                _onDisposed += action;
        }

        /// <summary>
        /// �Ƴ��ͷ�ʱ�ļ���
        /// </summary>
        /// <param name="action"></param>
        protected void RemoveOnDisposed(Action action)
        {
            if (action != null)
                _onDisposed -= action;
        }

        /// <summary>
        /// ����ͷ�ʱ�ļ���
        /// </summary>
        protected void ClearOnDisposed()
        {
            _onDisposed = null;
        }
    }
}