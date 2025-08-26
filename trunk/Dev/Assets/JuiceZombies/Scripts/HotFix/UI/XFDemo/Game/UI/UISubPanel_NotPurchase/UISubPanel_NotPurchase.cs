//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_NotPurchase)]
    internal sealed class UISubPanel_NotPurchaseEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_NotPurchase;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_NotPurchase>();
        }
    }

    public partial class UISubPanel_NotPurchase : UI, IAwake<int>
    {
        public void Initialize()
        {
        }

        public void Initialize(int talentID)
        {
            var tanlentMap = ConfigManager.Instance.Tables.Tbtalent.DataMap;
            var lang = ConfigManager.Instance.Tables.Tblanguage;
            this.GetFromReference(KBtn_Close)?.GetComponent<XButton>().onClick.Add(Close);
            this.GetFromReference(KTxt_Title)?.GetTextMeshPro() 
                .SetTMPText(lang.Get("common_lack_3_title").current + lang.Get("common_demand").current);
            long needCount = (long)tanlentMap[talentID].cost[0].z - ResourcesSingletonOld.Instance.UserInfo.RoleAssets.UsBill;
            this.GetButton(KBtn_Common)?.OnClick.Add(GotoShop);
            this.GetFromReference(KTxt_Num).GetTextMeshPro().SetTMPText(needCount.ToString());
        }

        private void GotoShop()
        {
            //?????4
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out UI jiyu))
            {
                var ui = jiyu as UIPanel_JiyuGame;
                var tagMap = ConfigManager.Instance.Tables.Tbtag.DataMap;
                ui.OnBtnClickEvent(tagMap[1].sort);
            }

            Close();
        }

        protected override void OnClose()
        {
            Log.Debug("ClosePanel");
            base.OnClose();
        }
    }
}