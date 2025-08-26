//---------------------------------------------------------------------
// UnicornStudio
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
using UnityEngine.UI;
using UnityEngine.Windows;


namespace XFramework
{
    [UIEvent(UIType.UIPanel_Shop)]
    internal sealed class UIPanel_ShopEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Shop;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Shop>();
        }
    }


    public partial class UIPanel_Shop : UI, IAwake
    {
        #region property

        //------------------------------------------------------------------------------
        private Tbrecharge tbrecharge;
        private Tbdraw_box tbdraw_Box;
        private Tbconstant tbconstant;
        private Tbshop_daily tbshop_Daily;
        private Tblanguage tblanguage;
        private Tbtag_func tbtag_Func;
        private Tbfund tbfund;
        private Tbfund_reward tbfund_Reward;
        private Tbmonthly tbmonthly;
        private Tbprice tbprice;
        private Tbuser_variable tbuser_Variable;
        private Tbgift tbgift;
        private Tbgift_group tbgift_Group;
        private Tbchapter tbchapter;
        private Tbquality tbquality;
        private Tbspecials tbspecials;

        public int MaxPosType = 0;
        public int NowPosType1 = 1;
        public int NowPosType2 = 0;

        private long timerId = 0;

        private float ContentH = 0;
        private float ModuleDeltaY = 50;
        private float BaseTopOffset = 240;
        private float SecTabHeight = 100;

        private UI LastSecTabBtn = null;
        private UI LastFundPoint = null;


        private List<int> NowModuleIdList = new List<int>();

        /// <summary>
        /// 旧版cts 弃用
        /// </summary>
        private CancellationTokenSource ctsOld = new CancellationTokenSource();

        /// <summary>
        /// 动效cts
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();

        private Dictionary<UI, int> ModuleUIAndIDDic = new Dictionary<UI, int>();

        private float LastScrollValue = 1;
        //------------------------------------------------------------------------------


        private Tblanguage language;
        private Tbtag_func tag_Func;


        private List<string> TabNameList = new List<string>();
        private List<string> TabIconList = new List<string>();

        public UICommon_TopTab TopTabUi;


        //public string ShopRoot = "ShopRoot";
        private List<long> drawToNowList = new List<long>();
        private List<UI> effectUIs = new List<UI>();
        public XScrollRectComponent scrollRect;
        private float curWidth;
        private float defaultWidth = -Screen.width * 0f;

        public int tagId = 1;

        public List<int> tagsTimes = new List<int>() { 2, 1, 1, 1};

        #endregion

        public void Initialize()
        {
            DataInit();
            //helpppppppppp();


            //构建红点树
            BuildModleRedDot();
            FirstLevelTab();

            SingleTabOnClick(1);
            TimeUpdateInit();
            InitCloseTip();

            //SelectShopStateByTagPosType(3);
            //SingleTabOnClick(2);

            scrollRect = GetFromReference(UIPanel_Shop.KScrollView).GetXScrollRect();
            scrollRect.OnDrag.Add(OnScrollRectDragging);
            scrollRect.OnBeginDrag.Add(CloseAllTips);
            scrollRect.OnEndDrag.Add(() =>
            {
                curWidth = 0;
                scrollRect.Get().vertical = true;
                if (UnicornUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                {
                    var uis = ui as UIPanel_JiyuGame;
                    uis.OnEndDrag();
                }
            });


            InitResource();

            //GetAllMoudelUIs();
        }

        public async void GetAllMoudelUIs(bool enable)
        {
            //effectUIs.Clear();
         

            var ui1103s=GetAll1103SBox();
            for (int i = 0; i < ui1103s.Count; i++)
            {
                var ui = ui1103s[i] as UISubPanel_Shop_1103_Box;
                ui.EnableEffectUIs(enable);
                //effectUIs.AddRange(ui.GetEffectUIs());
            }

            if (enable)
            {
                await UniTask.Delay(550);
            }

            var ui1102s = GetAll1102SBox();
            for (int i = 0; i < ui1102s.Count; i++)
            {
                Log.Debug($"1102:{ui1102s.Count}", Color.cyan);
                var ui = ui1102s[i] as UISubPanel_Shop_1102_SBox;
                ui.EnableEffectUIs(enable);
                //effectUIs.AddRange(ui.GetEffectUIs());
            }

        }

        public List<UI> GetEffectUIs()
        {
            return effectUIs;
        }

        public void InitResource()
        {
            var KText_Diamond = GetFromReference(UIPanel_Shop.KText_Diamond);
            var KText_Money = GetFromReference(UIPanel_Shop.KText_Money);


            var icon = UnicornUIHelper.GetRewardTextIconName("icon_diamond_add");
            KText_Diamond.GetTextMeshPro().SetTMPText(icon + UnicornUIHelper.ReturnFormatResourceNum(1));

            icon = UnicornUIHelper.GetRewardTextIconName("icon_money_add");
            KText_Money.GetTextMeshPro().SetTMPText(icon + UnicornUIHelper.ReturnFormatResourceNum(2));
        }

        public void SelectShopStateByTagPosType(int inputPosType, int secTabInitPosType = 1)
        {
            // postype to index
            int i = 0;
            Dictionary<int, int> posTypeToIndexDic = new Dictionary<int, int>();
            foreach (var tbtf in tbtag_Func.DataList)
            {
                if (tbtf.tagId == 1)
                {
                    if (posTypeToIndexDic.ContainsKey(tbtf.posType))
                    {
                    }
                    else
                    {
                        int ihelp = i;
                        posTypeToIndexDic.Add(tbtf.posType, ihelp);
                        i++;
                    }
                }
            }

            TopTabUi.BtnOnClick(posTypeToIndexDic[inputPosType]);
            SingleTabOnClick(inputPosType, secTabInitPosType);
            //Log.Debug("wqewewqe", Color.cyan);
            UnicornTweenHelper.SetEaseAlphaAndPosUtoB(TopTabUi.GetFromReference(UICommon_TopTab.KScrollView), 0, 50f);
        }

        public void RotateShop()
        {
            Dictionary<int, int> helpDic = new Dictionary<int, int>();
            foreach (var tbtf in tbtag_Func.DataList)
            {
                if (tbtf.tagId == 1)
                {
                    if (helpDic.ContainsKey(tbtf.posType))
                    {
                    }
                    else
                    {
                        helpDic.Add(tbtf.posType, 1);
                    }
                }
            }

            List<int> posTypeList = new List<int>();
            foreach (var hd in helpDic)
            {
                posTypeList.Add(hd.Key);
            }

            posTypeList.Sort();

            for (int i = 0; i < posTypeList.Count; i++)
            {
                int ihelp = i;
                helpDic[posTypeList[ihelp]] = ihelp;
            }

            //SelectShopStateByTagPosType(NowPosType1,NowPosType2);
            if (NowPosType1 >= posTypeList.Count)
            {
                SelectShopStateByTagPosType(posTypeList[0]);
            }
            else
            {
                SelectShopStateByTagPosType(posTypeList[helpDic[NowPosType1] + 1]);
            }
        }

        private void OnShopTestResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 10, OnShopTestResponse);
            var shopMap = new GameShopMap();
            shopMap.MergeFrom(e.data);
            Debug.Log(shopMap);

            if (e.data.IsEmpty)
            {
                Debug.Log("e is empty");
                return;
            }

            ResourcesSingletonOld.Instance.shopMap = shopMap;

            //helpppppppppp().Forget();

            //DataInit();
            ////BoxTimeTest();
            //SetTabRedPoint();
            //FirstLevelTab();
            //SingleTabOnClick(1);
            //TimeUpdateInit();
        }

        private void helpppppppppp()
        {
            var contentList = this.GetFromReference(KContent).GetList();
            for (int i = 0; i < 30; i++)
            {
                contentList.CreateWithUIType(UIType.UISubPanel_Shop_Fund_Item, false);
            }

            for (int i = 0; i < 50; i++)
            {
                contentList.CreateWithUIType<Vector3>(UIType.UICommon_RewardItem, new Vector3(2, 0, 1), false);
            }

            for (int i = 0; i < 15; i++)
            {
                contentList.CreateWithUIType(UIType.UISubPanel_Shop_Pre_Row, false);
            }

            contentList.Clear();
        }

        private void BoxTimeTest()
        {
            //var boxList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].BoxInfoList;
            //List<int> ints = new List<int>();
            //for (int i = 0; i < boxList.Count; i++)
            //{
            //    int ihelp = i;
            //    if (tbdraw_Box.Get(boxList[i].Id).limitType == 2 || tbdraw_Box.Get(boxList[i].Id).limitType == 4)
            //    {
            //        ints.Add(ihelp);
            //    }
            //}

            //foreach (var i in ints)
            //{
            //    ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].BoxInfoList[i].SetStartTime = TimeHelper.ClientNowSeconds() - tbdraw_Box.Get(ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].BoxInfoList[i].Id).dateLimit + 90;
            //}
        }

        #region New Shop

        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------

        private void InitCloseTip()
        {
            this.GetFromReference(KTipCloseButton).GetXButton().OnClick.Add(() => { CloseAllTips(); });
            this.GetFromReference(KScrollView).GetXButton().OnClick.Add(() => { CloseAllTips(); });
            // this.GetFromReference(KScrollView).GetXScrollRect().OnValueChanged.Add((f) =>
            // {
            //     float scrollHeight = this.GetFromReference(KContent).GetRectTransform().Height();
            //     // if (math.abs(f.y - LastScrollValue) * scrollHeight >= 1)
            //     // {
            //     //     //Log.Debug($"InitCloseTip111");
            //     //     CloseAllTips();
            //     // }
            //
            //     LastScrollValue = f.y;
            // });
            TopTabUi.GetFromReference(UICommon_TopTab.KScrollView).GetXButton().OnClick
                .Add(() =>
                {
                    Log.Debug($"InitCloseTip222");
                    CloseAllTips();
                });
            // TopTabUi.GetFromReference(UICommon_TopTab.KScrollView).GetScrollRect().OnValueChanged.Add((f) =>
            // {
            //     // Log.Debug($"InitCloseTip333");
            //     // CloseAllTips();
            // });
        }

        private void CloseAllTips()
        {
            Log.Debug($"CloseAllTips");
            UnicornUIHelper.DestoryAllTips();
            CloseNotEnough();
        }


        private void CloseNotEnough()
        {
            UIHelper.Remove(UIType.UICommon_ResourceNotEnough);
        }

        private void TimeUpdateInit()
        {
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(1000, Update);
        }

        private void DataInit()
        {
            tbtag_Func = ConfigManager.Instance.Tables.Tbtag_func;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbdraw_Box = ConfigManager.Instance.Tables.Tbdraw_box;
            tbrecharge = ConfigManager.Instance.Tables.Tbrecharge;
            tbshop_Daily = ConfigManager.Instance.Tables.Tbshop_daily;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
            tbfund = ConfigManager.Instance.Tables.Tbfund;
            tbfund_Reward = ConfigManager.Instance.Tables.Tbfund_reward;
            tbmonthly = ConfigManager.Instance.Tables.Tbmonthly;
            tbprice = ConfigManager.Instance.Tables.Tbprice;
            tbuser_Variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbgift = ConfigManager.Instance.Tables.Tbgift;
            tbgift_Group = ConfigManager.Instance.Tables.Tbgift_group;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            tbquality = ConfigManager.Instance.Tables.Tbquality;
            tbspecials = ConfigManager.Instance.Tables.Tbspecials;
        }

        private int GetTabNumAndSetTabData()
        {
            int posTypeMax = 0;
            foreach (var a in tbtag_Func.DataList)
            {
                if (a.tagId == 1)
                {
                    if (posTypeMax < a.posType)
                    {
                        posTypeMax = a.posType;
                    }
                }
            }

            var tagList = new List<tag_func>();
            for (int i = 1; i <= posTypeMax; i++)
            {
                foreach (var a in tbtag_Func.DataList)
                {
                    if (a.tagId == 1 && a.posType == i)
                    {
                        tagList.Add(a);
                        break;
                    }
                }
            }

            tagList.Sort(delegate(tag_func t1, tag_func t2) { return t1.posType.CompareTo(t2.posType); });

            foreach (var a in tagList)
            {
                TabNameList.Add(tblanguage.Get(a.sub1Name).current);
                TabIconList.Add(a.sub1Icon);
            }

            return posTypeMax;
        }

        private int GetSecTabNume(int posType)
        {
            int pos2Type = 0;
            foreach (var a in tbtag_Func.DataList)
            {
                if (a.tagId == 1 && a.posType == posType)
                {
                    if (pos2Type < a.sub2Pos)
                    {
                        pos2Type = a.sub2Pos;
                    }
                }
            }

            return pos2Type;
        }

        /// <summary>
        /// 商店最上面的长条
        /// </summary>
        private void FirstLevelTab()
        {
            int FirstLevelTabNum = GetTabNumAndSetTabData();
            MaxPosType = FirstLevelTabNum;
            List<TopTabStruct> tabStructs = new List<TopTabStruct>();
            for (int i = 0; i < FirstLevelTabNum; i++)
            {
                int ihelp = i + 1;
                var tabStructHelp = new TopTabStruct();
                tabStructHelp.TabWidth = 264;
                tabStructHelp.bg = "img_shopbtn_unselected";
                tabStructHelp.selectBg = "img_shopbtn_selected";
                tabStructHelp.name = TabNameList[i];
                tabStructHelp.icon = TabIconList[i];
                tabStructs.Add(tabStructHelp);
            }

            var thisLocalTransform = this.GetComponent<Transform>();

            var tabPos = this.GetFromReference(KPos_Tab).GetList();
            var ui =
                tabPos.CreateWithUIType<List<TopTabStruct>>(UIType.UICommon_TopTab, tabStructs,
                    true) as UICommon_TopTab;
            TopTabUi = ui;
            ui.GetRectTransform().SetOffsetWithTop(0);
            ui.GetRectTransform().SetOffsetWithBottom(0);

            SetFirstLevelTabOnClick(ui);
            //UnicornUIHelper.ForceRefreshLayout(this.GetFromReference(KPos_Tab));


            //初始化红点 为了外层红点
            foreach (var tbfc in tbtag_Func.DataList)
            {
                SetEveryModuleRedPointState(tbfc.id);
            }
        }

        private void SetTabRedPoint()
        {
            //RedPointMgr.instance.Add(ShopRoot, null, null, RedPointType.Enternal);
        }

        private void SetFirstLevelTabOnClick(UICommon_TopTab topTab)
        {
            int i = 1;
            foreach (var tab in topTab.TabList)
            {
                Log.Debug($"第:{i}个页签", Color.cyan);
                int ihelp = i;
                var KBtn_Common = tab.GetFromReference(UISubPanel_CommonBtn.KBtn_Common);
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common, () =>
                {
                    if (ihelp == NowPosType1)
                    {
                        return;
                    }
                    CloseAllTips();
                    SingleTabOnClick(ihelp);
                },audioId:0);

                // tab.GetFromReference(UISubPanel_CommonBtn.KBtn_Common).GetButton().OnClick?.Add(() =>
                // {
                //     
                // });

                var nodeName = NodeNames.GetTagRedDotName(1) + '|' + ihelp.ToString();
                RedDotManager.Instance.AddListener(nodeName,
                    (num) => { tab.GetFromReference(UISubPanel_CommonBtn.KImg_RedDot)?.SetActive(num > 0); });

                int sub2Pos = 0;
                foreach (var tbfc in tbtag_Func.DataList)
                {
                    if (tbfc.tagId == 1)
                    {
                        if (tbfc.posType == ihelp)
                        {
                            sub2Pos = tbfc.sub2Pos;
                            break;
                        }
                    }
                }

                if (sub2Pos != 0)
                {
                    Dictionary<int, string> intStrDic = new Dictionary<int, string>();
                    foreach (var tbfc in tbtag_Func.DataList)
                    {
                        if (tbfc.tagId == 1)
                        {
                            if (tbfc.posType == ihelp)
                            {
                                if (intStrDic.ContainsKey(tbfc.sub2Pos))
                                {
                                }
                                else
                                {
                                    intStrDic.Add(tbfc.sub2Pos, tbfc.sub2Name);
                                }
                            }
                        }
                    }

                    //foreach (var intStrPair in intStrDic)
                    //{
                    //    RedPointMgr.instance.Add(ShopRoot, intStrPair.Value, TopTabRedPointStr[ihelp],
                    //        RedPointType.Enternal);
                    //}
                }


                i++;
            }
        }

        /// <summary>
        /// 构建商店基础红点树并初始化最外层红点
        /// </summary>
        private void BuildModleRedDot()
        {
            Log.Debug("redPointInit", Color.cyan);

            int tagId = 1;

            var tagName = NodeNames.GetTagRedDotName(tagId);

            foreach (var tbfc in tbtag_Func.DataList)
            {
                if (tbfc.tagId == 1)
                {
                    var redDotName1 = tagName + '|' + tbfc.posType.ToString();

                    RedDotManager.Instance.InsterNode(redDotName1);
                    if (tbfc.posType != 4)
                    {
                        var redDotName2 = redDotName1 + '|' + tbfc.id.ToString();
                        RedDotManager.Instance.InsterNode(redDotName2);
                    }
                    else
                    {
                        var redDotName2 = redDotName1 + '|' + tbfc.sub2Pos.ToString();
                        RedDotManager.Instance.InsterNode(redDotName2);
                        var redDotName3 = redDotName2 + '|' + tbfc.id.ToString();
                        RedDotManager.Instance.InsterNode(redDotName3);
                    }

                    //if (tbfc.sub2Pos == 0)
                    //{


                    //InitRedPointByModuleID(tbfc.id,tbfc.posType);
                    //    //SetEveryModuleRedPointState(tbfc.id);
                    //}
                    //else
                    //{
                    //    InitRedPointByModuleID(tbfc.id, tbfc.sub2Name);
                    //}
                }
            }


            var node = RedDotManager.Instance.GetNode(tagName + "|4");
            node.PrintTree();
        }

        #region redpoint

        /// <summary>
        /// 设置商店外部显示时红点
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="parentPos"></param>
        private void InitRedPointByModuleID(int moduleID, int parentPos)
        {
            //switch (moduleID)
            //{
            //    case 1101:
            //        Module1101_InitRedPoint(parentKey);
            //        break;
            //    case 1102:
            //        Module1102_InitRedPoint(parentKey);
            //        break;
            //    case 1103:
            //        Module1103_InitRedPoint(parentKey);
            //        break;
            //    case 1201:
            //        Module1201_InitRedPoint(parentKey);
            //        break;
            //    case 1202:
            //        Module1202_InitRedPoint(parentKey);
            //        break;
            //    case 1301:
            //        Module1301_InitRedPoint(parentKey);
            //        break;
            //    case 1302:
            //        Module1302_InitRedPoint(parentKey);
            //        break;
            //    case 1401:
            //        Module1401_InitRedPoint(parentKey);
            //        break;
            //    case 1402:
            //        Module1402_InitRedPoint(parentKey);
            //        break;
            //    case 1403:
            //        Module1403_InitRedPoint(parentKey);
            //        break;
            //    case 1404:
            //        Module1404_InitRedPoint(parentKey);
            //        break;
            //}
        }

        private void Module1101_InitRedPoint(string parentKey)
        {
            //foreach (var tbdb in tbdraw_Box.DataList)
            //{
            //    if (tbdb.tagFunc == 1101)
            //    {
            //        RedPointMgr.instance.Add(ShopRoot, "module1101" + tbdb.id.ToString() + "one", parentKey,
            //            RedPointType.Once);
            //        RedPointMgr.instance.Add(ShopRoot, "module1101" + tbdb.id.ToString() + "ten", parentKey,
            //            RedPointType.Once);
            //        RedPointMgr.instance.SetState(ShopRoot, "module1101" + tbdb.id.ToString() + "one",
            //            RedPointState.Hide, 0);
            //        RedPointMgr.instance.SetState(ShopRoot, "module1101" + tbdb.id.ToString() + "ten",
            //            RedPointState.Hide, 0);
            //    }
            //}
        }

        private void Module1102_InitRedPoint(string parentKey)
        {
            foreach (var tbdb in tbdraw_Box.DataList)
            {
                if (tbdb.tagFunc == 1102)
                {
                    //RedPointMgr.instance.Add(ShopRoot, "module1102" + tbdb.id.ToString() + "one", parentKey,
                    //    RedPointType.Once);
                    //RedPointMgr.instance.Add(ShopRoot, "module1102" + tbdb.id.ToString() + "ten", parentKey,
                    //    RedPointType.Once);
                    //RedPointMgr.instance.SetState(ShopRoot, "module1102" + tbdb.id.ToString() + "one",
                    //    RedPointState.Hide, 0);
                    //RedPointMgr.instance.SetState(ShopRoot, "module1102" + tbdb.id.ToString() + "ten",
                    //    RedPointState.Hide, 0);
                }
            }
        }

        private void Module1103_InitRedPoint(string parentKey)
        {
            foreach (var tbdb in tbdraw_Box.DataList)
            {
                if (tbdb.tagFunc == 1103)
                {
                    //RedPointMgr.instance.Add(ShopRoot, "module1103" + tbdb.id.ToString(), parentKey, RedPointType.Once);
                    //RedPointMgr.instance.SetState(ShopRoot, "module1103" + tbdb.id.ToString(), RedPointState.Hide, 0);
                }
            }
        }

        private void Module1201_InitRedPoint(string parentKey)
        {
            //RedPointMgr.instance.Add(ShopRoot, "module1201", parentKey, RedPointType.Once);
            //RedPointMgr.instance.SetState(ShopRoot, "module1201", RedPointState.Hide, 0);
        }

        private void Module1202_InitRedPoint(string parentKey)
        {
            foreach (var tbsl in tbspecials.DataList)
            {
                if (tbsl.freeRule.Count != 0)
                {
                    //RedPointMgr.instance.Add(ShopRoot, "module1202" + tbsl.id.ToString(), parentKey, RedPointType.Once);
                    //RedPointMgr.instance.SetState(ShopRoot, "module1202" + tbsl.id.ToString(), RedPointState.Hide, 0);
                }
            }
        }

        private void Module1301_InitRedPoint(string parentKey)
        {
        }

        private void Module1302_InitRedPoint(string parentKey)
        {
            if (ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1302))
            {
                var giftList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList;
                foreach (var gf in giftList)
                {
                    if (gf.EndTime > TimeHelper.ClientNowSeconds() && tbgift.Get(gf.GiftId).freeRule.Count != 0)
                    {
                        //RedPointMgr.instance.Add(ShopRoot, "module1302" + gf.GiftId.ToString(), parentKey,
                        //    RedPointType.Once);
                        //RedPointMgr.instance.SetState(ShopRoot, "module1302" + gf.GiftId.ToString(), RedPointState.Hide,
                        //    0);
                    }
                }
            }
        }

        private void Module1401_InitRedPoint(string parentKey)
        {
        }

        private void Module1402_InitRedPoint(string parentKey)
        {
            //RedPointMgr.instance.Add(ShopRoot, "module1402", parentKey, RedPointType.Once);
            //RedPointMgr.instance.SetState(ShopRoot, "module1402", RedPointState.Hide, 0);
        }

        private void Module1403_InitRedPoint(string parentKey)
        {
            //RedPointMgr.instance.Add(ShopRoot, "module1403", parentKey, RedPointType.Once);
            //RedPointMgr.instance.SetState(ShopRoot, "module1403", RedPointState.Hide, 0);
        }

        private void Module1404_InitRedPoint(string parentKey)
        {
            //foreach (var tbm in tbmonthly.DataList)
            //{
            //    RedPointMgr.instance.Add(ShopRoot, "module1404" + tbm.id, parentKey, RedPointType.Once);
            //    RedPointMgr.instance.SetState(ShopRoot, "module1404" + tbm.id, RedPointState.Hide, 0);
            //}
        }

        private void RemoveRedPoint()
        {
            //RedPointMgr.instance.Remove(ShopRoot, ShopRoot);
        }

        #endregion

        #region set redpoint

        private void SetEveryModuleRedPointState(int moduleID)
        {
            switch (moduleID)
            {
                case 1101:
                    SetModule1101RedPointState();
                    break;
                case 1102:
                    SetModule1102RedPointState();
                    break;
                case 1103:
                    SetModule1103RedPointState();
                    break;
                case 1201:
                    SetModule1201RedPointState().Forget();
                    break;
                case 1202:
                    SetModule1202RedPointState();
                    break;
                case 1301:
                    SetModule1301RedPointState();
                    break;
                case 1302:
                    SetModule1302RedPointState();
                    break;
                case 1401:
                    SetModule1401RedPointState();
                    break;
                case 1402:
                    SetModule1402RedPointState();
                    break;
                case 1403:
                    SetModule1403RedPointState();
                    break;
                case 1404:
                    SetModule1404RedPointState();
                    break;
            }
        }

        private void SetModule1101RedPointState()
        {
        }

        private void SetModule1102RedPointState()
        {
        }

        private void SetModule1103RedPointState()
        {
        }

        private async UniTaskVoid SetModule1201RedPointState()
        {
            if (ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1201))
            {
                var dbList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList;
                DailyBuy dailyBuy = new DailyBuy();
                foreach (var b in dbList)
                {
                    if (tbshop_Daily.Get(b.Sign).pos == 1)
                    {
                        dailyBuy = b;
                        break;
                    }
                }

                int sign = dailyBuy.Sign;
                int times = dailyBuy.BuyCount;
                long upTime = dailyBuy.UpTime;

                if (times < tbshop_Daily.Get(sign).times)
                {
                    //位置1
                    if (TimeHelper.ClientNowSeconds() - upTime > tbconstant.Get("shop_daily_ad_cd").constantValue)
                    {
                        //不在CD

                        var redDotStr = NodeNames.GetTagFuncRedDotName(1201);
                        RedDotManager.Instance.SetRedPointCnt(redDotStr, 1);
                    }
                    else
                    {
                        //在CD

                        var redDotStr = NodeNames.GetTagFuncRedDotName(1201);
                        RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                        long cdTime = tbconstant.Get("shop_daily_ad_cd").constantValue - TimeHelper.ClientNowSeconds() +
                                      upTime;

                        await UniTask.Delay((int)cdTime * 1000);
                    }
                }
                else
                {
                    var redDotStr = NodeNames.GetTagFuncRedDotName(1201);
                    RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                }
            }
        }

        private void SetModule1202RedPointState()
        {
            foreach (var tbsl in tbspecials.DataList)
            {
                if (tbsl.freeRule.Count != 0)
                {
                    if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.TryGetValue(1202, out var indexModule))
                    {
                        continue;
                    }

                    var gsList = indexModule.GameSpecialsList;
                    GameSpecials gameSpecials = new GameSpecials();
                    foreach (var gs in gsList)
                    {
                        if (gs.SpecialId == tbsl.id)
                        {
                            gameSpecials = gs;
                            break;
                        }
                    }

                    if (gameSpecials.FreeCount == 1)
                    {
                        var redDotStr = NodeNames.GetTagFuncRedDotName(1202);
                        RedDotManager.Instance.SetRedPointCnt(redDotStr + gameSpecials.Type, 1);
                        //itemUI.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);
                        //RedPointMgr.instance.SetState(ShopRoot, "module1202" + tbsl.id.ToString(), RedPointState.Show);
                    }
                    else
                    {
                        var redDotStr = NodeNames.GetTagFuncRedDotName(1202);
                        RedDotManager.Instance.SetRedPointCnt(redDotStr + gameSpecials.Type, 0);

                        //RedDotManager.Instance.AddRedPointCnt(redDotStr, 1);
                        //RedPointMgr.instance.SetState(ShopRoot, "module1202" + tbsl.id.ToString(), RedPointState.Hide);
                    }
                }
            }
        }

        private void SetModule1301RedPointState()
        {
        }

        private void SetModule1302RedPointState()
        {
            if (ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1302))
            {
                var giftList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList;
                foreach (var gf in giftList)
                {
                    var redDotStr = NodeNames.GetTagFuncRedDotName(1302);
                    RedDotManager.Instance.InsterNode(redDotStr + gf.GiftId.ToString());
                    RedDotManager.Instance.SetRedPointCnt(redDotStr + gf.GiftId, 0);
                    if (gf.EndTime > TimeHelper.ClientNowSeconds() && tbgift.Get(gf.GiftId).freeRule.Count != 0)
                    {
                        if (gf.FreeTimes > 0)
                        {
                            RedDotManager.Instance.SetRedPointCnt(redDotStr + gf.GiftId, 1);
                            //RedPointMgr.instance.SetState(ShopRoot, "module1302" + gf.GiftId.ToString(),
                            //    RedPointState.Show, 0);
                            //SetModule1302RedPointStateHelp(gf).Forget();
                        }
                        else
                        {
                            RedDotManager.Instance.SetRedPointCnt(redDotStr + gf.GiftId, 0);
                            //RedPointMgr.instance.SetState(ShopRoot, "module1302" + gf.GiftId.ToString(),
                            //    RedPointState.Hide, 0);
                        }
                    }
                }
            }
        }

        private async UniTaskVoid SetModule1302RedPointStateHelp(GiftInfo gf)
        {
            await UniTask.Delay((int)(gf.EndTime - TimeHelper.ClientNowSeconds() + 1) * 1000);
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI uuii))
            {
                //RedPointMgr.instance.SetState(ShopRoot, "module1302" + gf.GiftId.ToString(), RedPointState.Hide, 0);
                if (NowPosType1 == tbtag_Func.Get(1302).posType)
                {
                    UpDateNowInterface();
                }
            }
        }

        /// <summary>
        /// 1401没有红点
        /// </summary>
        private void SetModule1401RedPointState()
        {
            var redDotStr = NodeNames.GetTagFuncRedDotName(1401);
            RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
        }

        private void SetModule1402RedPointState(UI secTabUI = default, int subpos = 0)
        {
            foreach (var rc in tbrecharge.DataList)
            {
                if (rc.tagFunc == 1402)
                {
                    if (rc.freeRule.Count > 0)
                    {
                        var clist = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1402].ChargeInfoList;
                        ChargeInfo chargeInfo = new ChargeInfo();
                        foreach (var c in clist)
                        {
                            if (c.Id == rc.id)
                            {
                                chargeInfo = c;
                                break;
                            }
                        }

                        if (chargeInfo.FreeSum > 0)
                        {
                            var redDotStr = NodeNames.GetTagFuncRedDotName(1402);
                            RedDotManager.Instance.SetRedPointCnt(redDotStr, 1);
                            if (secTabUI != default && subpos == 1)
                            {
                                secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KImg_RedPoint).SetActive(true);
                            }
                        }
                        else if (chargeInfo.AdSum > 0)
                        {
                            var redDotStr = NodeNames.GetTagFuncRedDotName(1402);
                            RedDotManager.Instance.SetRedPointCnt(redDotStr, 1);
                            if (secTabUI != default && subpos == 1)
                            {
                                secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KImg_RedPoint).SetActive(true);
                            }
                        }
                        else
                        {
                            var redDotStr = NodeNames.GetTagFuncRedDotName(1402);
                            RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                            if (secTabUI != default && subpos == 1)
                            {
                                secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KImg_RedPoint).SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        public void SetModule1403RedPointState(UI secTabUI = default, int subpos = 0)
        {
            var level = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Level;

            int fundNum = 0;
            foreach (var tbf in tbfund.DataList)
            {
                if (level >= tbf.unlockLevel)
                {
                    fundNum++;
                }
            }

            bool allRedPoint = false;

            #region upRedPoint

            bool upRedPoint = false;

            var fundReverseSortList = new List<fund>();
            foreach (var tbf in tbfund.DataList)
            {
                fundReverseSortList.Add(tbf);
            }

            fundReverseSortList.Sort(delegate(fund f1, fund f2) { return f2.unlockLevel.CompareTo(f1.unlockLevel); });

            List<int> upHaveRedPointList = new List<int>();

            for (int i = 0; i < fundReverseSortList.Count; i++)
            {
                int ihelp = i;
                if (fundReverseSortList.Count - ihelp <= fundNum)
                {
                    //Determine if there are red dots
                    bool haveRedDotOrNot = false;
                    foreach (var gf in ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList)
                    {
                        if (gf.FoundId == fundReverseSortList[ihelp].id)
                        {
                            if (gf.IsLook == 0)
                            {
                                haveRedDotOrNot = true;
                            }
                            else if (gf.IsLook == 1)
                            {
                                haveRedDotOrNot = false;
                            }

                            break;
                        }
                    }

                    //no red point, or determine red point
                    if (haveRedDotOrNot)
                    {
                        //have red dot
                        upHaveRedPointList.Add(fundReverseSortList[ihelp].id);
                    }
                    else
                    {
                        //no red dot
                    }
                }
            }

            if (upHaveRedPointList.Count > 0)
            {
                upRedPoint = true;
            }
            else
            {
                upRedPoint = false;
            }

            #endregion


            #region downRedPoint

            bool downRedPoint = false;

            foreach (var thisFundByID in ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList)
            {
                var thisFundRewardList = new List<fund_reward>();
                foreach (var tbfr in tbfund_Reward.DataList)
                {
                    if (tbfr.id == thisFundByID.FoundId)
                    {
                        thisFundRewardList.Add(tbfr);
                    }
                }

                thisFundRewardList.Sort(delegate(fund_reward f1, fund_reward f2)
                {
                    return f1.level.CompareTo(f2.level);
                });


                bool thisDownRedPoint = false;

                Dictionary<int, int> leftGetDic = new Dictionary<int, int>();
                Dictionary<int, int> midGetDic = new Dictionary<int, int>();
                Dictionary<int, int> rightGetDic = new Dictionary<int, int>();

                foreach (var tbfr in thisFundRewardList)
                {
                    leftGetDic.Add(tbfr.level, 0);
                    midGetDic.Add(tbfr.level, 0);
                    rightGetDic.Add(tbfr.level, 0);
                }

                var shopMap = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList;
                foreach (var f in shopMap)
                {
                    if (f.FoundId == thisFundRewardList[0].id)
                    {
                        if (f.RewardOneList != null)
                        {
                            foreach (int getLevel in f.RewardOneList)
                            {
                                leftGetDic[getLevel] = 1;
                            }
                        }

                        if (f.RewardTwoList != null)
                        {
                            foreach (int getLevel in f.RewardTwoList)
                            {
                                midGetDic[getLevel] = 1;
                            }
                        }

                        if (f.RewardThreeList != null)
                        {
                            foreach (int getLevel in f.RewardThreeList)
                            {
                                rightGetDic[getLevel] = 1;
                            }
                        }
                    }
                }

                List<int> haveRedPointList = new List<int>();
                haveRedPointList.Clear();

                foreach (var tbfr in thisFundRewardList)
                {
                    if (tbfr.level <= level)
                    {
                        if (leftGetDic[tbfr.level] == 0)
                        {
                            haveRedPointList.Add(tbfr.level);
                        }
                        else
                        {
                            //have Received
                        }

                        if (thisFundByID.RewardTwoStatus == 0)
                        {
                        }
                        else
                        {
                            if (midGetDic[tbfr.level] == 0)
                            {
                                haveRedPointList.Add(tbfr.level);
                            }
                            else
                            {
                                //have Received
                            }
                        }

                        if (thisFundByID.RewardThreeStatus == 0)
                        {
                        }
                        else
                        {
                            if (rightGetDic[tbfr.level] == 0)
                            {
                                haveRedPointList.Add(tbfr.level);
                            }
                            else
                            {
                                //have Received
                            }
                        }
                    }
                    else
                    {
                    }
                }

                if (haveRedPointList.Count > 0)
                {
                    //down have red point
                    thisDownRedPoint = true;
                }
                else
                {
                    //down do not have red point
                    thisDownRedPoint = false;
                }

                downRedPoint = downRedPoint || thisDownRedPoint;
            }

            #endregion


            if (upRedPoint || downRedPoint)
            {
                allRedPoint = true;
            }
            else
            {
                allRedPoint = false;
            }

            if (allRedPoint)
            {
                Log.Debug("有红点");
                var redDotStr = NodeNames.GetTagFuncRedDotName(1403);
                RedDotManager.Instance.SetRedPointCnt(redDotStr, 1);

                if (secTabUI != default && subpos == 2)
                {
                    secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KImg_RedPoint).SetActive(true);
                }

                //RedPointMgr.instance.SetState(ShopRoot, "module1403", RedPointState.Show);
            }
            else
            {
                var redDotStr = NodeNames.GetTagFuncRedDotName(1403);
                RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                if (secTabUI != default && subpos == 2)
                {
                    secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KImg_RedPoint).SetActive(false);
                }
                //RedPointMgr.instance.SetState(ShopRoot, "module1403", RedPointState.Hide);
            }
        }

        /// <summary>
        /// 月卡的外层红点
        /// </summary>
        private void SetModule1404RedPointState(UI secTabUI = default, int subpos = 0)
        {
            foreach (var tbm in tbmonthly.DataList)
            {
                var cardMap = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1404].SpecialCard.Unclaimed;
                UnReward cardData = new UnReward();
                foreach (var cm in cardMap)
                {
                    if (cm.Value.Id == tbm.id)
                    {
                        cardData = cm.Value;
                        break;
                    }
                }


                bool isBuy = false;
                if (cardData.EndTime <= TimeHelper.ClientNowSeconds())
                {
                    isBuy = false;
                }
                else
                {
                    isBuy = true;
                }

                int tagFuncID = 1404;
                var redDotStr = NodeNames.GetTagFuncRedDotName(tagFuncID) + '|';
                var redString = redDotStr + tbm.id.ToString();
                RedDotManager.Instance.InsterNode(redString);

                if (isBuy)
                {
                    //got or not get
                    if (cardData.SumDay < 1)
                    {
                        //got
                        RedDotManager.Instance.SetRedPointCnt(redString, 0);
                        if (secTabUI != default && subpos == 3)
                        {
                            secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KImg_RedPoint).SetActive(false);
                        }
                    }
                    else if (cardData.SumDay == 1)
                    {
                        RedDotManager.Instance.SetRedPointCnt(redString, 1);
                        if (secTabUI != default && subpos == 3)
                        {
                            secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KImg_RedPoint).SetActive(true);
                        }
                    }
                    else if (cardData.SumDay > 1)
                    {
                        RedDotManager.Instance.SetRedPointCnt(redString, 1);
                        if (secTabUI != default && subpos == 3)
                        {
                            secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KImg_RedPoint).SetActive(true);
                        }
                    }
                }
            }
        }

        #endregion

        private Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");

            float r = (byte)(System.Convert.ToUInt32(hex.Substring(0, 2), 16));
            float g = (byte)(System.Convert.ToUInt32(hex.Substring(2, 2), 16));
            float b = (byte)(System.Convert.ToUInt32(hex.Substring(4, 2), 16));

            r /= 255f;
            g /= 255f;
            b /= 255f;

            return new Color(r, g, b);
        }

        public void SingleTabOnClick(int inputPosType, int secTabInitPosType = 1, int fundIndexHelp = -1,
            bool isUpdate = false)
        {
            
            Log.Debug($"第:{inputPosType}个页签", Color.cyan);
            tagsTimes[inputPosType - 1]--;


            Module1201_Pos1UI = null;
            ModuleUIAndIDDic.Clear();


            if (ctsOld != null)
            {
                Log.Debug($"cts000 SingleTabOnClick ");
                ctsOld.Cancel();
                ctsOld.Dispose();
                cts.Cancel();
                cts.Dispose();
            }

            cts = new CancellationTokenSource();
            ctsOld = new CancellationTokenSource();

            if (isUpdate)
            {
                Module1403_IsNew = true;
            }
            else
            {
                Module1403_IsNew = false;
                this.GetFromReference(KContent).GetRectTransform().SetAnchoredPositionY(0);
            }

            //this.GetFromReference(KScrollView).GetScrollRect().OnValueChanged.Clear();
            Now1403FundIndex = fundIndexHelp;
            NowPosType1 = inputPosType;
            NowPosType2 = secTabInitPosType;
            ModuleClose();
            NowModuleIdList.Clear();
            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            shopViewList.Clear();
            var secTabViewList = this.GetFromReference(KPos_Sec_Tab).GetList();
            secTabViewList.Clear();

            var moduleIdList = new List<int>();
            foreach (var tagfunc in tbtag_Func.DataList)
            {
                if (tagfunc.tagId == 1 && tagfunc.posType == inputPosType)
                {
                    moduleIdList.Add(tagfunc.id);
                }
            }

            if (moduleIdList.Count <= 0)
            {
                return;
            }

            ContentH = 0;
            float scrollViewOffsetTop = BaseTopOffset;


            if (tbtag_Func.Get(moduleIdList[0]).sub2Pos == 0)
            {
                //no sec tab
                moduleIdList.Sort(delegate(int m1, int m2)
                {
                    return tbtag_Func.Get(m1).sort.CompareTo(tbtag_Func.Get(m2).sort);
                });

                foreach (int tagID in moduleIdList)
                {
                    CreateModuleByID(tagID, isUpdate);
                    NowModuleIdList.Add(tagID);
                    ContentH += ModuleDeltaY;
                }

                ContentH -= ModuleDeltaY;
            }
            else
            {
                //have sec tab
                //create sec tab btn base on a new ui prefab
                //and change scroll view offset top -> scrollViewOffsetTop += sec h
                scrollViewOffsetTop += SecTabHeight;
                var secTabPos = secTabViewList.CreateWithUIType(UIType.UICommon_Select_Tab, false);
                var secTabContentList = secTabPos.GetFromReference(UICommon_Select_Tab.KContent).GetList();
                secTabContentList.Clear();

                //find sec tab number
                int tabNum = GetSecTabNume(inputPosType);

                var tagList = new List<tag_func>();
                for (int i = 1; i <= tabNum; i++)
                {
                    foreach (var a in tbtag_Func.DataList)
                    {
                        if (a.tagId == 1 && a.posType == inputPosType && a.sub2Pos == i)
                        {
                            tagList.Add(a);
                            break;
                        }
                    }
                }

                tagList.Sort(delegate(tag_func t1, tag_func t2) { return t1.sub2Pos.CompareTo(t2.sub2Pos); });

                for (int i = 0; i < tagList.Count; i++)
                {
                    int ihelp = i;
                    var secTabUI = secTabContentList.CreateWithUIType(UIType.UISubPanel_Select_Tab_Btn, false);
                    int subpos = ihelp + 1;
                    Log.Debug($"subpos:{subpos}", Color.cyan);
                    var nodeName = NodeNames.GetTagRedDotName(1) + "|4|" + subpos.ToString();
                    //RedDotManager.Instance.InsterNode(nodeName);
                    //RedDotManager.Instance.RemoveAllListeners(nodeName);

                    SetModule1402RedPointState(secTabUI, subpos);
                    SetModule1403RedPointState(secTabUI, subpos);
                    SetModule1404RedPointState(secTabUI, subpos);
                    RedDotManager.Instance.RemoveAllListeners(nodeName);
                    RedDotManager.Instance.AddListener(nodeName, (num) =>
                    {
                        Log.Debug($"num:{num}", Color.cyan);
                        secTabUI?.GetFromReference(UISubPanel_Select_Tab_Btn.KImg_RedPoint)?.SetActive(num > 0);
                    });


                    var tabBtn = secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KBtn);
                    secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KText).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get(tagList[ihelp].sub2Name).current);
                    secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KBtn).GetImage().SetSpriteAsync("r2", false);
                    secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KText).GetTextMeshPro().SetFontSize(35);
                    var secTagList = new List<tag_func>();
                    foreach (var moduleID in moduleIdList)
                    {
                        if (tbtag_Func.Get(moduleID).sub2Pos == ihelp + 1)
                        {
                            secTagList.Add(tbtag_Func.Get(moduleID));
                        }
                    }

                    secTagList.Sort(delegate(tag_func t1, tag_func t2) { return t1.sort.CompareTo(t2.sort); });

                    if (ihelp + 1 == secTabInitPosType)
                    {
                        NowModuleIdList.Clear();
                        LastSecTabBtn = secTabUI;
                        secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KBtn).GetImage()
                            .SetSpriteAsync("r1", false);
                        secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KText).GetTextMeshPro().SetFontSize(45);
                        foreach (var secT in secTagList)
                        {
                            CreateModuleByID(secT.id, isUpdate);
                            NowModuleIdList.Add(secT.id);
                            ContentH += ModuleDeltaY;
                        }

                        ContentH -= ModuleDeltaY;
                    }

                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(tabBtn,
                        () => { OnSecTabBtnClick(scrollViewOffsetTop, ihelp, secTabUI, secTagList); });
                }


                UnicornUIHelper.ForceRefreshLayout(secTabPos.GetFromReference(UICommon_Select_Tab.KContent));
                //set sec tab onclick -> 
                //foreach create module and add module h & deltay -> minus one deltay -> set contenth 

                //onclick pos one
            }

            SetContentH(ContentH, scrollViewOffsetTop);
            UnicornUIHelper.ForceRefreshLayout(this.GetFromReference(KContent));


            //sort
            shopViewList.Sort(delegate(UI ui1, UI ui2)
            {
                return tbtag_Func.Get(ModuleUIAndIDDic[ui1]).sort
                    .CompareTo(tbtag_Func.Get(ModuleUIAndIDDic[ui2]).sort);
            });
        }

        private void OnSecTabBtnClick(float scrollViewOffsetTop, int ihelp, UI secTabUI, List<tag_func> secTagList)
        {
            if (secTabUI == LastSecTabBtn)
            {
                return;
            }


            Log.Debug("sectabbtn", Color.cyan);


            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            shopViewList.Clear();
            this.GetFromReference(KContent).GetRectTransform().SetAnchoredPositionY(0);
            ModuleUIAndIDDic.Clear();
            NowPosType2 = ihelp + 1;
            NowModuleIdList.Clear();

            ContentH = 0;

            LastSecTabBtn.GetFromReference(UISubPanel_Select_Tab_Btn.KBtn).GetImage().SetSpriteAsync("r2", false);
            LastSecTabBtn.GetFromReference(UISubPanel_Select_Tab_Btn.KText).GetTextMeshPro()
                .SetFontSize(42);

            secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KBtn).GetImage().SetSpriteAsync("r1", false);
            secTabUI.GetFromReference(UISubPanel_Select_Tab_Btn.KText).GetTextMeshPro().SetFontSize(54);

            foreach (var secT in secTagList)
            {
                CreateModuleByID(secT.id);
                NowModuleIdList.Add(secT.id);
                ContentH += ModuleDeltaY;
            }

            ContentH -= ModuleDeltaY;

            LastSecTabBtn = secTabUI;
            SetContentH(ContentH, scrollViewOffsetTop);
            UnicornUIHelper.ForceRefreshLayout(this.GetFromReference(KContent));
        }

        public void SetShopContentH(float input)
        {
            ContentH += input;
        }

        private void CreateModuleByID(int moduleID, bool isUpdate = false)
        {
            this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().spacing = 0;
            this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().padding.top = 0;
            switch (moduleID)
            {
                case 1101:
                    Module1101(isUpdate);
                    return;
                case 1102:
                    Module1102(isUpdate);
                    return;
                case 1103:
                    Module1103(isUpdate);
                    return;
                case 1201:
                    this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().spacing = -25;
                    Module1201(isUpdate);
                    return;
                case 1202:
                    this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().spacing = -25;
                    Module1202(isUpdate);
                    return;
                case 1301:
                    this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().spacing = 0;
                    Module1301(isUpdate);
                    return;
                case 1302:
                    this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().spacing = 0;
                    Module1302(isUpdate);
                    return;
                case 1401:

                    Module1401(isUpdate);
                    return;
                case 1402:
                    this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().padding.top = 11;
                    Module1402(isUpdate);
                    return;
                case 1403:
                    Module1403(isUpdate);
                    return;
                case 1404:
                    Module1404(isUpdate);
                    return;
                default:
                    Debug.Log("something is wrong");
                    return;
            }
        }

        #region module1101

        private Dictionary<UI, int> Module1101UIIDDic = new Dictionary<UI, int>();

        private async void Module1101(bool isUpdate)
        {
            Module1101UIIDDic.Clear();
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1101))
            {
                Debug.Log("no 1101");
                return;
            }
            else
            {
                //Debug.Log("111111111111111111111");
                Debug.Log(ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].BoxInfoList);
            }

            //return;
            int thisModuleID = 1101;
            var boxIDList = GetBoxIDByModuleID(thisModuleID);
            if (boxIDList.Count <= 0)
            {
                return;
            }

            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            var bgUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_Box_Bg, false);
            this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().spacing = 104;
            //bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetRectTransform().SetAnchoredPositionY(-10);
            bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetComponent<VerticalLayoutGroup>().spacing = 68;
            ModuleUIAndIDDic.Add(bgUI, thisModuleID);

            float thisModuleH = 0;
            if (boxIDList.Count > 0)
            {
                thisModuleH = 980 * boxIDList.Count + (boxIDList.Count - 1) * 30;
            }

            bgUI.GetRectTransform().SetHeight(thisModuleH);
            var boxUIList = bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetList();
            boxUIList.Clear();

            foreach (var id in boxIDList)
            {
                List<int> ints = new List<int>();
                ints.Add(thisModuleID);
                ints.Add(id);
                var ui = boxUIList.CreateWithUIType<List<int>>(UIType.UISubPanel_Shop_1102_SBox, ints, false);
                ////ui.GetFromReference(UISubPanel_Shop_1102_SBox.Kbox).GetImage().SetAlpha(0);
                ui.GetFromReference(UISubPanel_Shop_1102_SBox.KContainerDesc).GetComponent<CanvasGroup>().alpha = 1;
                ui.SetParent(this, false);
                ui.GetFromReference(UISubPanel_Shop_1102_SBox.KImg_RightUp).SetActive(true);

                if (tbdraw_Box.Get(id).limitType == 2 || tbdraw_Box.Get(id).limitType == 4)
                {
                    Module1101UIIDDic.Add(ui.GetFromReference(UISubPanel_Shop_1102_SBox.KText_RightUp), id);
                }
            }

            UnicornUIHelper.ForceRefreshLayout(bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg));
            var children = boxUIList.Children;
            await UniTask.Delay(400, cancellationToken: cts.Token);
            if (tagsTimes[0] >= 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    var ui = children[i].GetFromReference(UISubPanel_Shop_1102_SBox.KLimitedTimeBoxBg);
                    children[i].GetFromReference(UISubPanel_Shop_1102_SBox.KContainerDesc).GetComponent<CanvasGroup>().alpha = 0;
                    if (ui != null)
                    {
                        var height = ui.GetRectTransform().AnchoredPosition().y;
                        UnicornTweenHelper.SetEaseAlphaAndPosUtoB(ui, height, 800, cts.Token, 0.2f);
                    }
                }
            }



            ContentH += thisModuleH;
        }


        public void SetShopUIState()
        {
        }


        private void Module1101_Help_SetTextByTime()
        {
            if (Module1101UIIDDic != null)
            {
                foreach (var a in Module1101UIIDDic)
                {
                    var bList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].BoxInfoList;
                    BoxInfo boxInfo = new BoxInfo();
                    foreach (var box in bList)
                    {
                        if (box.Id == a.Value)
                        {
                            boxInfo = box;
                            break;
                        }
                    }

                    long startTime = boxInfo.SetStartTime;
                    long endTime = startTime + tbdraw_Box.Get(a.Value).dateLimit;
                    long deltaTime = endTime - TimeHelper.ClientNowSeconds();

                    if (deltaTime <= 0)
                    {
                        UpDateNowInterface();
                        return;
                    }
                    else
                    {
                        string str1 = tblanguage.Get("drawbox_type_1_text").current;
                        str1 = str1.Replace("{0}", UnicornUIHelper.GeneralTimeFormat(new int4(2, 4, 2, 1), deltaTime));
                        if (tbdraw_Box.Get(a.Value).limitType == 2)
                        {
                            a.Key.GetTextMeshPro().SetTMPText(str1);
                        }
                        else if (tbdraw_Box.Get(a.Value).limitType == 4)
                        {
                            string str2 = tblanguage.Get("drawbox_type_2_text").current;
                            str2 = str2.Replace("{0}",
                                (tbdraw_Box.Get(boxInfo.Id).timesLimit - boxInfo.DrawCount).ToString());
                            a.Key.GetTextMeshPro().SetTMPText(str1 + "\n" + str2);
                        }
                    }
                    //ResourcesSingletonOld.Instance.UpdateResourceUI
                }
            }
        }


        public List<UI> GetAll1102SBox()
        {
            List<UI> list = new List<UI>();
            list.Clear();
            var uiroot = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();

            var uis = uiroot.Children;
            for (int i = 0; i < uis.Count; i++)
            {
                var ui = uis[i];
                var uiBg = ui.GetFromReference(UISubPanel_Shop_Box_Bg.KBg);
                if (uiBg != null)
                {
                    var uitargets = uiBg.GetList().Children;
                    foreach (var item in uitargets)
                    {
                        if (item.Name.Contains("1102"))
                        {
                            Log.Debug($"itemname:{item.Name}");
                            list.Add(item);
                        }
                    }
                }
            }

            return list;
        }
        public List<UI> GetAll1103SBox()
        {
            List<UI> list = new List<UI>();
            list.Clear();
            var uiroot = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();

            var uis = uiroot.Children;
            for (int i = 0; i < uis.Count; i++)
            {
                var ui = uis[i];
                var uiBg = ui.GetFromReference(UISubPanel_Shop_Box_Bg.KBg);
                if (uiBg != null)
                {
                    var uitargets = uiBg.GetList().Children;
                    foreach (var item in uitargets)
                    {
                        if (item.Name.Contains("1103"))
                        {
                            Log.Debug($"itemname:{item.Name}");
                            list.Add(item);
                        }
                    }
                }
            }

            return list;
        }
        #endregion

        #region module1102

        private void Module1102(bool isUpdate)
        {
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1102))
            {
                return;
            }

            //Debug.Log("222222222");
            Debug.Log(ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1102].BoxInfoList);

            int thisModuleID = 1102;
            var boxIDList = GetBoxIDByModuleID(thisModuleID);
            if (boxIDList.Count <= 0)
            {
                return;
            }

            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            var bgUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_Box_Bg, false);
            this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().spacing = 104;
            bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetComponent<VerticalLayoutGroup>().spacing = 68;
            ModuleUIAndIDDic.Add(bgUI, thisModuleID);

            float thisModuleH = 0;
            if (boxIDList.Count > 0)
            {
                thisModuleH = 980 * boxIDList.Count + (boxIDList.Count - 1) * 30;
            }

            bgUI.GetRectTransform().SetHeight(thisModuleH);
            var boxUIList = bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetList();
            boxUIList.Clear();

            foreach (var item in boxIDList)
            {
                List<int> ints = new List<int>();
                ints.Add(1102);
                ints.Add(item);
                var ui = boxUIList.CreateWithUIType<List<int>>(UIType.UISubPanel_Shop_1102_SBox, ints, false);
                ui.GetFromReference(UISubPanel_Shop_1102_SBox.KImg_RightUp).SetActive(false);
            }

            UnicornUIHelper.ForceRefreshLayout(bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg));

            ContentH += thisModuleH;
        }

        #endregion

        #region module1103

        private void Module1103(bool isUpdate)
        {
            //return;
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1103))
            {
                return;
            }

            int thisModuleID = 1103;
            var boxIDList = GetBoxIDByModuleID(thisModuleID);
            if (boxIDList.Count <= 0)
            {
                return;
            }

            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            var bgUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_Box_Bg, false);
            ModuleUIAndIDDic.Add(bgUI, thisModuleID);
            this.GetFromReference(KContent).GetComponent<VerticalLayoutGroup>().spacing = 104;
            bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetComponent<VerticalLayoutGroup>().spacing = 30;
            float thisModuleH = 0;
            if (boxIDList.Count > 0)
            {
                thisModuleH = 257 * boxIDList.Count + (boxIDList.Count - 1) * 30;
            }

            bgUI.GetRectTransform().SetHeight(thisModuleH);
            var boxUIList = bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetList();
            boxUIList.Clear();

            foreach (var id in boxIDList)
            {
                List<int> ints = new List<int>();
                ints.Add(1103);
                ints.Add(id);
                var boxUI = boxUIList.CreateWithUIType<List<int>>(UIType.UISubPanel_Shop_1103_Box, ints, false);
                
                //Module1103_Help_TxtAndImgSet(boxUI, id);
            }

            UnicornUIHelper.ForceRefreshLayout(bgUI.GetFromReference(UISubPanel_Shop_Box_Bg.KBg));

            ContentH += thisModuleH;
        }

        private void Module1103_Help_TxtAndImgSet(UI boxUI, int id)
        {
            var boxList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1103].BoxInfoList;
            BoxInfo boxInfo = new BoxInfo();
            foreach (var b in boxList)
            {
                if (b.Id == id)
                {
                    boxInfo = b;
                    break;
                }
            }

            int a1 = boxInfo.Numbers[0];
            int BoxID = id;

            boxUI.GetFromReference(UISubPanel_Shop_1103_Box.KBoxTitleText).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbdraw_Box.Get(BoxID).name).current);


            List<int> descQualityList = tbdraw_Box.Get(BoxID).descPara;
            //int descNum = ;

            List<string> strings = new List<string>(descQualityList.Count);
            for (int i = 0; i < descQualityList.Count; i++)
            {
                string helpStr1 = tblanguage.Get(tbquality.Get(descQualityList[i]).name).current;
                helpStr1 = UnityHelper.RichTextColor(helpStr1, tbquality.Get(descQualityList[i]).fontColor);
                Log.Debug(helpStr1, Color.cyan);
                strings.Add(helpStr1);
                //equipTxt = string.Format(equipTxt.Replace("{" + i.ToString() + "}", helpStr1);
            }

            string BoxDescHelpTxt = "drawbox_num" + descQualityList.Count.ToString() + "_text";
            string equipTxt = string.Format(tblanguage.Get(BoxDescHelpTxt).current, strings);

            string moneyTxt = tblanguage.Get("drawbox_money_text").current;
            moneyTxt = moneyTxt.Replace("{0}", tbdraw_Box.Get(BoxID).money.ToString());
            boxUI.GetFromReference(UISubPanel_Shop_1103_Box.KBoxDescText).GetTextMeshPro()
                .SetTMPText(moneyTxt + "," + equipTxt);


            List<Vector2> guaranteePara = new List<Vector2>();
            guaranteePara = tbdraw_Box.Get(BoxID).guaranteePara;
            int guaranteeLength = guaranteePara.Count;
            string helpStr2 = tblanguage.Get("drawbox_guarantee_text").current;
            helpStr2 = helpStr2.Replace("{0}", a1.ToString());
            if (guaranteeLength == 1)
            {
                helpStr2 = helpStr2.Replace("{1}", UnityHelper.RichTextColor(
                    tblanguage.Get(tbquality.Get((int)tbdraw_Box.Get(BoxID).guaranteePara[0][0]).name).current,
                    tbquality.Get((int)tbdraw_Box.Get(BoxID).guaranteePara[0][0]).fontColor));
            }
            else
            {
                Debug.Log("guaranteePara is wrong!");
            }

            boxUI.GetFromReference(UISubPanel_Shop_1103_Box.KBoxTimeText).GetTextMeshPro().SetTMPText(helpStr2);

            boxUI.GetFromReference(UISubPanel_Shop_1103_Box.KBoxImage).GetImage()
                .SetSpriteAsync(tbdraw_Box.Get(BoxID).pic, false);
        }

        private void Module1103_Help_BtnSet()
        {
        }

        #endregion

        #region module1201

        private UI Module1201_TimeText = null;

        private bool Module1201_InCD = false;

        private UI Module1201_Pos1UI = null;

        /// <summary>
        /// 每日特供 每日特惠
        /// </summary>
        private void Module1201(bool isUpdate)
        {
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1201))
            {
                return;
            }
            //if (!ResourcesSingletonOld.Instance.unlockList.Contains(1201))
            //{
            //    return;
            //}


            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            var bgUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_ModelBG, false);
            ModuleUIAndIDDic.Add(bgUI, 1201);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KImg_NoGift).SetActive(false);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KImageBg).SetActive(false);
            bgUI.GetImage(UISubPanel_Shop_ModelBG.KPos_Layout).SetAlpha(0f);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.top = 13;
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.left = 0;
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().cellSize =
                new Vector2(310, 425);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().spacing =
                new Vector2(50, 0);
            float thisModuleH = 2 * 450 + 100;
            bgUI.GetRectTransform().SetHeight(thisModuleH);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KTitleText).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbtag_Func.Get(1201).name).current);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KDescText).SetActive(true);
            Module1201_TimeText = bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KDescText);
            Module1201_Help_SetTextByTime();

            var bgList = bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetList();
            bgList.Clear();
            Module1201_Help_CreateItem(bgList, bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout), bgUI,
                isUpdate);


            ContentH += thisModuleH;

            var node = RedDotManager.Instance.GetNode(NodeNames.GetTagRedDotName(1));
            node.PrintTree();
        }

        private async UniTaskVoid Module1201_Help_CreateItem(UIListComponent bgList, UI layoutUI, UI bgUI,
            bool isUpdate)
        {
            int dailyShopNumber = 6;
            for (int i = 1; i < dailyShopNumber + 1; i++)
            {
                int ihelp = i;
                var ui = bgList.CreateWithUIType<int3>(UIType.UISubPanel_Shop_item,
                    new int3(1201, ihelp, isUpdate ? 1 : 0), false);
                ui.SetParent(this, false);
                ui.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
                ui.GetFromReference(UISubPanel_Shop_item.KItemTitleTopBg).SetActive(false);
                ui.GetFromReference(UISubPanel_Shop_item.KGoldText).SetActive(false);
                ui.GetImage(UISubPanel_Shop_item.KBg).SetAlpha(1);
                var dbList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList;
                DailyBuy dailyBuy = new DailyBuy();
                foreach (var b in dbList)
                {
                    if (tbshop_Daily.Get(b.Sign).pos == ihelp)
                    {
                        dailyBuy = b;
                        break;
                    }
                }

                int sign = dailyBuy.Sign;
                int times = dailyBuy.BuyCount;
                long upTime = dailyBuy.UpTime;
                if (tbshop_Daily.DataMap.ContainsKey(sign))
                {
                    var reList = ui.GetFromReference(UISubPanel_Shop_item.KRewardItemPos).GetList();
                    reList.Clear();
                    var rewardItemUI = reList.CreateWithUIType<Vector3>(UIType.UICommon_RewardItem,
                        tbshop_Daily.Get(sign).reward[0], false);
                    rewardItemUI.GetFromReference(UICommon_RewardItem.KImg_Outer).SetActive(false);
                    rewardItemUI.GetRectTransform().SetScale(new Vector2(1.27f, 1.27f));
                    UnicornUIHelper.SetRewardOnClick(tbshop_Daily.Get(sign).reward[0], rewardItemUI);
                    ui.GetFromReference(UISubPanel_Shop_item.KRewardText).GetTextMeshPro()
                        .SetTMPText(UnicornUIHelper.GetRewardName(tbshop_Daily.Get(sign).reward[0]));

                    ui.GetFromReference(UISubPanel_Shop_item.KRewardText).SetActive(true);
                    ui.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
                    ui.GetFromReference(UISubPanel_Shop_item.KItemTitleTopBg).SetActive(false);
                    ui.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(false);
                    ui.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(false);
                    ui.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(false);
                    ui.GetFromReference(UISubPanel_Shop_item.KItemImage).SetActive(false);
                    ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);
                    ui.GetImage(UISubPanel_Shop_item.KBg).SetAlpha(255f);
                    //判断折扣显示
                    if (tbshop_Daily[sign].discount != 0)
                    {
                        ui.GetFromReference(UISubPanel_Shop_item.KCutText).GetTextMeshPro().SetTMPText(
                            tbshop_Daily[sign].discount.ToString() +
                            tblanguage.Get("shop_daily_discount_text").current);
                        if (tbshop_Daily[sign].discountPic != "")
                        {
                            ui.GetFromReference(UISubPanel_Shop_item.KCutBg).GetImage()
                                .SetSpriteAsync(tbshop_Daily.Get(sign).discountPic, false);
                        }

                        ui.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(true);
                    }
                    else
                    {
                        ui.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(false);
                    }

                    if (times < tbshop_Daily.Get(sign).times)
                    {
                        //位置1
                        if (tbshop_Daily.Get(sign).pos == 1)
                        {
                            Module1201_Pos1UI = ui;
                            ui.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(true);
                            ui.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_green", false);

                            //不在cd
                            if (TimeHelper.ClientNowSeconds() - upTime >
                                tbconstant.Get("shop_daily_ad_cd").constantValue)
                            {
                                var redDotStr = NodeNames.GetTagFuncRedDotName(1201);
                                RedDotManager.Instance.SetRedPointCnt(redDotStr, 1);
                                ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);


                                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);

                                ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);

                                ui.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(true);
                                ui.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_green", false);

                                //判断是广告免费还是免费
                                if (times < tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1])
                                {
                                    //ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(true);
                                    Log.Debug(
                                        $"次数:{tbshop_Daily[sign].freeRule[0][0]},次数2:{tbshop_Daily[sign].freeRule[0][1]}",
                                        Color.cyan);
                                    //免费
                                    if (tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1] -
                                        times > 1)
                                    {
                                        ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                            .SetTMPText(tblanguage.Get("common_free_text").current + "(" +
                                                        (tbshop_Daily[sign].freeRule[0][0] -
                                                         tbshop_Daily[sign].freeRule[0][1] - times).ToString()
                                                        + ")");
                                    }
                                    else
                                    {
                                        ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                            .SetTMPText(tblanguage.Get("common_free_text").current);
                                    }
                                }
                                else
                                {
                                    //广告免费

                                    //ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(true);
                                    if (tbshop_Daily[sign].freeRule[0][1] -
                                        (times - (tbshop_Daily[sign].freeRule[0][0] -
                                                  tbshop_Daily[sign].freeRule[0][1])) > 1)
                                    {
                                        ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                            .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +tblanguage.Get("common_free_text").current + "(" +
                                                        (tbshop_Daily[sign].freeRule[0][1]
                                                         - (times - (tbshop_Daily[sign].freeRule[0][0] -
                                                                     tbshop_Daily[sign].freeRule[0][1]))).ToString()
                                                        + ")");
                                    }
                                    else
                                    {
                                        ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                            .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                                                        tblanguage.Get("common_free_text").current);
                                    }
                                }

                                ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
                            }
                            //在cd
                            else
                            {
                                var redDotStr = NodeNames.GetTagFuncRedDotName(1201);
                                RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                                ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);


                                //在CD
                                //RedPointMgr.instance.SetState("ShopRoot", "module1201", RedPointState.Hide);
                                long cdTime = tbconstant.Get("shop_daily_ad_cd").constantValue -
                                    TimeHelper.ClientNowSeconds() + upTime;
                                //cdTime = -cdTime;

                                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(false);
                                ui.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(false);
                                ui.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_yellow_1", false);
                                ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                                ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                                ui.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                                    .SetTMPText(UnicornUIHelper.GeneralTimeFormat(new int4(1, 2, 1, 1), cdTime) + "\n"
                                        + tblanguage.Get("shop_daily_1_countdown_text").current);
                                Module1201_InCD = true;
                            }
                        }
                        //其他位置
                        else
                        {
                            //除了1之外的其他位置
                            string str = tbshop_Daily[sign].cost[0][2].ToString();
                            //ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro().SetTMPText(tbshop_Daily[sign].cost[0][2].ToString());
                            if (tbshop_Daily[sign].cost[0][0] == 3)
                            {
                                if (ResourcesSingletonOld.Instance.UserInfo.RoleAssets.UsBill <
                                    tbshop_Daily[sign].cost[0][2])
                                {
                                    str = UnityHelper.RichTextColor(str, "FF0000");
                                }


                                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);

                                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                    .SetTMPText(UnicornUIHelper.GetRewardTextIconName(tbuser_Variable[3].icon) + str);
                                //消耗游戏币
                            }

                            if (tbshop_Daily[sign].cost[0][0] == 2)
                            {
                                if (ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin <
                                    tbshop_Daily[sign].cost[0][2])
                                {
                                    str = UnityHelper.RichTextColor(str, "FF0000");
                                }


                                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);

                                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                    .SetTMPText(UnicornUIHelper.GetRewardTextIconName(tbuser_Variable[2].icon) + str);
                                //消耗充值货币
                            }

                            ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
                        }
                    }
                    //没有购买次数
                    else
                    {
                        if (tbshop_Daily[sign].pos == 1)
                        {
                            ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(false);


                            //当前位置位1
                            //RedPointMgr.instance.SetState("ShopRoot", "module1201", RedPointState.Hide);
                            ui.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro().SetTMPText(
                                tblanguage.Get("shop_daily_gained_text").current);
                            ui.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(false);
                            ui.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_yellow_1", false);


                            var redDotStr = NodeNames.GetTagFuncRedDotName(1201);
                            RedDotManager.Instance.SetRedPointCnt(redDotStr, 0);
                            ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);
                        }
                        else
                        {
                            //当前位置为除了1之外的位置
                            ui.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro().SetTMPText(
                                tblanguage.Get("shop_daily_purchased_text").current);

                            ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(false);
                        }

                        ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                        ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                    }
                }

                var BuyBtn = ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common);


                //UnicornTweenHelper.SetScaleWithBounce(ui.GetFromReference(UISubPanel_Shop_item.KBg),0.2f,1.2f,0,1,Ease.OutQuad);
            }

            bgList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Shop_item;
                var uib = b as UISubPanel_Shop_item;
                return uia.ID.CompareTo(uib.ID);
            });
            UnicornUIHelper.ForceRefreshLayout(layoutUI);

            await UniTask.Delay(1000 * (int)TimeHelper.GetToTomorrowTime());
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui1))
            {
                //发送查询
                //Debug.Log("发送1201查询");
                WebMessageHandlerOld.Instance.AddHandler(11, 6, On1201QueryResponse);
                NetWorkManager.Instance.SendMessage(11, 6);
            }
        }


        private void Module1201_Help_SetTextByTime()
        {
            if (ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1201))
            {
                string str = tblanguage.Get("shop_daily_countdown_text").current;
                str = str + UnicornUIHelper.GeneralTimeFormat(new int4(2, 3, 2, 1), TimeHelper.GetToTomorrowTime());
                if (Module1201_TimeText != null)
                {
                    Module1201_TimeText?.GetTextMeshPro()?.SetTMPText(str);
                }


                //item time
                var dbList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList;
                DailyBuy dailyBuy = new DailyBuy();
                foreach (var db in dbList)
                {
                    if (tbshop_Daily.Get(db.Sign).pos == 1)
                    {
                        dailyBuy = db;
                        break;
                    }
                }

                if (Module1201_Pos1UI != null)
                {
                    if (tbconstant.Get("shop_daily_ad_cd").constantValue - TimeHelper.ClientNowSeconds() +
                        dailyBuy.UpTime > 0)
                    {
                        long cdTime = tbconstant.Get("shop_daily_ad_cd").constantValue - TimeHelper.ClientNowSeconds() +
                                      dailyBuy.UpTime;
                        if (cdTime > 0)
                        {
                            Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(false);
                            Module1201_Pos1UI.GetImage(UISubPanel_Shop_item.KImg_Btn)
                                .SetSpriteAsync("icon_btn_yellow_1", false);
                            Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                                .SetTMPText(UnicornUIHelper.GeneralTimeFormat(new int4(1, 2, 1, 1), cdTime)
                                            + "\n" + tblanguage.Get("shop_daily_1_countdown_text").current);
                        }
                        else
                        {
                            int sign = dailyBuy.Sign;
                            int times = dailyBuy.BuyCount;
                            //判断一下
                            if (times < tbshop_Daily[sign].times)
                            {
                                //RedPointMgr.instance.SetState("ShopRoot", "12011", RedPointState.Show);
                                ResourcesSingletonOld.Instance.UpdateResourceUI();
                                Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);
                                if (times < tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1])
                                {
                                    //免费
                                    if (tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1] - times >
                                        1)
                                    {
                                        Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid)
                                            .GetTextMeshPro()
                                            .SetTMPText(tblanguage.Get("common_free_text").current + "(" +
                                                        (tbshop_Daily[sign].freeRule[0][0] -
                                                         tbshop_Daily[sign].freeRule[0][1] -
                                                         times).ToString()
                                                        + ")");
                                    }
                                    else
                                    {
                                        Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid)
                                            .GetTextMeshPro()
                                            .SetTMPText(tblanguage.Get("common_free_text").current);
                                    }
                                }
                                else
                                {
                                    Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                                    if (tbshop_Daily[sign].freeRule[0][1] -
                                        (times - (tbshop_Daily[sign].freeRule[0][0] -
                                                  tbshop_Daily[sign].freeRule[0][1])) > 1)
                                    {
                                        Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid)
                                            .GetTextMeshPro()
                                            .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                                                        tblanguage.Get("common_free_text").current + "(" +
                                                        (tbshop_Daily[sign].freeRule[0][1]
                                                         - (times - (tbshop_Daily[sign].freeRule[0][0] -
                                                                     tbshop_Daily[sign].freeRule[0][1]))).ToString()
                                                        + ")");
                                    }
                                    else
                                    {
                                        Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid)
                                            .GetTextMeshPro()
                                            .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                                                        tblanguage.Get("common_free_text").current);
                                    }
                                }

                                Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
                            }
                            else
                            {
                                Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                                    .SetTMPText(tblanguage.Get("shop_daily_gained_text").current);
                                Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                                Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        int sign = dailyBuy.Sign;
                        int times = dailyBuy.BuyCount;
                        //判断一下
                        if (times < tbshop_Daily[sign].times)
                        {
                            //RedPointMgr.instance.SetState("ShopRoot", "module1201", RedPointState.Show);
                            //ResourcesSingletonOld.Instance.UpdateResourceUI();
                            Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);
                            if (times < tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1])
                            {
                                //免费
                                if (tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1] - times > 1)
                                {
                                    Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid)
                                        .GetTextMeshPro()
                                        .SetTMPText(tblanguage.Get("common_free_text").current + "(" +
                                                    (tbshop_Daily[sign].freeRule[0][0] -
                                                     tbshop_Daily[sign].freeRule[0][1] -
                                                     times).ToString()
                                                    + ")");
                                }
                                else
                                {
                                    Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid)
                                        .GetTextMeshPro()
                                        .SetTMPText(tblanguage.Get("common_free_text").current);
                                }
                            }
                            else
                            {
                                //广告免费
                                Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                                if (tbshop_Daily[sign].freeRule[0][1] -
                                    (times - (tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1])) >
                                    1)
                                {
                                    Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid)
                                        .GetTextMeshPro()
                                        .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                                                    tblanguage.Get("common_free_text").current + "(" +
                                                    (tbshop_Daily[sign].freeRule[0][1]
                                                     - (times - (tbshop_Daily[sign].freeRule[0][0] -
                                                                 tbshop_Daily[sign].freeRule[0][1]))).ToString()
                                                    + ")");
                                }
                                else
                                {
                                    Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KText_Mid)
                                        .GetTextMeshPro()
                                        .SetTMPText(tblanguage.Get("common_free_text").current);
                                }
                            }

                            Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
                        }
                        else
                        {
                            Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                                .SetTMPText(tblanguage.Get("shop_daily_gained_text").current);
                            Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                            Module1201_Pos1UI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                        }
                    }
                }
            }
        }

        #endregion

        #region module1202

        private Dictionary<UI, int> Module1202_Help_SpUIToID = new Dictionary<UI, int>();

        /// <summary>
        /// 特惠 包括每日每周每月
        /// </summary>
        private void Module1202(bool isUpdate)
        {
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1202))
            {
                return;
            }
            //if (!ResourcesSingletonOld.Instance.unlockList.Contains(1202))
            //{
            //    return;
            //}


            Module1202_Help_SpUIToID.Clear();

            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            var bgUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_Special_Bg, false);

            ModuleUIAndIDDic.Add(bgUI, 1202);

            int dayCount = 0;
            int weekCount = 0;
            int monthCount = 0;

            foreach (var tsp in tbspecials.DataList)
            {
                if (tsp.type == 1)
                {
                    dayCount += 1;
                }

                if (tsp.type == 2)
                {
                    weekCount += 1;
                }

                if (tsp.type == 3)
                {
                    monthCount += 1;
                }
            }

            float dayH = 450 * (int)Math.Ceiling(dayCount / 3.0);
            float weekH = 450 * (int)Math.Ceiling(weekCount / 3.0);
            float monthH = 450 * (int)Math.Ceiling(monthCount / 3.0);

            float thisModuleH = 3 * 100 + 2 * 50 + dayH + weekH + monthH;
            bgUI.GetRectTransform().SetHeight(thisModuleH);
            var uiList = bgUI.GetFromReference(UISubPanel_Shop_Special_Bg.KBg).GetList();
            uiList.Clear();
            Module1202_Help_SpUIToID = new Dictionary<UI, int>();

            for (int i = 1; i <= 3; i++)
            {
                int ihelp = i;
                var ui = uiList.CreateWithUIType(UIType.UISubPanel_Shop_ModelBG, false);
                ui.GetFromReference(UISubPanel_Shop_ModelBG.KImg_NoGift).SetActive(false);
                ui.GetImage(UISubPanel_Shop_ModelBG.KPos_Layout).SetAlpha(0f);
                ui.GetFromReference(UISubPanel_Shop_ModelBG.KImageBg).SetActive(false);
                ui.GetFromReference(UISubPanel_Shop_ModelBG.KDescText).SetActive(true);
                Module1202_Help_SpUIToID.Add(ui, ihelp);
                if (i == 1)
                {
                    ui.GetRectTransform().SetHeight(dayH + 100);
                    ui.GetFromReference(UISubPanel_Shop_ModelBG.KTitleText).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("shop_specials_day").current);
                }

                if (i == 2)
                {
                    ui.GetRectTransform().SetHeight(weekH + 100);
                    ui.GetFromReference(UISubPanel_Shop_ModelBG.KTitleText).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("shop_specials_week").current);
                }

                if (i == 3)
                {
                    ui.GetRectTransform().SetHeight(monthH + 100);
                    ui.GetFromReference(UISubPanel_Shop_ModelBG.KTitleText).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("shop_specials_month").current);
                }


                var redDotStr = NodeNames.GetTagFuncRedDotName(1202);
                RedDotManager.Instance.InsterNode(redDotStr + i.ToString());

                var bgList = ui.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetList();
                bgList.Clear();
                ui.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.top =
                    13;
                ui.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.left =
                    0;
                ui.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().cellSize =
                    new Vector2(310, 425);
                ui.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().spacing =
                    new Vector2(70, 0);
                Module1202_Help_CreateItem(bgList, ihelp, isUpdate);
                UnicornUIHelper.ForceRefreshLayout(ui.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout));
            }

            Module1202_Help_SetTimeText();
            uiList.Sort(delegate(UI ui1, UI ui2)
            {
                return Module1202_Help_SpUIToID[ui1].CompareTo(Module1202_Help_SpUIToID[ui2]);
            });
            UnicornUIHelper.ForceRefreshLayout(bgUI.GetFromReference(UISubPanel_Shop_Special_Bg.KBg));

            ContentH += thisModuleH;
        }

        private void Module1202_Help_CreateItem(UIListComponent bgList, int type, bool isUpdate)
        {
            List<specials> specialList = new List<specials>();
            foreach (var tsp in tbspecials.DataList)
            {
                if (tsp.type == type)
                {
                    specialList.Add(tsp);
                }
            }

            specialList.Sort(delegate(specials s1, specials s2) { return s1.sort.CompareTo(s2.sort); });


            foreach (var sp in specialList)
            {
                var itemUI = bgList.CreateWithUIType<int3>(UIType.UISubPanel_Shop_item,
                    new int3(1202, sp.id, isUpdate ? 1 : 0), false);
                itemUI.GetImage(UISubPanel_Shop_item.KBg).SetAlpha(1);
                itemUI.GetFromReference(UISubPanel_Shop_item.KItemTitleTopBg).SetActive(false);
                if (sp.ratio == 0)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
                }
                else
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(true);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KTitleText).GetTextMeshPro()
                        .SetTMPText(sp.ratio.ToString() + "%" + tblanguage.Get("gift_ratio_text").current);
                }

                if (sp.type == 1)
                {
                    itemUI.GetImage(UISubPanel_Shop_item.KBg).SetSpriteAsync("specials_daily_item_container", false);
                }

                if (sp.type == 2)
                {
                    itemUI.GetImage(UISubPanel_Shop_item.KBg).SetSpriteAsync("specials_week_item_container", false);
                }

                if (sp.type == 3)
                {
                    itemUI.GetImage(UISubPanel_Shop_item.KBg).SetSpriteAsync("specials_month_item_container", false);
                }

                itemUI.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(false);
                itemUI.GetFromReference(UISubPanel_Shop_item.KItemImage).SetActive(false);

                itemUI.GetFromReference(UISubPanel_Shop_item.KGoldText).SetActive(false);
                itemUI.GetFromReference(UISubPanel_Shop_item.KItemDescText).SetActive(false);
                itemUI.GetFromReference(UISubPanel_Shop_item.KRewardText).SetActive(false);
                itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);
                itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(false);

                List<Vector3> vector3s = new List<Vector3>();
                vector3s = sp.reward;
                UnicornUIHelper.SortRewards(vector3s);

                if (sp.reward.Count == 1)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(false);
                    var reList = itemUI.GetFromReference(UISubPanel_Shop_item.KRewardItemPos).GetList();
                    reList.Clear();
                    var reItemUI = reList.CreateWithUIType<Vector3>(UIType.UICommon_RewardItem, vector3s[0], false);
                    reItemUI.GetFromReference(UICommon_RewardItem.KImg_Outer).SetActive(false);

                    SetRewardOnclickInShop(reItemUI, vector3s[0]);
                }

                if (sp.reward.Count == 2)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(true);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(false);
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[0],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Two_Left));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[1],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Two_Right));
                    SetRewardOnclickInShop(itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Two_Left), vector3s[0]);
                    SetRewardOnclickInShop(itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Two_Right),
                        vector3s[1]);
                }

                if (sp.reward.Count == 3)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(true);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(false);
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[0],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[1],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Left));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[2],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Right));
                    SetRewardOnclickInShop(itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up), vector3s[0]);
                    SetRewardOnclickInShop(itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Left), vector3s[1]);
                    SetRewardOnclickInShop(itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Right), vector3s[2]);
                }

                if (sp.reward.Count >= 4)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(true);
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[0],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up_Left));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[1],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up_Right));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[2],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Down_Left));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[3],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Down_Right));
                    SetRewardOnclickInShop(itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up_Left), vector3s[0]);
                    SetRewardOnclickInShop(itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up_Right), vector3s[1]);
                    SetRewardOnclickInShop(itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Down_Left),
                        vector3s[2]);
                    SetRewardOnclickInShop(itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Down_Right),
                        vector3s[3]);
                }

                var gsList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1202].GameSpecialsList;
                GameSpecials gameSpecials = new GameSpecials();
                foreach (var gs in gsList)
                {
                    if (gs.SpecialId == sp.id)
                    {
                        gameSpecials = gs;
                        break;
                    }
                }

                //is buy or not 


                itemUI.GetFromReference(UISubPanel_Shop_item.KImg_RedDot).SetActive(false);

                //没有免费次数
                if (sp.freeRule.Count == 0)
                {
                    //Debug.Log("gameSpecials.BuyCount" + gameSpecials.BuyCount);
                    if (gameSpecials.BuyCount == 1)
                    {
                        //can buy
                        itemUI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                            .SetTMPText(tbprice.Get(sp.price).rmb.ToString() +
                                        tblanguage.Get("common_coin_unit").current);
                    }
                    else
                    {
                        //can not buy
                        itemUI.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(false);
                        itemUI.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_yellow_1", false);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                            .SetTMPText(tblanguage.Get("common_state_purchased").current);
                    }
                }

                //保证是第一个
                else
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(true);
                    itemUI.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_green", false);


                    if (gameSpecials.FreeCount == 1)
                    {
                        var redDotStr = NodeNames.GetTagFuncRedDotName(1202);
                        RedDotManager.Instance.SetRedPointCnt(redDotStr + type.ToString(), 1);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);
                        //can buy
                        //RedPointMgr.instance.SetState(ShopRoot, "module1202" + sp.id.ToString(), RedPointState.Show);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                            .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                                        tblanguage.Get("common_free_text").current);
                    }
                    else
                    {
                        var redDotStr = NodeNames.GetTagFuncRedDotName(1202);
                        RedDotManager.Instance.SetRedPointCnt(redDotStr + type.ToString(), 0);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);

                        //can not buy
                        //RedPointMgr.instance.SetState(ShopRoot, "module1202" + sp.id.ToString(), RedPointState.Hide);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(false);
                        itemUI.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_yellow_1", false);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                            .SetTMPText(tblanguage.Get("common_state_purchased").current);
                    }
                }
            }

            bgList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Shop_item;
                var uib = b as UISubPanel_Shop_item;
                return uia.ID.CompareTo(uib.ID);
            });
        }

        private void SetRewardOnclickInShop(UI rewardUI, Vector3 rewardV3)
        {
            //if (rewardV3.x == 11)
            //{
            //    var btn = rewardUI.GetFromReference(UICommon_RewardItem.KBtn_Item);
            //    btn.GetXButton().OnClick.Add(() =>
            //    {
            //        MyGameEquip equip = new MyGameEquip()
            //        {
            //            equip = new GameEquip()
            //            {
            //                EquipId = (int)rewardV3.x,
            //                Quality = (int)rewardV3.y,
            //                Level = 1
            //            }
            //        };

            //        var tipUI = UIHelper.Create<MyGameEquip>(UIType.UICommon_EquipTips, equip);
            //        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle).SetActive(false);
            //        tipUI.GetFromReference(UICommon_EquipTips.KBottom).SetActive(false);
            //        tipUI.GetFromReference(UICommon_EquipTips.KBtn_Decrease).SetActive(false);

            //        var itemPos = UnicornUIHelper.GetUIPos(rewardUI);

            //        float tipMidH = tipUI.GetFromReference(UICommon_EquipTips.KMid).GetRectTransform().Height();
            //        float tipTopH = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform().Height();
            //        float tipTopHelp = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle).GetRectTransform().Height();
            //        float tipBottomH = tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).GetRectTransform().Height();
            //        float tipBottomHelp = tipUI.GetFromReference(UICommon_EquipTips.KBottom).GetRectTransform().Height();
            //        float screeenH = Screen.height;
            //        float itemH = rewardUI.GetRectTransform().Height();

            //        if (itemPos.y - tipTopH - tipMidH - itemH > -screeenH / 2)
            //        {
            //            //down
            //            tipUI.GetRectTransform().SetAnchoredPositionY(itemPos.y - tipMidH / 2 - tipTopH - itemH + tipTopHelp - tipTopH);
            //            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(true);
            //            tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(false);
            //            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform().SetAnchoredPositionX(itemPos.x);
            //        }
            //        else
            //        {
            //            //up
            //            tipUI.GetRectTransform().SetAnchoredPositionY(itemPos.y + tipMidH / 2 + tipBottomH - itemH - tipBottomHelp + tipBottomH);
            //            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(false);
            //            tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(true);
            //            tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).GetRectTransform().SetAnchoredPositionX(itemPos.x);
            //        }
            //    });
            //}
            //else
            //{
            //    UnicornUIHelper.SetRewardOnClick(rewardV3, rewardUI);
            //}

            UnicornUIHelper.SetRewardOnClick(rewardV3, rewardUI);
        }

        private void Module1202_Help_SetTimeText()
        {
            foreach (var a in Module1202_Help_SpUIToID)
            {
                long endTime = 0;
                var gsList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1202].GameSpecialsList;

                foreach (var sp in gsList)
                {
                    if (tbspecials.Get(sp.SpecialId).type == a.Value)
                    {
                        endTime = sp.NextBuyTime;
                    }
                }

                long deltaTime = endTime - TimeHelper.ClientNowSeconds();

                string timeStr = "";
                if (a.Value == 1)
                {
                    timeStr = UnicornUIHelper.GeneralTimeFormat(new int4(2, 3, 2, 1), deltaTime);
                }
                else
                {
                    timeStr = UnicornUIHelper.GeneralTimeFormat(new int4(3, 4, 2, 1), deltaTime);
                }

                a.Key.GetFromReference(UISubPanel_Shop_ModelBG.KDescText).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("text_countdown_text").current + timeStr);
            }
        }

        #endregion

        #region module1301

        private void Module1301(bool isUpdate)
        {
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1301))
            {
                return;
            }

            var allGiftList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1301].GiftInfoList;
            List<GiftInfo> thisModuleList = new List<GiftInfo>();
            foreach (var g in allGiftList)
            {
                if (tbgift.Get(g.GiftId).group == 100)
                {
                    thisModuleList.Add(g);
                }
            }

            if (thisModuleList.Count > 0)
            {
                var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
                shopViewList.Clear();
                var bgUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_ModelBG, false);
                bgUI.GetImage(UISubPanel_Shop_ModelBG.KPos_Layout).SetAlpha(0f);
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.top =
                    0;
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding
                    .left = 50;
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().cellSize =
                    new Vector2(310, 420);
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().spacing =
                    new Vector2(70, 15);
                ModuleUIAndIDDic.Add(bgUI, 1301);
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KImg_NoGift).SetActive(false);
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KTitleText).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(tbtag_Func.Get(1301).name).current);
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KDescText).SetActive(true);
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KDescText).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(tbtag_Func.Get(1301).desc).current);

                var bgList = bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KAllItemBg).GetList();
                bgList.Clear();
                var giftUI = bgList.CreateWithUIType(UIType.UISubPanel_Shop_1301_ChapterGift, false);
                float giftH = giftUI.GetRectTransform().Height();
                bgUI.GetRectTransform().SetHeight(giftH + 100);
                giftUI.GetRectTransform().SetAnchoredPositionY(0);

                ContentH += giftH + 100;
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KAllItemBg).GetRectTransform().SetAnchoredPositionY(-94f);
            }
            else
            {
                return;
            }
        }

        #endregion

        #region module1302

        private Dictionary<UI, int> Module1302_GiftUI_ID_DIC = new Dictionary<UI, int>();

        /// <summary>
        /// 限时礼包
        /// </summary>
        private void Module1302(bool isUpdate)
        {
            Module1302_GiftUI_ID_DIC.Clear();

            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            var bgUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_ModelBG, false);
            //bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KBG_Title).GetImage().SetSprite("modebgtitle_2", false);
            bgUI.GetImage(UISubPanel_Shop_ModelBG.KPos_Layout).SetAlpha(0f);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KImageBg).SetActive(false);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.top = 0;
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.left =
                50;
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().cellSize =
                new Vector2(310, 420);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().spacing =
                new Vector2(70, 15);
            ModuleUIAndIDDic.Add(bgUI, 1302);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KImg_NoGift).SetActive(false);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KText_NoGift).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("gift_limitedtime_blank_text").current);

            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KTitleText).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbtag_Func.Get(1302).name).current);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KDescText).SetActive(false);

            float thisModuleH = 100;

            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1302))
            {
                Debug.Log("no 1302");
                thisModuleH += 800;

                bgUI.GetRectTransform().SetHeight(thisModuleH);
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KImg_NoGift).SetActive(true);


                ContentH += thisModuleH;
                return;
            }

            var gList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList;
            int gcount = 0;
            foreach (var g in gList)
            {
                if (g.EndTime > TimeHelper.ClientNowSeconds())
                {
                    gcount++;
                }
            }

            if (gcount == 0)
            {
                thisModuleH += 800;

                bgUI.GetRectTransform().SetHeight(thisModuleH);
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KImg_NoGift).SetActive(true);


                ContentH += thisModuleH;
                return;
            }
            else
            {
                Dictionary<int, List<int>> groupIDDic = new Dictionary<int, List<int>>();
                foreach (var gg in tbgift_Group.DataList)
                {
                    groupIDDic.Add(gg.id, new List<int>());
                }

                foreach (var g in gList)
                {
                    if (g.EndTime > TimeHelper.ClientNowSeconds())
                    {
                        groupIDDic[tbgift.Get(g.GiftId).group].Add(g.GiftId);
                    }
                }

                List<int> giftGroupIDList = new List<int>();
                foreach (var g in groupIDDic)
                {
                    if (g.Value.Count > 0)
                    {
                        giftGroupIDList.Add(g.Key);
                    }

                    g.Value.Sort();
                }

                giftGroupIDList.Sort();

                var bgbgList = bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KAllItemBg).GetList();
                var realBg = bgbgList.CreateWithUIType(UIType.UISubPanel_Shop_Box_Bg, false);
                var bgList = realBg.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetList();

                float allGH = 0;

                foreach (var g in giftGroupIDList)
                {
                    int gCount = groupIDDic[g].Count;
                    float thisGH = 425 * (int)Math.Ceiling(gCount / 3.0);
                    thisGH += 240;

                    var giftui = bgList.CreateWithUIType(UIType.UISubPanel_Shop_1302_LimitGift, false);
                    giftui.GetFromReference(UISubPanel_Shop_1302_LimitGift.KPos_Item).GetRectTransform()
                        .SetAnchoredPositionY(-248f);
                    giftui.GetRectTransform().SetHeight(thisGH);
                    Module1302_GiftUI_ID_DIC.Add(giftui, groupIDDic[g][0]);
                    Module1302_Help_SetGift(giftui, g, groupIDDic[g], isUpdate).Forget();

                    allGH += thisGH + 45;
                }

                Module1302_SetTextByTime();
                realBg.GetRectTransform().SetHeight(allGH);
                realBg.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetComponent<VerticalLayoutGroup>().padding.top =
                    65;
                realBg.GetFromReference(UISubPanel_Shop_Box_Bg.KBg).GetComponent<VerticalLayoutGroup>().spacing = 50;
                UnicornUIHelper.ForceRefreshLayout(realBg.GetFromReference(UISubPanel_Shop_Box_Bg.KBg));
                thisModuleH += allGH;

                bgUI.GetRectTransform().SetHeight(thisModuleH);
                bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KAllItemBg).GetRectTransform().SetAnchoredPositionY(-94f);
            }

            ContentH += thisModuleH;
        }

        private async UniTaskVoid Module1302_Help_SetGift(UI limitGiftui, int groupID, List<int> idList, bool isUpdate)
        {
            //Debug.LogError("设置文字1111111111");
            limitGiftui.GetFromReference(UISubPanel_Shop_1302_LimitGift.KImg_Bg).GetImage()
                .SetSpriteAsync(tbgift_Group.Get(groupID).pic, false);
            limitGiftui.GetFromReference(UISubPanel_Shop_1302_LimitGift.KText_Title).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbgift_Group.Get(groupID).name).current);

            //Log.Debug("dddddddddddddddddddddddddd22222222", Color.cyan);
            var uiList = limitGiftui.GetFromReference(UISubPanel_Shop_1302_LimitGift.KPos_Item).GetList();
            uiList.Clear();
            foreach (var id in idList)
            {
                var itemUI =
                    await uiList.CreateWithUITypeAsync<int3>(UIType.UISubPanel_Shop_item,
                        new int3(1302, id, isUpdate ? 1 : 0), false);
                var thisGift = tbgift.Get(id);
                itemUI.GetImage(UISubPanel_Shop_item.KBg).SetAlpha(1);
                itemUI.GetFromReference(UISubPanel_Shop_item.KItemTitleTopBg).SetActive(false);
                if (thisGift.ratio == 0)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
                }
                else
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(true);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KTitleText).GetTextMeshPro()
                        .SetTMPText(thisGift.ratio.ToString() + "%" + tblanguage.Get("gift_ratio_text").current);
                }

                itemUI.GetImage(UISubPanel_Shop_item.KBg).SetSpriteAsync("specials_daily_item_container", false);

                itemUI.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(false);
                itemUI.GetFromReference(UISubPanel_Shop_item.KItemImage).SetActive(false);

                itemUI.GetFromReference(UISubPanel_Shop_item.KGoldText).SetActive(false);
                itemUI.GetFromReference(UISubPanel_Shop_item.KItemDescText).SetActive(false);
                itemUI.GetFromReference(UISubPanel_Shop_item.KRewardText).SetActive(false);
                itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);

                List<Vector3> vector3s = new List<Vector3>();
                vector3s = thisGift.reward;
                UnicornUIHelper.SortRewards(vector3s);


                if (thisGift.reward.Count == 1)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(false);
                    var reList = itemUI.GetFromReference(UISubPanel_Shop_item.KRewardItemPos).GetList();
                    reList.Clear();
                    var reUI = reList.CreateWithUIType<Vector3>(UIType.UICommon_RewardItem, vector3s[0], false);
                    reUI.GetFromReference(UICommon_RewardItem.KImg_Outer).SetActive(false);
                    UnicornUIHelper.SetRewardOnClick(vector3s[0], reUI);
                }

                if (thisGift.reward.Count == 2)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(true);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(false);
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[0],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Two_Left));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[1],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Two_Right));
                    UnicornUIHelper.SetRewardOnClick(vector3s[0],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Two_Left));
                    UnicornUIHelper.SetRewardOnClick(vector3s[1],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Two_Right));
                }

                if (thisGift.reward.Count == 3)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(true);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(false);
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[0],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[1],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Left));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[2],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Right));
                    UnicornUIHelper.SetRewardOnClick(vector3s[0],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up));
                    UnicornUIHelper.SetRewardOnClick(vector3s[1],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Left));
                    UnicornUIHelper.SetRewardOnClick(vector3s[2],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Right));
                }

                if (thisGift.reward.Count >= 4)
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(true);
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[0],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up_Left));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[1],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up_Right));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[2],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Down_Left));
                    UnicornUIHelper.SetRewardIconAndCountText(vector3s[3],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Down_Right));
                    UnicornUIHelper.SetRewardOnClick(vector3s[0],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up_Left));
                    UnicornUIHelper.SetRewardOnClick(vector3s[1],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Up_Right));
                    UnicornUIHelper.SetRewardOnClick(vector3s[2],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Down_Left));
                    UnicornUIHelper.SetRewardOnClick(vector3s[3],
                        itemUI.GetFromReference(UISubPanel_Shop_item.KReward_Down_Right));
                }

                var giList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList;
                GiftInfo giftInfo = new GiftInfo();

                foreach (var gi in giList)
                {
                    if (gi.GiftId == id)
                    {
                        giftInfo = gi;
                        break;
                    }
                }

                var itemNodeStr = NodeNames.GetTagFuncRedDotName(1302) + '|' + thisGift.id.ToString();
                RedDotManager.Instance.InsterNode(itemNodeStr);
                //is buy or not
                if (thisGift.freeRule.Count == 0)
                {
                    RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 0);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);

                    itemUI.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(true);
                    if (giftInfo.Times > 0)
                    {
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).SetActive(true);
                        string giftTimesStr = tblanguage.Get("gift_chances_left_text").current;
                        var timesStr = giftTimesStr.Replace("{0}", giftInfo.Times.ToString());
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).GetTextMeshPro()
                            .SetTMPText(timesStr);
                        //can buy
                        itemUI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro().SetTMPText(
                            tbprice.Get(thisGift.price).rmb.ToString() + tblanguage.Get("common_coin_unit").current);
                    }
                    else
                    {
                        //can not buy

                        itemUI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                            .SetTMPText(tblanguage.Get("common_state_purchased").current);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).SetActive(false);
                    }
                }
                else
                {
                    itemUI.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(true);
                    itemUI.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_green", false);
                    //RedPointMgr.instance.Init(ShopRoot, "module1302" + thisGift.id.ToString(),
                    //    (RedPointState state, int data) =>
                    //    {
                    //        if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui))
                    //        {
                    //            if (itemUI != null)
                    //            {
                    //                if (state == RedPointState.Show)
                    //                {
                    //                    itemUI.GetFromReference(UISubPanel_Shop_item.KImg_RedPoint).SetActive(true);
                    //                }
                    //                else
                    //                {
                    //                    itemUI.GetFromReference(UISubPanel_Shop_item.KImg_RedPoint).SetActive(false);
                    //                }
                    //            }
                    //        }
                    //    });

                    itemUI.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
                    itemUI.GetFromReference(UISubPanel_Shop_item.KItemTitleTopBg).SetActive(false);
                    if (giftInfo.FreeTimes > 0)
                    {
                        RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 1);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);

                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).SetActive(true);
                        string giftTimesStr = tblanguage.Get("gift_chances_left_text").current;
                        var timesStr = giftTimesStr.Replace("{0}", giftInfo.FreeTimes.ToString());
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).GetTextMeshPro()
                            .SetTMPText(timesStr);
                        //can buy
                        if (thisGift.freeRule[0][0] - giftInfo.FreeTimes <
                            thisGift.freeRule[0][0] - thisGift.freeRule[0][1])
                        {
                            //free
                            itemUI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);

                            itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                            //itemUI.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage().SetSprite("Advert", false);
                            itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                .SetTMPText(tblanguage.Get("common_free_text").current);
                        }
                        else
                        {
                            //advert
                            itemUI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);

                            itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                            itemUI.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                                .SetTMPText(UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                                            tblanguage.Get("common_free_text").current);
                        }
                    }
                    else
                    {
                        RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 0);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);
                        //can not buy
                        itemUI.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
                            .SetTMPText(tblanguage.Get("common_state_purchased").current);
                        itemUI.GetFromReference(UISubPanel_Shop_item.KText_RemainingTimes).SetActive(false);
                    }
                }
            }

            //Log.Debug("dddddddddddddddddddddddddd3333", Color.cyan);
            UnicornUIHelper.ForceRefreshLayout(limitGiftui.GetFromReference(UISubPanel_Shop_1302_LimitGift.KPos_Item));
        }

        private void Module1302_SetTextByTime()
        {
            //Debug.LogError("设置时间");
            foreach (var giid in Module1302_GiftUI_ID_DIC)
            {
                var giList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1302].GiftInfoList;
                long endTime = 0;
                foreach (var gi in giList)
                {
                    if (gi.GiftId == giid.Value)
                    {
                        endTime = gi.EndTime;
                        break;
                    }
                }

                long deltaTime = endTime - TimeHelper.ClientNowSeconds();

                if (deltaTime < 0)
                {
                    UpDateNowInterface();
                    break;
                }

                string timeStr = "";

                timeStr = UnicornUIHelper.GeneralTimeFormat(new int4(2, 3, 2, 1), deltaTime);
                //Log.Debug("dddddddddddddddddddddddddd4444444", Color.cyan);
                giid.Key.GetFromReference(UISubPanel_Shop_1302_LimitGift.KText_Countdown).GetTextMeshPro()
                    .SetTMPText(timeStr);
            }
        }

        #endregion

        #region module1401

        private void Module1401(bool isUpdate)
        {
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1401))
            {
                return;
            }

            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            var bgUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_ModelBG, false);
            ModuleUIAndIDDic.Add(bgUI, 1401);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KImg_NoGift).SetActive(false);

            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KTitleText).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbtag_Func.Get(1401).name).current);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KDescText).SetActive(false);
            bgUI.GetImage(UISubPanel_Shop_ModelBG.KPos_Layout).SetAlpha(1f);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.top = 30;
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.left =
                0;
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().cellSize =
                new Vector2(330, 415);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().spacing =
                new Vector2(28, 15);
            List<recharge> rechargeList = new List<recharge>();
            foreach (var recharge in tbrecharge.DataList)
            {
                if (recharge.tagFunc == 1401)
                {
                    rechargeList.Add(recharge);
                }
            }

            rechargeList.Sort(delegate(recharge rc1, recharge rc2) { return rc1.sort.CompareTo(rc2.sort); });
            float thisModuleH = (rechargeList.Count / 3) * 420 + (rechargeList.Count / 3) * 30 + 100;

            bgUI.GetRectTransform().SetHeight(thisModuleH);

            var uiList = bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetList();
            uiList.Clear();

            foreach (var rc in rechargeList)
            {
                Module1401_Help_CreateItem(uiList, rc, isUpdate);
            }

            UnicornUIHelper.ForceRefreshLayout(bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout));

            ContentH += thisModuleH;
        }

        private void Module1401_Help_CreateItem(UIListComponent uiList, recharge rc, bool isUpdate)
        {
            Log.Debug($"rcid:{rc.id}");
            var ui = uiList.CreateWithUIType<int3>(UIType.UISubPanel_Shop_item, new int3(1401, rc.id, isUpdate ? 1 : 0),
                false);

            ui.GetImage(UISubPanel_Shop_item.KBg).SetAlpha(0f);
            ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KItemTitleTopBg).SetActive(true);
            ui.GetFromReference(UISubPanel_Shop_item.KItemImagePos).SetActive(true);
            ui.GetFromReference(UISubPanel_Shop_item.KItemImagePos).GetRectTransform().SetAnchoredPositionY(-75f);
            ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
            ui.GetFromReference(UISubPanel_Shop_item.KGoldText).SetActive(true);
            ui.GetFromReference(UISubPanel_Shop_item.KItemImage).SetActive(true);

            ui.GetFromReference(UISubPanel_Shop_item.KItemDescText).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KRewardText).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(false);
            ui.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_blue_1", false);
            var clist = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1401].ChargeInfoList;
            ChargeInfo chargeInfo = new ChargeInfo();
            foreach (var c in clist)
            {
                if (c.Id == rc.id)
                {
                    chargeInfo = c;
                    break;
                }
            }

            if (chargeInfo.ChargeSum == 0)
            {
                //if is first time
                ui.GetFromReference(UISubPanel_Shop_item.KTitleTextTop).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("recharge_double_text").current);
            }
            else
            {
                ui.GetFromReference(UISubPanel_Shop_item.KTitleTextTop).GetTextMeshPro().SetTMPText(tblanguage
                    .Get("recharge_extra_text").current.Replace("{0}", rc.valueExtra.ToString()));
            }

            ui.GetFromReference(UISubPanel_Shop_item.KGoldNumTxt).GetTextMeshPro().SetTMPText(rc.value.ToString());
            ui.GetFromReference(UISubPanel_Shop_item.KGoldDescTxt).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(rc.name).current);
            ui.GetFromReference(UISubPanel_Shop_item.KItemImage).GetImage().SetSprite(rc.pic, true);
            ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
            ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro()
                .SetTMPText(tbprice.Get(rc.price).rmb.ToString() + tblanguage.Get("common_coin_unit").current);
            var thisBuyBtn = ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common);
            IntValue intValue = new IntValue();
            intValue.Value = rc.id;
            //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(thisBuyBtn, () =>
            //{
            //    //if (ResourcesSingletonOld.Instance.shopInit.shopHelpDic[1401][recharge.id][0] == 0)
            //    //{
            //    //    m1401IntHelp = recharge.value * 2;
            //    //}
            //    //else
            //    //{
            //    //    m1401IntHelp = recharge.value + recharge.valueExtra;
            //    //}

            //    //WebMessageHandlerOld.Instance.AddHandler(11, 4, On1401BuyResponse);
            //    //NetWorkManager.Instance.SendMessage(11, 4, intValue);
            //});
            UnicornTweenHelper.PlayUIImageSweepFX(ui.GetFromReference(UISubPanel_Shop_item.KImg_Btn),
                cancellationToken: cts.Token);
        }

        #endregion

        #region module1402

        private void Module1402(bool isUpdate)
        {
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1402))
            {
                return;
            }

            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();
            var bgUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_ModelBG, false);
            bgUI.GetImage(UISubPanel_Shop_ModelBG.KPos_Layout).SetAlpha(0f);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.top = 10;
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().padding.left =
                0;
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().cellSize =
                new Vector2(310, 415);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetComponent<GridLayoutGroup>().spacing =
                new Vector2(72, 15);

            ModuleUIAndIDDic.Add(bgUI, 1402);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KImg_NoGift).SetActive(false);

            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KTitleText).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbtag_Func.Get(1402).name).current);
            bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KDescText).SetActive(false);

            List<recharge> rechargeList = new List<recharge>();
            foreach (var recharge in tbrecharge.DataList)
            {
                if (recharge.tagFunc == 1402)
                {
                    rechargeList.Add(recharge);
                }
            }

            rechargeList.Sort(delegate(recharge rc1, recharge rc2) { return rc1.sort.CompareTo(rc2.sort); });
            float thisModuleH = (rechargeList.Count / 3) * 420 + (rechargeList.Count / 3 - 1) * 30 + 100 + 30;

            bgUI.GetRectTransform().SetHeight(thisModuleH);

            var uiList = bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout).GetList();
            uiList.Clear();

            foreach (var rc in rechargeList)
            {
                Module1402_Help_CreateItem(uiList, rc, isUpdate);
            }

            UnicornUIHelper.ForceRefreshLayout(bgUI.GetFromReference(UISubPanel_Shop_ModelBG.KPos_Layout));


            ContentH += thisModuleH;
        }

        private void Module1402_Help_CreateItem(UIListComponent uiList, recharge rc, bool isUpdate)
        {
            Log.Debug($"id:{rc.id}");
            var ui = uiList.CreateWithUIType<int3>(UIType.UISubPanel_Shop_item, new int3(1402, rc.id, isUpdate ? 1 : 0),
                false);
            ui.GetImage(UISubPanel_Shop_item.KBg).SetSpriteAsync("shop_daily_item_container", false);
            ui.GetImage(UISubPanel_Shop_item.KBg).SetAlpha(255f);
            var clist = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1402].ChargeInfoList;
            ChargeInfo chargeInfo = new ChargeInfo();
            foreach (var c in clist)
            {
                if (c.Id == rc.id)
                {
                    chargeInfo = c;
                    break;
                }
            }

            var itemNodeStr = NodeNames.GetTagFuncRedDotName(1402) + '|' + rc.id.ToString();
            RedDotManager.Instance.InsterNode(itemNodeStr);
            //RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 1);
            ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);


            ui.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KItemTitleTopBg).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KItemImagePos).SetActive(true);
            ui.GetFromReference(UISubPanel_Shop_item.KItemImagePos).GetRectTransform().SetAnchoredPositionY(-71f);
            ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
            ui.GetFromReference(UISubPanel_Shop_item.KGoldText).SetActive(true);
            ui.GetFromReference(UISubPanel_Shop_item.KItemImage).SetActive(true);

            ui.GetFromReference(UISubPanel_Shop_item.KItemDescText).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KRewardText).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KPos_Two_Re).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KPos_Three_Re).SetActive(false);
            ui.GetFromReference(UISubPanel_Shop_item.KPos_Four_Re).SetActive(false);

            int moneyHelp = ResourcesSingletonOld.Instance.levelInfo.maxPassChapterID;
            //Debug.Log(moneyHelp);
            if (moneyHelp == 0)
            {
                moneyHelp = tbchapter[1].money;
            }
            else
            {
                foreach (var chapter in tbchapter.DataList)
                {
                    if (moneyHelp == chapter.id)
                    {
                        moneyHelp = chapter.money;
                        break;
                    }
                }
            }

            moneyHelp = moneyHelp * 6 * rc.value;
            moneyHelp = (moneyHelp * (ResourcesSingletonOld.Instance.UserInfo.PatrolGainName + 100)) / 100;
            ui.GetFromReference(UISubPanel_Shop_item.KGoldNumTxt).GetTextMeshPro().SetTMPText(moneyHelp.ToString());
            ui.GetFromReference(UISubPanel_Shop_item.KGoldNumTxt).GetRectTransform().SetAnchoredPositionY(0f);
            ui.GetFromReference(UISubPanel_Shop_item.KGoldDescTxt).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(rc.name).current.Replace("{0}", rc.value.ToString()));

            ui.GetFromReference(UISubPanel_Shop_item.KItemImage).GetImage().SetSpriteAsync(rc.pic, true);

            if (rc.unit == 2)
            {
                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro().SetTMPText(
                    UnicornUIHelper.GetRewardTextIconName(tbuser_Variable.Get(2).icon) + rc.price.ToString());
            }

            //if (rc.freeRule.Count > 0)
            //{

            //}

            //免费次数大于0
            if (chargeInfo.FreeSum > 0)
            {
                ui.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(true);
                ui.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_green", false);

                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro().SetTMPText(
                    tblanguage.Get("common_free_text").current + "(" + chargeInfo.FreeSum.ToString() + ")");


                RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 1);
                ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);

                if (rc.freeRule.Count > 0)
                {
                    //RedPointMgr.instance.SetState("ShopRoot", "module1402", RedPointState.Show);
                }
            }
            else if (chargeInfo.AdSum > 0)
            {
                RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 1);
                ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(true);

                ui.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(true);
                ui.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_green", false);

                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).SetActive(true);
                ui.GetFromReference(UISubPanel_Shop_item.KText_Mid).GetTextMeshPro().SetTMPText(
                    UnicornUIHelper.GetRewardTextIconName("icon_advertise") +
                    tblanguage.Get("common_free_text").current + "(" + chargeInfo.AdSum.ToString() + ")");

                if (rc.freeRule.Count > 0)
                {
                    //RedPointMgr.instance.SetState("ShopRoot", "module1402", RedPointState.Show);
                }
            }
            else
            {
                RedDotManager.Instance.SetRedPointCnt(itemNodeStr, 0);
                ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot)?.SetActive(false);

                ui.GetFromReference(UISubPanel_Shop_item.KImg_Green).SetActive(false);
                ui.GetImage(UISubPanel_Shop_item.KImg_Btn).SetSpriteAsync("icon_btn_yellow_1", false);
                if (rc.freeRule.Count > 0)
                {
                    //RedPointMgr.instance.SetState("ShopRoot", "module1402", RedPointState.Hide);
                }
            }

            var thisBuyBtn = ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common);
            IntValue intValue = new IntValue();
            intValue.Value = rc.id;

            //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(thisBuyBtn, () =>
            //{
            //    //m1402IntHelp = moneyHelp;
            //    //m1402Unit = recharge.unit;
            //    //m1402PriceHelp = recharge.price;
            //    //m1402IdHelp = recharge.id;
            //    //m1402UI = ui;
            //    //Recharge = recharge;

            //    //WebMessageHandlerOld.Instance.AddHandler(11, 4, On1402Response);
            //    //NetWorkManager.Instance.SendMessage(11, 4, intValue);
            //});
        }

        #endregion

        #region module1403

        private int Now1403FundIndex = -1;

        //private long Module1403_Last_Time = 0;

        private float Module1403_Last_ListH = 0;

        private List<UI> Module1403GetUIList = new List<UI>();

        private List<UI> Module1403_PointUIList = new List<UI>();

        private List<int> Module1403_NewUIIndexList = new List<int>();

        private bool Module1403_IsNew = false;

        private int Module1403_NowNewUIIndexListIndex = 0;

        private List<UI> Module1403_TopTab_NewUI = new List<UI>();

        private void Module1403(bool isUpdate)
        {
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1403))
            {
                return;
            }

            var a = Module1403_Help_FindFirstFund();
            //Debug.Log("a.x" + a.x);
            //Debug.Log("a.y" + a.y);
            Module1403_PointUIList.Clear();
            Module1403_NewUIIndexList.Clear();
            //Module1403_Last_Time = TimeHelper.ClientNow();
            LastFundPoint = null;

            #region Determine if it is necessary to jump to another fund (like fund 2, fund 3)

            int InitFundIndex = 0;
            if (Now1403FundIndex < 0)
            {
            }
            else
            {
                InitFundIndex = Now1403FundIndex;
            }
            //InitFundIndex = 1;
            //a.x = InitFundIndex + 1;

            #endregion

            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();

            #region top select

            Module1403_Last_ListH = 0;
            var fundUI = shopViewList.CreateWithUIType<UI>(UIType.UISubPanel_Shop_1403_Fund, this, false);
            fundUI.GetRectTransform().SetWidth(Screen.width);
            ModuleUIAndIDDic.Add(fundUI, 1403);
            //fundUI.SetParent(this, false);

            return;

            //float showW = Screen.width - 90;

            //if (showW <= 0)
            //{
            //    return;
            //}

            //int fundNum = 0;
            //int level = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Level;
            //foreach (var tbf in tbfund.DataList)
            //{
            //    if (level >= tbf.unlockLevel)
            //    {
            //        fundNum++;
            //    }
            //}
            ////fundNum = 3;

            ////this.GetFromReference(KScrollView).GetScrollRect().OnValueChanged.Add((f) =>
            ////{
            ////    Module1403_Last_Time = TimeHelper.ClientNow();
            ////});

            ////attenton!!!!!!: index != id
            //int nowFundIndex = 0;

            //fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform().SetWidth(showW * fundNum);
            //fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform().SetOffsetWithTop(0);
            //fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform().SetOffsetWithBottom(0);
            //fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetComponent<GridLayoutGroup>().cellSize =
            //    new(showW, 210);
            //var fundTitleList = fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetList();

            //var pointList = fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KPoint_List).GetList();
            //List<UI> fundPointList = new List<UI>();
            //string pointSelectedColor = "FFFFFF";
            //string pointNotSelectedColor = "94A3B8";

            //var fundForwardSortList = new List<fund>();
            //foreach (var tbf in tbfund.DataList)
            //{
            //    fundForwardSortList.Add(tbf);
            //}

            //fundForwardSortList.Sort(delegate(fund f1, fund f2) { return f1.unlockLevel.CompareTo(f2.unlockLevel); });

            //Dictionary<int, int> indexIDDic = new Dictionary<int, int>();
            //for (int i = 0; i < fundForwardSortList.Count; i++)
            //{
            //    indexIDDic.Add(i, fundForwardSortList[i].id);
            //}

            //var fundReverseSortList = new List<fund>();
            //foreach (var tbf in tbfund.DataList)
            //{
            //    fundReverseSortList.Add(tbf);
            //}

            //fundReverseSortList.Sort(delegate(fund f1, fund f2) { return f2.unlockLevel.CompareTo(f1.unlockLevel); });

            //if (fundNum <= 1)
            //{
            //    //no point
            //    fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KScrollView).GetComponent<ScrollRect>().movementType =
            //        ScrollRect.MovementType.Clamped;
            //}
            //else
            //{
            //    fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KScrollView).GetComponent<ScrollRect>().movementType =
            //        ScrollRect.MovementType.Clamped;
            //    for (int i = 0; i < fundReverseSortList.Count; i++)
            //    {
            //        int ihelp = i;
            //        if (fundReverseSortList.Count - ihelp <= fundNum)
            //        {
            //            var fundPoint = pointList.CreateWithUIType(UIType.UISubPanel_Shop_Fund_Point, false);
            //            Module1403_PointUIList.Add(fundPoint);
            //            fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KText_New).GetTextMeshPro()
            //                .SetTMPText(tblanguage.Get("text_new_1").current);

            //            //Determine if there are red dots
            //            bool haveRedDotOrNot = false;
            //            foreach (var gf in ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList)
            //            {
            //                if (gf.FoundId == fundReverseSortList[ihelp].id)
            //                {
            //                    if (gf.IsLook == 0)
            //                    {
            //                        if (gf.FoundId == a.x)
            //                        {
            //                            haveRedDotOrNot = false;
            //                        }
            //                        else
            //                        {
            //                            haveRedDotOrNot = true;
            //                        }
            //                    }
            //                    else if (gf.IsLook == 1)
            //                    {
            //                        haveRedDotOrNot = false;
            //                    }

            //                    break;
            //                }
            //            }

            //            //no red point, or determine red point
            //            if (haveRedDotOrNot)
            //            {
            //                //have red dot
            //                fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip).SetActive(true);
            //                //fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip).SetActive(false);
            //                int indexHelp = fundReverseSortList.Count - 1 - ihelp;
            //                Module1403_NewUIIndexList.Add(indexHelp);
            //            }
            //            else
            //            {
            //                //no red dot
            //                fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip).SetActive(false);
            //            }

            //            if (fundReverseSortList[ihelp].id == indexIDDic[InitFundIndex])
            //            {
            //                fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KImg_Circle).GetImage()
            //                    .SetColor(HexToColor(pointSelectedColor));
            //                LastFundPoint = fundPoint;
            //            }
            //            else
            //            {
            //                fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KImg_Circle).GetImage()
            //                    .SetColor(HexToColor(pointNotSelectedColor));
            //            }

            //            fundPointList.Add(fundPoint);
            //        }
            //    }

            //    if (Module1403_NewUIIndexList.Count > 1)
            //    {
            //        for (int i = 0; i < Module1403_NewUIIndexList.Count; i++)
            //        {
            //            if (i == 0)
            //            {
            //                LastFundPoint = Module1403_PointUIList[Module1403_NewUIIndexList[i]];
            //                Module1403_NowNewUIIndexListIndex = 0;
            //            }
            //            else
            //            {
            //                //Module1403_PointUIList[Module1403_NewUIIndexList[i]].GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip).SetActive(false);
            //            }
            //        }
            //    }

            //    UnicornUIHelper.ForceRefreshLayout(fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KPoint_List));
            //}

            //fundPointList.Reverse();

            //var fundTopScroll = fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KScrollView).GetXScrollRect();
            //fundTopScroll.OnEndDrag.Add(() =>
            //{
            //    //Module1403_Last_Time = TimeHelper.ClientNow();
            //    float contentX = fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform()
            //        .AnchoredPosition().x;
            //    contentX = math.abs(contentX);
            //    int posHelpIndex = (int)(contentX / showW);
            //    if (contentX > posHelpIndex * showW + showW / 2)
            //    {
            //        posHelpIndex += 1;
            //    }

            //    nowFundIndex = posHelpIndex;
            //    Module1403_Help_DoTabMove(fundNum, fundUI, nowFundIndex, indexIDDic, showW);
            //    Module1403_Help_ChangePointState(fundPointList);
            //    Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex, fundUI);
            //});

            //for (int i = 0; i < fundNum; i++)
            //{
            //    int ihelp = i;

            //    var fundSelectUI = fundTitleList.CreateWithUIType(UIType.UISubPanel_Shop_Fund_Select_Tab, false);
            //    fundSelectUI.GetRectTransform().SetWidth(showW);

            //    fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KText_Desc).GetTextMeshPro()
            //        .SetTMPText(tblanguage.Get(fundForwardSortList[ihelp].desc).current);
            //    fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KText_Prompt).GetTextMeshPro()
            //        .SetTMPText(tblanguage.Get("fund_desc_tips").current);
            //    fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImage).GetImage()
            //        .SetSpriteAsync(fundForwardSortList[ihelp].bg, false);
            //    fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip).SetActive(true);

            //    Debug.Log("fundForwardSortList[ihelp].id" + fundForwardSortList[ihelp].id);
            //    Debug.Log("indexIDDic[InitFundIndex]" + indexIDDic[InitFundIndex]);

            //    if (fundForwardSortList[ihelp].id == indexIDDic[InitFundIndex])
            //    {
            //        nowFundIndex = ihelp;
            //        Module1403_Help_DoTabMove(fundNum, fundUI, nowFundIndex, indexIDDic, showW, true);


            //        bool haveRedDotOrNot = false;
            //        foreach (var gf in ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList)
            //        {
            //            if (gf.FoundId == fundForwardSortList[ihelp].id)
            //            {
            //                Debug.Log("gf.id:" + gf.FoundId + "; gf.islook:" + gf.IsLook +
            //                          "; fundForwardSortList[ihelp].id:" + fundForwardSortList[ihelp].id);
            //                if (gf.IsLook == 0)
            //                {
            //                    //if (gf.FoundId == a.x)
            //                    //{
            //                    //    haveRedDotOrNot = false;
            //                    //}
            //                    //else
            //                    //{
            //                    //    haveRedDotOrNot = true;
            //                    //}
            //                    haveRedDotOrNot = true;

            //                    Module1403_Help_RemoveNewByID(gf.FoundId);
            //                    //ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList[fundListIndex].IsLook = 1;
            //                }
            //                else if (gf.IsLook == 1)
            //                {
            //                    haveRedDotOrNot = false;
            //                }

            //                break;
            //            }
            //        }

            //        Debug.Log("haveRedDotOrNot" + haveRedDotOrNot);

            //        if (haveRedDotOrNot)
            //        {
            //            fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip).SetActive(true);

            //            Debug.Log("set img tip true" + InitFundIndex);
            //        }
            //        else
            //        {
            //            fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip).SetActive(false);
            //        }
            //    }
            //    else
            //    {
            //        fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip).SetActive(false);
            //    }

            //    Module1403_TopTab_NewUI.Add(fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip));
            //}

            //UnicornUIHelper.ForceRefreshLayout(fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KContent));

            //Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex, fundUI);

            //var leftBtn = fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left);
            //var RightBtn = fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right);

            //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(leftBtn, () =>
            //{
            //    //Module1403_Last_Time = TimeHelper.ClientNow();
            //    nowFundIndex -= 1;
            //    Module1403_Help_DoTabMove(fundNum, fundUI, nowFundIndex, indexIDDic, showW);
            //    Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex, fundUI);
            //    Module1403_Help_ChangePointState(fundPointList);
            //});
            //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(RightBtn, () =>
            //{
            //    //Module1403_Last_Time = TimeHelper.ClientNow();
            //    nowFundIndex += 1;
            //    Module1403_Help_DoTabMove(fundNum, fundUI, nowFundIndex, indexIDDic, showW);
            //    Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex, fundUI);
            //    Module1403_Help_ChangePointState(fundPointList);
            //});

            //#endregion

            //#region item list

            ////Module1403_Help_CreateItem(InitFundID, fundUI);

            #endregion

            //ContentH += 210;
        }

        // private void Module1403_Help_LeftAndRightBtnSetActive(int fundNum, int nowFundIndex, UI fundUI)
        // {
        //     if (fundNum > 1)
        //     {
        //         if (nowFundIndex == 0)
        //         {
        //             fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right).SetActive(true);
        //             fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left).SetActive(false);
        //         }
        //         else if (nowFundIndex == fundNum - 1)
        //         {
        //             fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right).SetActive(false);
        //             fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left).SetActive(true);
        //         }
        //         else
        //         {
        //             fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right).SetActive(true);
        //             fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left).SetActive(true);
        //         }
        //     }
        //     else
        //     {
        //         fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right).SetActive(false);
        //         fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left).SetActive(false);
        //     }
        // }

        /// <summary>
        /// 弃用
        /// </summary>
        /// <param name="fundNum"></param>
        /// <param name="fundUI"></param>
        /// <param name="nowFundIndex"></param>
        /// <param name="indexIDDic"></param>
        /// <param name="showW"></param>
        /// <param name="isFirst"></param>
        // private void Module1403_Help_DoTabMove(int fundNum, UI fundUI, int nowFundIndex,
        //     Dictionary<int, int> indexIDDic, float showW, bool isFirst = false)
        // {
        //     fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform()
        //         .DoAnchoredPositionX(-nowFundIndex * showW, 0.2f);
        //     Module1403_Help_CreateItem(indexIDDic[nowFundIndex], fundUI, fundNum);
        //     Now1403FundIndex = nowFundIndex;
        //     if (isFirst)
        //     {
        //     }
        //     else
        //     {
        //         Module1403_IsNew = false;
        //         foreach (var newui in Module1403_TopTab_NewUI)
        //         {
        //             newui.SetActive(false);
        //         }
        //     }
        // }

        // private void Module1403_Help_ChangePointState(List<UI> fundPointList)
        // {
        //     string pointSelectedColor = "FFFFFF";
        //     string pointNotSelectedColor = "94A3B8";
        //     if (fundPointList.Count > 0)
        //     {
        //         if (LastFundPoint != fundPointList[Now1403FundIndex])
        //         {
        //             LastFundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KImg_Circle).GetImage()
        //                 .SetColor(HexToColor(pointNotSelectedColor));
        //             fundPointList[Now1403FundIndex].GetFromReference(UISubPanel_Shop_Fund_Point.KImg_Circle).GetImage()
        //                 .SetColor(HexToColor(pointSelectedColor));
        //             LastFundPoint = fundPointList[Now1403FundIndex];
        //         }
        //     }
        // }

        // private void Module1403_Help_CreateItem(int id, UI fundUI, int fundNum)
        // {
        //     Debug.LogError($"Module1403_Help_CreateItem");
        //     Module1403GetUIList.Clear();
        //     GameFoundation thisFundByID = null;
        //     var fundList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList;
        //     int fundListIndex = 0;
        //     foreach (var f in fundList)
        //     {
        //         if (f.FoundId == id)
        //         {
        //             thisFundByID = f;
        //             break;
        //         }
        //
        //         fundListIndex++;
        //     }
        //
        //     if (cts != null)
        //     {
        //         cts.Cancel();
        //     }
        //
        //     cts = new CancellationTokenSource();
        //     var fundUIItemList = fundUI.GetFromReference(UISubPanel_Shop_1403_Fund.KPos_Help_List).GetList();
        //     fundUIItemList.Clear();
        //     var shopFundListUI = fundUIItemList.CreateWithUIType(UIType.UISubPanel_Shop_Fund_List, false);
        //
        //     //if (thisFundByID.IsLook == 0)
        //     //{
        //     //    Module1403_Help_RemoveNewByID(id);
        //     //    ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList[fundListIndex].IsLook = 1;
        //     //}
        //
        //     #region set fund bg
        //
        //     //left
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Left).SetActive(false);
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Left).SetActive(true);
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Left).GetTextMeshPro()
        //         .SetTMPText(tblanguage.Get("common_free_text").current);
        //
        //     //mid
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Mid).GetTextMeshPro()
        //         .SetTMPText(tblanguage.Get("common_state_purchased").current);
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Buy_Mid).GetTextMeshPro()
        //         .SetTMPText(tblanguage.Get("common_state_buy").current);
        //     var textPriceMid = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Price_Mid);
        //     textPriceMid.GetTextMeshPro().SetTMPText(tbprice.Get(tbfund.Get(id).price1).rmb.ToString() +
        //                                              tblanguage.Get("common_coin_unit").current);
        //     if (thisFundByID.RewardTwoStatus == 0)
        //     {
        //         shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Mid).SetActive(true);
        //         shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Mid).SetActive(false);
        //         var btnBuyMid = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Mid);
        //         UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btnBuyMid, () =>
        //         {
        //             Debug.LogError($"Module1403_Help_CreateItemC03");
        //             const string Fund1 = "C03";
        //             var shopStr = UnicornUIHelper.GetShopStr(Fund1, id);
        //             
        //             NetWorkManager.Instance.SendMessage(CMDOld.PREPAY, new StringValue
        //             {
        //                 Value = shopStr
        //             });
        //             /*
        //             //Module1403_Last_Time = TimeHelper.ClientNow();
        //             WebMessageHandlerOld.Instance.AddHandler(11, 11, On1403BuyFundResponse);
        //             string buyFundStr = id.ToString() + ";2";
        //             StringValue stringValue = new StringValue();
        //             stringValue.Value = buyFundStr;
        //             NetWorkManager.Instance.SendMessage(11, 11, stringValue);*/
        //         });
        //     }
        //     else
        //     {
        //         shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Mid).SetActive(false);
        //         shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Mid).SetActive(true);
        //     }
        //
        //     //right
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Right).GetTextMeshPro()
        //         .SetTMPText(tblanguage.Get("common_state_purchased").current);
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Buy_Right).GetTextMeshPro()
        //         .SetTMPText(tblanguage.Get("common_state_buy").current);
        //     var textPriceRight = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Price_Right);
        //     textPriceRight.GetTextMeshPro().SetTMPText(tbprice.Get(tbfund.Get(id).price2).rmb.ToString() +
        //                                                tblanguage.Get("common_coin_unit").current);
        //     if (thisFundByID.RewardThreeStatus == 0)
        //     {
        //         shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Right).SetActive(true);
        //         shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Right).SetActive(false);
        //         var btnBuyRight = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Right);
        //         UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btnBuyRight, () =>
        //         {
        //             const string Fund1 = "C04";
        //             var shopStr = UnicornUIHelper.GetShopStr(Fund1, id);
        //             
        //             NetWorkManager.Instance.SendMessage(CMDOld.PREPAY, new StringValue
        //             {
        //                 Value = shopStr
        //             });
        //             /*
        //             //Module1403_Last_Time = TimeHelper.ClientNow();
        //             WebMessageHandlerOld.Instance.AddHandler(11, 11, On1403BuyFundResponse);
        //             string buyFundStr = id.ToString() + ";3";
        //             StringValue stringValue = new StringValue();
        //             stringValue.Value = buyFundStr;
        //             NetWorkManager.Instance.SendMessage(11, 11, stringValue);*/
        //         });
        //     }
        //     else
        //     {
        //         shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Right).SetActive(false);
        //         shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Right).SetActive(true);
        //     }
        //
        //     #endregion
        //
        //     var thisFundRewardList = new List<fund_reward>();
        //     foreach (var tbfr in tbfund_Reward.DataList)
        //     {
        //         if (tbfr.id == id)
        //         {
        //             thisFundRewardList.Add(tbfr);
        //         }
        //     }
        //
        //     thisFundRewardList.Sort(delegate(fund_reward f1, fund_reward f2) { return f1.level.CompareTo(f2.level); });
        //
        //     int unLockCount = 0;
        //     var level = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Level;
        //     foreach (var tbfr in thisFundRewardList)
        //     {
        //         if (tbfr.level <= level)
        //         {
        //             unLockCount++;
        //         }
        //         else
        //         {
        //             break;
        //         }
        //     }
        //
        //     float itemListH = thisFundRewardList.Count * 425 + (thisFundRewardList.Count - 1) * 36;
        //
        //     //set bg H
        //     //float unlockH = unLockCount * 267 + (unLockCount - 1) * 36 + 5;
        //     //float lockH = 0;
        //     //if (unLockCount == 0)
        //     //{
        //     //    lockH = itemListH + 5;
        //     //    unlockH = 0;
        //     //}
        //     //else
        //     //{
        //     //    lockH = itemListH - unlockH + 5;
        //     //}
        //
        //     //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_Item_Bg_Left).GetRectTransform()
        //     //    .SetHeight(unlockH);
        //     //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Left).GetRectTransform()
        //     //    .SetHeight(lockH);
        //     //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Left).GetRectTransform()
        //     //    .SetAnchoredPositionY(-lockH);
        //     //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_Item_Bg_Mid).GetRectTransform()
        //     //    .SetHeight(unlockH);
        //     //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Mid).GetRectTransform()
        //     //    .SetHeight(lockH);
        //     //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Mid).GetRectTransform()
        //     //    .SetAnchoredPositionY(-lockH);
        //     //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_Item_Bg_Right).GetRectTransform()
        //     //    .SetHeight(unlockH);
        //     //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Right).GetRectTransform()
        //     //    .SetHeight(lockH);
        //     //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Right).GetRectTransform()
        //     //    .SetAnchoredPositionY(-lockH);
        //
        //
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetRectTransform()
        //         .SetHeight(itemListH);
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetRectTransform()
        //         .SetOffsetWithLeft(0);
        //     shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetRectTransform()
        //         .SetOffsetWithRight(0);
        //     ContentH -= Module1403_Last_ListH;
        //     ContentH += itemListH + 200;
        //     Module1403_Last_ListH = itemListH + 200;
        //     shopFundListUI.GetRectTransform().SetHeight(itemListH + 200);
        //     fundUI.GetRectTransform().SetHeight(itemListH + 200 + 210);
        //     //Log.Debug("dddddddddddddddddddddddddd",Color.cyan);
        //     //giftui.GetFromReference(UISubPanel_Shop_1302_LimitGift.KPos_Item).GetRectTransform().SetAnchoredPositionY(-248f);
        //
        //     SetModule1403RedPointState();
        //
        //
        //     Module1403_Help_CreateOneRowItem(shopFundListUI, thisFundRewardList, thisFundByID).Forget();
        // }

        // private async UniTaskVoid Module1403_Help_CreateOneRowItem(UI shopFundListUI,
        //     List<fund_reward> thisFundRewardList, GameFoundation thisFundByID)
        // {
        //     var itemRowList = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetList();
        //     itemRowList.Clear();
        //
        //     int level = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Level;
        //
        //     Dictionary<int, int> leftGetDic = new Dictionary<int, int>();
        //     Dictionary<int, int> midGetDic = new Dictionary<int, int>();
        //     Dictionary<int, int> rightGetDic = new Dictionary<int, int>();
        //
        //     foreach (var tbfr in thisFundRewardList)
        //     {
        //         leftGetDic.Add(tbfr.level, 0);
        //         midGetDic.Add(tbfr.level, 0);
        //         rightGetDic.Add(tbfr.level, 0);
        //     }
        //
        //     string color1Str = "5AFF20";
        //     string color2Str = "FF69A5";
        //     string color3Str = "FFBF0F";
        //
        //     var shopMap = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList;
        //     foreach (var f in shopMap)
        //     {
        //         if (f.FoundId == thisFundRewardList[0].id)
        //         {
        //             if (f.RewardOneList != null)
        //             {
        //                 foreach (int getLevel in f.RewardOneList)
        //                 {
        //                     leftGetDic[getLevel] = 1;
        //                 }
        //             }
        //
        //             if (f.RewardTwoList != null)
        //             {
        //                 foreach (int getLevel in f.RewardTwoList)
        //                 {
        //                     midGetDic[getLevel] = 1;
        //                 }
        //             }
        //
        //             if (f.RewardThreeList != null)
        //             {
        //                 foreach (int getLevel in f.RewardThreeList)
        //                 {
        //                     rightGetDic[getLevel] = 1;
        //                 }
        //             }
        //         }
        //     }
        //
        //     int i = 0;
        //     foreach (var tbfr in thisFundRewardList)
        //     {
        //         int ihelp = i;
        //         var itemRow =
        //             await itemRowList.CreateWithUITypeAsync(UIType.UISubPanel_Shop_Fund_Item, false, cts.Token);
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.Kimg_bg)?.SetActive(false);
        //
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link).SetActive(false);
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_unlock).SetActive(false);
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End).SetActive(false);
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End_unlock).SetActive(false);
        //
        //         //已解锁
        //         if (tbfr.level <= level)
        //         {
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.Kimg_bg)?.SetActive(true);
        //             //itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link)?.SetActive(false);
        //             //最后一个
        //             if (ihelp == thisFundRewardList.Count - 1)
        //             {
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End_unlock).SetActive(true);
        //             }
        //             else
        //             {
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_unlock)?.SetActive(true);
        //             }
        //         }
        //         else
        //         {
        //             if (ihelp == thisFundRewardList.Count - 1)
        //             {
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End).SetActive(true);
        //             }
        //             else
        //             {
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link).SetActive(true);
        //             }
        //         }
        //
        //         //itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_unlock)?.SetActive(true);
        //
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Level).GetTextMeshPro()
        //             .SetTMPText(tbfr.level.ToString());
        //
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Remind_Left).GetTextMeshPro()
        //             .SetTMPText(tblanguage.Get("common_state_gain").current);
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Remind_Mid).GetTextMeshPro()
        //             .SetTMPText(tblanguage.Get("common_state_gain").current);
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Remind_Right).GetTextMeshPro()
        //             .SetTMPText(tblanguage.Get("common_state_gain").current);
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Received_Left).GetTextMeshPro()
        //             .SetTMPText(tblanguage.Get("common_state_gained").current);
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Received_Mid).GetTextMeshPro()
        //             .SetTMPText(tblanguage.Get("common_state_gained").current);
        //         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Received_Right).GetTextMeshPro()
        //             .SetTMPText(tblanguage.Get("common_state_gained").current);
        //
        //         var leftReward = itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KReward_Left);
        //         UnicornUIHelper.SetRewardIconAndCountText(tbfr.reward1[0], leftReward);
        //         var MidReward = itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KReward_Mid);
        //         UnicornUIHelper.SetRewardIconAndCountText(tbfr.reward2[0], MidReward);
        //         var RightReward = itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KReward_Right);
        //         UnicornUIHelper.SetRewardIconAndCountText(tbfr.reward3[0], RightReward);
        //
        //         var leftReBtn = leftReward.GetFromReference(UICommon_RewardItem.KBtn_Item);
        //         var midReBtn = MidReward.GetFromReference(UICommon_RewardItem.KBtn_Item);
        //         var rightReBtn = RightReward.GetFromReference(UICommon_RewardItem.KBtn_Item);
        //         //string sprite1Str = "icon_item_green";
        //         //string sprite2Str = "icon_item_purple";
        //         //string sprite3Str = "icon_item_yellow";
        //
        //         //leftReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetSpriteAsync(sprite1Str, false);
        //         //// .SetColor(HexToColor(color1Str));
        //         //MidReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetSpriteAsync(sprite2Str, false);
        //         //RightReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetSpriteAsync(sprite3Str, false);
        //
        //         Log.Debug($"index:{ihelp}", Color.cyan);
        //
        //         ////最后一个
        //         //if (ihelp == thisFundRewardList.Count - 1)
        //         //{
        //         //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End).SetActive(true);
        //         //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End_unlock).SetActive(true);
        //         //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link).SetActive(false);
        //         //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_unlock).SetActive(false);
        //         //}
        //         ////第一个
        //         //if (ihelp == 0)
        //         //{
        //         //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link).SetActive(false);
        //         //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_unlock).SetActive(false);
        //         //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End).SetActive(false);
        //         //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End_unlock).SetActive(false);
        //         //}
        //
        //
        //         if (tbfr.level <= level)
        //         {
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Left).SetActive(false);
        //             if (leftGetDic[tbfr.level] == 0)
        //             {
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Left).SetActive(false);
        //                 //Debug.Log(tbfr.level.ToString() + "set receive false");
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Left).SetActive(true);
        //                 Module1403GetUIList.Add(itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Left));
        //                 leftReBtn.GetXButton().OnClick.Add(() =>
        //                 {
        //                     //Module1403_Last_Time = TimeHelper.ClientNow();
        //                     //get reward
        //                     WebMessageHandlerOld.Instance.AddHandler(11, 9, On1403GetFundRewardResponse);
        //                     string reStr = tbfr.id.ToString() + ";1;" + tbfr.level.ToString();
        //                     StringValue stringValue = new StringValue();
        //                     stringValue.Value = reStr;
        //                     NetWorkManager.Instance.SendMessage(11, 9, stringValue);
        //                 });
        //             }
        //             else
        //             {
        //                 //have Received
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Left).SetActive(true);
        //                 //Debug.Log(tbfr.level.ToString() + "set receive true");
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Left).SetActive(false);
        //                 leftReBtn.GetXButton().OnClick.Add(() =>
        //                 {
        //                     //Module1403_Last_Time = TimeHelper.ClientNow();
        //                     UIHelper.CreateAsync(UIType.UICommon_Resource,
        //                         tblanguage.Get("common_state_gained_reward").current);
        //                 });
        //             }
        //
        //             if (thisFundByID.RewardTwoStatus == 0)
        //             {
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Mid).SetActive(true);
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid).SetActive(false);
        //                 midReBtn.GetXButton().OnClick.Add(() =>
        //                 {
        //                     //Module1403_Last_Time = TimeHelper.ClientNow();
        //                     UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("fund_unlock_tips").current);
        //                 });
        //             }
        //             else
        //             {
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Mid).SetActive(false);
        //                 if (midGetDic[tbfr.level] == 0)
        //                 {
        //                     itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Mid).SetActive(false);
        //                     itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid).SetActive(true);
        //                     Module1403GetUIList.Add(
        //                         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid));
        //                     midReBtn.GetXButton().OnClick.Add(() =>
        //                     {
        //                         //Module1403_Last_Time = TimeHelper.ClientNow();
        //                         //get reward
        //                         WebMessageHandlerOld.Instance.AddHandler(11, 9, On1403GetFundRewardResponse);
        //                         string reStr = tbfr.id.ToString() + ";2;" + tbfr.level.ToString();
        //                         StringValue stringValue = new StringValue();
        //                         stringValue.Value = reStr;
        //                         NetWorkManager.Instance.SendMessage(11, 9, stringValue);
        //                     });
        //                 }
        //                 else
        //                 {
        //                     //have Received
        //                     itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Mid).SetActive(true);
        //                     itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid).SetActive(false);
        //                     midReBtn.GetXButton().OnClick.Add(() =>
        //                     {
        //                         //Module1403_Last_Time = TimeHelper.ClientNow();
        //                         UIHelper.CreateAsync(UIType.UICommon_Resource,
        //                             tblanguage.Get("common_state_gained_reward").current);
        //                     });
        //                 }
        //             }
        //
        //             if (thisFundByID.RewardThreeStatus == 0)
        //             {
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Right).SetActive(true);
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right).SetActive(false);
        //                 rightReBtn.GetXButton().OnClick.Add(() =>
        //                 {
        //                     //Module1403_Last_Time = TimeHelper.ClientNow();
        //                     UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("fund_unlock_tips").current);
        //                 });
        //             }
        //             else
        //             {
        //                 itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Right).SetActive(false);
        //                 if (rightGetDic[tbfr.level] == 0)
        //                 {
        //                     itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Right).SetActive(false);
        //                     itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right).SetActive(true);
        //                     Module1403GetUIList.Add(
        //                         itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right));
        //                     rightReBtn.GetXButton().OnClick.Add(() =>
        //                     {
        //                         //Module1403_Last_Time = TimeHelper.ClientNow();
        //                         //get reward
        //                         WebMessageHandlerOld.Instance.AddHandler(11, 9, On1403GetFundRewardResponse);
        //                         string reStr = tbfr.id.ToString() + ";3;" + tbfr.level.ToString();
        //                         StringValue stringValue = new StringValue();
        //                         stringValue.Value = reStr;
        //                         NetWorkManager.Instance.SendMessage(11, 9, stringValue);
        //                     });
        //                 }
        //                 else
        //                 {
        //                     //have Received
        //                     itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Right).SetActive(true);
        //                     itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right).SetActive(false);
        //                     rightReBtn.GetXButton().OnClick.Add(() =>
        //                     {
        //                         UIHelper.CreateAsync(UIType.UICommon_Resource,
        //                             tblanguage.Get("common_state_gained_reward").current);
        //                     });
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             leftReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetColor(Color.white);
        //             MidReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetColor(Color.white);
        //             RightReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetColor(Color.white);
        //
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Left).SetActive(true);
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Mid).SetActive(true);
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Right).SetActive(true);
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Left).SetActive(false);
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid).SetActive(false);
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right).SetActive(false);
        //
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Left).SetActive(false);
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Mid).SetActive(false);
        //             itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Right).SetActive(false);
        //
        //             if (tbfr.reward1[0][0] == 3)
        //             {
        //                 leftReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
        //                     .SetSprite("gold_grey", false);
        //             }
        //             else
        //             {
        //                 leftReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
        //                     .SetSprite("icon_diamond_grey", false);
        //             }
        //
        //             if (tbfr.reward2[0][0] == 3)
        //             {
        //                 MidReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
        //                     .SetSprite("gold_grey", false);
        //             }
        //             else
        //             {
        //                 MidReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
        //                     .SetSprite("icon_diamond_grey", false);
        //             }
        //
        //             if (tbfr.reward3[0][0] == 3)
        //             {
        //                 RightReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
        //                     .SetSprite("gold_grey", false);
        //             }
        //             else
        //             {
        //                 RightReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
        //                     .SetSprite("icon_diamond_grey", false);
        //             }
        //
        //             leftReBtn.GetXButton().OnClick.Add(() =>
        //             {
        //                 //Module1403_Last_Time = TimeHelper.ClientNow();
        //                 UIHelper.CreateAsync(UIType.UICommon_Resource,
        //                     tblanguage.Get("common_state_level_limit").current);
        //             });
        //             midReBtn.GetXButton().OnClick.Add(() =>
        //             {
        //                 //Module1403_Last_Time = TimeHelper.ClientNow();
        //                 UIHelper.CreateAsync(UIType.UICommon_Resource,
        //                     tblanguage.Get("common_state_level_limit").current);
        //             });
        //             rightReBtn.GetXButton().OnClick.Add(() =>
        //             {
        //                 //Module1403_Last_Time = TimeHelper.ClientNow();
        //                 UIHelper.CreateAsync(UIType.UICommon_Resource,
        //                     tblanguage.Get("common_state_level_limit").current);
        //             });
        //         }
        //
        //         i++;
        //     }
        //
        //     UnicornUIHelper.ForceRefreshLayout(shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item));
        // }

        // private void Module1403_Help_RemoveNewByID(int id)
        // {
        //     //Module1403_PointUIList[Module1403_PointUIList.Count - index - 1].GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip).SetActive(false);
        //     //Module1403_NewUIIndexList.Remove(index);
        //     //if (Module1403_NewUIIndexList.Count > 0)
        //     //{
        //     //    Module1403_NowNewUIIndexListIndex = 0;
        //     //}
        //     IntValue intValue = new IntValue();
        //     intValue.Value = id;
        //     WebMessageHandlerOld.Instance.AddHandler(11, 15, On1403LookFundResponse);
        //     NetWorkManager.Instance.SendMessage(11, 15, intValue);
        //     Debug.Log("send 1403 11-15 web");
        //     for (int i = 0; i < ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList.Count; i++)
        //     {
        //         if (ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList[i].FoundId == id)
        //         {
        //             ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList[i].IsLook = 1;
        //             break;
        //         }
        //     }
        // }
        private int2 Module1403_Help_FindFirstFund()
        {
            int2 fundIdAndLevel = new int2();

            var GameFoundationList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1403].GameFoundationList;
            List<GameFoundation> fundList = new List<GameFoundation>();
            foreach (var f in GameFoundationList)
            {
                fundList.Add(f);
            }

            int level = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Level;

            var fundForwardSortList = new List<fund>();
            foreach (var tbf in tbfund.DataList)
            {
                fundForwardSortList.Add(tbf);
            }

            fundForwardSortList.Sort(delegate(fund f1, fund f2) { return f1.unlockLevel.CompareTo(f2.unlockLevel); });

            List<int> fundIDList = new List<int>();
            foreach (var f in fundForwardSortList)
            {
                if (level >= f.unlockLevel)
                {
                    fundIDList.Add(f.id);
                }
            }

            foreach (var fundID in fundIDList)
            {
                GameFoundation f = new GameFoundation();
                foreach (var fu in fundList)
                {
                    if (fu.FoundId == fundID)
                    {
                        f = fu;
                    }
                }

                int thisFundCount = 0;
                foreach (var tbfr in tbfund_Reward.DataList)
                {
                    if (tbfr.id == fundID && tbfr.level <= level)
                    {
                        thisFundCount++;
                    }
                }

                int allReCount = thisFundCount;
                if (f.RewardTwoStatus == 1)
                {
                    allReCount += thisFundCount;
                }

                if (f.RewardThreeStatus == 1)
                {
                    allReCount += thisFundCount;
                }

                if (allReCount == f.RewardOneList.Count + f.RewardTwoList.Count + f.RewardThreeList.Count)
                {
                    fundIdAndLevel.x = fundID;
                    continue;
                }
                else
                {
                    fundIdAndLevel.x = fundID;
                    break;
                }
            }

            foreach (var f in fundList)
            {
                if (f.FoundId == fundIdAndLevel.x)
                {
                    //one
                    int one = 0;
                    Dictionary<int, int> levelGetDicOne = new Dictionary<int, int>();
                    List<fund_reward> frListOne = new List<fund_reward>();
                    foreach (var tbfr in tbfund_Reward.DataList)
                    {
                        if (tbfr.id == fundIdAndLevel.x && tbfr.level <= level)
                        {
                            levelGetDicOne.Add(tbfr.level, 0);
                            frListOne.Add(tbfr);
                        }
                    }

                    frListOne.Sort(delegate(fund_reward f1, fund_reward f2) { return f1.level.CompareTo(f2.level); });
                    foreach (var r in f.RewardOneList)
                    {
                        levelGetDicOne[r] = 1;
                    }

                    foreach (var fr in frListOne)
                    {
                        one = fr.level;
                        if (levelGetDicOne[fr.level] == 1)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }


                    //two
                    int two = 0;
                    if (f.RewardTwoStatus == 0)
                    {
                        two = 99999;
                    }
                    else
                    {
                        Dictionary<int, int> levelGetDicTwo = new Dictionary<int, int>();
                        List<fund_reward> frListTwo = new List<fund_reward>();
                        foreach (var tbfr in tbfund_Reward.DataList)
                        {
                            if (tbfr.id == fundIdAndLevel.x && tbfr.level <= level)
                            {
                                levelGetDicTwo.Add(tbfr.level, 0);
                                frListTwo.Add(tbfr);
                            }
                        }

                        frListTwo.Sort(
                            delegate(fund_reward f1, fund_reward f2) { return f1.level.CompareTo(f2.level); });
                        foreach (var r in f.RewardTwoList)
                        {
                            levelGetDicTwo[r] = 1;
                        }

                        foreach (var fr in frListTwo)
                        {
                            two = fr.level;
                            if (levelGetDicTwo[fr.level] == 1)
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    //three
                    int three = 0;
                    if (f.RewardThreeStatus == 0)
                    {
                        three = 99999;
                    }
                    else
                    {
                        Dictionary<int, int> levelGetDicThree = new Dictionary<int, int>();
                        List<fund_reward> frListThree = new List<fund_reward>();
                        foreach (var tbfr in tbfund_Reward.DataList)
                        {
                            if (tbfr.id == fundIdAndLevel.x && tbfr.level <= level)
                            {
                                levelGetDicThree.Add(tbfr.level, 0);
                                frListThree.Add(tbfr);
                            }
                        }

                        frListThree.Sort(delegate(fund_reward f1, fund_reward f2)
                        {
                            return f1.level.CompareTo(f2.level);
                        });
                        foreach (var r in f.RewardTwoList)
                        {
                            levelGetDicThree[r] = 1;
                        }

                        foreach (var fr in frListThree)
                        {
                            three = fr.level;
                            if (levelGetDicThree[fr.level] == 1)
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    int final = math.min(one, two);
                    final = math.min(final, three);
                    fundIdAndLevel.y = final;
                }
            }

            return fundIdAndLevel;
        }

        #endregion

        #region module1404

        private Dictionary<int, UI> MonthCardIDUIDIc = new Dictionary<int, UI>();

        private async void Module1404(bool isUpdate)
        {
            if (!ResourcesSingletonOld.Instance.shopMap.IndexModuleMap.ContainsKey(1404))
            {
                return;
            }

            MonthCardIDUIDIc.Clear();
            float module1404DeltaY = 141;
            List<monthly> monthlies = new List<monthly>();
            foreach (var tbm in tbmonthly.DataList)
            {
                monthlies.Add(tbm);
                Log.Debug($"monthid:{tbm.id}", Color.red);
            }

            monthlies.Sort(delegate(monthly m1, monthly m2) { return m1.sort.CompareTo(m2.sort); });
            var curHeight = this.GetFromReference(KScrollView).GetRectTransform().Height();


            float this1404H = curHeight;

            var shopViewList = this.GetFromReference(KScrollView).GetXScrollRect().Content.GetList();


            //var cardUI=await UIHelper.CreateAsyncNew(this, UIType.UISubPanel_Shop_1404_MonthCard, shopViewList.GameObject.transform) as UISubPanel_Shop_1404_MonthCard;
            var cardUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_1404_MonthCard, false);
            cardUI.GetRectTransform().SetHeight(this1404H);
            ModuleUIAndIDDic.Add(cardUI, 1404);
            var cardList = cardUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KPos).GetList();

            foreach (var tbm in monthlies)
            {
                var card = cardList.CreateWithUIType(UIType.UISubPanel_Shop_MonCard, tbm.id, false);
                MonthCardIDUIDIc.Add(tbm.id, card);
                Log.Debug($"monthid:{tbm.id}", Color.cyan);
                Module1404_Help_SetCard(card, tbm);
                if (!isUpdate)
                {
                    if (tbm.id == 1)
                    {
                        UnicornTweenHelper.SetEaseAlphaAndPosRtoL(card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Card),
                            -544,
                            800, cts.Token,
                            0.35f, false, false);
                    }
                    else
                    {
                        UnicornTweenHelper.SetEaseAlphaAndPosLtoR(card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Card),
                            -544,
                            800, cts.Token,
                            0.35f, false, false);
                    }

                    UnicornTweenHelper.SetEaseAlphaAndPosUtoB(card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Card), 382,
                        200, cts.Token,
                        0.35f, false, false);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Card).GetComponent<CanvasGroup>().alpha = 0.2f;
                    card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Card).GetComponent<CanvasGroup>().DOFade(1, 0.5f)
                        .SetEase(Ease.InQuad);
                    UnicornTweenHelper.SetAngleRotate(card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Card), 0, 45, 0.35f,
                        cts.Token);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KTitleBg).GetImage().SetAlpha(0);
                }


                //await UniTask.Delay(200);
            }


            Module1404_Help_SetEnergyMAX();
            UnicornUIHelper.ForceRefreshLayout(cardUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KPos));

            ContentH += this1404H;
            if (!isUpdate)
            {
                await UniTask.Delay(300);

                for (int i = 0; i < cardList.Children.Count; i++)
                {
                    var posY = cardList.Children[i].GetFromReference(UISubPanel_Shop_MonCard.KTitleBg)
                        .GetRectTransform()
                        .AnchoredPosition().y;
                    UnicornTweenHelper.SetEaseAlphaAndPosUtoB(
                        cardList.Children[i].GetFromReference(UISubPanel_Shop_MonCard.KTitleBg), posY, 60, cts.Token,
                        0.3f, true,
                        false);
                    cardList.Children[i].GetFromReference(UISubPanel_Shop_MonCard.KTitleBg).GetImage().SetAlpha(1);

                    UnicornTweenHelper.PlayUIImageTranstionFX(
                        cardList.Children[i].GetFromReference(UISubPanel_Shop_MonCard.KTitleBg), cts.Token, "A1DD01",
                        UnicornTweenHelper.UIDir.Up, 0, 1f);
                }
            }
        }

        private void Module1404_Help_SetEnergyMAX()
        {
            var cardMap = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1404].SpecialCard.Unclaimed;
            int addEnergy = 0;
            foreach (var cm in cardMap)
            {
                var cardData = cm.Value;
                if (cardData.EndTime <= TimeHelper.ClientNowSeconds())
                {
                }
                else
                {
                    var spList = tbmonthly.Get(cardData.Id).userVariable;
                    foreach (var sp in spList)
                    {
                        if (sp[0] == 15)
                        {
                            addEnergy += (int)sp[1];
                        }
                    }
                }
            }

            if (addEnergy + tbuser_Variable.Get(1).init != ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax)
            {
                ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax = addEnergy + tbuser_Variable.Get(1).init;
                ResourcesSingletonOld.Instance.UpdateResourceUI();
            }
        }

        private void Module1404_Help_SetCard(UI card, monthly tbm)
        {
            //if (UnicornUIHelper.TryGetUI(UIType.UISubPanel_Shop_1404_MonthCard, out UI uui))
            //{
            //    Log.Debug("有滴答滴答滴答滴答滴滴答答哒哒哒哒哒哒哒哒哒哒哒哒哒哒哒哒哒哒",Color.cyan);
            //}


            int tagFuncID = 1404;
            var redDotStr = NodeNames.GetTagFuncRedDotName(tagFuncID) + '|';
            var redString = redDotStr + tbm.id.ToString();
            RedDotManager.Instance.InsterNode(redString);
            RedDotManager.Instance.SetRedPointCnt(redString, 0);
            card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Card).GetImage().SetSpriteAsync(tbm.pic1, false);
            var cardImg = "monthly_" + tbm.id.ToString();
            card.GetFromReference(UISubPanel_Shop_MonCard.KCard).GetImage().SetSpriteAsync(cardImg, false);
            var tittleImg = "monthly_tittle_" + tbm.id.ToString();
            card.GetFromReference(UISubPanel_Shop_MonCard.KTitleBg).GetImage().SetSpriteAsync(tittleImg, false);
            card.GetFromReference(UISubPanel_Shop_MonCard.KText_Note).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("monthly_tips").current);
            //card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Bg).GetImage().SetSpriteAsync(tbm.pic2, false);
            card.GetFromReference(UISubPanel_Shop_MonCard.KText_Name).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tbm.name).current);
            card.GetFromReference(UISubPanel_Shop_MonCard.KText_State).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_unactivated").current);
            card.GetFromReference(UISubPanel_Shop_MonCard.KText_Effect).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_in_effect").current);
            card.GetFromReference(UISubPanel_Shop_MonCard.KText_Get).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_gain").current);
            card.GetFromReference(UISubPanel_Shop_MonCard.KText_Got).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_gained").current);
            //card.GetFromReference(UISubPanel_Shop_MonCard.KText_GetDiamond).GetTextMeshPro()
            //    .SetTMPText(tblanguage.Get("common_state_gain").current);

            #region Special Benefits

            //card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Special).GetImage().SetSpriteAsync(tbm.pic3, false);
            card.GetFromReference(UISubPanel_Shop_MonCard.KText_Special).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("text_exclusive").current);

            int specialCount = tbm.desc.Count;


            card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_1).SetActive(false);
            card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_2).SetActive(false);
            card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_3).SetActive(false);
            card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_4).SetActive(false);
            card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_5).SetActive(false);
            card.GetFromReference(UISubPanel_Shop_MonCard.KTextNoAD).SetActive(false);

            if (tbm.id == 1)
            {
                card.GetFromReference(UISubPanel_Shop_MonCard.KTextNoAD).SetActive(true);
                card.GetFromReference(UISubPanel_Shop_MonCard.KTextNoAD).GetTextMeshPro()
                    .SetTMPText(Module1404_Help_Special(tbm, 0));
                card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Describe).GetRectTransform()
                    .SetAnchoredPositionY(-402);
                card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Describe).GetRectTransform().SetHeight(168);
            }
            else
            {
                card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Describe).GetRectTransform()
                    .SetAnchoredPositionY(-250);
                card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Describe).GetRectTransform().SetHeight(300);
            }

            for (int i = 0; i < specialCount; i++)
            {
                int ihelp = i;
                //int2 benefitProperty = new int2();
                //benefitProperty.x = (int)tbm.userVariable[ihelp].x;
                //benefitProperty.y = (int)tbm.userVariable[ihelp].y;
                if (i == 0)
                {
                    if (tbm.id == 1)
                    {
                    }
                    else
                    {
                        card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_1).SetActive(true);
                        //line 1
                        card.GetFromReference(UISubPanel_Shop_MonCard.KImgSpecial).SetActive(true);
                        card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_1).GetTextMeshPro()
                            .SetTMPText("1." + Module1404_Help_Special(tbm, i));
                    }
                }

                if (i == 1)
                {
                    card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_2).SetActive(true);
                    //line 2
                    //card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_3).GetRectTransform().SetAnchoredPositionY(0);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_2).GetTextMeshPro()
                        .SetTMPText("2." + Module1404_Help_Special(tbm, i));
                    if (tbm.id == 1)
                    {
                        card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_2).GetTextMeshPro()
                            .SetTMPText("1." + Module1404_Help_Special(tbm, i));
                    }
                    //var textHeight = card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_4).GetTextMeshPro().Get().preferredHeight;
                    //card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_4).GetRectTransform().SetHeight(textHeight-10f);
                }

                if (i == 2)
                {
                    card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_3).SetActive(true);
                    //line 3
                    //card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_3).GetRectTransform().SetAnchoredPositionY(0);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_3).GetTextMeshPro()
                        .SetTMPText("3." + Module1404_Help_Special(tbm, i));
                    if (tbm.id == 1)
                    {
                        card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_3).GetTextMeshPro()
                            .SetTMPText("2." + Module1404_Help_Special(tbm, i));
                    }
                }

                if (i == 3)
                {
                    card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_4).SetActive(true);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_4).GetTextMeshPro()
                        .SetTMPText("4." + Module1404_Help_Special(tbm, i));
                    var textHeight = card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_4).GetTextMeshPro().Get()
                        .preferredHeight;
                    card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_4).GetRectTransform().SetHeight(textHeight);
                }

                if (i == 4)
                {
                    card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_5).SetActive(true);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_5).GetTextMeshPro()
                        .SetTMPText("5." + Module1404_Help_Special(tbm, i));
                }
            }

            //card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_2).GetTextMeshPro()
            //    .SetTMPText("2." + tblanguage.Get("monthly_reward").current);
            //card.GetFromReference(UISubPanel_Shop_MonCard.KText_number_Line_2).GetTextMeshPro()
            //    .SetTMPText(tbm.reward[0][2].ToString());

            //card.GetFromReference(UISubPanel_Shop_MonCard.KText_Line_3).GetTextMeshPro()
            //    .SetTMPText("3." + tblanguage.Get("monthly_daily_reward").current);
            //card.GetFromReference(UISubPanel_Shop_MonCard.KText_number_Line_3).GetTextMeshPro()
            //    .SetTMPText(tbm.rewardDaily[0][2].ToString());

            #endregion


            #region Set State

            var cardMap = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1404].SpecialCard.Unclaimed;
            UnReward cardData = new UnReward();
            foreach (var cm in cardMap)
            {
                if (cm.Value.Id == tbm.id)
                {
                    cardData = cm.Value;
                }
            }


            bool isBuy = false;
            bool isActiveAd = false;
            ResourcesSingletonOld.Instance.monthBuy = false;
            isActiveAd = ResourcesSingletonOld.Instance.UserInfo.AdFreeFlag == 0 ? false : true;
            if (cardData.EndTime <= TimeHelper.ClientNowSeconds())
            {
                isBuy = false;
            }
            else
            {
                isBuy = true;
            }

            if ((!isBuy && tbm.id == 2) || (!isActiveAd && tbm.id == 1))
            {
                //not buy
                card.GetFromReference(UISubPanel_Shop_MonCard.KText_State).SetActive(true);
                card.GetFromReference(UISubPanel_Shop_MonCard.KBtn_Buy).SetActive(true);
                card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Tip).SetActive(false);
                card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Btn_Get).SetActive(false);

                card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).SetActive(false);

                //if (UnicornUIHelper.TryGetUI(UIType.UISubPanel_Shop_1404_MonthCard,out UI cardParent)){
                //    var cardParentUI = cardParent as UISubPanel_Shop_1404_MonthCard;
                //    cardParentUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KText_TimeDownUp).SetActive(false);
                //    cardParentUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KText_TimeDownBottom).SetActive(false);
                //}
                card.GetFromReference(UISubPanel_Shop_MonCard.KText_BuyBtn).GetTextMeshPro()
                    .SetTMPText(tbprice.Get(tbm.price).rmb.ToString() + tblanguage.Get("common_coin_unit").current);

                var KBtn_Buy = card.GetFromReference(UISubPanel_Shop_MonCard.KBtn_Buy);

                var cardUI = card as UISubPanel_Shop_MonCard;
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Buy, () =>
                {
                    const string Fund1 = "C02";
                    UnicornUIHelper.SendBuyMessage(Fund1, cardUI.id);
                    JiYuEventManager.Instance.RegisterEvent("OnShopResponse", (a) =>
                    {
                        var shopNum = UnicornUIHelper.GetShopNum(a);

                        switch (shopNum)
                        {
                            case "C02":

                                WebMessageHandlerOld.Instance.AddHandler(CMDOld.INITPLAYER, OnRefreshAddVaule);
                                NetWorkManager.Instance.SendMessage(CMDOld.INITPLAYER);
                                break;
                        }
                    });
                });

                UnicornTweenHelper.PlayUIImageSweepFX(KBtn_Buy, cancellationToken: cts.Token);
            }
            else
            {
                UnicornTweenHelper.PlayUIImageSweepFX(card.GetFromReference(UISubPanel_Shop_MonCard.KCard),
                    cancellationToken: cts.Token);
                card.GetFromReference(UISubPanel_Shop_MonCard.KText_State).SetActive(false);
                card.GetFromReference(UISubPanel_Shop_MonCard.KImg_Tip).SetActive(true);
                card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Btn_Get).SetActive(true);
                card.GetFromReference(UISubPanel_Shop_MonCard.KBtn_Buy).SetActive(false);
                //月卡逻辑
                if (tbm.id == 2)
                {
                    ResourcesSingletonOld.Instance.monthBuy = true;
                    card.GetFromReference(UISubPanel_Shop_MonCard.KBtn_Buy).SetActive(true);
                    //day compare
                    long deltaSec = cardData.EndTime - TimeHelper.ClientNowSeconds();

                    Debug.Log(deltaSec);
                    long oneDaySec = 24 * 60 * 60;


                    if (deltaSec < oneDaySec)
                    {
                        string timeStr = tblanguage.Get("text_remain_time").current;
                        timeStr = timeStr + UnicornUIHelper.GeneralTimeFormat(new int4(2, 3, 2, 1), deltaSec);
                        card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).GetTextMeshPro()
                            .SetTMPText(timeStr);
                        //cardParentUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KText_TimeDownUp).GetTextMeshPro().SetTMPText(timeStr);
                        //cardParentUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KText_TimeDownBottom).GetTextMeshPro().SetTMPText(timeStr);
                    }
                    else
                    {
                        string timeStr = tblanguage.Get("text_remain_time").current;
                        timeStr = timeStr + UnicornUIHelper.GeneralTimeFormat(new int4(4, 4, 2, 1), deltaSec);
                        card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).GetTextMeshPro()
                            .SetTMPText(timeStr);
                        //cardParentUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KText_TimeDownUp).GetTextMeshPro().SetTMPText(timeStr);
                        //cardParentUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KText_TimeDownBottom).GetTextMeshPro().SetTMPText(timeStr);
                    }

                    //续费逻辑
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_BuyBtn).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("common_state_renew").current);

                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).SetActive(true);
                    var buyBtn = card.GetFromReference(UISubPanel_Shop_MonCard.KBtn_Buy);
                    var cardUI = card as UISubPanel_Shop_MonCard;
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(buyBtn, () =>
                    {
                        const string Fund1 = "C02";
                        UnicornUIHelper.SendBuyMessage(Fund1, cardUI.id);
                        JiYuEventManager.Instance.RegisterEvent("OnShopResponse", (a) =>
                        {
                            var shopNum = UnicornUIHelper.GetShopNum(a);

                            switch (shopNum)
                            {
                                case "C02":
                                    WebMessageHandlerOld.Instance.AddHandler(16, 3, On1404QueryCardResponse);
                                    NetWorkManager.Instance.SendMessage(16, 3);
                                    WebMessageHandlerOld.Instance.AddHandler(CMDOld.INITPLAYER, OnRefreshAddVaule);
                                    NetWorkManager.Instance.SendMessage(CMDOld.INITPLAYER);
                                    break;
                            }
                        });
                    });
                }

                //不能领取
                if (cardData.SumDay < 1)
                {
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_Got).SetActive(true);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KBtn_Get).SetActive(false);
                    RedDotManager.Instance.SetRedPointCnt(redString, 0);
                    //GetFromReference(KImg_AdvertRedDot)?.SetActive(false);

                    //RedPointMgr.instance.SetState(ShopRoot, "module1404" + tbm.id, RedPointState.Hide);
                }
                else if (cardData.SumDay == 1)
                {
                    // RedPointMgr.instance.SetState(ShopRoot, "module1404" + tbm.id, RedPointState.Show);


                    RedDotManager.Instance.SetRedPointCnt(redString, 1);

                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_Got).SetActive(false);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KBtn_Get).SetActive(true);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_Get).SetActive(true);
                }
                else if (cardData.SumDay > 1)
                {
                    // RedPointMgr.instance.SetState(ShopRoot, "module1404" + tbm.id, RedPointState.Show);

                    RedDotManager.Instance.SetRedPointCnt(redString, 1);

                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_Got).SetActive(false);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KBtn_Get).SetActive(true);
                    //card.GetFromReference(UISubPanel_Shop_MonCard.KPos_GetDiamond).SetActive(true);
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_Get).SetActive(false);
                    //card.GetFromReference(UISubPanel_Shop_MonCard.KText_Number_Get).GetTextMeshPro()
                    //    .SetTMPText((cardData.SumDay * tbm.rewardDaily[0][2]).ToString());
                }

                var getBtn = card.GetFromReference(UISubPanel_Shop_MonCard.KBtn_Get);
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(getBtn, () =>
                {
                    WebMessageHandlerOld.Instance.AddHandler(16, 2, On1404GetCardResponse);
                    IntValue intValue = new IntValue();
                    intValue.Value = tbm.id;
                    NetWorkManager.Instance.SendMessage(16, 2, intValue);
                });
            }

            #endregion

            if (tbm.id == 1)
            {
                card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).SetActive(false);
            }

            UnicornUIHelper.ForceRefreshLayout(card.GetFromReference(UISubPanel_Shop_MonCard.KPos_Line_4));
        }

        private string Module1404_Help_Special(monthly tbm, int index)
        {
            var desc1 = tbm.desc[index];
            var desc2 = tbm.descPara[index].ToString();
            desc2 = UnityHelper.RichTextColor(desc2, "67CC4D");
            desc2 = UnityHelper.RichTextSize(desc2, 40);
            string result = string.Format(tblanguage.Get(desc1).current, desc2);

            var addstr = UnityHelper.RichTextSize(UnityHelper.RichTextColor("+", "67CC4D"), 40);
            result = result.Replace("+", addstr);
            var percentstr = UnityHelper.RichTextSize(UnityHelper.RichTextColor("%", "67CC4D"), 40);
            result = result.Replace("%", percentstr);
            return result;
        }

        private string Module1404_Help_SpecialTest(int2 input)
        {
            #region old

            string result = tblanguage.Get(tbuser_Variable.Get(input.x).desc).current;
            switch (input.x)
            {
                case 11:
                    var str = "+" + (input.y / 100).ToString() + "%";
                    str = UnityHelper.RichTextColor(str, "67CC4D");
                    str = UnityHelper.RichTextSize(str, 40);
                    result = result.Replace("{0}", str);
                    break;
                case 15:
                    str = "+" + input.y.ToString();
                    str = UnityHelper.RichTextColor(str, "67CC4D");
                    str = UnityHelper.RichTextSize(str, 40);
                    result = result.Replace("{0}", str);
                    break;
                case 16:
                    str = "+" + input.y.ToString();
                    str = UnityHelper.RichTextColor(str, "67CC4D");
                    str = UnityHelper.RichTextSize(str, 40);
                    result = result.Replace("{0}", str);
                    break;
                case 17:
                    break;
                default:
                    break;
            }

            #endregion

            return result;
        }

        private void Module1404_Help_SetTextByTime()
        {
            //if (!UnicornUIHelper.TryGetUI(UIType.UISubPanel_Shop_1404_MonthCard, out UI cardParent))
            //{
            //    return;
            //}
            //var cardParentUI = cardParent as UISubPanel_Shop_1404_MonthCard;
            foreach (var cm in ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1404].SpecialCard.Unclaimed)
            {
                UnReward cardData = new UnReward();
                cardData = cm.Value;
                Log.Debug($"key:{cm.Key},value:{cm.Value}");
                if (!MonthCardIDUIDIc.ContainsKey(cm.Key))
                {
                    continue;
                }

                var card = MonthCardIDUIDIc[cm.Key];
                if (cardData.Id == 1)
                {
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).SetActive(false);
                }
                else
                {
                    card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).SetActive(false);
                    if (cardData.EndTime > TimeHelper.ClientNowSeconds())
                    {
                        card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).SetActive(true);
                        long deltaSec = cardData.EndTime - TimeHelper.ClientNowSeconds();
                        long oneDaySec = 24 * 60 * 60;
                        if (deltaSec < oneDaySec)
                        {
                            string timeStr = tblanguage.Get("text_remain_time").current;
                            timeStr = timeStr + UnicornUIHelper.GeneralTimeFormat(new int4(2, 3, 2, 1), deltaSec);
                            card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).GetTextMeshPro()
                                .SetTMPText(timeStr);
                        }
                        else
                        {
                            string timeStr = tblanguage.Get("text_remain_time").current;
                            timeStr = timeStr + UnicornUIHelper.GeneralTimeFormat(new int4(4, 4, 2, 1), deltaSec);
                            card.GetFromReference(UISubPanel_Shop_MonCard.KText_TimeDown).GetTextMeshPro()
                                .SetTMPText(timeStr);
                            //cardParentUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KText_TimeDownUp).GetTextMeshPro()
                            //  .SetTMPText(timeStr);
                            //cardParentUI.GetFromReference(UISubPanel_Shop_1404_MonthCard.KText_TimeDownBottom).GetTextMeshPro()
                            //.SetTMPText(timeStr);
                        }
                    }
                }
            }
        }

        #endregion

        private void ModuleClose()
        {
            if (ctsOld != null)
            {
                ctsOld.Cancel();
            }
        }

        #region Update

        private void Update()
        {
            foreach (int ModuleID in NowModuleIdList)
            {
                switch (ModuleID)
                {
                    case 1101:
                        Module1101Update();
                        break;
                    case 1102:

                        break;
                    case 1103:

                        break;
                    case 1201:
                        Module1201Update();
                        break;
                    case 1202:
                        Module1202Update();
                        break;
                    case 1301:

                        break;
                    case 1302:
                        Module1302Update();
                        break;
                    case 1401:

                        break;
                    case 1402:

                        break;
                    case 1403:
                        Module1403Update();
                        break;
                    case 1404:
                        Module1404Update();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Module1101Update()
        {
            Module1101_Help_SetTextByTime();
        }

        private void Module1201Update()
        {
            Module1201_Help_SetTextByTime();
        }

        private void Module1202Update()
        {
            Module1202_Help_SetTimeText();
        }


        private void Module1302Update()
        {
            Module1302_SetTextByTime();
        }


        private void Module1403Update()
        {
            //foreach (var getUI in Module1403GetUIList)
            //{
            //    UIUpAndDownIn1Sec(getUI);
            //}

            //if (Module1403_NewUIIndexList.Count > 1)
            //{
            //    //if (TimeHelper.ClientNow() - Module1403_Last_Time >= 1000)
            //    if (TimeHelper.ClientNow() - Module1403_Last_Time >= tbconstant.Get("fund_switch_time").constantValue * 1000)
            //    {
            //        Module1403_Last_Time = TimeHelper.ClientNow();
            //        Module1403_Help_NewTipChange();
            //    }
            //}
        }

        private void Module1404Update()
        {
            Module1404_Help_SetTextByTime();
        }

        // private void UIUpAndDownIn1Sec(UI input)
        // {
        //     input.GetRectTransform().DoAnchoredPositionY(-10, 0.5f).AddOnCompleted(() =>
        //     {
        //         input.GetRectTransform().DoAnchoredPositionY(10, 0.5f);
        //     });
        // }

        #endregion

        private List<int> GetBoxIDByModuleID(int moduleID)
        {
            List<int> list = new List<int>();
            foreach (var tbdb in tbdraw_Box.DataList)
            {
                if (tbdb.tagFunc == moduleID)
                {
                    var shopMap = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[moduleID].BoxInfoList;
                    foreach (var s in shopMap)
                    {
                        if (s.Id == tbdb.id)
                        {
                            if (moduleID == 1101)
                            {
                                if (tbdb.limitType == 2 || tbdb.limitType == 4)
                                {
                                    long startTime = s.SetStartTime;
                                    long endTime = startTime + tbdb.dateLimit;
                                    long deltaTime = endTime - TimeHelper.ClientNowSeconds();

                                    if (deltaTime > 0)
                                    {
                                        if (tbdb.limitType == 4)
                                        {
                                            if (s.DrawCount > 0)
                                            {
                                                list.Add(tbdb.id);
                                            }
                                        }
                                        else
                                        {
                                            list.Add(tbdb.id);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (tbdb.limitType == 3)
                                    {
                                        if (s.DrawCount > 0)
                                        {
                                            list.Add(tbdb.id);
                                            break;
                                        }
                                        else
                                        {
                                            //list.Add(tbdb.id);
                                            //break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                list.Add(tbdb.id);
                                break;
                            }
                        }
                    }
                }
            }

            //sort
            if (list.Count <= 0)
            {
            }
            else
            {
                list.Sort(delegate(int id1, int id2)
                {
                    return tbdraw_Box.Get(id1).sort.CompareTo(tbdraw_Box.Get(id2).sort);
                });
            }

            return list;
        }

        private void SetContentH(float height, float offsetTop)
        {
            //set offset top
            this.GetFromReference(KScrollView).GetRectTransform().SetOffsetWithTop(-offsetTop);

            //set content h
            float ScrollH = this.GetFromReference(KScrollView).GetRectTransform().Height();

            this.GetFromReference(KScrollView).GetXScrollRect().Content.GetRectTransform().SetHeight(height);
            this.GetFromReference(KScrollView).GetXScrollRect().Content.GetRectTransform().SetOffsetWithLeft(0);
            this.GetFromReference(KScrollView).GetXScrollRect().Content.GetRectTransform().SetOffsetWithRight(0);

            // if (height <= ScrollH)
            // {
            //     this.GetFromReference(KScrollView).GetComponent<ScrollRect>().movementType =
            //         ScrollRect.MovementType.Clamped;
            // }
            // else
            // {
            //     this.GetFromReference(KScrollView).GetComponent<ScrollRect>().movementType =
            //         ScrollRect.MovementType.Elastic;
            // }
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

        #region WebResponse

        #region 1201Response

        private void On1201QueryResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 6, On1201QueryResponse);

            ByteValueList dailyBuys = new ByteValueList();
            dailyBuys.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(dailyBuys);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            var dbList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList;
            foreach (var d in dailyBuys.Values)
            {
                DailyBuy daily = new DailyBuy();
                daily.MergeFrom(d);
                int i = 0;
                foreach (var db in dbList)
                {
                    if (tbshop_Daily.Get(db.Sign).pos == tbshop_Daily.Get(daily.Sign).pos)
                    {
                        ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1201].DailyBuyList[i] = daily;
                        break;
                    }

                    i++;
                }
            }

            UpDateNowInterface();
            ResourcesSingletonOld.Instance.UpdateResourceUI();
        }

        #endregion

        #region 1401Response

        //private void On1401BuyResponse(object sender, WebMessageHandlerOld.Execute e)
        //{
        //    WebMessageHandlerOld.Instance.RemoveHandler(11, 4, On1401BuyResponse);
        //    var intvalue = new IntValue();
        //    intvalue.MergeFrom(e.data);
        //    Debug.Log(e);
        //    Debug.Log(intvalue);
        //    if (e.data.IsEmpty)
        //    {
        //        Log.Debug("e.data.IsEmpty", Color.red);
        //    }
        //}

        #endregion

        #region 1402Response

        #endregion

        #region 1403Response

        private void On1403LookFundResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 15, On1403LookFundResponse);

            //Debug.Log("On1403LookFundResponse");

            //StringValue stringValue = new StringValue();
            //stringValue.MergeFrom(e.data);

            //if (e.data.IsEmpty)
            //{
            //    return;
            //}

            //if (stringValue.Value == "success")
            //{
            //    WebMessageHandlerOld.Instance.AddHandler(11, 8, On1403QueryFundResponse);
            //    NetWorkManager.Instance.SendMessage(11, 8);
            //}
        }

        private void On1403BuyFundResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 11, On1403BuyFundResponse);
            var str = new StringValue();
            str.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Debug.Log("11 - 11 is empty");
                return;
            }

            if (str.Value == "success")
            {
                WebMessageHandlerOld.Instance.AddHandler(11, 8, On1403QueryFundResponse);
                NetWorkManager.Instance.SendMessage(11, 8);
            }
            else
            {
                //defeat
            }
        }

        private void On1403QueryFundResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 8, On1403QueryFundResponse);
            var fundList = new ByteValueList();
            fundList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Debug.Log("11 - 8 is empty");
                return;
            }

            List<GameFoundation> gfList = new List<GameFoundation>();
            foreach (var f in fundList.Values)
            {
                GameFoundation gameFoundation = new GameFoundation();
                gameFoundation.MergeFrom(f);
                gfList.Add(gameFoundation);
            }

            var shopMap = ResourcesSingletonOld.Instance.shopMap;

            if (fundList.Values.Count != shopMap.IndexModuleMap[1403].GameFoundationList.Count)
            {
                Debug.Log("11 - 8 fundationlist count is wrong");
                return;
            }

            //shopMap.IndexModuleMap.


            for (int i = 0; i < shopMap.IndexModuleMap[1403].GameFoundationList.Count; i++)
            {
                shopMap.IndexModuleMap[1403].GameFoundationList[i] = gfList[i];
            }

            UpDateNowInterface();
            ResourcesSingletonOld.Instance.UpdateResourceUI();
        }

        private void On1403GetFundRewardResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 9, On1403GetFundRewardResponse);

            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                return;
            }

            foreach (var itemstr in stringValueList.Values)
            {
                UnicornUIHelper.AddReward(UnityHelper.StrToVector3(itemstr), true);
            }

            WebMessageHandlerOld.Instance.AddHandler(11, 8, On1403QueryFundResponse);
            NetWorkManager.Instance.SendMessage(11, 8);
        }

        #endregion

        #region 1404Response

        private void On1404GetCardResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(16, 2, On1404GetCardResponse);

            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                return;
            }

            foreach (var itemstr in stringValueList.Values)
            {
                UnicornUIHelper.AddReward(UnityHelper.StrToVector3(itemstr), true);
            }


            WebMessageHandlerOld.Instance.AddHandler(CMDOld.INITPLAYER, OnRefreshAddVaule);
            NetWorkManager.Instance.SendMessage(CMDOld.INITPLAYER);
        }

        private void OnRefreshAddVaule(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.INITPLAYER, OnRefreshAddVaule);
            var gameRole = new GameRole();
            gameRole.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingletonOld.Instance.UserInfo.PatrolGainName = gameRole.PatrolGainName;
            ResourcesSingletonOld.Instance.UserInfo.AdFreeFlag = gameRole.AdFreeFlag;
            Log.Debug($"gameRole.AdFreeFlag:{gameRole.AdFreeFlag}", Color.red);
            WebMessageHandlerOld.Instance.AddHandler(16, 3, On1404QueryCardResponse);
            NetWorkManager.Instance.SendMessage(16, 3);
        }


        private void On1404BuyCardResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(16, 1, On1404BuyCardResponse);

            StringValueList stringValueList = new StringValueList();
            stringValueList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Debug.Log("16 - 1 is empty");
                return;
            }

            foreach (var itemstr in stringValueList.Values)
            {
                UnicornUIHelper.AddReward(UnityHelper.StrToVector3(itemstr), true);
            }

            WebMessageHandlerOld.Instance.AddHandler(16, 3, On1404QueryCardResponse);
            NetWorkManager.Instance.SendMessage(16, 3);
        }

        private void On1404QueryCardResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(16, 3, On1404QueryCardResponse);

            SpecialInterest specialInterest = new SpecialInterest();
            specialInterest.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                return;
            }

            ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1404].SpecialCard = specialInterest;

            ResourcesSingletonOld.Instance.UpdateResourceUI();
            UpDateNowInterface();
        }

        #endregion

        #endregion

        #region Update the current interface

        public void UpDateNowInterface()
        {
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI uuii))
            {
                SingleTabOnClick(NowPosType1, NowPosType2, Now1403FundIndex, true);
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------

        #endregion

        void OnScrollRectDragging(Vector2 delta)
        {
            bool preferHorizontal = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y); // 判断水平滑动

            if (preferHorizontal)
            {
                if (UnicornUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                {
                    var uis = ui as UIPanel_JiyuGame;
                    uis.m_IsEndMove = false;
                    uis.m_IsDrag = true;
                    float maxWidth = -Screen.width * (uis.m_AllPageCount - 1);
                    curWidth += delta.x;
                    defaultWidth = -Screen.width * uis.unlockTags.FindIndex(a => a == tagId);
                    //float targetPosX = curWidth + delta.x;
                    uis.SetScrollRectHorNorPos((curWidth + defaultWidth) / maxWidth);
                    scrollRect.Get().vertical = false;
                    //右+ 左-
                }


                //Log.Debug($"delta:{delta}");
                // 处理水平滑动
                //scrollRect.SetEnabled(false);
            }
        }

        protected override void OnClose()
        {
            //RemoveTimer();
            //RemoveRedPoint();
            ctsOld.Cancel();
            ctsOld.Dispose();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}