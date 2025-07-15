//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Unity.Mathematics;
using UnityEngine;
using static HotFix_UI.JiYuUIHelper;
using UnityEngine.UIElements;
using System.Threading;
using System;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Sign)]
    internal sealed class UIPanel_SignEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Sign;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Sign>();
        }
    }

    public partial class UIPanel_Sign : UI, IAwake<UISubPanel_IconBtnItem>
    {
        #region 参数

        private long thisSignDayNum = 0;
        private int DailyCanSignOrNot = 0;
        private int BigCanSignOrNot = 0;
        private long CheckInDay = 0;
        private int SignDayHelp = 0;
        private int dailyMaxDay = 0;
        private int bigMaxDay = 0;
        private long timerId = 0;
        private long timerId0 = 0;
        private Tblanguage tblanguage;
        private Tbsign_daily tbsign_Daily;
        private Tbsign_acc tbsign_Acc;
        private UISubPanel_IconBtnItem uiParentBtn;
        private CancellationTokenSource cts = new CancellationTokenSource();

        #endregion


        public async void Initialize(UISubPanel_IconBtnItem ui)
        {
            await JiYuUIHelper.InitBlur(this);

            uiParentBtn = ui;
            this.GetFromReference(KBg_Btn).GetXButton().OnClick?.Add(() =>
            {
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out UI ui))
                {
                    var ui1 = ui as UIPanel_Battle;
                    //ui1.RedPointSetState();
                }

                GetAllBox(true);
                Debug.Log("close bg");
                Close();
            });

            SetData();
            TimerInit();
            SetUpdate();
            DataInit();
            BtnInit();
            TxtInit();
            SetState();
            var height1 = this.GetFromReference(KImg_Bg).GetRectTransform().AnchoredPosition().y;
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(KImg_Bg), height1, 100, cts.Token, 0.3f, true,
                true);

            JiYuTweenHelper.SetEaseAlphaAndScale(this.GetFromReference(KPos_Up), 0.25f, false);
        }


        /// <summary>
        /// 设置定时任务查询数据，可能不需要了
        /// </summary>
        private async void QuertInit()
        {
            /*long timeHelp = TimeHelper.GetToTomorrowTime();
            await UniTask.Delay(1000 * (int)timeHelp);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Sign, out UI uuii))
            {
                WebMessageHandler.Instance.AddHandler(12, 1, OnQueryResponse);
                NetWorkManager.Instance.SendMessage(12, 1);
            }*/
        }

        /// <summary>
        /// 初始化相关数据
        /// </summary>
        private void DataInit()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbsign_Acc = ConfigManager.Instance.Tables.Tbsign_acc;
            tbsign_Daily = ConfigManager.Instance.Tables.Tbsign_daily;
            if (tbsign_Daily.DataList.Count > 0)
            {
                dailyMaxDay = tbsign_Daily.DataList[tbsign_Daily.DataList.Count - 1].id;
            }
            else
            {
                dailyMaxDay = 0;
            }

            if (tbsign_Acc.DataList.Count > 0)
            {
                bigMaxDay = tbsign_Acc.DataList[tbsign_Acc.DataList.Count - 1].day;
            }
            else
            {
                bigMaxDay = 0;
            }

            WebMessageHandler.Instance.AddHandler(12, 1, OnSignResponse);
            NetWorkManager.Instance.SendMessage(12, 1);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        private void SetData()
        {
            #region

            thisSignDayNum = ResourcesSingleton.Instance.signData.ServerDay;
            CheckInDay = ResourcesSingleton.Instance.signData.CheckInDay;
            DailyCanSignOrNot = ResourcesSingleton.Instance.signData.OnDayStatus;
            BigCanSignOrNot = ResourcesSingleton.Instance.signData.BoxOnDayStatus;

            #endregion
        }

        /// <summary>
        /// 创建update
        /// </summary>
        private void TimerInit()
        {
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(200, update);
        }
        private void SetUpdate()
        {
            var timerMgr = TimerManager.Instance;
            timerId0 = timerMgr.StartRepeatedTimer(2000, UpdateClose);
        }

        private void UpdateClose()
        {
            var KCommon_CloseInfo = GetFromReference(UIPanel_Sign.KCommon_CloseInfo);
            KCommon_CloseInfo.GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_close").current);

            KCommon_CloseInfo.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KCommon_CloseInfo.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
        }

        /// <summary>
        /// 设置初始化
        /// </summary>
        private void TxtInit()
        {
            this.GetFromReference(KText_Title).GetTextMeshPro().SetTMPText(tblanguage.Get("func_3601_name").current);
            this.GetFromReference(KText_Btn).GetTextMeshPro().SetTMPText(tblanguage.Get("sign_state_gain").current);
            this.GetFromReference(KText_Next).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("sign_daily_next_text").current);
            string DialogStrHelp = tblanguage.Get("sign_acc_no_text").current;
            DialogStrHelp = DialogStrHelp.Replace("{0}", CheckInDay.ToString());
            int CheckInDayHelp = (int)(CheckInDay % tbsign_Acc.DataList[tbsign_Acc.DataList.Count - 1].day);
            for (int i = 0; i < tbsign_Acc.DataList.Count; i++)
            {
                if (tbsign_Acc.DataList[i].day <= CheckInDayHelp)
                {
                    continue;
                }
                else
                {
                    DialogStrHelp =
                        DialogStrHelp.Replace("{1}", (tbsign_Acc.DataList[i].day - CheckInDayHelp).ToString());
                }
            }

            this.GetFromReference(KText_Dialog).GetTextMeshPro().SetTMPText(DialogStrHelp);
            TxtSet();
        }

        /// <summary>
        /// 设置倒计时文本
        /// </summary>
        private void TxtSet()
        {
            long timeS = 0;
            string timeStr = "";
            timeS = TimeHelper.GetToTomorrowTime();
            timeStr = JiYuUIHelper.GeneralTimeFormat(new int4(2, 3, 2, 1), timeS);
            this.GetFromReference(KText_Countdown).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("sign_countdown_text").current + timeStr);
        }

        /// <summary>
        /// 更新函数
        /// </summary>
        private void update()
        {
           
            TxtSet();
        }

        /// <summary>
        /// 添加点击事件
        /// </summary>
        private void BtnInit()
        {
            var SignBtn = this.GetFromReference(KBtn);
            //每日签到按钮
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(SignBtn, () => { GetDailyBox(); });
            var BigBtn = this.GetFromReference(KBtn_BigBox);
            //累计签到
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(BigBtn, () => { GetBigBox(); });
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        private async void SetState()
        {
            var itemList = this.GetFromReference(KPos_Item).GetList();
            itemList.Clear();
            if (DailyCanSignOrNot == 0)
            {
                SignDayHelp = JudgeSignDay(thisSignDayNum);
                foreach (var re in tbsign_Daily.Get(SignDayHelp).reward)
                {
                    await itemList.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, re, false);
                }
            }
            else
            {
                SignDayHelp = JudgeSignDay(thisSignDayNum);
                foreach (var re in tbsign_Daily.Get(SignDayHelp).reward)
                {
                    var ui = await itemList.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, re, false);
                    JiYuUIHelper.SetRewardOnClick(re, ui);
                }
            }

            var nodeName = NodeNames.GetTagFuncRedDotName(3601);
            JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KPos_Item));
            if (DailyCanSignOrNot == 0)
            {
                this.GetFromReference(KBtn).SetActive(true);
                this.GetFromReference(KText_Next).SetActive(false);

                RedDotManager.Instance.SetRedPointCnt(nodeName, 1);
                this.GetFromReference(KImg_RedPoint).SetActive(true);
            }
            else
            {
                this.GetFromReference(KBtn).SetActive(false);
                this.GetFromReference(KText_Next).SetActive(true);

                RedDotManager.Instance.SetRedPointCnt(nodeName, 0);
                this.GetFromReference(KImg_RedPoint).SetActive(false);
                if (uiParentBtn != default)
                {
                    uiParentBtn.GetFromReference(UISubPanel_IconBtnItem.KImgRedDot).SetActive(false);
                }
            }

            if (accCanBeGetOrNot())
            {
                GetFromReference(KImg_Dialog)?.SetActive(false);
                GetFromReference(KImg_DialogGet).SetActive(true);
                string DialogStrHelp = tblanguage.Get("sign_acc_yes_text").current;
                DialogStrHelp = DialogStrHelp.Replace("{0}", CheckInDay.ToString());
                this.GetFromReference(KText_DialogGet).GetTextMeshPro().SetTMPText(DialogStrHelp);
            }
            else
            {
                List<Vector3> checkRe = new List<Vector3>();
                checkRe.Clear();
                GetFromReference(KImg_Dialog)?.SetActive(true);
                GetFromReference(KImg_DialogGet).SetActive(false);
                string DialogStrHelp = tblanguage.Get("sign_acc_no_text").current;
                DialogStrHelp = DialogStrHelp.Replace("{0}", CheckInDay.ToString());
                int CheckInDayHelp = (int)(CheckInDay % tbsign_Acc.DataList[tbsign_Acc.DataList.Count - 1].day);
                for (int i = 0; i < tbsign_Acc.DataList.Count; i++)
                {
                    if (tbsign_Acc.DataList[i].day <= CheckInDayHelp)
                    {
                        continue;
                    }
                    else
                    {
                        checkRe = tbsign_Acc.DataList[i].reward;
                        DialogStrHelp = DialogStrHelp.Replace("{1}",
                            (tbsign_Acc.DataList[i].day - CheckInDayHelp).ToString());
                    }
                }

                var rewardList = this.GetFromReference(Krewardcontainer).GetList();
                rewardList.Clear();
                foreach (var re in checkRe)
                {
                    var ui = await rewardList.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, re, false);
                    ui.GetRectTransform().SetScale(new Vector2(0.66f, 0.66f));
                    JiYuUIHelper.SetRewardOnClick(re, ui);
                }

                this.GetFromReference(KText_Dialog).GetTextMeshPro().SetTMPText(DialogStrHelp);
            }


            JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KImg_BigBox));
            await UniTask.Delay(200);
            if (this.GetFromReference(KBtn).GameObject.activeSelf)
            {
                JiYuTweenHelper.PlayUIImageSweepFX(this.GetFromReference(KBtn));
            }
        }

        /// <summary>
        /// 判断签到日期对应sign表的位置
        /// </summary>
        /// <param name="serverDay"></param>
        /// <returns></returns>
        private int JudgeSignDay(long serverDay)
        {
            if (serverDay >= 1 && serverDay <= dailyMaxDay)
            {
                return (int)serverDay;
            }
            else
            {
                return (int)(serverDay - (serverDay / dailyMaxDay) * dailyMaxDay);
            }
        }

        /// <summary>
        /// 点击每日签到的点击事件
        /// </summary>
        private void GetDailyBox()
        {
            if (DailyCanSignOrNot == 0)
            {
                WebMessageHandler.Instance.AddHandler(12, 2, OnDailResponse);
                LongValue longValue = new LongValue();
                longValue.Value = thisSignDayNum;
                NetWorkManager.Instance.SendMessage(12, 2, longValue);
                //await UniTask.Delay(2000);
            }
        }

        /// <summary>
        /// 每日签到回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDailResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(12, 2, OnDailResponse);
            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(stringValueList);
            if (e.data.IsEmpty)
            {
                JiYuUIHelper.ClearCommonResource();
                await UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("text_reward_expire").current);
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            List<Vector3> reList = new List<Vector3>();
            foreach (var itemstr in stringValueList.Values)
            {
                reList.Add(UnityHelper.StrToVector3(itemstr));
            }

            UIHelper.Create(UIType.UICommon_Reward, reList);
            WebMessageHandler.Instance.AddHandler(12, 1, OnSignResponse);
            NetWorkManager.Instance.SendMessage(12, 1);
            //Close();
        }

        /// <summary>
        /// 查询回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSignResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(12, 1, OnSignResponse);
            GameCheckIn gameCheckIn = new GameCheckIn();
            gameCheckIn.MergeFrom(e.data);
            Debug.Log(gameCheckIn);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.signData = gameCheckIn;
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Sign, out UI uuii))
            {
                SetData();
                SetState();
            }
        }

        /// <summary>
        /// 累积签到点击事件
        /// </summary>
        /// <param name="isClose"></param>
        private void GetBigBox(bool isClose = false)
        {
            if (accCanBeGetOrNot())
            {
                WebMessageHandler.Instance.AddHandler(12, 3, OnBigResponse);
                LongValue longValue = new LongValue();
                longValue.Value = thisSignDayNum;
                NetWorkManager.Instance.SendMessage(12, 3, longValue);
            }
            else
            {
                //没达成点击没有效果

                //if (isClose == true)
                //{
                //}
                //else
                //{
                //    List<Vector3> checkRe = new List<Vector3>();
                //    checkRe.Clear();
                //    int CheckInDayHelp = (int)(CheckInDay % tbsign_Acc.DataList[tbsign_Acc.DataList.Count - 1].day);
                //    for (int i = 0; i < tbsign_Acc.DataList.Count; i++)
                //    {
                //        if (tbsign_Acc.DataList[i].day <= CheckInDayHelp)
                //        {
                //            continue;
                //        }
                //        else
                //        {
                //            checkRe = tbsign_Acc.DataList[i].reward;
                //            break;
                //        }
                //    }

                //    //var tipUI = UIHelper.Create<List<Vector3>>(UIType.UICommon_Reward_Tip, checkRe,
                //    //    this.GetFromReference(KPos_Tip).GameObject.transform);
                //    //tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownUp).SetActive(false);
                //    //tipUI.GetFromReference(UICommon_Reward_Tip.KImg_ArrowDownLeft).SetActive(true);
                //    //this.GetFromReference(KBtn_Tips_Help).SetActive(true);
                //    //this.GetFromReference(KBtn_Tips_Help).GetXButton().OnClick?.Add(() =>
                //    //{
                //    //    tipUI.Dispose();
                //    //    this.GetFromReference(KBtn_Tips_Help).SetActive(false);
                //    //});
                //}
            }
        }

        /// <summary>
        /// 累积签到回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnBigResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(12, 3, OnBigResponse);
            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(stringValueList);
            if (e.data.IsEmpty)
            {
                JiYuUIHelper.ClearCommonResource();
                await UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("text_reward_expire").current);
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            List<Vector3> reList = new List<Vector3>();
            foreach (var itemstr in stringValueList.Values)
            {
                reList.Add(UnityHelper.StrToVector3(itemstr));
            }

            UIHelper.Create(UIType.UICommon_Reward, reList);
            WebMessageHandler.Instance.AddHandler(12, 1, OnSignResponse);
            NetWorkManager.Instance.SendMessage(12, 1);
            //SetData();
            //SetState();
        }

        /// <summary>
        /// 判断acc能否领取
        /// </summary>
        /// <returns></returns>
        private bool accCanBeGetOrNot()
        {
            bool accRe = false;
            if (DailyCanSignOrNot == 1)
            {
                if (BigCanSignOrNot == 0)
                {
                    int CheckInDayHelp = (int)(CheckInDay % tbsign_Acc.DataList[tbsign_Acc.DataList.Count - 1].day);
                    for (int i = 0; i < tbsign_Acc.DataList.Count; i++)
                    {
                        if (tbsign_Acc.DataList[i].day == CheckInDayHelp)
                        {
                            accRe = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    return accRe;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 领取全部礼包
        /// </summary>
        /// <param name="isClose"></param>
        private void GetAllBox(bool isClose = false)
        {
            GetDailyBox();
            //GetBigBox(isClose);
        }

        /// <summary>
        /// 删除update
        /// </summary>
        private void RemoveTimer()
        {
            if (this.timerId != 0)
            {
                var timerMgr = TimerManager.Instance;
                timerMgr?.RemoveTimerId(ref this.timerId);
                this.timerId = 0;
            }
            if (this.timerId0 != 0)
            {
                var timerMgr = TimerManager.Instance;
                timerMgr?.RemoveTimerId(ref this.timerId0);
                this.timerId0 = 0;
            }
        }

        protected override void OnClose()
        {
            RemoveTimer();
            //GetAllBox();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}