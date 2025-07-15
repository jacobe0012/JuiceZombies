using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using System.Text;

namespace XRL
{
    /// <summary>
    /// 从模板创建文件，有如下好处：
    /// 1. 编码格式统一为UTF-8带签名
    /// 2. 代码统一格式化
    /// 3. 添加命名空间
    /// </summary>
    public class CreateClass : MonoBehaviour
    {
        [MenuItem("Assets/ECSTemplates/MonoBehaviour")]
        public static void CreateMonoBehaviour()
        {
            string templatePath = "Assets/Editor/Templates/csharp/MonoBehaviour.txt";
            Create(templatePath);
        }

        [MenuItem("Assets/ECSTemplates/Baker")]
        public static void CreateBaker()
        {
            string templatePath = "Assets/Editor/Templates/csharp/Baker.txt";
            Create(templatePath);
        }

        [MenuItem("Assets/ECSTemplates/IComponentData")]
        public static void CreateIComponentData()
        {
            string templatePath = "Assets/Editor/Templates/csharp/IComponentData.txt";
            Create(templatePath);
        }

        [MenuItem("Assets/ECSTemplates/SystemBase")]
        public static void CreateSystemBase()
        {
            string templatePath = "Assets/Editor/Templates/csharp/SystemBase.txt";
            Create(templatePath);
        }

        [MenuItem("Assets/ECSTemplates/ISystem")]
        public static void CreateISystem()
        {
            string templatePath = "Assets/Editor/Templates/csharp/ISystem.txt";
            Create(templatePath);
        }


        private static void Create(string path)
        {
            if (IsSelectedFolder())
            {
                string folderPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

                // 创建代码文件的过程中，在重命名时调用该方法
                Texture2D icon = EditorGUIUtility.IconContent("d_cs Script Icon").image as Texture2D;
                ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                    ScriptableObject.CreateInstance<CreateScriptAsset>(),
                    folderPath + "/MyNewScript.cs",
                    icon,
                    path);
            }
        }

        private static bool IsSelectedFolder()
        {
            return Path.GetExtension(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0])) == string.Empty;
        }
    }

    public class CreateScriptAsset : EndNameEditAction
    {
        // 重命名新创建的Script时，调用它
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            // 基于模板文件生成具体文件时，需替换文件中的部分内容：如类名替换为文件名
            UnityEngine.Object obj = CreateTemplateScriptAsset(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }

        private static UnityEngine.Object CreateTemplateScriptAsset(string newScriptPath, string templatePath)
        {
            // 读入模板内容
            StreamReader sr = new StreamReader(templatePath);
            string content = sr.ReadToEnd();
            sr.Close();

            // 修改内容中的部分内容
            content = content.Replace("#ScriptName#", Path.GetFileNameWithoutExtension(newScriptPath));
            content = content.Replace("#Author#", "JerryYang");
            content = content.Replace("#CreateTime#", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            content = content.Replace("#Copyright#", "Copyright © 2019-2022 XRLGame Co., Ltd. All rights reserved.");
            content = content.Replace("#Feedback#", "yang2686022430@163.com.");

            // 将修改后的内容写入新创建的文件
            StreamWriter sw = new StreamWriter(Path.GetFullPath(newScriptPath), false, new UTF8Encoding(true, false));
            sw.Write(content);
            sw.Close();

            // 导入资源
            AssetDatabase.ImportAsset(newScriptPath);
            return AssetDatabase.LoadAssetAtPath(newScriptPath, typeof(UnityEngine.Object));
        }
    }
}