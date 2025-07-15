using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using HotFix_UI;
using Spine.Unity.Editor;
using XFramework;

public class Spine2Animator : EditorWindow
{
    private string jsonFilePath = NetWorkManager.savePath;

    private string wsUrl;

    [MenuItem("JiYuStudio Tools/Spine转Animator工具")]
    private static void ShowWindow(MenuCommand command)
    {
        SkeletonBakingWindow.Init(command);
    }
}