//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------


namespace XFramework
{
    [UIEvent(UIType.UICommon_Reward_Tip2)]
    internal sealed class UICommon_Reward_Tip2Event : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_Reward_Tip2;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Reward_Tip2>();
        }
    }

    public partial class UICommon_Reward_Tip2 : UI, IAwake
    {
        //private int itemW = 156; 
        public void Initialize()
        {
            //int rewardCount = rewards.Count;
            //this.GetFromReference(KBg).GetRectTransform().SetWidth(itemW);
            //var bgList = this.GetFromReference(KBg).GetList();
            //bgList.Clear();
            //         for (int i = 0; i < rewardCount; i++) 
            //{
            //	await bgList.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, rewards[i], false);
            //}
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}