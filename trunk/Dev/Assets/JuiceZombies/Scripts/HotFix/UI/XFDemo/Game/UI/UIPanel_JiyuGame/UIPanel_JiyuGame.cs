using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using HotFix_UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_JiyuGame)]
    internal sealed class UIPanel_JiyuGameEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_JiyuGame;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_JiyuGame>();
        }
    }

    public partial class UIPanel_JiyuGame : UI, IAwake<GameRole>
    {
        private Tbtag tbtag;
        private Tbuser_level tbuserLevel;
        private Tbdraw_box tbdraw_Box;
        private Tbrecharge tbrecharge;
        private Tbconstant tbconstant;
        private Tbuser_avatar tbuser_avatar;
        private Tbtag_func tbtag_func;
        private Tblanguage tblanguage;
        private Tbguide tbguide;

        public List<UI> toggleUIList = new List<UI>();

        //private UI lastTabUI;
        public int lastSortId;
        public int lastTagId;
        private long timerId;
        private long timerId0;
        private GameRole gameRole;

        //public static bool equipIsDone = false;

        public bool m_IsDrag;
        public bool m_IsEndMove;

        public float m_SlideSpeed = 5;
        private float m_TargetPos = 0;
        public int m_AllPageCount = 4; //总页数

        public int m_NowIndex = 0; //当前位置索引
        private int m_LastIndex = -1; //上一次的索引
        private List<float> m_ListPageValue = new List<float> { }; //总页数索引比列 0-1 

        public List<int> unlockTags = new List<int>();

        private XScrollRectComponent m_ScrollRect;

        private bool isBegin = false;
        private int guideId;

        private CancellationTokenSource cts = new CancellationTokenSource();


        public async void Initialize(GameRole args)
        {
            //Debug.LogError("CreateUIPanel_JiyuGame");
            gameRole = args;
            InitJson();
            this.InitPanel();
        }

        void InitJson()
        {
            tbuserLevel = ConfigManager.Instance.Tables.Tbuser_level;
            tbdraw_Box = ConfigManager.Instance.Tables.Tbdraw_box;
            tbrecharge = ConfigManager.Instance.Tables.Tbrecharge;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
            tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbguide = ConfigManager.Instance.Tables.Tbguide;
        }

        private async UniTask InitPanel()
        {
            // var input = "type=3;para=[101,85,86]";
            // JiYuUIHelper.ExtractUnlockOrGoToStr(input, out int type, out List<int> paras);

            var timerMgr = TimerManager.Instance;

            //tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtag = ConfigManager.Instance.Tables.Tbtag;

            var KOptions = this.GetFromReference(UIPanel_JiyuGame.KOptions);
            var KContentPos = this.GetFromReference(UIPanel_JiyuGame.KContentPos);
            var KScrollView = this.GetFromReference(UIPanel_JiyuGame.KScrollView);
            var KBg_TestGuid = GetFromReference(UIPanel_JiyuGame.KBg_TestGuid);


            var contTransform = KContentPos.GetComponent<Transform>();
            //var optionTransform = KOptions.GetComponent<Transform>();
            optionRectTransform = KOptions.GetComponent<RectTransform>();

            m_ScrollRect = KScrollView.GetXScrollRect();

            m_ScrollRect.OnBeginDrag.Add(() =>
            {
                //Log.Error($"OnBeginDrag");
                //Log.Debug($" {m_ScrollRect.Get().horizontalNormalizedPosition}");
                m_IsEndMove = false;
                m_IsDrag = true;
                JiYuUIHelper.DestoryAllTips();
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui2))
                {
                    var uiMain = ui2 as UIPanel_Main;
                    uiMain.BtnTreasure();
                }
            });
            //m_ScrollRect.OnDrag.Add(OnScrollRectDragging);
            m_ScrollRect.OnEndDrag.Add(OnEndDrag);
            //m_ScrollRect.OnDrag.Add(OnScrollRectDragging);
            float pageWidth = KContentPos.GetRectTransform().Width();
            float pageHeigth = KContentPos.GetRectTransform().Height();


            var list = KOptions.GetList();

            list.Clear();
            unlockTags.Clear();
            initialPositions = new Vector2[tbtag.DataList.Count];

            for (int i = 0; i < tbtag.DataList.Count; i++)
            {
                int index = i;
                var tag = tbtag.DataList[index];
                if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tag.id))
                {
                    unlockTags.Add(tag.id);
                }
            }

            unlockTags.Sort((a, b) => { return a.CompareTo(b); });
            int initSortId = 3;
            for (int i = 0; i < tbtag.DataList.Count; i++)
            {
                int index = i;
                var tag = tbtag.DataList[index];
                var ui =
                    await list.CreateWithUITypeAsync(UIType.UISubPanel_ToggleItem, tag.id, tag.sort,
                        false) as UISubPanel_ToggleItem;

                var KImg_RedDot = ui.GetFromReference(UISubPanel_ToggleItem.KImg_RedDot);
                var KText = ui.GetFromReference(UISubPanel_ToggleItem.KText);
                var KContent = ui.GetFromReference(UISubPanel_ToggleItem.KContent);
                var KImg_Btn = ui.GetFromReference(UISubPanel_ToggleItem.KImg_Btn);
                //KRedDot.SetActive(false);

                int monsterCollectionId = 5104;
                if (tag.id == 5)
                {
                    var itemStr =
                        $"{NodeNames.GetTagRedDotName(5)}";
                    KImg_RedDot?.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr) > 0);
                    RedDotManager.Instance.AddListener(itemStr, (num) =>
                    {
                        //Log.Error($"Jiyu {itemStr} {num}");
                        KImg_RedDot?.SetActive(num > 0);
                    });
                }


                //var ui = await UIHelper.CreateAsync(option, UIType.UIToggleItem, tag.sort, optionTransform);
                toggleUIList.Add(ui);
                ui.SetParent(this, false);

                initialPositions[index] = KContent.GetRectTransform().AnchoredPosition();


                if (tag.init == 1)
                {
                    //m_NowIndex = index;

                    UI panelUI = null;
                    var global = Common.Instance.Get<Global>();
                    global.GameObjects?.Cover?.SetViewActive(false);


                    //lastTabUI = panelUI;
                    initSortId = tag.sort;
                    // lastTagId = tag.id;
                }

                var uiBtn = KImg_Btn.GetXButton();
                if (uiBtn == null)
                {
                    Log.Error($"{this.Name}.GetXButton() is null");
                    return;
                }

                uiBtn.SetPointerActive(true);

                uiBtn.SetLongPressInterval(JiYuTweenHelper.LongPressInterval);

                uiBtn.SetMaxLongPressCount(JiYuTweenHelper.MaxLongPressCount);


                uiBtn.OnClick.Add(() =>
                {
                    if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tag.id))
                    {
                        OnBtnClickEvent(tag.sort);
                        SetToPageIndex(tag.id);
                    }
                    else
                    {
                        var str = $"{tblanguage.Get($"text_goto_failed_{tag.id}").current}";
                        JiYuUIHelper.ClearCommonResource();
                        UIHelper.CreateAsync(UIType.UICommon_Resource, str);
                    }
                });

                uiBtn.OnLongPressEnd.Add((f) =>
                {
                    if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tag.id))
                    {
                        OnBtnClickEvent(tag.sort);
                        SetToPageIndex(tag.id);
                    }
                    else
                    {
                        var str = $"{tblanguage.Get($"text_goto_failed_{tag.id}").current}";
                        JiYuUIHelper.ClearCommonResource();
                        UIHelper.CreateAsync(UIType.UICommon_Resource, str);
                    }
                });

                //uiBtn.SetEnabled(false);


                CreateTagPanel(tag.id);
            }

            list.Sort((a, b) =>
            {
                var uia = a as UISubPanel_ToggleItem;
                var uib = b as UISubPanel_ToggleItem;
                return uia.sort.CompareTo(uib.sort);
            });
            // for (int i = 0; i < this.children.Count; i++)
            // {
            //     this.children[i].GameObject.transform.SetSiblingIndex(i);
            // }
            //
            // listContent.Sort((a, b) =>
            // {
            //     //UI ascr, bscr;
            //     int atagId = 0;
            //     int btagId = 0;
            //     if (a is UIPanel_Shop)
            //     {
            //         var ascr = a as UIPanel_Shop;
            //         atagId = ascr.tagId;
            //     }
            //
            //     if (b is UIPanel_Shop)
            //     {
            //         var bscr = b as UIPanel_Shop;
            //         btagId = bscr.tagId;
            //     }
            //
            //     if (a is UISubPanel_Equipment)
            //     {
            //         var ascr = a as UISubPanel_Equipment;
            //         atagId = ascr.tagId;
            //     }
            //
            //     if (b is UISubPanel_Equipment)
            //     {
            //         var ascr = b as UISubPanel_Equipment;
            //         btagId = ascr.tagId;
            //     }
            //
            //     if (a is UIPanel_Main)
            //     {
            //         var ascr = a as UIPanel_Main;
            //         atagId = ascr.tagId;
            //     }
            //
            //     if (b is UIPanel_Main)
            //     {
            //         var ascr = b as UIPanel_Main;
            //         btagId = ascr.tagId;
            //     }
            //
            //     if (a is UIPanel_Challege)
            //     {
            //         var ascr = a as UIPanel_Challege;
            //         atagId = ascr.tagId;
            //     }
            //
            //     if (b is UIPanel_Challege)
            //     {
            //         var ascr = b as UIPanel_Challege;
            //         btagId = ascr.tagId;
            //     }
            //
            //     if (a is UIPanel_Person)
            //     {
            //         var ascr = a as UIPanel_Person;
            //         atagId = ascr.tagId;
            //     }
            //
            //     if (b is UIPanel_Person)
            //     {
            //         var ascr = b as UIPanel_Person;
            //         btagId = ascr.tagId;
            //     }
            //
            //     return tbtag.Get(atagId).sort.CompareTo(tbtag.Get(btagId).sort);
            // });

            RefreshTags();

            m_ScrollRect.Content.GetRectTransform().SetHeight(pageHeigth);

            //OnBtnClickEvent(initSortId);
            OnButtonClickAnim(initSortId - 1);

            SetResourceUI();
            AudioManager.Instance.ClearFModBgmAudio();
            //AudioManager.Instance.StopFModAudio(2103);

            var lastChapterId = JsonManager.Instance.userData.lastChapterId;
            Tbchapter tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            var blockId = tbchapter.Get(lastChapterId).blockId;
            int audioId = 1150 + blockId;

            AudioManager.Instance.PlayFModAudio(audioId);
            Debug.Log($"audioIdaudioId{audioId}");

            timerId0 = timerMgr.RepeatedFrameTimer(this.Update);
            // await UniTask.Delay(1000);
            //
            // var KOptionslist = KOptions.GetList();
            // foreach (var child in KOptionslist.Children)
            // {
            //     var childs = child as UISubPanel_ToggleItem;
            //     if (childs.tagId == 2)
            //     {
            //         JiYuUIHelper.SetForceGuideRectUI(childs, KBg_TestGuid);
            //         break;
            //     }
            // }

            // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var uimain))
            // {
            //     var KBtn_Start = uimain.GetFromReference(UIPanel_Main.KBtn_Start);
            //     JiYuUIHelper.SetForceGuideRectUI(KBtn_Start, KBg_TestGuid);
            //     // var KBtn_Start = uimain.GetFromReference(UIPanel_Main.KBtn_Start);
            //     // KBg_TestGuid
            // }
        }

        public void ForceRefreshToggles()
        {
            var KOptions = this.GetFromReference(UIPanel_JiyuGame.KOptions);
            JiYuUIHelper.ForceRefreshLayout(KOptions);
        }

        public async UniTaskVoid Guide(int panelId)
        {
            if (ResourcesSingleton.Instance.settingData.GuideList.Contains(1))
            {
                return;
            }

            //Log.Error($"JiYuPanelId{panelId}");
            JiYuUIHelper.EnableJiYuMask(true);
            var KOptions = this.GetFromReference(UIPanel_JiyuGame.KOptions);
            switch (panelId)
            {
                case 1:
                    var guide1 = tbguide.DataList.Where(a => a.guideType == 317).FirstOrDefault();
                    if (ResourcesSingleton.Instance.settingData.GuideList.Contains(guide1.group))
                    {
                        UIHelper.Remove(UIType.UISubPanel_Guid);
                        await UniTask.Delay(1000);
                        await UIHelper.CreateAsync(UIType.UISubPanel_Guid, guide1.id, 2);
                    }


                    break;
                case 2:
                    var guide2 = tbguide.DataList.Where(a => a.guideType == 316).FirstOrDefault();
                    if (ResourcesSingleton.Instance.settingData.GuideList.Contains(guide2.group))
                    {
                        UIHelper.Remove(UIType.UISubPanel_Guid);
                        await UniTask.Delay(1000);

                        await UIHelper.CreateAsync(UIType.UISubPanel_Guid, guide2.id, 2);
                    }


                    break;
            }

            JiYuUIHelper.EnableJiYuMask(false);
        }

        public async UniTask UnLockTag(int tagId)
        {
            var KOptions = this.GetFromReference(UIPanel_JiyuGame.KOptions);
            var list = KOptions.GetList();

            foreach (var ui in list.Children)
            {
                var uis = ui as UISubPanel_ToggleItem;
                if (uis.tagId == tagId)
                {
                    uis.RefreshBtnEnable();
                    break;
                }
            }

            unlockTags.Add(tagId);
            unlockTags.Sort((a, b) => { return a.CompareTo(b); });
            await CreateTagPanel(tagId);
        }

        public void RefreshTags()
        {
            var KContentPos = this.GetFromReference(UIPanel_JiyuGame.KContentPos);
            float pageWidth = KContentPos.GetRectTransform().Width();

            m_AllPageCount = unlockTags.Count;
            for (int i = 0; i < unlockTags.Count; i++)
            {
                if (tbtag.Get(unlockTags[i]).init == 1)
                {
                    m_NowIndex = i;
                    Log.Debug($"unlockTags[i] {unlockTags[i]}  {m_NowIndex}");
                    break;
                }
            }

            if (JsonManager.Instance.userData.tagId > 0)
            {
                //TODO:
                //SetToPageIndex(JsonManager.Instance.userData.tagId);
            }

            m_ListPageValue.Clear();
            for (float i = 0; i < m_AllPageCount; i++)
            {
                m_ListPageValue.Add(i * (1f / (m_AllPageCount - 1)));

                //Log.Debug($"(i * (1f / m_AllPageCount)) {i * (1f / m_AllPageCount)}");
            }

            m_TargetPos = m_ListPageValue[m_NowIndex];

            m_ScrollRect.Content.GetRectTransform().SetWidth(pageWidth * m_AllPageCount);
            for (int i = 0; i < m_ScrollRect.Content.GameObject.transform.childCount; i++)
            {
                var tran = m_ScrollRect.Content.GameObject.transform.GetChild(i) as RectTransform;
                tran.SetAnchoredPositionX(i * pageWidth);
            }
        }

        /// <summary>
        /// 跳转tagId界面
        /// </summary>
        /// <param name="tagId">tagId</param>
        /// <returns>状态-1 未解锁此tagid 0在当前 1正常/returns>
        public int GoToTagId(int tagId)
        {
            int type = 1;
            if (tagId == lastTagId)
            {
                return 0;
            }

            var tag = tbtag.Get(tagId);
            var sort = tag.sort;
            if (ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tagId))
            {
                OnBtnClickEvent(sort);
                SetToPageIndex(tagId);
            }
            else
            {
                var str = $"{tblanguage.Get($"text_goto_failed_{tag.id}").current}";
                JiYuUIHelper.ClearCommonResource();
                UIHelper.CreateAsync(UIType.UICommon_Resource, str);
                return -1;
            }

            return type;
        }

        public void SetScrollRectHorNorPos(float value)
        {
            value = Mathf.Clamp01(value);
            //Log.Debug($"qian {m_ScrollRect.Get().horizontalNormalizedPosition}");
            m_ScrollRect.Get().horizontalNormalizedPosition = value;
            //Log.Debug($"hou {m_ScrollRect.Get().horizontalNormalizedPosition}");
        }

        void OnScrollRectDragging(Vector2 delta)
        {
            bool preferHorizontal = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y); // 判断水平滑动

            if (preferHorizontal)
            {
                // Log.Debug(
                //     $"delta:{delta} {m_ScrollRect.Get().horizontalNormalizedPosition} deltaX:{Mathf.Abs(delta.x)}");

                //Log.Debug($"{}");

                // 处理水平滑动
                //scrollRect.SetEnabled(false);
            }
        }

        public async UniTask ReCreateAllTagPanel()
        {
            var KContentPos = this.GetFromReference(UIPanel_JiyuGame.KContentPos);

            var tran = m_ScrollRect.Content.GetRectTransform().Get();


            for (int i = 1; i < 6; i++)
            {
                var tagId = i;
                if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tagId))
                {
                    continue;
                }

                CreateTagPanel(tagId);
            }
        }

        async UniTask CreateTagPanel(int id)
        {
            if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(id))
            {
                return;
            }

            var KContentPos = this.GetFromReference(UIPanel_JiyuGame.KContentPos);

            var tran = m_ScrollRect.Content.GetRectTransform().Get();
            float pageWidth = KContentPos.GetRectTransform().Width();
            float pageHeigth = KContentPos.GetRectTransform().Height();


            UI ui = null;


            switch (id)
            {
                case 1:
                    ui = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_Shop, tran);
                    break;
                case 2:
                    ui = await UIHelper.CreateAsyncNew(this, UIType.UISubPanel_Equipment, tran);

                    break;
                case 3:
                    ui = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_Main, tran);
                    break;
                case 4:
                    var maxChapterID = ResourcesSingleton.Instance.levelInfo.maxPassChapterID;
                    if (maxChapterID >= 2)
                    {
                        ui = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_Challege, tran);
                    }
                    // else
                    // {
                    //     ui = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_Battle, tran);
                    // }


                    break;
                case 5:
                    ui = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_Person, tran);


                    break;
            }

            var index = unlockTags.FindIndex(a => a == id);
            Log.Debug($"CreateTagPanel {index}");
            ui?.GameObject?.transform?.SetSiblingIndex(index);
            ui?.GetRectTransform()?.SetAnchoredPosition(new Vector2(pageWidth * (index), 0));
            SetPivotLeftUp(ui);
            ui?.GetRectTransform()?.SetWidth(pageWidth);
            ui?.GetRectTransform()?.SetHeight(pageHeigth);
        }


        private void RefereshMailRedDot()
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out var ui))
            {
                var img = ui?.GetFromReference(UIPanel_Battle.KImg_ExtendRedDot);

                img?.SetActive(JiYuUIHelper.IsMailRedDot());
            }

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out var ui0))
            {
                var emailRedDot = ui0?.GetFromReference(UIPanel_Battle.KemailButton_BG)?.GetRectTransform()
                    .GetChild(0).gameObject;
                emailRedDot?.SetActive(JiYuUIHelper.IsMailRedDot());
            }
        }

        public void RefreshTalentRedDot()
        {
            //UI temp = GetToggle(2);
            //if (temp == default) return;
            //var talent = temp as UISubPanel_ToggleItem;
            //if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out UI uiTemp))
            //{
            //    var battleView = uiTemp as UIPanel_Battle;
            //    if (battleView.isDisplayRedDot())
            //    {
            //        talent.GetRedDot().SetActive(true);
            //    }
            //    else
            //    {
            //        talent.GetRedDot().SetActive(false);
            //    }
            //}
        }


        #region 黄金国

        /// <summary>
        /// 更新商店红点
        /// </summary>
        public void RefreshShopRedPoint()
        {
            UI temp = GetToggle(1);
            if (temp == default) return;
            var shop = temp as UISubPanel_ToggleItem;
            List<bool> bools = new List<bool>();
            bools.Add(false);

            List<long> updateTimeList = new List<long>();

            #region old red point

            ////1102
            //foreach (var db in tbdraw_Box.DataList)
            //{
            //    if (db.tagFunc == 1102)
            //    {
            //        long ItemNum;
            //        if (ResourcesSingleton.Instance.items.TryGetValue(tbdraw_Box[db.id].item, out long value))
            //        {
            //            ItemNum = value;
            //        }
            //        else
            //        {
            //            ItemNum = 0;
            //            //报错
            //            Debug.Log("There is no Item of Key " + tbdraw_Box[db.id].item);
            //        }

            //        bool a = false;
            //        if (ItemNum <= 0)
            //        {
            //            a = false;
            //        }
            //        else
            //        {
            //            a = true;
            //        }

            //        bools.Add(a);
            //    }
            //}

            ////1103
            //foreach (var db in tbdraw_Box.DataList)
            //{
            //    if (db.tagFunc == 1103 && db.drawType == 5)
            //    {
            //        bool a = false;
            //        if (ResourcesSingleton.Instance.shopInit.shopHelpDic.TryGetValue(db.tagFunc,
            //                out Dictionary<int, List<long>> value))
            //        {
            //            if (ResourcesSingleton.Instance.shopInit.shopHelpDic[db.tagFunc][db.id][2] == 1)
            //            {
            //                a = true;
            //            }
            //            else
            //            {
            //                RefreshShopRedPointHelp(db);
            //                a = false;
            //            }
            //        }

            //        bools.Add(a);
            //    }
            //}

            ////1402
            //foreach (var recharge in tbrecharge.DataList)
            //{
            //    if (recharge.tagFunc == 1402)
            //    {
            //        bool b = false;
            //        if (ResourcesSingleton.Instance.shopInit.shopHelpDic.TryGetValue(1402,
            //                out Dictionary<int, List<long>> value))
            //        {
            //            if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][1] > 0
            //                || ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][2] > 0)
            //            {
            //                b = true;
            //            }
            //            else
            //            {
            //                b = false;
            //            }
            //        }

            //        bools.Add(b);
            //    }
            //}

            ////1201
            //int dailyShopNumber = 6;
            //bool c = false;
            //for (int i = 1; i < dailyShopNumber + 1; i++)
            //{
            //    if (ResourcesSingleton.Instance.shopInit.shopHelpDic.ContainsKey(1201))
            //    {
            //        int sign = (int)ResourcesSingleton.Instance.shopInit.shopHelpDic[1201][i][0];
            //        int times = (int)ResourcesSingleton.Instance.shopInit.shopHelpDic[1201][i][1];
            //        long upTime = ResourcesSingleton.Instance.shopInit.shopHelpDic[1201][i][2];
            //        if (ConfigManager.Instance.Tables.Tbshop_daily[sign].pos == 1)
            //        {
            //            if (times < ConfigManager.Instance.Tables.Tbshop_daily[sign].times)
            //            {
            //                if (TimeHelper.ClientNowSeconds() - upTime >=
            //                    tbconstant.Get("shop_daily_ad_cd").constantValue)
            //                {
            //                    c = true;
            //                }
            //                else
            //                {
            //                    c = false;
            //                }
            //            }
            //            else
            //            {
            //                c = false;
            //            }
            //        }
            //    }
            //}

            //bools.Add(c);

            #endregion

            #region 1101

            if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1101))
            {
                var boxList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1101].BoxInfoList;
                if (boxList.Count > 0)
                {
                    foreach (var box in boxList)
                    {
                        if (tbdraw_Box.Get(box.Id).limitType == 2 || tbdraw_Box.Get(box.Id).limitType == 4)
                        {
                            long startTime = box.SetStartTime;
                            long endTime = startTime + tbdraw_Box.Get(box.Id).dateLimit;
                            long deltaTime = endTime - TimeHelper.ClientNowSeconds();
                            if (deltaTime > 0)
                            {
                                if (tbdraw_Box.Get(box.Id).limitType == 4)
                                {
                                    if (box.DrawCount > 0)
                                    {
                                        bools.Add(ShopModule1101HaveRedPointOrNot(box));
                                    }
                                }
                                else
                                {
                                    bools.Add(ShopModule1101HaveRedPointOrNot(box));
                                }
                            }
                        }
                        else
                        {
                            if (tbdraw_Box.Get(box.Id).limitType == 3)
                            {
                                if (box.DrawCount > 0)
                                {
                                    bools.Add(ShopModule1101HaveRedPointOrNot(box));
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region 1102

            if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1102))
            {
                var boxList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1102].BoxInfoList;
                if (boxList.Count > 0)
                {
                    foreach (var box in boxList)
                    {
                        long ItemNum;
                        if (ResourcesSingleton.Instance.items.TryGetValue(tbdraw_Box.Get(box.Id).item, out long value))
                        {
                            ItemNum = value;
                        }
                        else
                        {
                            ItemNum = 0;
                        }

                        if (ItemNum > 0)
                        {
                            bools.Add(true);
                        }
                    }
                }
            }

            #endregion

            #region 1103

            if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1103))
            {
                foreach (var db in tbdraw_Box.DataList)
                {
                    if (db.tagFunc == 1103 && db.drawType == 5)
                    {
                        var boxList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1103].BoxInfoList;
                        BoxInfo boxInfo = new BoxInfo();
                        foreach (var box in boxList)
                        {
                            if (box.Id == db.id)
                            {
                                boxInfo = box;
                                break;
                            }
                        }

                        if (boxInfo.AdCount > 0)
                        {
                            bools.Add(true);
                        }
                    }
                }
            }

            #endregion

            #region 1201

            if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1201))
            {
                var dbList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1201].DailyBuyList;
                DailyBuy dailyBuy = new DailyBuy();
                foreach (var b in dbList)
                {
                    if (ConfigManager.Instance.Tables.Tbshop_daily.Get(b.Sign).pos == 1)
                    {
                        dailyBuy = b;
                        break;
                    }
                }

                int sign = dailyBuy.Sign;
                int times = dailyBuy.BuyCount;
                long upTime = dailyBuy.UpTime;

                if (times < ConfigManager.Instance.Tables.Tbshop_daily.Get(sign).times)
                {
                    //位置1
                    if (TimeHelper.ClientNowSeconds() - upTime > tbconstant.Get("shop_daily_ad_cd").constantValue)
                    {
                        //不在CD
                        //RedPointMgr.instance.SetState("ShopRoot", "module1201", RedPointState.Show);
                        bools.Add(true);
                    }
                    else
                    {
                        //在CD
                        updateTimeList.Add(tbconstant.Get("shop_daily_ad_cd").constantValue + upTime);
                    }
                }
            }

            #endregion

            #region 1202

            if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1202))
            {
                foreach (var sp in ConfigManager.Instance.Tables.Tbspecials.DataList)
                {
                    if (sp.freeRule.Count != 0)
                    {
                        var gsList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1202].GameSpecialsList;
                        GameSpecials gameSpecials = new GameSpecials();
                        foreach (var gs in gsList)
                        {
                            if (gs.SpecialId == sp.id)
                            {
                                gameSpecials = gs;
                                break;
                            }
                        }

                        if (gameSpecials.FreeCount == 1)
                        {
                            bools.Add(true);
                        }
                    }
                }
            }

            #endregion

            #region 1302

            if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1302))
            {
                var giftList = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1302].GiftInfoList;

                List<long> endTimeList = new List<long>();

                foreach (var gf in giftList)
                {
                    if (gf.EndTime > TimeHelper.ClientNowSeconds() &&
                        ConfigManager.Instance.Tables.Tbgift.Get(gf.GiftId).freeRule.Count != 0)
                    {
                        if (gf.FreeTimes > 0)
                        {
                            bools.Add(true);
                            endTimeList.Add(gf.EndTime);
                        }
                    }
                }

                if (endTimeList.Count > 0)
                {
                    endTimeList.Sort();
                    long recentEndTime = endTimeList[0];
                    updateTimeList.Add(recentEndTime);
                }
            }

            #endregion

            #region 1402

            if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1402))
            {
                foreach (var rc in tbrecharge.DataList)
                {
                    if (rc.tagFunc == 1402)
                    {
                        if (rc.freeRule.Count > 0)
                        {
                            var clist = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1402].ChargeInfoList;
                            ChargeInfo chargeInfo = new ChargeInfo();
                            foreach (var charge in clist)
                            {
                                if (charge.Id == rc.id)
                                {
                                    chargeInfo = charge;
                                    break;
                                }
                            }

                            if (chargeInfo.FreeSum > 0)
                            {
                                bools.Add(true);
                            }
                            else if (chargeInfo.AdSum > 0)
                            {
                                bools.Add(true);
                            }
                        }
                    }
                }
            }

            #endregion

            #region 1403

            if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1403))
            {
                var level = ResourcesSingleton.Instance.UserInfo.RoleAssets.Level;

                int fundNum = 0;
                foreach (var tbf in ConfigManager.Instance.Tables.Tbfund.DataList)
                {
                    if (level >= tbf.unlockLevel)
                    {
                        fundNum++;
                    }
                }

                #region UpRedPoint

                bool upRedPoint = false;

                var fundReverseSortList = new List<fund>();
                foreach (var tbf in ConfigManager.Instance.Tables.Tbfund.DataList)
                {
                    fundReverseSortList.Add(tbf);
                }

                fundReverseSortList.Sort(
                    delegate(fund f1, fund f2) { return f2.unlockLevel.CompareTo(f1.unlockLevel); });

                List<int> upHaveRedPointList = new List<int>();

                for (int i = 0; i < fundReverseSortList.Count; i++)
                {
                    int ihelp = i;
                    if (fundReverseSortList.Count - ihelp <= fundNum)
                    {
                        //Determine if there are red dots
                        bool haveRedDotOrNot = false;
                        foreach (var gf in ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403].GameFoundationList)
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

                #region DownRedPoint

                bool downRedPoint = false;

                foreach (var thisFundByID in ResourcesSingleton.Instance.shopMap.IndexModuleMap[1403]
                             .GameFoundationList)
                {
                    var thisFundRewardList = new List<fund_reward>();
                    foreach (var tbfr in ConfigManager.Instance.Tables.Tbfund_reward.DataList)
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
                    bools.Add(true);
                }
            }

            #endregion

            #region 1404

            if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(1404))
            {
                foreach (var tbm in ConfigManager.Instance.Tables.Tbmonthly.DataList)
                {
                    var cardMap = ResourcesSingleton.Instance.shopMap.IndexModuleMap[1404].SpecialCard.Unclaimed;
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

                    if (isBuy)
                    {
                        //got or not get
                        if (cardData.SumDay < 1)
                        {
                            //got
                            //RedPointMgr.instance.SetState(ShopRoot, "module1404" + tbm.id, RedPointState.Hide);
                        }
                        else if (cardData.SumDay == 1)
                        {
                            //RedPointMgr.instance.SetState(ShopRoot, "module1404" + tbm.id, RedPointState.Show);
                            bools.Add(true);
                        }
                        else if (cardData.SumDay > 1)
                        {
                            //RedPointMgr.instance.SetState(ShopRoot, "module1404" + tbm.id, RedPointState.Show);
                            bools.Add(true);
                        }
                    }
                }
            }

            #endregion

            if (updateTimeList.Count > 0)
            {
                updateTimeList.Sort();
                long recentUpdateTime = updateTimeList[0];
                ShopModuleDelayRefresh(recentUpdateTime).Forget();
            }

            bool redBool = false;
            for (int i = 0; i < bools.Count; i++)
            {
                redBool = redBool || bools[i];
            }

            if (redBool == true)
            {
                shop.GetRedDot().SetActive(true);
            }
            else
            {
                shop.GetRedDot().SetActive(false);
            }
        }

        public void RefreshPersonRedPoint()
        {
            UI temp = GetToggle(5);
            if (temp == default) return;
            var person = temp as UISubPanel_ToggleItem;
            bool noticeRed = false;
            if (JsonManager.Instance.sharedData.noticesList == null)
            {
            }
            else
            {
                if (JsonManager.Instance.sharedData.noticesList.notices == null)
                {
                }
                else
                {
                    if (JsonManager.Instance.sharedData.noticesList.notices.Count == 0)
                    {
                    }
                    else
                    {
                        foreach (var nt in JsonManager.Instance.sharedData.noticesList.notices)
                        {
                            if (nt.readStatus == 0)
                            {
                                //unread
                                noticeRed = true;
                                break;
                            }
                        }
                    }
                }
            }

            //Log.Debug("noticeRed" + noticeRed);
            if (noticeRed)
            {
                //have red point
                person.GetRedDot().SetActive(true);
            }
            else
            {
                //do not have red point
                person.GetRedDot().SetActive(false);
            }
        }

        //public async void RefreshShopRedPointHelp(draw_box db)
        //{
        //    if (TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.shopInit.shopHelpDic[db.tagFunc][db.id][4] >
        //        db.adCd)
        //    {
        //    }
        //    else
        //    {
        //        await UniTask.Delay(1000 * (db.adCd - (int)(TimeHelper.ClientNowSeconds() -
        //                                                    ResourcesSingleton.Instance.shopInit.shopHelpDic[db.tagFunc]
        //                                                        [db.id][4])));
        //        ResourcesSingleton.Instance.shopInit.shopHelpDic[db.tagFunc][db.id][2] = 1;
        //        RefreshShopRedPoint();
        //    }
        //}

        public bool ShopModule1101HaveRedPointOrNot(BoxInfo box)
        {
            long ItemNum;
            if (ResourcesSingleton.Instance.items.TryGetValue(tbdraw_Box.Get(box.Id).item, out long value))
            {
                ItemNum = value;
            }
            else
            {
                ItemNum = 0;
            }

            if (ItemNum > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async UniTaskVoid ShopModuleDelayRefresh(long input)
        {
            if (input <= TimeHelper.ClientNowSeconds())
            {
                return;
            }
            else
            {
                var deltaTime = input - TimeHelper.ClientNowSeconds() + 1;
                await UniTask.Delay(1000 * (int)deltaTime);
                RefreshShopRedPoint();
            }
        }

        #endregion

        public void SetToPageIndex(int index)
        {
            //var tag = tbtag.DataList.Where(a => a.sort == index).ToList().FirstOrDefault();

            int index0 = unlockTags.FindIndex(n => n == index);

            m_IsDrag = false;
            //m_NowIndex = index - 1;
            m_NowIndex = index0;
            m_TargetPos = m_ListPageValue[m_NowIndex];
            //Log.Debug($"m_NowIndex {m_NowIndex} m_TargetPos{m_TargetPos}");
        }

        void SetPivotLeftUp(UI ui)
        {
            ui?.GetRectTransform()?.SetPivot(new Vector2(0, 1));
            ui?.GetRectTransform()?.SetAnchorMin(new Vector2(0, 1));
            ui?.GetRectTransform()?.SetAnchorMax(new Vector2(0, 1));
        }

        private UI GetToggle(int index)
        {
            if (toggleUIList.Count >= index)
            {
                return toggleUIList[tbtag[index].sort - 1];
            }

            return default;
        }

        public int GetLastTabId()
        {
            return lastSortId;
        }

        public void SetResourceUI()
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
            {
                var uis = ui as UIPanel_Main;
                uis?.RefreshResourceUI();
            }

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI uiShop))
            {
                var uiShops = uiShop as UIPanel_Shop;
                uiShops.InitResource();
            }

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Person, out UI uiPerson))
            {
                var uiPersons = uiPerson as UIPanel_Person;
                uiPersons.RefreshResourceUI();
            }

            // RefereshMailRedDot();
            // RefreshTalentRedDot();
            // RefreshShopRedPoint();
            // RefreshPersonRedPoint();
        }


        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
            timerMgr?.RemoveTimerId(ref this.timerId0);
            this.timerId0 = 0;
        }

        private void CloseTimeView()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
            timerMgr?.RemoveTimerId(ref this.timerId0);
            this.timerId0 = 0;
        }

        private void OnQueryMailResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            //new MailInfo()
            var resultMailList = new ResultMailList();
            resultMailList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            foreach (var mailInfo in resultMailList.MailInfoList)
            {
                Log.Debug($"{mailInfo.ToString()}", Color.green);
            }

            foreach (var mailInfo in resultMailList.ReadMailList)
            {
                Log.Debug($"{mailInfo.ToString()}", Color.cyan);
            }


            WebMessageHandlerOld.Instance.RemoveHandler(5, 1, OnQueryMailResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(5, 2, OnQueryMailResponse);
        }

        // void CaptureScreenshot()
        // {
        //     string filePath = "Assets/Resources";
        //
        //     if (!Directory.Exists(filePath))
        //     {
        //         Directory.CreateDirectory(filePath);
        //     }
        //
        //     string fileName = string.Format("Screenshot_{0}.png", System.DateTime.Now.ToString("yy-MM-dd_HH-mm-ss"));
        //     string fullPath = Path.Combine(filePath, fileName);
        //
        //     ScreenCapture.CaptureScreenshot(fullPath);
        //     Debug.Log("Screenshot saved: " + fullPath);
        // }


        void OnEndMove()
        {
            var tag = tbtag.Get(unlockTags[m_NowIndex]);

            //var tag = tbtag.DataList.Where(a => a.id == unlockTags[m_NowIndex]).FirstOrDefault();
            if (tag.sort == lastSortId)
            {
                return;
            }

            OnBtnClickEvent(tag.sort);
        }

        void OnEndOnePanel()
        {
            var KBg_JiYuMask = this.GetFromReference(UIPanel_JiyuGame.KBg_JiYuMask);
            KBg_JiYuMask.SetActive(false);
            //Log.Error($"OnEndOnePanel{m_NowIndex}");
            isBegin = false;
        }

        void OnStartOnePanel()
        {
            var KBg_JiYuMask = this.GetFromReference(UIPanel_JiyuGame.KBg_JiYuMask);
            KBg_JiYuMask.SetActive(true);
            //Log.Error($"OnStartOnePanel{m_NowIndex}");
            isBegin = true;
        }

        async void Update()
        {
            if (!m_IsDrag)
            {
                if (m_ScrollRect == null) return;
                var scrollRect = m_ScrollRect.Get();
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                    scrollRect.horizontalNormalizedPosition,
                    m_TargetPos, Time.deltaTime * m_SlideSpeed);
                if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - m_TargetPos) < 0.02f)
                {
                    if (!m_IsEndMove)
                    {
                        m_IsEndMove = true;
                        OnEndMove();
                    }

                    if (isBegin)
                    {
                        OnEndOnePanel();
                    }
                }
                //Log.Debug($"horizontalNormalizedPosition:{scrollRect.horizontalNormalizedPosition}");
            }


            // if (Input.GetKeyDown(KeyCode.O))
            // {
            //     var test = this.GetComponent<MonsterWeaponTest>();
            //     var tbmonster = ConfigManager.Instance.Tables.Tbmonster;
            //     var tbmonsterAttr = ConfigManager.Instance.Tables.Tbmonster_attr;
            //     var Tbmonster_model = ConfigManager.Instance.Tables.Tbmonster_model;
            //     var str = "";
            //     foreach (var monster in tbmonster.DataList)
            //     {
            //         if (monster.monsterWeaponIndex.Count <= 0)
            //         {
            //             continue;
            //         }
            //
            //         var para1 = monster.monsterWeaponIndex[0];
            //         var para2 = monster.monsterWeaponIndex[1];
            //         var para3 = monster.monsterWeaponIndex[2];
            //         var para4 = monster.monsterWeaponIndex[3];
            //         var model = Tbmonster_model.Get(tbmonsterAttr.Get(monster.monsterAttrId).bookId);
            //
            //         var go = test.gos.Where(a => a.name == model.model).FirstOrDefault();
            //
            //         if (go != null)
            //         {
            //             var trans = go.GetComponent<Transform>().GetChild(0).transform;
            //
            //             var ratios = trans.localScale.x / 0.5f;
            //             para1 = (int)(para1 * ratios);
            //             para2 = (int)(para2 * ratios);
            //             para3 = (int)(para3 * ratios);
            //             str += $"id:{monster.id}  para:{para1};{para2};{para3};{para4}\n";
            //             //Debug.Log($"monsterId:{monster.id} monsterWeaponIndex:{para1};{para2};{para3};{para4}");
            //         }
            //     }
            //
            //     Debug.Log($"{str}");
            //     File.WriteAllText("Assets/text.txt", str);
            //     // foreach (var go in test.gos)
            //     // {
            //     //     Debug.Log($"go:{go.name} ");
            //     // }
            // }
            //
            // if (Input.GetKeyDown(KeyCode.L))
            // {
            //     if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui))
            //     {
            //         var uis = ui as UIPanel_Main;
            //         var KBtn_Diamond = uis.GetFromReference(UIPanel_Main.KBtn_Diamond);
            //         var KBtn_Money = uis.GetFromReference(UIPanel_Main.KBtn_Money);
            //
            //         Log.Debug(
            //             $"KBtn_Diamond{JiYuUIHelper.GetUIPos(KBtn_Diamond)}  {JiYuUIHelper.GetUIPos(KBtn_Money)}",
            //             Color.green);
            //     }
            //     //JiYuUIHelper.PrintMemery();
            // }
            //
            // if (Input.GetKeyDown(KeyCode.M))
            // {
            //     await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionShattersExit);
            //     Log.Debug($"333");
            // }
            //
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     var uim = Common.Instance.Get<UIManager>();
            //     uim.PrintAll();
            // }
            //

            //
            // if (Input.GetKeyDown(KeyCode.K))
            // {
            //     JiYuUIHelper.GoToSomePanel($"type=1;para=[142]");
            // }
            //
            // if (Input.GetKeyDown(KeyCode.Q))
            // {
            //     //UIHelper.Remove(UIType.UIPanel_AnimTools);
            //     if (JiYuUIHelper.TryGetUI(UIType.UIPanel_AnimTools, out var ui))
            //     {
            //         var uis = ui as UIPanel_AnimTools;
            //         uis.Refresh();
            //     }
            //     else
            //     {
            //         UIHelper.CreateAsync(UIType.UIPanel_AnimTools);
            //     }
            // }
            //
            // if (Input.GetKeyDown(KeyCode.W))
            // {
            //     if (JiYuUIHelper.TryGetUI(UIType.UIPanel_AnimTools, out var ui))
            //     {
            //         var uis = ui as UIPanel_AnimTools;
            //         uis.stop = true;
            //         uis.ReSet();
            //     }
            // }


            // if (Input.GetKeyDown(KeyCode.O))
            // {
            //     NetWorkManager.Instance.SendMessage(CMD.GETMAINPROPERTY);
            // }
            // if (Input.GetKeyDown(KeyCode.E))
            // {
            //     if (JiYuUIHelper.TryGetUI(UIType.UIPanel_AnimTools, out var ui))
            //     {
            //         var uis = ui as UIPanel_AnimTools;
            //         uis.stop = true;
            //         uis.ReSet();
            //         UIHelper.Remove(UIType.UIPanel_AnimTools);
            //     }
            // }

            // if (Input.GetKeyDown(KeyCode.F9))
            // {
            //     Log.Debug($"{JiYuUIHelper.GeneralTimeFormat(new int4(1, 3, 2, 1), 4225)}",
            //         Color.cyan);
            // }
            //
            // if (Input.GetKeyDown(KeyCode.F1))
            // {
            //     UIHelper.CreateAsync(UIType.UIPanel_ParasTest);
            // }

            // if (Input.GetKeyDown(KeyCode.F5))
            // {
            //     UIHelper.CreateAsync(UIType.UIPanel_Mail);
            // }
            //
            // if (Input.GetKeyDown(KeyCode.F6))
            // {
            //     UIHelper.CreateAsync(UIType.UIPanel_MonsterCollection);
            // }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                Log.Debug($"重载配置表中...", Color.cyan);
                await ConfigManager.Instance.InitTables();
                Log.Debug($"成功重载配置表", Color.cyan);
            }
        }


        public void DestoryAllToggle()
        {
            CloseTimeView();
            // foreach (var ui in toggleUIList)
            // {
            //     //ui.Dispose();
            //     UnityEngine.GameObject.Destroy(ui.GameObject);
            // }

            //ResourcesSingleton.Instance.DisposeJiYuGamePanel();
            // toggleUIList.Clear();
            // this.Close();
        }

        // public int GetlastTabId()
        // {
        //     return lastSortId;
        // }


        private Vector2[] initialPositions; // 初始位置

        private RectTransform optionRectTransform;

        // public struct JiyuGameData
        // {
        //     public int sort;
        //     public bool init;
        // }
        public void RefreshToggleLanguage()
        {
            //Log.Error($"RefreshToggleLanguage");
            var option = this.GetFromReference(KOptions);
            var list = option.GetList();
            foreach (var VARIABLE in list.Children)
            {
                var ui = VARIABLE as UISubPanel_ToggleItem;
                if (ui.sort == lastSortId)
                {
                    ui.RefreshLanguage();
                    break;
                }
            }
        }

        public void ChangeHorizontalNormalizedPosition(float horizontalNormalizedPosition)
        {
            float tempPos = 0;
            horizontalNormalizedPosition = Mathf.Clamp01(horizontalNormalizedPosition);
            tempPos = horizontalNormalizedPosition;

            //获取拖动的值  
            var index = 0;
            float offset = Mathf.Abs(m_ListPageValue[index] - tempPos); //拖动的绝对值  
            for (int i = 1; i < m_ListPageValue.Count; i++)
            {
                float temp = Mathf.Abs(tempPos - m_ListPageValue[i]);
                if (temp < offset)
                {
                    index = i;
                    offset = temp;
                }
            }

            m_TargetPos = m_ListPageValue[index];
            m_NowIndex = index;
        }

        /// 拖拽结束   
        public void OnEndDrag()
        {
            Log.Debug($"OnEndDrag");
            m_IsDrag = false;
            if (m_ListPageValue.Count == 1)
            {
                return;
            }

            OnStartOnePanel();
            // switch (m_NowIndex)
            // {
            //     case 0:
            //
            //         if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out var ui0))
            //         {
            //             var uis = ui0 as UIPanel_Shop;
            //             uis.scrollRect.SetEnabled(true);
            //         }
            //
            //         break;
            //     case 1:
            //         if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui1))
            //         {
            //             var uis = ui1 as UIPanel_Equipment;
            //             uis.scrollRect.SetEnabled(true);
            //             uis.DestorySelected();
            //             uis.RefreshItemAndEquip();
            //         }
            //
            //         break;
            //     case 2:
            //
            //         if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui2))
            //         {
            //             var uis = ui2 as UIPanel_Main;
            //             uis.scrollRect.SetEnabled(true);
            //         }
            //
            //         break;
            // }
            ChangeHorizontalNormalizedPosition(m_ScrollRect.Get().horizontalNormalizedPosition);
        }


        void EnableTopPos(bool enable)
        {
            //TODO:3
            // var content = GetFromReference(UIPanel_JiyuGame.KContentPos);
            // var topPos = GetFromReference(UIPanel_JiyuGame.KTopPos);
            // float offset = enable ? -186 : 0f;
            // content.GetRectTransform().SetOffsetWithTop(offset);
            // topPos.SetActive(enable);
        }

        public async UniTaskVoid OnBtnClickEvent(int sort)
        {
            //var option = this.GetFromReference(KOptions);
            //var contentPos = this.GetFromReference(KContentPos);
            //var contTransform = contentPos.GetComponent<Transform>();
            //Log.Error($"sort {sort} lastSortId{lastSortId} lastTagId{lastTagId}");
            // if (isInit)
            // {
            //     //Log.Error($"OnBtnClickEvent AudioManager");
            //    
            // }
            AudioManager.Instance.PlayFModAudio(1103);
            if (sort == lastSortId)
            {
                //return;
                switch (lastTagId)
                {
                    case 1:
                        //TODO:

                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui1))
                        {
                            var uis1 = ui1 as UIPanel_Shop;
                            uis1.RotateShop();
                        }

                        break;
                    case 2:
                        if (JiYuUIHelper.TryGetUI(UIType.UISubPanel_Equipment, out UI ui2))
                        {
                            var uis2 = ui2 as UISubPanel_Equipment;
                            uis2.OnLoopClick();
                        }

                        break;
                    case 3:
                        break;
                    case 4:
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Challege, out UI ui4))
                        {
                            var uis4 = ui4 as UIPanel_Challege;
                            uis4.OnLoopClick();
                        }

                        break;
                    case 5:


                        break;
                }
                //return;
            }
            else
            {
                OnStartOnePanel();
                lastSortId = sort;
                JiYuUIHelper.CreateTagPools(sort);
                JiYuTweenHelper.SetEffectUIState(sort);
                switch (sort)
                {
                    case 1:
                        if (ResourcesSingleton.Instance.isUIInit)
                        {
                            AudioManager.Instance.PlayFModAudio(1210);
                        }

                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out UI ui1))
                        {
                            Log.Debug("uipanel_shop", Color.cyan);
                            var uis1 = ui1 as UIPanel_Shop;
                            uis1.tagsTimes = new List<int>() { 1, 1, 1, 1 };
                            uis1?.scrollRect?.SetEnabled(true);
                            uis1.SelectShopStateByTagPosType(uis1.NowPosType1, uis1.NowPosType2);
                        }


                        break;
                    case 2:


                        if (JiYuUIHelper.TryGetUI(UIType.UISubPanel_Equipment, out var ui2))
                        {
                            Log.Debug("UISubPanel_Equipment", Color.cyan);
                            var uis = ui2 as UISubPanel_Equipment;
                            uis.Refresh();
                            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(uis.GetFromReference(UISubPanel_Equipment.KTops), 20,
                                50f);
                        }

                        break;
                    case 3:
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui3))
                        {
                            Log.Debug("UIPanel_Main", Color.cyan);
                            var uis = ui3 as UIPanel_Main;
                            uis.scrollRect.SetEnabled(true);
                        }

                        break;
                    case 4:
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Challege, out UI ui4))
                        {
                            Log.Debug("UIPanel_Challege", Color.cyan);
                            var uis4 = ui4 as UIPanel_Challege;
                            uis4.OnBtnClickEvent(1);
                            var endValue = uis4.GetFromReference(UIPanel_Challege.KScroller_AreaInfo).GetRectTransform()
                                .AnchoredPosition().y;
                            JiYuTweenHelper.SetEaseAlphaAndPosB2U(
                                uis4.GetFromReference(UIPanel_Challege.KScroller_AreaInfo), endValue);
                        }

                        break;
                    case 5:
                        if (ResourcesSingleton.Instance.isUIInit)
                        {
                            //AudioManager.Instance.PlayFModAudio(1241);
                        }

                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Person, out UI ui5))
                        {
                            Log.Debug("UIPanel_Person", Color.cyan);
                            var uis5 = ui5 as UIPanel_Person;

                            uis5.GetFromReference(UIPanel_Person.KMidMonster).GetRectTransform()
                                .SetAnchoredPositionX(-80f);
                            //uis5.GetFromReference(UIPanel_Person.KMidMonster).GetImage().SetAlpha(0);

                            uis5.GetFromReference(UIPanel_Person.KMidAchievement).GetRectTransform()
                                .SetAnchoredPositionX(80f);
                            //uis5.GetFromReference(UIPanel_Person.KMidAchievement).GetImage().SetAlpha(0);

                            uis5.GetFromReference(UIPanel_Person.KMidFriend).GetRectTransform()
                                .SetAnchoredPositionY(80f);
                            //uis5.GetFromReference(UIPanel_Person.KMidFriend).GetImage().SetAlpha(0);

                            uis5.GetFromReference(UIPanel_Person.KMidGonghui).GetRectTransform()
                                .SetAnchoredPositionY(80f);
                            //uis5.GetFromReference(UIPanel_Person.KMidGonghui).GetImage().SetAlpha(0);
                            SetEaseEffectForTag5(uis5).Forget();
                        }


                        break;
                }

                lastTagId = tbtag.DataList.Where(tag => tag.sort == sort).ToList()[0].id;
                await OnButtonClickAnim(sort - 1);

                Log.Debug($"GuidelastTagId{lastTagId}");
                Guide(lastTagId).Forget();
                //await UniTask.Delay(800);
            }


            //lastTabUI?.Dispose();
            //
            // UI panelUI = null;
            // switch (sort)
            // {
            //     case 1:
            //         EnableTopPos(false);
            //         //panelUI = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_Shop, contTransform);
            //
            //         break;
            //     case 2:
            //
            //         EnableTopPos(false);
            //         //panelUI = await UIHelper.CreateAsyncNew(this, UIType.UISubPanel_Equipment, contTransform);
            //
            //         //var panelUI = await UIHelper.CreateAsyncNew(this, UIType.UIEquipScrollView, contTransform);
            //         //panelUI2.GetRectTransform().SetAnchoredPositionY(-111f);
            //
            //         break;
            //     case 3:
            //         EnableTopPos(true);
            //         //panelUI = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_Main, contTransform);
            //
            //         break;
            //     case 4:
            //         EnableTopPos(true);
            //
            //         //panelUI = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_BattleTecnology, contTransform);
            //         //panelUI = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_Challege, contTransform);
            //         break;
            //     case 5:
            //         EnableTopPos(false);
            //         //panelUI = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_Person, contTransform);
            //
            //         break;
            // }

            // panelUI?.SetParent(this, false);
            // //this.AddChild(panelUI);
            // lastTabUI = panelUI;
        }

        private async UniTask SetEaseEffectForTag5(UIPanel_Person uis5)
        {
            await UniTask.Delay(200, cancellationToken: cts.Token);


            JiYuTweenHelper.SetEaseAlphaAndPosLtoR(
                uis5.GetFromReference(UIPanel_Person.KMidMonster), 0, 80, cts.Token, 0.2f, false, false);
            JiYuTweenHelper.SetEaseAlphaAndPosRtoL(
                uis5.GetFromReference(UIPanel_Person.KMidAchievement), 0, 80, cts.Token, 0.2f, false, false);
            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(
                uis5.GetFromReference(UIPanel_Person.KMidFriend), 0, 80, cts.Token, 0.2f, false, false);
            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(
                uis5.GetFromReference(UIPanel_Person.KMidGonghui), 0, 80, cts.Token, 0.2f, false, false);
        }

        public async UniTask OnButtonClickAnim(int index)
        {
            JiYuUIHelper.DestoryAllTips();
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui2))
            {
                var uiMain = ui2 as UIPanel_Main;
                uiMain.BtnTreasure();
            }

            const float Multi = 1.5f;
            const float AnimationDuration = 0.15f; // 动画持续时间
            const float Spacing = 20f; // 按钮之间的间距
            // float firstX = 160;
            // float intelnal = 190;

            // const float OnClickAnimTime = 0.08f;
            // const float OnPressAnimTime = 0.12f;
            // const float LongPressInterval = 0.12f;
            // const int MaxLongPressCount = 1;
            // 计算其他按钮的目标位置
            Vector2[] targetPositions = new Vector2[tbtag.DataList.Count];
            for (int i = 0; i < tbtag.DataList.Count; i++)
            {
                if (i < index)
                {
                    targetPositions[i] = new Vector2(initialPositions[i].x - Spacing, initialPositions[i].y);
                }
                else if (i > index)
                {
                    targetPositions[i] = new Vector2(initialPositions[i].x + Spacing, initialPositions[i].y);
                }
            }

            //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            // 播放其他按钮的移动动画
            for (int i = 0; i < toggleUIList.Count; i++)
            {
                var KText = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KText);
                var KArrow = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KArrow);
                var KContent = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KContent);
                var KImg_Btn = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KImg_Btn);
                var KImg_ArrowLeft = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KImg_ArrowLeft);
                var KImg_ArrowRight = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KImg_ArrowRight);
                var KIcon = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KIcon);
                if (i != index)
                {
                    KText.SetActive(false);

                    KArrow.SetActive(false);

                    //buttons[i].GetComponent<RectTransform>().DOAnchorPos(targetPositions[i], animationDuration);
                    // KContent.GetRectTransform()
                    //     .DoAnchoredPosition(targetPositions[i], animationDuration);
                    toggleUIList[i].GetRectTransform().DoScale(new Vector3(1f, 1f, 1f), AnimationDuration);
                    KContent.GetRectTransform()
                        .DoScale(new Vector3(1f, 1f, 1f), AnimationDuration);


                    //toggleUIList[i].GetImage().SetSpriteAsync("bg_tag_notSelected", false);
                    KImg_Btn.GetImage()
                        .SetSpriteAsync("bg_tag_notSelected", false);
                    //toggleDic[i].GetImage().sp
                    //toggleUIList[i].GetImage().SetAlpha(0);
                    KIcon.GetRectTransform().SetAnchoredPositionY(0);
                }
                else
                {
                    // toggleDic[i].Children["Content"].GetRectTransform()
                    //     .DoAnchoredPosition(new Vector2(firstX + index * intelnal, 75), animationDuration)
                    //     .AddOnCompleted(() => { });

                    KText.SetActive(true);
                    KArrow.SetActive(true);
                    KImg_ArrowLeft.SetActive(i != 0);
                    KImg_ArrowRight.SetActive(i != toggleUIList.Count - 1);
                    //icon&text
                    KContent.GetRectTransform()
                        .DoLocalMove(new Vector3(0, 50f, 0f), AnimationDuration);

                    // KContent.GetRectTransform()
                    //     .DoAnchoredPositionY(50, animationDuration);
                    KContent.GetRectTransform()
                        .DoScale(new Vector3(1f, Multi, 1f), AnimationDuration);
                    KImg_Btn.GetImage()
                        .SetSpriteAsync("bg_tag_isSelected", false);

                    //toggleUIList[i].GetImage().SetSpriteAsync("bg_tag_isSelected", false);
                    //toggleUIList[i].GetImage().SetColor(Color.white);

                    toggleUIList[i].GetRectTransform().DoScale(new Vector3(Multi, 1f, 1f), AnimationDuration);
                    KIcon.GetRectTransform().SetAnchoredPositionY(-25);
                }
            }

            //await UniTask.Delay((int)(animationDuration * 1000f + 100));
            for (int i = 0; i < toggleUIList.Count; i++)
            {
                var KText = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KText);
                var KArrow = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KArrow);
                var KContent = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KContent);
                var KImg_Btn = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KImg_Btn);
                var KImg_ArrowLeft = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KImg_ArrowLeft);
                var KImg_ArrowRight = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KImg_ArrowRight);

                if (i != index)
                {
                    KText.SetActive(false);

                    KArrow.SetActive(false);

                    //buttons[i].GetComponent<RectTransform>().DOAnchorPos(targetPositions[i], animationDuration);
                    KContent.GetRectTransform()
                        .SetAnchoredPosition(targetPositions[i]);
                    // KContent.GetRectTransform()
                    //     .SetAnchoredPositionX(0);

                    toggleUIList[i].GetRectTransform().SetScale(new Vector3(1f, 1f, 1f));
                    KContent.GetRectTransform()
                        .SetScale(new Vector3(1f, 1f, 1f));


                    //toggleUIList[i].GetImage().SetSpriteAsync("bg_tag_notSelected", false);
                    // KImg_Btn.GetImage()
                    //     .SetSpriteAsync("bg_tag_notSelected", false);
                    //toggleDic[i].GetImage().sp
                    //toggleUIList[i].GetImage().SetAlpha(0);
                }
                else
                {
                    // toggleDic[i].Children["Content"].GetRectTransform()
                    //     .DoAnchoredPosition(new Vector2(firstX + index * intelnal, 75), animationDuration)
                    //     .AddOnCompleted(() => { });

                    KText.SetActive(true);
                    KArrow.SetActive(true);
                    KImg_ArrowLeft.SetActive(i != 0);
                    KImg_ArrowRight.SetActive(i != toggleUIList.Count - 1);
                    //icon&text
                    KContent.GetRectTransform()
                        .SetLocalPosition(new Vector3(0, 50f, 0f));
                    KContent.GetRectTransform()
                        .SetScale(new Vector3(1f, Multi, 1f));
                    // KImg_Btn.GetImage()
                    //     .SetSpriteAsync("bg_tag_isSelected", false);

                    //toggleUIList[i].GetImage().SetSpriteAsync("bg_tag_isSelected", false);
                    //toggleUIList[i].GetImage().SetColor(Color.white);

                    toggleUIList[i].GetRectTransform().SetScale(new Vector3(Multi, 1f, 1f));
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(optionRectTransform);

            for (int i = 0; i < toggleUIList.Count; i++)
            {
                var KText = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KText);
                var KArrow = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KArrow);
                var KContent = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KContent);
                var KImg_Btn = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KImg_Btn);
                var KImg_ArrowLeft = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KImg_ArrowLeft);
                var KImg_ArrowRight = toggleUIList[i].GetFromReference(UISubPanel_ToggleItem.KImg_ArrowRight);

                KContent.GetRectTransform().SetAnchoredPositionX(0);
            }

            //await UniTask.Yield();
            LayoutRebuilder.ForceRebuildLayoutImmediate(optionRectTransform);

            await UniTask.Delay((int)(AnimationDuration * 1000));
        }


        protected override void OnClose()
        {
            m_ListPageValue.Clear();
            RemoveTimer();
            RedDotManager.Instance.ClearAllListeners(NodeNames.GetTagRedDotName(5));
            toggleUIList.Clear();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}