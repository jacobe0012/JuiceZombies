using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class TextureResize
{
    [Title("非透明像素最大宽高/像素")] public int littleMaxWidth = 256 - 40; // 5x5 grid
    [Title("宽/像素")] public int width = 256; // 5x5 grid
    [Title("高/像素")] public int height = 256; // 5x5 grid

    [Title("输入路径:")] [FolderPath] public string inputPath = "Assets/Animator2GPUAnimTempFloder";

    [Title("是否放缩有效像素")] public bool isScale = true;
    // [Title("输出路径:(默认为存放预制件的目录)")] [FolderPath]
    // public string outputPath = "Assets/FramePicTempFloder";

    //[Title("预烘焙的预制件列表:")] public List<GameObject> GameObjects;


    [Button(name: "一键转换")]
    public void Button()
    {
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

        string[] imagePaths = AssetDatabase.FindAssets("t:texture2D", new[] { inputPath });
        EditorUtility.DisplayProgressBar("Replacing...", "Start replace", 0);
        int index = 0;


        foreach (var path in imagePaths)
        {
            index++;
            EditorUtility.DisplayProgressBar("Replacing...", "Start replace", index / imagePaths.Length);

            string assetPath = AssetDatabase.GUIDToAssetPath(path);
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (textureImporter != null)
            {
                // Change the texture type to Sprite 2D
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.isReadable = true;
                // Apply changes
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh();
                //Debug.Log("Texture type changed to Sprite.");
            }
            else
            {
                Debug.LogError("Texture not found at the specified path: " + path);
            }

            //texture.tet = TextureImporterType.Sprite;
            // // Create a new texture to hold the packed images
            // Texture2D packedTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            // Color[] pixels = new Color[width * height];
            // packedTexture.SetPixels(pixels);
            CropTransparentPixels(texture, assetPath);
            if (isScale)
            {
                ScaleImage(texture, assetPath);
            }

            ResizeBoardImage(texture, assetPath);
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        //EditorGUIUtility.PingObject(obj);
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
    }

    private void ResizeBoardImage(Texture2D originalTexture, string path)
    {
        int newWidth = width;
        int newHeight = height;

        // 创建一个新的 256x256 的纹理
        Texture2D newTexture = new Texture2D(newWidth, newHeight);

        // 清空新纹理，设置为全透明
        Color clearColor = new Color(0, 0, 0, 0);
        Color[] clearPixels = new Color[newWidth * newHeight];
        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = clearColor;
        }

        newTexture.SetPixels(clearPixels);

        // 计算将原始纹理居中放置的新纹理中的起始位置
        int startX = (newWidth - originalTexture.width) / 2;
        int startY = (newHeight - originalTexture.height) / 2;

        // 将原始纹理的像素拷贝到新纹理中
        for (int x = 0; x < originalTexture.width; x++)
        {
            for (int y = 0; y < originalTexture.height; y++)
            {
                // 获取原始图片的像素
                Color pixelColor = originalTexture.GetPixel(x, y);
                // 设置到新纹理的相应位置
                newTexture.SetPixel(startX + x, startY + y, pixelColor);
            }
        }

        // 应用像素变化
        newTexture.Apply();

        // 保存新纹理为 PNG 文件
        SaveTextureAsPNG(newTexture, path);

        // 刷新 AssetDatabase 以加载新图片
        AssetDatabase.Refresh();
    }

    private void ScaleImage(Texture2D originalTexture, string path)
    {
        var max = Mathf.Max(originalTexture.width, originalTexture.height);
        //var max = originalTexture.width;
        float scaleFactor = littleMaxWidth / (float)max;
        // if (scaleFactor > 1)
        // {
        //     return;
        // }

        if (scaleFactor <= 0)
        {
            Debug.LogWarning("Scale factor must be greater than 0.");
            return;
        }

        // 计算目标尺寸
        int targetWidth = Mathf.RoundToInt(originalTexture.width * scaleFactor);
        int targetHeight = Mathf.RoundToInt(originalTexture.height * scaleFactor);

        // 创建一个新的纹理，大小为目标尺寸
        Texture2D scaledTexture = new Texture2D(targetWidth, targetHeight);

        // 使用双线性插值调整大小
        for (int x = 0; x < targetWidth; x++)
        {
            for (int y = 0; y < targetHeight; y++)
            {
                // 计算原始纹理中的对应坐标
                float u = (float)x / targetWidth;
                float v = (float)y / targetHeight;

                // 获取插值后的像素
                Color color = originalTexture.GetPixelBilinear(u, v);

                // 设置像素
                scaledTexture.SetPixel(x, y, color);
            }
        }

        // 应用像素更改
        scaledTexture.Apply();

        // 保存缩放后的纹理为 PNG 文件
        SaveTextureAsPNG(scaledTexture, path);

        // 刷新 AssetDatabase 以加载新图片
        AssetDatabase.Refresh();
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