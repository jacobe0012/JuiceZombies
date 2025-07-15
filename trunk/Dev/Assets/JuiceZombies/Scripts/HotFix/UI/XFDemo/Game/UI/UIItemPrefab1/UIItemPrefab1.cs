namespace XFramework
{
    [UIEvent(UIType.UIItemPrefab1)]
    internal sealed class UIItemPrefab1Event : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIItemPrefab1;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIItemPrefab1>();
        }
    }

    public partial class UIItemPrefab1 : UI, IAwake
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