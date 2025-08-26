//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_RewardItem)]
    internal sealed class UICommon_RewardItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_RewardItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_RewardItem>();
        }
    }

    public partial class UICommon_RewardItem : UI, IAwake<Vector3>
    {
        public Vector3 trueReward;

        public void Initialize(Vector3 reward)
        {
            trueReward = reward;

            //装备堆叠显示
            UnicornUIHelper.SetRewardIconAndCountText(reward, this);
            //UnicornTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UICommon_RewardItem.KBg_Item), 0, 50, 0.35f, false, false);
            GetFromReference(KEffect).SetActive(false);

            //UnicornUIHelper.ChangePaddingLR(this, 50, 0.2f);
            //
            //Time.timeScale
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}