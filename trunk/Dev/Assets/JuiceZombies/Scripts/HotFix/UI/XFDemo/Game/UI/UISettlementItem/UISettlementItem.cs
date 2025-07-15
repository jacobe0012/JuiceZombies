//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UISettlementItem)]
    internal sealed class UISettlementItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISettlementItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        //public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UISettlementItem>();
        }
    }

    public struct SettlementStruct
    {
        public bool Clearance;
        public int id;
        public int count;
    };

    public partial class UISettlementItem : UI, IAwake<SettlementStruct>
    {
        public async void Initialize(SettlementStruct settlementStruct)
        {
            //读取多语言表
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            //设置是否是通关奖励
            this.GetFromReference(KClearance_Txt).GetTextMeshPro()
                .SetTMPText(languageConfig.Get("level_commn_reward").current);
            if (settlementStruct.Clearance)
            {
                this.GetFromReference(KClearance_Txt).SetActive(true);
            }

            //设置物品数量
            this.GetFromReference(KItemCountText).GetTextMeshPro().SetTMPText("x " + settlementStruct.count.ToString());
            //设置图片
            switch (settlementStruct.id)
            {
                case 3:
                    //设置金币图片
                    await this.GetFromReference(KItemIcon).GetImage().SetSpriteAsync("gold", false);
                    break;
                case 4:
                    //设置经验图片
                    await this.GetFromReference(KItemIcon).GetImage().SetSpriteAsync("DanCurrency_Exp", false);
                    break;
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}