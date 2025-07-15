using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DigitalOpus.MB.Core;
using DigitalOpus.MB.MBEditor;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using GPUECSAnimationBaker.Engine.Baker;
using GpuEcsAnimationBaker.Engine.Data;
using HotFix_UI;
using Main;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using TMPro;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Stateful;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Material = UnityEngine.Material;
using Object = UnityEngine.Object;


public class TextStyleChange
{
    // [Title("输出路径:(默认为存放预制件的目录)")] [FolderPath]
    // public string outputPath = "Assets/ApesGang/Prefabs/Monster/Animator2GPUAnimTempFloder";

    [Title("拖入使用到的所有预制件文件夹路径:")] [FolderPath]
    public string prefabsPath = "Assets/ApesGang/Art_Resources/UI/Prefabs";

    // private string sharedTexturePath = "Assets/ApesGang/Shaders/JiYuShaders/Texture/JiYuShaderTextureArray.png";
    // private string disloveTexturePath = "Assets/ApesGang/Shaders/JiYuShaders/Texture/seamlessNoise.png";

    [Button(name: "一键烘焙")]
    public async void Button()
    {
        string[] allPrefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabsPath });

        if (allPrefabGuids.Length < 1)
        {
            Debug.LogError($"没有要烘焙的预制件");
            return;
        }

        var asset1 = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/ApesGang/Art_Resources/UI/Fonts/youshebiaotiyuan SDF.asset");

        var asset2 = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/ApesGang/Art_Resources/UI/Fonts/SourceHanSansSC-Bold-2 SDF.asset");

        Dictionary<string, Material> type1nameToMaterial = new Dictionary<string, Material>();

        Dictionary<string, Material> type2nameToMaterial = new Dictionary<string, Material>();

        var type2Mat = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/ApesGang/Art_Resources/UI/Fonts/fonts material/SourceHanSansSC-Bold-2-all SDF.mat");
        type2nameToMaterial.Add("type2_default", type2Mat);
        var type2color000000 = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/ApesGang/Art_Resources/UI/Fonts/fonts material/SourceHanSansSC-Bold-2-yinying SDF_color_000000.mat");
        type2nameToMaterial.Add("type2_color000000", type2color000000);
        var type2color2e2924 = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/ApesGang/Art_Resources/UI/Fonts/fonts material/SourceHanSansSC-Bold-2-yinying SDF_color_2e2924.mat");
        type2nameToMaterial.Add("type2_color2e2924", type2color2e2924);
        var type2color19273b = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/ApesGang/Art_Resources/UI/Fonts/fonts material/SourceHanSansSC-Bold-2-yinying SDF_color_19273b.mat");
        type2nameToMaterial.Add("type2_color19273b", type2color19273b);
        var type1Mat = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/ApesGang/Art_Resources/UI/Fonts/fonts material/youshebiaotiyuan-all SDF.mat");
        type1nameToMaterial.Add("type1_default", type1Mat);
        var type1color000000 = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/ApesGang/Art_Resources/UI/Fonts/fonts material/youshebiaotiyuan-yinying SDF_color_000000.mat");
        type1nameToMaterial.Add("type1_color000000", type1color000000);

        await ConfigManager.Instance.InitTables();
        for (int i = 0; i < allPrefabGuids.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Replacing...", "Start replace", i);
            string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGuids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                var textMeshPros = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var textMeshPro in textMeshPros)
                {
                    var name = textMeshPro.gameObject.name;
                    if (name.Contains($"_id"))
                    {
                        Match match = Regex.Match(name, @"_id(\d+)", RegexOptions.IgnoreCase);

                        if (match.Success)
                        {
                            // match.Groups[1] refers to the content of the first capturing group (which is our \d+)
                            string numberString = match.Groups[1].Value;

                            if (int.TryParse(numberString, out int result))
                            {
                                var tbFont = ConfigManager.Instance.Tables.Tbfont.Get(result);
                                //1.优设标题圆  youshebiaotiyuan
                                //2.思源黑体    SourceHanSansSC-Bold-2

                                if (tbFont.type == 1)
                                {
                                    textMeshPro.font = asset1;
                                    if (string.IsNullOrEmpty(tbFont.proj))
                                    {
                                        textMeshPro.fontSharedMaterial = type1Mat;
                                    }
                                    else
                                    {
                                        var kv = type1nameToMaterial.Where(kv => kv.Key.Contains(tbFont.proj))
                                            .FirstOrDefault();
                                        if (kv.Value != null)
                                        {
                                            textMeshPro.fontSharedMaterial = kv.Value;
                                        }
                                        else
                                        {
                                            Debug.LogError($"type:{tbFont.type} proj:{tbFont.proj} Mat not found");
                                        }
                                    }
                                }
                                else
                                {
                                    textMeshPro.font = asset2;
                                    if (string.IsNullOrEmpty(tbFont.proj))
                                    {
                                        textMeshPro.fontSharedMaterial = type2Mat;
                                    }
                                    else
                                    {
                                        var kv = type2nameToMaterial.Where(kv => kv.Key.Contains(tbFont.proj))
                                            .FirstOrDefault();
                                        if (kv.Value != null)
                                        {
                                            textMeshPro.fontSharedMaterial = kv.Value;
                                        }
                                        else
                                        {
                                            Debug.LogError($"type:{tbFont.type} proj:{tbFont.proj} Mat not found");
                                        }
                                    }
                                }

                                if (textMeshPro.enableAutoSizing)
                                {
                                    textMeshPro.fontSizeMin = tbFont.size * 0.4f;
                                    textMeshPro.fontSizeMax = tbFont.size;
                                }
                                else
                                {
                                    textMeshPro.fontSize = tbFont.size;
                                }

                                textMeshPro.color = UnityHelper.HexRGB2Color(tbFont.color);
                            }
                        }
                    }
                }
            }

            EditorUtility.SetDirty(prefab);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        Debug.Log($"Finished");
        AssetDatabase.Refresh();
        //Object obj = AssetDatabase.LoadMainAssetAtPath(outputPath);
        //EditorGUIUtility.PingObject(obj);
    }
}