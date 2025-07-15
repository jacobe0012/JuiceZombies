namespace XFramework
{
    public sealed class MiniTweenManager : CommonObject, IAwake, IAwake<ITimeNow>, IUpdate
    {
        /// <summary>
        /// tween�ṩ��
        /// </summary>
        private MiniTweenProvider tweenProvider;

        private static MiniTweenManager _instance;

        public static MiniTweenManager Instance => _instance;

        void IAwake.Initialize()
        {
            (this as IAwake<ITimeNow>).Initialize(new ActualTime());
        }

        void IAwake<ITimeNow>.Initialize(ITimeNow timeNow)
        {
            tweenProvider = new MiniTweenProvider(timeNow);
        }

        protected override void Init()
        {
            base.Init();
            _instance = this;
        }

        protected override void Destroy()
        {
            _instance = null;
            tweenProvider?.Dispose();
            tweenProvider = null;
        }

        void IUpdate.Update()
        {
            tweenProvider?.Update();
        }

        public MiniTween Get(long id)
        {
            return tweenProvider?.Get(id);
        }

        public MiniTween<T> Get<T>(long id)
        {
            return tweenProvider?.Get<T>(id);
        }

        /// <summary>
        /// ִ�ж���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">�����ĸ���</param>
        /// <param name="startValue">��ʼֵ</param>
        /// <param name="endValue">Ŀ��ֵ</param>
        /// <param name="arg">����</param>
        /// <param name="tweenMode">����ģʽ</param>
        /// <returns></returns>
        public MiniTween<T> To<T>(XObject parent, T startValue, T endValue, float arg, MiniTweenMode tweenMode)
        {
            return tweenProvider.To(parent, startValue, endValue, arg, tweenMode);
        }

        /// <summary>
        /// �Գ���ʱ��ģʽ��ִ�ж���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">�����ĸ���</param>
        /// <param name="startValue">��ʼֵ</param>
        /// <param name="endValue">Ŀ��ֵ</param>
        /// <param name="duration">����ʱ��</param>
        /// <returns></returns>
        public MiniTween<T> To<T>(XObject parent, T startValue, T endValue, float duration)
        {
            return tweenProvider.To(parent, startValue, endValue, duration, MiniTweenMode.Duration);
        }

        /// <summary>
        /// �Գ���ʱ��ģʽ��ִ�ж���, startValueΪĬ��ֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">�����ĸ���</param>
        /// <param name="endValue">Ŀ��ֵ</param>
        /// <param name="duration">����ʱ��</param>
        /// <returns></returns>
        public MiniTween<T> To<T>(XObject parent, T endValue, float duration)
        {
            return tweenProvider.To(parent, default, endValue, duration);
        }
    }
}