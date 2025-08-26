//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Challege_AreaInfo)]
    internal sealed class UIPanel_Challege_AreaInfoEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPanel_Challege_AreaInfo;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Challege_AreaInfo>();
        }
    }

    public partial class UIPanel_Challege_AreaInfo : UI, IAwake<int>
    {
        public int index;

        public void Initialize(int challengeID)
        {
            // UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_AreaInfo), () => OnInfoClick(challengeID));
        }

        private void OnInfoClick(int challengeID)
        {
            this.GetFromReference(KImg_Select)?.SetActive(true);
            //this.GetFromReference(KImg_Panel_Select)?.SetActive(true);
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Challege, out UI ui))
            {
                var uIPanel_Challege = ui as UIPanel_Challege;
                uIPanel_Challege?.UpdateFromCurrentMainID(challengeID);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}