//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_EquipLevelConsumeItem)]
    internal sealed class UISubPanel_EquipLevelConsumeItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_EquipLevelConsumeItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_EquipLevelConsumeItem>();
        }
    }

    public partial class UISubPanel_EquipLevelConsumeItem : UI, IAwake<Vector3>
    {
        public Vector3 reward;

        public void Initialize(Vector3 args)
        {
            reward = args;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}