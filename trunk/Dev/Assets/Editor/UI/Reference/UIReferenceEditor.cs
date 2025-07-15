using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using System;
using UnityEditor;
using System.IO;
using System.Text;

namespace XFramework.Editor
{
    [CustomEditor(typeof(UIReference))]
    public class UIReferenceEditor : ReferenceEditor
    {
        private enum ResourceType
        {
            Resources,
            AA,
        }

        private RectTransform rectTransform;

        private UIReferenceConfig config;

        ///// <summary>
        ///// ����ģ���ļ�·��
        ///// </summary>
        //private const string TemplateFilePath = "./Template/UICodeTemplate.txt";

        ///// <summary>
        ///// UI���ϵ�·��
        ///// </summary>
        //private const string SetFilePath = "Assets/Scripts/XFramework/Runtime/Module/UI/UISet.cs";

        ///// <summary>
        ///// UIType���·��
        ///// </summary>
        //private const string UITypeFilePath = "Assets/Scripts/XFramework/Runtime/Module/UI/UIType.cs";

        ///// <summary>
        ///// UI���Ŀ¼
        ///// </summary>
        //private const string ClassExploer = "Assets/Scripts/XFramework/Runtime/World/Game/UI";

        private const string DefaultNamespace = "XFramework";

        private const string TextDataName = "_textData";

        /// <summary>
        /// �����ļ�·��
        /// </summary>
        private const string ConfigPath = "Assets/Editor/UI/Reference/Config/UIReferenceSetting.asset";

        /// <summary>
        /// UIType��ߵĲ���
        /// </summary>
        private const string UITypePrefix = "public const string (className) =";

        /// <summary>
        /// UIType��ֵ
        /// </summary>
        private const string UITypeValue = "nameof((NAMESPACE)(className));";

        private ResourceType resourceType = ResourceType.AA;

        /// <summary>
        /// ������������ռ�����
        /// </summary>
        private string configNamespace;

        /// <summary>
        /// ����������ռ�����
        /// </summary>
        private string configNamespaceAndPoint;

        /// <summary>
        /// using
        /// </summary>
        private string usingContent;

        /// <summary>
        /// �۵������Զ���
        /// </summary>
        private static bool foldoutMultiObject = true;

        private HashSet<string> multiObjectKeys = new HashSet<string>();

        protected override void OnEnable()
        {
            base.OnEnable();
            rectTransform = (target as UIReference).Get<RectTransform>();

            var configSetting = AssetDatabase.LoadAssetAtPath<UIReferenceSetting>(ConfigPath);
            if (configSetting != null)
                config = configSetting.Config;

            if (config != null)
            {
                configNamespace = config.Namespace.Trim();
                configNamespaceAndPoint = config.Namespace.Trim();
                if (!configNamespace.IsNullOrEmpty())
                    configNamespaceAndPoint += ".";

                if (configNamespace != DefaultNamespace)
                    usingContent = $"\nusing {DefaultNamespace};";
                else
                    usingContent = string.Empty;
            }                
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (config == null || !rectTransform)
                return;

            EditorGUILayout.Space();

            DisplaySingleElements("Multilingual Elements", TextDataName, ref foldoutMultiObject, multiObjectKeys);

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("����Key", GUILayout.MinHeight(45) /*"Export Keys"*/))
            {
                ExportKeys();
            }

            if (GUILayout.Button("ˢ��\n�����Զ���", GUILayout.MinHeight(45)))
            {
                RefreshAllIMultilingual();
            }

            if (GUILayout.Button("��������\n(����UI)", GUILayout.MinHeight(45) /*"Export All (AllowManagement)"*/))
            {
                ExportAll(true);
            }

            if (GUILayout.Button("��������\n(����UI)", GUILayout.MinHeight(45) /*"Export All (Common)"*/))
            {
                ExportAll(false);
            }

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        /// <summary>
        /// �������е�key
        /// </summary>
        private void ExportKeys()
        {
            string folderPath = GetFolderPath();
            if (folderPath.IsNullOrEmpty())
            {
                Debug.LogError("����Ŀ¼Ϊ�գ�");
                return;
            }

            string className = GetClassName();
            ExportKeys(className, folderPath);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// ����UI����
        /// </summary>
        private void ExportAll(bool allowManagement)
        {
            if (Application.isPlaying)
                return;

            string folderPath = GetFolderPath();
            if (folderPath.IsNullOrEmpty())
            {
                Debug.LogError("����Ŀ¼Ϊ�գ�");
                return;
            }

            string className = GetClassName();

            ExportCode(className, folderPath, allowManagement);
            ExportKeys(className, folderPath);
            ExportUIType(className);
            ExportUIPathSet(className);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="className"></param>
        /// <param name="folderPath"></param>
        /// <param name="allowManagement"></param>
        private void ExportCode(string className, string folderPath, bool allowManagement)
        {
            string fileName = $"{className}.cs";
            string classFilePath = $"{folderPath}/{fileName}";

            if (File.Exists(classFilePath))
            {
                Debug.LogError($"{fileName}�ļ��Ѿ�����\n·��Ϊ{classFilePath}");
                return;
            }

            string templateFilePath = config.CodeTemplateFilePath;

            if (!File.Exists(templateFilePath))
            {
                Debug.LogError($"����Code����ʱ�ļ�������, ����·��Ϊ{templateFilePath}");
                return;
            }

            string template = File.ReadAllText(templateFilePath);
            template = template.Replace("(className)", className)
                .Replace("(UITypeName)", className)
                .Replace("(NAMESPACE)", configNamespace)
                .Replace("(USING)", usingContent).Replace("#CreateTime#", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            
            
            #region nodeContent

            string prefix = "K";
            StringBuilder sb = new StringBuilder();
            int index = 0;
            //var KScrollView_item = GetFromReference(UIPanel_Main.KScrollView_item);
            foreach (var key in prefixKeys)
            {
                string content = $"\t\t\tvar {prefix}{key} = GetFromReference({className}.{prefix}{key});";
                if (index == 0)
                    sb.Append(content);
                else
                    sb.Append($"\n{content}");

                ++index;
            }

            foreach (var key in dragKeys)
            {
                string content = $"\t\t\tvar {prefix}{key} = GetFromReference({className}.{prefix}{key});";
                if (index == 0)
                    sb.Append(content);
                else
                    sb.Append($"\n{content}");

                ++index;
            }

            template = template.Replace("(nodeContent)", sb.ToString());

            #endregion
            if (allowManagement)
            {
                template = template.Replace("(IUILayer)", ", IUILayer").Replace("(AllowManagement)", allowManagement.ToString().ToLower());
                template = template.Replace("(IUILayerCode)", "public UILayer Layer => UILayer.Low;");
            }
            else
            {
                template = template.Replace("(IUILayer)", string.Empty).Replace("(AllowManagement)", allowManagement.ToString().ToLower());
                template = template.Replace("(IUILayerCode)", "// ��UI�ǲ���UIManager�����");
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.WriteAllText(classFilePath, template);

            Debug.Log($"{fileName}���ɳɹ�\n·��Ϊ{classFilePath}");
        }

        /// <summary>
        /// �������е�Key
        /// </summary>
        private void ExportKeys(string className, string folderPath)
        {
            string templateFilePath = config.KeyTemplateFilePath;
            
            if (!File.Exists(templateFilePath))
            {
                Debug.LogError($"����Key����ʱ�ļ�������, ����·��Ϊ{templateFilePath}");
                return;
            }

            string template = File.ReadAllText(templateFilePath);
            string prefix = "K";
            StringBuilder sb = new StringBuilder();
            int index = 0;

            foreach (var key in prefixKeys)
            {
                string content = $"\t\tpublic const string {prefix}{key} = \"{key}\";";
                if (index == 0)
                    sb.Append(content);
                else
                    sb.Append($"\n{content}");

                ++index;
            }
            foreach (var key in dragKeys)
            {
                string content = $"\t\tpublic const string {prefix}{key} = \"{key}\";";
                if (index == 0)
                    sb.Append(content);
                else
                    sb.Append($"\n{content}");

                ++index;
            }

            template = template.Replace("(className)", className).Replace("(content)", sb.ToString()).Replace("(NAMESPACE)", configNamespace);

            folderPath = $"{folderPath}/Keys";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"{className}Keys.cs";
            string classFilePath = $"{folderPath}/{fileName}";
            File.WriteAllText(classFilePath, template);

            Debug.Log($"{fileName}���ɳɹ�\n·��Ϊ{classFilePath}");
        }

        /// <summary>
        /// ������UIType
        /// </summary>
        /// <param name="className"></param>
        private void ExportUIType(string className)
        {
            string filePath = config.UITypeFilePath;

            if (!File.Exists(filePath))
            {
                Debug.LogError($"UIType.cs�����ڣ�����·��Ϊ{filePath}");
                return;
            }

            string flag = "#endregion 1";
            List<string> lines = new List<string>(File.ReadAllLines(filePath));
            string prefix = UITypePrefix.Replace("(className)", className);
            string content = $"\t\t{prefix} {UITypeValue.Replace("(className)", className).Replace("(NAMESPACE)", configNamespaceAndPoint)}";
            int nameIndex = lines.FindIndex(str => str.Contains(prefix));

            if (nameIndex >= 0)
            {
                lines[nameIndex] = content;
            }
            else
            {
                int flagIndex = lines.FindIndex(str => str.Trim() == flag);
                if (flagIndex >= 0)
                {
                    int inex = flagIndex;
                    lines.Insert(flagIndex, content);
                    lines.Insert(flagIndex + 1, string.Empty);
                }
                else
                {
                    Debug.LogError($"UIType.cs��û���ҵ����{flag}");
                    return;
                }
            }

            File.WriteAllLines(filePath, lines);
            Debug.Log($"{className}����ӵ�UIType.cs��");
        }

        /// <summary>
        /// ������UIPathSet
        /// </summary>
        /// <param name="className"></param>
        private void ExportUIPathSet(string className)
        {
            string filePath = config.UIPathSetFilePath;

            if (!File.Exists(filePath))
            {
                Debug.LogError($"UIPathSet.cs�����ڣ�����·��Ϊ{filePath}");
                return;
            }

            string flag = "#endregion 1";
            string assetPath = GetAssetPath();
            string name = rectTransform.name;
            List<string> lines = new List<string>(File.ReadAllLines(filePath));
            string prefix = $"public const string {className} =";
            string content = $"\t\t{prefix} \"{name}\";";
            int nameIndex = lines.FindIndex(str => str.Contains(prefix));

            if (nameIndex >= 0)
            {
                lines[nameIndex] = content;
            }
            else
            {
                int flagIndex = lines.FindIndex(str => str.Trim() == flag);
                if (flagIndex >= 0)
                {
                    int inex = flagIndex;
                    lines.Insert(flagIndex, content);
                    lines.Insert(flagIndex + 1, string.Empty);
                }
                else
                {
                    Debug.LogError($"UIPathSet.cs��û���ҵ����{flag}");
                    return;
                }
            }

            File.WriteAllLines(filePath, lines);
            Debug.Log($"{className}����ӵ�UIPathSet.cs��");
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns></returns>
        private string GetClassName()
        {
            string name = rectTransform.name;
            string className = name;

            if (className.EndsWith("panel", true, null))
                className = className.Remove(className.Length - 5);

            if (className.EndsWith("ui", true, null))
                className = className.Remove(className.Length - 2);

            if (className.StartsWith("ui", true, null))
                className = className.Remove(0, 2);

            className = $"UI{className}";

            return className;
        }

        /// <summary>
        /// Ŀ¼·��
        /// </summary>
        /// <returns></returns>
        private string GetFolderPath()
        {
            string path = config?.ClassExploer;
            if (path.IsNullOrEmpty())
                return null;

            string className = GetClassName();
            path = $"{path}/{className}";
            return path;
        }

        /// <summary>
        /// ��Դ·��
        /// </summary>
        /// <returns></returns>
        private string GetAssetPath()
        {
            GameObject obj = rectTransform?.gameObject;
            if (!obj)
                return string.Empty;

            string path = AssetDatabase.GetAssetPath(obj);
            if (path.IsNullOrEmpty())
                return string.Empty;

            switch (resourceType)
            {
                case ResourceType.Resources:
                    {
                        if (path.StartsWith("Assets/", true, null))
                        {
                            path = path.Substring(7);
                        }

                        if (path.StartsWith("Resources/"))
                        {
                            path = path.Substring(10);
                        }

                        if (path.EndsWith(".prefab", true, null))
                        {
                            path = path.Substring(0, path.Length - 7);
                        }

                        return path;
                    }
                case ResourceType.AA:
                    {
                        path = Path.GetFileNameWithoutExtension(path);
                        return path;
                    }
                default:
                    return path;
            }
        }

        #region Language

        /// <summary>
        /// ˢ�¸ö��������еļ̳���IMultilingual���������ӵ�UIReference��
        /// </summary>
        private void RefreshAllIMultilingual()
        {
            UnityEditor.SerializedProperty dataProperty = serializedObject.FindProperty(TextDataName);
            dataProperty.ClearArray();

            List<IMultilingual> texts = new List<IMultilingual>();
            rectTransform.GetComponentsInChildren<IMultilingual>(true, texts);
            Dictionary<string, int> keys = new Dictionary<string, int>();
            foreach (IMultilingual txt in texts)
            {
                if (!(txt is Object obj))
                    continue;

                if (txt.IgnoreLanguage)
                    continue;

                string key = obj.name;
                string newKey = key;

                keys.TryGetValue(key, out int count);
                if (count > 0)
                    newKey += count.ToString();

                keys[key] = ++count;

                AddReference(dataProperty, newKey, obj);
            }

            UnityEditor.EditorUtility.SetDirty(this);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        /// <summary>
        /// ��ӵ�������
        /// </summary>
        /// <param name="dataProperty"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        private void AddReference(SerializedProperty dataProperty, string key, Object obj)
        {
            int index = dataProperty.arraySize;
            dataProperty.InsertArrayElementAtIndex(index);
            var element = dataProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Key").stringValue = key;
            element.FindPropertyRelative("Object").objectReferenceValue = obj;
        }

        #endregion
    }
}
#endif