//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UIPanel_SkillItem)]
    internal sealed class UIPanel_SkillItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPanel_SkillItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_SkillItem>();
        }
    }

    public partial class UIPanel_SkillItem : UI, IAwake<int, int>
    {
        public int index;

        /// <summary>10
        /// 所属技能id
        /// </summary>
        public int skillId;

        private long timerId;

        public void Initialize(int args, int args1)
        {
            index = args;
            skillId = args1;
            var KContainer_Stars = GetFromReference(UIPanel_SkillItem.KContainer_Stars);
        }

        void Update()
        {
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            //timerId = timerMgr.StartRepeatedTimer(updateInternal, this.Update);
            timerId = timerMgr.RepeatedFrameTimer(this.Update);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }

        protected override void OnClose()
        {
            RemoveTimer();
            base.OnClose();
        }
    }
}