using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Object = UnityEngine.Object;

namespace XFramework
{
    [System.Serializable]
    public class ElementData
    {
        public string Key;
        public Object Object;
    }


    [DisallowMultipleComponent]
    public class Reference : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Ҫ���ҵ�ǰ׺
        /// </summary>
        [SerializeField, HideInInspector]
        protected string findPrefix = "rd_";

        /// <summary>
        /// ����ǰ׺�����ж���
        /// </summary>
        [SerializeField, HideInInspector]
        private List<ElementData> elements = new List<ElementData>();

        /// <summary>
        /// ��ק�����ж���
        /// </summary>
        [SerializeField, HideInInspector]
        private List<ElementData> dragElements = new List<ElementData>();

        /// <summary>
        /// elements��dragElements�����ж���
        /// </summary>
        private Dictionary<string, GameObject> references = new Dictionary<string, GameObject>();

        /// <summary>
        /// references��GameObject���ϵ��������ȡ��һ�ξʹ�����
        /// </summary>
        private UnOrderMap<string, Type, Component> components = new UnOrderMap<string, Type, Component>();

        private string _name;

        private string myName => _name ??= this.name;

        /// <summary>
        /// ���Ի�ȡ��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGet<T>(out T component) where T : Component
        {
            component = Get<T>();
            return component != null;
        }

        /// <summary>
        /// ���Ի�ȡ��������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGet(Type type, out Component component)
        {
            component = Get(type);
            return component != null;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : Component
        {
            return Get(typeof(T)) as T;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Component Get(Type type)
        {
            string name = this.myName;
            if (components.TryGetValue(name, type, out Component component))
            {
                if (component) 
                    return component;
                else 
                    components.Remove(name, type);
            }

            component = base.GetComponent(type);
            if (component != null)
                components.Add(name, type, component);

            return component;
        }

        /// <summary>
        /// ���Ի�ȡ�Ӷ���
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TryGetChild(string key, out GameObject obj)
        {
            obj = GetChild(key);
            return obj != null;
        }

        /// <summary>
        /// ��ȡ�Ӷ���
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject GetChild(string key)
        {
            if (references.TryGetValue(key, out GameObject obj))
                return obj;

            return null;
        }

        /// <summary>
        /// ���Դ�ĳ���Ӷ����ϻ�ȡһ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGetChild<T>(string key, out T component) where T : Component
        {
            component = GetChild<T>(key);
            return component != null;
        }

        /// <summary>
        /// ���Դ�ĳ���Ӷ����ϻ�ȡһ�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGetChild(string key, Type type, out Component component)
        {
            component = GetChild(key, type);
            return component != null;
        }

        /// <summary>
        /// ��ĳ���Ӷ����ϻ�ȡһ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetChild<T>(string key) where T : Component
        {
            Type type = typeof(T);

            return GetChild(key, type) as T;
        }

        /// <summary>
        /// ��ĳ���Ӷ����ϻ�ȡһ�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Component GetChild(string key, Type type)
        {
            if (components.TryGetValue(key, type, out Component o))
            {
                if (o) 
                    return o;
                else 
                    components.Remove(key, type);
            }

            o = GetChild(key).GetComponent(type);
            if (o != null)
                components.Add(key, type, o);

            return o;
        }

        /// <summary>
        /// �Ƴ������¼�����
        /// </summary>
        /// <param name="type"></param>
        public void RemoveAllListeners()
        {

        }

        public virtual void OnBeforeSerialize()
        {
            
        }

        /// <summary>
        /// �����л�֮��ִ��
        /// </summary>
        public virtual void OnAfterDeserialize()
        {
            references.Clear();
            components.Clear();

            AddElements(elements);
            AddElements(dragElements);
        }

        /// <summary>
        /// ���Ԫ�ص������ֵ���
        /// </summary>
        /// <param name="list"></param>
        private void AddElements(List<ElementData> list)
        {
            foreach (ElementData item in list)
            {
                string key = item.Key;
                GameObject obj = item.Object as GameObject;

                if (obj == null)
                    continue;

                if (!references.ContainsKey(key))
                    references.Add(key, obj);
            }
        }

        private void OnDestroy()
        {
            RemoveAllListeners();
            references.Clear();
            components.Clear();
        }
    }
}
