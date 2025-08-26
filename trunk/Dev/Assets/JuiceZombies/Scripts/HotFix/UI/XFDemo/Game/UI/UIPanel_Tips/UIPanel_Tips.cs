//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Tips)]
    internal sealed class UIPanel_TipsEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Tips;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Tips>();
        }
    }

    public partial class UIPanel_Tips : UI, IAwake<string>
    {
        public void Initialize(string value)
        {
            GetFromReference(KTextInfo).GetTextMeshPro().SetTMPText(value);
            this.GetButton(KButtonClose)?.OnClick.Add(Close);
        }

        protected override void OnClose()
        {
            base.OnClose();
            Close();
        }
    }
}