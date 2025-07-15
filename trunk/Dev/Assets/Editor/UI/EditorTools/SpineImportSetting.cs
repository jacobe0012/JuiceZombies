using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;
using Newtonsoft.Json.Linq;
using Spine;

/// <summary>
/// 修改spine文件 版本
/// </summary>
public class SpineImportSetting : AssetPostprocessor
{
    //任何资源（包括文件夹）导入都会被调用的方法
    void OnPreprocessAsset()
    {
        ProcessSpineJson();
        //ProcessSpineAtlasAndRenameSpinePng();
    }

    void OnPreprocessTexture()
    {
        //ProcessImgSettings();
    }

    void ProcessSpineJson()
    {
        try
        {
            if (!this.assetPath.EndsWith(".json"))
                return;

            //先判断是否是 spine 文件
            string msg = File.ReadAllText(this.assetPath, Encoding.UTF8);
            JObject jo = JObject.Parse(msg);
            string item = jo["skeleton"]["spine"].ToString();

            if (!string.IsNullOrEmpty(item) && item.ToString() != "3.8")
            {
                jo["skeleton"]["spine"] = "3.8"; //修改版本为3.8版本
                File.WriteAllText(this.assetPath, jo.ToString());
                AssetDatabase.Refresh();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"SpineImportSetting 异常 {e.Message}");
        }
    }

    void ProcessImgSettings()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;

        // 检查导入的资源是否为图片文件
        if (textureImporter.textureType == TextureImporterType.Default)
        {
            // 转化为Sprite
            textureImporter.textureType = TextureImporterType.Sprite;

            // 设置为可读写
            textureImporter.isReadable = true;

            //Debug.Log("Texture imported as Sprite and set as Read/Write enabled.");
        }
    }

    void ProcessSpineAtlasAndRenameSpinePng()
    {
        try
        {
            if (!this.assetPath.EndsWith(".atlas.txt"))
                return;

            string[] lines = File.ReadAllLines(this.assetPath, Encoding.UTF8); // 读取文件中的所有行

            for (int i = 0; i < Mathf.Min(3, lines.Length); i++) // 遍历前三行
            {
                if (lines[i].Contains(".png")) // 检查是否包含".png"字符串
                {
                    lines[i] = lines[i].Replace(".png", "_Pic.png"); // 替换包含".png"的行内容
                    Debug.Log("Modified Line " + i + ": " + lines[i]); // 输出修改后的行内容
                }
            }

            // 将修改后的内容写回文件
            File.WriteAllLines(this.assetPath, lines);


            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError($"SpineImportSetting 异常 {e.Message}");
        }
    }
}