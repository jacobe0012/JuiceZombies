using System;
using System.Collections.Generic;
using System.Reflection;

namespace XFramework
{
    /// <summary>
    /// ���ͼ���
    /// </summary>
    public sealed class TypesManager : Singleton<TypesManager>, IDisposable
    {
        /// <summary>
        /// ע����ĳ���
        /// </summary>
        private Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

        /// <summary>
        /// �洢���еı������ClassBaseAttribute����
        /// <para>Key : ��ǵ�Attribute</para>
        /// <para>Value : ����ǵ����Type�б�</para>
        /// </summary>
        private UnOrderMapSet<Type, Type> allTypes = new UnOrderMapSet<Type, Type>();

        /// <summary>
        /// ���м̳���IEvent(T)�ӿڵ���
        /// <para>Key : ���Type</para>
        /// <para>Value : T�����б�</para>
        /// </summary>
        private UnOrderMapSet<Type, Type> allEvents = new UnOrderMapSet<Type, Type>();

        /// <summary>
        /// ���м̳���IInternalEvent(T)�ӿڵ���
        /// <para>Key : ���Type</para>
        /// <para>Value : T�����б�</para>
        /// </summary>
        private UnOrderMapSet<Type, Type> allInternalEvents = new UnOrderMapSet<Type, Type>();

        /// <summary>
        /// ���б����ObjectSystem��
        /// </summary>
        private Dictionary<Type, UnOrderMapSet<Type, object>> allSystem =
            new Dictionary<Type, UnOrderMapSet<Type, object>>();

        /// <summary>
        /// �̳���IDestroy�ӿڵĽӿ�����
        /// </summary>
        private UnOrderMapSet<Type, Type> allSystem2 = new UnOrderMapSet<Type, Type>();

        public void Init()
        {
            Add(this.GetType().Assembly);
        }

        /// <summary>
        /// ��ӳ���
        /// </summary>
        /// <param name="ass"></param>
        public void Add(Assembly ass)
        {
            string assName = ass.GetName().Name;
            assemblies[assName] = ass;

            allTypes.Clear();
            allEvents.Clear();
            foreach (Assembly assembly in assemblies.Values)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract)
                        continue;

                    //�ҵ����б����BaseAttribute���Ե��࣬������̳е�����
                    //Ȼ��浽allType����ȥ�����������ȡ������Ե�����
                    var clasAttris = type.GetCustomAttributes(typeof(BaseAttribute), true);
                    if (clasAttris.Length > 0)
                    {
                        foreach (BaseAttribute attri in clasAttris)
                        {
                            allTypes.Add(attri.AttributeType, type);
                        }
                    }

                    var interfaces = type.GetInterfaces();
                    if (interfaces.Length > 0)
                    {
                        foreach (Type interfaceType in interfaces)
                        {
                            //Log.Info(interfaceType.Name);
                            //�˴��ǽ��̳���IEvent<T>�ӿڵ��������T�����ʹ��ڣ��������һ����ע���¼�
                            if (interfaceType.Name == "IEvent`1")
                            {
                                Type[] genericArguments = interfaceType.GetGenericArguments();
                                foreach (Type argumentType in genericArguments)
                                {
                                    allEvents.Add(type, argumentType);
                                }
                            }

                            if (interfaceType.Name == "IInternalEvent`1")
                            {
                                Type[] genericArguments = interfaceType.GetGenericArguments();
                                foreach (Type argumentType in genericArguments)
                                {
                                    allInternalEvents.Add(type, argumentType);
                                }
                            }

                            if (interfaceType.Name != nameof(IDestroy))
                            {
                                if (typeof(IDestroy).IsAssignableFrom(interfaceType))
                                {
                                    allSystem2.Add(type, interfaceType);
                                }
                            }
                        }
                    }
                }
            }

            CreateSystem();
        }

        /// <summary>
        /// ����ϵͳ��
        /// </summary>
        private void CreateSystem()
        {
            allSystem.Clear();
            var types = GetTypes(typeof(ObjectSystemAttribute));
            foreach (var type in types)
            {
                var obj = Activator.CreateInstance(type);
                if (obj is ISystemType systemType)
                {
                    if (!allSystem.TryGetValue(systemType.GetSystemType(), out var dict))
                    {
                        dict = new UnOrderMapSet<Type, object>();
                        allSystem.Add(systemType.GetSystemType(), dict);
                    }

                    if (obj is IObjectType objectType)
                    {
                        dict.Add(objectType.GetObjectType(), obj);
                    }
                }
            }
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public Assembly GetAssembly(string assemblyName)
        {
            return assemblies.Get(assemblyName);
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="result"></param>
        public void GetAssemblies(ICollection<Assembly> result)
        {
            foreach (var item in assemblies.Values)
            {
                result.Add(item);
            }
        }

        /// <summary>
        /// ��ȡ�����ָ�����Ե���
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public HashSet<Type> GetTypes(Type attributeType)
        {
            if (allTypes.TryGetValue(attributeType, out var set))
                return set;

            return new HashSet<Type>();
        }

        /// <summary>
        /// ��ȡ���м̳���IEvent(T)����T������
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public HashSet<Type> GetEvents(Type objectType)
        {
            if (allEvents.TryGetValue(objectType, out var set))
                return set;

            return new HashSet<Type>();
        }

        /// <summary>
        /// ���Ի�ȡ���м̳���IEvent(T)����T������
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetEvents(Type objectType, out HashSet<Type> result)
        {
            result = null;

            if (allEvents.TryGetValue(objectType, out var set))
                result = set;

            return result != null;
        }

        /// <summary>
        /// ���Ի�ȡ���м̳���InternalIEvent(T)����T������
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetInternalEvents(Type objectType, out HashSet<Type> result)
        {
            result = null;

            if (allInternalEvents.TryGetValue(objectType, out var set))
                result = set;

            return result != null;
        }

        /// <summary>
        /// ���Ի�ȡĳϵͳ�б�
        /// </summary>
        /// <param name="systemType"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetSystems(Type systemType, Type objType, List<object> result)
        {
            if (allSystem.TryGetValue(systemType, out var dict))
            {
                if (dict.TryGetValue(objType, out var ret))
                {
                    result.AddRange(ret);
                }

                if (allSystem2.TryGetValue(objType, out var ret1))
                {
                    foreach (var type in ret1)
                    {
                        if (dict.TryGetValue(type, out ret))
                            result.AddRange(ret);
                    }
                }
            }

            return result.Count > 0;
        }

        public void Dispose()
        {
            assemblies.Clear();
            allTypes.Clear();
            allSystem.Clear();
            allSystem2.Clear();
            Instance = null;
        }
    }
}