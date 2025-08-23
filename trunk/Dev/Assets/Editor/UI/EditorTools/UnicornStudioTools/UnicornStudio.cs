using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Spine.Unity.Editor;
using UnityEditor;


public class UnicornStudioTools : OdinMenuEditorWindow
{
    [MenuItem("UnicornStudio Tools/工具箱")]
    private static void OpenWindow()
    {
        var window = GetWindow<UnicornStudioTools>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();
        tree.Add("Animator转GPU动画", new Animator2GPUAnim(), EditorIcons.SmartPhone);
        //tree.Add("地图图片转预制件", new Pic2Prefab(), EditorIcons.SmartPhone);
        tree.Add("图片转预制件", new NormalPic2Prefab(), EditorIcons.SmartPhone);
        tree.Add("合并小图为序列帧图", new PackTexture(), EditorIcons.SmartPhone);
        tree.Add("一键替换所有预制件字体为新字体", new ChangeFontInPrefabs(), EditorIcons.SmartPhone);
        tree.Add("调整居中图片大小", new TextureResize(), EditorIcons.SmartPhone);
        tree.Add("旋转图片", new TextureRotate(), EditorIcons.SmartPhone);
        tree.Add("图片色彩转换", new TextureReColor(), EditorIcons.SmartPhone);
        tree.Add("未引用图片清理", new TextureCleaner(), EditorIcons.SmartPhone);
        tree.Add("文字样式快速替换", new TextStyleChange(), EditorIcons.SmartPhone);
        tree.Add("自定义工具", new CustomTest(), EditorIcons.SmartPhone);


        //tree.Add("SkeletonBakingWindow", new SkeletonBakingWindow(), EditorIcons.SmartPhone);
        return tree;
    }
}