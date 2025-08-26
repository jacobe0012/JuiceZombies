//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public enum RedPointType
    {
        None,
        Enternal, //�����ӽڵ�
        Once, //���һ�ξ���ʧ
        //Many
    }

    public enum RedPointState
    {
        None,
        Show,
        Hide,
    }

    public class RedPoint
    {
        /// <summary>
        /// ���ؼ���(������һ�����ڵ�)
        /// </summary>
        public string key
        {
            get { return m_Key; }
        }

        /// <summary>
        /// �Լ��Ĺؼ���
        /// </summary>
        public string subKey
        {
            get { return m_SubKey; }
        }

        /// <summary>
        /// �Ƿ��Ǹ��ڵ�
        /// </summary>
        public bool isRoot
        {
            get { return m_IsRoot; }
        }

        /// <summary>
        /// �������
        /// </summary>
        public RedPointType type
        {
            get { return m_Type; }
        }

        /// <summary>
        /// ��ǰ״̬
        /// </summary>
        public RedPointState state
        {
            get { return m_State; }
        }

        /// <summary>
        /// ����
        /// </summary>
        public int data
        {
            get { return m_Data; }
        }

        /// <summary>
        /// ���ڵ�
        /// </summary>
        public RedPoint parent
        {
            get { return m_Parent; }
        }

        /// <summary>
        /// �ӽڵ�
        /// </summary>
        public List<RedPoint> children
        {
            get { return m_Children; }
        }


        public RedPoint(string key, string subKey, bool isRoot, RedPointType type)
        {
            m_Key = key;
            m_SubKey = subKey;
            m_IsRoot = isRoot;
            m_Type = type;
            m_State = RedPointState.Hide;
            m_Data = 0;
            m_Children = new List<RedPoint>();
        }

        public void Init(Action<RedPointState, int> showEvent, XButtonComponent btn)
        {
            m_ShowEvent = showEvent;

            if (btn != null)
            {
                m_Btn = btn;
                m_Btn.OnClick.AddListener(OnClick);
            }

            m_ShowEvent?.Invoke(m_State, m_Data);
        }

        public void AddChild(RedPoint node, string parentKey)
        {
            if (m_SubKey.Equals(parentKey))
            {
                node.SetParent(this);
                m_Children.Add(node);
                return;
            }

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].AddChild(node, parentKey);
            }
        }

        public RedPoint GetChild(string subKey)
        {
            //Debug.LogError("subKey :" + subKey);
            if (m_SubKey.Equals(subKey))
            {
                return this;
            }

            if (m_Children == null)
            {
                return null;
            }

            for (int i = 0; i < m_Children.Count; i++)
            {
                RedPoint node = m_Children[i].GetChild(subKey);

                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }

        public void RemoveChild(string subKey)
        {
            if (m_SubKey.Equals(subKey))
            {
                m_Parent.children.Remove(this);
                Dispose();
                return;
            }

            if (m_Children == null)
            {
                return;
            }

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].RemoveChild(subKey);
            }
        }

        public void SetParent(RedPoint parent)
        {
            m_Parent = parent;
        }

        public void SetState(string subKey, RedPointState state, int data)
        {
            RedPoint node = GetChild(subKey);

            #region ����

            //Debug.Log("set subkey " + subKey + " state to " + state.ToString());

            #endregion

            if (node == null)
            {
                return;
            }

            node.SetTreeState(subKey, state, data);

            m_Data = 0;

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Data += m_Children[i].m_Data;
            }

            m_ShowEvent?.Invoke(m_State, m_Data);
        }

        private void SetTreeState(string subKey, RedPointState state, int data)
        {
            m_State = state;

            if (m_SubKey.Equals(subKey))
            {
                m_Data = data;
            }
            else
            {
                m_Data = 0;

                for (int i = 0; i < m_Children.Count; i++)
                {
                    if (m_Children[i].state == RedPointState.Show)
                    {
                        m_State = RedPointState.Show;
                        m_Data += m_Children[i].data;
                    }
                }
            }

            if (m_Parent != null)
            {
                m_Parent.SetTreeState(subKey, state, data);
            }

            m_ShowEvent?.Invoke(m_State, m_Data);
        }

        private void OnClick()
        {
            if (m_Type == RedPointType.Once)
            {
                HideChildren();
                SetState(m_SubKey, RedPointState.Hide, m_Data);
            }
            //if (m_Type == RedPointType.Many)
            //{
            //    Debug.Log("m_Data1 : " + m_Data.ToString());
            //    m_Data -= 1;
            //    Debug.Log("m_Data2 : " + m_Data.ToString());
            //    SetState(m_SubKey, RedPointState.Show, m_Data); 
            //    Debug.Log("m_Data3 : " + m_Data.ToString());
            //    if (m_Data <= 0)
            //    {
            //        HideChildren();
            //        SetState(m_SubKey, RedPointState.Hide, m_Data);
            //    }
            //}
        }

        private void HideChildren()
        {
            m_State = RedPointState.Hide;

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].HideChildren();
            }

            m_ShowEvent?.Invoke(m_State, m_Data);
        }

        #region ���Դ�ӡ��

        public void PrintTree(int level = 0)
        {
            Debug.Log(new string(' ', level * 5) + level + ": " + m_SubKey + " " + m_State.ToString());


            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].PrintTree(level + 1);
            }
        }

        #endregion

        public void Dispose()
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].Dispose();
            }

            m_Children.Clear();
            m_Children = null;

            if (m_Btn != null)
            {
                //m_Btn.OnClick.RemoveListener(OnClick);
            }

            m_Btn = null;
            m_Parent = null;
            m_Key = null;
            m_SubKey = null;
            m_ShowEvent = null;
            m_Type = RedPointType.None;
            m_State = RedPointState.None;
        }

        private string m_Key = string.Empty;
        private string m_SubKey = string.Empty;
        private bool m_IsRoot = false;
        private int m_Data = 0;
        private RedPointType m_Type = RedPointType.None;
        private RedPointState m_State = RedPointState.None;
        private Action<RedPointState, int> m_ShowEvent = null;
        private XButtonComponent m_Btn;
        private RedPoint m_Parent = null;
        private List<RedPoint> m_Children = null;
    }

    public class RedPointMgr : IDisposable
    {
        public static RedPointMgr instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new RedPointMgr();
                }

                return s_Instance;
            }
        }

        public RedPointMgr()
        {
            m_ListRedPointTrees = new List<RedPoint>();
        }

        public void Add(string key, string subKey, string parentKey, RedPointType type)
        {
            RedPoint root = GetRoot(key);

            if (string.IsNullOrEmpty(subKey) || key.Equals(subKey))
            {
                if (root != null)
                {
                    Debug.LogError("The red point root [" + key + "] is already exist!");
                    return;
                }

                root = new RedPoint(key, key, true, type);
                m_ListRedPointTrees.Add(root);
            }
            else
            {
                if (root == null)
                {
                    Debug.LogError("The red point root [" + key + "] is invalid,please add it first");
                    return;
                }

                RedPoint node = new RedPoint(key, subKey, false, type);
                root.AddChild(node, parentKey);
            }
        }

        public void Remove(string key, string subKey)
        {
            if (string.IsNullOrEmpty(subKey) || key.Equals(subKey))
            {
                for (int i = m_ListRedPointTrees.Count - 1; i >= 0; i--)
                {
                    if (m_ListRedPointTrees[i].key.Equals(key))
                    {
                        m_ListRedPointTrees[i].Dispose();
                        m_ListRedPointTrees.RemoveAt(i);
                        return;
                    }
                }

                return;
            }

            RedPoint root = GetRoot(key);

            if (root == null)
            {
                return;
            }

            root.RemoveChild(subKey);
        }

        public void Init(string key, string subKey, Action<RedPointState, int> showEvent, XButtonComponent btn = null)
        {
            RedPoint root = GetRoot(key);

            if (root == null)
            {
                Debug.LogError("The red point root [" + key + "] is invalid,please add it first");
                return;
            }

            RedPoint node = root.GetChild(subKey);

            if (node == null)
            {
                Debug.LogError("The red point node [" + subKey + "] is invalid,please add it first");
                return;
            }

            node.Init(showEvent, btn);
        }

        public void SetState(string key, string subKey, RedPointState state, int data = 0)
        {
            RedPoint root = GetRoot(key);

            if (root == null)
            {
                Debug.LogError("The red point root [" + key + "] is invalid,please add it first");
                return;
            }

            root.SetState(subKey, state, data);
        }

        #region ���Դ�ӡ��

        public void PrintRedPointTree(string key, string subKey)
        {
            RedPoint root = GetRoot(key);

            if (root == null)
            {
                Debug.LogError("The red point root [" + key + "] is invalid,please add it first");
                return;
            }

            root.PrintTree();
        }

        #endregion


        private RedPoint GetRoot(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            for (int i = 0; i < m_ListRedPointTrees.Count; i++)
            {
                if (m_ListRedPointTrees[i].key.Equals(key))
                {
                    return m_ListRedPointTrees[i];
                }
            }

            return null;
        }

        public void Dispose()
        {
            for (int i = m_ListRedPointTrees.Count - 1; i >= 0; i--)
            {
                m_ListRedPointTrees[i].Dispose();
            }

            m_ListRedPointTrees.Clear();
        }

        #region �ƽ������

        //public void Set

        #endregion

        private static RedPointMgr s_Instance = null;
        private List<RedPoint> m_ListRedPointTrees = null;
    }


    [UIEvent(UIType.UIRedPointT)]
    internal sealed class UIRedPointTEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIRedPointT;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIRedPointT>();
        }
    }

    public partial class UIRedPointT : UI, IAwake
    {
        public int count1 = 2;
        public int count2 = 2;
        public int count3 = 3;

        public void Initialize()
        {
            //���������
            RedPointMgr.instance.Add(KButton1, null, null, RedPointType.Enternal);
            RedPointMgr.instance.Add(KButton1, KButton2, KButton1, RedPointType.Enternal);
            RedPointMgr.instance.Add(KButton1, KButton3, KButton2, RedPointType.Enternal);
            RedPointMgr.instance.Add(KButton1, KButton4, KButton2, RedPointType.Enternal);
            RedPointMgr.instance.Add(KButton1, KButton5, KButton3, RedPointType.Once);
            RedPointMgr.instance.Add(KButton1, KButton6, KButton4, RedPointType.Once);
            RedPointMgr.instance.Add(KButton1, KButton7, KButton4, RedPointType.Once);

            //��ʼ��
            RedPointMgr.instance.Init(KButton1, KButton1, OnButton1Show);
            RedPointMgr.instance.Init(KButton1, KButton2, OnButton2Show);
            RedPointMgr.instance.Init(KButton1, KButton3, OnButton3Show);
            RedPointMgr.instance.Init(KButton1, KButton4, OnButton4Show);
            RedPointMgr.instance.Init(KButton1, KButton5, OnButton5Show, this.GetFromReference(KButton5).GetXButton());
            RedPointMgr.instance.Init(KButton1, KButton6, OnButton6Show, this.GetFromReference(KButton6).GetXButton());
            RedPointMgr.instance.Init(KButton1, KButton7, OnButton7Show, this.GetFromReference(KButton7).GetXButton());

            //��Ӻ��
            this.GetFromReference(KSetButton5).GetButton().OnClick.AddListener(OnSB5Click);
            this.GetFromReference(KSetButton6).GetButton().OnClick.AddListener(OnSB6Click);
            this.GetFromReference(KSetButton7).GetButton().OnClick.AddListener(OnSB7Click);
        }

        private void OnButton1Show(RedPointState state, int data)
        {
            this.GetFromReference(KB1RP).SetActive(state == RedPointState.Show);
            Debug.Log("K1BP");
        }

        private void OnButton2Show(RedPointState state, int data)
        {
            this.GetFromReference(KB2RP).SetActive(state == RedPointState.Show);
            Debug.Log("K2BP");
        }

        private void OnButton3Show(RedPointState state, int data)
        {
            this.GetFromReference(KB3RP).SetActive(state == RedPointState.Show);
            Debug.Log("K3BP");
        }

        private void OnButton4Show(RedPointState state, int data)
        {
            this.GetFromReference(KB4RP).SetActive(state == RedPointState.Show);
            Debug.Log("K4BP");
        }

        private void OnButton5Show(RedPointState state, int data)
        {
            this.GetFromReference(KB5RP).SetActive(state == RedPointState.Show);
            Debug.Log("K5BP");
        }

        private void OnButton6Show(RedPointState state, int data)
        {
            this.GetFromReference(KB6RP).SetActive(state == RedPointState.Show);
            Debug.Log("K6BP");
        }

        private void OnButton7Show(RedPointState state, int data)
        {
            this.GetFromReference(KB7RP).SetActive(state == RedPointState.Show);
            Debug.Log("K7BP");
        }

        private void OnSB5Click()
        {
            RedPointMgr.instance.SetState(KButton1, KButton5, count1 == 0 ? RedPointState.Hide : RedPointState.Show,
                count1);
        }

        private void OnSB6Click()
        {
            RedPointMgr.instance.SetState(KButton1, KButton6, count2 == 0 ? RedPointState.Hide : RedPointState.Show,
                count2);
        }

        private void OnSB7Click()
        {
            RedPointMgr.instance.SetState(KButton1, KButton7, count3 == 0 ? RedPointState.Hide : RedPointState.Show,
                count3);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}