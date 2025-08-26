//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UIPanel_SelectItem)]
    internal sealed class UIPanel_SelectItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPanel_SelectItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_SelectItem>();
        }
    }

    public partial class UIPanel_SelectItem : UI, IAwake<int>
    {
        /// <summary>10
        /// 所属技能id
        /// </summary>
        public int skillId;

        public void Initialize(int args)
        {
            skillId = args;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}