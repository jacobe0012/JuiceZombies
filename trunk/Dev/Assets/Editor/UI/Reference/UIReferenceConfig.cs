using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace XFramework.Editor
{
    [CreateAssetMenu(menuName = "UIReference/Config", fileName = "UIReferenceConfig")]
    public class UIReferenceConfig : ScriptableObject
    {
        /// <summary>
        /// ���ɵ����Ŀ¼
        /// </summary>
        [Header("���ɵ����Ŀ¼")] [TextArea] public string ClassExploer = "Assets/JuiceZombies/Scripts/HotFix/UI/XFDemo/Game/UI";

        /// <summary>
        /// ����ģ���ļ�·��
        /// </summary>
        [Header("����ģ���ļ�·��")] [TextArea] public string CodeTemplateFilePath = "./Template/UICodeTemplate.txt";

        /// <summary>
        /// key��ģ���ļ�·��
        /// </summary>
        [Header("key��ģ���ļ�·��")] [TextArea] public string KeyTemplateFilePath = "./Template/UIReferenceKeyTemplate.txt";

        /// <summary>
        /// UI·���������·��
        /// </summary>
        [Header("UI·���������·��")] [TextArea]
        public string UIPathSetFilePath = "Assets/JuiceZombies/Scripts/HotFix/UI/XFDemo/Game/UI/Types/UIPathSet.cs";

        /// <summary>
        /// UIType���·��
        /// </summary>
        [Header("UIType���·��")] [TextArea]
        public string UITypeFilePath = "Assets/JuiceZombies/Scripts/HotFix/UI/XFDemo/Game/UI/Types/UIType.cs";

        /// <summary>
        /// �����ռ�
        /// </summary>
        [Header("�����ռ�")] public string Namespace = "XFramework";
    }
}
#endif