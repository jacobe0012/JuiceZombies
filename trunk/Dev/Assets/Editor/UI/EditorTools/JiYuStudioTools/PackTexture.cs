using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class PackTexture
{
    [Title("行个数")] public int RowCount = 5; // 5x5 grid
    [Title("列个数")] public int ColCount = 5; // 5x5 grid
    [Title("是否输出2的幂数像素图")] private bool isPowerOfTwo = false;

    [Title("是否批量文件夹输出(子文件夹命名需要包含‘*x*’几成几)")]
    public bool isAll = true;

    [Title("输入路径:(小图目录)")] [FolderPath] public string inputPath = "Assets/Animator2GPUAnimTempFloder";

    [Title("输出路径:(默认为存放预制件的目录)")] [FolderPath]
    public string outputPath = "Assets/FramePicTempFloder";


    //[Title("预烘焙的预制件列表:")] public List<GameObject> GameObjects;

    [Button(name: "合并")]
    async public void Button()
    {
        if (isAll)
        {
            var paths = AssetDatabase.GetSubFolders(inputPath);
            foreach (var path in paths)
            {
                string[] imagePaths = AssetDatabase.FindAssets("t:texture2D", new[] { path });
                ParseString(path, imagePaths, out var _1, out var _2);
                Debug.LogError($"path:{path}");
                var RowCount = _1;
                var ColCount = _2;

                //imagePaths.Sort();
                // var sortedNames = imagePaths.OrderBy(input =>
                // {
                //     string numberString = new string(input.Where(char.IsDigit).ToArray());
                //     return int.Parse(numberString);
                // }).ToList();
                List<string> sortedNames = imagePaths
                    .OrderBy(fileName =>
                    {
                        // 使用正则表达式提取数字部分
                        var match = Regex.Match(fileName, @"_(\d+)\.png$");
                        // 如果找到了数字部分，则转换为整数，否则返回一个大的数字以将其放在最后
                        return match.Success ? int.Parse(match.Groups[1].Value) : int.MaxValue;
                    })
                    .ToList();

                string assetPath0 = AssetDatabase.GUIDToAssetPath(sortedNames[0]);

                TextureImporter textureImporter0 = (TextureImporter)TextureImporter.GetAtPath(assetPath0);
                if (textureImporter0 != null)
                {
                    // Change the texture type to Sprite 2D
                    //textureImporter.textureType = TextureImporterType.Sprite;
                    //textureImporter.spriteImportMode = SpriteImportMode.Single;
                    textureImporter0.isReadable = true;
                    textureImporter0.npotScale = TextureImporterNPOTScale.None;
                    // Apply changes
                    AssetDatabase.ImportAsset(assetPath0, ImportAssetOptions.ForceUpdate);
                    //AssetDatabase.Refresh();
                    //await UniTask.Delay(50);
                    //Debug.Log("Texture type changed to Sprite.");
                }

                Texture2D texture0 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath0);
                // Calculate the size of the texture atlas
                //int tileSize = Mathf.Min(texture0.height, texture0.width); // Assumes all textures are the same size
                //tileSizeW =
                //int tileSize = texture0.width;


                int powerOfTwoSizeX = Mathf.NextPowerOfTwo(ColCount * texture0.width);
                int powerOfTwoSizeY = Mathf.NextPowerOfTwo(RowCount * texture0.height);
                if (!isPowerOfTwo)
                {
                    powerOfTwoSizeX = ColCount * texture0.width;
                    powerOfTwoSizeY = RowCount * texture0.height;
                }

                int textureWidth = texture0.width;
                int textureHeight = texture0.height;

                // 创建一张新的大图
                Texture2D packedTexture = new Texture2D(textureWidth * ColCount, textureHeight * RowCount);

                Vector2Int maxWH = default;
                // 将每一张小图放到合适的位置
                for (int i = 0; i < RowCount; i++)
                {
                    for (int j = 0; j < ColCount; j++)
                    {
                        int index = i * ColCount + j;

                        Texture2D texture = null;
                        if (index < sortedNames.Count)
                        {
                            string assetPath = AssetDatabase.GUIDToAssetPath(sortedNames[index]);
                            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                            texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                            if (texture == null) continue;
                            if (textureImporter != null)
                            {
                                textureImporter.isReadable = true;
                                textureImporter.npotScale = TextureImporterNPOTScale.None;
                                // Apply changes
                                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                                //AssetDatabase.Refresh();
                                //await UniTask.Delay(50);
                                //Debug.Log("Texture type changed to Sprite.");
                            }
                            else
                            {
                                Debug.LogError("Texture not found at the specified path: " + assetPath);
                            }
                        }

                        // 获取纹理路径和纹理

                        int xPos = j * textureWidth; // 横坐标
                        int yPos = (RowCount - 1 - i) * textureHeight; // 纵坐标，调整Y坐标确保从上到下填充
                        //Texture2D texture = textures[index];
                        if (texture != null)
                        {
                            var newmaxWH = GetValidPixelDimensions(texture);
                            if (newmaxWH.x > maxWH.x)
                            {
                                maxWH.x = newmaxWH.x;
                            }

                            if (newmaxWH.y > maxWH.y)
                            {
                                maxWH.y = newmaxWH.y;
                            }

                            // 确保每张小图都放置在正确位置
                            packedTexture.SetPixels(xPos, yPos, textureWidth, textureHeight, texture.GetPixels());
                        }
                        else
                        {
                            // 创建一个透明颜色数组
                            Color[] transparentPixels = new Color[textureWidth * textureHeight];
                            for (int k = 0; k < transparentPixels.Length; k++)
                            {
                                transparentPixels[k] = new Color(0, 0, 0, 0); // 透明的颜色
                            }

                            packedTexture.SetPixels(xPos, yPos, textureWidth, textureHeight, transparentPixels);
                        }
                    }
                }
                
                packedTexture.Apply();

                // Save the texture as a PNG file
                byte[] pngData = packedTexture.EncodeToPNG();
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                string folderName = Path.GetFileName(path);
                float scaleMulti = 1;
                if (maxWH.x >= maxWH.y)
                {
                    scaleMulti = texture0.width / (float)maxWH.x;
                }
                else
                {
                    scaleMulti = texture0.height / (float)maxWH.y;
                }


                float max = Mathf.Max(texture0.width, texture0.height);
                var sc = new Vector2(texture0.width / max, texture0.height / max);
                sc *= scaleMulti;
                File.WriteAllBytes(
                    $"{outputPath}/{folderName}_rc{RowCount}x{ColCount}_wh{texture0.width}x{texture0.height}_sc{sc.x.ToString("f5")}x{sc.y.ToString("f5")}.png",
                    pngData);
                AssetDatabase.Refresh();

                Debug.Log("Texture packing complete. Check the Assets folder for PackedTexture.png.");
            }
        }
        else
        {
            string[] imagePaths = AssetDatabase.FindAssets("t:texture2D", new[] { inputPath });
            ParseString(inputPath, imagePaths, out var _1, out var _2);
            Debug.LogError($"path:{inputPath}");
            var RowCount = _1;
            var ColCount = _2;

            //imagePaths.Sort();
            // var sortedNames = imagePaths.OrderBy(input =>
            // {
            //     string numberString = new string(input.Where(char.IsDigit).ToArray());
            //     return int.Parse(numberString);
            // }).ToList();
            List<string> sortedNames = imagePaths
                .OrderBy(fileName =>
                {
                    // 使用正则表达式提取数字部分
                    var match = Regex.Match(fileName, @"_(\d+)\.png$");
                    // 如果找到了数字部分，则转换为整数，否则返回一个大的数字以将其放在最后
                    return match.Success ? int.Parse(match.Groups[1].Value) : int.MaxValue;
                })
                .ToList();

            string assetPath0 = AssetDatabase.GUIDToAssetPath(sortedNames[0]);

            TextureImporter textureImporter0 = (TextureImporter)TextureImporter.GetAtPath(assetPath0);
            if (textureImporter0 != null)
            {
                // Change the texture type to Sprite 2D
                //textureImporter.textureType = TextureImporterType.Sprite;
                //textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter0.isReadable = true;
                textureImporter0.npotScale = TextureImporterNPOTScale.None;
                // Apply changes
                AssetDatabase.ImportAsset(assetPath0, ImportAssetOptions.ForceUpdate);
                //AssetDatabase.Refresh();
                //await UniTask.Delay(50);
                //Debug.Log("Texture type changed to Sprite.");
            }

            Texture2D texture0 = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath0);
            // Calculate the size of the texture atlas
            //int tileSize = Mathf.Min(texture0.height, texture0.width); // Assumes all textures are the same size
            //tileSizeW =
            //int tileSize = texture0.width;


            int powerOfTwoSizeX = Mathf.NextPowerOfTwo(ColCount * texture0.width);
            int powerOfTwoSizeY = Mathf.NextPowerOfTwo(RowCount * texture0.height);
            if (!isPowerOfTwo)
            {
                powerOfTwoSizeX = ColCount * texture0.width;
                powerOfTwoSizeY = RowCount * texture0.height;
            }

            int textureWidth = texture0.width;
            int textureHeight = texture0.height;

            // 创建一张新的大图
            Texture2D packedTexture = new Texture2D(textureWidth * ColCount, textureHeight * RowCount);


            Vector2Int maxWH = default;
            Vector2 sc = default;
            // 将每一张小图放到合适的位置

            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColCount; j++)
                {
                    int index = i * ColCount + j;

                    Texture2D texture = null;
                    if (index < sortedNames.Count)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(sortedNames[index]);
                        TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                        texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                        if (texture == null) continue;
                        if (textureImporter != null)
                        {
                            textureImporter.isReadable = true;
                            textureImporter.npotScale = TextureImporterNPOTScale.None;
                            // Apply changes
                            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                            //AssetDatabase.Refresh();
                            //await UniTask.Delay(50);
                            //Debug.Log("Texture type changed to Sprite.");
                        }
                        else
                        {
                            Debug.LogError("Texture not found at the specified path: " + assetPath);
                        }
                    }

                    // 获取纹理路径和纹理

                    int xPos = j * textureWidth; // 横坐标
                    int yPos = (RowCount - 1 - i) * textureHeight; // 纵坐标，调整Y坐标确保从上到下填充
                    //Texture2D texture = textures[index];

                    if (texture != null)
                    {
                        sc = GetPrefabScale(texture);
                        // var newmaxWH = GetValidPixelDimensions(texture);
                        // if (newmaxWH.x > maxWH.x)
                        // {
                        //     maxWH.x = newmaxWH.x;
                        // }
                        //
                        // if (newmaxWH.y > maxWH.y)
                        // {
                        //     maxWH.y = newmaxWH.y;
                        // }

                        // 确保每张小图都放置在正确位置
                        packedTexture.SetPixels(xPos, yPos, textureWidth, textureHeight, texture.GetPixels());
                    }
                    else
                    {
                        // 创建一个透明颜色数组
                        Color[] transparentPixels = new Color[textureWidth * textureHeight];
                        for (int k = 0; k < transparentPixels.Length; k++)
                        {
                            transparentPixels[k] = new Color(0, 0, 0, 0); // 透明的颜色
                        }

                        packedTexture.SetPixels(xPos, yPos, textureWidth, textureHeight, transparentPixels);
                    }
                }
            }
            
            packedTexture.Apply();

            // Save the texture as a PNG file
            byte[] pngData = packedTexture.EncodeToPNG();
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            string folderName = Path.GetFileName(inputPath);

            float scaleMulti = 1;
            if (maxWH.x >= maxWH.y)
            {
                scaleMulti = texture0.width / (float)maxWH.x;
            }
            else
            {
                scaleMulti = texture0.height / (float)maxWH.y;
            }


            // float max = Mathf.Max(texture0.width, texture0.height);
            // var sc = new Vector2(texture0.width / max, texture0.height / max);
            // sc *= scaleMulti;
            File.WriteAllBytes(
                $"{outputPath}/{folderName}_rc{RowCount}x{ColCount}_wh{texture0.width}x{texture0.height}_sc{sc.x.ToString("f5")}x{sc.y.ToString("f5")}.png",
                pngData);
            AssetDatabase.Refresh();

            Debug.Log("Texture packing complete. Check the Assets folder for PackedTexture.png.");
        }
    }

    static (int, int) FindClosestFactors(int target)
    {
        int a = 1, b = target; // 初始化最小差值的因数
        int minDiff = int.MaxValue; // 初始化最小差值为最大整数

        // 遍历 1 到 sqrt(target) 的整数
        for (int i = 1; i <= Mathf.Sqrt(target); i++)
        {
            if (target % i == 0) // 如果 i 是 target 的因数
            {
                int factor1 = i;
                int factor2 = target / i;

                // 计算两个因数的差值
                int diff = Mathf.Abs(factor1 - factor2);

                // 如果差值更小，更新 a 和 b
                if (diff < minDiff)
                {
                    a = factor1;
                    b = factor2;
                    minDiff = diff;
                }
            }
        }

        return (a, b); // 返回最接近的因数
    }

    void ParseString(string input, string[] strings, out int num1, out int num2)
    {
        // 使用正则表达式查找数字
        Regex regex = new Regex(@"(\d+)[xX](\d+)");
        num1 = 0;
        num2 = 0;
        // 尝试匹配字符串
        Match match = regex.Match(input);
        if (match.Success)
        {
            // 提取匹配到的数字
            num1 = int.Parse(match.Groups[1].Value);
            num2 = int.Parse(match.Groups[2].Value);
            if (num1 * num2 != strings.Length)
            {
                var factors = FindClosestFactors(strings.Length);
                num1 = factors.Item1;
                num2 = factors.Item2;
            }

            //Debug.LogError($"num1:{num1} num2:{num2}");
        }
        else
        {
            var factors = FindClosestFactors(strings.Length);
            num1 = factors.Item1;
            num2 = factors.Item2;
            Debug.Log("No match found");
        }
    }

    // 获取图片的有效像素（不透明部分）的宽高
    public static Vector2Int GetValidPixelDimensions(Texture2D texture)
    {
        // 获取纹理的宽和高
        int width = texture.width;
        int height = texture.height;

        // 初始化有效像素的边界
        int minX = width, maxX = 0, minY = height, maxY = 0;

        // 遍历每个像素，找到所有不透明像素的边界
        Color[] pixels = texture.GetPixels();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = pixels[y * width + x];

                // 如果像素的透明度大于0，说明是有效像素
                if (pixelColor.a > 0.03f)
                {
                    // 更新有效像素的边界
                    minX = Mathf.Min(minX, x);
                    maxX = Mathf.Max(maxX, x);
                    minY = Mathf.Min(minY, y);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }

        // 计算有效像素区域的宽度和高度
        if (minX > maxX || minY > maxY) // 如果没有有效像素
        {
            return Vector2Int.zero;
        }

        int validWidth = maxX - minX + 1;
        int validHeight = maxY - minY + 1;

        return new Vector2Int(validWidth, validHeight);
    }


    public static Vector2 GetPrefabScale(Texture2D texture)
    {
        Vector2Int maxWH = default;
        var newmaxWH = GetValidPixelDimensions(texture);
        if (newmaxWH.x > maxWH.x)
        {
            maxWH.x = newmaxWH.x;
        }

        if (newmaxWH.y > maxWH.y)
        {
            maxWH.y = newmaxWH.y;
        }

        float scaleMulti = 1;
        if (maxWH.x >= maxWH.y)
        {
            scaleMulti = texture.width / (float)maxWH.x;
        }
        else
        {
            scaleMulti = texture.height / (float)maxWH.y;
        }


        float max = Mathf.Max(texture.width, texture.height);
        var sc = new Vector2(texture.width / max, texture.height / max);
        sc *= scaleMulti;
        return sc;
    }
}