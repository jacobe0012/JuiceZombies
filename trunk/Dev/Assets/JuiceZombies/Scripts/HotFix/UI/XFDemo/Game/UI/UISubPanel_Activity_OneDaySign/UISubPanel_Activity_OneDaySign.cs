//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using cfg.config;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Activity_OneDaySign)]
    internal sealed class UISubPanel_Activity_OneDaySignEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Activity_OneDaySign;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Activity_OneDaySign>();
        }
    }


    public partial class UISubPanel_Activity_OneDaySign : UI, IAwake<int>
    {
        public int day;
        private long timerId;
        private Tblanguage tblanguage;

        public void Initialize(int args)
        {
            day = args;
            InitJson();
            InitNode();
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        }

        void InitNode()
        {
            var KImg_DayBg = GetFromReference(UISubPanel_Activity_OneDaySign.KImg_DayBg);
            //var KImg_DayBgSp = GetFromReference(UISubPanel_Activity_OneDaySign.KImg_DayBgSp);
            var KText_Day = GetFromReference(UISubPanel_Activity_OneDaySign.KText_Day);
            var KImg_ItemBg = GetFromReference(UISubPanel_Activity_OneDaySign.KImg_ItemBg);
            //var KImg_ItemBgSp = GetFromReference(UISubPanel_Activity_OneDaySign.KImg_ItemBgSp);
            var KPos_Item = GetFromReference(UISubPanel_Activity_OneDaySign.KPos_Item);
            var KCommon_Reward1 = GetFromReference(UISubPanel_Activity_OneDaySign.KCommon_Reward1);
            var KCommon_Reward2 = GetFromReference(UISubPanel_Activity_OneDaySign.KCommon_Reward2);
            var KCommon_Reward3 = GetFromReference(UISubPanel_Activity_OneDaySign.KCommon_Reward3);
            var KCommon_Reward4 = GetFromReference(UISubPanel_Activity_OneDaySign.KCommon_Reward4);
            var KPos_Right = GetFromReference(UISubPanel_Activity_OneDaySign.KPos_Right);
            var KBtn_Get = GetFromReference(UISubPanel_Activity_OneDaySign.KBtn_Get);
            var KImg_Btn = GetFromReference(UISubPanel_Activity_OneDaySign.KImg_Btn);
            var KText_Get = GetFromReference(UISubPanel_Activity_OneDaySign.KText_Get);
            //var KText_Lock = GetFromReference(UISubPanel_Activity_OneDaySign.KText_Lock);
            var KText_Lock1 = GetFromReference(UISubPanel_Activity_OneDaySign.KText_Lock1);

            string cdStr = tblanguage.Get("active_lock_text").current;
            KText_Lock1.GetTextMeshPro().SetTMPText(cdStr);
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            //timerId = timerMgr.StartRepeatedTimer(updateInternal, this.Update);
            timerId = timerMgr.StartRepeatedTimer(1000, this.Update);
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

        private void Update()
        {
            var KText_Lock2 = GetFromReference(UISubPanel_Activity_OneDaySign.KText_Lock2);

            long clientT = UnicornUIHelper.GetServerTimeStamp(true);

            if (!UnicornUIHelper.TryGetRemainingTime(TimeHelper.ClientNowSeconds(),
                    TimeHelper.ClientNowSeconds() + TimeHelper.GetToTomorrowTime(),
                    out var timeStr))
            {
                //Close();
            }

            // if (!UnicornUIHelper.TryGetRemainingTime(clientT, endTime, out var timeStr))
            // {
            //     Close();
            // }

            //cdStr += timeStr;

            KText_Lock2.GetTextMeshPro().SetTMPText(timeStr);
        }


        protected override void OnClose()
        {
            RemoveTimer();
            base.OnClose();
        }
    }
}