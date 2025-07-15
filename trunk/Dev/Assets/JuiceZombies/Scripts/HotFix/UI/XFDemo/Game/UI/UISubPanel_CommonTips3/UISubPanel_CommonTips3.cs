//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------


using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_CommonTips3)]
    internal sealed class UISubPanel_CommonTips3Event : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_CommonTips3;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_CommonTips3>();
        }
    }

    public partial class UISubPanel_CommonTips3 : UI, IAwake
    {
        public void Initialize()
        {
        }

        protected override void OnClose()
        {
            base.OnClose();
        }


        /// <summary>
        /// 初始化指定控件的样式,传入控件对应的key,这个默认设置文本
        /// </summary>
        public void InitWidgetTextInfo(string WidgetKey, string info)
        {
            var WidgetUI = GetFromReference(WidgetKey);
            WidgetUI.GetTextMeshPro().SetTMPText(info);
        }

        /// <summary>
        /// 初始化指定控件的样式,传入控件对应的key,这个默认设置文本
        /// </summary>
        public void InitWidgetTextInfo(string WidgetKey, string info, Color DefalutColor)
        {
            var WidgetUI = GetFromReference(WidgetKey);
            WidgetUI.GetTextMeshPro().SetTMPText(info);
            WidgetUI.GetTextMeshPro().SetColor(DefalutColor);
        }


        /// <summary>
        /// 初始化指定控件的样式,传入控件对应的key,这个默认设置图片
        /// </summary>
        public void InitWidgetImgInfo(string WidgetKey, string ImgName)
        {
            var WidgetUI = GetFromReference(WidgetKey);
            WidgetUI.GetImage().SetSprite(ImgName, false);
        }

        
        /// <summary>
        /// 返回Img_Tips对应长度
        /// </summary>
        /// <returns></returns>
        public float ReturnImgtIips()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                GetFromReference(KImg_Tips).GameObject.GetComponent<RectTransform>());
            float width = this.GetRectTransform(KImg_Tips).Width();
            return width;
        }
    }
}