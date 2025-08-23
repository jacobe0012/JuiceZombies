using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using XFramework;

public class ChangeFontInPrefabs
{
    [Title("新字体")] public TMP_FontAsset newFont; // 设置新的字体
    //private GameObject[] prefabs;

    [Title("输出路径:(默认为存放预制件的目录)")] [FolderPath]
    public string inputPath = "Assets/JuiceZombies/Art_Resources/UI/Prefabs";

    [Title("是否替换字体/给所有text组件加TMP_SpriteAsset")]
    public bool isFront; // 设置新的字体

    [Button(name: "一键替换")]
    public void Button()
    {
        //EditorGUILayout.LabelField("Select the new font:");
        //newFont = (TMP_FontAsset)EditorGUILayout.ObjectField(newFont, typeof(TMP_FontAsset), false);

        if (newFont != null)
        {
            // 获取所有的预制件
            string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new string[] { inputPath });

            var prefabs = new GameObject[prefabPaths.Length];
            EditorUtility.DisplayProgressBar("Replacing...", "Start replace", 0);
            int progress = 0;
            for (int i = 0; i < prefabPaths.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabPaths[i]);
                prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                EditorUtility.DisplayProgressBar("Replacing....", path, ((float)progress / prefabPaths.Length));

                // var textComponents = prefabs[i].GetComponentsInChildren<XTextMeshProUGUI>(true);
                //
                // foreach (var text in textComponents)
                // {
                //     // 设置Text组件的字体为新字体
                //     if (isFront)
                //     {
                //         text.font = newFont;
                //     }
                //     else
                //     {
                //         var asset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(
                //             "Assets/JuiceZombies/Art_Resources/UI/Atlas/Resources/Sprite Assets/item_atlas_tsa.asset");
                //
                //         text.spriteAsset = asset;
                //     }
                // }

                var textMeshPro = prefabs[i].GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var text in textMeshPro)
                {
                    if (isFront)
                    {
                        text.font = newFont;
                    }
                    else
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(
                            "Assets/JuiceZombies/Art_Resources/UI/Atlas/Resources/Sprite Assets/item_atlas_tsa.asset");

                        text.spriteAsset = asset;
                    }
                }

                EditorUtility.SetDirty(prefabs[i]);

                // 遍历预制件中的所有Text组件
                // TMP_Text[] textComponents = prefabs[i].GetComponentsInChildren<TMP_Text>(true);
                // foreach (TMP_Text text in textComponents)
                // {
                //     // 设置Text组件的字体为新字体
                //     text.font = newFont;
                // }
            }

            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            Debug.Log("Font changed in " + prefabs.Length + " prefabs.");
        }
        else
        {
            Debug.LogError("Please select a new font.");
        }
    }
}