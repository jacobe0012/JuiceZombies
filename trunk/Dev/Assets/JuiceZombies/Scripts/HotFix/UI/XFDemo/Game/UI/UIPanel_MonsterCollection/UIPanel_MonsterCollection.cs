//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HotFix_UI;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_MonsterCollection)]
    internal sealed class UIPanel_MonsterCollectionEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_MonsterCollection;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_MonsterCollection>();
        }
    }

    /// <summary>
    /// tag_func 5104 怪物图鉴
    /// </summary>
    public partial class UIPanel_MonsterCollection : UI, IAwake, ILoopScrollRectProvide<UICommon_Label>
    {
        private Tbmonster tbmonster;
        private Tblanguage tblanguage;
        private Tbpower tbpower;
        private Tbweapon tbweapon;
        private Tbracist tbracist;
        private Tbmonster_attr tbmonster_attr;
        private Tbtag_func tbtag_func;
        private Tbmonster_model tbmonster_model;
        private Tbmonster_feature tbmonster_feature;
        private int curPosId;
        private const float BottomTextInterval = 40f;
        private string m_RedDotName;

        private int tagFunc = 5104;

        //private List<monster> powerMonList = new List<monster>();
        private CancellationTokenSource cts = new CancellationTokenSource();

        public void Initialize()
        {
            InitJson();
            InitRedDot();
            InitPanel();
        }

        void InitRedDot()
        {
            m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);
        }

        void InitPanel()
        {
            var KText_TopTitle = GetFromReference(UIPanel_MonsterCollection.KText_TopTitle);
            var KBtn_ReceiveAll = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll);
            var KBtn_ReceiveAll2 = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll2);
            var KText_BtnReceive = GetFromReference(UIPanel_MonsterCollection.KText_BtnReceive);
            // var KText_BottomTitle = GetFromReference(UIPanel_MonsterCollection.KText_BottomTitle);
            // var KBtn_Close = GetFromReference(UIPanel_MonsterCollection.KBtn_Close);
            // var KScrollView_Item = GetFromReference(UIPanel_MonsterCollection.KScrollView_Item);
            var KScrollView_Main = GetFromReference(UIPanel_MonsterCollection.KScrollView_Main);

            var KPopUp = GetFromReference(UIPanel_MonsterCollection.KPopUp);
            var KText_Diamond = GetFromReference(UIPanel_MonsterCollection.KText_Diamond);
            var KImg_DiamondIcon = GetFromReference(UIPanel_MonsterCollection.KImg_DiamondIcon);
            var KMask = GetFromReference(UIPanel_MonsterCollection.KMask);
            var KRewardRoot = GetFromReference(UIPanel_MonsterCollection.KRewardRoot);
            var KTxtRewardNum = GetFromReference(UIPanel_MonsterCollection.KTxtRewardNum);


            KText_BtnReceive.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gainclick").current);


            KPopUp.SetActive(false);
            KMask.SetActive(false);
            KRewardRoot.SetActive(false);
            KText_Diamond.GetTextMeshPro()
                .SetTMPText(
                    UnicornUIHelper.ReturnFormatResourceNum(ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin));


            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_ReceiveAll, () =>
            {
                var str = new StringValue();
                str.Value = $"{curPosId + 1};2";
                NetWorkManager.Instance.SendMessage(CMDOld.RECEIVECOLLECTIONREWARD, str);

                List<int> temp = new List<int>();
                List<Vector3> reList = new List<Vector3>();
                if (curPosId == 0)
                {
                    foreach (var kv in ResourcesSingletonOld.Instance.resMonster.MonsterMap)
                    {
                        var monsterModel = tbmonster_model.GetOrDefault(kv.Key);
                        if (monsterModel != null && ResourcesSingletonOld.Instance.resMonster.MonsterMap[kv.Key] != 1)
                        {
                            temp.Add(kv.Key);
                            reList.Add(new Vector3(2, 0, monsterModel.diamond));
                        }
                    }

                    foreach (var k in temp)
                    {
                        ResourcesSingletonOld.Instance.resMonster.MonsterMap[k] = 1;
                        var itemStr = $"{m_RedDotName}|Pos{curPosId}|{k}";
                        RedDotManager.Instance.SetRedPointCnt(itemStr, 0);
                    }
                }
                else if (curPosId == 1)
                {
                    foreach (var kv in ResourcesSingletonOld.Instance.resMonster.WeaponMap)
                    {
                        var weapon = tbweapon.GetOrDefault(kv.Key);

                        if (weapon != null && ResourcesSingletonOld.Instance.resMonster.WeaponMap[kv.Key] != 1)
                        {
                            temp.Add(kv.Key);
                            reList.Add(new Vector3(2, 0, weapon.diamond));
                        }
                    }

                    //TODO:
                    foreach (var k in temp)
                    {
                        ResourcesSingletonOld.Instance.resMonster.WeaponMap[k] = 1;
                        var itemStr = $"{m_RedDotName}|Pos{curPosId}|{k}";
                        RedDotManager.Instance.SetRedPointCnt(itemStr, 0);
                    }
                }


                temp.Clear();
                //RefreshRedDot(curPowerId);
                //CreateMainItem(curPowerId).Forget();
                UnicornUIHelper.MergeRewardList(reList);
                UIHelper.Create(UIType.UICommon_Reward, reList);
                KBtn_ReceiveAll.GetXButton().SetEnabled(false);
                KBtn_ReceiveAll2.GetXButton().SetEnabled(true);
                KBtn_ReceiveAll.GetImage().SetColor("808080");
            });

            KBtn_ReceiveAll2.GetButton().OnClick.Add(() =>
            {
                UnicornUIHelper.ClearCommonResource();
                Log.Debug($"KBtn_ReceiveAll2");
                UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("book_click_fail_text").current);
            });
            CreateBottomBtnList().Forget();
            UnicornTweenHelper.SetEaseAlphaAndPosUtoB(this.GetFromReference(UIPanel_MonsterCollection.KScrollView_Item0),
                0,
                50f, cancellationToken: cts.Token);
        }

        void RefreshRedDot(int powerId)
        {
            var KCommon_Bottom = GetFromReference(UIPanel_MonsterCollection.KCommon_Bottom);
            var KScrollView_Item0 = KCommon_Bottom.GetFromReference(UICommon_Bottom.KScrollView_Item0);
            var scrollRect = KScrollView_Item0.GetScrollRect();
            var list = scrollRect.Content.GetList();
            foreach (var ui in list.Children)
            {
                var uis = ui as UICommon_BottomBtn;
                var KImg_RedDot = uis.GetFromReference(UICommon_BottomBtn.KImg_RedDot);
                if (uis.id == powerId)
                {
                    bool isRedDot =
                        ResourcesSingletonOld.Instance.resMonster.MonsterMap.Any(pair =>
                        {
                            var monster = tbmonster.GetOrDefault(pair.Key);
                            var monsterAttr = tbmonster_attr.Get(monster.monsterAttrId);
                            var monsterModel = tbmonster_model.GetOrDefault(monsterAttr.bookId);
                            return monster != null && monsterModel.powerId == powerId && pair.Value == 2;
                        });

                    KImg_RedDot.SetActive(isRedDot);
                    break;
                }
                //ResourcesSingletonOld.Instance.resMonster.MonsterMap.
            }
        }

        private async UniTaskVoid CreateBottomBtnList()
        {
            var KCommon_Bottom = GetFromReference(UIPanel_MonsterCollection.KCommon_Bottom);
            var KScrollView_Item0 = KCommon_Bottom.GetFromReference(UICommon_Bottom.KScrollView_Item0);
            var KBtn_Close = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_Close);
            var KText_BottomTitle = KCommon_Bottom.GetFromReference(UICommon_Bottom.KText_BottomTitle);
            var KBtn_TitleInfo = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_TitleInfo);
            var KLeft = KCommon_Bottom.GetFromReference(UICommon_Bottom.KLeft);
            KBtn_TitleInfo.SetActive(false);
            KText_BottomTitle.GetTextMeshPro().SetTMPText(tblanguage.Get("func_5104_name").current);
            await UniTask.Yield();
            const float DefaultWidth = 1074f;
            const float Internal = 120f;
            var leftWidth = KLeft.GetRectTransform().Width();
            var KScrollView_Item0Rect = KScrollView_Item0.GetRectTransform();
            KScrollView_Item0Rect.SetWidth(DefaultWidth - leftWidth - Internal);
            KScrollView_Item0Rect.SetOffsetWithBottom(0);
            KScrollView_Item0Rect.SetOffsetWithTop(0);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, () => { Close(); });
            UnicornUIHelper.DestoryAllTips();
            // var KScrollView_Item = GetFromReference(UIPanel_MonsterCollection.KScrollView_Item);
            var KBtn_ReceiveAll = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll);
            var KBtn_ReceiveAll2 = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll2);

            var scrollRect = KScrollView_Item0.GetScrollRect();
            var list = scrollRect.Content.GetList();
            list.Clear();
            for (int i = 0; i < 2; i++)
            {
                int index = i;
                //var power = tbpower.DataList[i];
                var ui =
                    await list.CreateWithUITypeAsync(UIType.UICommon_BottomBtn, index,
                            false) as
                        UICommon_BottomBtn;

                var KText_BtnReceive = ui.GetFromReference(UICommon_BottomBtn.KText_Btn);
                var KImg_RedDot = ui.GetFromReference(UICommon_BottomBtn.KImg_RedDot);
                var KBg_Mask = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask);
                var KBg_Mask1 = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask1);
                var itemStr = $"{m_RedDotName}|Pos{index}";
                var bottomRedDot = RedDotManager.Instance.GetRedPointCnt(itemStr);
                KImg_RedDot.SetActive(bottomRedDot > 0);

                //Log.Error($"bottomRed {itemStr} {num}");
                RedDotManager.Instance.AddListener(itemStr, (num) =>
                {
                    if (num > 0)
                    {
                        KBtn_ReceiveAll?.GetXButton()?.SetEnabled(true);
                        KBtn_ReceiveAll2?.GetXButton()?.SetEnabled(false);
                        KBtn_ReceiveAll?.GetImage()?.SetColor("22C55E");
                    }
                    else
                    {
                        KBtn_ReceiveAll?.GetXButton()?.SetEnabled(false);
                        KBtn_ReceiveAll2?.GetXButton()?.SetEnabled(true);
                        KBtn_ReceiveAll?.GetImage()?.SetColor("808080");
                    }

                    KImg_RedDot?.SetActive(num > 0);
                });
                var str = index == 0 ? "book_tag_1" : "book_tag_2";

                KText_BtnReceive.GetTextMeshPro().SetTMPText(tblanguage.Get(str).current);

                // var perWidth = KText_BtnReceive.GetTextMeshPro().Get().preferredWidth;
                //
                // ui.GetRectTransform().SetWidth(perWidth + BottomTextInterval);
                KBg_Mask.SetActive(true);
                KBg_Mask1.SetActive(true);

                if (index == 0)
                {
                    curPosId = index;
                    KBg_Mask.SetActive(false);
                    KBg_Mask1.SetActive(false);
                    CreateMainItem(ui.id).Forget();
                }

                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ui, () =>
                {
                    if (curPosId == ui.id)
                    {
                        return;
                    }

                    foreach (var child in list.Children)
                    {
                        var uichild = child as UICommon_BottomBtn;

                        if (uichild.id == curPosId)
                        {
                            var KBg_Mask = uichild.GetFromReference(UICommon_BottomBtn.KBg_Mask);
                            var KBg_Mask1 = uichild.GetFromReference(UICommon_BottomBtn.KBg_Mask1);
                            KBg_Mask.SetActive(true);
                            KBg_Mask1.SetActive(true);
                            //uichild.GetRectTransform().DoScale2(new Vector2(1f, 1f), 0.2f);
                            break;
                        }
                    }

                    KBg_Mask.SetActive(false);
                    KBg_Mask1.SetActive(false);
                    curPosId = ui.id;


                    var itemStr = $"{m_RedDotName}|Pos{curPosId}";
                    RedDotManager.Instance.ClearChildrenListeners(itemStr);

                    CreateMainItem(ui.id).Forget();
                }, 1101);
            }

            list.Sort((a, b) =>
            {
                var uia = a as UICommon_BottomBtn;
                var uib = b as UICommon_BottomBtn;
                return uia.id.CompareTo(uib.id);
            });
            // for (int i = 0; i < tbpower.DataList.Count; i++)
            // {
            //     int index = i;
            //     var power = tbpower.DataList[i];
            //     var ui =
            //         await list.CreateWithUITypeAsync(UIType.UICommon_BottomBtn, power.id,
            //                 false) as
            //             UICommon_BottomBtn;
            //
            //     var KText_BtnReceive = ui.GetFromReference(UICommon_BottomBtn.KText_Btn);
            //     var KImg_RedDot = ui.GetFromReference(UICommon_BottomBtn.KImg_RedDot);
            //     var KBg_Mask = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask);
            //     var KBg_Mask1 = ui.GetFromReference(UICommon_BottomBtn.KBg_Mask1);
            //     var itemStr = $"{m_RedDotName}|Power{power.id}";
            //
            //     KImg_RedDot.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr) > 0);
            //
            //
            //     RedDotManager.Instance.AddListener(itemStr, (num) =>
            //     {
            //         //Log.Error($"bottomRed {itemStr} {num}");
            //         if (num > 0)
            //         {
            //             KBtn_ReceiveAll?.GetXButton()?.SetEnabled(true);
            //             KBtn_ReceiveAll2?.GetXButton()?.SetEnabled(false);
            //             KBtn_ReceiveAll?.GetImage()?.SetColor("22C55E");
            //         }
            //         else
            //         {
            //             KBtn_ReceiveAll?.GetXButton()?.SetEnabled(false);
            //             KBtn_ReceiveAll2?.GetXButton()?.SetEnabled(true);
            //             KBtn_ReceiveAll?.GetImage()?.SetColor("808080");
            //         }
            //
            //         KImg_RedDot?.SetActive(num > 0);
            //     });
            //     KText_BtnReceive.GetTextMeshPro().SetTMPText(tblanguage.Get(power.name).current);
            //
            //     var perWidth = KText_BtnReceive.GetTextMeshPro().Get().preferredWidth;
            //
            //     ui.GetRectTransform().SetWidth(perWidth + BottomTextInterval);
            //     KBg_Mask.SetActive(true);
            //     KBg_Mask1.SetActive(true);
            //
            //     if (index == 0)
            //     {
            //         curPowerId = ui.id;
            //         KBg_Mask.SetActive(false);
            //         KBg_Mask1.SetActive(false);
            //         CreateMainItem(ui.id).Forget();
            //     }
            //
            //     UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ui, () =>
            //     {
            //         if (curPowerId == ui.id)
            //         {
            //             return;
            //         }
            //
            //         foreach (var child in list.Children)
            //         {
            //             var uichild = child as UICommon_BottomBtn;
            //
            //             if (uichild.id == curPowerId)
            //             {
            //                 var KBg_Mask = uichild.GetFromReference(UICommon_BottomBtn.KBg_Mask);
            //                 var KBg_Mask1 = uichild.GetFromReference(UICommon_BottomBtn.KBg_Mask1);
            //                 KBg_Mask.SetActive(true);
            //                 KBg_Mask1.SetActive(true);
            //                 //uichild.GetRectTransform().DoScale2(new Vector2(1f, 1f), 0.2f);
            //                 break;
            //             }
            //         }
            //
            //         KBg_Mask.SetActive(false);
            //         KBg_Mask1.SetActive(false);
            //         curPowerId = ui.id;
            //
            //
            //         var itemStr = $"{m_RedDotName}|Power{curPowerId}";
            //         RedDotManager.Instance.ClearChildrenListeners(itemStr);
            //
            //         CreateMainItem(ui.id).Forget();
            //     });
            // }
        }

        private async UniTaskVoid CreateMainItem(int posId)
        {
            //curPowerId = powerId;
            var KText_TopTitle = GetFromReference(UIPanel_MonsterCollection.KText_TopTitle);
            var KScrollView_Main = GetFromReference(UIPanel_MonsterCollection.KScrollView_Main);
            var KBtn_ReceiveAll = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll);
            var KBtn_ReceiveAll2 = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll2);
            var KPopUp = GetFromReference(UIPanel_MonsterCollection.KPopUp);
            var KText_Diamond = GetFromReference(UIPanel_MonsterCollection.KText_Diamond);
            var KImg_DiamondIcon = GetFromReference(UIPanel_MonsterCollection.KImg_DiamondIcon);
            var KMask = GetFromReference(UIPanel_MonsterCollection.KMask);

            var MyKRewardRoot = GetFromReference(UIPanel_MonsterCollection.KRewardRoot);
            var MyKTxtRewardNum = GetFromReference(UIPanel_MonsterCollection.KTxtRewardNum);


            //RefreshRedDot(curPowerId);
            //powerMonList.Clear();
            int lableCount = 0;
            if (posId == 0)
            {
                lableCount = tbpower.DataList.Where(a => a.switch0 == 1).ToList().Count;
            }
            else if (posId == 1)
            {
                lableCount = tbweapon.DataList.Where(a => a.type <= 3).GroupBy(a => a.type).Select(a => a.First())
                    .ToList().Count;
                //Log.Debug($"lableCount{lableCount}");
            }


            // powerMonList.Sort((a, b) =>
            // {
            //     bool compareNew = ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(a.id) &&
            //                       ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(b.id);
            //     if (compareNew)
            //     {
            //         if (ResourcesSingletonOld.Instance.resMonster.MonsterMap[a.id] == 2 &&
            //             ResourcesSingletonOld.Instance.resMonster.MonsterMap[b.id] == 1)
            //             return -1;
            //         else if (ResourcesSingletonOld.Instance.resMonster.MonsterMap[a.id] == 1 &&
            //                  ResourcesSingletonOld.Instance.resMonster.MonsterMap[b.id] == 2)
            //             return 1;
            //     }
            //
            //     if (ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(a.id) &&
            //         !ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(b.id))
            //         return -1;
            //     else if (!ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(a.id) &&
            //              ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(b.id))
            //         return 1;
            //
            //     return a.id.CompareTo(b.id);
            // });
            //
            // int unLockList =
            //     powerMonList.Count(item => ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(item.id));
            //


            var str = posId == 0 ? "book_tag_1" : "book_tag_2";
            //TODO:
            var allMonsterCount = tbmonster_model.DataList.Where(a => a.powerId > 0).ToList().Count;
            var allWeaponCount = tbweapon.DataList.Count;
            string totalStr = default;
            if (posId == 0)
            {
                totalStr =
                    $"{tblanguage.Get(str).current} {ResourcesSingletonOld.Instance.resMonster.MonsterMap.Count}/{allMonsterCount}";
            }
            else if (posId == 1)
            {
                totalStr =
                    $"{tblanguage.Get(str).current} {ResourcesSingletonOld.Instance.resMonster.WeaponMap.Count}/{allWeaponCount}";
            }


            KText_TopTitle.GetTextMeshPro().SetTMPText(totalStr);

            var itemStr = $"{m_RedDotName}|Pos{posId}";
            var bottomRedDot = RedDotManager.Instance.GetRedPointCnt(itemStr);


            if (bottomRedDot > 0)
            {
                KBtn_ReceiveAll?.GetXButton()?.SetEnabled(true);
                KBtn_ReceiveAll2?.GetXButton()?.SetEnabled(false);
                KBtn_ReceiveAll?.GetImage()?.SetColor("22C55E");
            }
            else
            {
                KBtn_ReceiveAll?.GetXButton()?.SetEnabled(false);
                KBtn_ReceiveAll2?.GetXButton()?.SetEnabled(true);
                KBtn_ReceiveAll?.GetImage()?.SetColor("808080");
            }

            var loopRect = KScrollView_Main.GetLoopScrollRect<UICommon_Label>();
            //loopRect.Dispose();
            //loopRect.Content.GetList().Clear();
            // var list = loopRect.Content.GetList();
            // list.Clear();
            // const float Internal = 18f;
            // const int rowCount = 3;
            // var oneWidth = (Screen.width - (rowCount + 1) * Internal) / (float)rowCount;
            //
            // var grid = loopRect.Content.GetComponent<GridLayoutGroup>();
            // var temp = grid.cellSize;
            // temp.x = oneWidth;
            // grid.cellSize = temp;


            loopRect.SetProvideData(UIPathSet.UICommon_Label, this);
            loopRect.SetTotalCount(lableCount);
            loopRect.RefillCells();


            // var scrollRect = KScrollView_Main.GetScrollRect();
            // var list = scrollRect.Content.GetList();
            //
            // //scrollRect.Content.SetActive(false);
            // list.Clear();
            //  for (int i = 0; i < powerMonList.Count; i++)
            // {
            //     int index = i;
            //     var monster = powerMonList[index];
            //
            //     var ui =
            //         await list.CreateWithUITypeAsync(UIType.UISubPanel_Collection_Item, monster.id,
            //                 false) as
            //             UISubPanel_Collection_Item;
            //     var KTxtUnlock = ui.GetFromReference(UISubPanel_Collection_Item.KTxtUnlock);
            //     var KTxtNew = ui.GetFromReference(UISubPanel_Collection_Item.KTxtNew);
            //     var KTxtName = ui.GetFromReference(UISubPanel_Collection_Item.KTxtName);
            //     var KTxtRewardNum = ui.GetFromReference(UISubPanel_Collection_Item.KTxtRewardNum);
            //     var KImgEnemy = ui.GetFromReference(UISubPanel_Collection_Item.KImgEnemy);
            //     var KImgBg = ui.GetFromReference(UISubPanel_Collection_Item.KImgBg);
            //     var KUnLock = ui.GetFromReference(UISubPanel_Collection_Item.KUnLock);
            //     var KDislayRoot = ui.GetFromReference(UISubPanel_Collection_Item.KDislayRoot);
            //
            //     var KImgBorder1 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBorder1);
            //     var KImgBorder2 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBorder2);
            //     var KImgBandage1 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBandage1);
            //     var KImgBandage2 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBandage2);
            //     var KImgUnlock = ui.GetFromReference(UISubPanel_Collection_Item.KImgUnlock);
            //
            //
            //     KTxtUnlock.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_locking").current);
            //     KTxtNew.GetTextMeshPro().SetTMPText(tblanguage.Get("text_new_2").current);
            //     KTxtRewardNum.GetTextMeshPro().SetTMPText(monster.diamond.ToString());
            //     KTxtName.GetTextMeshPro().SetTMPText(tblanguage.Get(monster.name).current);
            //     KImgBg.GetImage().SetSpriteAsync(monster.bg, false).Forget();
            //     var monsterAttr = tbmonster_attr.GetOrDefault(monster.monsterAttrId);
            //     if (monsterAttr != null)
            //     {
            //         var picStr = $"{monsterAttr.spine}_pic";
            //         var xImage = KImgEnemy.GetXImage();
            //         xImage.SetSprite(picStr, false);
            //
            //         var picRect = KImgEnemy.GetRectTransform();
            //
            //         //xImage.Get(). activeSprite
            //         if (monster.picCutPara.Count > 0)
            //         {
            //             var picPadding = new Vector4();
            //             picPadding.x = 1 - (monster.picCutPara[0].y / 10000f);
            //             picPadding.y = 1 - (monster.picCutPara[1].y / 10000f);
            //             picPadding.z = 1 - (monster.picCutPara[2].y / 10000f);
            //             picPadding.w = 1 - (monster.picCutPara[3].y / 10000f);
            //             xImage.SetCutting(picPadding);
            //         }
            //
            //         if (monster.picPosPara.Count > 0)
            //         {
            //             var posX = picRect.Width() * (monster.picPosPara[0].y / 10000f);
            //             var posY = picRect.Height() * (monster.picPosPara[0].z / 10000f);
            //             var scale = monster.picPosPara[0].x / 10000f;
            //             picRect.SetScale(new Vector3(scale, scale, scale));
            //             picRect.SetAnchoredPosition(new Vector2(posX, posY));
            //         }
            //
            //         xImage.SetNativeSize();
            //     }
            //
            //     KImgBg.SetActive(true);
            //     if (ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(monster.id))
            //     {
            //         KUnLock.SetActive(false);
            //         KTxtName.SetActive(true);
            //         //1：已领取 2：未领取
            //         var itemStr = $"{m_RedDotName}|Power{monster.powerId}|{monster.id}";
            //         KDislayRoot.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr) > 0);
            //
            //         KImgBorder1.GetComponent<XImage>().SetGrayed(false);
            //         KImgBorder2.GetComponent<XImage>().SetGrayed(false);
            //         KImgBandage1.GetComponent<XImage>().SetGrayed(false);
            //         KImgBandage2.GetComponent<XImage>().SetGrayed(false);
            //         KImgUnlock.GetComponent<XImage>().SetGrayed(false);
            //         KImgEnemy.GetComponent<XImage>().SetGrayed(false);
            //         KImgBg.GetComponent<XImage>().SetGrayed(false);
            //
            //         RedDotManager.Instance.AddListener(itemStr, (num) =>
            //         {
            //             //Log.Error($"RedDotManager {itemStr} {num}");
            //             KDislayRoot?.SetActive(num > 0);
            //         });
            //     }
            //     else
            //     {
            //         KTxtName.SetActive(false);
            //
            //         KUnLock.SetActive(true);
            //         KDislayRoot.SetActive(false);
            //
            //         KImgBorder1.GetComponent<XImage>().SetGrayed(true);
            //         KImgBorder2.GetComponent<XImage>().SetGrayed(true);
            //         KImgBandage1.GetComponent<XImage>().SetGrayed(true);
            //         KImgBandage2.GetComponent<XImage>().SetGrayed(true);
            //         KImgUnlock.GetComponent<XImage>().SetGrayed(true);
            //         KImgEnemy.GetComponent<XImage>().SetGrayed(true);
            //         KImgBg.GetComponent<XImage>().SetGrayed(true);
            //     }
            //
            //     //
            //     UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ui, async () =>
            //     {
            //         if (!ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(monster.id))
            //         {
            //             UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("book_lock_text").current);
            //             return;
            //         }
            //
            //         bool isUnLock = true;
            //         //var monsterConfig = tbmonster.Get(monster.id);
            //         var uiUnlock = await UIHelper.CreateAsync(UIType.UIPanel_Collection_Unlock, monster.id);
            //
            //
            //         var KAnimalRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KAnimalRoot);
            //         var KTxtEnemyName = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtEnemyName);
            //         var KTxtNew = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtNew);
            //         var KRewardRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KRewardRoot);
            //         var KImgReward = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KImgReward);
            //         var KTxtRewardNum = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtRewardNum);
            //         var KTxtPower = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtPower);
            //         var KTxtWeapon = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtWeapon);
            //         var KTxtRacist = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtRacist);
            //         var KTxtDse = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtDse);
            //
            //         KTxtEnemyName.GetTextMeshPro().SetTMPText(tblanguage.Get(monster.name).current);
            //
            //         KTxtNew.GetTextMeshPro().SetTMPText(tblanguage.Get("text_new_2").current);
            //         KTxtRewardNum.GetTextMeshPro().SetTMPText(monster.diamond.ToString());
            //         KTxtPower.GetTextMeshPro()
            //             .SetTMPText(tblanguage.Get(tbpower.Get(monster.powerId).name).current);
            //         KTxtWeapon.GetTextMeshPro()
            //             .SetTMPText(tblanguage.Get(tbmonster_weapon.Get(monster.monsterWeaponId).name).current);
            //         KTxtRacist.GetTextMeshPro()
            //             .SetTMPText(tblanguage.Get(tbracist.Get(monster.racistId).name).current);
            //         KTxtDse.GetTextMeshPro().SetTMPText(tblanguage.Get(monster.desc).current);
            //         KRewardRoot.SetActive(ResourcesSingletonOld.Instance.resMonster.MonsterMap[monster.id] == 2);
            //         uiUnlock.GetButton().OnClick.Add(async () =>
            //         {
            //             uiUnlock.SetActive(false);
            //             KMask.SetActive(true);
            //             isUnLock = false;
            //             if (KRewardRoot.GameObject.activeSelf)
            //             {
            //                 var str = new StringValue();
            //                 str.Value = $"1;{monster.id}";
            //                 NetWorkManager.Instance.SendMessage(CMDOld.RECEIVECOLLECTIONREWARD, str);
            //                 ResourcesSingletonOld.Instance.resMonster.MonsterMap[monster.id] = 1;
            //                 var itemStr = $"{m_RedDotName}|Power{monster.powerId}|{monster.id}";
            //                 RedDotManager.Instance.SetRedPointCnt(itemStr, 0);
            //                 SortCollectionItems();
            //
            //                 //TODO:播放动画 
            //                 KPopUp.SetActive(true);
            //                 var popRec = KPopUp.GetRectTransform();
            //                 var iconRec = KImg_DiamondIcon.GetRectTransform();
            //                 var textRec = KText_Diamond.GetRectTransform();
            //                 var textCom = KText_Diamond.GetTextMeshPro();
            //                 iconRec.SetScale(Vector2.one);
            //                 textRec.SetScale(Vector2.one);
            //                 popRec.DoAnchoredPosition(new Vector2(popRec.Width(), popRec.AnchoredPosition().y),
            //                     new Vector2(0, popRec.AnchoredPosition().y), 0.5f);
            //
            //                 MyKTxtRewardNum.GetTextMeshPro().SetTMPText(monster.diamond.ToString());
            //                 var MyKRewardRootRec = MyKRewardRoot.GetRectTransform();
            //                 MyKRewardRootRec.SetAnchoredPosition(Vector2.zero);
            //                 MyKRewardRoot.SetActive(true);
            //
            //
            //                 await UniTask.Delay(500);
            //                 var finallyPos = UnicornUIHelper.GetUIPos(KPopUp);
            //                 finallyPos.x -= popRec.Width() / 2f;
            //                 finallyPos.y -= popRec.Height() / 2f;
            //                 MyKRewardRootRec.DoAnchoredPosition(Vector2.zero, finallyPos, 1f).AddOnCompleted(() =>
            //                 {
            //                     MyKRewardRoot.SetActive(false);
            //                 });
            //                 iconRec.DoScale2(new Vector2(1.2f, 1.2f), 1f);
            //                 textRec.DoScale2(new Vector2(1.2f, 1.2f), 1f);
            //
            //                 var everyTime = 1000 / monster.diamond;
            //
            //                 for (int j = 0; j < monster.diamond; j++)
            //                 {
            //                     var index = j + 1;
            //                     textCom.SetTMPText($"{ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin + index}");
            //                     await UniTask.Delay(everyTime);
            //                 }
            //
            //                 ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin += monster.diamond;
            //
            //                 // m_Tweener = m_NumText.transform.DOScale(3f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
            //                 // {
            //                 //     m_NumText.text = (++m_Num).ToString();
            //                 //     m_NumText.transform.DOScale(1f, 0.25f).OnComplete(() =>
            //                 //     {
            //                 //         m_tweenNum--;
            //                 //         DoTween();
            //                 //     });
            //                 // });
            //
            //                 await UniTask.Delay(500);
            //
            //                 popRec.DoAnchoredPosition(
            //                     new Vector2(popRec.AnchoredPosition().x, popRec.AnchoredPosition().y),
            //                     new Vector2(popRec.Width(), popRec.AnchoredPosition().y), 0.5f);
            //                 await UniTask.Delay(500);
            //                 KPopUp.SetActive(false);
            //
            //                 //Log.Error($"SetRedPointCnt {itemStr}");
            //                 //CreateMainItem(curPowerId).Forget();
            //             }
            //
            //             KMask.SetActive(false);
            //             uiUnlock.Dispose();
            //         });
            //
            //         var skeletonGraphic = KAnimalRoot.GetComponent<SkeletonGraphic>();
            //         skeletonGraphic.skeletonDataAsset =
            //             await ResourcesManager.LoadAssetAsync<SkeletonDataAsset>(
            //                 "spine_monster_common_Json_SkeletonData");
            //
            //
            //         // foreach (var VARIABLE in skeletonGraphic.SkeletonData.Skins)
            //         // {
            //         //     Debug.LogError($"--{VARIABLE.Name}--");
            //         // }
            //
            //         skeletonGraphic.Initialize(true);
            //         //TODO:读表换成spine
            //         //Debug.LogError($"LogError {tbmonster_attr.Get(monster.monsterAttrId).spine}");
            //         skeletonGraphic.Skeleton.SetSkin(tbmonster_attr.Get(monster.monsterAttrId).spine);
            //         skeletonGraphic.Skeleton.SetSlotsToSetupPose(); // 重置插槽姿势
            //
            //         var weaponStr = $"weapon/{tbmonster_weapon.Get(monster.monsterWeaponId).name}";
            //         skeletonGraphic.Skeleton.SetAttachment("weapon_solt", weaponStr);
            //
            //
            //         uiUnlock.SetActive(true);
            //
            //         while (isUnLock)
            //         {
            //             int index = Random.Range(0, skeletonGraphic.SkeletonData.Animations.Items.Length);
            //             string randomAnimationName =
            //                 skeletonGraphic.SkeletonData.Animations.Items[index].Name;
            //             if (randomAnimationName.Contains("die"))
            //             {
            //                 continue;
            //             }
            //
            //             float duration =
            //                 skeletonGraphic.SkeletonData.Animations.Items[index].Duration;
            //             skeletonGraphic.AnimationState.SetAnimation(0, randomAnimationName, false);
            //
            //             //var skeletonData = skeletonGraphic.skeletonDataAsset.GetSkeletonData(false);
            //             // 获取动画信息
            //             //var animation = skeletonData.FindAnimation(randomAnimationName);
            //             //var time = skeletonGraphic.AnimationState.GetCurrent(0).AnimationTime;
            //             //Log.Error($"{randomAnimationName} finished");
            //             await UniTask.Delay((int)(duration * 1000) + 500);
            //
            //             //skeletonGraphic.AnimationState.SetAnimation(0, "Player_Stand", true);
            //         }
            //     });
            // }

            //SortCollectionItems();

            //scrollRect.Content.SetActive(true);
        }

        void SortCollectionItems1()
        {
            var KScrollView_Main = GetFromReference(UIPanel_MonsterCollection.KScrollView_Main);
            var loopRect = KScrollView_Main.GetLoopScrollRect<UISubPanel_Collection_Item>();
            var list = loopRect.Content.GetList();
            list.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Collection_Item;
                var uib = b as UISubPanel_Collection_Item;

                bool compareNew = ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uia.monsterId) &&
                                  ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uib.monsterId);
                if (compareNew)
                {
                    if (ResourcesSingletonOld.Instance.resMonster.MonsterMap[uia.monsterId] == 2 &&
                        ResourcesSingletonOld.Instance.resMonster.MonsterMap[uib.monsterId] == 1)
                        return -1;
                    else if (ResourcesSingletonOld.Instance.resMonster.MonsterMap[uia.monsterId] == 1 &&
                             ResourcesSingletonOld.Instance.resMonster.MonsterMap[uib.monsterId] == 2)
                        return 1;
                }

                if (ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uia.monsterId) &&
                    !ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uib.monsterId))
                    return -1;
                else if (!ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uia.monsterId) &&
                         ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uib.monsterId))
                    return 1;

                return uia.monsterId.CompareTo(uib.monsterId);
            });
            //UnicornUIHelper.ForceRefreshLayout(scrollRect.Content);
        }

        void SortCollectionItems()
        {
            var KScrollView_Main = GetFromReference(UIPanel_MonsterCollection.KScrollView_Main);
            var loopRect = KScrollView_Main.GetLoopScrollRect<UICommon_Label>();
            var list0 = loopRect.Content.GetList();
            foreach (var child in list0.Children)
            {
                var KContainer_Content = child.GetFromReference(UICommon_Label.KContainer_Content);
                var list = KContainer_Content.GetList();

                if (curPosId == 0)
                {
                    list.Sort((a, b) =>
                    {
                        var uia = a as UISubPanel_Collection_Item;
                        var uib = b as UISubPanel_Collection_Item;

                        bool compareNew =
                            ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uia.monsterId) &&
                            ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uib.monsterId);
                        if (compareNew)
                        {
                            if (ResourcesSingletonOld.Instance.resMonster.MonsterMap[uia.monsterId] == 2 &&
                                ResourcesSingletonOld.Instance.resMonster.MonsterMap[uib.monsterId] == 1)
                                return -1;
                            else if (ResourcesSingletonOld.Instance.resMonster.MonsterMap[uia.monsterId] == 1 &&
                                     ResourcesSingletonOld.Instance.resMonster.MonsterMap[uib.monsterId] == 2)
                                return 1;
                        }

                        if (ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uia.monsterId) &&
                            !ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uib.monsterId))
                            return -1;
                        else if (!ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uia.monsterId) &&
                                 ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uib.monsterId))
                            return 1;

                        return uia.monsterId.CompareTo(uib.monsterId);
                    });
                }
                else if (curPosId == 1)
                {
                    list.Sort((a, b) =>
                    {
                        var uia = a as UISubPanel_Collection_Item;
                        var uib = b as UISubPanel_Collection_Item;

                        bool compareNew = ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uia.monsterId) &&
                                          ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uib.monsterId);
                        if (compareNew)
                        {
                            if (ResourcesSingletonOld.Instance.resMonster.WeaponMap[uia.monsterId] == 2 &&
                                ResourcesSingletonOld.Instance.resMonster.WeaponMap[uib.monsterId] == 1)
                                return -1;
                            else if (ResourcesSingletonOld.Instance.resMonster.WeaponMap[uia.monsterId] == 1 &&
                                     ResourcesSingletonOld.Instance.resMonster.WeaponMap[uib.monsterId] == 2)
                                return 1;
                        }

                        if (ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uia.monsterId) &&
                            !ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uib.monsterId))
                            return -1;
                        else if (!ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uia.monsterId) &&
                                 ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uib.monsterId))
                            return 1;

                        return uia.monsterId.CompareTo(uib.monsterId);
                    });
                }
            }


            //UnicornUIHelper.ForceRefreshLayout(scrollRect.Content);
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbpower = ConfigManager.Instance.Tables.Tbpower;
            tbmonster = ConfigManager.Instance.Tables.Tbmonster;
            tbweapon = ConfigManager.Instance.Tables.Tbweapon;
            tbracist = ConfigManager.Instance.Tables.Tbracist;
            tbmonster_attr = ConfigManager.Instance.Tables.Tbmonster_attr;
            tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            tbmonster_model = ConfigManager.Instance.Tables.Tbmonster_model;
            tbmonster_feature = ConfigManager.Instance.Tables.Tbmonster_feature;
        }


        async UniTaskVoid PlayAnim(int diamond)
        {
            var KText_TopTitle = GetFromReference(UIPanel_MonsterCollection.KText_TopTitle);
            var KScrollView_Main = GetFromReference(UIPanel_MonsterCollection.KScrollView_Main);
            var KBtn_ReceiveAll = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll);
            var KBtn_ReceiveAll2 = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll2);
            var KPopUp = GetFromReference(UIPanel_MonsterCollection.KPopUp);
            var KText_Diamond = GetFromReference(UIPanel_MonsterCollection.KText_Diamond);
            var KImg_DiamondIcon = GetFromReference(UIPanel_MonsterCollection.KImg_DiamondIcon);
            var KMask = GetFromReference(UIPanel_MonsterCollection.KMask);
            var MyKRewardRoot = GetFromReference(UIPanel_MonsterCollection.KRewardRoot);
            var MyKTxtRewardNum = GetFromReference(UIPanel_MonsterCollection.KTxtRewardNum);
            KMask.SetActive(true);
            //TODO:播放动画 
            KPopUp.SetActive(true);
            var popRec = KPopUp.GetRectTransform();
            var iconRec = KImg_DiamondIcon.GetRectTransform();
            var textRec = KText_Diamond.GetRectTransform();
            var textCom = KText_Diamond.GetTextMeshPro();
            iconRec.SetScale(Vector2.one);
            textRec.SetScale(Vector2.one);
            popRec.DoAnchoredPosition(new Vector2(popRec.Width(), popRec.AnchoredPosition().y),
                new Vector2(0, popRec.AnchoredPosition().y), 0.5f);

            var str = UnicornUIHelper.GetRewardTextIconName("icon_diamond");

            MyKTxtRewardNum.GetTextMeshPro().SetTMPText(diamond.ToString());
            var MyKRewardRootRec = MyKRewardRoot.GetRectTransform();
            MyKRewardRootRec.SetAnchoredPosition(Vector2.zero);
            MyKRewardRoot.SetActive(true);


            await UniTask.Delay(500);
            var finallyPos = UnicornUIHelper.GetUIPos(KPopUp);
            finallyPos.x -= popRec.Width() / 2f;
            finallyPos.y -= popRec.Height() / 2f;
            MyKRewardRootRec.DoAnchoredPosition(Vector2.zero, finallyPos, 1f).AddOnCompleted(() =>
            {
                MyKRewardRoot.SetActive(false);
            });
            iconRec.DoScale2(new Vector2(1.2f, 1.2f), 1f);
            textRec.DoScale2(new Vector2(1.2f, 1.2f), 1f);

            var everyTime = 1000 / diamond;

            for (int j = 0; j < diamond; j++)
            {
                var index = j + 1;
                long count = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin + index;
                var countStr = UnicornUIHelper.FormatNumber(count);
                textCom.SetTMPText($"{countStr}");
                await UniTask.Delay(everyTime);
            }

            ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin += diamond;

            // m_Tweener = m_NumText.transform.DOScale(3f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
            // {
            //     m_NumText.text = (++m_Num).ToString();
            //     m_NumText.transform.DOScale(1f, 0.25f).OnComplete(() =>
            //     {
            //         m_tweenNum--;
            //         DoTween();
            //     });
            // });

            await UniTask.Delay(500);

            popRec.DoAnchoredPosition(
                new Vector2(popRec.AnchoredPosition().x, popRec.AnchoredPosition().y),
                new Vector2(popRec.Width(), popRec.AnchoredPosition().y), 0.5f);
            await UniTask.Delay(500);
            KPopUp.SetActive(false);

            //Log.Error($"SetRedPointCnt {itemStr}");
            //CreateMainItem(curPowerId).Forget();


            KMask.SetActive(false);
        }

        public async void ProvideData(UICommon_Label lableui, int index)
        {
            const string unlockStr = "? ? ?";

            var KText_TopTitle = GetFromReference(UIPanel_MonsterCollection.KText_TopTitle);
            var KScrollView_Main = GetFromReference(UIPanel_MonsterCollection.KScrollView_Main);
            var KBtn_ReceiveAll = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll);
            var KBtn_ReceiveAll2 = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll2);
            var KPopUp = GetFromReference(UIPanel_MonsterCollection.KPopUp);
            var KText_Diamond = GetFromReference(UIPanel_MonsterCollection.KText_Diamond);
            var KImg_DiamondIcon = GetFromReference(UIPanel_MonsterCollection.KImg_DiamondIcon);
            var KMask = GetFromReference(UIPanel_MonsterCollection.KMask);
            var MyKRewardRoot = GetFromReference(UIPanel_MonsterCollection.KRewardRoot);
            var MyKTxtRewardNum = GetFromReference(UIPanel_MonsterCollection.KTxtRewardNum);
            if (curPosId == 0)
            {
                var powerList = tbpower.DataList.Where(a => a.switch0 == 1).OrderBy(a => a.sort).ToList();
                //var power = tbpower.DataList.Where(a => a.sort == index + 1).First();
                var power = powerList[index];
                ObjectHelper.Awake(lableui, power.id);
                var verticalLayoutGroup = lableui.GetComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.padding.left = 0;
                var KText_Label = lableui.GetFromReference(UICommon_Label.KText_Label);
                var KText_LabelR = lableui.GetFromReference(UICommon_Label.KText_LabelR);
                var KContainer_Content = lableui.GetFromReference(UICommon_Label.KContainer_Content);

                var powerAllList = tbmonster_model.DataList.Where(a => a.powerId == power.id).ToList();
                var powerUnlockedList = ResourcesSingletonOld.Instance.resMonster.MonsterMap.Keys.ToList()
                    .Where(a => tbmonster_model.Get(a).powerId == power.id);

                KText_LabelR.SetActive(true);
                var titleStr =
                    $"{tblanguage.Get($"{power.name}").current}";
                titleStr = powerUnlockedList.Count() > 0 ? titleStr : unlockStr;
                KText_Label.GetTextMeshPro().SetTMPText(titleStr);
                KText_LabelR.GetTextMeshPro().SetTMPText($"{powerUnlockedList.Count()}/{powerAllList.Count}");
                var gridLayoutGroup = KContainer_Content.GetComponent<GridLayoutGroup>();
                gridLayoutGroup.cellSize = new Vector2(369f, 568f);
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = 3;
                var temp = gridLayoutGroup.spacing;
                temp.x = 14f;
                gridLayoutGroup.spacing = temp;
                var itemList = KContainer_Content.GetList();
                itemList.Clear();
                foreach (var monsterAttr in powerAllList)
                {
                    int bookId = monsterAttr.id;
                    var ui = await itemList.CreateWithUITypeAsync(UIType.UISubPanel_Collection_Item, bookId, false);
                    var itemRec = ui.GetRectTransform();
                    itemRec.SetScale(Vector3.one);
                    var KTxtUnlock = ui.GetFromReference(UISubPanel_Collection_Item.KTxtUnlock);
                    var KTxtNew = ui.GetFromReference(UISubPanel_Collection_Item.KTxtNew);
                    var KTxtName = ui.GetFromReference(UISubPanel_Collection_Item.KTxtName);
                    var KTxtRewardNum = ui.GetFromReference(UISubPanel_Collection_Item.KTxtRewardNum);
                    var KImgBg = ui.GetFromReference(UISubPanel_Collection_Item.KImgBg);
                    var KUnLock = ui.GetFromReference(UISubPanel_Collection_Item.KUnLock);
                    var KDislayRoot = ui.GetFromReference(UISubPanel_Collection_Item.KDislayRoot);
                    var KReward = ui.GetFromReference(UISubPanel_Collection_Item.KReward);

                    var KImgBorder1 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBorder1);
                    var KImgBorder2 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBorder2);
                    var KImgBandage1 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBandage1);
                    var KImgBandage2 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBandage2);
                    var KImgUnlock = ui.GetFromReference(UISubPanel_Collection_Item.KImgUnlock);
                    //var KSpineEnemy = ui.GetFromReference(UISubPanel_Collection_Item.KSpineEnemy);
                    var KImgEnemy = ui.GetFromReference(UISubPanel_Collection_Item.KImgEnemy);
                    var KImgBgQuality = ui.GetFromReference(UISubPanel_Collection_Item.KImgBgQuality);
                    var KImg_Icon = ui.GetFromReference(UISubPanel_Collection_Item.KImg_Icon);
                    var KImgMask = ui.GetFromReference(UISubPanel_Collection_Item.KImgMask);

                    //
                    KTxtUnlock.SetActive(false);
                    KReward.SetActive(true);
                    //KTxtUnlock.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_locking").current);
                    KTxtNew.GetTextMeshPro().SetTMPText(tblanguage.Get("text_new_2").current);
                    var str = UnicornUIHelper.GetRewardTextIconName("icon_diamond");
                    KTxtRewardNum.GetTextMeshPro().SetTMPText(monsterAttr.diamond.ToString());
                    UnicornUIHelper.ForceRefreshLayout(KReward);
                    KTxtName.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterAttr.name).current);

                    //bg_monster_icons_1
                    var bgStr = monsterAttr.bookType == 2 ? "bg_monster_pic_2" : "bg_monster_pic_1";
                    var bgQualityStr = monsterAttr.bookType == 2
                        ? "collection_bg_quality_3"
                        : "collection_bg_quality_1";
                    KImgMask.GetImage().SetSpriteAsync("Img_Mask_Monster", false).Forget();
                    KImgBg.GetImage().SetSpriteAsync(bgStr, false).Forget();
                    KImgBgQuality.GetImage().SetSpriteAsync(bgQualityStr, false).Forget();
                    //var monsterAttr = tbmonster_attr.Get(monster.monsterAttrId);

                    // var sg = KSpineEnemy.GetComponent<SkeletonGraphic>();
                    // if (monsterAttr != null)
                    // {
                    //     //Log.Debug($"model spine {monsterAttr.model} {monsterAttr.spine}");
                    //
                    //     UnicornUIHelper.SetSpine(sg, monsterAttr.spine, monsterAttr.model);
                    //
                    //     // var picStr = $"{monsterAttr.spine}_pic";
                    //     // var xImage = KImgEnemy.GetXImage();
                    //     // xImage.SetSprite(picStr, false);
                    //     //
                    //     // var picRect = KImgEnemy.GetRectTransform();
                    //     //
                    //     // //xImage.Get(). activeSprite
                    //     // if (monster.picCutPara.Count > 0)
                    //     // {
                    //     //     var picPadding = new Vector4();
                    //     //     picPadding.x = 1 - (monster.picCutPara[0].y / 10000f);
                    //     //     picPadding.y = 1 - (monster.picCutPara[1].y / 10000f);
                    //     //     picPadding.z = 1 - (monster.picCutPara[2].y / 10000f);
                    //     //     picPadding.w = 1 - (monster.picCutPara[3].y / 10000f);
                    //     //     xImage.SetCutting(picPadding);
                    //     // }
                    //     //
                    //     // if (monster.picPosPara.Count > 0)
                    //     // {
                    //     //     var posX = picRect.Width() * (monster.picPosPara[0].y / 10000f);
                    //     //     var posY = picRect.Height() * (monster.picPosPara[0].z / 10000f);
                    //     //     var scale = monster.picPosPara[0].x / 10000f;
                    //     //     picRect.SetScale(new Vector3(scale, scale, scale));
                    //     //     picRect.SetAnchoredPosition(new Vector2(posX, posY));
                    //     // }
                    //
                    //     //xImage.SetNativeSize();
                    // }


                    KImgBg.SetActive(true);
                    KTxtName.SetActive(true);
                    KUnLock.SetActive(false);
                    KImgEnemy.SetActive(false);
                    KImg_Icon.SetActive(false);

                    if (ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(bookId))
                    {
                        if (ResourcesSingletonOld.Instance.resMonster.MonsterMap[bookId] == 1)
                        {
                            KReward.SetActive(false);
                        }

                        //1：已领取 2：未领取

                        KImgEnemy.SetActive(true);
                        KImgEnemy.GetImage().SetSpriteAsync($"{monsterAttr.bookPic}", true).Forget();
                        var itemStr = $"{m_RedDotName}|Pos{curPosId}|{bookId}";
                        KDislayRoot.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr) > 0);
                        KReward.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr) > 0);
                        KImgBorder1.GetComponent<XImage>().SetGrayed(false);
                        KImgBorder2.GetComponent<XImage>().SetGrayed(false);
                        KImgBandage1.GetComponent<XImage>().SetGrayed(false);
                        KImgBandage2.GetComponent<XImage>().SetGrayed(false);
                        KImgUnlock.GetComponent<XImage>().SetGrayed(false);
                        //KImgEnemy.GetComponent<XImage>().SetGrayed(false);

                        //sg.material = ResourcesManager.LoadAsset<Material>("SpineNormal");
                        KImgMask.SetActive(false);

                        RedDotManager.Instance.AddListener(itemStr, (num) =>
                        {
                            //Log.Error($"RedDotManager {itemStr} {num}");
                            KDislayRoot?.SetActive(num > 0);
                        });
                    }
                    else
                    {
                        KImg_Icon.SetActive(true);
                        KImg_Icon.GetXImage().SetSpriteAsync("pic_monstercollection_locked", true);
                        KTxtName.GetTextMeshPro().SetTMPText(unlockStr);

                        KDislayRoot.SetActive(false);

                        KImgBorder1.GetComponent<XImage>().SetGrayed(true);
                        KImgBorder2.GetComponent<XImage>().SetGrayed(true);
                        KImgBandage1.GetComponent<XImage>().SetGrayed(true);
                        KImgBandage2.GetComponent<XImage>().SetGrayed(true);
                        KImgUnlock.GetComponent<XImage>().SetGrayed(true);
                        //KImgEnemy.GetComponent<XImage>().SetGrayed(true);
                        //sg.material = ResourcesManager.LoadAsset<Material>("SpineGray");
                        //sg.material = ResourcesManager.LoadAsset<Material>("SpineNormal");
                        KImgMask.SetActive(true);
                        //KImgBg.GetComponent<XImage>().SetGrayed(true);
                    }

                    //
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ui, async () =>
                    {
                        if (!ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(bookId))
                        {
                            UnicornUIHelper.ClearCommonResource();
                            UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("book_lock_text").current);
                            return;
                        }

                        bool isUnLock = true;
                        //var monsterConfig = tbmonster.Get(monster.id);
                        var uiUnlock = await UIHelper.CreateAsync(UIType.UIPanel_Collection_Unlock, bookId);
                        await UnicornUIHelper.InitBlur(uiUnlock);

                        var KAnimalRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KAnimalRoot);
                        var KTxtEnemyName = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtEnemyName);
                        var KTxtNew = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtNew);
                        var KRewardRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KRewardRoot);
                        var KImgReward = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KImgReward);
                        var KTxtRewardNum = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtRewardNum);
                        var KTxt1 = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxt1);
                        var KTxt2 = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxt2);
                        var KTxt3 = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxt3);
                        var KImg3 = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KImg3);

                        var KTxtDse = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtDse);
                        var KTagRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTagRoot);
                        var KImg_Icon = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KImg_Icon);
                        var KCommon_ItemTips = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KCommon_ItemTips);
                        KImg_Icon.SetActive(false);
                        KAnimalRoot.SetActive(true);
                        KTagRoot.SetActive(true);
                        KTxtDse.GetRectTransform().SetHeight(388f);
                        KTxtEnemyName.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterAttr.name).current);
                        var str = UnicornUIHelper.GetRewardTextIconName("icon_diamond");
                        KTxtNew.GetTextMeshPro().SetTMPText(tblanguage.Get("text_new_2").current);
                        KTxtRewardNum.GetTextMeshPro().SetTMPText(str + monsterAttr.diamond.ToString());
                        KTxt1.GetTextMeshPro()
                            .SetTMPText(tblanguage.Get(tbpower.Get(monsterAttr.powerId).name).current);
                        KTxt2.GetTextMeshPro()
                            .SetTMPText(tblanguage.Get(tbracist.Get(monsterAttr.racistId).name).current);
                        //TODO:特性

                        KImg3.SetActive(false);
                        if (monsterAttr.featureId.Count > 0)
                        {
                            KImg3.SetActive(true);
                            var monsterFeature = tbmonster_feature.Get(monsterAttr.featureId[0]);


                            KTxt3.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterFeature.name).current);
                            var KTxt_Des = KCommon_ItemTips.GetFromReference(UICommon_ItemTips.KTxt_Des);

                            KTxt_Des.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterFeature.desc).current);
                            KCommon_ItemTips.SetActive(false);
                            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KImg3,
                                () => { KCommon_ItemTips.SetActive(!KCommon_ItemTips.GameObject.activeSelf); });
                        }

                        KTxtDse.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterAttr.desc).current);


                        KRewardRoot.SetActive(
                            ResourcesSingletonOld.Instance.resMonster.MonsterMap[bookId] == 2);
                        uiUnlock.GetButton().OnClick.Add(async () =>
                        {
                            Log.Debug($"uiUnlock");
                            uiUnlock.SetActive(false);
                            KMask.SetActive(true);
                            isUnLock = false;
                            if (KRewardRoot.GameObject.activeSelf)
                            {
                                var str = new StringValue();
                                str.Value = $"{curPosId + 1};{1};{bookId}";
                                NetWorkManager.Instance.SendMessage(CMDOld.RECEIVECOLLECTIONREWARD, str);
                                ResourcesSingletonOld.Instance.resMonster.MonsterMap[bookId] = 1;
                                var itemStr = $"{m_RedDotName}|Pos{curPosId}|{bookId}";
                                RedDotManager.Instance.SetRedPointCnt(itemStr, 0);
                                SortCollectionItems();
                                PlayAnim(monsterAttr.diamond).Forget();
                            }
                            else
                            {
                                KMask.SetActive(false);
                            }

                            //KMask.SetActive(false);
                            uiUnlock.Dispose();
                        });

                        var skeletonGraphic = KAnimalRoot.GetComponent<SkeletonGraphic>();

                        if (monsterAttr != null)
                        {
                            Log.Debug($"model spine {monsterAttr.model} {monsterAttr.spine}");

                            UnicornUIHelper.SetSpine(skeletonGraphic, monsterAttr.spine, monsterAttr.model);
                        }

                        // skeletonGraphic.skeletonDataAsset =
                        //     await ResourcesManager.LoadAssetAsync<SkeletonDataAsset>(
                        //         "spine_monster_common_Json_SkeletonData");


                        // foreach (var VARIABLE in skeletonGraphic.SkeletonData.Skins)
                        // {
                        //     Debug.LogError($"--{VARIABLE.Name}--");
                        // }

                        //skeletonGraphic.Initialize(true);
                        //TODO:读表换成spine
                        //Debug.LogError($"LogError {tbmonster_attr.Get(monster.monsterAttrId).spine}");
                        //skeletonGraphic.Skeleton.SetSkin(tbmonster_attr.Get(monster.monsterAttrId).spine);
                        //skeletonGraphic.Skeleton.SetSlotsToSetupPose(); // 重置插槽姿势

                        // var weaponStr = $"weapon/{tbmonster_weapon.Get(monster.monsterWeaponId).name}";
                        // skeletonGraphic.Skeleton.SetAttachment("weapon_solt", weaponStr);


                        uiUnlock.SetActive(true);

                        while (isUnLock)
                        {
                            int index = Random.Range(0, skeletonGraphic.SkeletonData.Animations.Items.Length);
                            string randomAnimationName =
                                skeletonGraphic.SkeletonData.Animations.Items[index].Name;
                            if (randomAnimationName.Contains("die"))
                            {
                                continue;
                            }

                            float duration =
                                skeletonGraphic.SkeletonData.FindAnimation(randomAnimationName).Duration;
                            skeletonGraphic.AnimationState.SetAnimation(0, randomAnimationName, false);

                            //var skeletonData = skeletonGraphic.skeletonDataAsset.GetSkeletonData(false);
                            // 获取动画信息
                            //var animation = skeletonData.FindAnimation(randomAnimationName);
                            //var time = skeletonGraphic.AnimationState.GetCurrent(0).AnimationTime;
                            //Log.Error($"{randomAnimationName} finished");
                            await UniTask.Delay((int)(duration * 1000));

                            //skeletonGraphic.AnimationState.SetAnimation(0, "Player_Stand", true);
                        }
                    });
                }

                itemList.Sort((a, b) =>
                {
                    var uia = a as UISubPanel_Collection_Item;
                    var uib = b as UISubPanel_Collection_Item;

                    bool compareNew = ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uia.monsterId) &&
                                      ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uib.monsterId);
                    if (compareNew)
                    {
                        if (ResourcesSingletonOld.Instance.resMonster.MonsterMap[uia.monsterId] == 2 &&
                            ResourcesSingletonOld.Instance.resMonster.MonsterMap[uib.monsterId] == 1)
                            return -1;
                        else if (ResourcesSingletonOld.Instance.resMonster.MonsterMap[uia.monsterId] == 1 &&
                                 ResourcesSingletonOld.Instance.resMonster.MonsterMap[uib.monsterId] == 2)
                            return 1;
                    }

                    if (ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uia.monsterId) &&
                        !ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uib.monsterId))
                        return -1;
                    else if (!ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uia.monsterId) &&
                             ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(uib.monsterId))
                        return 1;

                    return uia.monsterId.CompareTo(uib.monsterId);
                });
            }
            else if (curPosId == 1)
            {
                var weaponList = tbweapon.DataList.Where(a => a.type <= 3).GroupBy(a => a.type).Select(a => a.First())
                    .ToList();
                var weapon = weaponList[index];
                ObjectHelper.Awake(lableui, weapon.type);
                var verticalLayoutGroup = lableui.GetComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.padding.left = 0;
                var KText_Label = lableui.GetFromReference(UICommon_Label.KText_Label);
                var KText_LabelR = lableui.GetFromReference(UICommon_Label.KText_LabelR);
                var KContainer_Content = lableui.GetFromReference(UICommon_Label.KContainer_Content);

                var powerAllList = tbweapon.DataList.Where(a => a.type == weapon.type).ToList();
                var powerUnlockedList = ResourcesSingletonOld.Instance.resMonster.WeaponMap.Keys.ToList()
                    .Where(a => tbweapon.Get(a).type == weapon.type);
                KText_LabelR.SetActive(true);
                var titleStr =
                    $"{tblanguage.Get($"weapon_{weapon.type}_type").current}";

                titleStr = powerUnlockedList.Count() > 0 ? titleStr : unlockStr;

                KText_Label.GetTextMeshPro().SetTMPText(titleStr);
                KText_LabelR.GetTextMeshPro().SetTMPText($"{powerUnlockedList.Count()}/{powerAllList.Count}");

                var gridLayoutGroup = KContainer_Content.GetComponent<GridLayoutGroup>();
                gridLayoutGroup.cellSize = new Vector2(369f, 568f);
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = 3;
                var temp = gridLayoutGroup.spacing;
                temp.x = 14f;
                gridLayoutGroup.spacing = temp;
                var itemList = KContainer_Content.GetList();
                itemList.Clear();
                Log.Debug($"powerAllList{powerAllList.Count}", Color.green);
                foreach (var monsterAttr in powerAllList)
                {
                    int bookId = monsterAttr.id;
                    var ui = await itemList.CreateWithUITypeAsync(UIType.UISubPanel_Collection_Item, bookId, false);
                    var itemRec = ui.GetRectTransform();
                    itemRec.SetScale(Vector3.one);
                    var KTxtUnlock = ui.GetFromReference(UISubPanel_Collection_Item.KTxtUnlock);
                    var KTxtNew = ui.GetFromReference(UISubPanel_Collection_Item.KTxtNew);
                    var KTxtName = ui.GetFromReference(UISubPanel_Collection_Item.KTxtName);
                    var KTxtRewardNum = ui.GetFromReference(UISubPanel_Collection_Item.KTxtRewardNum);
                    var KReward = ui.GetFromReference(UISubPanel_Collection_Item.KReward);
                    var KImgBg = ui.GetFromReference(UISubPanel_Collection_Item.KImgBg);
                    var KImgBgQuality = ui.GetFromReference(UISubPanel_Collection_Item.KImgBgQuality);
                    var KUnLock = ui.GetFromReference(UISubPanel_Collection_Item.KUnLock);
                    var KDislayRoot = ui.GetFromReference(UISubPanel_Collection_Item.KDislayRoot);

                    var KImgBorder1 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBorder1);
                    var KImgBorder2 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBorder2);
                    var KImgBandage1 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBandage1);
                    var KImgBandage2 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBandage2);
                    var KImgUnlock = ui.GetFromReference(UISubPanel_Collection_Item.KImgUnlock);
                    var KSpineEnemy = ui.GetFromReference(UISubPanel_Collection_Item.KSpineEnemy);
                    var KImg_Icon = ui.GetFromReference(UISubPanel_Collection_Item.KImg_Icon);
                    var KImgMask = ui.GetFromReference(UISubPanel_Collection_Item.KImgMask);

                    KTxtUnlock.SetActive(false);
                    //KTxtUnlock.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_locking").current);
                    KTxtNew.GetTextMeshPro().SetTMPText(tblanguage.Get("text_new_2").current);
                    var str = UnicornUIHelper.GetRewardTextIconName("icon_diamond");
                    KTxtRewardNum.GetTextMeshPro().SetTMPText(monsterAttr.diamond.ToString());
                    UnicornUIHelper.ForceRefreshLayout(KReward);
                    KTxtName.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterAttr.name).current);

                    var bgStr = $"bg_weapon_pic_{monsterAttr.quality}";

                    KImgBg.GetImage().SetSpriteAsync(bgStr, false).Forget();
                    KImgBgQuality.GetImage().SetSpriteAsync("collection_bg_quality_" + monsterAttr.quality, false)
                        .Forget();
                    //var monsterAttr = tbmonster_attr.Get(monster.monsterAttrId);
                    KImgMask.GetImage().SetSpriteAsync("Img_Mask_Weapon", false).Forget();
                    var sg = KSpineEnemy.GetComponent<SkeletonGraphic>();
                    if (monsterAttr != null)
                    {
                        //Log.Debug($"model spine {monsterAttr.model} {monsterAttr.spine}");

                        //TODO:
                        //UnicornUIHelper.SetSpine(sg, monsterAttr.spine, monsterAttr.model);

                        // var picStr = $"{monsterAttr.spine}_pic";
                        // var xImage = KImgEnemy.GetXImage();
                        // xImage.SetSprite(picStr, false);
                        //
                        // var picRect = KImgEnemy.GetRectTransform();
                        //
                        // //xImage.Get(). activeSprite
                        // if (monster.picCutPara.Count > 0)
                        // {
                        //     var picPadding = new Vector4();
                        //     picPadding.x = 1 - (monster.picCutPara[0].y / 10000f);
                        //     picPadding.y = 1 - (monster.picCutPara[1].y / 10000f);
                        //     picPadding.z = 1 - (monster.picCutPara[2].y / 10000f);
                        //     picPadding.w = 1 - (monster.picCutPara[3].y / 10000f);
                        //     xImage.SetCutting(picPadding);
                        // }
                        //
                        // if (monster.picPosPara.Count > 0)
                        // {
                        //     var posX = picRect.Width() * (monster.picPosPara[0].y / 10000f);
                        //     var posY = picRect.Height() * (monster.picPosPara[0].z / 10000f);
                        //     var scale = monster.picPosPara[0].x / 10000f;
                        //     picRect.SetScale(new Vector3(scale, scale, scale));
                        //     picRect.SetAnchoredPosition(new Vector2(posX, posY));
                        // }

                        //xImage.SetNativeSize();
                    }

                    KReward.SetActive(true);
                    KImgBg.SetActive(true);
                    KTxtName.SetActive(true);
                    KUnLock.SetActive(false);
                    KSpineEnemy.SetActive(false);
                    KImg_Icon.SetActive(true);
                    if (ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(bookId))
                    {
                        if (ResourcesSingletonOld.Instance.resMonster.WeaponMap[bookId] == 1)
                        {
                            KReward.SetActive(false);
                        }


                        KImg_Icon.SetActive(true);
                        KImg_Icon.GetXImage().SetSpriteAsync(monsterAttr.pic, true);

                        //1：已领取 2：未领取

                        var itemStr = $"{m_RedDotName}|Pos{curPosId}|{bookId}";
                        KDislayRoot.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr) > 0);

                        KImgBorder1.GetComponent<XImage>().SetGrayed(false);
                        KImgBorder2.GetComponent<XImage>().SetGrayed(false);
                        KImgBandage1.GetComponent<XImage>().SetGrayed(false);
                        KImgBandage2.GetComponent<XImage>().SetGrayed(false);
                        KImgUnlock.GetComponent<XImage>().SetGrayed(false);
                        //KImgEnemy.GetComponent<XImage>().SetGrayed(false);

                        sg.material = ResourcesManager.LoadAsset<Material>("SpineNormal");
                        KImgMask.SetActive(false);
                        RedDotManager.Instance.AddListener(itemStr, (num) =>
                        {
                            //Log.Error($"RedDotManager {itemStr} {num}");
                            KDislayRoot?.SetActive(num > 0);
                        });
                    }
                    else
                    {
                        KImg_Icon.SetActive(true);
                        KImg_Icon.GetXImage().SetSpriteAsync("pic_monstercollection_locked", true);
                        KTxtName.GetTextMeshPro().SetTMPText(unlockStr);

                        KDislayRoot.SetActive(false);

                        KImgBorder1.GetComponent<XImage>().SetGrayed(true);
                        KImgBorder2.GetComponent<XImage>().SetGrayed(true);
                        KImgBandage1.GetComponent<XImage>().SetGrayed(true);
                        KImgBandage2.GetComponent<XImage>().SetGrayed(true);
                        KImgUnlock.GetComponent<XImage>().SetGrayed(true);
                        //KImgEnemy.GetComponent<XImage>().SetGrayed(true);
                        KImgMask.SetActive(true);
                    }

                    //
                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ui, async () =>
                    {
                        if (!ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(bookId))
                        {
                            UnicornUIHelper.ClearCommonResource();
                            UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("book_lock_text").current);
                            return;
                        }

                        bool isUnLock = true;
                        //var monsterConfig = tbmonster.Get(monster.id);
                        var uiUnlock =
                            await UIHelper.CreateAsync(UIType.UIPanel_Collection_Unlock, bookId) as
                                UIPanel_Collection_Unlock;
                        await UnicornUIHelper.InitBlur(uiUnlock);
                        uiUnlock.playAnim = true;
                        uiUnlock.PlayAnim();
                        var KAnimalRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KAnimalRoot);
                        var KTxtEnemyName = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtEnemyName);
                        var KTxtNew = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtNew);
                        var KRewardRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KRewardRoot);
                        var KImgReward = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KImgReward);
                        var KTxtRewardNum = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtRewardNum);
                        var KTxt1 = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxt1);
                        var KTxt2 = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxt2);
                        var KTxt3 = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxt3);
                        var KTxtDse = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtDse);
                        var KTagRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTagRoot);
                        var KImg_Icon = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KImg_Icon);
                        KImg_Icon.SetActive(true);
                        KAnimalRoot.SetActive(false);
                        KImg_Icon.GetXImage().SetSpriteAsync(monsterAttr.pic, true);


                        KTagRoot.SetActive(false);
                        KTxtDse.GetRectTransform().SetHeight(501f);
                        KTxtEnemyName.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterAttr.name).current);

                        KTxtNew.GetTextMeshPro().SetTMPText(tblanguage.Get("text_new_2").current);
                        var str = UnicornUIHelper.GetRewardTextIconName("icon_diamond");
                        KTxtRewardNum.GetTextMeshPro().SetTMPText(str + monsterAttr.diamond.ToString());
                        // KTxt1.GetTextMeshPro()
                        //     .SetTMPText(tblanguage.Get(tbpower.Get(monsterAttr.powerId).name).current);
                        // KTxt2.GetTextMeshPro()
                        //     .SetTMPText(tblanguage.Get(tbracist.Get(monsterAttr.racistId).name).current);
                        //TODO:特性
                        // KTxt3.GetTextMeshPro()
                        //     .SetTMPText(tblanguage.Get(tbweapon.Get(monster.monsterWeaponId).name).current);

                        KTxtDse.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterAttr.desc).current);


                        KRewardRoot.SetActive(
                            ResourcesSingletonOld.Instance.resMonster.WeaponMap[bookId] == 2);
                        uiUnlock.GetButton().OnClick.Add(async () =>
                        {
                            Log.Debug($"uiUnlock");
                            uiUnlock.SetActive(false);
                            KMask.SetActive(true);
                            isUnLock = false;
                            if (KRewardRoot.GameObject.activeSelf)
                            {
                                var str = new StringValue();
                                str.Value = $"{curPosId + 1};{1};{bookId}";
                                NetWorkManager.Instance.SendMessage(CMDOld.RECEIVECOLLECTIONREWARD, str);
                                ResourcesSingletonOld.Instance.resMonster.WeaponMap[bookId] = 1;
                                var itemStr = $"{m_RedDotName}|Pos{curPosId}|{bookId}";
                                RedDotManager.Instance.SetRedPointCnt(itemStr, 0);
                                SortCollectionItems();

                                PlayAnim(monsterAttr.diamond).Forget();
                            }
                            else
                            {
                                KMask.SetActive(false);
                            }

                            //KMask.SetActive(false);
                            uiUnlock.Dispose();
                        });

                        //var skeletonGraphic = KAnimalRoot.GetComponent<SkeletonGraphic>();

                        // if (monsterAttr != null)
                        // {
                        //     //Log.Debug($"model spine {monsterAttr.model} {monsterAttr.spine}");
                        //     //TODO
                        //     //UnicornUIHelper.SetSpine(skeletonGraphic, monsterAttr.spine, monsterAttr.model);
                        // }

                        // skeletonGraphic.skeletonDataAsset =
                        //     await ResourcesManager.LoadAssetAsync<SkeletonDataAsset>(
                        //         "spine_monster_common_Json_SkeletonData");


                        // foreach (var VARIABLE in skeletonGraphic.SkeletonData.Skins)
                        // {
                        //     Debug.LogError($"--{VARIABLE.Name}--");
                        // }

                        //skeletonGraphic.Initialize(true);
                        //TODO:读表换成spine
                        //Debug.LogError($"LogError {tbmonster_attr.Get(monster.monsterAttrId).spine}");
                        //skeletonGraphic.Skeleton.SetSkin(tbmonster_attr.Get(monster.monsterAttrId).spine);
                        //skeletonGraphic.Skeleton.SetSlotsToSetupPose(); // 重置插槽姿势

                        // var weaponStr = $"weapon/{tbmonster_weapon.Get(monster.monsterWeaponId).name}";
                        // skeletonGraphic.Skeleton.SetAttachment("weapon_solt", weaponStr);


                        uiUnlock.SetActive(true);

                        // while (isUnLock)
                        // {
                        //     int index = Random.Range(0, skeletonGraphic.SkeletonData.Animations.Items.Length);
                        //     string randomAnimationName =
                        //         skeletonGraphic.SkeletonData.Animations.Items[index].Name;
                        //     if (randomAnimationName.Contains("die"))
                        //     {
                        //         continue;
                        //     }
                        //
                        //     float duration =
                        //         skeletonGraphic.SkeletonData.FindAnimation(randomAnimationName).Duration;
                        //     skeletonGraphic.AnimationState.SetAnimation(0, randomAnimationName, false);
                        //
                        //     //var skeletonData = skeletonGraphic.skeletonDataAsset.GetSkeletonData(false);
                        //     // 获取动画信息
                        //     //var animation = skeletonData.FindAnimation(randomAnimationName);
                        //     //var time = skeletonGraphic.AnimationState.GetCurrent(0).AnimationTime;
                        //     //Log.Error($"{randomAnimationName} finished");
                        //     await UniTask.Delay((int)(duration * 1000));
                        //
                        //     //skeletonGraphic.AnimationState.SetAnimation(0, "Player_Stand", true);
                        // }
                    });
                }

                itemList.Sort((a, b) =>
                {
                    var uia = a as UISubPanel_Collection_Item;
                    var uib = b as UISubPanel_Collection_Item;

                    bool compareNew = ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uia.monsterId) &&
                                      ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uib.monsterId);
                    if (compareNew)
                    {
                        if (ResourcesSingletonOld.Instance.resMonster.WeaponMap[uia.monsterId] == 2 &&
                            ResourcesSingletonOld.Instance.resMonster.WeaponMap[uib.monsterId] == 1)
                            return -1;
                        else if (ResourcesSingletonOld.Instance.resMonster.WeaponMap[uia.monsterId] == 1 &&
                                 ResourcesSingletonOld.Instance.resMonster.WeaponMap[uib.monsterId] == 2)
                            return 1;
                    }

                    if (ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uia.monsterId) &&
                        !ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uib.monsterId))
                        return -1;
                    else if (!ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uia.monsterId) &&
                             ResourcesSingletonOld.Instance.resMonster.WeaponMap.ContainsKey(uib.monsterId))
                        return 1;

                    return uia.monsterId.CompareTo(uib.monsterId);
                });
            }

            // var monster = powerMonList[index];
            // ObjectHelper.Awake(ui, monster.id);
            // var monsterAttr = tbmonster_attr.Get(monster.monsterAttrId);
            // Log.Debug("dfadfsdfdfdfd");
            // var KText_TopTitle = GetFromReference(UIPanel_MonsterCollection.KText_TopTitle);
            // var KScrollView_Main = GetFromReference(UIPanel_MonsterCollection.KScrollView_Main);
            // var KBtn_ReceiveAll = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll);
            // var KBtn_ReceiveAll2 = GetFromReference(UIPanel_MonsterCollection.KBtn_ReceiveAll2);
            // var KPopUp = GetFromReference(UIPanel_MonsterCollection.KPopUp);
            // var KText_Diamond = GetFromReference(UIPanel_MonsterCollection.KText_Diamond);
            // var KImg_DiamondIcon = GetFromReference(UIPanel_MonsterCollection.KImg_DiamondIcon);
            // var KMask = GetFromReference(UIPanel_MonsterCollection.KMask);
            // var MyKRewardRoot = GetFromReference(UIPanel_MonsterCollection.KRewardRoot);
            // var MyKTxtRewardNum = GetFromReference(UIPanel_MonsterCollection.KTxtRewardNum);
            //
            // var KTxtUnlock = ui.GetFromReference(UISubPanel_Collection_Item.KTxtUnlock);
            // var KTxtNew = ui.GetFromReference(UISubPanel_Collection_Item.KTxtNew);
            // var KTxtName = ui.GetFromReference(UISubPanel_Collection_Item.KTxtName);
            // var KTxtRewardNum = ui.GetFromReference(UISubPanel_Collection_Item.KTxtRewardNum);
            // //var KImgEnemy = ui.GetFromReference(UISubPanel_Collection_Item.KImgEnemy);
            // var KImgBg = ui.GetFromReference(UISubPanel_Collection_Item.KImgBg);
            // var KUnLock = ui.GetFromReference(UISubPanel_Collection_Item.KUnLock);
            // var KDislayRoot = ui.GetFromReference(UISubPanel_Collection_Item.KDislayRoot);
            //
            // var KImgBorder1 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBorder1);
            // var KImgBorder2 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBorder2);
            // var KImgBandage1 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBandage1);
            // var KImgBandage2 = ui.GetFromReference(UISubPanel_Collection_Item.KImgBandage2);
            // var KImgUnlock = ui.GetFromReference(UISubPanel_Collection_Item.KImgUnlock);
            // var KSpineEnemy = ui.GetFromReference(UISubPanel_Collection_Item.KSpineEnemy);
            // KTxtUnlock.SetActive(false);
            // //KTxtUnlock.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_locking").current);
            // KTxtNew.GetTextMeshPro().SetTMPText(tblanguage.Get("text_new_2").current);
            //
            // KTxtRewardNum.GetTextMeshPro().SetTMPText(monsterAttr.diamond.ToString());
            // KTxtName.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterAttr.name).current);
            //
            // //bg_monster_icons_1
            // var bgStr = monster.type == 32 ? "bg_monster_icons_2" : "bg_monster_icons_1";
            // KImgBg.GetImage().SetSpriteAsync(bgStr, false).Forget();
            // //var monsterAttr = tbmonster_attr.Get(monster.monsterAttrId);
            //
            // var sg = KSpineEnemy.GetComponent<SkeletonGraphic>();
            // if (monsterAttr != null)
            // {
            //     //Log.Debug($"model spine {monsterAttr.model} {monsterAttr.spine}");
            //
            //     UnicornUIHelper.SetSpine(sg, monsterAttr.spine, monsterAttr.model);
            //
            //     // var picStr = $"{monsterAttr.spine}_pic";
            //     // var xImage = KImgEnemy.GetXImage();
            //     // xImage.SetSprite(picStr, false);
            //     //
            //     // var picRect = KImgEnemy.GetRectTransform();
            //     //
            //     // //xImage.Get(). activeSprite
            //     // if (monster.picCutPara.Count > 0)
            //     // {
            //     //     var picPadding = new Vector4();
            //     //     picPadding.x = 1 - (monster.picCutPara[0].y / 10000f);
            //     //     picPadding.y = 1 - (monster.picCutPara[1].y / 10000f);
            //     //     picPadding.z = 1 - (monster.picCutPara[2].y / 10000f);
            //     //     picPadding.w = 1 - (monster.picCutPara[3].y / 10000f);
            //     //     xImage.SetCutting(picPadding);
            //     // }
            //     //
            //     // if (monster.picPosPara.Count > 0)
            //     // {
            //     //     var posX = picRect.Width() * (monster.picPosPara[0].y / 10000f);
            //     //     var posY = picRect.Height() * (monster.picPosPara[0].z / 10000f);
            //     //     var scale = monster.picPosPara[0].x / 10000f;
            //     //     picRect.SetScale(new Vector3(scale, scale, scale));
            //     //     picRect.SetAnchoredPosition(new Vector2(posX, posY));
            //     // }
            //
            //     //xImage.SetNativeSize();
            // }
            //
            // KImgBg.SetActive(true);
            // KTxtName.SetActive(true);
            // KUnLock.SetActive(false);
            // if (ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(monster.id))
            // {
            //     //1：已领取 2：未领取
            //
            //     var itemStr = $"{m_RedDotName}|Power{monsterAttr.powerId}|{monster.id}";
            //     KDislayRoot.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr) > 0);
            //
            //     KImgBorder1.GetComponent<XImage>().SetGrayed(false);
            //     KImgBorder2.GetComponent<XImage>().SetGrayed(false);
            //     KImgBandage1.GetComponent<XImage>().SetGrayed(false);
            //     KImgBandage2.GetComponent<XImage>().SetGrayed(false);
            //     KImgUnlock.GetComponent<XImage>().SetGrayed(false);
            //     //KImgEnemy.GetComponent<XImage>().SetGrayed(false);
            //
            //     sg.material = ResourcesManager.LoadAsset<Material>("SpineNormal");
            //     KImgBg.GetComponent<XImage>().SetGrayed(false);
            //
            //     RedDotManager.Instance.AddListener(itemStr, (num) =>
            //     {
            //         //Log.Error($"RedDotManager {itemStr} {num}");
            //         KDislayRoot?.SetActive(num > 0);
            //     });
            // }
            // else
            // {
            //     //KTxtName.SetActive(false);
            //
            //     //KUnLock.SetActive(true);
            //     KDislayRoot.SetActive(false);
            //
            //     KImgBorder1.GetComponent<XImage>().SetGrayed(true);
            //     KImgBorder2.GetComponent<XImage>().SetGrayed(true);
            //     KImgBandage1.GetComponent<XImage>().SetGrayed(true);
            //     KImgBandage2.GetComponent<XImage>().SetGrayed(true);
            //     KImgUnlock.GetComponent<XImage>().SetGrayed(true);
            //     //KImgEnemy.GetComponent<XImage>().SetGrayed(true);
            //     sg.material = ResourcesManager.LoadAsset<Material>("SpineGray");
            //     KImgBg.GetComponent<XImage>().SetGrayed(true);
            // }
            //
            // //
            // UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ui, async () =>
            // {
            //     if (!ResourcesSingletonOld.Instance.resMonster.MonsterMap.ContainsKey(monster.id))
            //     {
            //         UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get("book_lock_text").current);
            //         return;
            //     }
            //
            //     bool isUnLock = true;
            //     //var monsterConfig = tbmonster.Get(monster.id);
            //     var uiUnlock = await UIHelper.CreateAsync(UIType.UIPanel_Collection_Unlock, monster.id);
            //
            //
            //     var KAnimalRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KAnimalRoot);
            //     var KTxtEnemyName = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtEnemyName);
            //     var KTxtNew = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtNew);
            //     var KRewardRoot = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KRewardRoot);
            //     var KImgReward = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KImgReward);
            //     var KTxtRewardNum = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtRewardNum);
            //     var KTxtPower = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtPower);
            //     var KTxtWeapon = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtWeapon);
            //     var KTxtRacist = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtRacist);
            //     var KTxtDse = uiUnlock.GetFromReference(UIPanel_Collection_Unlock.KTxtDse);
            //
            //     KTxtEnemyName.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterAttr.name).current);
            //
            //     KTxtNew.GetTextMeshPro().SetTMPText(tblanguage.Get("text_new_2").current);
            //     KTxtRewardNum.GetTextMeshPro().SetTMPText(monsterAttr.diamond.ToString());
            //     KTxtPower.GetTextMeshPro()
            //         .SetTMPText(tblanguage.Get(tbpower.Get(monsterAttr.powerId).name).current);
            //     KTxtWeapon.GetTextMeshPro()
            //         .SetTMPText(tblanguage.Get(tbweapon.Get(monster.monsterWeaponId).name).current);
            //     KTxtRacist.GetTextMeshPro()
            //         .SetTMPText(tblanguage.Get(tbracist.Get(monsterAttr.racistId).name).current);
            //     KTxtDse.GetTextMeshPro().SetTMPText(tblanguage.Get(monsterAttr.desc).current);
            //
            //
            //     KRewardRoot.SetActive(ResourcesSingletonOld.Instance.resMonster.MonsterMap[monster.id] == 2);
            //     uiUnlock.GetButton().OnClick.Add(async () =>
            //     {
            //         Log.Debug($"uiUnlock");
            //         uiUnlock.SetActive(false);
            //         KMask.SetActive(true);
            //         isUnLock = false;
            //         if (KRewardRoot.GameObject.activeSelf)
            //         {
            //             var str = new StringValue();
            //             str.Value = $"1;{monster.id}";
            //             NetWorkManager.Instance.SendMessage(CMDOld.RECEIVECOLLECTIONREWARD, str);
            //             ResourcesSingletonOld.Instance.resMonster.MonsterMap[monster.id] = 1;
            //             var itemStr = $"{m_RedDotName}|Power{monsterAttr.powerId}|{monster.id}";
            //             RedDotManager.Instance.SetRedPointCnt(itemStr, 0);
            //             SortCollectionItems();
            //
            //             //TODO:播放动画 
            //             KPopUp.SetActive(true);
            //             var popRec = KPopUp.GetRectTransform();
            //             var iconRec = KImg_DiamondIcon.GetRectTransform();
            //             var textRec = KText_Diamond.GetRectTransform();
            //             var textCom = KText_Diamond.GetTextMeshPro();
            //             iconRec.SetScale(Vector2.one);
            //             textRec.SetScale(Vector2.one);
            //             popRec.DoAnchoredPosition(new Vector2(popRec.Width(), popRec.AnchoredPosition().y),
            //                 new Vector2(0, popRec.AnchoredPosition().y), 0.5f);
            //
            //             MyKTxtRewardNum.GetTextMeshPro().SetTMPText(monsterAttr.diamond.ToString());
            //             var MyKRewardRootRec = MyKRewardRoot.GetRectTransform();
            //             MyKRewardRootRec.SetAnchoredPosition(Vector2.zero);
            //             MyKRewardRoot.SetActive(true);
            //
            //
            //             await UniTask.Delay(500);
            //             var finallyPos = UnicornUIHelper.GetUIPos(KPopUp);
            //             finallyPos.x -= popRec.Width() / 2f;
            //             finallyPos.y -= popRec.Height() / 2f;
            //             MyKRewardRootRec.DoAnchoredPosition(Vector2.zero, finallyPos, 1f).AddOnCompleted(() =>
            //             {
            //                 MyKRewardRoot.SetActive(false);
            //             });
            //             iconRec.DoScale2(new Vector2(1.2f, 1.2f), 1f);
            //             textRec.DoScale2(new Vector2(1.2f, 1.2f), 1f);
            //
            //             var everyTime = 1000 / monsterAttr.diamond;
            //
            //             for (int j = 0; j < monsterAttr.diamond; j++)
            //             {
            //                 var index = j + 1;
            //                 textCom.SetTMPText($"{ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin + index}");
            //                 await UniTask.Delay(everyTime);
            //             }
            //
            //             ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin += monsterAttr.diamond;
            //
            //             // m_Tweener = m_NumText.transform.DOScale(3f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
            //             // {
            //             //     m_NumText.text = (++m_Num).ToString();
            //             //     m_NumText.transform.DOScale(1f, 0.25f).OnComplete(() =>
            //             //     {
            //             //         m_tweenNum--;
            //             //         DoTween();
            //             //     });
            //             // });
            //
            //             await UniTask.Delay(500);
            //
            //             popRec.DoAnchoredPosition(
            //                 new Vector2(popRec.AnchoredPosition().x, popRec.AnchoredPosition().y),
            //                 new Vector2(popRec.Width(), popRec.AnchoredPosition().y), 0.5f);
            //             await UniTask.Delay(500);
            //             KPopUp.SetActive(false);
            //
            //             //Log.Error($"SetRedPointCnt {itemStr}");
            //             //CreateMainItem(curPowerId).Forget();
            //         }
            //
            //         KMask.SetActive(false);
            //         uiUnlock.Dispose();
            //     });
            //
            //     var skeletonGraphic = KAnimalRoot.GetComponent<SkeletonGraphic>();
            //
            //     if (monsterAttr != null)
            //     {
            //         Log.Debug($"model spine {monsterAttr.model} {monsterAttr.spine}");
            //
            //         UnicornUIHelper.SetSpine(skeletonGraphic, monsterAttr.spine, monsterAttr.model);
            //     }
            //
            //     // skeletonGraphic.skeletonDataAsset =
            //     //     await ResourcesManager.LoadAssetAsync<SkeletonDataAsset>(
            //     //         "spine_monster_common_Json_SkeletonData");
            //
            //
            //     // foreach (var VARIABLE in skeletonGraphic.SkeletonData.Skins)
            //     // {
            //     //     Debug.LogError($"--{VARIABLE.Name}--");
            //     // }
            //
            //     //skeletonGraphic.Initialize(true);
            //     //TODO:读表换成spine
            //     //Debug.LogError($"LogError {tbmonster_attr.Get(monster.monsterAttrId).spine}");
            //     //skeletonGraphic.Skeleton.SetSkin(tbmonster_attr.Get(monster.monsterAttrId).spine);
            //     //skeletonGraphic.Skeleton.SetSlotsToSetupPose(); // 重置插槽姿势
            //
            //     // var weaponStr = $"weapon/{tbmonster_weapon.Get(monster.monsterWeaponId).name}";
            //     // skeletonGraphic.Skeleton.SetAttachment("weapon_solt", weaponStr);
            //
            //
            //     uiUnlock.SetActive(true);
            //
            //     while (isUnLock)
            //     {
            //         int index = Random.Range(0, skeletonGraphic.SkeletonData.Animations.Items.Length);
            //         string randomAnimationName =
            //             skeletonGraphic.SkeletonData.Animations.Items[index].Name;
            //         if (randomAnimationName.Contains("die"))
            //         {
            //             continue;
            //         }
            //
            //         float duration =
            //             skeletonGraphic.SkeletonData.FindAnimation(randomAnimationName).Duration;
            //         skeletonGraphic.AnimationState.SetAnimation(0, randomAnimationName, false);
            //
            //         //var skeletonData = skeletonGraphic.skeletonDataAsset.GetSkeletonData(false);
            //         // 获取动画信息
            //         //var animation = skeletonData.FindAnimation(randomAnimationName);
            //         //var time = skeletonGraphic.AnimationState.GetCurrent(0).AnimationTime;
            //         //Log.Error($"{randomAnimationName} finished");
            //         await UniTask.Delay((int)(duration * 1000));
            //
            //         //skeletonGraphic.AnimationState.SetAnimation(0, "Player_Stand", true);
            //     }
            // });
        }

        protected override void OnClose()
        {
            RedDotManager.Instance.ClearChildrenListeners(m_RedDotName);
            UIHelper.Remove(UIType.UIPanel_Collection_Unlock);
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}