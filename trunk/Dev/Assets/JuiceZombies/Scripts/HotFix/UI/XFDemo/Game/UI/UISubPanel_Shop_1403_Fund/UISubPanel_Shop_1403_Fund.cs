//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_1403_Fund)]
    internal sealed class UISubPanel_Shop_1403_FundEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_1403_Fund;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_1403_Fund>();
        }
    }

    public partial class UISubPanel_Shop_1403_Fund : UI, IAwake<UI>
    {
        #region property

        private int Now1403FundIndex = -1;

        //private long Module1403_Last_Time = 0;

        public float Module1403_Last_ListH = 0;

        private List<UI> Module1403GetUIList = new List<UI>();

        private List<UI> Module1403_PointUIList = new List<UI>();

        private List<int> Module1403_NewUIIndexList = new List<int>();

        private Dictionary<int, UI> Module1403_IDToPointNewDic = new Dictionary<int, UI>();

        private bool Module1403_IsNew = false;

        private int Module1403_NowNewUIIndexListIndex = 0;

        private List<UI> Module1403_TopTab_NewUI = new List<UI>();

        private UI LastFundPoint = null;

        private CancellationTokenSource cts = new CancellationTokenSource();
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

        private float thisModuleH = 0;
        private long timerId = 0;

        #endregion

        public void Initialize(UI parent)
        {
            if (!ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1403))
            {
                Close();
            }

            this.SetParent(parent, false);
            DataInit();
            Module1403Init();
            SetUpdate();

            InitEffect();
        }

        private void InitEffect()
        {
            var shopFundListUI = this.GetFromReference(UISubPanel_Shop_1403_Fund.KPos_Help_List).GetList().Children[0];
            var itemRowList = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetList();
            int i = 0;
            foreach (var item in itemRowList.Children)
            {
                i++;
                var items = item as UISubPanel_Shop_Fund_Item;
                if (i <= 10)
                {
                    JiYuTweenHelper.SetEaseAlphaAndPosB2U(items.GetFromReference(UISubPanel_Shop_Fund_Item.Kmid),
    0, 200, cancellationToken: cts.Token, 0.35f + 0.02f * i, true, true);
                }

                //JiYuTweenHelper.ChangeSoftness(item.GetFromReference(UISubPanel_Shop_Fund_Item.Kmid), 300, 0.35f, cancellationToken: cts.Token);
            }
        }

        private void SetUpdate()
        {
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(1000, Update);
        }

        private void Update()
        {
            foreach (var getUI in Module1403GetUIList)
            {
                UIUpAndDownIn1Sec(getUI);
            }
        }

        private void UIUpAndDownIn1Sec(UI input)
        {
            input.GetRectTransform().DoAnchoredPositionY(-10, 0.5f).AddOnCompleted(() =>
            {
                input.GetRectTransform().DoAnchoredPositionY(10, 0.5f);
            });
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

        private void Module1403Init()
        {
            var a = Module1403_Help_FindFirstFund();
            Debug.Log("a.x" + a.x);
            Debug.Log("a.y" + a.y);
            Module1403_PointUIList.Clear();
            Module1403_NewUIIndexList.Clear();
            Module1403_IDToPointNewDic.Clear();
            //Module1403_Last_Time = TimeHelper.ClientNow();
            LastFundPoint = null;

            #region Determine if it is necessary to jump to another fund (like fund 2, fund 3)

            int InitFundID = a.x;
            int InitFundIndex = 0;
            //if (Now1403FundIndex < 0)
            //{

            //}
            //else
            //{
            //    InitFundIndex = Now1403FundIndex;
            //}
            //InitFundIndex = 1;
            //a.x = InitFundIndex + 1;

            #endregion

            //var shopViewList = this.GetFromReference(KScrollView).GetScrollRect().Content.GetList();

            #region top select

            Module1403_Last_ListH = 0;
            //var fundUI = shopViewList.CreateWithUIType(UIType.UISubPanel_Shop_1403_Fund, false);
            //fundUI.GetRectTransform().SetWidth(Screen.width);


            //ModuleUIAndIDDic.Add(fundUI, 1403);
            float showW = Screen.width - 47;

            if (showW <= 0)
            {
                return;
            }

            int fundNum = 0;
            int level = ResourcesSingleton.Instance.UserInfo.RoleAssets.Level;
            foreach (var tbf in tbfund.DataList)
            {
                if (level >= tbf.unlockLevel)
                {
                    fundNum++;
                }
            }
            //Debug.Log("level:" + level + "; fundnum:" + fundNum);
            //fundNum = 3;

            //this.GetFromReference(KScrollView).GetScrollRect().OnValueChanged.Add((f) =>
            //{
            //    Module1403_Last_Time = TimeHelper.ClientNow();
            //});

            //attenton!!!!!!: index != id
            int nowFundIndex = 0;

            //this.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform().SetWidth(showW * fundNum);
            this.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform().SetOffsetWithTop(0);
            this.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform().SetOffsetWithBottom(0);
            //this.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetComponent<GridLayoutGroup>().cellSize =
            //    new(showW, 237);
            var fundTitleList = this.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetList();

            var pointList = this.GetFromReference(UISubPanel_Shop_1403_Fund.KPoint_List).GetList();
            List<UI> fundPointList = new List<UI>();
            string pointSelectedColor = "FFFFFF";
            string pointNotSelectedColor = "94A3B8";

            var fundForwardSortList = new List<fund>();
            foreach (var tbf in tbfund.DataList)
            {
                fundForwardSortList.Add(tbf);
            }

            fundForwardSortList.Sort(delegate(fund f1, fund f2) { return f1.unlockLevel.CompareTo(f2.unlockLevel); });

            Dictionary<int, int> indexIDDic = new Dictionary<int, int>();
            for (int i = 0; i < fundForwardSortList.Count; i++)
            {
                indexIDDic.Add(i, fundForwardSortList[i].id);
            }

            var fundReverseSortList = new List<fund>();
            foreach (var tbf in tbfund.DataList)
            {
                fundReverseSortList.Add(tbf);
            }

            fundReverseSortList.Sort(delegate(fund f1, fund f2) { return f2.unlockLevel.CompareTo(f1.unlockLevel); });

            if (fundNum <= 1)
            {
                //no point
                this.GetFromReference(UISubPanel_Shop_1403_Fund.KScrollView).GetComponent<ScrollRect>().movementType =
                    ScrollRect.MovementType.Clamped;
            }
            else
            {
                this.GetFromReference(UISubPanel_Shop_1403_Fund.KScrollView).GetComponent<ScrollRect>().movementType =
                    ScrollRect.MovementType.Clamped;
                for (int i = 0; i < fundReverseSortList.Count; i++)
                {
                    int ihelp = i;
                    if (fundReverseSortList.Count - ihelp <= fundNum)
                    {
                        var fundPoint = pointList.CreateWithUIType(UIType.UISubPanel_Shop_Fund_Point, false);
                        Module1403_PointUIList.Add(fundPoint);
                        fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KText_New).GetTextMeshPro()
                            .SetTMPText(tblanguage.Get("text_new_1").current);

                        Module1403_IDToPointNewDic.Add(fundReverseSortList[ihelp].id,
                            fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip));

                        //Determine if there are red dots
                        bool haveRedDotOrNot = false;
                        foreach (var gf in ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList)
                        {
                            if (gf.FoundId == fundReverseSortList[ihelp].id)
                            {
                                if (gf.IsLook == 0)
                                {
                                    if (gf.FoundId == a.x)
                                    {
                                        haveRedDotOrNot = false;
                                    }
                                    else
                                    {
                                        haveRedDotOrNot = true;
                                    }
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
                            fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip).SetActive(true);
                            //fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip).SetActive(false);
                            int indexHelp = fundReverseSortList.Count - 1 - ihelp;
                            Module1403_NewUIIndexList.Add(indexHelp);
                        }
                        else
                        {
                            //no red dot
                            fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip).SetActive(false);
                        }

                        if (fundReverseSortList[ihelp].id == InitFundID)
                        {
                            fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KImg_Circle).GetImage()
                                .SetColor(HexToColor(pointSelectedColor));
                            LastFundPoint = fundPoint;
                        }
                        else
                        {
                            fundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KImg_Circle).GetImage()
                                .SetColor(HexToColor(pointNotSelectedColor));
                        }

                        fundPointList.Add(fundPoint);
                    }
                }

                if (Module1403_NewUIIndexList.Count > 1)
                {
                    for (int i = 0; i < Module1403_NewUIIndexList.Count; i++)
                    {
                        if (i == 0)
                        {
                            LastFundPoint = Module1403_PointUIList[Module1403_NewUIIndexList[i]];
                            Module1403_NowNewUIIndexListIndex = 0;
                        }
                        else
                        {
                            //Module1403_PointUIList[Module1403_NewUIIndexList[i]].GetFromReference(UISubPanel_Shop_Fund_Point.KPos_Tip).SetActive(false);
                        }
                    }
                }

                JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(UISubPanel_Shop_1403_Fund.KPoint_List));
            }

            fundPointList.Reverse();

            var fundTopScroll = this.GetFromReference(UISubPanel_Shop_1403_Fund.KScrollView).GetXScrollRect();
            fundTopScroll.OnEndDrag.Add(() =>
            {
                //Module1403_Last_Time = TimeHelper.ClientNow();
                float contentX = this.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform()
                    .AnchoredPosition().x;
                contentX = math.abs(contentX);
                int posHelpIndex = (int)(contentX / showW);
                if (contentX > posHelpIndex * showW + showW / 2)
                {
                    posHelpIndex += 1;
                }

                nowFundIndex = posHelpIndex;
                Module1403_Help_DoTabMove(fundNum, nowFundIndex, indexIDDic, showW, false);
                Module1403_Help_ChangePointState(fundPointList);
                Debug.Log("Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex)1;");
                Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex);
            });

            for (int i = 0; i < fundNum; i++)
            {
                int ihelp = i;

                var fundSelectUI = fundTitleList.CreateWithUIType(UIType.UISubPanel_Shop_Fund_Select_Tab, false);
                fundSelectUI.GetRectTransform().SetWidth(showW);

                fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KText_Desc).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(fundForwardSortList[ihelp].desc).current);
                fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KText_Prompt).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("fund_desc_tips").current);
                fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImage).GetImage()
                    .SetSpriteAsync(fundForwardSortList[ihelp].bg, true);
                fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip).SetActive(true);

                //Debug.Log("fundForwardSortList[ihelp].id" + fundForwardSortList[ihelp].id);
                //Debug.Log("indexIDDic[InitFundIndex]" + indexIDDic[InitFundIndex]);

                if (fundForwardSortList[ihelp].id == InitFundID)
                {
                    nowFundIndex = ihelp;
                    Module1403_Help_DoTabMove(fundNum, nowFundIndex, indexIDDic, showW, true);


                    bool haveRedDotOrNot = false;
                    foreach (var gf in ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList)
                    {
                        if (gf.FoundId == fundForwardSortList[ihelp].id)
                        {
                            //Debug.Log("gf.id:" + gf.FoundId + "; gf.islook:" + gf.IsLook + "; fundForwardSortList[ihelp].id:" + fundForwardSortList[ihelp].id);
                            if (gf.IsLook == 0)
                            {
                                //if (gf.FoundId == a.x)
                                //{
                                //    haveRedDotOrNot = false;
                                //}
                                //else
                                //{
                                //    haveRedDotOrNot = true;
                                //}
                                haveRedDotOrNot = true;

                                //Module1403_Help_RemoveNewByID(gf.FoundId);
                                Module1403_Help_OnlyRemoveNewResources(gf.FoundId);

                                //ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList[fundListIndex].IsLook = 1;
                            }
                            else if (gf.IsLook == 1)
                            {
                                haveRedDotOrNot = false;
                            }

                            break;
                        }
                    }
                    //Debug.Log("haveRedDotOrNot" + haveRedDotOrNot);

                    if (haveRedDotOrNot)
                    {
                        fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip).SetActive(true);

                        //Debug.Log("set img tip true" + InitFundIndex);
                    }
                    else
                    {
                        fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip).SetActive(false);
                    }
                }
                else
                {
                    fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip).SetActive(false);
                }

                Module1403_TopTab_NewUI.Add(fundSelectUI.GetFromReference(UISubPanel_Shop_Fund_Select_Tab.KImg_Tip));
            }

            JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(UISubPanel_Shop_1403_Fund.KContent));

            Debug.Log("Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex)2;");
            Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex);

            var leftBtn = this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left);
            var RightBtn = this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(leftBtn, () =>
            {
                //Module1403_Last_Time = TimeHelper.ClientNow();
                Debug.Log("left btn");
                nowFundIndex -= 1;
                Module1403_Help_DoTabMove(fundNum, nowFundIndex, indexIDDic, showW);
                Debug.Log("Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex)3;");
                Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex);
                Module1403_Help_ChangePointState(fundPointList);
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(RightBtn, () =>
            {
                //Module1403_Last_Time = TimeHelper.ClientNow();
                Debug.Log("right btn");
                nowFundIndex += 1;
                Debug.Log("right btn 1");
                Module1403_Help_DoTabMove(fundNum, nowFundIndex, indexIDDic, showW);
                Debug.Log("right btn 2");
                Debug.Log("Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex)4;");
                Module1403_Help_LeftAndRightBtnSetActive(fundNum, nowFundIndex);
                Module1403_Help_ChangePointState(fundPointList);
            });

            #endregion

            #region item list

            //Module1403_Help_CreateItem(InitFundID, fundUI);

            #endregion

            var parent = this.GetParent<UIPanel_Shop>();
            parent.SetShopContentH(237);
            thisModuleH += 237;
            //ContentH += 210;

            JumpToThisLevel(a);
        }

        private void Module1403_Help_LeftAndRightBtnSetActive(int fundNum, int nowFundIndex)
        {
            Debug.Log("Module1403_Help_LeftAndRightBtnSetActive fundnum:" + fundNum + "; nowfundindex:" + nowFundIndex);
            if (fundNum > 1)
            {
                if (nowFundIndex == 0)
                {
                    this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right).SetActive(true);
                    this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left).SetActive(false);
                }
                else if (nowFundIndex == fundNum - 1)
                {
                    this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right).SetActive(false);
                    this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left).SetActive(true);
                }
                else
                {
                    this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right).SetActive(true);
                    this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left).SetActive(true);
                }
            }
            else
            {
                this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Right).SetActive(false);
                this.GetFromReference(UISubPanel_Shop_1403_Fund.KBtn_Left).SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fundNum"></param>
        /// <param name="nowFundIndex"></param>
        /// <param name="indexIDDic"></param>
        /// <param name="showW"></param>
        /// <param name="isRefresh">是否是刷新导致的重新生成</param>
        /// <param name="isFirst"></param>
        private void Module1403_Help_DoTabMove(int fundNum, int nowFundIndex,
            Dictionary<int, int> indexIDDic, float showW, bool isRefresh = true, bool isFirst = false)
        {
            Log.Debug($"isRefresh:{isRefresh}", Color.cyan);

            this.GetFromReference(UISubPanel_Shop_1403_Fund.KContent).GetRectTransform()
                .DoAnchoredPositionX(-nowFundIndex * showW, 0.2f);
            Module1403_Help_CreateItem(indexIDDic[nowFundIndex], fundNum, isRefresh);
            int lastFundIndex = Now1403FundIndex;
            Now1403FundIndex = nowFundIndex;
            if (isFirst)
            {
            }
            else
            {
                Module1403_IsNew = false;
                foreach (var newui in Module1403_TopTab_NewUI)
                {
                    newui.SetActive(false);
                }

                if (nowFundIndex >= 0)
                {
                    Module1403_Help_OnlyRemoveNewResources(indexIDDic[nowFundIndex]);
                }

                if (lastFundIndex >= 0)
                {
                    Module1403_Help_RemoveNewByID(indexIDDic[lastFundIndex]);
                }
            }
           



        }

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

        private void Module1403_Help_ChangePointState(List<UI> fundPointList)
        {
            string pointSelectedColor = "FFFFFF";
            string pointNotSelectedColor = "94A3B8";
            if (fundPointList.Count > 0)
            {
                if (LastFundPoint != fundPointList[Now1403FundIndex])
                {
                    LastFundPoint.GetFromReference(UISubPanel_Shop_Fund_Point.KImg_Circle).GetImage()
                        .SetColor(HexToColor(pointNotSelectedColor));
                    fundPointList[Now1403FundIndex].GetFromReference(UISubPanel_Shop_Fund_Point.KImg_Circle).GetImage()
                        .SetColor(HexToColor(pointSelectedColor));
                    LastFundPoint = fundPointList[Now1403FundIndex];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fundNum"></param>
        /// <param name="isRefresh">是否是刷新导致的重新生成</param>
        private async void Module1403_Help_CreateItem(int id, int fundNum, bool isRefresh)
        {
            Module1403GetUIList.Clear();
            GameFoundation thisFundByID = null;
            var fundList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList;
            int fundListIndex = 0;
            foreach (var f in fundList)
            {
                if (f.FoundId == id)
                {
                    thisFundByID = f;
                    break;
                }

                fundListIndex++;
            }

            var parent = this.GetParent<UIPanel_Shop>();
            // if (parent.ctsOld != null)
            // {
            //     parent.ctsOld.Cancel();
            // }
            //
            // parent.ctsOld = new CancellationTokenSource();

            var fundUIItemList = this.GetFromReference(UISubPanel_Shop_1403_Fund.KPos_Help_List).GetList();
            fundUIItemList.Clear();
            var shopFundListUI = fundUIItemList.CreateWithUIType(UIType.UISubPanel_Shop_Fund_List, false);

            //if (thisFundByID.IsLook == 0)
            //{
            //    Module1403_Help_RemoveNewByID(id);
            //    ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList[fundListIndex].IsLook = 1;
            //}

            #region set fund bg

            //left
            shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Left).SetActive(false);
            shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Left).SetActive(true);
            shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Left).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_free_text").current);

            //mid
            shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Mid).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_purchased").current);
            shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Buy_Mid).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_buy").current);
            var textPriceMid = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Price_Mid);
            textPriceMid.GetTextMeshPro().SetTMPText(tbprice.Get(tbfund.Get(id).price1).rmb.ToString() +
                                                     tblanguage.Get("common_coin_unit").current);
            if (thisFundByID.RewardTwoStatus == 0)
            {
                shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Mid).SetActive(true);
                shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Mid).SetActive(false);
                var btnBuyMid = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Mid);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btnBuyMid, () =>
                {
                    const string Fund1 = "C03";
                    JiYuUIHelper.SendBuyMessage(Fund1, id);
                    JiYuEventManager.Instance.RegisterEvent("OnShopResponse", (a) =>
                    {
                        var shopNum = JiYuUIHelper.GetShopNum(a);

                        switch (shopNum)
                        {
                            case "C03":
                                WebMessageHandlerOld.Instance.AddHandler(11, 8, On1403QueryFundAndUpdateDownResponse);
                                NetWorkManager.Instance.SendMessage(11, 8);
                                break;
                        }
                    });
                });
            }
            else
            {
                shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Mid).SetActive(false);
                shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Mid).SetActive(true);
            }

            //right
            shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Right).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_purchased").current);
            shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Buy_Right).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("common_state_buy").current);
            var textPriceRight = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Price_Right);
            textPriceRight.GetTextMeshPro().SetTMPText(tbprice.Get(tbfund.Get(id).price2).rmb.ToString() +
                                                       tblanguage.Get("common_coin_unit").current);
            if (thisFundByID.RewardThreeStatus == 0)
            {
                shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Right).SetActive(true);
                shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Right).SetActive(false);
                var btnBuyRight = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Right);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btnBuyRight, () =>
                {
                    const string Fund1 = "C04";
                    JiYuUIHelper.SendBuyMessage(Fund1, id);
                    JiYuEventManager.Instance.RegisterEvent("OnShopResponse", (a) =>
                    {
                        var shopNum = JiYuUIHelper.GetShopNum(a);

                        switch (shopNum)
                        {
                            case "C04":

                                WebMessageHandlerOld.Instance.AddHandler(11, 8, On1403QueryFundAndUpdateDownResponse);
                                NetWorkManager.Instance.SendMessage(11, 8);
                                break;
                        }
                    });
                    /*
                    //Module1403_Last_Time = TimeHelper.ClientNow();
                    WebMessageHandlerOld.Instance.AddHandler(11, 11, On1403BuyFundResponse);
                    string buyFundStr = id.ToString() + ";3";
                    StringValue stringValue = new StringValue();
                    stringValue.Value = buyFundStr;
                    NetWorkManager.Instance.SendMessage(11, 11, stringValue);*/
                });
            }
            else
            {
                shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KBtn_Buy_Right).SetActive(false);
                shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KText_Name_Right).SetActive(true);
            }

            #endregion

            var thisFundRewardList = new List<fund_reward>();
            foreach (var tbfr in tbfund_Reward.DataList)
            {
                if (tbfr.id == id)
                {
                    thisFundRewardList.Add(tbfr);
                }
            }

            thisFundRewardList.Sort(delegate(fund_reward f1, fund_reward f2) { return f1.level.CompareTo(f2.level); });

            // int unLockCount = 0;
            // var level = ResourcesSingleton.Instance.UserInfo.RoleAssets.Level;
            // foreach (var tbfr in thisFundRewardList)
            // {
            //     if (tbfr.level <= level)
            //     {
            //         unLockCount++;
            //     }
            //     else
            //     {
            //         break;
            //     }
            // }
            //
            // float itemListH = thisFundRewardList.Count * 267 + (thisFundRewardList.Count - 1) * 36;

            //set bg H
            //float unlockH = unLockCount * 267 + (unLockCount - 1) * 36 + 5;
            //float lockH = 0;
            //if (unLockCount == 0)
            //{
            //    lockH = itemListH + 5;
            //    unlockH = 0;
            //}
            //else
            //{
            //    lockH = itemListH - unlockH + 5;
            //}

            //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_Item_Bg_Left).GetRectTransform()
            //    .SetHeight(unlockH);
            //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Left).GetRectTransform()
            //    .SetHeight(lockH);
            //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Left).GetRectTransform()
            //    .SetAnchoredPositionY(-lockH);
            //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_Item_Bg_Mid).GetRectTransform()
            //    .SetHeight(unlockH);
            //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Mid).GetRectTransform()
            //    .SetHeight(lockH);
            //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Mid).GetRectTransform()
            //    .SetAnchoredPositionY(-lockH);
            //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_Item_Bg_Right).GetRectTransform()
            //    .SetHeight(unlockH);
            //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Right).GetRectTransform()
            //    .SetHeight(lockH);
            //shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KImg_locked_Bg_Right).GetRectTransform()
            //    .SetAnchoredPositionY(-lockH);


            // shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetRectTransform()
            //     .SetHeight(itemListH);

            //var parent = this.GetParent<UIPanel_Shop>();
            //parent.SetShopContentH(-Module1403_Last_ListH);
            //thisModuleH -= Module1403_Last_ListH;
            //parent.SetShopContentH(itemListH + 200);
            //thisModuleH += itemListH + 200;
            //ContentH -= Module1403_Last_ListH;
            //ContentH += itemListH + 200;
            //Module1403_Last_ListH = itemListH + 200;
            //shopFundListUI.GetRectTransform().SetHeight(itemListH + 200);
           
            parent.SetModule1403RedPointState();
            await Module1403_Help_CreateOneRowItem(shopFundListUI, thisFundRewardList, thisFundByID, isRefresh);


            await UniTask.Yield();
            shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetRectTransform()
                .SetOffsetWithLeft(0);
            shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetRectTransform()
                .SetOffsetWithRight(0);

            var height1 = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KUpper).GetRectTransform().Height();

            var height2 = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetRectTransform()
                .Height();

            shopFundListUI.GetRectTransform().SetHeight(height1 + height2);
            this.GetRectTransform().SetHeight(310 + 133 + shopFundListUI.GetRectTransform().Height());
            JiYuUIHelper.ForceRefreshLayout(parent?.GetFromReference(UIPanel_Shop.KContent));
          
        }

        private async UniTask Module1403_Help_CreateOneRowItem(UI shopFundListUI,
            List<fund_reward> thisFundRewardList, GameFoundation thisFundByID, bool isRefresh)
        {
            var itemRowList = shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item).GetList();
            itemRowList.Clear();

            int level = ResourcesSingleton.Instance.UserInfo.RoleAssets.Level;

            Dictionary<int, int> leftGetDic = new Dictionary<int, int>();
            Dictionary<int, int> midGetDic = new Dictionary<int, int>();
            Dictionary<int, int> rightGetDic = new Dictionary<int, int>();

            foreach (var tbfr in thisFundRewardList)
            {
                leftGetDic.Add(tbfr.level, 0);
                midGetDic.Add(tbfr.level, 0);
                rightGetDic.Add(tbfr.level, 0);
            }

            string color1Str = "5AFF20";
            string color2Str = "FF69A5";
            string color3Str = "FFBF0F";

            var shopMap = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList;
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

            var parent = this.GetParent<UIPanel_Shop>();

            int i = 0;
            Dictionary<UI, int> helpSortDic = new Dictionary<UI, int>();
            foreach (var tbfr in thisFundRewardList)
            {
                int ihelp = i;
                var itemRow =
                    await itemRowList.CreateWithUITypeAsync(UIType.UISubPanel_Shop_Fund_Item, false, cts.Token);
                //var level = ResourcesSingleton.Instance.UserInfo.RoleAssets.Level;
                //itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.Kimg_bg)?.SetActive(false);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link).SetActive(false);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_unlock).SetActive(false);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End).SetActive(false);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End_unlock).SetActive(false);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Gap).SetActive(false);

                //已解锁
                if (tbfr.level <= level)
                {
                    if (tbfr.level + 1 == level)
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Gap).SetActive(true);
                    }

                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.Kimg_bg)?.SetActive(true);
                    //itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link)?.SetActive(false);
                    //最后一个
                    if (ihelp == thisFundRewardList.Count - 1)
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End_unlock).SetActive(true);
                    }
                    else
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_unlock)?.SetActive(true);
                    }
                }
                else
                {
                    if (ihelp == thisFundRewardList.Count - 1)
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End).SetActive(true);
                    }
                    else
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link).SetActive(true);
                    }

                    if ((tbfr.level - 1) <= level)
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End_unlock).SetActive(true);
                    }
                }


                helpSortDic.Add(itemRow, ihelp);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Level).GetTextMeshPro()
                    .SetTMPText(tbfr.level.ToString());

                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Remind_Left).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gain").current);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Remind_Mid).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gain").current);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Remind_Right).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gain").current);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Received_Left).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gained").current);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Received_Mid).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gained").current);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KText_Received_Right).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gained").current);

                var leftReward = itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KReward_Left);
                JiYuUIHelper.SetRewardIconAndCountText(tbfr.reward1[0], leftReward);
                var MidReward = itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KReward_Mid);
                JiYuUIHelper.SetRewardIconAndCountText(tbfr.reward2[0], MidReward);
                var RightReward = itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KReward_Right);
                JiYuUIHelper.SetRewardIconAndCountText(tbfr.reward3[0], RightReward);

                var leftReBtn = leftReward.GetFromReference(UICommon_RewardItem.KBtn_Item);
                var midReBtn = MidReward.GetFromReference(UICommon_RewardItem.KBtn_Item);
                var rightReBtn = RightReward.GetFromReference(UICommon_RewardItem.KBtn_Item);

                //if (ihelp == thisFundRewardList.Count - 1)
                //{
                //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link_End).SetActive(true);
                //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link).SetActive(false);
                //}


                //if (ihelp == 0)
                //{
                //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link).SetActive(false);
                //}
                //else
                //{
                //    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Link).SetActive(true);
                //}


                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskRight).SetActive(false);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskLeft).SetActive(false);
                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskMid).SetActive(false);

                if (tbfr.level <= level)
                {
                    //leftReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage()
                    //    .SetColor(HexToColor(color1Str));
                    //MidReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetColor(HexToColor(color2Str));
                    //RightReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage()
                    //    .SetColor(HexToColor(color3Str));

                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Left).SetActive(false);
                    if (leftGetDic[tbfr.level] == 0)
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Left).SetActive(false);
                        //Debug.Log(tbfr.level.ToString() + "set receive false");
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Left).SetActive(true);
                        Module1403GetUIList.Add(itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Left));
                        leftReBtn.GetXButton().OnClick.Add(() =>
                        {
                            if (itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskLeft).GameObject.activeSelf)
                            {
                                JiYuUIHelper.SetRewardOnClick(tbfr.reward1[0], leftReward);
                            }
                            else
                            {
                                WebMessageHandlerOld.Instance.AddHandler(11, 9, On1403GetFundRewardResponse);
                                string reStr = tbfr.id.ToString() + ";1;" + tbfr.level.ToString();
                                StringValue stringValue = new StringValue();
                                stringValue.Value = reStr;
                                NetWorkManager.Instance.SendMessage(11, 9, stringValue);
                            }
                        });
                    }
                    else
                    {
                        //have Received
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Left).SetActive(true);
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskLeft).SetActive(true);
                        //Debug.Log(tbfr.level.ToString() + "set receive true");
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Left).SetActive(false);
                        leftReBtn.GetXButton().OnClick.Add(() =>
                        {
                            if (itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskLeft).GameObject.activeSelf)
                            {
                                JiYuUIHelper.SetRewardOnClick(tbfr.reward1[0], leftReward);
                            }

                            JiYuUIHelper.ClearCommonResource();
                            //Module1403_Last_Time = TimeHelper.ClientNow();
                            UIHelper.CreateAsync(UIType.UICommon_Resource,
                                tblanguage.Get("common_state_gained_reward").current);
                        });
                    }

                    if (thisFundByID.RewardTwoStatus == 0)
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Mid).SetActive(true);
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskMid).SetActive(true);
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid).SetActive(false);
                        midReBtn.GetXButton().OnClick.Add(() =>
                        {
                            if (itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskMid).GameObject.activeSelf)
                            {
                                JiYuUIHelper.SetRewardOnClick(tbfr.reward2[0], MidReward);
                            }

                            JiYuUIHelper.ClearCommonResource();
                            //Module1403_Last_Time = TimeHelper.ClientNow();
                            UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("fund_unlock_tips").current);
                        });
                    }
                    else
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Mid).SetActive(false);
                        if (midGetDic[tbfr.level] == 0)
                        {
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Mid).SetActive(false);
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid).SetActive(true);
                            Module1403GetUIList.Add(
                                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid));
                            midReBtn.GetXButton().OnClick.Add(() =>
                            {
                                if (itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskMid).GameObject
                                    .activeSelf)
                                {
                                    JiYuUIHelper.SetRewardOnClick(tbfr.reward2[0], MidReward);
                                }
                                else
                                {
                                    //Module1403_Last_Time = TimeHelper.ClientNow();
                                    //get reward
                                    WebMessageHandlerOld.Instance.AddHandler(11, 9, On1403GetFundRewardResponse);
                                    string reStr = tbfr.id.ToString() + ";2;" + tbfr.level.ToString();
                                    StringValue stringValue = new StringValue();
                                    stringValue.Value = reStr;
                                    NetWorkManager.Instance.SendMessage(11, 9, stringValue);
                                }
                            });
                        }
                        else
                        {
                            //have Received
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Mid).SetActive(true);
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskMid).SetActive(true);
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid).SetActive(false);
                            midReBtn.GetXButton().OnClick.Add(() =>
                            {
                                if (itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskMid).GameObject
                                    .activeSelf)
                                {
                                    JiYuUIHelper.SetRewardOnClick(tbfr.reward2[0], MidReward);
                                }

                                JiYuUIHelper.ClearCommonResource();
                                //Module1403_Last_Time = TimeHelper.ClientNow();
                                UIHelper.CreateAsync(UIType.UICommon_Resource,
                                    tblanguage.Get("common_state_gained_reward").current);
                            });
                        }
                    }

                    if (thisFundByID.RewardThreeStatus == 0)
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Right).SetActive(true);
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskRight).SetActive(true);
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right).SetActive(false);
                        rightReBtn.GetXButton().OnClick.Add(() =>
                        {
                            if (itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskRight).GameObject.activeSelf)
                            {
                                JiYuUIHelper.SetRewardOnClick(tbfr.reward3[0], RightReward);
                            }

                            JiYuUIHelper.ClearCommonResource();
                            //Module1403_Last_Time = TimeHelper.ClientNow();
                            UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("fund_unlock_tips").current);
                        });
                    }
                    else
                    {
                        itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Right).SetActive(false);
                        if (rightGetDic[tbfr.level] == 0)
                        {
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Right).SetActive(false);
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right).SetActive(true);
                            Module1403GetUIList.Add(
                                itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right));
                            rightReBtn.GetXButton().OnClick.Add(() =>
                            {
                                if (itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskRight).GameObject
                                    .activeSelf)
                                {
                                    JiYuUIHelper.SetRewardOnClick(tbfr.reward3[0], RightReward);
                                }
                                else
                                {
                                    //Module1403_Last_Time = TimeHelper.ClientNow();
                                    //get reward
                                    WebMessageHandlerOld.Instance.AddHandler(11, 9, On1403GetFundRewardResponse);
                                    string reStr = tbfr.id.ToString() + ";3;" + tbfr.level.ToString();
                                    StringValue stringValue = new StringValue();
                                    stringValue.Value = reStr;
                                    NetWorkManager.Instance.SendMessage(11, 9, stringValue);
                                }
                            });
                        }
                        else
                        {
                            //have Received
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Right).SetActive(true);
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskRight).SetActive(true);
                            itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right).SetActive(false);
                            rightReBtn.GetXButton().OnClick.Add(() =>
                            {
                                if (itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskRight).GameObject
                                    .activeSelf)
                                {
                                    JiYuUIHelper.SetRewardOnClick(tbfr.reward3[0], RightReward);
                                }

                                JiYuUIHelper.ClearCommonResource();
                                UIHelper.CreateAsync(UIType.UICommon_Resource,
                                    tblanguage.Get("common_state_gained_reward").current);
                            });
                        }
                    }
                }
                else
                {
                    leftReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetColor(Color.white);
                    MidReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetColor(Color.white);
                    RightReward.GetFromReference(UICommon_RewardItem.KBg_Item).GetImage().SetColor(Color.white);

                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Left).SetActive(true);
                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Mid).SetActive(true);
                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Lock_Right).SetActive(true);
                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskLeft).SetActive(true);
                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskMid).SetActive(true);
                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImgMaskRight).SetActive(true);


                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Left).SetActive(false);
                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Mid).SetActive(false);
                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Remind_Right).SetActive(false);

                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Left).SetActive(false);
                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Mid).SetActive(false);
                    itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KImg_Received_Right).SetActive(false);


                    //if (tbfr.reward1[0][0] == 3)
                    //{
                    //    leftReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
                    //        .SetSprite("gold_grey", false);
                    //}
                    //else
                    //{
                    //    leftReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
                    //        .SetSprite("icon_diamond_grey", false);
                    //}

                    //if (tbfr.reward2[0][0] == 3)
                    //{
                    //    MidReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
                    //        .SetSprite("gold_grey", false);
                    //}
                    //else
                    //{
                    //    MidReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
                    //        .SetSprite("icon_diamond_grey", false);
                    //}

                    //if (tbfr.reward3[0][0] == 3)
                    //{
                    //    RightReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
                    //        .SetSprite("gold_grey", false);
                    //}
                    //else
                    //{
                    //    RightReward.GetFromReference(UICommon_RewardItem.KImg_ItemIcon).GetImage()
                    //        .SetSprite("icon_diamond_grey", false);
                    //}

                    leftReBtn.GetXButton().OnClick.Add(() =>
                    {
                        JiYuUIHelper.ClearCommonResource();
                        //Module1403_Last_Time = TimeHelper.ClientNow();
                        UIHelper.CreateAsync(UIType.UICommon_Resource,
                            tblanguage.Get("common_state_level_limit").current);
                    });
                    midReBtn.GetXButton().OnClick.Add(() =>
                    {
                        JiYuUIHelper.ClearCommonResource();
                        //Module1403_Last_Time = TimeHelper.ClientNow();
                        UIHelper.CreateAsync(UIType.UICommon_Resource,
                            tblanguage.Get("common_state_level_limit").current);
                    });
                    rightReBtn.GetXButton().OnClick.Add(() =>
                    {
                        JiYuUIHelper.ClearCommonResource();
                        //Module1403_Last_Time = TimeHelper.ClientNow();
                        UIHelper.CreateAsync(UIType.UICommon_Resource,
                            tblanguage.Get("common_state_level_limit").current);
                    });
                }


                var left = itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KReward_Left).GetChild("Btn_Item");
                var mid = itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KReward_Mid).GetChild("Btn_Item");
                var right = itemRow.GetFromReference(UISubPanel_Shop_Fund_Item.KReward_Right).GetChild("Btn_Item");
                if (!isRefresh)
                {
                    JiYuTweenHelper.SetEaseAlphaAndPosB2U(left, 0, 50, cts.Token, 0.35f, false, false);
                    JiYuTweenHelper.SetEaseAlphaAndPosB2U(mid, 0, 50, cts.Token, 0.35f, false, false);
                    JiYuTweenHelper.SetEaseAlphaAndPosB2U(right, 0, 50, cts.Token, 0.35f, false, false);
                    JiYuTweenHelper.ChangeSoftness(left, 300, 0.2f, cancellationToken: cts.Token);
                    JiYuTweenHelper.ChangeSoftness(right, 300, 0.2f, cancellationToken: cts.Token);
                    JiYuTweenHelper.ChangeSoftness(mid, 300, 0.2f, cancellationToken: cts.Token);
                }


                //JiYuUIHelper.ChangePaddingLR(this, 50, 0.2f);

                i++;
            }

            itemRowList.Sort(delegate(UI ui1, UI ui2) { return helpSortDic[ui1].CompareTo(helpSortDic[ui2]); });

            JiYuUIHelper.ForceRefreshLayout(shopFundListUI.GetFromReference(UISubPanel_Shop_Fund_List.KPos_Item));
            var children = itemRowList.Children;
            //foreach ( UI child in children)
            //{
            //    JiYuTweenHelper.SetEaseAlphaAndPosB2U()
            //}
        }

        private void Module1403_Help_RemoveNewByID(int id)
        {
            //Module1403_Help_OnlyRemoveNewResources(id);
            if (Module1403_IDToPointNewDic.Count > 0)
            {
                Module1403_IDToPointNewDic[id].SetActive(false);
            }
        }

        private void Module1403_Help_OnlyRemoveNewResources(int id)
        {
            for (int i = 0; i < ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList.Count; i++)
            {
                if (ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList[i].FoundId == id)
                {
                    if (ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList[i].IsLook == 0)
                    {
                        ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList[i].IsLook = 1;
                        IntValue intValue = new IntValue();
                        intValue.Value = id;
                        WebMessageHandlerOld.Instance.AddHandler(11, 15, On1403LookFundResponse);
                        NetWorkManager.Instance.SendMessage(11, 15, intValue);
                    }

                    break;
                }
            }
        }

        private int2 Module1403_Help_FindFirstFund()
        {
            int2 fundIdAndLevel = new int2();

            var GameFoundationList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList;
            List<GameFoundation> fundList = new List<GameFoundation>();
            foreach (var f in GameFoundationList)
            {
                fundList.Add(f);
            }

            int level = ResourcesSingleton.Instance.UserInfo.RoleAssets.Level;

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
                        foreach (var r in f.RewardThreeList)
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

                    int final1 = math.min(one, two);
                    int final = math.min(final1, three);
                    fundIdAndLevel.y = final;
                }
            }

            return fundIdAndLevel;
        }

        private void On1403LookFundResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 15, On1403LookFundResponse);
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
                WebMessageHandlerOld.Instance.AddHandler(11, 8, On1403QueryFundAndUpdateDownResponse);
                NetWorkManager.Instance.SendMessage(11, 8);
            }
            else
            {
                //defeat
            }
        }

        private void On1403QueryFundAndUpdateDownResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(11, 8, On1403QueryFundAndUpdateDownResponse);

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

            var shopMap = ResourcesSingleton.Instance.shopMap;

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

            //UpDateNowInterface();
            //ResourcesSingleton.Instance.UpdateResourceUI();
            UpdateDown();
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

            var shopMap = ResourcesSingleton.Instance.shopMap;

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

            //UpDateNowInterface();
            ResourcesSingleton.Instance.UpdateResourceUI();
            //Debug.Log("ResourcesSingleton.Instance.UpdateResourceUI()");
        }

        private void UpdateDown()
        {
            int fundNum = 0;
            int level = ResourcesSingleton.Instance.UserInfo.RoleAssets.Level;
            foreach (var tbf in tbfund.DataList)
            {
                if (level >= tbf.unlockLevel)
                {
                    fundNum++;
                }
            }

            var fundForwardSortList = new List<fund>();
            foreach (var tbf in tbfund.DataList)
            {
                fundForwardSortList.Add(tbf);
            }

            fundForwardSortList.Sort(delegate(fund f1, fund f2) { return f1.unlockLevel.CompareTo(f2.unlockLevel); });

            Dictionary<int, int> indexIDDic = new Dictionary<int, int>();
            for (int i = 0; i < fundForwardSortList.Count; i++)
            {
                indexIDDic.Add(i, fundForwardSortList[i].id);
            }

            float showW = Screen.width - 90;

            Module1403_Help_DoTabMove(fundNum, Now1403FundIndex, indexIDDic, showW, true);
        }

        private void UpdateUp()
        {
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
                JiYuUIHelper.AddReward(UnityHelper.StrToVector3(itemstr), true);
            }

            WebMessageHandlerOld.Instance.AddHandler(11, 8, On1403QueryFundAndUpdateDownResponse);
            NetWorkManager.Instance.SendMessage(11, 8);
        }

        private void JumpToThisLevel(int2 input)
        {
            var parent = this.GetParent<UIPanel_Shop>();
            float scrollViewH = parent.GetFromReference(UIPanel_Shop.KScrollView).GetRectTransform().Height();
            //float contentH = parent.GetFromReference(UIPanel_Shop.KContent).GetRectTransform().Height();
            float contenPosY = parent.GetFromReference(UIPanel_Shop.KContent).GetRectTransform().AnchoredPosition().y;
            float needPosY1 = this.GetFromReference(KPos_Help_Top).GetRectTransform().Height();
            float needPosY2 = 195;
            float unLockCount = 0;
            foreach (var gf in tbfund_Reward.DataList)
            {
                if (gf.level <= input.y && gf.id == input.x)
                {
                    unLockCount++;
                }
            }

            float needPosY3 = unLockCount * 267 + (unLockCount - 1) * 36 + 5;

            bool isInScreenOrNot = false;
            if (contenPosY + needPosY1 + needPosY2 + needPosY3 < scrollViewH)
            {
                isInScreenOrNot = true;
            }
            else
            {
                isInScreenOrNot = false;
            }

            if (isInScreenOrNot)
            {
            }
            else
            {
                //if (unLockCount > 0)
                //{
                //    needPosY3 = (unLockCount - 1) * 267 + (unLockCount - 2) * 36 + 5;
                //}

                if (contenPosY + needPosY1 + needPosY2 + needPosY3 + scrollViewH / 2 > thisModuleH)
                {
                    parent.GetFromReference(UIPanel_Shop.KContent).GetRectTransform()
                        .SetAnchoredPositionY(thisModuleH - scrollViewH);
                }
                else
                {
                    parent.GetFromReference(UIPanel_Shop.KContent).GetRectTransform()
                        .SetAnchoredPositionY(contenPosY + needPosY1 + needPosY2 + needPosY3 - scrollViewH / 2);
                }
                //parent.GetFromReference(UIPanel_Shop.KContent).GetRectTransform().SetAnchoredPositionY(contenPosY + needPosY1 + needPosY2 + needPosY3 - scrollViewH / 2);
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

        protected override void OnClose()
        {
            RemoveTimer();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}