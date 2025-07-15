//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UIRapidCompound)]
    internal sealed class UIRapidCompoundEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIRapidCompound;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIRapidCompound>();
        }
    }

    public partial class UIRapidCompound : UI, IAwake
    {
        private long timerId;

        //每帧更新的时间
        private int refreshTime;

        //面板存在的时间
        private int coolTime;

        private bool isCreatePanel = false;

        public void Initialize()
        {
            refreshTime = 20;
            coolTime = 2000;
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(refreshTime, this.Update);
        }

        private void Update()
        {
            coolTime -= refreshTime;
            if (coolTime <= 0 && !isCreatePanel)
            {
                OnClose();

                isCreatePanel = true;
            }
        }

        protected override void OnClose()
        {
            //关闭当前面板
            Close();

            //打开一键合成结果面板
            UIHelper.Create(UIType.UIRapidCompoundResult, UILayer.High);
        }
    }
}