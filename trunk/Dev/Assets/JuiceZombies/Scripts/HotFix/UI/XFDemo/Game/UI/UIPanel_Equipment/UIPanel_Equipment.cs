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
using HotFix_UI;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Equipment)]
    internal sealed class UIPanel_EquipmentEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPanel_Equipment;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Equipment>();
        }
    }


    public partial class UIPanel_Equipment : UI, IAwake<List<ModuleInfoStruct>>
    {
        private float InitfontSize = 45;
        private float ChangefontSize = 50;
        public List<ModuleInfoStruct> tempStructList = new List<ModuleInfoStruct>();

        private Tblanguage tblanguage;

        //上一次点击的按钮索引
        public int lastModuleId;

        private List<UICommon_Label> UICommon_Labels = new List<UICommon_Label>();

        private Dictionary<int, UICommon_EquipItem> wearEquipsDic = new Dictionary<int, UICommon_EquipItem>();
        //装备和通用合成材料的数据 装备key为1-6,通合为100-600;
        //private Dictionary<int, List<GameEquip>> tempDics = new Dictionary<int, List<GameEquip>>();


        private bool isAllEquip = true;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private long timerId;
        private Vector2 lastScrollValue;

        public bool isDone;

        //public bool isCompuseDone = false;
        //public UICommon_EquipItem lastClickTipItem = null;
        public XScrollRectComponent scrollRect;

        private float curWidth;
        private float defaultWidth = -Screen.width * 1f;
        private int tagId = 2;

        public void Initialize(List<ModuleInfoStruct> args)
        {
            Log.Debug($"UIPanel_Equipment Initialize");
            JiYuUIHelper.SortEquipments();

            InitPanel().Forget();
            tempStructList.AddRange(args);

            //tempStructList = args;

            //Log.Debug(tempStructList.ToString(), Color.red);
            //lastButtonIndex = tempStructList[0].sortValues;

            // if (!UIPanel_JiyuGame.equipIsDone)
            // {
            InitTab2WidegetInfo(tempStructList).Forget();
            //UIPanel_JiyuGame.equipIsDone = true;
            //}


            //按品质或者部位排序,点击合成的按钮
            //RegisterWidget();

            OnClickTips();
            StartTimer();
            Anim().Forget();
        }

        async UniTaskVoid Anim()
        {
            //Log.Error($"{ModuleS.Count}");
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(
                this.GetFromReference(UIPanel_Equipment.KTop_AnimationPos), 0, 250f, cts.Token, 0.4f,
                false, false);
            var bottom = this.GetFromReference(UIPanel_Equipment.KBottom);
            var childs = bottom.GetList().Children;
            for (int i = 0; i < childs.Count; i++)
            {
                var child = childs[i];
                var itemRoot = child.GetFromReference(UICommon_Label.KContainer_Content);
                var items = itemRoot.GetList().Children;
                for (int j = 0; j < items.Count; j++)
                {
                    var item = items[j];
                    if (j < 40)
                    {
                        await JiYuTweenHelper.SetEaseAlphaAndPosB2U(
                            item.GetFromReference(UICommon_RewardItem.KBtn_Item), 0, 22, cts.Token, 0.2f, true,
                            false);
                        await UniTask.Delay(10);
                    }
                }
            }
        }

        void OnScrollRectDragging(Vector2 delta)
        {
            bool preferHorizontal = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y); // 判断水平滑动
            bool preferVertical = Mathf.Abs(delta.y) >= Mathf.Abs(delta.x); // 判断水平滑动
            if (preferHorizontal)
            {
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
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
                    //OnDestoryAllTips();
                    //右+ 左-
                }


                //Log.Debug($"delta:{delta}");
                // 处理水平滑动
                //scrollRect.SetEnabled(false);
            }

            if (preferVertical)
            {
                OnDestoryAllTips();
            }
        }

        public enum EquipPanelType
        {
            //0:装备界面 1:合成界面 2:合成选择之后界面 3:装备界面武器
            Main,
            Compose,
            ComposeSelected,
            MainWeapon,
        }

        public enum WearEquipType
        {
            None,
            Wear,
            UnWear,
            Change
        }

        public void RefreshAllWearEquip()
        {
            for (int i = 1; i < 7; i++)
            {
                RefreshWearEquip(i);
            }
        }

        private void RefreshWearEquip(int equipPosId)
        {
            if (ResourcesSingleton.Instance.equipmentData.isWearingEquipments.ContainsKey(equipPosId))
            {
                // Log.Error($"equipPosId{equipPosId}");
                // foreach (var kv in  ResourcesSingleton.Instance.equipmentData.isWearingEquipments)
                // {
                //     Log.Error($"isWearingEquipments{kv.Key} {kv.Value.equip.PartId} {kv.Value.equip.EquipId} {kv.Value.equip.Quality}");
                // }
                // foreach (var kv in  ResourcesSingleton.Instance.equipmentData.equipments)
                // {
                //     Log.Error($"equipments{kv.Key} ");
                // }
                //
                var partId = ResourcesSingleton.Instance.equipmentData.isWearingEquipments[equipPosId].equip.PartId;
                if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(partId, out var equip))
                {
                    Log.Error($"当前装备在总装备列表里找不到,数据有误! partId:{partId}");
                    return;
                }

                // var equip = ResourcesSingleton.Instance.equipmentData.equipments[
                //     ResourcesSingleton.Instance.equipmentData.isWearingEquipments[equipPosId].equip.PartId];
                //TODO:更换有问题
                wearEquipsDic[equipPosId].SetActive(true);
                if (equipPosId == 1)
                {
                    wearEquipsDic[equipPosId]
                        .Initialize(equip, EquipPanelType.MainWeapon);
                }
                else
                {
                    wearEquipsDic[equipPosId]
                        .Initialize(equip, EquipPanelType.Main);
                }
            }
            else
            {
                wearEquipsDic[equipPosId].SetActive(false);
            }
        }

        public async UniTaskVoid SortContentEquip()
        {
            await UniTask.Yield();
            var KContent_Equip = GetFromReference(UIPanel_Equipment.KContent_Equip);
            JiYuUIHelper.ForceRefreshLayout(KContent_Equip);
        }

        public void RefreshMainRedDot(int equipPosId, WearEquipType type, long uid, long lastUid)
        {
            if (!HasCommonLabel(CommonLabelType.AllEquip))
            {
                return;
            }

            RefreshWearEquip(equipPosId);
            var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
            var list = KBottom.GetList();
            foreach (var ui in list.Children)
            {
                var item = ui as UICommon_Label;
                item.RefreshRewardItems();

                if ((int)item.type == equipPosId || isAllEquip)
                {
                    switch (type)
                    {
                        case WearEquipType.None:

                            break;
                        case WearEquipType.Wear:
                            item.WearEquip(uid);
                            break;
                        case WearEquipType.UnWear:
                            item.UnWearEquip(uid);
                            break;
                        case WearEquipType.Change:
                            item.ChangeEquip(lastUid, uid);
                            break;
                    }

                    item.RefreshEquipItems();
                    //return;
                }
            }

            SortContentEquip();
            //InitBottom();

            // foreach (var item in UICommon_Labels)
            // {
            //     item.RefreshRewardItems();
            //     if ((int)item.type == equipPosId || isAllEquip)
            //     {
            //         switch (type)
            //         {
            //             case WearEquipType.None:
            //
            //                 break;
            //             case WearEquipType.Wear:
            //                 item.WearEquip(uid);
            //                 break;
            //             case WearEquipType.UnWear:
            //                 item.UnWearEquip(uid);
            //                 break;
            //             case WearEquipType.Change:
            //                 item.ChangeEquip(lastUid, uid);
            //                 break;
            //         }
            //
            //         item.RefreshEquipItems();
            //         //return;
            //     }
            // }
        }

        public void SortItems(int equipPosId, bool isCompoundSort = false)
        {
            if (!HasCommonLabel(CommonLabelType.AllEquip))
            {
                return;
            }

            var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
            var list = KBottom.GetList();
            foreach (var ui in list.Children)
            {
                var item = ui as UICommon_Label;
                if ((int)item.type == equipPosId || item.type == CommonLabelType.AllEquip)
                {
                    item.SortItems(isCompoundSort);
                    return;
                }
            }

            // foreach (var item in UICommon_Labels)
            // {
            //     if ((int)item.type == equipPosId || item.type == CommonLabelType.AllEquip)
            //     {
            //         item.SortItems(isCompoundSort);
            //         return;
            //     }
            // }
        }

        // private void OnAnimationComplete(TrackEntry trackEntry)
        // {
        //     var KPlayerAnim = GetFromReference(UIPanel_Equipment.KPlayerAnim);
        //
        //     var palyerAnimaition = KPlayerAnim.GetComponent<SkeletonGraphic>();
        //     palyerAnimaition.AnimationState.SetAnimation(0, "Player_Stand", true);
        // }

        public async UniTaskVoid PlayEquipAnimation(int equipid)
        {
            return;
            var KPlayerAnim = GetFromReference(UIPanel_Equipment.KPlayerAnim);

            //TODO:根据武器不同 切换
            var palyerAnimaition = KPlayerAnim.GetComponent<SkeletonGraphic>();
            //palyerAnimaition.Skeleton.SetAttachment("Weapon", "101");
            //palyerAnimaition.AnimationState.SetAnimation(0, "equip_101_spine", false);
            palyerAnimaition.AnimationState.ClearTracks();
            palyerAnimaition.AnimationState.SetAnimation(0, "idle", false);
            var duration = JiYuUIHelper.GetAnimaionDuration(palyerAnimaition, "idle");
            await UniTask.Delay(duration, cancellationToken: cts.Token);

            palyerAnimaition.AnimationState.SetAnimation(0, "idle", true);
        }

        public void DestorySelected()
        {
            var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
            var list = KBottom.GetList();
            foreach (var uiList in list.Children)
            {
                var uilable = uiList as UICommon_Label;
                uilable.DestoryAllSelected();
            }

            foreach (var kv in wearEquipsDic)
            {
                var selected = kv.Value.GetFromReference(UICommon_EquipItem.KImg_Selected);
                selected.SetActive(false);
            }
        }

        public void OnDestoryAllTips()
        {
            JiYuUIHelper.DestoryAllTips();
        }

        public void SetWearedEquipOnClick(Dictionary<int, UICommon_EquipItem> wearEquipsDic, int posId,
            UICommon_EquipItem ui0)
        {
            var btn = ui0.GetFromReference(UICommon_EquipItem.KBtn_Item);
            //var KMidBottom = GetFromReference(UIPanel_Equipment.KMidBottom);
            if (!wearEquipsDic.TryGetValue(posId, out var ui))
            {
                return;
            }

            // var scrollRect = this.GetScrollRect();
            // scrollRect.SetVerticalNormalizedPosition(1);
            //
            // await UniTask.Yield();
            //OnDestoryAllTips();

            JiYuUIHelper.SetEquipOnClick(ui0);

            // JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
            // {
            //     if (!wearEquipsDic.TryGetValue(posId, out var ui))
            //     {
            //         return;
            //     }
            //
            //     // var scrollRect = this.GetScrollRect();
            //     // scrollRect.SetVerticalNormalizedPosition(1);
            //     //
            //     // await UniTask.Yield();
            //     OnDestoryAllTips();
            //
            //     if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(ui.uid,
            //             out var equip))
            //     {
            //         return;
            //     }
            //
            //     var isSelected = ui.GetFromReference(UICommon_EquipItem.KImg_Selected);
            //     isSelected.SetActive(true);
            //     //lastClickTipItem = ui;
            //     //lastClickTipItem?.GetFromReference()
            //     // JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var uijiyu);
            //     // var bottom = uijiyu.GetFromReference(UIPanel_JiyuGame.KButtomPos);
            //
            //
            //     var tipUI = await UIHelper.CreateAsyncNew(this, UIType.UIPanel_EquipTips, equip,
            //             this.GameObject.transform) as
            //         UICommon_EquipTips;
            //     //TODO：穿戴中装备tips 位置固定
            //     //JiYuTweenHelper.SetEaseAlphaAndPosB2U(tipUI, -690f);
            //
            //
            //     // var btn = tipUI.GetFromReference(UICommon_EquipTips.is);
            //     // tipUI.get
            // }));
        }

        public void RefreshPlayerProperty()
        {
            var KText_RightAtkNum = GetFromReference(UIPanel_Equipment.KText_RightAtkNum);
            var KText_RightHpNum = GetFromReference(UIPanel_Equipment.KText_RightHpNum);

            // Log.Error($"atk{ResourcesSingleton.Instance.playerProperty.playerData.defaultProperty.atk}");
            // Log.Error($"maxHp{ResourcesSingleton.Instance.playerProperty.playerData.defaultProperty.maxHp}");
            // KText_RightAtkNum.GetTextMeshPro()
            //     .SetTMPText(ResourcesSingleton.Instance.playerProperty.chaProperty.atk.ToString());
            // KText_RightHpNum.GetTextMeshPro()
            //     .SetTMPText(ResourcesSingleton.Instance.playerProperty.chaProperty.maxHp.ToString());
            ResourcesSingleton.Instance.mainProperty.TryGetValue(102010, out int defaultHp);
            ResourcesSingleton.Instance.mainProperty.TryGetValue(102020, out int hpRatios);
            ResourcesSingleton.Instance.mainProperty.TryGetValue(102030, out int hpAdd);
            ResourcesSingleton.Instance.mainProperty.TryGetValue(103010, out int defaultAtk);
            ResourcesSingleton.Instance.mainProperty.TryGetValue(103020, out int atkRatios);
            ResourcesSingleton.Instance.mainProperty.TryGetValue(103030, out int atkAdd);

            int hp = (int)(defaultHp * (1 + hpRatios / 10000f) + hpAdd);
            int atk = (int)(defaultAtk * (1 + atkRatios / 10000f) + atkAdd);
            KText_RightAtkNum.GetTextMeshPro()
                .SetTMPText(atk.ToString());
            KText_RightHpNum.GetTextMeshPro()
                .SetTMPText(hp.ToString());

            foreach (var kv in ResourcesSingleton.Instance.mainProperty)
            {
                Log.Debug($"mainProperty:{kv.Key} : {kv.Value}");
            }
        }


        async UniTaskVoid InitPanel()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            var KContent_Equip = GetFromReference(UIPanel_Equipment.KContent_Equip);
            var KTop = GetFromReference(UIPanel_Equipment.KTop);
            var KMid = GetFromReference(UIPanel_Equipment.KMid);
            var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
            var KImg_Strip = GetFromReference(UIPanel_Equipment.KImg_Strip);
            var KBtn_Mask = GetFromReference(UIPanel_Equipment.KBtn_Mask);
            var KTop_EquipPos = GetFromReference(UIPanel_Equipment.KTop_EquipPos);
            var KImg_Atk = GetFromReference(UIPanel_Equipment.KImg_Atk);
            var KBtn_SortBy = GetFromReference(UIPanel_Equipment.KBtn_SortBy);
            var KText_SortBy = GetFromReference(UIPanel_Equipment.KText_SortBy);
            var KBtn_Compound = GetFromReference(UIPanel_Equipment.KBtn_Compound);
            var KText_Compound = GetFromReference(UIPanel_Equipment.KText_Compound);
            var KPlayerAnim = GetFromReference(UIPanel_Equipment.KPlayerAnim);

            var KImg_Exp = GetFromReference(UIPanel_Equipment.KImg_Exp);
            var KText_PlayerLevel = GetFromReference(UIPanel_Equipment.KText_PlayerLevel);
            var KText_PlayerName = GetFromReference(UIPanel_Equipment.KText_PlayerName);
            var KBtn_PlayerInfo = GetFromReference(UIPanel_Equipment.KBtn_PlayerInfo);

            var KText_MidAtk = GetFromReference(UIPanel_Equipment.KText_MidAtk);
            var KText_MidHp = GetFromReference(UIPanel_Equipment.KText_MidHp);

            this.GetRectTransform().SetOffset(0, 0, 0, 0);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var uiMain))
            {
                var uis = uiMain as UIPanel_Main;
                uis.ShowLevelExp(ResourcesSingleton.Instance.UserInfo, KImg_Exp,
                    KText_PlayerLevel);
            }

            KMid.GetRectTransform().SetWidth(Screen.width);
            KText_PlayerName.GetTextMeshPro().SetTMPText(ResourcesSingleton.Instance.UserInfo.Nickname);
            //KImg_LeftAtk.GetImage().SetSpriteAsync("icon_equip_tips_atk", false).Forget();
            //KImg_LeftHp.GetImage().SetSpriteAsync("icon_equip_tips_heart", false).Forget();
            var str = JiYuUIHelper.GetRewardTextIconName("icon_atk");
            KText_MidAtk.GetTextMeshPro().SetTMPText(str + tblanguage.Get("attr_atk").current);
            str = JiYuUIHelper.GetRewardTextIconName("icon_hp");
            KText_MidHp.GetTextMeshPro().SetTMPText(str + tblanguage.Get("attr_hp").current);
            KText_Compound.GetTextMeshPro()
                .SetTMPText(tblanguage.Get("equip_merge_title").current);
            RefreshPlayerProperty();

            var palyerAnimaition = KPlayerAnim.GetComponent<SkeletonGraphic>();
            //palyerAnimaition.Skeleton.SetAttachment("Weapon", "101");
            palyerAnimaition.startingAnimation = "idle";
            palyerAnimaition.startingLoop = true;
            //palyerAnimaition.AnimationState.SetAnimation(0, "idle", true);
            palyerAnimaition.Initialize(true);

            var mygameequip = new MyGameEquip
            {
                equip = new EquipDto()
                {
                    PartId = -1,
                    //RoleId = 0,
                    EquipId = 101,
                    //PosId = 0,
                    Quality = 1,
                    EquipLevel = 1,
                    //MainEntryId = 0,
                    //MainEntryInit = 0,
                    //MainEntryGrow = 0,
                },
                isWearing = false,
                canCompound = false
            };
            const float itemWidth = 195f;
            var horizontalLayoutGroup = KTop_EquipPos.GetComponent<HorizontalLayoutGroup>();
            var interval = (Screen.width - itemWidth * 5f) / 6f;
            horizontalLayoutGroup.spacing = interval;
            horizontalLayoutGroup.padding.left = (int)interval;
            horizontalLayoutGroup.padding.right = (int)interval;

            var wearList = KTop_EquipPos.GetList();
            wearList.Clear();
            for (int i = 0; i < 5; i++)
            {
                Transform child = wearList.Get().GetChild(i);

                UI parent = wearList.Create(child.gameObject, true);
                //this.AddChild(parent);
                var ui = await UIHelper.CreateAsync(parent, UIType.UICommon_EquipItem, mygameequip, EquipPanelType.Main,
                    child) as UICommon_EquipItem;
                ui.SetActive(false);
                parent.AddChild(ui);
                wearEquipsDic.TryAdd(i + 2, ui);
                SetWearedEquipOnClick(wearEquipsDic, i + 2, ui);

                var rect = ui.GetRectTransform();
                rect.SetAnchoredPosition(0, 0);
                rect.SetPivot(new Vector2(0.5f, 0.5f));
                rect.SetAnchorMax(Vector2.one);
                rect.SetAnchorMin(Vector2.zero);
                rect.SetOffset(0, 0, 0, 0);
            }

            //wearList.
            //KImg_Atk.Dispose();
            var ui0 = await UIHelper.CreateAsyncWithPrefabKey(KImg_Atk, UIType.UICommon_EquipItem,
                UIPathSet.UICommon_EquipItemWeapon, mygameequip, EquipPanelType.MainWeapon,
                KImg_Atk.GameObject.transform) as UICommon_EquipItem;

            KImg_Atk.AddChild(ui0);
            var rect0 = ui0.GetRectTransform();
            rect0.SetAnchoredPosition(0, 0);
            rect0.SetPivot(new Vector2(0.5f, 0.5f));
            rect0.SetAnchorMax(Vector2.one);
            rect0.SetAnchorMin(Vector2.zero);
            rect0.SetOffset(0, 0, -5, 5);
            rect0.SetScale(new Vector3(0.92f, 0.92f, 0.92f));
            ui0.SetActive(false);
            wearEquipsDic.TryAdd(1, ui0);
            SetWearedEquipOnClick(wearEquipsDic, 1, ui0);
            RefreshAllWearEquip();

            if (isAllEquip)
            {
                KText_SortBy.GetTextMeshPro().SetTMPText(tblanguage.Get("equip_sort_quality").current);
            }
            else
            {
                KText_SortBy.GetTextMeshPro().SetTMPText(tblanguage.Get("equip_sort_position").current);
            }

            scrollRect = this.GetXScrollRect();
            scrollRect.SetVerticalNormalizedPosition(1);
            //TODO:
            scrollRect.OnDrag.Add(OnScrollRectDragging);
            scrollRect.OnBeginDrag.Add(OnDestoryAllTips);

            scrollRect.OnEndDrag.Add(() =>
            {
                curWidth = 0;
                scrollRect.Get().vertical = true;
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                {
                    var uis = ui as UIPanel_JiyuGame;
                    uis.OnEndDrag();
                }

                //OnDestoryAllTips();
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_SortBy, () =>
            {
                Log.Debug($"KBtn_SortBy", Color.green);
                isAllEquip = !isAllEquip;

                OnClickActionEvent(2101, true);
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Compound, () =>
            {
                Log.Debug($"KBtn_Compound", Color.green);
                UIHelper.CreateAsync(UIType.UIPanel_Compound);
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_PlayerInfo, () => { });
        }

        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.U))
            // {
            //     Log.Debug($" cct.Cancel(); cct.Cancel();", Color.green);
            //     cct.Cancel();
            // }
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            //timerId = timerMgr.StartRepeatedTimer(updateInternal, this.Update);
            timerId = timerMgr.RepeatedFrameTimer(this.Update);
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


        void OnClickTips()
        {
            var KBtn_Mask = GetFromReference(UIPanel_Equipment.KBtn_Mask);
            var KContent_Equip = GetFromReference(UIPanel_Equipment.KContent_Equip);

            KBtn_Mask.GetButton().OnClick.Add(() =>
            {
                OnDestoryAllTips();
                DestorySelected();
            });

            var rect = KContent_Equip.GetRectTransform();
            var closeHeigth = 64 * 3f;
            //scrollRect.SetVerticalNormalizedPosition(0.5f);
            //scrollRect.OnValueChanged.
            //scrollRect.
            //scrollRect.
            // scrollRect.OnDrag.Add((a) =>
            // {
            //     OnDestoryAllTips();
            // });
            // scrollRect.OnValueChanged.Add((f) =>
            // {
            //     var height = rect.Height();
            //
            //     //TODO:
            //     if (!isDone)
            //     {
            //         if (math.abs(math.length(lastScrollValue) - math.length(f)) > (closeHeigth * 2 / height))
            //         {
            //             OnDestoryAllTips();
            //             lastScrollValue = f;
            //         }
            //     }
            //     else
            //     {
            //         // OnDestoryAllTips();
            //         // lastScrollValue = f;
            //
            //         // if (math.abs(math.length(lastScrollValue) - math.length(f)) > math.EPSILON)
            //         // {
            //         //     OnDestoryAllTips();
            //         //     lastScrollValue = f;
            //         //     //lastScrollValue = f;
            //         // }
            //         // if (math.abs(math.length(lastScrollValue) - math.length(f)) > (closeHeigth / 2f) / height)
            //         // {
            //         //     OnDestoryAllTips();
            //         //     lastScrollValue = f;
            //         // }
            //     }
            // });
        }

        // private void RegisterWidget()
        // {
        //     var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
        //     this.GetButton(UIPanel_Equipment.KBtn_Common)?.OnClick.Add(() => SortByRule(KBottom, cct.Token).Forget());
        //     this.GetButton(UIPanel_Equipment.KBtn_Common_1)?.OnClick.Add(OpenCompound);
        // }


        private void InitBottom(float buttom = 0)
        {
            return;
            var midHeight = GetFromReference(UIPanel_Equipment.KMid).GetRectTransform().Height();
            var topHeight = GetFromReference(UIPanel_Equipment.KTop).GetRectTransform().Height();
            var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
            var KContent = GetFromReference(UIPanel_Equipment.KContent_Equip);

            var thisHeight = this.GetRectTransform().Height();
            var beforeBottomHeight0 = thisHeight - (midHeight + topHeight);
            var bottomRect = KBottom.GetRectTransform();
            var bottomHeight = bottomRect.Height() - beforeBottomHeight0;


            //bottomHeight = math.clamp(bottomHeight, beforeBottomHeight0, bottomHeight);
            var contentRect = KContent.GetRectTransform();
            var bottomNew = bottomHeight - contentRect.OffsetMax().y;


            // if (buttom != 0)
            // {
            //     contentRect.SetOffsetWithBottom(-math.abs(buttom));
            //     return;
            // }

            if (bottomNew <= 0)
            {
                //bottomNew = 0;
                contentRect.SetOffsetWithTop(0);
                contentRect.SetOffsetWithBottom(0);
                return;
            }


            contentRect.SetHeight(bottomHeight + 50);
            //contentRect.SetOffsetWithTop(0);
            contentRect.SetOffsetWithLeft(0);
            contentRect.SetOffsetWithRight(0);
            //contentRect.SetOffsetWithBottom(-bottomNew);
            // var scrollRect = this.GetScrollRect();
            // scrollRect.SetVerticalNormalizedPosition(1);
        }


        //初始化二级页签的信息,为模块添加指定的监听事件
        public async UniTaskVoid InitTab2WidegetInfo(List<ModuleInfoStruct> args)
        {
            //Log.Error($"InitTab2WidegetInfo");
            // var KSubPanel_CommonBtn3 = GetFromReference(UIPanel_Equipment.KSubPanel_CommonBtn3);
            // var KSubPanel_CommonBtn3_1 = GetFromReference(UIPanel_Equipment.KSubPanel_CommonBtn3_1);
            var KImg_Strip = GetFromReference(UIPanel_Equipment.KImg_Strip);
            var list = KImg_Strip.GetList();
            float offset = 200;
            int countIndex = 0;

            list.Clear();
            foreach (var item in args)
            {
                var moduleId = item.moduleId;
                var ui =
                    await list.CreateWithUITypeAsync(UIType.UISubPanel_EquipBtn, moduleId,
                        false) as UISubPanel_EquipBtn;
                var KBtn = ui.GetFromReference(UISubPanel_EquipBtn.KBtn);
                var KText_Name = ui.GetFromReference(UISubPanel_EquipBtn.KText_Name);
                var KImg_Bottom = ui.GetFromReference(UISubPanel_EquipBtn.KImg_Bottom);

                var uiRect = ui.GetRectTransform();
                uiRect.SetPivot(new Vector2(0, 0.5f));
                uiRect.SetAnchorMin(new Vector2(0, 0.5f));
                uiRect.SetAnchorMax(new Vector2(0, 0.5f));

                uiRect.SetAnchoredPosition(new Vector2(offset * countIndex + 30f, 6f));
                countIndex++;
                KImg_Bottom.SetActive(true);
                KText_Name.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(item.name).current);
                KImg_Bottom.GetImage().SetSpriteAsync("img_equip_unselected", false);
                if (item.sortValues == 1)
                {
                    KImg_Bottom.GetImage().SetSpriteAsync("img_equip_selected", false);

                    OnClickActionEvent(item.moduleId);
                }

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn,
                    async () =>
                    {
                        foreach (var item in list.Children)
                        {
                            var ui = item as UISubPanel_EquipBtn;
                            var KImg_Bottom = ui.GetFromReference(UISubPanel_EquipBtn.KImg_Bottom);
                            KImg_Bottom.GetImage().SetSpriteAsync("img_equip_unselected", false);
                        }

                        KImg_Bottom.GetImage().SetSpriteAsync("img_equip_selected", false);
                        OnClickActionEvent(moduleId);
                        SortContentEquip().Forget();
                    });
            }
        }

        public void GoToSubPanel(int moduleId)
        {
            var KImg_Strip = GetFromReference(UIPanel_Equipment.KImg_Strip);
            var list = KImg_Strip.GetList();
            foreach (var item in list.Children)
            {
                var ui = item as UISubPanel_EquipBtn;
                ui.ChageState2(false, ChangefontSize);
            }

            foreach (var item in list.Children)
            {
                var ui = item as UISubPanel_EquipBtn;
                if (ui.tagFuncId == moduleId)
                {
                    ui.ChageState2(true, ChangefontSize);
                    OnClickActionEvent(moduleId);
                }
            }
        }

        bool isPaper(int id)
        {
            if (id >= 1020001 && id <= 1020006)
            {
                return true;
            }

            return false;
        }

        public bool HasCommonLabel(CommonLabelType type)
        {
            var itemConfig = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;

            switch (type)
            {
                case CommonLabelType.Prop:


                    foreach (var item in ResourcesSingleton.Instance.items)
                    {
                        var myitem = itemConfig.Get(item.Key);
                        if (myitem.page == 1 && !isPaper(item.Key) && myitem.displayYn == 0)
                        {
                            return true;
                        }
                    }

                    break;
                case CommonLabelType.Consume:
                    foreach (var item in ResourcesSingleton.Instance.items)
                    {
                        var myitem = itemConfig.Get(item.Key);
                        if (myitem.page == 2 && !isPaper(item.Key) && myitem.displayYn == 0)
                        {
                            return true;
                        }
                    }

                    break;
                case CommonLabelType.Paper:
                    foreach (var item in ResourcesSingleton.Instance.items)
                    {
                        var myitem = itemConfig.Get(item.Key);
                        if (isPaper(item.Key) && myitem.displayYn == 0)
                        {
                            return true;
                        }
                    }

                    break;
                case CommonLabelType.GeneralMaterial:

                    if (ResourcesSingleton.Instance.equipmentData.isMaterials.Count > 0)
                    {
                        return true;
                    }

                    break;
                case CommonLabelType.Weapon: //武器
                    foreach (var item in ResourcesSingleton.Instance.equipmentData.equipments)
                    {
                        int equipId = item.Value.equip.EquipId * 100 + item.Value.equip.Quality;
                        var equip = equip_data.Get(equipId);

                        if ((int)type == equip.posId && item.Value.equip.EquipId % 100 != 0)
                        {
                            return true;
                        }
                    }

                    break;
                case CommonLabelType.Clothing: //衣服
                    foreach (var item in ResourcesSingleton.Instance.equipmentData.equipments)
                    {
                        int equipId = item.Value.equip.EquipId * 100 + item.Value.equip.Quality;
                        var equip = equip_data.Get(equipId);

                        if ((int)type == equip.posId && item.Value.equip.EquipId % 100 != 0)
                        {
                            return true;
                        }
                    }

                    break;
                case CommonLabelType.Glove: //手套
                    foreach (var item in ResourcesSingleton.Instance.equipmentData.equipments)
                    {
                        int equipId = item.Value.equip.EquipId * 100 + item.Value.equip.Quality;
                        var equip = equip_data.Get(equipId);

                        if ((int)type == equip.posId && item.Value.equip.EquipId % 100 != 0)
                        {
                            return true;
                        }
                    }

                    break;
                case CommonLabelType.Pants: //裤子
                    foreach (var item in ResourcesSingleton.Instance.equipmentData.equipments)
                    {
                        int equipId = item.Value.equip.EquipId * 100 + item.Value.equip.Quality;
                        var equip = equip_data.Get(equipId);

                        if ((int)type == equip.posId && item.Value.equip.EquipId % 100 != 0)
                        {
                            return true;
                        }
                    }

                    break;
                case CommonLabelType.Belt: //腰带
                    foreach (var item in ResourcesSingleton.Instance.equipmentData.equipments)
                    {
                        int equipId = item.Value.equip.EquipId * 100 + item.Value.equip.Quality;
                        var equip = equip_data.Get(equipId);

                        if ((int)type == equip.posId && item.Value.equip.EquipId % 100 != 0)
                        {
                            return true;
                        }
                    }

                    break;
                case CommonLabelType.Shoes: //鞋子
                    foreach (var item in ResourcesSingleton.Instance.equipmentData.equipments)
                    {
                        int equipId = item.Value.equip.EquipId * 100 + item.Value.equip.Quality;
                        var equip = equip_data.Get(equipId);

                        if ((int)type == equip.posId && item.Value.equip.EquipId % 100 != 0)
                        {
                            return true;
                        }
                    }

                    break;
                case CommonLabelType.AllEquip: //按品质排序
                    int matCount = 0;

                    foreach (var VARIABLE in ResourcesSingleton.Instance.equipmentData.isMaterials)
                    {
                        matCount += VARIABLE.Value;
                    }

                    if (ResourcesSingleton.Instance.equipmentData.equipments.Count > matCount)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

        public bool EquipIsEmpty()
        {
            bool hasPaper = false;
            for (int i = 1020001; i <= 1020006; i++)
            {
                if (ResourcesSingleton.Instance.items.ContainsKey(i))
                {
                    hasPaper = true;
                    break;
                }
            }

            if (ResourcesSingleton.Instance.equipmentData.equipments.Count < 1 &&
                !hasPaper)
            {
                return true;
            }

            return false;
        }

        private bool BagIsEmpty()
        {
            int countPaper = 0;

            for (int i = 1020001; i <= 1020006; i++)
            {
                if (ResourcesSingleton.Instance.items.ContainsKey(i))
                {
                    countPaper++;
                }
            }

            if (ResourcesSingleton.Instance.items.Count - countPaper > 0)
            {
                return false;
            }

            return true;
        }

        //按品质或者部位排序
        private async UniTaskVoid OnClickEquip(CancellationToken cct)
        {
            //DestroyLables();
            // if (EquipIsEmpty())
            //     return;

            var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
            var KText_SortBy = GetFromReference(UIPanel_Equipment.KText_SortBy);

            var list = KBottom.GetList();
            list.Clear();
            List<UniTask> tasks = new List<UniTask>();
            ResourcesSingleton.Instance.equipmentData.isCompoundSort = false;
            //按照品质排序
            if (isAllEquip)
            {
                KText_SortBy.GetTextMeshPro().SetTMPText(tblanguage.Get("equip_sort_quality").current);
                if (HasCommonLabel(CommonLabelType.AllEquip))
                {
                    //按品质排序的装备
                    var ui = await list.CreateWithUITypeAsync(UIType.UICommon_Label, CommonLabelType.AllEquip, false,
                            false,
                            cct) as
                        UICommon_Label;
                    // var ui = await UIHelper.CreateAsync(this, UIType.UICommon_Label, (CommonLabelType)10, false,
                    //         KBottom.GameObject.transform, cct) as
                    //     UICommon_Label;
                    var task = ui.InitEquip(CommonLabelType.AllEquip, cct);
                    tasks.Add(task);
                    UICommon_Labels.Add(ui);
                }
            }
            //按照部位排序
            else
            {
                KText_SortBy.GetTextMeshPro().SetTMPText(tblanguage.Get("equip_sort_position").current);

                for (int i = 1; i < 7; i++)
                {
                    if (!HasCommonLabel((CommonLabelType)i))
                        continue;
                    var ui = await list.CreateWithUITypeAsync(UIType.UICommon_Label, (CommonLabelType)i, true, false,
                            cct) as
                        UICommon_Label;

                    UICommon_Labels.Add(ui);
                    var task = ui.InitEquip((CommonLabelType)i, cct);
                    tasks.Add(task);
                }
            }

            for (int i = 7; i < 9; i++)
            {
                var index = i;
                if (!HasCommonLabel((CommonLabelType)index))
                {
                    //Log.Error($"{(CommonLabelType)i}");
                    continue;
                }

                var ui = await list.CreateWithUITypeAsync(UIType.UICommon_Label, (CommonLabelType)index, true, false,
                        cct) as
                    UICommon_Label;
                // var ui =
                //     await UIHelper.CreateAsync(this, UIType.UICommon_Label, (CommonLabelType)i, true,
                //             KBottom.GameObject.transform, cct) as
                //         UICommon_Label;
                UICommon_Labels.Add(ui);
                var task = ui.InitBag((CommonLabelType)index, cct);
                tasks.Add(task);
            }

            SortLabels();
            //SetBottomPos(UICommon_Labels, false);
            await UniTask.WhenAll(tasks);


            //SortItems();
            // foreach (var VARIABLE in UICommon_Labels)
            // {
            //     SortItems((int)VARIABLE.type);
            // }

            //
            await UniTask.Yield();
            InitBottom();
            ForceRefreshLayout();
            isDone = true;
            tasks.Clear();
        }


        private async UniTaskVoid OnClickBag(CancellationToken cct)
        {
            // if (BagIsEmpty())
            //     return;
            var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
            var list = KBottom.GetList();
            list.Clear();
            List<UniTask> tasks = new List<UniTask>();
            for (int i = 9; i < 11; i++)
            {
                if (!HasCommonLabel((CommonLabelType)i))
                    continue;
                var ui = await list.CreateWithUITypeAsync(UIType.UICommon_Label, (CommonLabelType)i, true, false,
                        cct) as
                    UICommon_Label;

                UICommon_Labels.Add(ui);
                var task = ui.InitBag((CommonLabelType)i, cct);
                tasks.Add(task);
            }

            //SetBottomPos(UICommon_Labels, false);
            SortLabels();
            await UniTask.WhenAll(tasks);

            //this.cct = new CancellationTokenSource();
            await UniTask.Yield();
            InitBottom();
            ForceRefreshLayout();
            isDone = true;
            tasks.Clear();
        }

        void ForceRefreshLayout()
        {
            var KBottom = GetFromReference(UIPanel_Equipment.KBottom);

            var list = KBottom.GetList();
            foreach (var ui in list.Children)
            {
                var uilabel = ui as UICommon_Label;
                uilabel.ForceRefreshLayout();
            }

            JiYuUIHelper.ForceRefreshLayout(KBottom);
        }

        private void SortLabels()
        {
            //UICommon_Labels.Sort((a, b) => a.type.CompareTo(b.type));

            var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
            var list = KBottom.GetList();
            list.Sort((aui, bui) =>
            {
                var a = aui as UICommon_Label;
                var b = bui as UICommon_Label;

                return a.type.CompareTo(b.type);
            });


            // for (int i = 0; i < UICommon_Labels.Count; i++)
            // {
            //     UICommon_Labels[i].GameObject.transform.SetSiblingIndex(i);
            // }
        }

        public void RefreshItemAndEquip()
        {
            Log.Debug($"RefreshItemAndEquip");
            var temp = lastModuleId;
            lastModuleId = 0;

            OnClickActionEvent(temp, true);
        }

        public void OnClickActionEvent(int moduleId, bool isForceFresh = false)
        {
            if (moduleId == lastModuleId && !isForceFresh)
            {
                return;
            }

            // if (lastisAllEquip != isAllEquip)
            // {
            //     return;
            // }
            //JiYuUIHelper.SortEquipments();

            lastModuleId = moduleId;
            isDone = false;
            InitBottom(9999);
            OnDestoryAllTips();
            Log.Debug($"OnClickActionEvent", Color.green);
            //清空内容

            var KBtn_SortBy = GetFromReference(UIPanel_Equipment.KBtn_SortBy);
            var KBtn_Compound = GetFromReference(UIPanel_Equipment.KBtn_Compound);

            UICommon_Labels.Clear();
            if (this.cts != null)
            {
                this.cts.Cancel();
                this.cts.Dispose();
                this.cts = new CancellationTokenSource();
            }

            //产生新的内容


            KBtn_SortBy.SetActive(moduleId == 2101 ? true : false);
            KBtn_Compound.SetActive(moduleId == 2101 ? true : false);

            switch (moduleId)
            {
                case 2101:
                    OnClickEquip(cts.Token).Forget();
                    break;
                case 2102:
                    OnClickBag(cts.Token).Forget();
                    break;
                case 2103:
                    break;
                case 2104:
                    break;
            }
        }

        public override void OnFocus()
        {
            // if (isDone)
            // {
            //     return;
            // }
            //
            // var lastId = lastModuleId;
            // lastModuleId = 0;
            // OnClickActionEvent(lastId, false);
            //Log.Error($"UIPanel_Equipment OnFocus");
        }

        public override void OnBlur()
        {
            //Log.Error($"UIPanel_Equipment OnBlur");
        }

        protected override void OnClose()
        {
            Log.Debug($"UIPanel_Equipment OnClose");

            // foreach (var VARIABLE in wearEquipsDic)
            // {
            //     VARIABLE.Value.Dispose();
            // }

            //isCompuseDone = false;

            this.cts?.Cancel();
            this.cts?.Dispose();


            //lastClickTipItem = null;
            //OnDestoryAllTips();
            JiYuUIHelper.DestoryAllTips();
            wearEquipsDic.Clear();
            tempStructList.Clear();
            UICommon_Labels.Clear();
            //DestroyLables();
            //DestorySubBtns();
            base.OnClose();
        }
    }
}