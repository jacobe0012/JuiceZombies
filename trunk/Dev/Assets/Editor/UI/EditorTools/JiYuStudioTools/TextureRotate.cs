using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class TextureRotate
{
    //[Title("非透明像素最大宽高/像素")] public int littleMaxWidth = 256 - 40; // 5x5 grid
    [Title("是否逆时针旋转")] public bool isLeftRotate = false; // 5x5 grid
    [Title("旋转度数")] public int degrees = 90; // 5x5 grid

    [Title("输入路径:")] [FolderPath] public string inputPath = "Assets/Animator2GPUAnimTempFloder";

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
                textureImporter.spriteImportMode = SpriteImportMode.Single;
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
            RotateTexture(texture, assetPath);
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        //EditorGUIUtility.PingObject(obj);
    }

    private Texture2D RotateTexture(Texture2D originalTexture, string path)
    {
        int width = originalTexture.width;
        int height = originalTexture.height;
        Texture2D rotatedTexture = new Texture2D(width, height);

        // 将角度转换为弧度，并根据方向调整符号
        float radian = degrees * Mathf.Deg2Rad * (!isLeftRotate ? 1 : -1);
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        // 中心点，用于旋转计算
        Vector2 pivot = new Vector2(width / 2f, height / 2f);

        // 对每个像素进行旋转计算
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 计算旋转前的位置
                Vector2 originalPos = new Vector2(x - pivot.x, y - pivot.y);

                // 旋转公式：x' = x * cos(θ) - y * sin(θ)
                //           y' = x * sin(θ) + y * cos(θ)
                Vector2 rotatedPos = new Vector2(
                    originalPos.x * cos - originalPos.y * sin,
                    originalPos.x * sin + originalPos.y * cos
                );

                rotatedPos += pivot;

                // 取样原始纹理的颜色
                Color pixelColor = SampleTexture(originalTexture, rotatedPos);

                // 设置旋转后纹理的颜色
                rotatedTexture.SetPixel(x, y, pixelColor);
            }
        }

        rotatedTexture.Apply();
        // 保存裁剪后的纹理为 PNG 文件
        SaveTextureAsPNG(rotatedTexture, path);

        // 刷新 AssetDatabase 以加载新图片
        AssetDatabase.Refresh();
        return rotatedTexture;
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