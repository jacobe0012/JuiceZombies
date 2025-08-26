using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using HotFix_UI;
using XFramework;

public class ChangeWsUrlTool : EditorWindow
{
    private string jsonFilePath = NetWorkManager.savePath;

    private string wsUrl;

    [MenuItem("UnicornStudio Tools/更改连入服务器IP工具")]
    private static void ShowWindow()
    {
        GetWindow<ChangeWsUrlTool>("更改连入服务器IP工具");
    }

    private void OnGUI()
    {
        //GUILayout.Label("连入服务器IP地址:", EditorStyles.boldLabel);
        GUILayout.Label("");
        // 读取JSON文件
        LoadJSON();

        // 显示当前的wsUrl值
        GUILayout.Label("当前连入服务器IP: " + wsUrl, EditorStyles.boldLabel);
        GUILayout.Label("");
        // 输入新的wsUrl值
        //

        // 更新按钮
        string wsUrl0 = string.Format(NetWorkManager.emptyUrl, NetWorkManager.wsUrl0);
        if (GUILayout.Button($"切换到{wsUrl0}"))
        {
            // 更新JSON数据中的wsUrl字段
            //UpdateWsUrl();

            // 保存更新后的JSON数据
            SaveJSON(NetWorkManager.wsUrl0);
        }

        string wsUrl1 = string.Format(NetWorkManager.emptyUrl, NetWorkManager.wsUrl1);
        if (GUILayout.Button($"切换到{wsUrl1}"))
        {
            // 更新JSON数据中的wsUrl字段
            //UpdateWsUrl();

            // 保存更新后的JSON数据
            SaveJSON(NetWorkManager.wsUrl1);
        }
        // 更新按钮
        // if (GUILayout.Button("切换到192.168.2.3"))
        // {
        //     // 更新JSON数据中的wsUrl字段
        //     //UpdateWsUrl();
        //
        //     // 保存更新后的JSON数据
        //     SaveJSON(3);
        // }


        // if (GUILayout.Button("切换到输入的IP地址"))
        // {
        //     // 更新JSON数据中的wsUrl字段
        //     //UpdateWsUrl();
        //     var anyInput = EditorGUILayout.TextField("New wsUrl:192.168.2.","");
        //     // 保存更新后的JSON数据
        //     SaveJSON(Int32.Parse(anyInput));
        // }
    }

    private void LoadJSON()
    {
        if (File.Exists(jsonFilePath))
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            var webUrlData = JsonHelper.ToObject<NetWorkManager.WebUrlData>(jsonString);
            wsUrl = webUrlData.webUrl;
        }
        else
        {
            Debug.LogWarning("JSON file does not exist at path: " + jsonFilePath);
        }
    }


    private void SaveJSON(int ip)
    {
        // 保存更新后的JSON数据
        string wsUrl = string.Format(NetWorkManager.emptyUrl, ip);
        var wbData = new NetWorkManager.WebUrlData
        {
            webUrl = wsUrl
        };
        var webjson = JsonHelper.ToJson(wbData);

        File.WriteAllText(jsonFilePath, webjson);
        AssetDatabase.Refresh();
    }
}