//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_EquipSkill)]
    internal sealed class UISubPanel_EquipSkillEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_EquipSkill;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_EquipSkill>();
        }
    }

    public partial class UISubPanel_EquipSkill : UI, IAwake
    {
        public void Initialize()
        {
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}