//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Pass_Token_Item)]
    internal sealed class UISubPanel_Pass_Token_ItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Pass_Token_Item;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Pass_Token_Item>();
        }
    }

    public partial class UISubPanel_Pass_Token_Item : UI, IAwake<Vector3>
    {
        public async void Initialize(Vector3 reward)
        {
            //var item =await this.GetFromReference(KPos_Item).GetList().CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, reward, false);
            //item.GetRectTransform().SetAnchoredPositionX(0);
            //         item.GetRectTransform().SetAnchoredPositionY(0);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}