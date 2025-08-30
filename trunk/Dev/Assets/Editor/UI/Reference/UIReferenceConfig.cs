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
        /// ??????????
        /// </summary>
        [Header("??????????")] [TextArea] public string ClassExploer = "Assets/JuiceZombies/Scripts/HotFix/UI/XFDemo/Game/UI";

        /// <summary>
        /// ??????????·??
        /// </summary>
        [Header("??????????·??")] [TextArea] public string CodeTemplateFilePath = "./Template/UICodeTemplate.txt";

        /// <summary>
        /// key????????·??
        /// </summary>
        [Header("key????????·??")] [TextArea] public string KeyTemplateFilePath = "./Template/UIReferenceKeyTemplate.txt";

        /// <summary>
        /// UI·?????????·??
        /// </summary>
        [Header("UI·?????????·??")] [TextArea]
        public string UIPathSetFilePath = "Assets/JuiceZombies/Scripts/HotFix/UI/XFDemo/Game/UI/Types/UIPathSet.cs";

        /// <summary>
        /// UIType???·??
        /// </summary>
        [Header("UIType???·??")] [TextArea]
        public string UITypeFilePath = "Assets/JuiceZombies/Scripts/HotFix/UI/XFDemo/Game/UI/Types/UIType.cs";

        /// <summary>
        /// ???????
        /// </summary>
        [Header("???????")] public string Namespace = "XFramework";
    }
}
#endif