using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using XFramework;

public class TextureCleaner
{
    //[Title("非透明像素最大宽高/像素")] public int littleMaxWidth = 256 - 40; // 5x5 grid
    //[Title("是否逆时针旋转")] public bool isLeftRotate = false; // 5x5 grid
    [Title("拖入使用到的所有预制件文件夹路径:")] [FolderPath]
    public string prefabsPath = "Assets/ApesGang/Art_Resources/UI/Prefabs";

    [Title("拖入待清理的图片文件夹路径:")] [FolderPath] public string texturesPath = "Assets/ApesGang/Art_Resources/UI/Textures";

    [Title("拖入白名单文本文件夹路径:")] [FolderPath] public string cleanListText = "Assets/ApesGang/Art_Resources/PicsWhiteList";

    [Title("拖入白名单图片文件夹路径:")] [FolderPath]
    public string cleanListPic = "Assets/ApesGang/Art_Resources/UI/Textures/commonNew";

    private string deleteFloder = "Assets/ApesGang/Art_Resources/UI/Textures/DeleteFloder";

    private string configPicFloder = "Assets/ApesGang/Art_Resources/UI/Textures/configPicFloder";
    // [Title("输出路径:(默认为存放预制件的目录)")] [FolderPath]
    // public string outputPath = "Assets/FramePicTempFloder";

    //[Title("预烘焙的预制件列表:")] public List<GameObject> GameObjects;


    [Button(name: "一键转换")]
    public void Button()
    {
        FindAndCleanImages();
        // if (outputPath == "")
        // {
        //     outputPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(skeletonDataAsset))
        //         .Replace('\\', '/') + "/Baked";
        //     System.IO.Directory.CreateDirectory(outputPath);
        // }
        // else
        // {
        // }

        //System.IO.Directory.CreateDirectory(outputPath);

        // string[] imagePaths = AssetDatabase.FindAssets("t:texture2D", new[] { this.texturesPath });
        // string[] prefabs = AssetDatabase.FindAssets("t:Prefab", new[] { this.prefabsPath });
        //
        // EditorUtility.DisplayProgressBar("Progressing...", "Start cleaning", 0);
        // int index = 0;
        //
        // List<string> deleteList = new List<string>();
        //
        // foreach (var path in imagePaths)
        // {
        //     index++;
        //     EditorUtility.DisplayProgressBar("Progressing...", "Start cleaning", index / imagePaths.Length);
        //
        //
        //     foreach (var VARIABLE in prefabs)
        //     {
        //     }
        // }
        //
        // AssetDatabase.SaveAssets();
        // EditorUtility.ClearProgressBar();
        // AssetDatabase.Refresh();
        //EditorGUIUtility.PingObject(obj);
    }

    private void FindAndCleanImages()
    {
        if (!Directory.Exists(deleteFloder))
        {
            Directory.CreateDirectory(deleteFloder);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(configPicFloder))
        {
            Directory.CreateDirectory(configPicFloder);
            AssetDatabase.Refresh();
        }

        string[] allImageGuids = AssetDatabase.FindAssets("t:texture2D", new[] { texturesPath });
        string[] allPrefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabsPath });

        string[] cleans = AssetDatabase.FindAssets("t:textAsset", new[] { cleanListText });
        string[] cleanListpic = AssetDatabase.FindAssets("t:texture2D", new[] { cleanListPic });
        // 用于存储不带扩展名的文件名的列表
        HashSet<string> cleanList = new HashSet<string>();
        HashSet<string> configpicList = new HashSet<string>();
        //Debug.Log($"clean1:{cleans.Length}");
        // 2. 遍历 GUID 数组，获取每个资源的路径，然后提取并清理文件名
        foreach (string guid in cleans)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guid);
            //Debug.Log($"clean1:{filePath}");
            if (filePath.EndsWith(".txt"))
            {
                // 读取文件内容
                string fileContent = File.ReadAllText(filePath);

                // 使用分号 (;) 分隔并将结果添加到 List 中
                string[] strings = fileContent.Split('&');

                foreach (string str in strings)
                {
                    cleanList.Add(str.Trim()); // 去除多余空格
                    configpicList.Add(str.Trim()); // 去除多余空格
                    //Debug.Log($"clean1:{str}");
                }
            }
            // 获取不带扩展名的文件名（例如 "carbon_subtract-filled"）
            //string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithExtension);

            //cleanList.Add(fileNameWithoutExtension);
        }

        // 2. 遍历 GUID 数组，获取每个资源的路径，然后提取并清理文件名
        foreach (string guid in cleanListpic)
        {
            // 将 GUID 转换为资源在项目中的完整路径
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 获取带扩展名的文件名（例如 "carbon_subtract-filled.png"）
            string fileNameWithExtension = Path.GetFileName(assetPath);

            // 获取不带扩展名的文件名（例如 "carbon_subtract-filled"）
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithExtension);

            cleanList.Add(fileNameWithoutExtension);
        }

        // foreach (var clean in cleanList)
        // {
        //     Debug.Log($"clean:{clean}");
        // }

        List<string> unreferencedImagePaths = new List<string>();
        HashSet<string> referencedImageGuids = new HashSet<string>();

        EditorUtility.DisplayProgressBar("Scanning Prefabs", "Collecting referenced images...", 0f);

        // First, build a set of all textures referenced by prefabs
        for (int i = 0; i < allPrefabGuids.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGuids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogWarning($"Could not load prefab at path: {prefabPath}");
                continue;
            }

            // Get all renderers and UI Images in the prefab and its children
            // Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
            // foreach (Renderer renderer in renderers)
            // {
            //     foreach (Material material in renderer.sharedMaterials)
            //     {
            //         if (material != null)
            //         {
            //             // Check all texture properties on the material
            //             Shader shader = material.shader;
            //             for (int j = 0; j < ShaderUtil.Get = PropertyCount(shader); j++)
            //             {
            //                 if (ShaderUtil.GetPropertyType(shader, j) == ShaderUtil.ShaderPropertyType.TexEnv)
            //                 {
            //                     Texture texture = material.GetTexture(ShaderUtil.GetPropertyName(shader, j));
            //                     if (texture != null)
            //                     {
            //                         string textureGuid =
            //                             AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
            //                         if (!string.IsNullOrEmpty(textureGuid))
            //                         {
            //                             referencedImageGuids.Add(textureGuid);
            //                         }
            //                     }
            //                 }
            //             }
            //         }
            //     }
            // }

            // Check for SpriteRenderers
            SpriteRenderer[] spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                if (sr.sprite != null)
                {
                    string spriteTextureGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sr.sprite));
                    if (!string.IsNullOrEmpty(spriteTextureGuid))
                    {
                        referencedImageGuids.Add(spriteTextureGuid);
                    }
                }
            }

            // Check for UnityEngine.UI.Image components
            UnityEngine.UI.Image[] uiImages = prefab.GetComponentsInChildren<UnityEngine.UI.Image>(true);
            foreach (UnityEngine.UI.Image uiImage in uiImages)
            {
                if (uiImage.sprite != null)
                {
                    string uiImageTextureGuid =
                        AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(uiImage.sprite));
                    if (!string.IsNullOrEmpty(uiImageTextureGuid))
                    {
                        referencedImageGuids.Add(uiImageTextureGuid);
                    }
                }
            }

            XImage[] uixImages = prefab.GetComponentsInChildren<XImage>(true);
            foreach (var uixImage in uixImages)
            {
                if (uixImage.sprite != null)
                {
                    string uiImageTextureGuid =
                        AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(uixImage.sprite));
                    if (!string.IsNullOrEmpty(uiImageTextureGuid))
                    {
                        referencedImageGuids.Add(uiImageTextureGuid);
                    }
                }
            }
            // You might need to add checks for other components or custom scripts that use Textures
            // Example for a custom script property:
            // MyCustomScript[] customScripts = prefab.GetComponentsInChildren<MyCustomScript>(true);
            // foreach (MyCustomScript script in customScripts)
            // {
            //     if (script.myTextureField != null)
            //     {
            //         string customTextureGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(script.myTextureField));
            //         if (!string.IsNullOrEmpty(customTextureGuid))
            //         {
            //             referencedImageGuids.Add(customTextureGuid);
            //         }
            //     }
            // }

            EditorUtility.DisplayProgressBar("Scanning Prefabs", "Collecting referenced images...",
                (float)(i + 1) / allPrefabGuids.Length);
        }

        EditorUtility.DisplayProgressBar("Checking Images", "Identifying unreferenced images...", 0f);

        // Now, compare all images against the set of referenced images
        for (int i = 0; i < allImageGuids.Length; i++)
        {
            string imageGuid = allImageGuids[i];
            string imagePath = AssetDatabase.GUIDToAssetPath(imageGuid);

            //转移表图
            if (configpicList.Count > 0)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);
                bool skip = false;
                
                foreach (var clean in configpicList)
                {
                    if (fileNameWithoutExtension.Contains(clean))
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    string fileName = System.IO.Path.GetFileName(imagePath);
                    string newPath = System.IO.Path.Combine(configPicFloder, fileName);

                    // 移动资产
                    string result = AssetDatabase.MoveAsset(imagePath, newPath);

                    //Debug.Log($"排除白名单名字:{fileNameWithoutExtension}");
                    continue;
                }
            }
            if (cleanList.Count > 0)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);
                bool skip = false;
                foreach (var clean in cleanList)
                {
                    if (fileNameWithoutExtension.Contains(clean))
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    Debug.Log($"排除白名单名字:{fileNameWithoutExtension}");
                    continue;
                }
                // if (cleanList.Contains(fileNameWithoutExtension))
                // {
                //     Debug.Log($"排除白名单名字:{fileNameWithoutExtension}");
                //     continue;
                // }
            }

            
            if (!referencedImageGuids.Contains(imageGuid))
            {
                unreferencedImagePaths.Add(imagePath);
                Debug.Log($"待删除的图片路径:{imagePath}");
            }
            else
            {
                Debug.Log($"有引用的图片路径:{imagePath}");
            }

            EditorUtility.DisplayProgressBar("Checking Images", "Identifying unreferenced images...",
                (float)(i + 1) / allImageGuids.Length);
        }

        EditorUtility.ClearProgressBar();


        int deletedCount = 0;
        foreach (var path in unreferencedImagePaths)
        {
            string fileName = System.IO.Path.GetFileName(path);
            string newPath = System.IO.Path.Combine(deleteFloder, fileName);

            // 移动资产
            string result = AssetDatabase.MoveAsset(path, newPath);

            //Debug.Log($"待删除的有path:{path}");
            //AssetDatabase.MoveAsset(path, deleteFloder);
            //AssetDatabase.DeleteAsset(path);
            deletedCount++;
        }

        AssetDatabase.Refresh();
    }

    private Color SampleTexture(Texture2D texture, Vector2 position)
    {
        // 如果采样位置在纹理边界之外，返回透明
        if (position.x < 0 || position.x >= texture.width || position.y < 0 || position.y >= texture.height)
        {
            return Color.clear;
        }

        // 取样纹理中的颜色
        return texture.GetPixel((int)position.x, (int)position.y);
    }

    private void CropTransparentPixels(Texture2D originalTexture, string path)
    {
        int width = originalTexture.width;
        int height = originalTexture.height;

        // 初始化边界值为最大范围
        int left = width, right = 0, top = 0, bottom = height;

        // 遍历图片找到非透明像素的边界
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixelColor = originalTexture.GetPixel(x, y);
                if (pixelColor.a > 0.03f) // 检查透明度
                {
                    if (x < left) left = x;
                    if (x > right) right = x;
                    if (y < bottom) bottom = y;
                    if (y > top) top = y; // 修正此处，top 需要记录最大 y
                }
            }
        }

        // 计算裁剪区域的宽高
        int croppedWidth = right - left + 1;
        int croppedHeight = top - bottom + 1;

        // 如果图片完全透明，避免出现无效裁剪的情况
        if (croppedWidth <= 0 || croppedHeight <= 0)
        {
            Debug.LogWarning("No non-transparent pixels found to crop.");
            return;
        }

        // 创建一个新的纹理，并将裁剪的像素复制到新纹理中
        Texture2D croppedTexture = new Texture2D(croppedWidth, croppedHeight);
        for (int x = 0; x < croppedWidth; x++)
        {
            for (int y = 0; y < croppedHeight; y++)
            {
                Color pixelColor = originalTexture.GetPixel(left + x, bottom + y);
                croppedTexture.SetPixel(x, y, pixelColor);
            }
        }

        // 应用像素变化
        croppedTexture.Apply();

        // 保存裁剪后的纹理为 PNG 文件
        SaveTextureAsPNG(croppedTexture, path);

        // 刷新 AssetDatabase 以加载新图片
        AssetDatabase.Refresh();

        Object obj = AssetDatabase.LoadMainAssetAtPath(deleteFloder);
        EditorGUIUtility.PingObject(obj);
    }

    private void SaveTextureAsPNG(Texture2D texture, string path)
    {
        // 将纹理编码为 PNG 数据
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            // 将 PNG 数据保存为文件
            System.IO.File.WriteAllBytes(path, pngData);
            Debug.Log("Saved resized texture to: " + path);
        }
    }
}