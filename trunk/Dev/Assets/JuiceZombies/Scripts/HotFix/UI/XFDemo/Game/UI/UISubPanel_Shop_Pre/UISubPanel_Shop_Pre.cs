//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Pre)]
    internal sealed class UISubPanel_Shop_PreEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Pre;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Pre>();
        }
    }

    public partial class UISubPanel_Shop_Pre : UI, IAwake<int>
    {
        #region 参数

        private Tbdraw_banner tbdraw_Banner;
        private List<UI> bgList = new List<UI>();
        public List<UI> helpList = new List<UI>();
        private int ModuleDeltaY = 20;
        private Tbdraw_box tbdraw_Box;
        private Tblanguage tblanguage;
        private UI lastBtn;
        private UI BottomUI;
        private UI lastRowUI = null;
        private UI lastItemUI = null;
        private UI lastBg = null;
        private int lastRowIndex = -1;
        private float tipH = 0;
        private List<UI> BtnList = new List<UI>();
        private float cellSize = 192;
        private float Hspacing = 40;
        private float Wspacing = 30;
        private int rowInt = 1;

        #endregion

        public async void Initialize(int BoxID)
        {
            await JiYuUIHelper.InitBlur(this);
            if (JiYuUIHelper.TryGetUI(UIType.UICommon_ResourceNotEnough, out UI ui))
            {
                ui.Dispose();
            }


            tbdraw_Box = ConfigManager.Instance.Tables.Tbdraw_box;

            tbdraw_Banner = ConfigManager.Instance.Tables.Tbdraw_banner;

            tblanguage = ConfigManager.Instance.Tables.Tblanguage;

            rowInt = (int)math.floor(this.GetFromReference(KScrollView).GetRectTransform().Width() /
                                     (cellSize + Wspacing));
            if (rowInt <= 0)
            {
                rowInt = 1;
            }

            PreInit(BoxID).Forget();

            BottomInit(BoxID);
        }

        #region 上一次代码

        //private async UniTask PreInit(int BoxID, bool inThisView = false, bool haveBlank = false, int previousRow = 0, draw_banner db = null, float tipsH = 0)
        //{
        //    List<draw_banner> banners = new List<draw_banner>();
        //    foreach (var banner in tbdraw_Banner.DataList)
        //    {
        //        if (banner.id == BoxID)
        //        {
        //            banners.Add(banner);
        //        }
        //    }
        //    banners.Sort(delegate (draw_banner banner1, draw_banner banner2) { return banner2.quality.CompareTo(banner1.quality); });

        //    var ContentList = this.GetFromReference(KScrollView).GetScrollRect().Content.GetList();
        //    ContentList.Clear();
        //    float contentWidth = this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().Width();
        //    Dictionary<UI, int> sortHelpDic = new Dictionary<UI, int>();

        //    int i = 0;
        //    float allHeight = 0;
        //    foreach (var banner in banners)
        //    {
        //        List<int> BoxInt = new List<int>();
        //        BoxInt.Add(BoxID);
        //        BoxInt.Add(banner.quality);
        //        var ui = await ContentList.CreateWithUITypeAsync<List<int>>(UIType.UISubPanel_Shop_PreBg, BoxInt, false);
        //        ui.GetRectTransform().SetWidth(contentWidth);

        //        int quality = banner.quality;
        //        if (quality > 50)
        //        {
        //            quality -= 50;
        //        }
        //        int reCount = banner.info.Count;

        //        List<Vector3> reList = new List<Vector3>();
        //        for (int j = 0; j < reCount; j++)
        //        {
        //            int jhelp = j;
        //            Vector3 vector3 = new Vector3();
        //            vector3[0] = 11;
        //            vector3[1] = banner.info[jhelp];
        //            vector3[2] = quality;
        //            reList.Add(vector3);
        //        }

        //        JiYuUIHelper.SortRewards(reList);

        //        int reHelpCount = reCount / rowInt;
        //        if (reCount % rowInt > 0)
        //        {
        //            reHelpCount++;
        //        }
        //        var thisBgList = ui.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetList();
        //        thisBgList.Clear();
        //        Dictionary<UI, int> UISortHelpDic = new Dictionary<UI, int>();

        //        if (haveBlank && banner == db)
        //        {
        //            if (haveBlank)
        //            {
        //                reHelpCount += 1;
        //            }

        //            for (int j = 0; j < reHelpCount; j++)
        //            {
        //                int jhelp = j;
        //                var uihelp = await thisBgList.CreateWithUITypeAsync(UIType.UISubPanel_Shop_Pre_Row, false);
        //                uihelp.GetRectTransform().SetWidth(contentWidth);
        //                uihelp.GetRectTransform().SetHeight(cellSize);
        //                UISortHelpDic.Add(uihelp, j);
        //                List<Vector3> rowHelpList = new List<Vector3>();
        //                if (haveBlank)
        //                {
        //                    if (j == previousRow + 1)
        //                    {
        //                        rowHelpList = new List<Vector3>();
        //                        uihelp.GetRectTransform().SetHeight(tipsH);
        //                    }
        //                    else
        //                    {
        //                        if (j > previousRow + 1)
        //                        {
        //                            jhelp -= 1;
        //                            for (int k = 0; k < rowInt; k++)
        //                            {
        //                                int khelp = k;
        //                                if (khelp + (j - 1) * rowInt < reCount)
        //                                {
        //                                    rowHelpList.Add(reList[khelp + (j - 1) * rowInt]);
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            for (int k = 0; k < rowInt; k++)
        //                            {
        //                                int khelp = k;
        //                                if (khelp + j * rowInt < reCount)
        //                                {
        //                                    rowHelpList.Add(reList[khelp + j * rowInt]);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    for (int k = 0; k < rowInt; k++)
        //                    {
        //                        int khelp = k;
        //                        if (khelp + j * rowInt < reCount)
        //                        {
        //                            rowHelpList.Add(reList[khelp + j * rowInt]);
        //                        }
        //                    }
        //                }
        //                var uiHelpHorizontalLayoutGroup = uihelp.GetFromReference(UISubPanel_Shop_Pre_Row.KBg).GetComponent<HorizontalLayoutGroup>();
        //                uiHelpHorizontalLayoutGroup.spacing = Wspacing;
        //                if (rowInt >= 1)
        //                {
        //                    uiHelpHorizontalLayoutGroup.padding.left = (int)((contentWidth - rowInt * (cellSize + Wspacing) + Wspacing) / 2) + 1;
        //                }
        //                else
        //                {
        //                    uiHelpHorizontalLayoutGroup.padding.left = 0;
        //                }
        //                CreateReward(uihelp.GetFromReference(UISubPanel_Shop_Pre_Row.KBg), rowHelpList, BoxID, jhelp, banner).Forget();
        //            }
        //        }
        //        else
        //        {
        //            for (int j = 0; j < reHelpCount; j++)
        //            {
        //                int jhelp = j;
        //                var uihelp = await thisBgList.CreateWithUITypeAsync(UIType.UISubPanel_Shop_Pre_Row, false);
        //                uihelp.GetRectTransform().SetWidth(contentWidth);
        //                uihelp.GetRectTransform().SetHeight(cellSize);
        //                UISortHelpDic.Add(uihelp, j);
        //                List<Vector3> rowHelpList = new List<Vector3>();

        //                for (int k = 0; k < rowInt; k++)
        //                {
        //                    int khelp = k;
        //                    if (khelp + j * rowInt < reCount)
        //                    {
        //                        rowHelpList.Add(reList[khelp + j * rowInt]);
        //                    }
        //                }

        //                var uiHelpHorizontalLayoutGroup = uihelp.GetFromReference(UISubPanel_Shop_Pre_Row.KBg).GetComponent<HorizontalLayoutGroup>();
        //                uiHelpHorizontalLayoutGroup.spacing = Wspacing;
        //                if (rowInt >= 1)
        //                {
        //                    uiHelpHorizontalLayoutGroup.padding.left = (int)((contentWidth - rowInt * (cellSize + Wspacing) + Wspacing) / 2) + 1;
        //                }
        //                else
        //                {
        //                    uiHelpHorizontalLayoutGroup.padding.left = 0;
        //                }
        //                CreateReward(uihelp.GetFromReference(UISubPanel_Shop_Pre_Row.KBg), rowHelpList, BoxID, jhelp, banner).Forget();
        //            }
        //        }


        //        thisBgList.Sort((UI ui1, UI ui2) =>
        //        {
        //            if (UISortHelpDic[ui1] <= UISortHelpDic[ui2])
        //            {
        //                return -1;
        //            }
        //            else
        //            {
        //                return 1;
        //            }
        //        });
        //        JiYuUIHelper.ForceRefreshLayout(ui.GetFromReference(UISubPanel_Shop_PreBg.KBg));

        //        if (haveBlank && banner == db)
        //        {
        //            ui.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetHeight(reHelpCount * (cellSize + Hspacing) + tipsH - cellSize);
        //        }
        //        else
        //        {
        //            ui.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetHeight(reHelpCount * (cellSize + Hspacing));
        //        }
        //        ui.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetOffsetWithLeft(0);
        //        ui.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetOffsetWithRight(0);

        //        //判断当前tips是否完全在屏幕内
        //        if (haveBlank && banner == db)
        //        {
        //            float tipsScrollH = allHeight + ui.GetFromReference(UISubPanel_Shop_PreBg.KTitleImg).GetRectTransform().Height() + Hspacing
        //                + (previousRow + 1) * (cellSize + Hspacing) + tipsH;
        //            float scrollHHelp = this.GetFromReference(KScrollView).GetRectTransform().Height() +
        //                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().AnchoredPosition().y;
        //            if (tipsScrollH > scrollHHelp)
        //            {
        //                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().DoAnchoredPositionY(tipsScrollH - this.GetFromReference(KScrollView).GetRectTransform().Height(), 0.12f);
        //            }
        //        }

        //        if (haveBlank && banner == db)
        //        {
        //            ui.GetRectTransform().SetHeight(reHelpCount * (cellSize + Hspacing) + ui.GetFromReference(UISubPanel_Shop_PreBg.KTitleImg).GetRectTransform().Height() + tipsH - cellSize);
        //            allHeight += reHelpCount * (cellSize + Hspacing) + ui.GetFromReference(UISubPanel_Shop_PreBg.KTitleImg).GetRectTransform().Height() + Hspacing + tipsH - cellSize;
        //        }
        //        else
        //        {
        //            ui.GetRectTransform().SetHeight(reHelpCount * (cellSize + Hspacing) + ui.GetFromReference(UISubPanel_Shop_PreBg.KTitleImg).GetRectTransform().Height());
        //            allHeight += reHelpCount * (cellSize + Hspacing) + ui.GetFromReference(UISubPanel_Shop_PreBg.KTitleImg).GetRectTransform().Height() + Hspacing;
        //        }

        //        sortHelpDic.Add(ui, i);
        //        JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);
        //        i++;
        //    }
        //    ContentList.Sort((UI ui1, UI ui2) =>
        //    {
        //        if (sortHelpDic[ui1] <= sortHelpDic[ui2])
        //        {
        //            return -1;
        //        }
        //        else
        //        {
        //            return 1;
        //        }
        //    });

        //    float ScrollH = this.GetFromReference(KScrollView).GetRectTransform().Height();
        //    this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetHeight(allHeight);
        //    this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithLeft(0);
        //    this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithRight(0);
        //    if (allHeight <= ScrollH)
        //    {
        //        this.GetFromReference(KScrollView).GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
        //    }
        //    else
        //    {
        //        this.GetFromReference(KScrollView).GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;
        //    }
        //    if (!inThisView)
        //    {
        //        this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetAnchoredPositionY(0);
        //    }
        //}

        #endregion

        #region 修改代码

        private async UniTask PreInit(int BoxID)
        {
            bgList.Clear();

            List<draw_banner> banners = new List<draw_banner>();
            foreach (var banner in tbdraw_Banner.DataList)
            {
                if (banner.id == tbdraw_Box.Get(BoxID).dropBannerId)
                {
                    banners.Add(banner);
                }
            }

            banners.Sort(delegate(draw_banner banner1, draw_banner banner2)
            {
                return banner2.quality.CompareTo(banner1.quality);
            });

            var ContentList = this.GetFromReference(KScrollView).GetScrollRect().Content.GetList();
            ContentList.Clear();
            float contentWidth = this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().Width();
            Dictionary<UI, int> sortHelpDic = new Dictionary<UI, int>();

            int i = 0;
            float allHeight = 0;
            foreach (var banner in banners)
            {
                int ihelp = i;
                List<int> BoxInt = new List<int>();
                BoxInt.Add(tbdraw_Box.Get(BoxID).dropBannerId);
                BoxInt.Add(banner.quality);
                var ui = await ContentList.CreateWithUITypeAsync<List<int>>(UIType.UISubPanel_Shop_PreBg, BoxInt,
                    false);
                bgList.Add(ui);
                ui.GetRectTransform().SetWidth(contentWidth);

                int quality = banner.quality;
                if (quality > 50)
                {
                    quality -= 50;
                }

                int reCount = banner.info.Count;

                List<Vector3> reList = new List<Vector3>();
                for (int j = 0; j < reCount; j++)
                {
                    int jhelp = j;
                    Vector3 vector3 = new Vector3();
                    vector3[0] = 11;
                    vector3[1] = banner.info[jhelp];
                    vector3[2] = quality;
                    reList.Add(vector3);
                }

                JiYuUIHelper.SortRewards(reList);

                int reHelpCount = reCount / rowInt;
                if (reCount % rowInt > 0)
                {
                    reHelpCount++;
                }

                var thisBgList = ui.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetList();
                thisBgList.Clear();
                Dictionary<UI, int> UISortHelpDic = new Dictionary<UI, int>();


                for (int j = 0; j < reHelpCount; j++)
                {
                    int jhelp = j;
                    var uihelp = await thisBgList.CreateWithUITypeAsync(UIType.UISubPanel_Shop_Pre_Row, false);
                    uihelp.GetRectTransform().SetWidth(contentWidth);
                    uihelp.GetRectTransform().SetHeight(cellSize);
                    UISortHelpDic.Add(uihelp, j);
                    List<Vector3> rowHelpList = new List<Vector3>();

                    for (int k = 0; k < rowInt; k++)
                    {
                        int khelp = k;
                        if (khelp + j * rowInt < reCount)
                        {
                            rowHelpList.Add(reList[khelp + j * rowInt]);
                        }
                    }

                    var uiHelpHorizontalLayoutGroup = uihelp.GetFromReference(UISubPanel_Shop_Pre_Row.KBg)
                        .GetComponent<HorizontalLayoutGroup>();
                    uiHelpHorizontalLayoutGroup.spacing = Wspacing;
                    if (rowInt >= 1)
                    {
                        uiHelpHorizontalLayoutGroup.padding.left =
                            (int)((contentWidth - rowInt * (cellSize + Wspacing) + Wspacing) / 2) + 1;
                    }
                    else
                    {
                        uiHelpHorizontalLayoutGroup.padding.left = 0;
                    }

                    CreateReward(uihelp.GetFromReference(UISubPanel_Shop_Pre_Row.KBg), rowHelpList,
                        ui as UISubPanel_Shop_PreBg, jhelp, ihelp).Forget();
                }


                thisBgList.Sort((UI ui1, UI ui2) =>
                {
                    if (UISortHelpDic[ui1] <= UISortHelpDic[ui2])
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                });
                JiYuUIHelper.ForceRefreshLayout(ui.GetFromReference(UISubPanel_Shop_PreBg.KBg));

                ui.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform()
                    .SetHeight(reHelpCount * (cellSize + Hspacing));

                ui.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetOffsetWithLeft(0);
                ui.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetOffsetWithRight(0);

                ui.GetRectTransform().SetHeight(reHelpCount * (cellSize + Hspacing) +
                                                ui.GetFromReference(UISubPanel_Shop_PreBg.KTitleImg).GetRectTransform()
                                                    .Height());
                ui.GetRectTransform().SetWidth(contentWidth);
                allHeight += reHelpCount * (cellSize + Hspacing) +
                             ui.GetFromReference(UISubPanel_Shop_PreBg.KTitleImg).GetRectTransform().Height() +
                             Hspacing;

                sortHelpDic.Add(ui, i);
                JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);
                i++;
            }

            ContentList.Sort((UI ui1, UI ui2) =>
            {
                if (sortHelpDic[ui1] <= sortHelpDic[ui2])
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });

            float ScrollH = this.GetFromReference(KScrollView).GetRectTransform().Height();
            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetHeight(allHeight);
            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithLeft(0);
            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithRight(0);
            if (allHeight <= ScrollH)
            {
                this.GetFromReference(KScrollView).GetComponent<ScrollRect>().movementType =
                    ScrollRect.MovementType.Clamped;
            }
            else
            {
                this.GetFromReference(KScrollView).GetComponent<ScrollRect>().movementType =
                    ScrollRect.MovementType.Elastic;
            }

            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetAnchoredPositionY(0);
        }

        #endregion

        private async void BottomInit(int BoxID)
        {
            var bottomList = this.GetFromReference(KPos_Bottom).GetList();
            bottomList.Clear();
            var bottomUi = await bottomList.CreateWithUITypeAsync(UIType.UICommon_Bottom, false);
            BottomUI = bottomUi;
            //bottomUi.GetRectTransform().SetAnchoredPosition(Vector2.zero);
            //bottomUi.GetImage().SetAlpha(0);
            var horizontalUi = bottomUi.GetFromReference(UICommon_Bottom.KScrollView_Item0);
            var backUi = bottomUi.GetFromReference(UICommon_Bottom.KBtn_Close);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(backUi, () => { Close(); });
            //int tbdbCount = tbdraw_Box.DataList.Count;
            var scrollRect = horizontalUi.GetScrollRect();

            var horizontalList = horizontalUi.GetScrollRect().Content.GetList();
            horizontalList.Clear();
            var idList = GetBoxList();

            for (int i = 0; i < idList.Count; i++)
            {
                int ihelp = i;
                var itemUi = await horizontalList.CreateWithUITypeAsync(UIType.UICommon_BottomBtn, false);
                itemUi.GetImage().SetColor("B9B9B9");
                //BtnList.Add(itemUi);
                if (idList[ihelp] == BoxID)
                {
                    lastBtn = itemUi;
                    itemUi.GetImage().SetColor("FFFFFF");
                    scrollRect.SetHorizontalNormalizedPosition((i + 1) / (float)idList.Count);
                    //itemUi.GetRectTransform().SetScale(new Vector2(1.1f, 1.1f));
                }

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(itemUi, () =>
                {
                    if (lastBtn != itemUi)
                    {
                        lastRowUI = null;
                        lastItemUI = null;
                        lastBg = null;
                        //lastBtn.GetRectTransform().DoScale2(new Vector2(1f, 1f), 0.2f);
                        lastBtn.GetImage().SetColor("B9B9B9");
                        itemUi.GetImage().SetColor("FFFFFF");
                        //DestroyList();
                        PreInit(idList[ihelp]);
                        lastBtn = itemUi;
                    }
                }, 1011);

                var text = itemUi.GetFromReference(UICommon_BottomBtn.KText_Btn);
                //Debug.LogError($"盲盒id:{idList[ihelp]}");
                text.GetTextMeshPro().SetTMPText(tblanguage.Get(tbdraw_Box.Get(idList[ihelp]).name).current);
            }
        }

        private List<int> GetBoxList()
        {
            //1101
            var module1101HelpList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1101].BoxInfoList;
            List<int> module1101List = new List<int>();
            foreach (var binfo in module1101HelpList)
            {
                if (tbdraw_Box.Get(binfo.Id).limitType == 2)
                {
                    if (binfo.SetStartTime + tbdraw_Box.Get(binfo.Id).dateLimit > TimeHelper.ClientNowSeconds())
                    {
                        module1101List.Add(binfo.Id);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (tbdraw_Box.Get(binfo.Id).limitType == 3)
                {
                    if (binfo.DrawCount > 0)
                    {
                        module1101List.Add(binfo.Id);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (tbdraw_Box.Get(binfo.Id).limitType == 4)
                {
                    if (binfo.SetStartTime + tbdraw_Box.Get(binfo.Id).dateLimit > TimeHelper.ClientNowSeconds())
                    {
                        if (binfo.DrawCount > 0)
                        {
                            module1101List.Add(binfo.Id);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            module1101List.Sort(delegate(int i1, int i2)
            {
                return tbdraw_Box.Get(i1).sort.CompareTo(tbdraw_Box.Get(i2).sort);
            });

            //1102
            List<int> module1102List = new List<int>();
            foreach (var tbdb in tbdraw_Box.DataList)
            {
                if (tbdb.tagFunc == 1102)
                {
                    module1102List.Add(tbdb.id);
                }
            }

            module1102List.Sort(delegate(int i1, int i2)
            {
                return tbdraw_Box.Get(i1).sort.CompareTo(tbdraw_Box.Get(i2).sort);
            });

            //1103
            List<int> module1103List = new List<int>();
            foreach (var tbdb in tbdraw_Box.DataList)
            {
                if (tbdb.tagFunc == 1103)
                {
                    module1103List.Add(tbdb.id);
                }
            }

            module1103List.Sort(delegate(int i1, int i2)
            {
                return tbdraw_Box.Get(i1).sort.CompareTo(tbdraw_Box.Get(i2).sort);
            });

            List<int> resultList = new List<int>();
            foreach (var i in module1101List)
            {
                resultList.Add(i);
            }

            foreach (var i in module1102List)
            {
                resultList.Add(i);
            }

            foreach (var i in module1103List)
            {
                resultList.Add(i);
            }

            return resultList;
        }

        #region 上一次代码

        //private async UniTaskVoid CreateReward(UI ui, List<Vector3> V3, int BoxID, int ThisRow, draw_banner db)
        //{
        //    var uiList = ui.GetList();
        //    uiList.Clear();
        //    Dictionary<UI, int> sortHelpDic = new Dictionary<UI, int>();
        //    for (int i = 0; i < V3.Count; i++)
        //    {
        //        int ihelp = i;
        //        var reui = await uiList.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, V3[ihelp], false);
        //        float rew = reui.GetRectTransform().Width();
        //        reui.GetRectTransform().SetScale(new Vector2(cellSize / rew, cellSize / rew));
        //        sortHelpDic.Add(reui, ihelp);

        //        var reBtn = reui.GetFromReference(UICommon_RewardItem.KBtn_Item);
        //        //reBtn.GetXButton().OnClick?.Add(() =>
        //        //{
        //        //    if (lastItemI == ihelp && lastDB == db && lastRow == ThisRow)
        //        //    {
        //        //        Debug.Log("==");
        //        //        //PreInit(BoxID, true, ThisRow, db, 300).Forget();
        //        //        PreInit(BoxID, true).Forget();
        //        //        lastItemI = -1;
        //        //        lastDB = null;
        //        //        lastRow = -1;
        //        //    }
        //        //    else
        //        //    {
        //        //        Debug.Log("!=");
        //        //        //PreInit(BoxID).Forget();
        //        //        PreInit(BoxID, true, true, ThisRow, db, 300).Forget();
        //        //        lastItemI = ihelp;
        //        //        lastDB = db; 
        //        //        lastRow = ThisRow;
        //        //    }
        //        //});
        //        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(reBtn, () =>
        //        {
        //            if (lastItemI == ihelp && lastDB == db && lastRow == ThisRow)
        //            {
        //                Debug.Log("==");
        //                //PreInit(BoxID, true, ThisRow, db, 300).Forget();
        //                PreInit(BoxID, true).Forget();
        //                lastItemI = -1;
        //                lastDB = null;
        //                lastRow = -1;
        //            }
        //            else
        //            {
        //                Debug.Log("!=");
        //                //PreInit(BoxID).Forget();
        //                PreInit(BoxID, true, true, ThisRow, db, 300).Forget();
        //                lastItemI = ihelp;
        //                lastDB = db;
        //                lastRow = ThisRow;
        //            }
        //        });
        //    }
        //    uiList.Sort((UI ui1, UI ui2) =>
        //    {
        //        if (sortHelpDic[ui1] <= sortHelpDic[ui2])
        //        {
        //            return -1;
        //        }
        //        else
        //        {
        //            return 1;
        //        }
        //    });
        //    JiYuUIHelper.ForceRefreshLayout(ui);
        //}

        #endregion

        #region 修改代码

        private async UniTaskVoid CreateReward(UI ui, List<Vector3> V3, UISubPanel_Shop_PreBg uiBg, int thisRow,
            int thisBgIndex)
        {
            var uiList = ui.GetList();
            uiList.Clear();
            Dictionary<UI, int> sortHelpDic = new Dictionary<UI, int>();
            //float tipH = 400;

            for (int i = 0; i < V3.Count; i++)
            {
                #region 生成items

                int ihelp = i;
                var reui = await uiList.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, V3[ihelp], false);
                float rew = reui.GetRectTransform().Width();
                reui.GetRectTransform().SetScale(new Vector2(cellSize / rew, cellSize / rew));
                sortHelpDic.Add(reui, ihelp);

                #endregion

                #region 生成tip源代码

                float titleH = uiBg.GetFromReference(UISubPanel_Shop_PreBg.KTitleImg).GetRectTransform().Height();
                var thisBgRowList = uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetList();
                var reBtn = reui.GetFromReference(UICommon_RewardItem.KBtn_Item);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(reBtn,
                    () => { CreateTipHelp(uiBg, titleH, thisBgRowList, V3, ihelp, reui, thisRow, thisBgIndex); });

                #endregion

                #region 生成tip新代码

                //var reBtn = reui.GetFromReference(UICommon_RewardItem.KBtn_Item);
                //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(reBtn, () =>
                //{

                //});

                #endregion
            }

            uiList.Sort((UI ui1, UI ui2) =>
            {
                if (sortHelpDic[ui1] <= sortHelpDic[ui2])
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            JiYuUIHelper.ForceRefreshLayout(ui);
        }

        #endregion

        private void ChangeRowIndex(UI ui, int thisRow)
        {
            ui.GameObject.transform.SetSiblingIndex(thisRow + 1);
        }

        private async void CreateTipHelp(UISubPanel_Shop_PreBg uiBg, float titleH,
            UIListComponent thisBgRowList, List<Vector3> V3, int ihelp, UI reui, int thisRow, int thisBgIndex)
        {
            float contentH = this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().Height();
            if (lastBg != uiBg && lastBg != null)
            {
                float BgH = lastBg.GetRectTransform().Height();
                if (lastRowUI != null)
                {
                    float lastH = lastRowUI.GetRectTransform().Height();
                    lastBg.GetRectTransform().SetHeight(BgH - lastH - Hspacing);
                    lastBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform()
                        .SetHeight(BgH - lastH - titleH - Hspacing);
                    this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                        .SetHeight(contentH - lastH - Hspacing);
                    this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithLeft(0);
                    this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithRight(0);
                    JiYuUIHelper.ForceRefreshLayout(uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg));
                    JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);

                    lastRowUI.Dispose();
                }

                contentH = this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().Height();
                BgH = uiBg.GetRectTransform().Height();
                var uiRow = await thisBgRowList.CreateWithUITypeAsync(UIType.UISubPanel_Shop_Pre_Row, false);
                CreateTip(uiRow, V3[ihelp], reui);
                uiRow.GetRectTransform().SetHeight(tipH);
                uiBg.GetRectTransform().SetHeight(BgH + tipH + Hspacing);
                uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform()
                    .SetHeight(BgH + tipH - titleH + Hspacing);
                uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetOffsetWithLeft(0);
                uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetOffsetWithRight(0);
                uiRow.GetRectTransform().SetWidth(this.GetFromReference(KScrollView).GetScrollRect().Content
                    .GetRectTransform().Width());
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                    .SetHeight(contentH + tipH + Hspacing);
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithLeft(0);
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithRight(0);
                ChangeRowIndex(uiRow, thisRow);
                JiYuUIHelper.ForceRefreshLayout(uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg));
                JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);
                InScreenOrNot(thisBgIndex, uiRow, tipH, thisRow, uiBg);
                lastItemUI = reui;
                lastRowUI = uiRow;
                lastRowIndex = thisRow;
            }
            else
            {
                if (lastItemUI != reui)
                {
                    if (lastRowIndex != thisRow)
                    {
                        float BgH = uiBg.GetRectTransform().Height();
                        if (lastRowUI != null)
                        {
                            float lastH = lastRowUI.GetRectTransform().Height();
                            uiBg.GetRectTransform().SetHeight(BgH - lastH - Hspacing);
                            uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform()
                                .SetHeight(BgH - lastH - titleH - Hspacing);
                            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                                .SetHeight(contentH - lastH - Hspacing);
                            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                                .SetOffsetWithLeft(0);
                            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                                .SetOffsetWithRight(0);
                            lastRowUI.Dispose();
                            JiYuUIHelper.ForceRefreshLayout(uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg));
                            JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);

                            await UniTask.Yield();
                        }

                        contentH = this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                            .Height();
                        BgH = uiBg.GetRectTransform().Height();
                        var uiRow = await thisBgRowList.CreateWithUITypeAsync(UIType.UISubPanel_Shop_Pre_Row, false);
                        CreateTip(uiRow, V3[ihelp], reui);
                        uiRow.GetRectTransform().SetHeight(tipH);
                        uiBg.GetRectTransform().SetHeight(BgH + tipH + Hspacing);
                        uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform()
                            .SetHeight(BgH + tipH - titleH + Hspacing);
                        uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetOffsetWithLeft(0);
                        uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform().SetOffsetWithRight(0);
                        uiRow.GetRectTransform().SetWidth(this.GetFromReference(KScrollView).GetScrollRect().Content
                            .GetRectTransform().Width());
                        this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                            .SetHeight(contentH + tipH + Hspacing);
                        this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                            .SetOffsetWithLeft(0);
                        this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                            .SetOffsetWithRight(0);
                        ChangeRowIndex(uiRow, thisRow);
                        JiYuUIHelper.ForceRefreshLayout(uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg));
                        JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);
                        InScreenOrNot(thisBgIndex, uiRow, tipH, thisRow, uiBg);
                        lastItemUI = reui;
                        lastRowUI = uiRow;
                        lastRowIndex = thisRow;
                    }
                    else
                    {
                        CreateTip(lastRowUI, V3[ihelp], reui);
                        lastItemUI = reui;
                    }
                }
                else
                {
                    lastItemUI = null;
                    lastRowUI.Dispose();
                    lastRowUI = null;
                    lastRowIndex = -1;
                    float BgH = uiBg.GetRectTransform().Height();
                    uiBg.GetRectTransform().SetHeight(BgH - tipH - Hspacing);
                    uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg).GetRectTransform()
                        .SetHeight(BgH - tipH - titleH - Hspacing);
                    this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                        .SetHeight(contentH - tipH - Hspacing);
                    this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithLeft(0);
                    this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithRight(0);
                    JiYuUIHelper.ForceRefreshLayout(uiBg.GetFromReference(UISubPanel_Shop_PreBg.KBg));
                    JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KScrollView).GetScrollRect().Content);
                }
            }

            lastBg = uiBg;
        }

        /// <summary>
        /// 检测生成的tip是否在屏幕内，如果不在屏幕内则自动滑动屏幕
        /// </summary>
        /// <param name="thisBgIndex">生成的tip所在的shop pre bg</param>
        /// <param name="uiRow">生成的tip的载体</param>
        /// <param name="tipH">tip的高度</param>
        /// <param name="thisRow"></param>
        /// <param name="uiBg"></param>
        private async void InScreenOrNot(int thisBgIndex, UI uiRow, float tipH, int thisRow, UI uiBg)
        {
            float tipHeightFormTop;
            float topBgH = 0;
            for (int i = 0; i < thisBgIndex; i++)
            {
                topBgH += bgList[i].GetRectTransform().Height() + Hspacing;
            }

            tipHeightFormTop = topBgH +
                               uiBg.GetFromReference(UISubPanel_Shop_PreBg.KTitleImg).GetRectTransform().Height() +
                               (thisRow + 1) * (cellSize + Hspacing) + tipH;
            float scrollHHelp = this.GetFromReference(KScrollView).GetRectTransform().Height() +
                                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                                    .AnchoredPosition().y;
            if (tipHeightFormTop > scrollHHelp)
            {
                await UniTask.DelayFrame(1);
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                    .DoAnchoredPositionY(
                        tipHeightFormTop - this.GetFromReference(KScrollView).GetRectTransform().Height(), 0.12f);
            }
        }

        private void CreateTip(UI ui, Vector3 V3, UI reui)
        {
            int equipId = (int)V3.y;
            var equip = new MyGameEquip()
            {
                reward = V3
            };
            ui.GetList().Clear();
            var tipUI = ui.GetList().CreateWithUIType(UIType.UICommon_EquipTips, equip, false);
            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle).SetActive(false);
            tipUI.GetFromReference(UICommon_EquipTips.KBottom).SetActive(false);
            tipUI.GetFromReference(UICommon_EquipTips.KBtn_Decrease).SetActive(false);
            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform().SetAnchoredPositionX(
                reui.GetRectTransform().AnchoredPosition().x -
                this.GetFromReference(KScrollView).GetRectTransform().Width() / 2);
            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(true);
            tipH = tipUI.GetFromReference(UICommon_EquipTips.KMid).GetRectTransform().Height() +
                   tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform().Height();
            tipUI.GetFromReference(UICommon_EquipTips.KMid).GetRectTransform().SetAnchoredPositionY(0);
            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform().SetAnchoredPositionY(0);
            tipUI.GetRectTransform().SetHeight(tipH);
            tipUI.GetRectTransform().SetOffsetWithLeft(0);
            tipUI.GetRectTransform().SetOffsetWithRight(0);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}