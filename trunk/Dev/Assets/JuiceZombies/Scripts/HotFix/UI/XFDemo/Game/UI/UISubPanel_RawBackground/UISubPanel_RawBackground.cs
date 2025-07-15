//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_RawBackground)]
    internal sealed class UISubPanel_RawBackgroundEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_RawBackground;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_RawBackground>();
        }
    }

    public partial class UISubPanel_RawBackground : UI, IAwake
    {
        private long timerId;

        private const float x = -0.03f, y = -0.03f;

        private RawImage rawImg;

        public void Initialize()
        {
            Log.Debug($"UISubPanel_RawBackground  Initialize", Color.yellow);
            rawImg = GetFromReference(KRawImg).GetComponent<RawImage>();

            StartTimer();
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.RepeatedFrameTimer(Update);
        }

        void Update()
        {
            //Log.Debug($"{Time.timeScale}", Color.yellow);
            rawImg.uvRect = (new Rect(rawImg.uvRect.position + new Vector2(x, y) * Time.deltaTime,
                rawImg.uvRect.size));
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