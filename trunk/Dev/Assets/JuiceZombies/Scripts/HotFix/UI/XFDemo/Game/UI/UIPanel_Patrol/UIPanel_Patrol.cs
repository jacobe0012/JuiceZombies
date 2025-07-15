//---------------------------------------------------------------------
// JiYuStudio
// Author: 迅捷蟹
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Spine.Unity;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Patrol)]
    internal sealed class UIPanel_PatrolEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Patrol;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Patrol>();
        }
    }

    public partial class UIPanel_Patrol : UI, IAwake
    {
        private Tblanguage tblanguage;
        private Tbconstant tbconstant;
        private Tbchapter tbchapter;
        private Tbuser_variable tbuser_varible;
        private int ConsumeValue;
        private Color InitTmpColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
        private Color LackColor = new Color(255 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);


        //private List<UICommon_RewardItem> itemList = new List<UICommon_RewardItem>();

        //关卡ID
        private int chapterID;
        private Tbquality tbquality;


        //测试时间戳
        private long TimeLine;
        private long DelateTime;

        //测试快速巡逻中的广告次数
        private int advertiseCount;
        private int energyCount;

        private int FrameNum = 0;
        private float AddValue = 0;
        private long timerId;
        private long playerEnergy;

        //自动巡逻奖励的list
        private List<Vector3> RewardList = new List<Vector3>();

        //快速巡逻奖励预览
        private List<Vector3> RewardList2 = new List<Vector3>();

        //收益
        private int normalHourMoney;
        private int normalHourExp;
        private int addHourMoney;
        private int addHourExp;


        public SkeletonGraphic palyerAnimaition;
        private UICommon_ItemTips tipPanel;
        private UI kSubPanel_CommonTips;

        private bool isAnimationActive;
        private int levelNum;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private long timerId0;

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            isAnimationActive = true;
            InitJsonFile();
            chapterID = ResourcesSingleton.Instance.levelInfo.maxPassChapterID;
            Debug.Log($"chapterID:{chapterID}");
            if (chapterID <= 0)
            {
                this.Close();
            }
            SetCloseTip(GetFromReference(KBg_Close));
            RemoveTimer();
            WebMessageHandler.Instance.AddHandler(CMD.INITPLAYER, OnReceiveAddVaule);
            WebMessageHandler.Instance.AddHandler(CMD.QUERYAUTOPATROL, OnReceiveAutoPatrol);
            NetWorkManager.Instance.SendMessage(CMD.INITPLAYER);
            InitWidgetAction();

            InitUIEffect();

        }

        private void SetUpdate()
        {
            var timerMgr = TimerManager.Instance;
            timerId0 = timerMgr.StartRepeatedTimer(2000, Update);
        }

        private void Update()
        {
            var KCommon_CloseInfo = GetFromReference(UIPanel_Patrol.KCommon_CloseInfo);
            KCommon_CloseInfo.GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_close").current);

            KCommon_CloseInfo.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KCommon_CloseInfo.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
        }

        public void SetCloseTip(UI ui)
        {
            ui.GetButton().OnClick?.Add(() => { JiYuUIHelper.DestoryAllTips(); ui.SetActive(false); });
        }
        private void InitUIEffect()
        {
            JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(KImgTop));

            var height1 = this.GetFromReference(KImage_AutoPatrol).GetRectTransform().AnchoredPosition().y;
            this.GetFromReference(KImage_AutoPatrol).GetComponent<CanvasGroup>().alpha = 0.5f;
            this.GetFromReference(KImage_RapidPatrol).GetComponent<CanvasGroup>().alpha = 0.5f;

            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(KImage_AutoPatrol), height1, 100, cts.Token, 0.3f,
                false,
                true);

            this.GetFromReference(KImage_AutoPatrol).GetComponent<CanvasGroup>().DOFade(1, 0.2f).SetEase(Ease.InQuad);
            this.GetFromReference(KImage_RapidPatrol).GetComponent<CanvasGroup>().DOFade(1, 0.2f).SetEase(Ease.InQuad);

            var height2 = this.GetFromReference(KImage_RapidPatrol).GetRectTransform().AnchoredPosition().y;
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(KImage_RapidPatrol), height2, 100, cts.Token,
                0.3f, false,
                true);


            var titleHeight1 = this.GetFromReference(KImage_AutoTitle).GetRectTransform().AnchoredPosition().y;
            var titleHeight2 = this.GetFromReference(KImage_RapidTitle).GetRectTransform().AnchoredPosition().y;
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(KImage_AutoTitle), titleHeight1, 20, cts.Token,
                0.3f, false,
                true);
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(KImage_RapidTitle), titleHeight2, 20, cts.Token,
                0.3f, false,
                true);
        }

        private void OnReceiveAddVaule(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.INITPLAYER, OnReceiveAddVaule);
            var gameRole = new GameRole();
            gameRole.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.UserInfo.PatrolGainName = gameRole.PatrolGainName;


            long timeValue = TimeHelper.ClientNowSeconds();
            var partorlValue = new LongValue();
            partorlValue.Value = timeValue;
            NetWorkManager.Instance.SendMessage(CMD.QUERYAUTOPATROL, partorlValue);
        }


        //按钮的注册方法
        private void InitWidgetAction()
        {
            GetFromReference(KBg_Patrol).GetXButton().RemoveAllListeners();
            GetFromReference(Kbtn_consume_strength).GetXButton().RemoveAllListeners();
            GetFromReference(Kbtn_consumeAD).GetXButton().RemoveAllListeners();
            GetFromReference(KBtnGet).GetXButton().RemoveAllListeners();
            GetFromReference(KImage_AutoMoney).GetXButton().RemoveAllListeners();
            GetFromReference(KImage_AutoExp).GetXButton().RemoveAllListeners();
            GetFromReference(KImage_AutoPatrol).GetXButton().RemoveAllListeners();
            GetFromReference(KImage_RapidPatrol).GetXButton().RemoveAllListeners();
            GetFromReference(KCloseMask).GetXButton().RemoveAllListeners();

            this.GetButton(KBg_Patrol)?.OnClick.Add(async () => await ClosePanel());
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(Kbtn_consume_strength),
                ConsumeStrengthToBuy);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(Kbtn_consumeAD), BuyADFunc);
            //this.GetButton()?.OnClick.Add(ConsumeStrengthToBuy);
            //this.GetButton(Kbtn_consumeAD)?.OnClick.Add(BuyADFunc);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtnGet), AutoPatrolFuc);

            this.GetButton(KImage_AutoMoney)?.OnClick.Add(() => AutoMoneyTipsFuncAsync().Forget());
            this.GetButton(KImage_AutoExp)?.OnClick.Add(() => AutoExpTipsFunc().Forget());
            this.GetButton(KImage_AutoPatrol)?.OnClick.Add(ClearAllTips);
            this.GetButton(KImage_RapidPatrol)?.OnClick.Add(ClearAllTips);
            this.GetButton(KCloseMask)?.OnClick.Add(ClearAllTips);
        }

        private async UniTask ClosePanel()
        {
            //JiYuTweenHelper.SetEaseAlphaAndPosUtoB(GetFromReference(UIPanel_Patrol.KPos_Patrol), 0 - 100, 100, 0.15f, false);
            //JiYuTweenHelper.SetEaseAlphaAndPosRtoL(GetFromReference(UIPanel_Patrol.KPos_Patrol), 0 - 100, 100, 0.15f, false);
            //GetFromReference(UIPanel_Patrol.KPos_Patrol).GetComponent<CanvasGroup>().alpha = 1f;
            //GetFromReference(UIPanel_Patrol.KPos_Patrol).GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetEase(Ease.InQuad);
            //await UniTask.Delay(150);
            ClearAllTips();
            Close();
        }


        //自动巡逻经验tips
        private async UniTaskVoid AutoExpTipsFunc()
        {
            var KImage_AutoExp = GetFromReference(UIPanel_Patrol.KImage_AutoExp);
            ClearAllTips();

            var list = KImage_AutoExp.GetList();
            list.Clear();


            var displayAdd = AddValue / 100f;
            //给控件初始化,文本说明的控件
            string tipStr = string.Format(tblanguage.Get("patrol_gain_name").current + "+"
                + (displayAdd).ToString() + "%" + "({0})", addHourExp - normalHourExp);
            //初始长度
            //float initWidth = childTipsUI.ReturnImgtIips();
            // await Task.Yield();
            if (!displayAdd.Equals(0))
            {
                tipStr = string.Format(tblanguage.Get("patrol_gain_name").current
                                       + "<color #" + tbquality.Get(2).fontColor + ">" +
                                       "+" + (displayAdd).ToString() + "%" + "({0})" + "</color>",
                    addHourExp - normalHourExp);
            }

            var tipUI = GetFromReference(KCommon_ItemTipsAutoExp);
            tipUI.SetActive(true);
            GetFromReference(KCommon_ItemTipsAutoMoney).SetActive(false);
            GetFromReference(KTxt_DesAutoExp).GetTextMeshPro().Get().alignment = TextAlignmentOptions.Center;
            GetFromReference(KTxt_DesAutoExp).GetTextMeshPro().SetTMPText(tipStr);
        }

        //自动巡逻金币tips
        private async UniTaskVoid AutoMoneyTipsFuncAsync()
        {
            //UIHelper.Remove(UIType.UICommon_ItemTips);
            ClearAllTips();


            var displayAdd = AddValue / 100f;
            string tipStr = string.Format(tblanguage.Get("patrol_gain_name").current + ":+"
                + (displayAdd).ToString() + "%" + "({0})", addHourMoney - normalHourMoney);
            if (!displayAdd.Equals(0))
            {
                tipStr = string.Format(tblanguage.Get("patrol_gain_name").current
                                       + "<color #" + tbquality.Get(2).fontColor + ">" +
                                       "+" + (displayAdd).ToString() + "%" + "({0})" + "</color>",
                    addHourMoney - normalHourMoney);
            }


            var tipUI = GetFromReference(KCommon_ItemTipsAutoMoney);
            tipUI.SetActive(true);
            GetFromReference(KCommon_ItemTipsAutoExp).SetActive(false);
            GetFromReference(KTxt_DesAutoMoney).GetTextMeshPro().SetTMPText(tipStr);
        }

        //自动巡逻
        private void AutoPatrolFuc()
        {
            //UIHelper.Remove(UIType.UICommon_ItemTips);
            ClearAllTips();
            WebMessageHandler.Instance.AddHandler(6, 4, OnReceiveAutoPatrolDrop);
            long timeValue = TimeHelper.ClientNowSeconds();
            var partorlValue = new LongValue();
            partorlValue.Value = timeValue;

            NetWorkManager.Instance.SendMessage(6, 4, partorlValue);
        }

        //快速巡逻,广告消耗方法
        private void BuyADFunc()
        {
            ClearAllTips();
            var intType = new IntValue();
            intType.Value = 1;

            WebMessageHandler.Instance.AddHandler(6, 1, OnReceiveRapidPatrol);
            NetWorkManager.Instance.SendMessage(6, 1, intType);
        }


        //快速巡逻,体力消耗方法
        private void ConsumeStrengthToBuy()
        {
            ClearAllTips();
            if (playerEnergy < tbconstant.Get("patrol_once_cost_energy").constantValue)
            {
                if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_BuyEnergy, out var ui))
                {
                    UIHelper.CreateAsync(UIType.UIPanel_BuyEnergy).Forget();
                }
            }
            else
            {
                var intType = new IntValue();
                intType.Value = 2;
                WebMessageHandler.Instance.AddHandler(6, 1, OnReceiveRapidPatrol);
                NetWorkManager.Instance.SendMessage(6, 1, intType);


                ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy -=
                    tbconstant.Get("patrol_once_cost_energy").constantValue;
                playerEnergy = ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy;
                ResourcesSingleton.Instance.UpdateResourceUI();
            }

            ClearAllTips();
        }

        //测试函数
        public void TestFunc()
        {
            //测试关卡ID
            //levelid = -1;
            //测试时间戳,单位为秒
            //TimeLine = 86395;
            //advertiseCount = 1;
            //energyCount = 3;
            playerEnergy = ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy;
            //测试加成
            //AddValue = 2600;
        }


        //初始化json文件
        public void InitJsonFile()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            tbuser_varible = ConfigManager.Instance.Tables.Tbuser_variable;
            tbquality = ConfigManager.Instance.Tables.Tbquality;
        }

        //初始化多语言文本,这个和控件状态无关
        public void InitLanguageWidget()
        {
            playerEnergy = ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy;

            #region 自动巡逻和快速巡逻标题tips

            GetFromReference(KText_Name).GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gain").current);
            GetFromReference(KTMP_AutoTips).GetTextMeshPro()
                .SetTMPText(tblanguage.GetOrDefault("patrol_auto_title").current);
            GetFromReference(KTMP_RapidTips).GetTextMeshPro().SetTMPText(tblanguage.Get("patrol_quick_title").current);
            //体力消耗数量
            GetFromReference(KText_PhyNum).GetTextMeshPro()
                .SetTMPText(tbconstant.Get("patrol_once_cost_energy").constantValue.ToString());

            int limitHour = tbconstant.Get("patrol_time_upperlimit").constantValue;
            limitHour = ReturnTime(limitHour, 3600.00f);
            string autoInfoStr = string.Format(tblanguage.Get("patrol_auto_text").current, limitHour);
            GetFromReference(KTMP_AutoInfo).GetTextMeshPro().SetTMPText(autoInfoStr);

            int limitMinute = tbconstant.Get("patrol_once_time").constantValue;
            limitMinute = ReturnTime(limitMinute, 60.00f);
            string rapidInfoStr = string.Format(tblanguage.Get("patrol_qucik_text").current, limitMinute);
            GetFromReference(KTMP_RapidInfo).GetTextMeshPro().SetTMPText(rapidInfoStr);

            #endregion

            #region 自动巡逻每小时收益

            //普通1小时收益
            normalHourMoney = tbchapter.Get(chapterID).money * 6;
            normalHourExp = tbchapter.Get(chapterID).exp * 6;

            var tenMinMoney = (int)(tbchapter.Get(chapterID).money * (1.0f + AddValue / 10000.0f));
            var tenMinExp = (int)(tbchapter.Get(chapterID).exp * (1.0f + AddValue / 10000.0f));

            //加成1小时的收益
            addHourMoney = tenMinMoney * 6;
            addHourExp = tenMinExp * 6;
            string str = "";
            if (!AddValue.Equals(0))
            {
                str = JiYuUIHelper.GetRewardTextIconName(JiYuUIHelper.GetVector3(JiYuUIHelper.Vector3Type.DOLLARS));
                str += "<color #" +
                       tbquality.Get(2).fontColor + ">" +
                       addHourMoney.ToString() + "</color>"
                       + "/" + tblanguage.Get("time_hour_2")
                           .current;
                GetFromReference(KTMP_AutoMoney).GetTextMeshPro().SetTMPText(str);

                str = JiYuUIHelper.GetRewardTextIconName(JiYuUIHelper.GetVector3(JiYuUIHelper.Vector3Type.EXP));
                str += "<color #" +
                       tbquality.Get(2).fontColor + ">" +
                       addHourExp.ToString() + "</color>" +
                       "/" + tblanguage.Get("time_hour_2").current;
                GetFromReference(KTMP_AutoExp).GetTextMeshPro().SetTMPText(str);
            }
            else
            {
                str = JiYuUIHelper.GetRewardTextIconName(JiYuUIHelper.GetVector3(JiYuUIHelper.Vector3Type.DOLLARS));
                str += normalHourMoney.ToString() + "/" + tblanguage.Get("time_hour_2").current;

                GetFromReference(KTMP_AutoMoney).GetTextMeshPro()
                    .SetTMPText(str);

                str = JiYuUIHelper.GetRewardTextIconName(JiYuUIHelper.GetVector3(JiYuUIHelper.Vector3Type.EXP));
                str += normalHourExp.ToString() + "/" + tblanguage.Get("time_hour_2").current;
                GetFromReference(KTMP_AutoExp).GetTextMeshPro()
                    .SetTMPText(str);
            }

            #endregion

            #region 初始化图标

            //金币图标
            //this.GetImage(KImage_MoneyIcon).SetSprite(tbuser_varible.Get(3).icon, false);

            ////经验图标
            //this.GetImage(KImage_ExpIcon).SetSprite(tbuser_varible.Get(4).icon, false);
            //体力图标
            str = JiYuUIHelper.GetRewardTextIconName(JiYuUIHelper.GetVector3(JiYuUIHelper.Vector3Type.ENEERGY));
            str += "15";
            this.GetTextMeshPro(KTMP_PhyNum).SetTMPText(str);
            //广告图标

            #endregion
        }


        public void InitMesage()
        {
            #region 初始化广告次数和体力可用次数

            ////对次数做限制,保证不超过每日次数
            //if (advertiseCount > tbconstant.Get("patrol_once_ad_times").constantValue)
            //{
            //    //this.Close();
            //    //string value = string.Format("当前广告次数为{0},广告次数异常!", advertiseCount);
            //    //UIHelper.Create(UIType.UIPanel_Tips, value, UILayer.Mid);
            //    //return;
            //}
            //else if (energyCount > tbconstant.Get("patrol_once_times_day").constantValue)
            //{
            //    //this.Close();
            //    //string value = string.Format("当前体力次数为{0},体力次数异常!", energyCount);
            //    //UIHelper.Create(UIType.UIPanel_Tips, value, UILayer.Mid);
            //    //return;
            //}

            var str = JiYuUIHelper.GetRewardTextIconName("icon_advertise");
            str += tblanguage.Get("common_free_text").current + "(" + advertiseCount.ToString() + ")";

            //免费广告次数
            GetFromReference(KText_ADGet).GetTextMeshPro()
                .SetTMPText(str);
            //体力可用次数
            if (energyCount > 0)
            {
                GetFromReference(KTMP_PhyInfo).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("patrol_times_text").current + energyCount.ToString());
                GetFromReference(KText_PhyNum).GetComponent<XTextMeshProUGUI>().color = InitTmpColor;
            }
            else
            {
                GetFromReference(KTMP_PhyInfo).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("patrol_times_text").current + 0);
            }

            if (playerEnergy < tbconstant.Get("patrol_once_cost_energy").constantValue)
            {
                //GetFromReference(KTMP_PhyInfo).GetTextMeshPro()
                //    .SetTMPText(tblanguage.Get("common_lack_1_text").current);
                GetFromReference(KText_PhyNum).GetComponent<XTextMeshProUGUI>().color = LackColor;
                GetFromReference(KText_PhyBuy).SetActive(true);
                GetFromReference(KText_PhyBuy).GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_buy").current);
                //this.GetRectTransform(KLeft_Pos).SetAnchoredPosition3D(Vector3.zero);
            }

            //Log.Debug((playerEnergy >= tbconstant.Get("patrol_once_cost_energy").constantValue).ToString(), Color.yellow);
            if (playerEnergy >= tbconstant.Get("patrol_once_cost_energy").constantValue)
            {
                GetFromReference(KText_PhyBuy).SetActive(false);
                //this.GetRectTransform(KLeft_Pos).SetAnchoredPosition3D(new Vector3(60, 0, 0));
            }

            #endregion


            #region 初始化按钮特定状态

            //看广告按钮
            UI freeBtn = GetFromReference(Kbtn_consumeAD);
            //体力按钮
            UI buyEnergyBtn = GetFromReference(Kbtn_consume_strength);
            //按钮上体力购买文字
            UI buyEnergyText = GetFromReference(KText_PhyBuy);

            GetFromReference(KImage_RewardInfo).SetActive(true);

            //免费和购买的的按钮container
            GetFromReference(KRightPos).SetActive(true);
            buyEnergyBtn.SetActive(true);
            var uimidText0 = GetFromReference(KMidText_Info);
            uimidText0.SetActive(false);


            if (advertiseCount > 0 && energyCount > 0)
            {
                RedDotManager.Instance.AddRedPointCnt(NodeNames.GetTagRedDotName(3502), 1);
                GetFromReference(KImage_RewardInfo2).SetActive(true);
                freeBtn.SetActive(true);

                buyEnergyBtn.GetXImage().SetGrayed(false);
            }
            else if (advertiseCount.Equals(0) && energyCount > 0)
            {
                RedDotManager.Instance.AddRedPointCnt(NodeNames.GetTagRedDotName(3502), 0);
                GetFromReference(KImage_RewardInfo2).SetActive(true);
                freeBtn.SetActive(false);

                buyEnergyBtn.GetXImage().SetGrayed(false);
            }
            else if (advertiseCount > 0 && energyCount.Equals(0))
            {
                RedDotManager.Instance.AddRedPointCnt(NodeNames.GetTagRedDotName(3502), 1);
                GetFromReference(KImage_RewardInfo2).SetActive(true);
                freeBtn.SetActive(true);

                buyEnergyBtn.GetXImage().SetGrayed(true);
            }
            else
            {
                RedDotManager.Instance.AddRedPointCnt(NodeNames.GetTagRedDotName(3502), 0);
                //这里记得关闭时恢复为默认
                //GetFromReference(KImage_RewardInfo2).SetActive(false);
                //GetFromReference(KRightPos).SetActive(false);
                var uimidText = GetFromReference(KMidText_Info);
                freeBtn.SetActive(false);
                buyEnergyBtn.GetXImage().SetGrayed(true);
                uimidText.SetActive(false);
                //uimidText.GetTextMeshPro().SetTMPText(tblanguage.Get("patrol_over_text").current);
            }

            #endregion


            #region 默认把小tips隐藏

            //kSubPanel_CommonTips = GetFromReference(KSubPanel_CommonTips);
            //kSubPanel_CommonTips.SetActive(false);

            #endregion


            #region 快速巡逻和奖励预览

            //清空子ui
            CreateRapidAward();

            #endregion
        }


        //返回所需时间,向下取整
        public int ReturnTime(int second, float Value)
        {
            var time = (int)(math.floor((float)second / Value));

            return time;
        }


        //判断时间是否达到上限,或者还没有到达10分钟,单位为秒
        public void ChageAutoPatrolState(long Timing)
        {
            GetFromReference(KBtnGet).SetActive(true);
            Log.Debug($"ChageAutoPatrolState:{TimeLine}", Color.cyan);
            //达到最大自动巡逻收益
            if (Timing >= tbconstant.Get("patrol_time_upperlimit").constantValue)
            {

                RedDotManager.Instance.SetRedPointCnt(NodeNames.GetTagRedDotName(3502),1);

                //按钮下的文字
                //var maxinfo=GetFromReference(km)
                var KTmp_infoUI = GetFromReference(KText_GetInfo);
                KTmp_infoUI.SetActive(false);

                GetFromReference(KText_GetIMax).SetActive(true);
                GetFromReference(KText_GetIMax).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("patrol_gain_max_text").current);
                //左上角控件文字更改
                GetFromReference(KTMP_TopAutoTips).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("patrol_time_max_text").current);

                GetFromReference(KBtnGet).GetXImage().SetGrayed(false);
            }

            else if (Timing < tbconstant.Get("patrol_unit_time").constantValue)
            {
                //按钮下的文字
                var Remainingtime = tbconstant.Get("patrol_unit_time").constantValue - Timing;
                string Timinstr = ReturnValue(Remainingtime);
                var KTmp_infoUI = GetFromReference(KText_GetInfo);
                KTmp_infoUI.SetActive(true);
                GetFromReference(KText_GetIMax).SetActive(false);
                KTmp_infoUI.GetTextMeshPro()
                    .SetTMPText(Timinstr + tblanguage.Get("patrol_gain_countdown_text").current);
                //右边领取按钮失活
                GetFromReference(KBtnGet).GetXImage().SetGrayed(true);
                GetFromReference(KBtnGet).GetButton().SetEnabled(false);
                //左上角控件文字更改
                string TimeStr = ReturnValue(Timing);

                GetFromReference(KTMP_TopAutoTips).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("patrol_time_text").current + TimeStr);
            }

            else if (Timing >= tbconstant.Get("patrol_unit_time").constantValue &&
                     Timing < tbconstant.Get("patrol_time_upperlimit").constantValue)

            {
                //按钮下的文字
                var KTmp_infoUI = GetFromReference(KText_GetInfo);
                //右边领取文字失活
                KTmp_infoUI.SetActive(false);
                GetFromReference(KText_GetIMax).SetActive(false);
                //右边按钮激活
                GetFromReference(KBtnGet).GetXImage().SetGrayed(false);
                GetFromReference(KBtnGet).GetButton().SetEnabled(true);
                //按钮下的文字
                string Timinstr = ReturnValue(Timing);

                //左上角文字更改

                GetFromReference(KTMP_TopAutoTips).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("patrol_time_text").current + Timinstr);
            }
        }


        //时间转文字,用于自动巡逻中
        public string ReturnValue(long second)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(second);

            if (second < 3600)
            {
                // 当 time＜3600 秒时，显示 AA分BB秒
                return string.Format("{0:D2}分{1:D2}秒", timeSpan.Minutes, timeSpan.Seconds);
            }
            else if (second <= 86400)
            {
                // 当 3600 ≤ time ≤ 86400 秒时，显示AA小时BB分
                return string.Format("{0:D2}小时{1:D2}分", timeSpan.Hours, timeSpan.Minutes);
            }
            else
            {
                // 当分钟数为0时,仅显示AA小时
                return string.Format("{0:D2}小时", timeSpan.Hours);
            }
        }


        private void XFUpdateAsync()
        {
            if (FrameNum.Equals(0))
            {
                Log.Debug($"timeline:{TimeLine}", Color.cyan);
                TimeLine += 1;
                FrameNum = 4;
                ChageAutoPatrolState(TimeLine);
            }
            else
            {
                FrameNum -= 1;
                return;
            }


            if ((TimeLine % tbconstant.Get("patrol_unit_time").constantValue).Equals(0) && TimeLine != 0 &&
                FrameNum == 4)

            {
                //TimerManager.Instance.WaitTill((DelateTime+ TimeHelper.ClientNowSeconds())*1000, DelayTask);
                //DelateTime = 0;

                DelayTask();
            }

            return;
        }

        private void DelayTask()
        {
            WebMessageHandler.Instance.AddHandler(6, 3, OnReceiveAutoPatrol2);

            long timeValue = TimeHelper.ClientNowSeconds();
            var partorlValue = new LongValue();
            partorlValue.Value = timeValue;

            NetWorkManager.Instance.SendMessage(6, 3, partorlValue);
        }

        private void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            timerMgr?.RemoveTimerId(ref this.timerId0);
            this.timerId0 = 0;
            this.timerId = 0;
            //FrameNum = 49;
        }

        private void AddTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(200, this.XFUpdateAsync);
            FrameNum = 0;
        }

        //快速巡逻奖励预览
        private async void CreateRapidAward()
        {
            //金币奖励预览
            var rapidAwardTime = tbconstant.Get("patrol_once_time").constantValue;
            var rapidUnitTime = tbconstant.Get("patrol_unit_time").constantValue;
            float TenMinAwardmoney = tbchapter.Get(chapterID).money;
            TenMinAwardmoney = (float)TenMinAwardmoney * (1 + AddValue / 100.0f);
            long Moneynum = (long)(TenMinAwardmoney * Mathf.Floor(rapidAwardTime / rapidUnitTime));
            var list = this.GetFromReference(KImage_RewardInfo2).GetList();
            list.Clear();
            var UIitem =
                await list.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, new Vector3(3, 0, Moneynum),
                    false) as UICommon_RewardItem;
            UIitem.GetComponent<RectTransform>().SetScale3(0.9f);
            AddClick(UIitem, new Vector3(3, 0, Moneynum));
            //经验奖励预览
            float TenMinAwardExp = tbchapter.Get(chapterID).exp;
            TenMinAwardExp = (float)TenMinAwardExp * (1 + AddValue / 100.0f);
            long Expnum = (long)(TenMinAwardExp * Mathf.Floor(rapidAwardTime / rapidUnitTime));
            var UIitem2 =
                await list.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, new Vector3(4, 0, Expnum), false)
                    as UICommon_RewardItem;
            UIitem2.GetComponent<RectTransform>().SetScale3(0.9f);
            AddClick(UIitem2, new Vector3(4, 0, Expnum));
            var ItemAward = tbchapter.Get(chapterID).patrolOnce.Count;
            List<Vector3> Awards = tbchapter.Get(chapterID).patrolOnce;
            Awards = Awards.OrderBy(v => v.y).ToList();

            //Log.Debug(Awards.Count.ToString(), Color.black);
            //随机经验图纸装备等奖励
            for (int i = 0; i < Awards.Count; i++)
            {
                long rewardnum = (long)Awards[i].z;
                if (rewardnum.Equals(0)) continue;
                var UIitem0 = await list.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem,
                    new Vector3(Awards[i].x, Awards[i].y, Awards[i].z), false) as UICommon_RewardItem;
                UIitem0.GetComponent<RectTransform>().SetScale3(0.9f);
                AddClick(UIitem0, new Vector3(Awards[i].x, Awards[i].y, Awards[i].z));
            }

            JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KImage_RewardInfo2));
        }


        //item点击事件
        private void AddClick(UI ui, Vector3 reward)
        {
            //this.GetFromReference(KSubPanel_CommonTips).SetActive(false);
            ClearAllTips();
            JiYuUIHelper.SetRewardOnClick(reward, ui,GetFromReference(KBg_Close));
        }


        //自动巡逻奖励预览
        private async void CreateAutoAward(long MoneyNum, long ExpNum, List<Vector3> Rewards)
        {
            //自动巡逻预览的父对象
            //var FatherPos = GetFromReference(KImage_RewardInfo);
            var list = GetFromReference(KImage_RewardInfo).GetList();
            list.Clear();
            //金币数量
            var UIitem =
                await list.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, new Vector3(3, 0, MoneyNum),
                    false) as UICommon_RewardItem;

            UIitem.GetComponent<RectTransform>().SetScale3(0.9f);
            //UIitem.SetParent(FatherPos, false);
            AddClick(UIitem, new Vector3(3, 0, MoneyNum));

            //经验数量
            var UIitem1 =
                await list.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, new Vector3(4, 0, ExpNum), false)
                    as UICommon_RewardItem;

            UIitem1.GetComponent<RectTransform>().SetScale3(0.9f);
            //UIitem1.SetParent(FatherPos, false);
            AddClick(UIitem1, new Vector3(4, 0, ExpNum));

            //随机奖励预览数量
            for (int i = 0; i < Rewards.Count; i++)
            {
                var UIitem0 = await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem,
                    new Vector3(Rewards[i].x, Rewards[i].y, Rewards[i].z), false) as UICommon_RewardItem;

                UIitem0.GetComponent<RectTransform>().SetScale3(0.9f);
                //UIitem0.SetParent(FatherPos, false);
                AddClick(UIitem0, new Vector3(Rewards[i].x, Rewards[i].y, Rewards[i].z));
            }

            JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KImage_RewardInfo));
        }

        //清空Tips;
        private void ClearAllTips()
        {
            JiYuUIHelper.DestoryAllTips();
            GetFromReference(KCommon_ItemTipsAutoExp).SetActive(false);
            GetFromReference(KCommon_ItemTipsAutoMoney).SetActive(false);
        }

        //初始化自动巡逻查询
        private void OnReceiveAutoPatrol(object sender, WebMessageHandler.Execute e)
        {
            var patrolRes = new PatrolRes();
            patrolRes.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                InitLanguageWidget();
                InitMesage();
                AddTimer();
                SetUpdate();
                Log.Debug("e.data.IsEmpty", Color.red);
                WebMessageHandler.Instance.RemoveHandler(CMD.QUERYAUTOPATROL, OnReceiveAutoPatrol);
                return;
            }

            var listRewardStrs = patrolRes.PatrolGain.PatrolOnce;

            //改变广告次数和体力次数
            advertiseCount = patrolRes.PatrolConfig.PatrolOnceAdTimes;
            energyCount = patrolRes.PatrolConfig.PatrolOnceTimesDay;
            AddValue = ResourcesSingleton.Instance.UserInfo.PatrolGainName;
            Log.Debug(
                $"AddValue:{AddValue}",
                Color.cyan);
            Log.Debug(
                $"ResourcesSingleton.Instance.UserInfo.PatrolGainName{ResourcesSingleton.Instance.UserInfo.PatrolGainName}",
                Color.cyan);

            var BackTimeLine = patrolRes.PatrolGain.PatrolTime;
            var MoenyNum = patrolRes.PatrolGain.Money;
            var ExpNum = patrolRes.PatrolGain.Exp;
            AddTimer();
            SetUpdate();
            //获取当前时间戳
            long NowTime = TimeHelper.ClientNowSeconds();

            TimeLine = NowTime - BackTimeLine;
            Log.Debug($"nowtime000:{NowTime},TimeLine000:{TimeLine},backtime000{BackTimeLine}", Color.cyan);
            if (TimeLine < 0) TimeLine = 0;
            InitLanguageWidget();
            InitMesage();

            //自动巡逻收益
            RewardList.Clear();
            foreach (var itemstr in listRewardStrs)
            {
                RewardList.Add(UnityHelper.StrToVector3(itemstr));
            }

            for (int i = 0; i < RewardList.Count; i++)
            {
                Log.Debug($"巡逻后端reward串:{RewardList[i]}", Color.cyan);
            }

            //生成预览
            if (MoenyNum != 0 && ExpNum != 0) CreateAutoAward(MoenyNum, ExpNum, RewardList);


            WebMessageHandler.Instance.RemoveHandler(6, 3, OnReceiveAutoPatrol);
        }

        //自动巡逻奖励
        private void OnReceiveAutoPatrolDrop(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(6, 4, OnReceiveAutoPatrolDrop);
            var patrolRes = new PatrolGain();
            patrolRes.MergeFrom(e.data);
            //测试代码
            var MoenyNum = patrolRes.Money;
            var ExpNum = patrolRes.Exp;

            long NowTime = TimeHelper.ClientNowSeconds();


            //TODO:领取弹版显示实际的收益
            var listRewardStrs = patrolRes.PatrolOnce;
            RewardList2.Clear();
            //添加金币
            RewardList2.Add(new Vector3(3, 0, MoenyNum));
            //添加经验
            RewardList2.Add(new Vector3(4, 0, ExpNum));
            //添加实际掉落
            foreach (var itemstr in listRewardStrs)
            {
                RewardList2.Add(UnityHelper.StrToVector3(itemstr));
            }

            UI CommonReWard = UIHelper.Create(UIType.UICommon_Reward, RewardList2);
            WebMessageHandler.Instance.AddHandler(CMD.QUERYAUTOPATROL, OnUpdatePatrolTime);
            NetWorkManager.Instance.SendMessage(CMD.QUERYAUTOPATROL);
            GetFromReference(KImage_RewardInfo).GetList().Clear();
            JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KImage_RewardInfo));
        }

        private void OnUpdatePatrolTime(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERYAUTOPATROL, OnUpdatePatrolTime);
            var patrolRes = new PatrolRes();
            patrolRes.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                InitLanguageWidget();
                InitMesage();
                AddTimer();
                SetUpdate();
                Log.Debug("e.data.IsEmpty", Color.red);
                WebMessageHandler.Instance.RemoveHandler(CMD.QUERYAUTOPATROL, OnReceiveAutoPatrol);
                return;
            }

            var listRewardStrs = patrolRes.PatrolGain.PatrolOnce;

            var BackTimeLine = patrolRes.PatrolGain.PatrolTime;
            //获取当前时间戳
            long NowTime = TimeHelper.ClientNowSeconds();
            TimeLine = NowTime - BackTimeLine;
            Log.Debug($"nowtime111:{NowTime},TimeLine111:{TimeLine},backtime111{BackTimeLine}", Color.cyan);
        }

        //快速巡逻奖励领取
        private void OnReceiveRapidPatrol(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(6, 1, OnReceiveRapidPatrol);
            var patrolRes = new PatrolRes();
            patrolRes.MergeFrom(e.data);
            var MoenyNum = patrolRes.PatrolGain.Money;
            var ExpNum = patrolRes.PatrolGain.Exp;
            int InitPhyCount = energyCount;

            //改变广告次数和体力次数
            advertiseCount = patrolRes.PatrolConfig.PatrolOnceAdTimes;
            energyCount = patrolRes.PatrolConfig.PatrolOnceTimesDay;
            var timeNum = patrolRes.PatrolGain.TimeNum;

            var AutoMaxTime = tbconstant.Get("patrol_time_upperlimit").constantValue;
            var AutoUnitTime = tbconstant.Get("patrol_unit_time").constantValue;
            var AutoMaxResTime = AutoMaxTime / AutoUnitTime;

            //TODO:领取弹版显示实际的收益
            var listRewardStrs = patrolRes.PatrolGain.PatrolOnce;
            List<Vector3> RewardListRes = new List<Vector3>();
            //添加金币
            RewardListRes.Add(new Vector3(3, 0, MoenyNum));
            //添加经验
            RewardListRes.Add(new Vector3(4, 0, ExpNum));
            //添加实际掉落
            foreach (var itemstr in listRewardStrs)
            {
                RewardListRes.Add(UnityHelper.StrToVector3(itemstr));
            }

            for (int i = 0; i < RewardListRes.Count; i++)
            {
                Log.Debug($"巡逻后端reward串:{RewardListRes[i]}", Color.cyan);
            }


            UI CommonReWard = UIHelper.Create(UIType.UICommon_Reward, RewardListRes);
            InitLanguageWidget();
            InitMesage();
            //红点取消
            if (advertiseCount.Equals(0) && timeNum < AutoMaxResTime && InitPhyCount.Equals(energyCount))
            {
                var uiManager = XFramework.Common.Instance.Get<UIManager>();
                //if (uiManager.TryGet(UIType.UIPanel_Battle, out var ui))
                //{
                //    var uiJiyuGame = ui as UIPanel_Battle;
                //    uiJiyuGame?.RedPoint();
                //}
            }
        }

        //自动巡逻查询
        private void OnReceiveAutoPatrol2(object sender, WebMessageHandler.Execute e)
        {
            var patrolRes = new PatrolRes();
            patrolRes.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                WebMessageHandler.Instance.RemoveHandler(6, 3, OnReceiveAutoPatrol2);
                return;
            }

            //Log.Debug(patrolRes.ToString(), Color.red);
            var listRewardStrs = patrolRes.PatrolGain.PatrolOnce;

            //改变广告次数和体力次数
            advertiseCount = patrolRes.PatrolConfig.PatrolOnceAdTimes;
            energyCount = patrolRes.PatrolConfig.PatrolOnceTimesDay;
            AddValue = ResourcesSingleton.Instance.UserInfo.PatrolGainName;

            var BackTimeLine = patrolRes.PatrolGain.PatrolTime;
            var MoenyNum = patrolRes.PatrolGain.Money;
            var ExpNum = patrolRes.PatrolGain.Exp;

            InitLanguageWidget();
            //自动巡逻收益
            RewardList.Clear();
            foreach (var itemstr in listRewardStrs)
            {
                RewardList.Add(UnityHelper.StrToVector3(itemstr));
            }

            for (int i = 0; i < RewardList.Count; i++)
            {
                Log.Debug($"巡逻后端reward串:{RewardList[i]}", Color.cyan);
            }


            //生成预览
            if (MoenyNum != 0 && ExpNum != 0) CreateAutoAward(MoenyNum, ExpNum, RewardList);

            WebMessageHandler.Instance.RemoveHandler(6, 3, OnReceiveAutoPatrol2);
        }


        protected override void OnClose()
        {
            isAnimationActive = false;

            RemoveTimer();
            ClearAllTips();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}