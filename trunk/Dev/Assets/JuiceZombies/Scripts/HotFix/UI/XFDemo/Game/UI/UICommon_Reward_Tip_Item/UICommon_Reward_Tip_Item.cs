//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjiguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Reward_Tip_Item)]
    internal sealed class UICommon_Reward_Tip_ItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_Reward_Tip_Item;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Reward_Tip_Item>();
        }
    }

    public partial class UICommon_Reward_Tip_Item : UI, IAwake<Vector3>
    {
        public void Initialize(Vector3 reward)
        {
            var itemlist = this.GetFromReference(KPos_Item).GetList();
            itemlist.Clear();
            var itemUI = itemlist.CreateWithUIType<Vector3>(UIType.UICommon_RewardItem, reward, false);
            JiYuUIHelper.SetRewardOnClick(reward, itemUI);
            itemUI.GetRectTransform().SetScale2(0.7f);
            //this.GetFromReference(KText_Item).GetTextMeshPro().SetTMPText(JiYuUIHelper.GetRewardName(reward));
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}