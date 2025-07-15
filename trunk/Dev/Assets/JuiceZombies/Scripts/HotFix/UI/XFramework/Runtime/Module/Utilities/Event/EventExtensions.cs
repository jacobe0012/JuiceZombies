namespace XFramework
{
    public static class EventExtensions
    {
        /// <summary>
        /// ע��һ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        public static void Register<T>(this IEvent<T> self) where T : struct
        {
            EventManager.Instance?.AddListener(self, typeof(T));
        }

        /// <summary>
        /// �Ƴ�һ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        public static void UnRegister<T>(this IEvent<T> self) where T : struct
        {
            EventManager.Instance?.RemoveListener(self, typeof(T));
        }
    }
}