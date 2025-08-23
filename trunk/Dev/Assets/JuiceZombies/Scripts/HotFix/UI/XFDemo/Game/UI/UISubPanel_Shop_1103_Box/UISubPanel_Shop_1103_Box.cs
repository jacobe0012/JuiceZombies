//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Unity.Entities;
using UnityEngine;
using static XFramework.UISubPanel_Shop_Draw;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_1103_Box)]
    internal sealed class UISubPanel_Shop_1103_BoxEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_1103_Box;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_1103_Box>();
        }
    }

    public partial class UISubPanel_Shop_1103_Box : UI, IAwake<List<int>>
    {
        #region property

        //多语言表
        public Tblanguage language;

        //draw_box表
        private Tbdraw_box tbdraw_Box;

        //quality表
        private Tbquality tbquality;

        //item表
        private Tbitem tbitem;

        //
        public Tbuser_variable tbuser_Variable;

        //读表辅助string
        private string BoxDescHelpTxt = "drawbox_num{0}_text";

        //
        private DrawParam drawParamUp;
        private DrawParam drawParamMid;
        private DrawParam drawParamDown;

        private int diamondConsume = 0;
        private int keyID = 0;
        private long CDint = 0;
        private bool isAdvert = false;
        private int boxID = 0;
        private List<int> boxInt;

        private long timerId = 0;
        //private long advertCD = 0;

        //private long drawToNow = 0;
        public bool isKey = false;
        public bool KeyHelp = false;
        private int DrawCount = 0;

        private int Index = 0;
        private string boxString;

        private CancellationTokenSource cts;
        private List<UI> effectUIs = new List<UI>();
        #endregion

        public void Initialize(List<int> BoxInt)
        {
            cts = new CancellationTokenSource();
            GetFromReference(KMid).GetComponent<CanvasGroup>().alpha = 0f;
            this.GetFromReference(KText_FreeCountDown).SetActive(false);
            //advertCD = TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopInit.shopHelpDic[BoxInt[0]][BoxInt[1]][4];
            //drawToNow = TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopInit.shopHelpDic[BoxInt[0]][BoxInt[1]][4];
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(200, this.Update1103);

            boxID = BoxInt[1];
            boxInt = BoxInt;

            var boxList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList;

            int i = 0;
            foreach (var b in boxList)
            {
                if (b.Id == boxID)
                {
                    Index = i;
                    break;
                }

                i++;
            }
            //advertCD = TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].DrawTime;
            //drawToNow = TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].DrawTime;

            Init(boxID);
            InitRedNod(boxID);

            //RedPointMgr.instance.Init("ShopRoot", "module" + tbdraw_Box[boxID].tagFunc.ToString() + boxID.ToString(),
            //    (RedPointState state, int data) =>
            //    {
            //        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui))
            //        {
            //            if (this != null)
            //            {
            //                this.GetFromReference(KImg_AdvertRedDot).SetActive(state == RedPointState.Show);
            //            }
            //        }
            //    }, this.GetFromReference(KAdvertBtn).GetXButton());
            //RedPointMgr.instance.SetState("ShopRoot", tbdraw_Box[boxID].tagFunc.ToString() + boxID.ToString(), RedPointState.Show, 1);

            TxtInit(BoxInt);
            BtnInit(boxID);

            BtnOnClickInit();

            InitEffect(boxID);
        }
        public void AddEffect(UI effect)
        {
            if (effect != null)
            {
                effectUIs.Add(effect);
            }
        }
        private async void InitEffect(int boxID)
        {
            AddEffect(GetFromReference(KMid));
            await UniTask.Delay(500,cancellationToken:cts.Token);
            if (boxID % 2 == 0)
            {
               JiYuTweenHelper.SetEaseAlphaAndPosLtoR(GetFromReference(KMid),0,200,cts.Token);
            }
            else
            {
                JiYuTweenHelper.SetEaseAlphaAndPosRtoL(GetFromReference(KMid), 0, 200, cts.Token);
            }
            await UniTask.Delay(500, cancellationToken: cts.Token);
            JiYuTweenHelper.PlayUIImageSweepFX(GetFromReference(KBoxImage));
        }

        public void EnableEffectUIs(bool isEnable)
        {
            foreach (var ui in effectUIs)
            {
                ui.SetActive(isEnable);
            }
        }

        private void InitRedNod(int boxID)
        {
            int tagFuncID = 1103;
            if (boxID / 1000 == 1)
            {
                tagFuncID = 1101;
            }

            var redDotStr = NodeNames.GetTagFuncRedDotName(tagFuncID) + '|';
            boxString = redDotStr + boxID.ToString();
            RedDotManager.Instance.InsterNode(boxString);
        }

        /// <summary>
        /// 读表初始化
        /// </summary>
        /// <param name="BoxID"></param>
        private void Init(int BoxID)
        {
            DrawParam drawParam = new DrawParam();
            drawParam.BoxId = BoxID;
            drawParam.DrawCount = 1;
            drawParam.DrawType = 0;


            drawParamUp = new DrawParam();
            drawParamUp.BoxId = BoxID;
            drawParamUp.DrawCount = 1;
            drawParamUp.DrawType = 0;


            drawParamMid = new DrawParam();
            drawParamMid.BoxId = BoxID;
            drawParamMid.DrawCount = 1;
            drawParamMid.DrawType = 0;


            drawParamDown = new DrawParam();
            drawParamDown.BoxId = BoxID;
            drawParamDown.DrawCount = 1;
            drawParamDown.DrawType = 0;

            //读取多语言表
            language = ConfigManager.Instance.Tables.Tblanguage;
            //读取draw_Bos表
            tbdraw_Box = ConfigManager.Instance.Tables.Tbdraw_box;
            //读取quality表
            tbquality = ConfigManager.Instance.Tables.Tbquality;
            //读取item表
            tbitem = ConfigManager.Instance.Tables.Tbitem;
            //读取user_Variable表
            tbuser_Variable = ConfigManager.Instance.Tables.Tbuser_variable;
        }

        /// <summary>
        /// 文本初始化
        /// </summary>
        /// <param name="BoxID">宝箱ID</param>
        private async void TxtInit(List<int> BoxInt)
        {
            //后端读取次数
            //int a1 = (int)ResourcesSingleton.Instance.shopInit.shopHelpDic[BoxInt[0]][BoxInt[1]][0];
            int a1 = (int)ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].Numbers[0];
            int BoxID = BoxInt[1];

            this.GetFromReference(KBoxTitleText).GetTextMeshPro()
                .SetTMPText(language.Get(tbdraw_Box[BoxID].name).current);


            List<int> descQualityList = tbdraw_Box[BoxID].descPara;
            int descNum = descQualityList.Count;
            BoxDescHelpTxt = string.Format(BoxDescHelpTxt, descNum);
            string equipTxt = language.Get(BoxDescHelpTxt).current;
            for (int i = 0; i < descNum; i++)
            {
                string helpStr1 = language.Get(tbquality[descQualityList[i]].name).current;
                helpStr1 = UnityHelper.RichTextColor(helpStr1, tbquality[descQualityList[i]].fontColor);
                equipTxt = equipTxt.Replace("{" + i.ToString() + "}", helpStr1);
            }

            string moneyTxt = language.Get("drawbox_money_text").current;
            moneyTxt = moneyTxt.Replace("{0}", tbdraw_Box[BoxID].money.ToString());
            this.GetFromReference(KBoxDescText).GetTextMeshPro().SetTMPText(moneyTxt + "," + equipTxt);


            List<Vector2> guaranteePara = new List<Vector2>();
            guaranteePara = tbdraw_Box[BoxID].guaranteePara;
            int guaranteeLength = guaranteePara.Count;
            string helpStr2 = language.Get("drawbox_guarantee_text").current;
            helpStr2 = helpStr2.Replace("{0}", a1.ToString());
            if (guaranteeLength == 1)
            {
                helpStr2 = helpStr2.Replace("{1}", UnityHelper.RichTextColor(
                    language.Get(tbquality[(int)tbdraw_Box[BoxID].guaranteePara[0][0]].name).current,
                    tbquality[(int)tbdraw_Box[BoxID].guaranteePara[0][0]].fontColor));
            }
            else
            {
                Debug.Log("guaranteePara is wrong!");
            }

            this.GetFromReference(KBoxTimeText).GetTextMeshPro().SetTMPText(helpStr2);

            await this.GetFromReference(KBoxImage).GetImage().SetSpriteAsync(tbdraw_Box[BoxID].pic, false);
        }

        /// <summary>
        /// draw界面抽取后改变本界面的保底文本
        /// </summary>
        /// <param name="DrawInt"></param>
        public void DrawSetTxt(List<int> DrawInt)
        {
            //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][0] = DrawInt[0];
            ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].Numbers[0] = DrawInt[0];
            TxtInit(boxInt);
            BtnInit(boxID);
        }

        /// <summary>
        /// 向后端发送数据，并打开动画界面，后面添加
        /// </summary>
        private void BtnOnClickUp()
        {
            DrawCount = drawParamUp.DrawCount;
            WebMessageHandlerOld.Instance.AddHandler(11, 3, OnClickBoxBtnResponse);
            NetWorkManager.Instance.SendMessage(11, 3, drawParamUp);
            if (drawParamUp.DrawType == 1)
            {
                isAdvert = true;
                //CDint = tbdraw_Box[boxID].adCd;
            }

            if (KeyHelp == true)
            {
                isKey = false;
            }
        }

        private void BtnOnClickMid()
        {
            //drawParam
            DrawCount = drawParamMid.DrawCount;
            WebMessageHandlerOld.Instance.AddHandler(11, 3, OnClickBoxBtnResponse);
            NetWorkManager.Instance.SendMessage(11, 3, drawParamMid);
            if (drawParamMid.DrawType == 1)
            {
                isAdvert = true;
                //CDint = tbdraw_Box[boxID].adCd;
            }
            //WebMessageHandlerOld.Instance.RemoveHandler(11, 3, OnClickBoxBtnResponse);
        }

        private void BtnOnClickDown()
        {
            //drawParam
            DrawCount = drawParamDown.DrawCount;
            WebMessageHandlerOld.Instance.AddHandler(11, 3, OnClickBoxBtnResponse);
            NetWorkManager.Instance.SendMessage(11, 3, drawParamDown);
            if (drawParamDown.DrawType == 1)
            {
                isAdvert = true;
                //CDint = tbdraw_Box[boxID].adCd;
            }

            if (KeyHelp == true)
            {
                isKey = true;
            }

            #region Guide

            GuideFinish();

            #endregion
        }

        public void GuideFinish()
        {
            if (ResourcesSingleton.Instance.settingData.GuideList.Contains(1))
            {
                return;
            }
            var tbguide = ConfigManager.Instance.Tables.Tbguide;
            var guide1 = tbguide.DataList.Where(a => a.guideType == 317).FirstOrDefault();
            if (ResourcesSingleton.Instance.settingData.GuideList.Contains(guide1.group))
            {
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui))
                {
                    var uis = ui as UIPanel_Main;
                    uis.OnGuideIdFinished(guide1.id);
                }

                UIHelper.Remove(UIType.UISubPanel_Guid);
            }
        }

        private void BtnOnClickPre()
        {
            //var ui = await UIHelper.CreateAsync(UIType.UIChangeName, new ChangeNameHelp { gameRole = role, checkResult = checkResult }, UILayer.Mid);
            //ui.SetParent(this, true);

            var ui = UIHelper.Create<int>(UIType.UISubPanel_Shop_Pre, boxID);
        }

        private void OnClickBoxBtnResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 3, OnClickBoxBtnResponse);
            DrawInfo drawInfo = new DrawInfo();
            drawInfo.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(drawInfo);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            //var reStrs = drawInfo.Reward;
            //List<Vector3> RewardList = new List<Vector3>();
            //RewardList.Add(new Vector3(boxID, boxID, boxID));
            //foreach (var re in reStrs) 
            //{
            //    RewardList.Add(UnityHelper.StrToVector3(re));
            //}
            //var ui = UIHelper.Create<List<Vector3>>(UIType.UISubPanel_Shop_Draw, RewardList);
            //ui.SetParent(this, false);
            bool isAdvertHelp = false;

            if (isAdvert)
            {
                //advertCD = CDint;
                isAdvert = false;
                //BtinInit(boxID);
                //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][2] = 0;
                //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][4] = TimeHelper.ClientNowSeconds();
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].AdCount = 0;
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].DrawTime =
                    TimeHelper.ClientNowSeconds();
                //TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopInit.shopHelpDic[BoxInt[0]][BoxInt[1]][4];
                isAdvertHelp = true;
            }
            else
            {
                if (isKey == true)
                {
                    Debug.LogError($"drawcount:{DrawCount}");
                    //Debug.LogError($"countbefore:{ResourcesSingleton.Instance.items[tbdraw_Box[boxID].item]}");
                    if (JiYuUIHelper.TryReduceReward(new Vector3(5, tbdraw_Box[boxID].item, DrawCount)))
                    {
                        Debug.LogError($"count:{ResourcesSingleton.Instance.items[tbdraw_Box[boxID].item]}");
                    }
                    else
                    {
                        var str = string.Format(language.Get("common_lack").current,
                            language.Get(tbitem.Get(tbdraw_Box.Get(boxID).item).name).current);
                        UIHelper.CreateAsync(UIType.UICommon_Resource, str);
                        return;
                    }
                }
                else
                {
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin -= diamondConsume;
                }
            }

            //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][0] = drawInfo.Guarantee[0];
            ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].Numbers[0] =
                drawInfo.Guarantee[0];
            ResourcesSingleton.Instance.UpdateResourceUI();
            TxtInit(boxInt);
            BtnInit(boxID);
            var reStrs = drawInfo.Reward;
            List<Vector3> RewardList = new List<Vector3>();
            RewardList.Add(new Vector3(boxID, boxID, boxID));
            foreach (var re in reStrs)
            {
                RewardList.Add(UnityHelper.StrToVector3(re));
            }

            var ui = UIHelper.Create<ShopDrawHelp>(UIType.UISubPanel_Shop_Draw, new ShopDrawHelp
            {
                rewardList = RewardList,
                isAdvert = isAdvertHelp
            });
            ui.SetParent(this, false);
        }


        /// <summary>
        /// 点击事件初始化
        /// </summary>
        private void BtnOnClickInit()
        {
            //this.GetFromReference(KAdvertBtn).GetButton().OnClick.Add(BtnOnClick);
            //this.GetFromReference(KMidBuyBtn).GetButton().OnClick.Add(BtnOnClick);
            //this.GetFromReference(KBuyBtn).GetButton().OnClick.Add(BtnOnClick);


            var adBtn = this.GetFromReference(KAdvertBtn);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(adBtn, BtnOnClickUp);
            var midBtn = this.GetFromReference(KMidBuyBtn);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(midBtn, BtnOnClickMid);
            var buyBtn = this.GetFromReference(KBuyBtn);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(buyBtn, BtnOnClickDown);
            var previewBtn = this.GetFromReference(KPreviewBtn);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(previewBtn, BtnOnClickPre);
            var btn = this.GetFromReference(KBgBoxImage);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, BtnOnClickPre);
        }

        /// <summary>
        /// 按钮初始化
        /// </summary>
        /// <param name="BoxID">宝箱ID</param>
        private void BtnInit(int BoxID)
        {
            //-------------------------------------------------------------------------------------------------
            int tagFuncID = 1103;
            var redDotStr = NodeNames.GetTagFuncRedDotName(tagFuncID) + '|';
            boxString = redDotStr + boxID.ToString();
            RedDotManager.Instance.InsterNode(boxString);
            RedDotManager.Instance.SetRedPointCnt(boxString, 0);
            GetFromReference(KImg_AdvertRedDot)?.SetActive(false);


            int type = tbdraw_Box[BoxID].drawType;

            //读后端
            int advertNum;
            //advertCD = ResourcesSingleton.Instance.shopInit.shopHelpDic[BoxInt[0]][BoxInt[1]][3];
            //if (advertCD > 0)
            //{
            //    advertNum = 0;
            //}
            //else
            //{
            //    advertNum = 1;
            //}
            //advertNum = (int)ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][2];
            advertNum = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].AdCount;
            if (advertNum == 0)
            {
                if (TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103]
                        .BoxInfoList[Index].DrawTime >= tbdraw_Box[BoxID].adCd)
                {
                    advertNum = 1;
                }
            }
            //advertNum = 0;

            if (advertNum > 0)
            {
                Log.Debug("hhhhhhhhhhhhhhhh");
                //RedDotManager.Instance.InsterNode(boxString);
                RedDotManager.Instance.SetRedPointCnt(boxString, 1);
                GetFromReference(KImg_AdvertRedDot)?.SetActive(true);
            }
            else
            {
                RedDotManager.Instance.SetRedPointCnt(boxString, 0);
                GetFromReference(KImg_AdvertRedDot)?.SetActive(false);
            }

            //var tag = RedDotManager.Instance.GetNode( NodeNames.GetTagRedDotName(1));
            //tag.PrintTree();


            int diamondConsume = tbdraw_Box[BoxID].one;

            int keyConsume;
            if (tbdraw_Box[BoxID].item > 0)
            {
                keyConsume = 1;
            }
            else
            {
                keyConsume = 0;
            }

            long keyNum;
            if (ResourcesSingleton.Instance.items.TryGetValue(tbdraw_Box[BoxID].item, out long value))
            {
                keyNum = value;
            }
            else
            {
                keyNum = 0;
            }
            //Debug.Log("keyNum : " + keyNum.ToString());


            BtnStateContext context = new BtnStateContext(new Type5NoAdvertWithoutKey());
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type5HaveAdvertWithoutKey();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type5NoAdvertWithKey();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type5HaveAdvertWithKey();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type1WithKey();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type1WithoutKey();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type2WithKey();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type2WithoutKey();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type4KeyFew();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type4KeyMany();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type4OnlyKey();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
            context.btnStateStrategy = new Type4WithOutKey();
            context.ChangeState(this, type, advertNum, diamondConsume, keyConsume, keyNum, BoxID);
        }

        private void Update1103()
        {
            //Debug.Log(1);
            //Debug.Log(advertCD);
            //if (drawToNow <= tbdraw_Box[boxID].adCd)
            //{
            //    drawToNow = TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][4];
            //    //Debug.Log("drawToNow" + drawToNow.ToString());
            //    //advertCD -= 1;
            //}
            //else
            //{
            //    ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][2] = 1;
            //    BtinInit(boxID);
            //}
            //CDint = advertCD;

            if (tbdraw_Box.Get(boxID).drawType == 5)
            {
                if (ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].AdCount <= 0)
                {
                    if (tbdraw_Box.Get(boxID).adCd > TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopMap
                            .IndexModuleMap[1103].BoxInfoList[Index].DrawTime)
                    {
                        long cdTime = tbdraw_Box.Get(boxID).adCd;
                        cdTime -= TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopMap
                            .IndexModuleMap[1103].BoxInfoList[Index].DrawTime;
                        string timeStr = "";
                        if (cdTime > 3600 * 24)
                        {
                            timeStr += UnityHelper.RichTextSize(UnityHelper.RichTextColor(
                                JiYuUIHelper.GeneralTimeFormat(new Unity.Mathematics.int4(3, 4, 2, 1), cdTime),
                                "5DC642"), 39);
                        }
                        else
                        {
                            timeStr += UnityHelper.RichTextSize(UnityHelper.RichTextColor(
                                JiYuUIHelper.GeneralTimeFormat(new Unity.Mathematics.int4(2, 3, 2, 1), cdTime),
                                "5DC642"), 39);
                        }

                        timeStr += "\n";
                        timeStr += language.Get("drawbox_free_countdown_text").current;
                        this.GetFromReference(KText_FreeCountDown).GetTextMeshPro().SetTMPText(timeStr);
                    }
                    else
                    {
                        ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList[Index].AdCount = 1;
                        BtnInit(boxID);
                    }
                }
            }
        }

        public sealed class BtnStateContext
        {
            private BtnStateStrategy _strategy;

            public BtnStateContext(BtnStateStrategy strategy)
            {
                _strategy = strategy;
            }

            public BtnStateStrategy btnStateStrategy
            {
                get { return _strategy; }
                set { _strategy = value; }
            }

            /// <summary>
            /// 传入按钮的各种参数
            /// </summary>
            /// <param name="ui">this</param>
            /// <param name="type">抽奖类型</param>
            /// <param name="advertNum">广告数量</param>
            /// <param name="diamondConsume">钻石消耗</param>
            /// <param name="keyConsume">钥匙消耗</param>
            /// <param name="keyNum">钥匙数量</param>
            /// <param name="boxID">宝箱ID</param>
            public void ChangeState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                _strategy.ChangeBtnState(ui, type, advertNum, diamondConsume, keyConsume, keyNum, boxID);
            }
        }

        public interface BtnStateStrategy
        {
            void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID);
        }

        /// <summary>
        /// 类型5，没有广告次数，钻石
        /// </summary>
        public sealed class Type5NoAdvertWithoutKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 5 && advertNum <= 0 && diamondConsume > 0 && (keyConsume <= 0 || keyNum <= 0))
                {
                    ui.GetFromReference(KText_FreeCountDown).SetActive(true);
                    //Debug.Log("类型5, 只用钻石不用钥匙，没有广告");
                    ui.GetFromReference(KAdvertBtn).SetActive(false);
                    ui.GetFromReference(KBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyBtn).SetActive(false);
                    //await ui.GetFromReference(KMidBuyImg_Left).GetImage().SetSpriteAsync(ui.tbuser_Variable[2].icon, false);
                    //ui.GetFromReference(KMidBuyText_Right).GetTextMeshPro().SetTMPText(diamondConsume.ToString());
                    ui.GetFromReference(KBuyText_Mid).GetTextMeshPro().SetTMPText(
                        JiYuUIHelper.GetRewardTextIconName(ui.tbuser_Variable[2].icon) + diamondConsume.ToString());

                    //修改点击事件参数
                    ui.drawParamDown.DrawCount = 1;
                    ui.drawParamDown.DrawType = 0;

                    ui.diamondConsume = diamondConsume;
                    ui.isKey = false;
                    ui.KeyHelp = false;

                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ui.tbdraw_Box[boxID].tagFunc.ToString() + boxID.ToString(), RedPointState.Hide, 0);
                }
            }
        }

        /// <summary>
        /// 类型5，没有广告次数，key
        /// </summary>
        public sealed class Type5NoAdvertWithKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 5 && advertNum <= 0 &&
                    ((keyConsume > 0 && keyNum > 0) || (diamondConsume <= 0 && keyConsume > 0)))
                {
                    ui.GetFromReference(KText_FreeCountDown).SetActive(true);
                    //Debug.Log("类型5, 用钥匙，没有广告");
                    ui.GetFromReference(KAdvertBtn).SetActive(false);
                    ui.GetFromReference(KBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyBtn).SetActive(false);
                    //await ui.GetFromReference(KMidBuyImg_Left).GetImage().SetSpriteAsync(ui.tbitem[ui.tbdraw_Box[boxID].item].icon, false);
                    //if (keyNum >= 10)
                    //{
                    //    ui.GetFromReference(KMidBuyText_Right).GetTextMeshPro().SetTMPText(keyNum.ToString() + "/" + (keyConsume * 10).ToString());
                    //    ui.drawParamMid.DrawCount = 10;
                    //}
                    //else
                    //{
                    //    ui.GetFromReference(KMidBuyText_Right).GetTextMeshPro().SetTMPText(keyNum.ToString() + "/" + keyConsume.ToString());
                    //    ui.drawParamMid.DrawCount = 1;
                    //}


                    if (keyNum >= 10)
                    {
                        ui.GetFromReference(KBuyText_Mid).GetTextMeshPro()
                            .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                        keyNum.ToString() + "/" + (keyConsume * 10).ToString());
                        ui.drawParamMid.DrawCount = 10;
                    }
                    else
                    {
                        ui.GetFromReference(KBuyText_Mid).GetTextMeshPro()
                            .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                        keyNum.ToString() + "/" + keyConsume.ToString());
                        ui.drawParamMid.DrawCount = 1;
                    }

                    //修改点击事件参数
                    ui.drawParamDown.DrawType = 0;
                    ui.diamondConsume = 0;
                    ui.isKey = true;
                    ui.KeyHelp = false;
                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ui.tbdraw_Box[boxID].tagFunc.ToString() + boxID.ToString(), RedPointState.Hide, 0);
                }
            }
        }

        /// <summary>
        /// 类型5，有广告次数，钻石
        /// </summary>
        public sealed class Type5HaveAdvertWithoutKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 5 && advertNum > 0 && diamondConsume > 0 && (keyConsume <= 0 || keyNum <= 0))
                {
                    ui.GetFromReference(KText_FreeCountDown).SetActive(false);
                    //Debug.Log("类型5, 只用钻石不用钥匙，有广告");
                    ui.GetFromReference(KAdvertBtn).SetActive(true);
                    ui.GetFromReference(KBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyBtn).SetActive(false);

                    ui.GetFromReference(KBuyText_Mid).GetTextMeshPro().SetTMPText(
                        JiYuUIHelper.GetRewardTextIconName(ui.tbuser_Variable[2].icon) + diamondConsume.ToString());
                    ui.GetFromReference(KAdvertText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName("icon_advertise") +
                                    ui.language.Get("common_free_text").current);

                    //修改点击事件参数
                    ui.drawParamUp.DrawCount = 1;
                    ui.drawParamUp.DrawType = 1;

                    ui.drawParamDown.DrawType = 0;
                    ui.drawParamDown.DrawCount = 1;

                    ui.diamondConsume = diamondConsume;
                    ui.isKey = false;
                    ui.KeyHelp = false;
                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ui.tbdraw_Box[boxID].tagFunc.ToString() + boxID.ToString(), RedPointState.Show, 1);
                }
            }
        }

        /// <summary>
        /// 类型5，有广告次数，key
        /// </summary>
        public sealed class Type5HaveAdvertWithKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 5 && advertNum > 0 &&
                    ((keyConsume > 0 && keyNum > 0) || (diamondConsume <= 0 && keyConsume > 0)))
                {
                    ui.GetFromReference(KText_FreeCountDown).SetActive(false);
                    //Debug.Log("类型5, 用钥匙，有广告");
                    ui.GetFromReference(KAdvertBtn).SetActive(true);
                    ui.GetFromReference(KBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyBtn).SetActive(false);

                    ui.GetFromReference(KAdvertText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName("icon_advertise") +
                                    ui.language.Get("common_free_text").current);
                    if (keyNum >= 10)
                    {
                        ui.GetFromReference(KBuyText_Mid).GetTextMeshPro()
                            .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                        keyNum.ToString() + "/" + (keyConsume * 10).ToString());
                        ui.drawParamDown.DrawCount = 10;
                    }
                    else
                    {
                        ui.GetFromReference(KBuyText_Mid).GetTextMeshPro()
                            .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                        keyNum.ToString() + "/" + keyConsume.ToString());
                        ui.drawParamDown.DrawCount = 1;
                    }

                    //修改点击事件参数
                    ui.drawParamUp.DrawCount = 1;
                    ui.drawParamUp.DrawType = 1;

                    ui.drawParamDown.DrawType = 0;
                    ui.diamondConsume = 0;

                    ui.isKey = true;
                    ui.KeyHelp = false;
                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ui.tbdraw_Box[boxID].tagFunc.ToString() + boxID.ToString(), RedPointState.Show, 1);
                }
            }
        }

        /// <summary>
        /// 类型1，仅单抽，key
        /// </summary>
        public sealed class Type1WithKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 1 && ((keyConsume > 0 && keyNum > 0) || (diamondConsume <= 0 && keyConsume > 0)))
                {
                    ui.GetFromReference(KAdvertBtn).SetActive(false);
                    ui.GetFromReference(KBuyBtn).SetActive(false);
                    ui.GetFromReference(KMidBuyBtn).SetActive(true);

                    if (keyNum >= 10)
                    {
                        ui.GetFromReference(KMidBuyText_Mid).GetTextMeshPro()
                            .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                        keyNum.ToString() + "/" + (keyConsume * 10).ToString());
                    }
                    else
                    {
                        ui.GetFromReference(KMidBuyText_Mid).GetTextMeshPro()
                            .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                        keyNum.ToString() + "/" + keyConsume.ToString());
                    }

                    ui.isKey = true;
                    ui.KeyHelp = false;
                }
            }
        }

        /// <summary>
        /// 类型1，仅单抽，钻石
        /// </summary>
        public sealed class Type1WithoutKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 1 && diamondConsume > 0 && (keyConsume <= 0 || keyNum <= 0))
                {
                    ui.GetFromReference(KAdvertBtn).SetActive(false);
                    ui.GetFromReference(KBuyBtn).SetActive(false);
                    ui.GetFromReference(KMidBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyText_Mid).GetTextMeshPro().SetTMPText(
                        JiYuUIHelper.GetRewardTextIconName(ui.tbuser_Variable[2].icon) + diamondConsume.ToString());
                    ui.isKey = false;
                    ui.KeyHelp = false;
                }
            }
        }

        /// <summary>
        /// 类型2，仅十抽，key
        /// </summary>
        public sealed class Type2WithKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 2 && ((keyConsume > 0 && keyNum > 0) || (diamondConsume <= 0 && keyConsume > 0)))
                {
                    ui.GetFromReference(KAdvertBtn).SetActive(false);
                    ui.GetFromReference(KBuyBtn).SetActive(false);
                    ui.GetFromReference(KMidBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                    keyNum.ToString() + "/" + (keyConsume * 10).ToString());
                    ui.isKey = true;
                    ui.KeyHelp = false;
                }
            }
        }

        /// <summary>
        /// 类型2，仅十抽，钻石
        /// </summary>
        public sealed class Type2WithoutKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 2 && diamondConsume > 0 && (keyConsume <= 0 || keyNum <= 10))
                {
                    ui.GetFromReference(KAdvertBtn).SetActive(false);
                    ui.GetFromReference(KBuyBtn).SetActive(false);
                    ui.GetFromReference(KMidBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyText_Mid).GetTextMeshPro().SetTMPText(
                        JiYuUIHelper.GetRewardTextIconName(ui.tbuser_Variable[2].icon) + diamondConsume.ToString());
                    ui.isKey = false;
                    ui.KeyHelp = false;
                }
            }
        }

        /// <summary>
        /// 类型4，不能用key
        /// </summary>
        public sealed class Type4WithOutKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 4 && diamondConsume > 0 && (keyConsume <= 0 || keyNum <= 0))
                {
                    ui.GetFromReference(KAdvertBtn).SetActive(true);
                    ui.GetFromReference(KBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyBtn).SetActive(false);
                    ui.GetFromReference(KBuyText_Mid).GetTextMeshPro().SetTMPText(
                        JiYuUIHelper.GetRewardTextIconName(ui.tbuser_Variable[2].icon) + diamondConsume.ToString());
                    ui.GetFromReference(KAdvertText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbuser_Variable[2].icon) +
                                    ui.tbdraw_Box[boxID].ten.ToString());
                    ui.isKey = false;
                    ui.KeyHelp = false;
                }
            }
        }

        /// <summary>
        /// 类型4，只能用key
        /// </summary>
        public sealed class Type4OnlyKey : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 4 && diamondConsume <= 0 && keyConsume > 0)
                {
                    ui.GetFromReference(KAdvertBtn).SetActive(true);
                    ui.GetFromReference(KBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyBtn).SetActive(false);

                    ui.GetFromReference(KBuyText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                    keyNum.ToString() + "/" + keyConsume.ToString());
                    ui.GetFromReference(KAdvertText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                    keyNum.ToString() + "/" + (keyConsume * 10).ToString());
                    ui.isKey = true;
                    ui.KeyHelp = false;
                }
            }
        }

        /// <summary>
        /// 类型4，都可以用，但是key小于10
        /// </summary>
        public sealed class Type4KeyFew : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 4 && diamondConsume > 0 && keyConsume > 0 && keyNum < 10 && keyNum > 0)
                {
                    ui.GetFromReference(KAdvertBtn).SetActive(true);
                    ui.GetFromReference(KBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyBtn).SetActive(false);
                    ui.GetFromReference(KBuyText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                    keyNum.ToString() + "/" + keyConsume.ToString());
                    ui.GetFromReference(KAdvertText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbuser_Variable[2].icon) +
                                    ui.tbdraw_Box[boxID].ten.ToString());
                    ui.KeyHelp = true;
                }
            }
        }

        /// <summary>
        /// 类型4，都可以用，但是key大于等于10
        /// </summary>
        public sealed class Type4KeyMany : UI, BtnStateStrategy
        {
            public async void ChangeBtnState(UISubPanel_Shop_1103_Box ui, int type, int advertNum, int diamondConsume,
                int keyConsume, long keyNum, int boxID)
            {
                if (type == 4 && diamondConsume > 0 && keyConsume > 0 && keyNum >= 0)
                {
                    ui.GetFromReference(KAdvertBtn).SetActive(true);
                    ui.GetFromReference(KBuyBtn).SetActive(true);
                    ui.GetFromReference(KMidBuyBtn).SetActive(false);
                    ui.GetFromReference(KBuyText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                    keyNum.ToString() + "/" + keyConsume.ToString());
                    ui.GetFromReference(KAdvertText_Mid).GetTextMeshPro()
                        .SetTMPText(JiYuUIHelper.GetRewardTextIconName(ui.tbitem[ui.tbdraw_Box[boxID].item].icon) +
                                    keyNum.ToString() + "/" + (keyConsume * 10).ToString());
                    ui.isKey = true;
                    ui.KeyHelp = false;
                }
            }
        }

        public void RemoveTimer()
        {
            if (this.timerId != 0)
            {
                Debug.Log("RemoveTimer");
                var timerMgr = TimerManager.Instance;
                timerMgr?.RemoveTimerId(ref this.timerId);
                this.timerId = 0;
            }
        }

        protected override void OnClose()
        {
            RemoveTimer();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}