//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using FMOD;
using TMPro;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_SubBtn)]
    internal sealed class UICommon_SubBtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_SubBtn;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_SubBtn>();
        }
    }

    public partial class UICommon_SubBtn : UI, IAwake<int>
    {
        public int tagFuncId;

        //字体大小
        public int initSize = 50;
        public int ChageSize = 60;


        //颜色通道
        public float InitAlaph = 190 / 255.0f;
        public float ChageAlaph = 255 / 255.0f;
        private Color InitColor;
        public int ButtonIndex;
        public int posId;

        public void Initialize(int posId)
        {
            this.posId = posId;
            tagFuncId = posId;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }


        //初始化多语言
        public void InitWideget(string Name, bool ImageState, string icon)
        {
            GetFromReference(KText_Name).GetTextMeshPro().SetTMPText(Name);
            GetFromReference(KText_Name).SetActive(ImageState);
            Log.Debug($"icon:{icon}", Color.cyan);
            GetFromReference(KImg_icon)?.GetImage().SetSprite(icon, false);
            //InitColor = GetFromReference(KImg_Bottom).GetComponent<XImage>().color;
        }


        //改变状态函数,主要是为了区分选用还是没有选用
        //两个参数是为了有些按钮选中没有下面的下划线
        //第二个参数是指是否有下划线
        public void ChageState(bool HasUnderLine)
        {
            //var textInitColor = GetFromReference(KText_Name).GetComponent<TextMeshProUGUI>().color;

            // if (BtnState)
            // {
            //     GetFromReference(KText_Name).GetComponent<TextMeshProUGUI>().fontSize = ChageSize;
            //     GetFromReference(KText_Name).GetComponent<TextMeshProUGUI>().color =
            //         new Color(textInitColor.r, textInitColor.g, textInitColor.b, ChageAlaph);
            //     GetFromReference(KImg_Bottom).GetComponent<XImage>().color =
            //         new Color(InitColor.r, InitColor.g, InitColor.b, ChageAlaph);
            // }
            // else
            // {
            //     GetFromReference(KText_Name).GetComponent<TextMeshProUGUI>().fontSize = initSize;
            //     GetFromReference(KText_Name).GetComponent<TextMeshProUGUI>().color =
            //         new Color(textInitColor.r, textInitColor.g, textInitColor.b, InitAlaph);
            //     GetFromReference(KImg_Bottom).GetComponent<XImage>().color =
            //         new Color(InitColor.r, InitColor.g, InitColor.b, InitAlaph);
            // }
            var KImg_Bottom = GetFromReference(UICommon_SubBtn.KImg_Bottom);
            // if (HasUnderLine) GetFromReference(KImg_Bottom).SetActive(BtnState);
            // else GetFromReference(KImg_Bottom).SetActive(HasUnderLine);
            KImg_Bottom.SetActive(HasUnderLine);
        }


        public void ChageState2(bool enableUnderLine, float fontSize)
        {
            var KText_Name = GetFromReference(UICommon_SubBtn.KText_Name);
            var KImg_Bottom = GetFromReference(UICommon_SubBtn.KImg_Bottom);
            if (enableUnderLine)
            {
                KImg_Bottom.GetImage().SetSpriteAsync("img_equip_selected", true);
            }
            else
            {
                KImg_Bottom.GetImage().SetSpriteAsync("img_equip_unselected", true);
            }
            // if (isSelected)
            // {
            //     // KText_Name.GetTextMeshPro().SetFontSize(fontSize);
            //     // KText_Name.GetTextMeshPro().SetAlpha(ChageAlaph);
            //     // KText_Name.GetTextMeshPro().SetFontStyle(FontStyles.Bold);
            //     // KImg_Bottom.GetImage().SetAlpha(ChageAlaph);
            // }
            // else
            // {
            //     // KText_Name.GetTextMeshPro().SetFontSize(fontSize);
            //     // KText_Name.GetTextMeshPro().SetAlpha(InitAlaph);
            //     // KText_Name.GetTextMeshPro().SetFontStyle(FontStyles.Normal);
            //     // KImg_Bottom.GetImage().SetAlpha(InitAlaph);
            // }

            KImg_Bottom.SetActive(enableUnderLine);
        }
    }
}