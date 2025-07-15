//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Fund_Select_Tab)]
    internal sealed class UISubPanel_Shop_Fund_Select_TabEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Fund_Select_Tab;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Fund_Select_Tab>();
        }
    }

    public partial class UISubPanel_Shop_Fund_Select_Tab : UI, IAwake
    {
        public void Initialize()
        {
            Tblanguage tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            this.GetFromReference(KText_New).GetTextMeshPro().SetTMPText(tblanguage.Get("text_new_1").current);
            //this.GetFromReference(KImg_Tip).SetActive(false);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}