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
using DG.Tweening;
using dnlib.Threading;
using Google.Protobuf;
using HotFix_UI;
using Main;
using NUnit.Framework;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using static XFramework.UISubPanel_Shop_Draw;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_1102_SBox)]
    internal sealed class UISubPanel_Shop_1102_SBoxEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_1102_SBox;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_1102_SBox>();
        }
    }

    public partial class UISubPanel_Shop_1102_SBox : UI, IAwake<List<int>>
    {
        #region property

        //多语言表
        private Tblanguage language;

        //draw_box表
        private Tbdraw_box tbdraw_Box;

        //quality表
        private Tbquality tbquality;

        //item表
        private Tbitem tbitem;

        //Tbuser_variable表
        private Tbuser_variable tbuser_Variable;

        //读表辅助string
        private string BoxDescHelpTxt = "drawbox_num{0}_text";


        private long keyNum = 0;
        private int drawNum = 0;

        private DrawParam drawParamLeft;
        private DrawParam drawParamRight;

        //private int SSize = 130;
        private int TimeSSize = 55;
        private int boxID;
        private List<int> boxInt;
        private int ModuleID = 0;

        private long timerId = 0;
        private long ThisSetStartTime = 0;

        private string boxString;

        #endregion

        private CancellationTokenSource cts = new CancellationTokenSource();
        private UIPanel_Shop parentUI;


        private List<UI> effectUIs = new List<UI>();

        public void Initialize(List<int> BoxInt)
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui))
            {
                parentUI = ui as UIPanel_Shop;
            }

            AddEffectUI();

            boxInt = BoxInt;
            ModuleID = BoxInt[0];
            boxID = BoxInt[1];

            DrawParam drawParam = new DrawParam();
            drawParam.BoxId = boxID;
            drawParam.DrawCount = 1;
            drawParam.DrawType = 0;
            drawParamLeft = new DrawParam();
            drawParamLeft.BoxId = boxID;
            drawParamLeft.DrawCount = 1;
            drawParamLeft.DrawType = 0;
            drawParamRight = drawParam;
            drawParamRight.DrawCount = 10;
            Debug.Log(boxID);


            TxtInit(boxID);

            InitRedDot(boxID);


            BtnTxtInit(boxID, false);
            BtnOnClickInit();
            this.GetFromReference(KLimitedTimeBoxBg).GetImage().SetSpriteAsync(tbdraw_Box.Get(boxID).pic, false)
                .Forget();
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out ui))
            {
                var uis = ui as UIPanel_Shop;
                if (uis.tagsTimes[0] >= 0)
                {
                    GetFromReference(Kbox).GetImage().SetAlpha(0);
                    //await UniTask.Delay(550, cancellationToken: cts.Token);
                    InitEffect().Forget();
                }
                else
                {
                    GetFromReference(Kbox).GetImage().SetAlpha(1);
                    EnableEffectUIs(true);
                }
               
            }
        }

        private void AddEffectUI()
        {
            var box = GetFromReference(Kbox);
            var tenBtn = this.GetFromReference(KTenTimesBtn);
            var oneBtn = this.GetFromReference(KOneTimeBtn);
            var lessBtn = this.GetFromReference(KBtn_LessThanTen);
            var titleDes = GetFromReference(KContainerDesc);
            var bg = GetFromReference(UISubPanel_Shop_1102_SBox.KLimitedTimeBoxBg);
            effectUIs.Clear();
            AddEffect(box, 550);
            AddEffect(tenBtn, 550);
            AddEffect(oneBtn, 550);
            AddEffect(lessBtn, 550);
            AddEffect(titleDes, 550);
            AddEffect(bg, 550);
        }

        public void AddEffect(UI effect, int delayTime)
        {
            if (effect != null)
            {
                effectUIs.Add(effect);
            }
            else
            {
            }
        }

        private async UniTaskVoid InitEffect()
        {
            await UniTask.Delay(550, cancellationToken: cts.Token);
            var box = GetFromReference(Kbox);
            var tenBtn = this.GetFromReference(KTenTimesBtn);
            var oneBtn = this.GetFromReference(KOneTimeBtn);
            var lessBtn = this.GetFromReference(KBtn_LessThanTen);
            var titleDes = GetFromReference(KContainerDesc);
            var bg = GetFromReference(UISubPanel_Shop_1102_SBox.KLimitedTimeBoxBg);


            box.GetImage().SetAlpha(1);

            JiYuTweenHelper.PlayUIImageTranstionFX(box, cts.Token, "8C50FF", JiYuTweenHelper.UIDir.Right);
            JiYuTweenHelper.PlayUIImageSweepFX(tenBtn, cts.Token);
            JiYuTweenHelper.PlayUIImageSweepFX(oneBtn, cts.Token);
            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(bg, 490, 800, cts.Token, 0.2f);
            JiYuTweenHelper.SetEaseAlphaAndPosRtoL(tenBtn, 0, 200, cts.Token);
            JiYuTweenHelper.SetEaseAlphaAndPosLtoR(oneBtn, 0, 200, cts.Token);
            if (lessBtn.GameObject.activeSelf)
            {
                JiYuTweenHelper.PlayUIImageSweepFX(lessBtn, cts.Token);
                JiYuTweenHelper.SetEaseAlphaAndPosLtoR(lessBtn, 0, 150, cts.Token);
            }

            await UniTask.Delay(300, cancellationToken: cts.Token);
            JiYuTweenHelper.SetEaseAlphaAndScale(titleDes, 0.25f, true, 3, 1, cts.Token);
        }

        public  void EnableEffectUIs(bool isEnable)
        {

            Log.Debug(effectUIs.Count.ToString(), Color.cyan);
            foreach (var ui in effectUIs)
            {
                ui.SetActive(isEnable);
            }
            //for (int i = 0; i < uis.Count; i++)
            //{
            //    if (!isEnable)
            //    {
            //        uis[i].SetActive(isEnable);
            //    }
            //    else
            //    {
            //        //UniTask.Delay(effectUIs[uis[0]]).Forget();
            //        uis[i].SetActive(isEnable);
            //    }
            //}
        }

        private void InitRedDot(int boxID)
        {
            int tagFuncID = 1102;
            if (boxID / 1000 == 1)
            {
                tagFuncID = 1101;
            }

            var redDotStr = NodeNames.GetTagFuncRedDotName(tagFuncID) + '|';
            boxString = redDotStr + boxID.ToString();
            RedDotManager.Instance.InsterNode(boxString);

            //RedDotManager.Instance.AddListener(itemStr, (num) =>
            //{
            //    Log.Debug($"{itemStr} {num}", Color.cyan);
            //    //redGO.SetActive(num > 0);
            //    KImg_RedDotMid?.SetActive(num > 0);
            //});
            //RedDotManager.Instance.AddListener(itemStr, (num) => {
            //    this.GetFromReference(KImg_OneRedPoint).SetActive(state == RedPointState.Show);
            //    this.GetFromReference(KImg_BtnRedPoint).SetActive(state == RedPointState.Show);
            //});
        }

        private void SetUpdate()
        {
            if (tbdraw_Box.Get(boxID).limitType == 2 || tbdraw_Box.Get(boxID).limitType == 4)
            {
                foreach (var b in ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList)
                {
                    if (b.Id == boxID)
                    {
                        ThisSetStartTime = b.SetStartTime;
                        break;
                    }
                }

                var timerMgr = TimerManager.Instance;
                timerId = timerMgr.StartRepeatedTimer(1000, Update);
            }
        }

        private void Update()
        {
            if (TimeHelper.ClientNowSeconds() > ThisSetStartTime + tbdraw_Box.Get(boxID).dateLimit)
            {
                ParentReSet();
                RemoveTimer();
            }
        }

        private void RemoveTimer()
        {
            if (this.timerId != 0)
            {
                var timerMgr = TimerManager.Instance;
                timerMgr?.RemoveTimerId(ref this.timerId);
                this.timerId = 0;
            }
        }

        /// <summary>
        /// draw界面抽取后改变本界面的保底文本
        /// </summary>
        /// <param name="DrawInt"></param>
        public void DrawSetTxt(List<int> DrawInt)
        {
            var boxList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList;
            int indexHelp = 0;
            for (int i = 0; i < boxList.Count; i++)
            {
                int ihelp = i;
                if (boxList[i].Id == boxID)
                {
                    indexHelp = ihelp;
                }
            }

            if (DrawInt.Count >= 2)
            {
                //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][0] = DrawInt[0];
                //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][1] = DrawInt[1];
                //ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[0] = DrawInt[0];
                //ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[1] = DrawInt[1];
                if (tbdraw_Box.Get(boxID).tagFunc == 1102)
                {
                    ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[0] =
                        DrawInt[0];
                    ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[1] =
                        DrawInt[1];
                }
                else if (tbdraw_Box.Get(boxID).tagFunc == 1101)
                {
                    string str0 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp]
                        .DrawGuarantee[0];
                    ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp]
                        .DrawGuarantee[0] = ReSetDrawGuarantee(str0, DrawInt[0]);
                    string str1 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp]
                        .DrawGuarantee[1];
                    ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp]
                        .DrawGuarantee[1] = ReSetDrawGuarantee(str1, DrawInt[1]);
                }

                TxtInit(boxID);
                BtnTxtInit(boxID);
            }
            else
            {
                Debug.Log("DrawInt's count is wrong");
            }
        }

        /// <summary>
        /// 初始化描述文本
        /// </summary>
        /// <param name="BoxID">宝箱ID</param>
        private void TxtInit(int BoxID)
        {
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

            var boxList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList;
            int indexHelp = 0;
            for (int i = 0; i < boxList.Count; i++)
            {
                //Debug.Log("bi[i] : " + boxList[i]);
                int ihelp = i;
                if (boxList[i].Id == boxID)
                {
                    indexHelp = ihelp;
                }
            }

            long a1 = 100;
            long a2 = 200;
            //读后端
            if (ModuleID == 1102)
            {
                // Debug.Log("this module is 1102");
                a1 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[0];
                a2 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[1];
            }
            else if (ModuleID == 1101)
            {
                //long a1 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[0];
                //long a2 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[1];

                a1 = GetDrawGuarantee(ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID]
                    .BoxInfoList[indexHelp].DrawGuarantee[0]);
                a2 = GetDrawGuarantee(ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID]
                    .BoxInfoList[indexHelp].DrawGuarantee[1]);
            }

            //long a1 = ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][0];
            //long a2 = ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][1];

            string Sstr = language.Get(tbdraw_Box[BoxID].name).current;
            string SstrHelp = UnityHelper.RichTextSize(Sstr[0].ToString(), 85);
            Sstr = Sstr.Substring(1);
            this.GetFromReference(KBoxTypeDescText).GetTextMeshPro().SetTMPText(SstrHelp + Sstr);


            List<int> descQualityList = tbdraw_Box[BoxID].descPara;
            int descNum = descQualityList.Count;
            BoxDescHelpTxt = string.Format(BoxDescHelpTxt, descNum);
            string equipTxt = language.Get(BoxDescHelpTxt).current;
            for (int i = 0; i < descNum; i++)
            {
                equipTxt = equipTxt.Replace("{" + i.ToString() + "}", UnityHelper.RichTextColor(
                    language.Get(tbquality[descQualityList[i]].name).current,
                    tbquality[descQualityList[i]].fontColor));
            }

            string moneyTxt = language.Get("drawbox_money_text").current;
            moneyTxt = moneyTxt.Replace("{0}", tbdraw_Box[BoxID].money.ToString());
            this.GetFromReference(KBoxDescText).GetTextMeshPro().SetTMPText(moneyTxt + "," + equipTxt);


            string timesHelpTxt = language.Get("drawbox_guarantee_text").current;
            string times1Txt = timesHelpTxt.Replace("{0}",
                UnityHelper.RichTextSizeAndColor(a1.ToString(), TimeSSize, "FFFF00"));
            times1Txt = times1Txt.Replace("{1}", UnityHelper.RichTextSizeAndColor(
                language.Get(tbquality[(int)tbdraw_Box[BoxID].guaranteePara[0][0]].name).current,
                TimeSSize, tbquality[(int)tbdraw_Box[BoxID].guaranteePara[0][0]].fontColor));
            string times2Txt = timesHelpTxt.Replace("{0}",
                UnityHelper.RichTextSizeAndColor(a2.ToString(), TimeSSize, "FFFF00"));
            times2Txt = times2Txt.Replace("{1}", UnityHelper.RichTextSizeAndColor(
                language.Get(tbquality[(int)tbdraw_Box[BoxID].guaranteePara[1][0]].name).current,
                TimeSSize, tbquality[(int)tbdraw_Box[BoxID].guaranteePara[1][0]].fontColor));
            this.GetFromReference(KTimesText).GetTextMeshPro().SetTMPText(times1Txt + "," + times2Txt);


            var textRightBottom = this.GetFromReference(KText_RightBottom).GetTextMeshPro();
            var textRightUp = this.GetFromReference(KText_RightUp).GetTextMeshPro();
            GetFromReference(KImg_RightUp).SetActive(false);
            GetFromReference(KImg_RightBottom).SetActive(false);
            var boxinformation = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp];
            string str1 = language.Get("drawbox_type_1_text").current;
            string str2 = language.Get("drawbox_type_2_text").current;
            if (ModuleID == 1101)
            {
                if (tbdraw_Box.Get(boxID).limitType == 2)
                {
                    GetFromReference(KImg_RightBottom).SetActive(true);
                    long startTime = boxinformation.SetStartTime;
                    long endTime = startTime + tbdraw_Box.Get(boxID).dateLimit;
                    long deltaTime = endTime - TimeHelper.ClientNowSeconds();
                    str1 = str1.Replace("{0}",
                        UnityHelper.RichTextColor(JiYuUIHelper.GeneralTimeFormat(new int4(2, 4, 2, 1), deltaTime),
                            "30FC00"));
                    textRightBottom.SetTMPText(str1);
                }

                if (tbdraw_Box.Get(boxID).limitType == 3)
                {
                    GetFromReference(KImg_RightBottom).SetActive(true);
                    str2 = str2.Replace("{0}",
                        UnityHelper.RichTextColor((boxinformation.DrawCount).ToString(), "30FC00"));
                    textRightBottom.SetTMPText(str2);
                }

                if (tbdraw_Box.Get(boxID).limitType == 4)
                {
                    GetFromReference(KImg_RightBottom).SetActive(true);
                    GetFromReference(KImg_RightUp).SetActive(true);
                    long startTime = boxinformation.SetStartTime;
                    long endTime = startTime + tbdraw_Box.Get(boxID).dateLimit;
                    long deltaTime = endTime - TimeHelper.ClientNowSeconds();
                    str1 = str1.Replace("{0}",
                        UnityHelper.RichTextColor(JiYuUIHelper.GeneralTimeFormat(new int4(2, 4, 2, 1), deltaTime),
                            "30FC00"));

                    str2 = str2.Replace("{0}",
                        UnityHelper.RichTextColor((boxinformation.DrawCount).ToString(), "30FC00"));
                    textRightBottom.SetTMPText(str2);
                    textRightUp.SetTMPText(str1);
                }
            }

            var width = textRightBottom.Get().preferredWidth;
            if (GetFromReference(KImg_RightBottom).GameObject.activeSelf)
            {
                GetFromReference(KText_RightBottom).GetRectTransform().SetWidth(width);
                GetFromReference(KImg_RightBottom).GetRectTransform().SetAnchoredPositionX(-width - 40);
            }

            if (GetFromReference(KImg_RightUp).GameObject.activeSelf)
            {
                var width1 = textRightUp.Get().preferredWidth;
                GetFromReference(KText_RightUp).GetRectTransform().SetWidth(width1);
                GetFromReference(KImg_RightUp).GetRectTransform().SetAnchoredPositionX(-width1 - 40);
            }
        }

        private int GetDrawGuarantee(string input)
        {
            int res = 0;

            var strings = input.Split(";");

            if (int.TryParse(strings[1], out var strInt))
            {
                res = strInt;
            }
            else
            {
                Debug.Log("drawGuarantee is wrong");
            }

            return res;
        }

        private string ReSetDrawGuarantee(string inputStr, int inputInt)
        {
            string resultStr = "";
            var strings = inputStr.Split(";");
            resultStr = strings[0] + ";" + inputInt.ToString();
            return resultStr;
        }

        /// <summary>
        /// 初始化抽奖按钮文本
        /// </summary>
        /// <param name="BoxID">宝箱ID</param>
        private async void BtnTxtInit(int BoxID, bool isUpdate = true)
        {
            long ItemNum;
            //读取钥匙数量
            if (ResourcesSingleton.Instance.items.TryGetValue(tbdraw_Box[BoxID].item, out long value))
            {
                ItemNum = value;
            }
            else
            {
                ItemNum = 0;
                //报错
                Debug.Log("There is no Item of Key " + tbdraw_Box[BoxID].item);
            }

            keyNum = ItemNum;

            var strNodeTen = boxString + "|ten";
            RedDotManager.Instance.InsterNode(strNodeTen);
            var strNodeOne = boxString + "|one";
            RedDotManager.Instance.InsterNode(strNodeOne);
            //RedDotManager.Instance.SetRedPointCnt(strNodeTen, 1);

            //根据不同的钥匙数量设置按钮不同的显示状态

            if (ModuleID == 1102)
            {
                this.GetFromReference(KContainerBoxGet).SetActive(true);
                this.GetFromReference(KPos_LessThanTen).SetActive(false);
                if (ItemNum > 0 && ItemNum < 10)
                {
                    var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                    //左边

                    this.GetFromReference(KOneText_Mid).GetTextMeshPro().SetTMPText(str + ItemNum.ToString() + "/1");
                    //右边
                    str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);

                    this.GetFromReference(KTenText_Mid).GetTextMeshPro()
                        .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                    RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                    GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "one", RedPointState.Show, 1);
                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "ten", RedPointState.Hide, 0);
                }
                else if (ItemNum >= 10)
                {
                    var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                    //左边
                    this.GetFromReference(KOneText_Mid).GetTextMeshPro().SetTMPText(str + ItemNum.ToString() + "/1");
                    //右边
                    this.GetFromReference(KTenText_Mid).GetTextMeshPro().SetTMPText(str + ItemNum.ToString() + "/10");


                    RedDotManager.Instance.SetRedPointCnt(strNodeTen, 1);
                    GetFromReference(KImg_TenRedPoint)?.SetActive(true);
                    RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                    GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "one", RedPointState.Show, 1);
                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "ten", RedPointState.Show, 1);
                }
                else
                {
                    RedDotManager.Instance.SetRedPointCnt(strNodeOne, 0);
                    RedDotManager.Instance.SetRedPointCnt(strNodeTen, 0);
                    GetFromReference(KImg_OneRedPoint)?.SetActive(false);
                    GetFromReference(KImg_TenRedPoint)?.SetActive(false);
                    //没有钥匙
                    var str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);
                    //左边

                    this.GetFromReference(KOneText_Mid).GetTextMeshPro()
                        .SetTMPText(str + tbdraw_Box[BoxID].one.ToString());
                    //右边

                    this.GetFromReference(KTenText_Mid).GetTextMeshPro()
                        .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "one", RedPointState.Hide, 0);
                    //RedPointMgr.instance.SetState("ShopRoot",
                    //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "ten", RedPointState.Hide, 0);
                }
            }
            else
            {
                GetFromReference(KImg_OneRedPoint)?.SetActive(false);
                GetFromReference(KImg_TenRedPoint)?.SetActive(false);
                //限次盲盒
                if (tbdraw_Box.Get(BoxID).limitType == 3 || tbdraw_Box.Get(BoxID).limitType == 4)
                {
                    int realdrawcount = 0;
                    foreach (var bi in ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList)
                    {
                        if (bi.Id == BoxID)
                        {
                            realdrawcount = bi.DrawCount;
                            break;
                        }
                    }

                    //没有抽奖次数就不显示
                    if (realdrawcount <= 0)
                    {
                        ParentReSet();
                    }

                    //抽奖次数小于10
                    if (realdrawcount < 10)
                    {
                        this.GetFromReference(KPos_LessThanTen).SetActive(true);
                        this.GetFromReference(KContainerBoxGet).SetActive(false);

                        //Log.Debug($"888888888888888888,,{ItemNum}", Color.cyan);
                        //钥匙数量大于0
                        if (ItemNum > 0)
                        {
                            this.GetFromReference(KImage_Btn_Left).GetImage()
                                .SetSprite(tbitem[tbdraw_Box[BoxID].item].icon, false);

                            RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                            GetFromReference(KImg_OneRedPoint)?.SetActive(true);

                            this.GetFromReference(KText_Mid).GetTextMeshPro().SetTMPText(ItemNum.ToString() + "/1");
                            //Log.Debug("dffffffffsdfffffffffffffff", Color.cyan);
                        }
                        //没有钥匙
                        else
                        {
                            //RedPointMgr.instance.SetState("ShopRoot",
                            //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "one",
                            //    RedPointState.Hide, 0);

                            RedDotManager.Instance.SetRedPointCnt(strNodeOne, 0);
                            await this.GetFromReference(KImage_Btn_Left).GetImage()
                                .SetSpriteAsync(tbuser_Variable[2].icon, false);
                            this.GetFromReference(KText_Mid).GetTextMeshPro()
                                .SetTMPText(tbdraw_Box[BoxID].one.ToString());
                        }
                    }
                    //抽奖次数大于10
                    else
                    {
                        this.GetFromReference(KPos_LessThanTen).SetActive(false);
                        this.GetFromReference(KContainerBoxGet).SetActive(true);

                        //钥匙数量小于10
                        if (ItemNum > 0 && ItemNum < 10)
                        {
                            var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                            //左边

                            this.GetFromReference(KOneText_Mid).GetTextMeshPro()
                                .SetTMPText(str + ItemNum.ToString() + "/1");
                            //右边
                            str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);

                            this.GetFromReference(KTenText_Mid).GetTextMeshPro()
                                .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());


                            RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                            GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                        }
                        //钥匙数量大于10
                        else if (ItemNum >= 10)
                        {
                            var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                            //左边
                            this.GetFromReference(KOneText_Mid).GetTextMeshPro()
                                .SetTMPText(str + ItemNum.ToString() + "/1");
                            //右边
                            this.GetFromReference(KTenText_Mid).GetTextMeshPro()
                                .SetTMPText(str + ItemNum.ToString() + "/10");

                            RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                            GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                            RedDotManager.Instance.SetRedPointCnt(strNodeTen, 1);
                            GetFromReference(KImg_TenRedPoint)?.SetActive(true);
                        }
                        else
                        {
                            var str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);
                            //左边

                            this.GetFromReference(KOneText_Mid).GetTextMeshPro()
                                .SetTMPText(str + tbdraw_Box[BoxID].one.ToString());
                            //右边

                            this.GetFromReference(KTenText_Mid).GetTextMeshPro()
                                .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                            //RedPointMgr.instance.SetState("ShopRoot",
                            //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "one",
                            //    RedPointState.Hide, 0);
                            //RedPointMgr.instance.SetState("ShopRoot",
                            //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "ten",
                            //    RedPointState.Hide, 0);
                        }
                    }
                }
                //限时盲盒
                else
                {
                    GetFromReference(KImg_OneRedPoint)?.SetActive(false);
                    GetFromReference(KImg_TenRedPoint)?.SetActive(false);


                    this.GetFromReference(KPos_LessThanTen).SetActive(false);
                    //钥匙不够十个
                    if (ItemNum > 0 && ItemNum < 10)
                    {
                        var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                        //左边

                        this.GetFromReference(KOneText_Mid).GetTextMeshPro()
                            .SetTMPText(str + ItemNum.ToString() + "/1");
                        //右边
                        str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);

                        this.GetFromReference(KTenText_Mid).GetTextMeshPro()
                            .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                        RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                        GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                    }
                    //钥匙大于10个
                    else if (ItemNum >= 10)
                    {
                        var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                        //左边
                        this.GetFromReference(KOneText_Mid).GetTextMeshPro()
                            .SetTMPText(str + ItemNum.ToString() + "/1");
                        //右边
                        this.GetFromReference(KTenText_Mid).GetTextMeshPro()
                            .SetTMPText(str + ItemNum.ToString() + "/10");

                        RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                        GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                        RedDotManager.Instance.SetRedPointCnt(strNodeTen, 1);
                        GetFromReference(KImg_TenRedPoint)?.SetActive(true);
                    }
                    else
                    {
                        var str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);
                        //左边

                        this.GetFromReference(KOneText_Mid).GetTextMeshPro()
                            .SetTMPText(str + tbdraw_Box[BoxID].one.ToString());
                        //右边

                        this.GetFromReference(KTenText_Mid).GetTextMeshPro()
                            .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                        //RedPointMgr.instance.SetState("ShopRoot",
                        //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "one", RedPointState.Hide,
                        //    0);
                        //RedPointMgr.instance.SetState("ShopRoot",
                        //    "module" + ModuleID.ToString() + boxID.ToString().ToString() + "ten", RedPointState.Hide,
                        //    0);
                    }
                }
            }
        }

        /// <summary>
        /// 一次的抽奖
        /// </summary>
        /// <param name="leftBtn"></param>
        private void BtnOnClickLeft(UI leftBtn)
        {
            JiYuUIHelper.DestoryAllTips();
            ///
            if (ModuleID == 1102)
            {
                drawNum = 1;
                WebMessageHandler.Instance.AddHandler(11, 2, OnClickBoxBtnResponse);
                NetWorkManager.Instance.SendMessage(11, 2, drawParamLeft);
                Debug.Log("left: " + drawParamLeft.DrawCount.ToString());
            }
            else if (ModuleID == 1101)
            {
                drawNum = 1;
                WebMessageHandler.Instance.AddHandler(11, 14, OnClick1101BoxBtnResponse);
                NetWorkManager.Instance.SendMessage(11, 14, drawParamLeft);
                Debug.Log("left: " + drawParamLeft.DrawCount.ToString());
            }


            //资源扣除判断
            //用钥匙购买
            if (keyNum > 0)
            {
                ResourcesSingleton.Instance.items[tbdraw_Box[boxID].item] -= 1;
            }
            else
            {
                var needCostInt = tbdraw_Box[boxID].one;
                var playerHave = ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin;
                if (needCostInt > playerHave)
                {
                    Vector3 v3 = new Vector3((int)JiYuUIHelper.Vector3Type.BITCOIN, 0, 0);
                    v3.z = needCostInt - playerHave;
                    var uiTip = UIHelper.Create<Vector3>(UIType.UICommon_ResourceNotEnough, v3);
                    JiYuUIHelper.SetResourceNotEnoughTip(uiTip, leftBtn);
                }
                else
                {
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin -= tbdraw_Box[boxID].one;
                }
            }

            TxtInit(boxID);

            if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui)) return;

            var uis = ui as UIPanel_Shop;
            var ui1102Sboxs = uis.GetAll1102SBox();
            Log.Debug($"dfadf dfd f:{ui1102Sboxs}");
            BtnRefresh(ui1102Sboxs);
            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        private void BtnRefresh(List<UI> ui1102Sboxs)
        {
            var strNodeTen = boxString + "|ten";
            RedDotManager.Instance.InsterNode(strNodeTen);
            var strNodeOne = boxString + "|one";
            RedDotManager.Instance.InsterNode(strNodeOne);

            for (int i = 0; i < ui1102Sboxs.Count; i++)
            {
                var ui1102 = ui1102Sboxs[i] as UISubPanel_Shop_1102_SBox;
                long ItemNum;
                int BoxID = ui1102.boxID;
                //读取钥匙数量
                if (ResourcesSingleton.Instance.items.TryGetValue(tbdraw_Box[BoxID].item, out long value))
                {
                    ItemNum = value;
                }
                else
                {
                    ItemNum = 0;
                    //报错
                    Debug.Log("There is no Item of Key " + tbdraw_Box[BoxID].item);
                }

                keyNum = ItemNum;


                //Log.Debug($"HHHHHHHHHHH：{ui1102.ModuleID}");
                if (ui1102.ModuleID == 1102)
                {
                    ui1102.GetFromReference(KContainerBoxGet).SetActive(true);
                    ui1102.GetFromReference(KPos_LessThanTen).SetActive(false);
                    if (ItemNum > 0 && ItemNum < 10)
                    {
                        var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                        //左边

                        ui1102.GetFromReference(KOneText_Mid).GetTextMeshPro()
                            .SetTMPText(str + ItemNum.ToString() + "/1");
                        //右边
                        str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);

                        ui1102.GetFromReference(KTenText_Mid).GetTextMeshPro()
                            .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                        RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                        ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                        //RedPointMgr.instance.SetState("ShopRoot",
                        //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "one", RedPointState.Show, 1);
                        //RedPointMgr.instance.SetState("ShopRoot",
                        //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "ten", RedPointState.Hide, 0);
                    }
                    else if (ItemNum >= 10)
                    {
                        var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                        //左边
                        ui1102.GetFromReference(KOneText_Mid).GetTextMeshPro()
                            .SetTMPText(str + ItemNum.ToString() + "/1");
                        //右边
                        ui1102.GetFromReference(KTenText_Mid).GetTextMeshPro()
                            .SetTMPText(str + ItemNum.ToString() + "/10");


                        RedDotManager.Instance.SetRedPointCnt(strNodeTen, 1);
                        ui1102.GetFromReference(KImg_TenRedPoint)?.SetActive(true);
                        RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                        ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                        //RedPointMgr.instance.SetState("ShopRoot",
                        //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "one", RedPointState.Show, 1);
                        //RedPointMgr.instance.SetState("ShopRoot",
                        //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "ten", RedPointState.Show, 1);
                    }
                    else
                    {
                        RedDotManager.Instance.SetRedPointCnt(strNodeOne, 0);
                        RedDotManager.Instance.SetRedPointCnt(strNodeTen, 0);
                        ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(false);
                        ui1102.GetFromReference(KImg_TenRedPoint)?.SetActive(false);
                        //没有钥匙
                        var str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);
                        //左边

                        ui1102.GetFromReference(KOneText_Mid).GetTextMeshPro()
                            .SetTMPText(str + tbdraw_Box[BoxID].one.ToString());
                        //右边

                        ui1102.GetFromReference(KTenText_Mid).GetTextMeshPro()
                            .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                        //RedPointMgr.instance.SetState("ShopRoot",
                        //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "one", RedPointState.Hide, 0);
                        //RedPointMgr.instance.SetState("ShopRoot",
                        //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "ten", RedPointState.Hide, 0);
                    }
                }
                else
                {
                    ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(false);
                    ui1102.GetFromReference(KImg_TenRedPoint)?.SetActive(false);
                    //限次盲盒
                    if (tbdraw_Box.Get(BoxID).limitType == 3 || tbdraw_Box.Get(BoxID).limitType == 4)
                    {
                        int realdrawcount = 0;
                        foreach (var bi in ResourcesSingleton.Instance.shopMap.IndexModuleMap[ui1102.ModuleID]
                                     .BoxInfoList)
                        {
                            if (bi.Id == BoxID)
                            {
                                realdrawcount = bi.DrawCount;
                                break;
                            }
                        }

                        //没有抽奖次数就不显示
                        if (realdrawcount <= 0)
                        {
                            ParentReSet();
                        }

                        //抽奖次数小于10
                        if (realdrawcount < 10)
                        {
                            ui1102.GetFromReference(KPos_LessThanTen).SetActive(true);
                            ui1102.GetFromReference(KContainerBoxGet).SetActive(false);

                            //Log.Debug($"888888888888888888,,{ItemNum}", Color.cyan);
                            //钥匙数量大于0
                            if (ItemNum > 0)
                            {
                                ui1102.GetFromReference(KImage_Btn_Left).GetImage()
                                    .SetSprite(tbitem[tbdraw_Box[BoxID].item].icon, false);

                                RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                                ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(true);

                                ui1102.GetFromReference(KText_Mid).GetTextMeshPro()
                                    .SetTMPText(ItemNum.ToString() + "/1");
                                //Log.Debug("dffffffffsdfffffffffffffff", Color.cyan);
                            }
                            //没有钥匙
                            else
                            {
                                //RedPointMgr.instance.SetState("ShopRoot",
                                //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "one",
                                //    RedPointState.Hide, 0);

                                RedDotManager.Instance.SetRedPointCnt(strNodeOne, 0);
                                ui1102.GetFromReference(KImage_Btn_Left).GetImage()
                                    .SetSprite(tbuser_Variable[2].icon, false);
                                ui1102.GetFromReference(KText_Mid).GetTextMeshPro()
                                    .SetTMPText(tbdraw_Box[BoxID].one.ToString());
                            }
                        }
                        //抽奖次数大于10
                        else
                        {
                            ui1102.GetFromReference(KPos_LessThanTen).SetActive(false);
                            ui1102.GetFromReference(KContainerBoxGet).SetActive(true);

                            //钥匙数量小于10
                            if (ItemNum > 0 && ItemNum < 10)
                            {
                                var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                                //左边

                                ui1102.GetFromReference(KOneText_Mid).GetTextMeshPro()
                                    .SetTMPText(str + ItemNum.ToString() + "/1");
                                //右边
                                str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);

                                ui1102.GetFromReference(KTenText_Mid).GetTextMeshPro()
                                    .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());


                                RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                                ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                            }
                            //钥匙数量大于10
                            else if (ItemNum >= 10)
                            {
                                var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                                //左边
                                ui1102.GetFromReference(KOneText_Mid).GetTextMeshPro()
                                    .SetTMPText(str + ItemNum.ToString() + "/1");
                                //右边
                                ui1102.GetFromReference(KTenText_Mid).GetTextMeshPro()
                                    .SetTMPText(str + ItemNum.ToString() + "/10");

                                RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                                ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                                RedDotManager.Instance.SetRedPointCnt(strNodeTen, 1);
                                ui1102.GetFromReference(KImg_TenRedPoint)?.SetActive(true);
                            }
                            else
                            {
                                var str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);
                                //左边

                                ui1102.GetFromReference(KOneText_Mid).GetTextMeshPro()
                                    .SetTMPText(str + tbdraw_Box[BoxID].one.ToString());
                                //右边

                                ui1102.GetFromReference(KTenText_Mid).GetTextMeshPro()
                                    .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                                //RedPointMgr.instance.SetState("ShopRoot",
                                //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "one",
                                //    RedPointState.Hide, 0);
                                //RedPointMgr.instance.SetState("ShopRoot",
                                //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "ten",
                                //    RedPointState.Hide, 0);
                            }
                        }
                    }
                    //限时盲盒
                    else
                    {
                        ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(false);
                        ui1102.GetFromReference(KImg_TenRedPoint)?.SetActive(false);


                        ui1102.GetFromReference(KPos_LessThanTen).SetActive(false);
                        //钥匙不够十个
                        if (ItemNum > 0 && ItemNum < 10)
                        {
                            var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                            //左边

                            ui1102.GetFromReference(KOneText_Mid).GetTextMeshPro()
                                .SetTMPText(str + ItemNum.ToString() + "/1");
                            //右边
                            str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);

                            ui1102.GetFromReference(KTenText_Mid).GetTextMeshPro()
                                .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                            RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                            ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                        }
                        //钥匙大于10个
                        else if (ItemNum >= 10)
                        {
                            var str = JiYuUIHelper.GetRewardTextIconName(tbitem[tbdraw_Box[BoxID].item].icon);
                            //左边
                            ui1102.GetFromReference(KOneText_Mid).GetTextMeshPro()
                                .SetTMPText(str + ItemNum.ToString() + "/1");
                            //右边
                            ui1102.GetFromReference(KTenText_Mid).GetTextMeshPro()
                                .SetTMPText(str + ItemNum.ToString() + "/10");

                            RedDotManager.Instance.SetRedPointCnt(strNodeOne, 1);
                            ui1102.GetFromReference(KImg_OneRedPoint)?.SetActive(true);
                            RedDotManager.Instance.SetRedPointCnt(strNodeTen, 1);
                            ui1102.GetFromReference(KImg_TenRedPoint)?.SetActive(true);
                        }
                        else
                        {
                            var str = JiYuUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon);
                            //左边

                            ui1102.GetFromReference(KOneText_Mid).GetTextMeshPro()
                                .SetTMPText(str + tbdraw_Box[BoxID].one.ToString());
                            //右边

                            ui1102.GetFromReference(KTenText_Mid).GetTextMeshPro()
                                .SetTMPText(str + tbdraw_Box[BoxID].ten.ToString());

                            //RedPointMgr.instance.SetState("ShopRoot",
                            //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "one", RedPointState.Hide,
                            //    0);
                            //RedPointMgr.instance.SetState("ShopRoot",
                            //    "module" + ui1102.ModuleID.ToString() + boxID.ToString().ToString() + "ten", RedPointState.Hide,
                            //    0);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 十连抽
        /// </summary>
        /// <param name="rightBtn"></param>
        private void BtnOnClickRight(UI rightBtn)
        {
            JiYuUIHelper.DestoryAllTips();


            if (ModuleID == 1102)
            {
                drawNum = 10;
                WebMessageHandler.Instance.AddHandler(11, 2, OnClickBoxBtnResponse);
                NetWorkManager.Instance.SendMessage(11, 2, drawParamRight);
                Debug.Log("Right: " + drawParamRight.DrawCount.ToString());
            }
            else if (ModuleID == 1101)
            {
                drawNum = 10;
                WebMessageHandler.Instance.AddHandler(11, 14, OnClick1101BoxBtnResponse);
                NetWorkManager.Instance.SendMessage(11, 14, drawParamRight);
                Debug.Log("Right: " + drawParamRight.DrawCount.ToString());
            }


            //资源扣除判断
            //用钥匙购买
            if (keyNum > 0)
            {
                ResourcesSingleton.Instance.items[tbdraw_Box[boxID].item] -= 10;
            }
            else
            {
                var needCostInt = tbdraw_Box[boxID].ten;
                var playerHave = ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin;
                if (needCostInt > playerHave)
                {
                    Vector3 v3 = new Vector3((int)JiYuUIHelper.Vector3Type.BITCOIN, 0, 0);
                    v3.z = needCostInt - playerHave;
                    var uiTip = UIHelper.Create<Vector3>(UIType.UICommon_ResourceNotEnough, v3);
                    JiYuUIHelper.SetResourceNotEnoughTip(uiTip, rightBtn);
                }
                else
                {
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin -= tbdraw_Box[boxID].ten;
                }
            }

            TxtInit(boxID);
            if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui)) return;

            var uis = ui as UIPanel_Shop;
            var ui1102Sboxs = uis.GetAll1102SBox();
            Log.Debug($"dfadf dfd f:{ui1102Sboxs}");
            BtnRefresh(ui1102Sboxs);
            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        private void BtnOnClickMid(UI midBtn)
        {
            JiYuUIHelper.DestoryAllTips();

            drawNum = 1;
            WebMessageHandler.Instance.AddHandler(11, 14, OnClick1101BoxBtnResponse);
            NetWorkManager.Instance.SendMessage(11, 14, drawParamLeft);


            //资源扣除判断
            //用钥匙购买
            if (keyNum > 0)
            {
                ResourcesSingleton.Instance.items[tbdraw_Box[boxID].item] -= 1;
            }
            else
            {
                var needCostInt = tbdraw_Box[boxID].one;
                var playerHave = ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin;
                if (needCostInt > playerHave)
                {
                    Vector3 v3 = new Vector3((int)JiYuUIHelper.Vector3Type.BITCOIN, 0, 0);
                    v3.z = needCostInt - playerHave;
                    var uiTip = UIHelper.Create<Vector3>(UIType.UICommon_ResourceNotEnough, v3);
                    JiYuUIHelper.SetResourceNotEnoughTip(uiTip, midBtn);
                }
                else
                {
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin -= tbdraw_Box[boxID].one;
                }
            }

            TxtInit(boxID);
            if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui)) return;

            var uis = ui as UIPanel_Shop;
            var ui1102Sboxs = uis.GetAll1102SBox();
            Log.Debug($"dfadf dfd f:{ui1102Sboxs}");
            BtnRefresh(ui1102Sboxs);
            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        private void BtnOnClickPre()
        {
            var ui = UIHelper.Create<int>(UIType.UISubPanel_Shop_Pre, boxID);
        }

        private void OnClickBoxBtnResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(11, 2, OnClickBoxBtnResponse);
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

            //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][0] = drawInfo.Guarantee[0];
            //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][1] = drawInfo.Guarantee[1];

            //TxtInit(boxID);
            //BtnTxtInit(boxID);


            var reStrs = drawInfo.Reward;
            List<Vector3> RewardList = new List<Vector3>
            {
                new Vector3(boxID, boxID, boxID)
            };
            foreach (var re in reStrs)
            {
                RewardList.Add(UnityHelper.StrToVector3(re));
            }

            var ui = UIHelper.Create<ShopDrawHelp>(UIType.UISubPanel_Shop_Draw, new ShopDrawHelp
            {
                rewardList = RewardList,
                isAdvert = false
            });
            ui.SetParent(this, false);

            var boxList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList;
            int indexHelp = 0;
            for (int i = 0; i < boxList.Count; i++)
            {
                int ihelp = i;
                if (boxList[i].Id == boxID)
                {
                    indexHelp = ihelp;
                }
            }

            if (tbdraw_Box.Get(boxID).tagFunc == 1102)
            {
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[0] =
                    drawInfo.Guarantee[0];
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[1] =
                    drawInfo.Guarantee[1];
            }
            else if (tbdraw_Box.Get(boxID).tagFunc == 1101)
            {
                string str0 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp]
                    .DrawGuarantee[0];
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].DrawGuarantee[0] =
                    ReSetDrawGuarantee(str0, drawInfo.Guarantee[0]);
                string str1 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp]
                    .DrawGuarantee[1];
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].DrawGuarantee[1] =
                    ReSetDrawGuarantee(str1, drawInfo.Guarantee[1]);
            }

            //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][0] = drawInfo.Guarantee[0];
            //ResourcesSingleton.Instance.shopInit.shopHelpDic[boxInt[0]][boxInt[1]][1] = drawInfo.Guarantee[1];

            TxtInit(boxID);
            BtnTxtInit(boxID);
            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        private void OnClick1101BoxBtnResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(11, 14, OnClick1101BoxBtnResponse);

            DrawInfo drawInfo = new DrawInfo();
            drawInfo.MergeFrom(e.data);
            Debug.Log(drawInfo);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

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
                isAdvert = false
            });
            ui.SetParent(this, false);


            var boxList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList;
            int indexHelp = 0;
            for (int i = 0; i < boxList.Count; i++)
            {
                int ihelp = i;
                if (boxList[i].Id == boxID)
                {
                    indexHelp = ihelp;
                }
            }

            if (tbdraw_Box.Get(boxID).tagFunc == 1102)
            {
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[0] =
                    drawInfo.Guarantee[0];
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].Numbers[1] =
                    drawInfo.Guarantee[1];
            }
            else if (tbdraw_Box.Get(boxID).tagFunc == 1101)
            {
                string str0 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp]
                    .DrawGuarantee[0];
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].DrawGuarantee[0] =
                    ReSetDrawGuarantee(str0, drawInfo.Guarantee[0]);
                string str1 = ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp]
                    .DrawGuarantee[1];
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].DrawGuarantee[1] =
                    ReSetDrawGuarantee(str1, drawInfo.Guarantee[1]);
            }

            if (tbdraw_Box.Get(boxID).limitType == 3 || tbdraw_Box.Get(boxID).limitType == 4)
            {
                ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].DrawCount -=
                    RewardList.Count - 1;
                if (ResourcesSingleton.Instance.shopMap.IndexModuleMap[ModuleID].BoxInfoList[indexHelp].DrawCount <= 0)
                {
                    ParentReSet();
                }
            }


            TxtInit(boxID);
            BtnTxtInit(boxID);
            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        public void ParentReSet()
        {
            var parent = this.GetParent<UIPanel_Shop>();
            parent.UpDateNowInterface();
        }

        private void BtnOnClickInit()
        {
            var leftBtn = this.GetFromReference(KOneTimeBtn);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(leftBtn, () => BtnOnClickLeft(leftBtn));
            var RightBtn = this.GetFromReference(KTenTimesBtn);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(RightBtn, () => BtnOnClickRight(RightBtn));
            var PreviewBtn = this.GetFromReference(KPreviewBtn);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(PreviewBtn, BtnOnClickPre);
            var midBtn = this.GetFromReference(KBtn_LessThanTen);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(midBtn, () => BtnOnClickMid(midBtn));
        }

        protected override void OnClose()
        {
            Log.Debug($"cts000 1102SBox ");
            //effectUIs.Clear();
            RemoveTimer();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}