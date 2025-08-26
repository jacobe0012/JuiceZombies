using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using XFramework;

public class ReplaceImageSourceEditor : EditorWindow
{
    // 指定的文件夹路径
    private string targetFolderPath = "";
    private string sourceFolderPath = "";
    private Object targetFolderPathObj;
    private Object sourceFolderPathObj;

    [MenuItem("UnicornStudio Tools/更改所有预制件的图片引用并替换图片工具")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceImageSourceEditor>("更改图片引用&替换工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("更改所有预制件的图片引用为指定文件夹下同名图片并替换", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        targetFolderPathObj = EditorGUILayout.ObjectField("待替换文件夹", targetFolderPathObj, typeof(DefaultAsset), false);
        sourceFolderPathObj = EditorGUILayout.ObjectField("原文件夹", sourceFolderPathObj, typeof(DefaultAsset), false);


        targetFolderPath = AssetDatabase.GetAssetPath(targetFolderPathObj);
        sourceFolderPath = AssetDatabase.GetAssetPath(sourceFolderPathObj);
        //Debug.Log($"文件夹路径：{folderPath}");


        EditorGUILayout.Space();
        if (GUILayout.Button("Replace"))
        {
            ReplaceImageSources();
        }
    }

    private string[] CombineArrays(string[] array1, string[] array2)
    {
        string[] combinedArray = new string[array1.Length + array2.Length];
        array1.CopyTo(combinedArray, 0);
        array2.CopyTo(combinedArray, array1.Length);
        return combinedArray;
    }

    private void ReplaceImageSources()
    {
        if (string.IsNullOrEmpty(targetFolderPath))
        {
            Debug.LogError("Folder path is empty!");
            return;
        }

        string[] imageFiles0 = Directory.GetFiles(targetFolderPath, "*.png", SearchOption.AllDirectories);
        string[] imageFiles1 = Directory.GetFiles(targetFolderPath, "*.jpg", SearchOption.AllDirectories);
        string[] imageFiles = CombineArrays(imageFiles0, imageFiles1);

        foreach (string filePath in imageFiles)
        {
            SetTextureType(filePath);
        }

        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets" });
        var prefabs = new GameObject[prefabPaths.Length];
        EditorUtility.DisplayProgressBar("Replacing...", "Start replace", 0);
        int progress = 0;
        for (int i = 0; i < prefabPaths.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabPaths[i]);
            prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            EditorUtility.DisplayProgressBar("Replacing....", path, ((float)progress / prefabPaths.Length));

            var images = prefabs[i].GetComponentsInChildren<Image>(true);

            foreach (Image image in images)
            {
                if (image.sprite == null)
                {
                    continue;
                }

                string imageName = Path.GetFileNameWithoutExtension(image?.sprite?.texture?.name);

                foreach (string imageFile in imageFiles)
                {
                    if (Path.GetFileNameWithoutExtension(imageFile) == imageName)
                    {
                        Debug.Log($"替换预制件{prefabs[i].name}的图片引用:{imageName}");
                        //Texture2D newTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(imageFile);
                        // image.sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height),
                        //     Vector2.one * 0.5f);
                        var newSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imageFile);
                        image.sprite = newSprite;
                        break;
                    }
                }
            }

            EditorUtility.SetDirty(prefabs[i]);
        }

        AssetDatabase.SaveAssets();

        ReplaceFiles(sourceFolderPath, targetFolderPath);
        //AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();

        Debug.Log("Image sources replaced successfully!");
    }

    private void ReplaceFiles(string sourceFolderPath, string targetFolderPath)
    {
        // string[] sourceFiles0 = Directory.GetFiles(sourceFolderPath, "*.png", SearchOption.AllDirectories);
        // string[] sourceFiles1 = Directory.GetFiles(sourceFolderPath, "*.jpg", SearchOption.AllDirectories);
        // string[] sourceFiles = CombineArrays(sourceFiles0, sourceFiles1);


        string[] targetFiles0 = Directory.GetFiles(targetFolderPath, "*.png", SearchOption.AllDirectories);
        string[] targetFiles1 = Directory.GetFiles(targetFolderPath, "*.jpg", SearchOption.AllDirectories);
        string[] targetFiles = CombineArrays(targetFiles0, targetFiles1);

        foreach (var targetFile in targetFiles)
        {
            // 获取文件名
            string fileName = Path.GetFileName(targetFile);

            // 构建目标文件路径
            //string targetFilePath = Path.Combine(targetFolderPath, fileName);

            // 获取目标文件夹中的第一个同名文件（包括子文件夹）
            string[] newtargetFiles = Directory.GetFiles(sourceFolderPath, fileName, SearchOption.AllDirectories);

            if (newtargetFiles.Length > 0)
            {
                // 获取第一个同名文件路径
                string firstTargetFile = newtargetFiles[0];

                // 替换文件
                AssetDatabase.DeleteAsset(firstTargetFile.Substring(firstTargetFile.IndexOf("Assets")));

                // 替换文件
                AssetDatabase.MoveAsset(targetFile, firstTargetFile.Substring(firstTargetFile.IndexOf("Assets")));
                AssetDatabase.DeleteAsset(targetFile.Substring(firstTargetFile.IndexOf("Assets")));

                // 刷新Asset数据库，确保Unity编辑器能够识别文件变化
                AssetDatabase.Refresh();


                Debug.Log($"文件 {fileName} 已成功替换。");
            }
            else
            {
                Debug.LogWarning($"目标文件 {fileName} 不存在，无法替换。");
            }
        }
    }

    private void SetTextureType(string filePath)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(filePath) as TextureImporter;
        if (textureImporter != null)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.mipmapEnabled = false;
            AssetDatabase.ImportAsset(filePath);
        }
    }
}