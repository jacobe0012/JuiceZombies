//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Spine.Unity;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using DG.Tweening;
using System.Threading;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Activity_Monopoly)]
    internal sealed class UIPanel_Activity_MonopolyEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Activity_Monopoly;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Activity_Monopoly>();
        }
    }

    public partial class UIPanel_Activity_Monopoly : UI, IAwake<int>
    {
        public int activityId;

        private Tblanguage tblanguage;
        private Tbtag_func tbtag_func;
        private Tbblock tbblock;
        private Tbchapter tbchapter;
        private Tblevel tblevel;
        private Tbuser_avatar tbuser_avatar;
        private Tbuser_variable tbuser_variable;
        private Tbuser_level tbuserLevel;
        private Tbdraw_box tbdraw_box;
        private Tbtag tbtag;
        private Tbconstant tbconstant;
        private Tbchapter_box tbchapter_box;
        private Tbactivity tbactivity;
        private Tbdays_challenge tbdays_challenge;
        private Tbbattlepass tbbattlepass;
        private Tbpiggy_bank tbpiggy_bank;
        private Tbdays_sign tbdays_sign;
        private Tbenergy_shop tbenergy_shop;
        private Tbmonopoly tbmonopoly;
        private Tbmonopoly_cell tbmonopoly_cell;
        private Tbmonopoly_event tbmonopoly_event;
        private Tbmonopoly_shop tbmonopoly_shop;
        private Tbmonopoly_event_lotto tbmonopoly_event_lotto;
        private Tbtask_group tbtask_group;
        private Tbtask_type tbtask_type;
        private Tbtask tbtask;
        private Tbitem tbitem;
        private int lastGridIndex;
        private int tagFunc;
        private RectTransform[] cellRectList;
        private long timerId;
        private long endTime;

        private Vector2 playerOffset;
        private Vector3 diceReward;
        private Vector3 shopItemReward;
        private string m_RedDotName;
        private int lottoNum = 3;
        private bool isInit = false;
        private Tbquality tbquality;
        private CancellationTokenSource cts = new CancellationTokenSource();

        #region animArgs

        const float DiceRollSpeed = 1.5f;

        const float DustAnimDuration = 0.02f;

        const float ScaleStart = 1;
        const float ScaleBiggest = 1.0925f;
        const float ScaleSmallest = 0.725f;
        const float ScaleBiggestDuration = 0.35f;
        const float ScaleSmallestDuration = 0.15f;

        #endregion


        public async void Initialize(int args)
        {
            GetFromReference(KImg_TitleBg).GetComponent<CanvasGroup>().alpha = 0;
            await JiYuUIHelper.InitBlur(this);

            activityId = args;
            InitJson();
            var KImg_Title = GetFromReference(UIPanel_Activity_Monopoly.KImg_Title);
            //KImg_Title.SetActive(false);
            WebMessageHandler.Instance.AddHandler(CMD.QUERTSINGLEACTIVITY, OnMonopolyResponse);

            NetWorkManager.Instance.SendMessage(CMD.QUERTSINGLEACTIVITY, new IntValue()
            {
                Value = activityId
            });
            SetCloseTip(GetFromReference(KBtn_Mask));

            //long clientT = (long)(TimeHelper.ClientNowSeconds() - ResourcesSingleton.Instance.ServerDeltaTime / 1000f);
        }

        private void OnMonopolyResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERTSINGLEACTIVITY, OnMonopolyResponse);
            GameActivity gameActivity = new GameActivity();
            gameActivity.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug($"OnAllDataResponse is empty");
                //return;
            }

            if (ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.ContainsKey(activityId))
            {
                ResourcesSingleton.Instance.activity.activityMap.ActivityMap_[activityId] = gameActivity;
            }
            else
            {
                ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.Add(activityId,
                    gameActivity);
            }

            NetWorkManager.Instance.SendMessage(CMD.QUERYACTIVITYTASK, new IntValue
            {
                Value = activityId
            });
            var height1 = this.GetFromReference(KMid).GetRectTransform().AnchoredPosition().y;
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(KMid), height1, 100, cts.Token, 0.3f, true,
                true);
            //var height1 = this.GetFromReference(KContainerMid).GetRectTransform().AnchoredPosition().y;
            //JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(KContainerMid), height1, 100, 0.3f, true, true);

            InitNode();
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            tbblock = ConfigManager.Instance.Tables.Tbblock;
            tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            tbuser_avatar = ConfigManager.Instance.Tables.Tbuser_avatar;
            tbuser_variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbuserLevel = ConfigManager.Instance.Tables.Tbuser_level;
            tbdraw_box = ConfigManager.Instance.Tables.Tbdraw_box;
            tbtag = ConfigManager.Instance.Tables.Tbtag;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
            tbchapter_box = ConfigManager.Instance.Tables.Tbchapter_box;
            tbactivity = ConfigManager.Instance.Tables.Tbactivity;
            tblevel = ConfigManager.Instance.Tables.Tblevel;
            tbdays_challenge = ConfigManager.Instance.Tables.Tbdays_challenge;
            tbbattlepass = ConfigManager.Instance.Tables.Tbbattlepass;
            tbpiggy_bank = ConfigManager.Instance.Tables.Tbpiggy_bank;
            tbdays_sign = ConfigManager.Instance.Tables.Tbdays_sign;
            tbenergy_shop = ConfigManager.Instance.Tables.Tbenergy_shop;
            tbmonopoly = ConfigManager.Instance.Tables.Tbmonopoly;
            tbmonopoly_cell = ConfigManager.Instance.Tables.Tbmonopoly_cell;
            tbmonopoly_event = ConfigManager.Instance.Tables.Tbmonopoly_event;
            tbmonopoly_shop = ConfigManager.Instance.Tables.Tbmonopoly_shop;
            tbmonopoly_event_lotto = ConfigManager.Instance.Tables.Tbmonopoly_event_lotto;
            tbtask_group = ConfigManager.Instance.Tables.Tbtask_group;
            tbtask_type = ConfigManager.Instance.Tables.Tbtask_type;
            tbtask = ConfigManager.Instance.Tables.Tbtask;
            tbitem = ConfigManager.Instance.Tables.Tbitem;
            tbquality = ConfigManager.Instance.Tables.Tbquality;
        }

        async void InitNode()
        {
            WebMessageHandler.Instance.AddHandler(CMD.MONOPOLYACTION, OnRollDiceResponse);
            WebMessageHandler.Instance.AddHandler(CMD.QUERYACTIVITYTASK, OnQueryMonopolyTaskResponse);
            var KConTainer = GetFromReference(UIPanel_Activity_Monopoly.KConTainer);
            var KContaineTop = GetFromReference(UIPanel_Activity_Monopoly.KContaineTop);
            var KContainerMid = GetFromReference(UIPanel_Activity_Monopoly.KContainerMid);
            var KContaineBottom = GetFromReference(UIPanel_Activity_Monopoly.KContaineBottom);
            var KCellList = GetFromReference(UIPanel_Activity_Monopoly.KCellList);
            var KText_Time = GetFromReference(UIPanel_Activity_Monopoly.KText_Time);
            var KBtn_Tip = GetFromReference(UIPanel_Activity_Monopoly.KBtn_Tip);
            var KText_EnergySum = GetFromReference(UIPanel_Activity_Monopoly.KText_EnergySum);
            var KText_Energy = GetFromReference(UIPanel_Activity_Monopoly.KText_Energy);
            var KText_ExchangeLeft = GetFromReference(UIPanel_Activity_Monopoly.KText_ExchangeLeft);
            var KBtn_ExchangeLeft = GetFromReference(UIPanel_Activity_Monopoly.KBtn_ExchangeLeft);
            var KBtn_ExchangeRight = GetFromReference(UIPanel_Activity_Monopoly.KBtn_ExchangeRight);
            var KText_ExchangeRight = GetFromReference(UIPanel_Activity_Monopoly.KText_ExchangeRight);
            var KText_RollNum = GetFromReference(UIPanel_Activity_Monopoly.KText_RollNum);
            var KText_DiceNum = GetFromReference(UIPanel_Activity_Monopoly.KText_DiceNum);
            var KBtn_Dice = GetFromReference(UIPanel_Activity_Monopoly.KBtn_Dice);
            var KBtn_DiceAdd = GetFromReference(UIPanel_Activity_Monopoly.KBtn_DiceAdd);
            var KBtn_Mask = GetFromReference(UIPanel_Activity_Monopoly.KBtn_Mask);
            var KBtn_CloseMask = GetFromReference(UIPanel_Activity_Monopoly.KBtn_CloseMask);
            var KBtn_Close = GetFromReference(UIPanel_Activity_Monopoly.KBtn_Close);
            var KPlayer = GetFromReference(UIPanel_Activity_Monopoly.KPlayer);
            var KSpine_Player = GetFromReference(UIPanel_Activity_Monopoly.KSpine_Player);
            var KImg_Lotto = GetFromReference(UIPanel_Activity_Monopoly.KImg_Lotto);
            var KImg_Title = GetFromReference(UIPanel_Activity_Monopoly.KImg_Title);
            var KImg_RedDotLeft = GetFromReference(UIPanel_Activity_Monopoly.KImg_RedDotLeft);
            var KImg_RedDotMid = GetFromReference(UIPanel_Activity_Monopoly.KImg_RedDotMid);
            var KImg_RedDotRight = GetFromReference(UIPanel_Activity_Monopoly.KImg_RedDotRight);
            var KText_Des = GetFromReference(UIPanel_Activity_Monopoly.KText_Des);
            var KCommon_ItemTips = GetFromReference(UIPanel_Activity_Monopoly.KCommon_ItemTips);
            var KMonoPoly_Touzi = GetFromReference(UIPanel_Activity_Monopoly.KMonoPoly_Touzi);
            var KMidMask = GetFromReference(UIPanel_Activity_Monopoly.KMidMask);
            KMidMask.GetButton().OnClick.Add(() => { JiYuUIHelper.DestoryAllTips(); });

            KMonoPoly_Touzi.SetActive(false);
            var RenderHeight = KContaineTop.GetRectTransform().Height() +
                               KContaineBottom.GetRectTransform().Height() +
                               KContainerMid.GetRectTransform().Height();
            var upOffset = (Screen.height - RenderHeight) / 2f;

            KConTainer.GetRectTransform().SetAnchoredPositionY(-upOffset);

            if (!ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                    out var gameActivity))
            {
                Close();
                return;
            }

            var activity = tbactivity.Get(activityId);
            var monopoly = tbmonopoly.Get(activity.link);
            tagFunc = monopoly.tagFunc;
            InitRedDot();

            string desc = tblanguage.Get(monopoly.desc).current;
            KText_Des.GetTextMeshPro().SetTMPText(desc);
            var height = KText_Des.GetTextMeshPro().Get().preferredHeight;
            var width = KText_Des.GetTextMeshPro().Get().preferredWidth;
            KText_Des.GetRectTransform().SetHeight(height);
            GetFromReference(Kcontent).GetRectTransform().SetHeight(height + 76 * 2);
        

            KImg_Title.GetImage().SetSprite(monopoly.pic, true);


            KText_RollNum.GetTextMeshPro().SetTMPText(tblanguage.Get("monopoly_active_button_text").current);

            diceReward = new Vector3(5, monopoly.diceItem, 1);
            shopItemReward = new Vector3(5, monopoly.shopItem, 1);

            
            string leftBtnStr =
                $"{JiYuUIHelper.GetRewardTextIconName(shopItemReward)}{tblanguage.Get("monopoly_shop_button").current}";
            
            KText_ExchangeLeft.GetTextMeshPro().SetTMPText(leftBtnStr);
            KText_ExchangeRight.GetTextMeshPro().SetTMPText(tblanguage.Get("monopoly_task_button").current);
            string diceStr =
                $"{JiYuUIHelper.GetRewardTextIconName(diceReward)}x{Mathf.Min(999, JiYuUIHelper.GetRewardCount(diceReward))}";

            string shopItemStr =
                $"{JiYuUIHelper.GetRewardTextIconName(shopItemReward)}x{Mathf.Min(99999, JiYuUIHelper.GetRewardCount(shopItemReward))}";
            string rewardStr =
                $"{JiYuUIHelper.GetRewardTextIconName(shopItemReward)}x{gameActivity.MonopolyRecord.TokenWeight}";
            //string rewardStr = $"x{gameActivity.MonopolyRecord.TokenWeight}";

            KText_DiceNum.GetTextMeshPro().SetTMPText(diceStr);

            KImg_RedDotLeft.SetActive(false);

            var itemStr = $"{m_RedDotName}|Pos0";
            KImg_RedDotMid.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr) > 0);
            RedDotManager.Instance.SetRedPointCnt(itemStr, (int)JiYuUIHelper.GetRewardCount(diceReward));
            var redGO = KImg_RedDotMid.GameObject;
            RedDotManager.Instance.AddListener(itemStr, (num) =>
            {
                Log.Debug($"{itemStr} {num}", Color.cyan);
                //redGO.SetActive(num > 0);
                KImg_RedDotMid?.SetActive(num > 0);
            });

            var itemStr1 = $"{m_RedDotName}|Pos1";
            KImg_RedDotRight.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr1) > 0);
            RedDotManager.Instance.AddListener(itemStr1, (num) =>
            {
                //Log.Debug($"{itemStr1} {num}", Color.cyan);
                KImg_RedDotRight?.SetActive(num > 0);
            });

            KText_Energy.GetTextMeshPro().SetTMPText(shopItemStr);


            KText_EnergySum.GetTextMeshPro().SetTMPText(rewardStr);
            KImg_Lotto.SetActive(false);
            KBtn_Mask.SetActive(false);
            KCommon_ItemTips.SetActive(false);
            KBtn_DiceAdd.SetActive(monopoly.bugDiceYn == 1);

            endTime = gameActivity.MonopolyRecord.EndTime;
            //lastGridIndex= gameActivity.MonopolyRecord.MonopolyGridNum;
            await UniTask.Yield();
            InitGridList();

            Log.Debug($"MonopolyGridNum {gameActivity.MonopolyRecord.MonopolyGridNum}");

            UpdateGrid(gameActivity.MonopolyRecord.MonopolyGridNum).Forget();

            KPlayer.GetRectTransform().SetAnchoredPosition(new Vector2(-64.5f, 0f));
            SetPlayerPos(gameActivity.MonopolyRecord.MonopolyGridNum).Forget();
            //gameActivity.MonopolyRecord.
            StartTimer();
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Tip,
                () => { KCommon_ItemTips.SetActive(!KCommon_ItemTips.GameObject.activeSelf); });

            KBtn_CloseMask.GetButton().OnClick.Add(() => { Close(); });

            JiYuTweenHelper.JiYuOnClickNoAnim(KBtn_Dice, async () =>
            {
                Log.Debug($"OnDiceClick");
                if (JiYuUIHelper.GetRewardCount(diceReward) <= 0)
                {
                    var ui = await UIHelper.CreateAsync(UIType.UIPanel_BuyDice, activityId);
                    JiYuTweenHelper.SetScaleWithBounce(ui.GetFromReference(UIPanel_BuyDice.KBg),
                        cancellationToken: cts.Token);
                    return;
                }

                AudioManager.Instance.PlayFModAudio(1401);
                //OnRollDiceResponse();
                NetWorkManager.Instance.SendMessage(CMD.MONOPOLYACTION);
            }, 1f);


            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, () => { Close(); });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_DiceAdd,
                async () =>
                {
                    var ui = await UIHelper.CreateAsync(UIType.UIPanel_BuyDice, activityId);
                    JiYuTweenHelper.SetScaleWithBounce(ui.GetFromReference(UIPanel_BuyDice.KBg),
                        cancellationToken: cts.Token);
                });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_ExchangeLeft,
                () => { JiYuUIHelper.DestoryAllTips(); UIHelper.CreateAsync(UIType.UIPanel_MonopolyShop, activityId); });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_ExchangeRight,
                () =>
                {
                    JiYuUIHelper.DestoryAllTips();
                    NetWorkManager.Instance.SendMessage(CMD.QUERYACTIVITYTASK, new IntValue
                    {
                        Value = activityId
                    });
                });
            JiYuTweenHelper.PlayUIImageTranstionFX(KImg_Title, cts.Token, "A1DD01", JiYuTweenHelper.UIDir.UpLeft);
            await UniTask.Delay(500, cancellationToken: cts.Token);
            JiYuTweenHelper.PlayUIImageSweepFX(GetFromReference(UIPanel_Activity_Monopoly.KBtn_Dice), cts.Token,
                "FFFFFF");
            JiYuTweenHelper.SetEaseAlphaAndPosRtoL(GetFromReference(KImg_TitleBg), 20, cancellationToken: cts.Token);
        }


        void InitRedDot()
        {
            m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);
            for (int i = 0; i < 2; i++)
            {
                var itemStr = $"{m_RedDotName}|Pos{i}";
                RedDotManager.Instance.InsterNode(itemStr);
            }
        }

        public void Refresh()
        {
            var KText_DiceNum = GetFromReference(UIPanel_Activity_Monopoly.KText_DiceNum);
            var KText_Energy = GetFromReference(UIPanel_Activity_Monopoly.KText_Energy);
            string diceStr =
                $"{JiYuUIHelper.GetRewardTextIconName(diceReward)}x{Mathf.Min(999, JiYuUIHelper.GetRewardCount(diceReward))}";
            string shopItemStr =
                $"{JiYuUIHelper.GetRewardTextIconName(shopItemReward)}x{Mathf.Min(99999, JiYuUIHelper.GetRewardCount(shopItemReward))}";
            KText_Energy.GetTextMeshPro().SetTMPText(shopItemStr);
            KText_DiceNum.GetTextMeshPro().SetTMPText(diceStr);
            var itemStr = $"{m_RedDotName}|Pos0";
            RedDotManager.Instance.SetRedPointCnt(itemStr, (int)JiYuUIHelper.GetRewardCount(diceReward));
        }

        async void InitGridList()
        {
            var KCellList = GetFromReference(UIPanel_Activity_Monopoly.KCellList);
            var cellListRect = KCellList.GameObject.GetComponent<RectTransform>();
            var cellCount = tbmonopoly_cell.DataList.Where((a) => a.group == 1).ToList().Count;
            cellRectList = new RectTransform[cellCount];
            for (int i = 0; i < cellListRect.childCount; i++)
            {
                int index = i;

                var child = cellListRect.GetChild(index);
                if (child.name.Contains("Grid"))
                {
                    int strIndex = child.name.IndexOf('_');

                    if (strIndex >= 0)
                    {
                        //Log.Debug($"list:{child.name.Substring(strIndex + 1)}");
                        if (int.TryParse(child.name.Substring(strIndex + 1), out int numberPart))
                        {
                            cellRectList[numberPart] = child.GetComponent<RectTransform>();
                        }
                    }
                }
            }

            foreach (var cell in cellRectList)
            {
                var SubPanel_GridAnim = FindChildByName(cell, "SubPanel_GridAnim");
                if (SubPanel_GridAnim == null)
                {
                    var uianim = await UIHelper.CreateAsync(this, UIType.UISubPanel_GridAnim, cell);
                    uianim.SetParent(this, false);
                    uianim.SetActive(false);
                    uianim.GetRectTransform().SetSiblingIndex(0);
                }
                else
                {
                    SubPanel_GridAnim.transform.SetSiblingIndex(0);
                }
            }
        }

        async UniTask PlayerMoveAnim(int lastGridIndex, int curGridIndex, int type = 0)
        {
            const float V = 500f;
            var KBtn_Mask = GetFromReference(UIPanel_Activity_Monopoly.KBtn_Mask);
            var KPlayer = GetFromReference(UIPanel_Activity_Monopoly.KPlayer);
            var KSpine_Player = GetFromReference(UIPanel_Activity_Monopoly.KSpine_Player);
            var playerRect = KPlayer.GetRectTransform();
            var sg = KSpine_Player.GetComponent<SkeletonGraphic>();
            var lastMonopolyCell = tbmonopoly_cell.Get(lastGridIndex);
            var curMonopolyCell = tbmonopoly_cell.Get(curGridIndex);
            var lastIndex = lastMonopolyCell.number - 1;
            var curIndex = curMonopolyCell.number - 1;
            var lastRect = cellRectList[lastIndex];
            var curRect = cellRectList[curIndex];

            var scaleX = Mathf.Abs(KSpine_Player.GetRectTransform().Scale2().x);

            KBtn_Mask.SetActive(true);
            EnableGridAnim(false, null);
            DoScaleAnim(lastRect, default, 0);

            //事件301传送
            if (type == 1)
            {
                var curPlayerRect = playerRect.AnchoredPosition();
                var s = math.length(curRect.AnchoredPosition() + playerOffset - curPlayerRect);
                var t = s / V;
                KSpine_Player.GetRectTransform().SetScaleX(-scaleX);
                playerRect.DoAnchoredPosition(curRect.AnchoredPosition() + playerOffset, t);
                EnableGridAnim(true, curRect);
                DoScaleAnim(curRect, default, 2).Forget();

                await UniTask.Delay((int)(1000 * t));
            }
            else
            {
                sg.startingLoop = true;
                sg.startingAnimation = "run";
                sg.Initialize(true);

                if (curIndex - lastIndex > 0)
                {
                    for (int i = 0; i < curIndex - lastIndex; i++)
                    {
                        var index = i;

                        var nextIndex0 = lastIndex + index;
                        var nextIndex1 = lastIndex + 1 + index;

                        var nextRect0 = cellRectList[nextIndex0];

                        var nextRect1 = cellRectList[nextIndex1];

                        var curPlayerRect = playerRect.AnchoredPosition();
                        var s = math.length(nextRect1.AnchoredPosition() + playerOffset - curPlayerRect);
                        var t = s / V;

                        if (nextRect1.AnchoredPosition().x - nextRect0.AnchoredPosition().x > 0)
                        {
                            KSpine_Player.GetRectTransform().SetScaleX(scaleX);
                        }
                        else if (nextRect1.AnchoredPosition().x - nextRect0.AnchoredPosition().x < 0)
                        {
                            KSpine_Player.GetRectTransform().SetScaleX(-scaleX);
                        }

                        playerRect.DoAnchoredPosition(nextRect1.AnchoredPosition() + playerOffset, t);

                        if (index == curIndex - lastIndex - 1 &&
                            !nextRect1.name.Contains("Grid_21"))
                        {
                            EnableGridAnim(true, nextRect1);
                            DoScaleAnim(nextRect1, default, 2).Forget();
                            Log.Debug($"name{nextRect1.name}");

                            //TODO:smokeAnim
                            if (nextRect1.name.Contains("Grid_0"))
                            {
                                sg.startingLoop = true;
                                sg.startingAnimation = "idle";
                                sg.Initialize(true);
                                await UpdateGrid(curGridIndex, true);
                                await UniTask.Delay(500);
                            }
                        }
                        else
                        {
                            DoScaleAnim(nextRect1).Forget();
                        }


                        await UniTask.Delay((int)(1000 * t));
                    }
                }
                else
                {
                    var count0 = cellRectList.Length - 1 - lastIndex + 1;
                    var count1 = curIndex;

                    for (int i = 0; i < count0; i++)
                    {
                        var index = i;

                        var nextIndex0 = lastIndex + index;

                        var nextIndex1 = lastIndex + 1 + index == cellRectList.Length ? 0 : lastIndex + 1 + index;
                        var nextRect0 = cellRectList[nextIndex0];

                        var nextRect1 = cellRectList[nextIndex1];


                        var curPlayerRect = playerRect.AnchoredPosition();
                        var s = math.length(nextRect1.AnchoredPosition() + playerOffset - curPlayerRect);
                        var t = s / V;

                        if (nextRect1.AnchoredPosition().x - nextRect0.AnchoredPosition().x > 0)
                        {
                            KSpine_Player.GetRectTransform().SetScaleX(scaleX);
                        }
                        else if (nextRect1.AnchoredPosition().x - nextRect0.AnchoredPosition().x < 0)
                        {
                            KSpine_Player.GetRectTransform().SetScaleX(-scaleX);
                        }

                        playerRect.DoAnchoredPosition(nextRect1.AnchoredPosition() + playerOffset, t);

                        DoScaleAnim(nextRect1).Forget();

                        await UniTask.Delay((int)(1000 * t));
                    }

                    //TODO:smokeAnim
                    sg.startingLoop = true;
                    sg.startingAnimation = "idle";
                    sg.Initialize(true);
                    await UpdateGrid(curGridIndex, true);
                    await UniTask.Delay(500);
                    sg.startingLoop = true;
                    sg.startingAnimation = "run";
                    sg.Initialize(true);

                    for (int i = 0; i < count1; i++)
                    {
                        var index = i;

                        var nextIndex0 = index;
                        var nextIndex1 = 1 + index;

                        var nextRect0 = cellRectList[nextIndex0];

                        var nextRect1 = cellRectList[nextIndex1];


                        var curPlayerRect = playerRect.AnchoredPosition();
                        var s = math.length(nextRect1.AnchoredPosition() + playerOffset - curPlayerRect);
                        var t = s / V;

                        if (nextRect1.AnchoredPosition().x - nextRect0.AnchoredPosition().x > 0)
                        {
                            KSpine_Player.GetRectTransform().SetScaleX(scaleX);
                        }
                        else if (nextRect1.AnchoredPosition().x - nextRect0.AnchoredPosition().x < 0)
                        {
                            KSpine_Player.GetRectTransform().SetScaleX(-scaleX);
                        }

                        playerRect.DoAnchoredPosition(nextRect1.AnchoredPosition() + playerOffset, t);

                        if (index == count1 - 1 &&
                            !nextRect1.name.Contains("Grid_21"))
                        {
                            EnableGridAnim(true, nextRect1);
                            DoScaleAnim(nextRect1, default, 2).Forget();
                            Log.Debug($"name{nextRect1.name}");
                        }
                        else
                        {
                            DoScaleAnim(nextRect1).Forget();
                        }

                        await UniTask.Delay((int)(1000 * t));
                    }
                }

                sg.startingLoop = true;
                sg.startingAnimation = "idle";
                sg.Initialize(true);
            }

            KBtn_Mask.SetActive(false);
        }

        public void EnableGridAnim(bool enable, Transform parent = null, string AnimName = "Anim_Shining")
        {
            if (enable)
            {
                var SubPanel_GridAnim = FindChildByName(parent, $"SubPanel_GridAnim");
                SubPanel_GridAnim?.SetActive(enable);
                var Anim_Shining = FindChildByName(SubPanel_GridAnim?.transform, AnimName);
                Anim_Shining?.SetActive(enable);
            }
            else
            {
                foreach (var rect in cellRectList)
                {
                    var SubPanel_GridAnim = FindChildByName(rect, $"SubPanel_GridAnim");
                    SubPanel_GridAnim?.SetActive(enable);
                    var Anim_Shining = FindChildByName(SubPanel_GridAnim?.transform, AnimName);
                    Anim_Shining?.SetActive(enable);
                }
            }
        }
        public void SetCloseTip(UI ui)
        {
            ui.GetButton().OnClick?.Add(() => { JiYuUIHelper.DestoryAllTips(); ui.SetActive(false); });
        }

        public GameObject FindChildByName(Transform parent, string name)
        {
            if (parent == null)
            {
                return null;
            }

            // 遍历当前父节点下的所有子节点
            foreach (Transform child in parent)
            {
                // 如果找到匹配名称的子节点，返回该子节点的GameObject
                if (child.name.Contains(name))
                {
                    return child.gameObject;
                }

                // 如果该子节点有子节点，递归查找
                GameObject found = FindChildByName(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            // 如果找不到，返回null
            return null;
        }


        public async UniTask<AsyncUnit> DoScaleAnim(RectTransform rec,
            CancellationToken cancellationToken = default, int type = 3)
        {
            if (rec == null)
            {
                return AsyncUnit.Default;
            }

            try
            {
                rec.SetScale(new float3(ScaleStart, ScaleStart, ScaleStart));
                //变小变大还原
                if (type == 3)
                {
                    var tween = rec.DOScale(new float3(ScaleSmallest, ScaleSmallest, ScaleSmallest),
                            ScaleSmallestDuration)
                        .SetEase(Ease.InQuad);
                    tween.SetUpdate(true);
                    await UniTask.Delay((int)(1000 * ScaleSmallestDuration));

                    var tween1 = rec.DOScale(new float3(ScaleBiggest, ScaleBiggest, ScaleBiggest), ScaleBiggestDuration)
                        .SetEase(Ease.InQuad);
                    tween1.SetUpdate(true);
                    await UniTask.Delay((int)(1000 * ScaleBiggestDuration));
                    var tween3 = rec.DOScale(new float3(ScaleStart, ScaleStart, ScaleStart), ScaleSmallestDuration)
                        .SetEase(Ease.InQuad);
                    tween3.SetUpdate(true);
                    await UniTask.Delay((int)(1000 * ScaleSmallestDuration));
                }
                //变大
                else if (type == 2)
                {
                    var tween = rec.DOScale(new float3(ScaleSmallest, ScaleSmallest, ScaleSmallest),
                            ScaleSmallestDuration)
                        .SetEase(Ease.InQuad);
                    tween.SetUpdate(true);
                    await UniTask.Delay((int)(1000 * ScaleSmallestDuration));

                    var tween1 = rec.DOScale(new float3(ScaleBiggest, ScaleBiggest, ScaleBiggest), ScaleBiggestDuration)
                        .SetEase(Ease.InQuad);
                    tween1.SetUpdate(true);
                    await UniTask.Delay((int)(1000 * ScaleBiggestDuration));
                }
                //还原
                else if (type == 0)
                {
                    var tween3 = rec.DOScale(new float3(ScaleStart, ScaleStart, ScaleStart), ScaleSmallestDuration)
                        .SetEase(Ease.InQuad);
                    tween3.SetUpdate(true);
                    await UniTask.Delay((int)(1000 * ScaleSmallestDuration));
                }


                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                // 清理 DOTween 动画
                rec?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        async UniTaskVoid SetPlayerPos(int gridIndex)
        {
            var KPlayer = GetFromReference(UIPanel_Activity_Monopoly.KPlayer);
            var playerRect = KPlayer.GetRectTransform();
            var monopolyCell = tbmonopoly_cell.Get(gridIndex);
            var curRect = cellRectList[monopolyCell.number - 1];
            var firstRect = cellRectList[0];
            playerOffset = playerRect.AnchoredPosition() - firstRect.AnchoredPosition();

            playerRect.SetAnchoredPosition(curRect.AnchoredPosition() + playerOffset);
            await UniTask.Delay(500);
            EnableGridAnim(true, curRect);
            DoScaleAnim(curRect, default, 2).Forget();
        }

        async UniTask UpdateGrid(int gridIndex, bool enableAnim = false)
        {
            var KCellList = GetFromReference(UIPanel_Activity_Monopoly.KCellList);
            var monopolyCell = tbmonopoly_cell.Get(gridIndex);
            var lastMonopolyCell = tbmonopoly_cell.GetOrDefault(lastGridIndex);
            lastGridIndex = gridIndex;

            if (lastMonopolyCell != null && lastMonopolyCell.group == monopolyCell.group)
            {
                return;
            }

            var cellList = tbmonopoly_cell.DataList.Where(a => a.group == monopolyCell.group)
                .OrderBy(item => item.number).ToList();
            var cellListRect = KCellList.GameObject.GetComponent<RectTransform>();

            var gridLayoutGroup = KCellList.GameObject.GetComponent<GridLayoutGroup>();
            var cellWidth = gridLayoutGroup.cellSize.x;
            var list = KCellList.GetList();

            list.Clear();
            //await UniTask.Delay(200);
            for (int i = 0; i < cellRectList.Length; i++)
            {
                var index = i;
                var cellRect = cellRectList[index];

                var cell = cellList[index];
                if (cell.monopolyEvent > 0)
                {
                    continue;
                }

                var ui = await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, cell.reward[0], false);
                ui.GameObject.transform.SetParent(cellRect);
                ui.SetActive(false);
                if (enableAnim)
                {
                    EnableGridAnim(true, cellRect, "Anim_Dust");
                    await UniTask.Delay((int)(DustAnimDuration * 1000));
                }

                JiYuUIHelper.SetRewardOnClick(cell.reward[0], ui,GetFromReference(KBtn_Mask));
                ui.SetActive(true);
                var KBg_Item = ui.GetFromReference(UICommon_RewardItem.KBg_Item);
                KBg_Item.SetActive(false);
                JiYuTweenHelper.SetEaseAlphaAndScale(ui.GetFromReference(UICommon_RewardItem.KBtn_Item),
                    cancellationToken: cts.Token);
                ui.GetRectTransform().SetHeight(156);
                ui.GetRectTransform().SetWidth(156);
                ui.GetRectTransform().SetScale2(0.8f);
                ui.GetRectTransform().SetAnchoredPosition(Vector2.zero);
                ui.GetRectTransform().SetAnchoredPositionY(7.5f);
                var reward = cell.reward[0];

                cellRect.gameObject.GetComponent<Image>().SetSprite(
                    ResourcesManager.LoadAsset<Sprite>("monopoly_" + JiYuUIHelper.GetRewardQuality(reward).ToString()),
                    false);
            }

            if (enableAnim)
            {
                EnableGridAnim(false, null, "Anim_Dust");
            }

            JiYuUIHelper.ForceRefreshLayout(KCellList);
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            //timerId = timerMgr.StartRepeatedTimer(updateInternal, this.Update);
            timerId = timerMgr.StartRepeatedTimer(1000, this.Update);
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

       
        private void Update()
        {
            var KCommon_CloseInfo = GetFromReference(UIPanel_Activity_Monopoly.KCommon_CloseInfo);
            KCommon_CloseInfo.GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_close").current);

            KCommon_CloseInfo.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KCommon_CloseInfo.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });

            var KText_Time = GetFromReference(UIPanel_Activity_Monopoly.KText_Time);
            string cdStr = tblanguage.Get("active_countdown_text").current;
            long clientT = JiYuUIHelper.GetServerTimeStamp(true);
            if (!JiYuUIHelper.TryGetRemainingTime(clientT, endTime, out var timeStr))
            {
                Close();
                return;
            }

            cdStr += timeStr;

            KText_Time.GetTextMeshPro().SetTMPText(cdStr);
        }


        async public void OnQueryMonopolyTaskResponse(object sender, WebMessageHandler.Execute e)
        {
            if (!isInit)
            {
                isInit = true;
                ByteValueList taskList = new ByteValueList();

                taskList.MergeFrom(e.data);

                if (e.data.IsEmpty)
                {
                    Log.Debug("e.data.IsEmpty", Color.red);
                    return;
                }

                //23:活动类型
                // if (!JiYuUIHelper.TryGetActivityLink(23, out var activityId, out var link))
                // {
                //     return;
                // }

                ResourcesSingleton.Instance.activity.activityTaskDic.TryRemove(activityId, out var list);

                var tasks = new List<GameTaskInfo>();
                Log.Debug($"gameTaskInfo", Color.green);
                foreach (var taskBytes in taskList.Values)
                {
                    GameTaskInfo gameTaskInfo = new GameTaskInfo();
                    gameTaskInfo.MergeFrom(taskBytes);
                    tasks.Add(gameTaskInfo);

                    Log.Debug($"{gameTaskInfo.ToString()}", Color.green);
                }

                // var tbtask = ConfigManager.Instance.Tables.Tbtask;
                // var tbtask_group = ConfigManager.Instance.Tables.Tbtask_group;
                // var tbtask_type = ConfigManager.Instance.Tables.Tbtask_type;
                // var tbmonopoly = ConfigManager.Instance.Tables.Tbmonopoly;


                int redDotNum = 0;

                //var m_RedDotName = NodeNames.GetTagFuncRedDotName(tbmonopoly.Get(link).tagFunc);

                var taskListStr =
                    $"{m_RedDotName}|Pos1";

                foreach (var gameTask in tasks)
                {
                    //RedDotManager.Instance.ClearChildrenListeners(pos1);
                    var task = tbtask.Get(gameTask.Id);
                    var task_group = tbtask_group.Get(task.group);
                    var task_type = tbtask_type.Get(task.type);
                    if (gameTask.Para >= task.para[0] && gameTask.Status == 0)
                    {
                        redDotNum++;
                    }
                }

                Log.Debug($"isRedDot{redDotNum}", Color.green);
                RedDotManager.Instance.SetRedPointCnt(taskListStr, redDotNum);

                // var node = RedDotManager.Instance.GetNode(m_RedDotName);
                // node.PrintTree();

                ResourcesSingleton.Instance.activity.activityTaskDic.TryAdd(activityId, tasks);
            }
            else
            {
                ByteValueList taskList = new ByteValueList();
                taskList.MergeFrom(e.data);

                if (e.data.IsEmpty)
                {
                    Log.Debug("e.data.IsEmpty", Color.red);
                    return;
                }


                ResourcesSingleton.Instance.activity.activityTaskDic.TryRemove(activityId, out var list);

                var tasks = new List<GameTaskInfo>();
                foreach (var taskBytes in taskList.Values)
                {
                    GameTaskInfo gameTaskInfo = new GameTaskInfo();
                    gameTaskInfo.MergeFrom(taskBytes);
                    tasks.Add(gameTaskInfo);

                    Log.Debug($"{gameTaskInfo.ToString()}", Color.green);
                }

                ResourcesSingleton.Instance.activity.activityTaskDic.TryAdd(activityId, tasks);
                var pos = 0f;
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_MonopolyTaskShop, out var ui))
                {
                    var KScrollView = ui.GetFromReference(UIPanel_MonopolyTaskShop.KScrollView);
                    var loopRect = KScrollView.GetLoopScrollRect<UISubPanel_MonopolyTaskShopItem>();
                    pos = loopRect.Get().verticalNormalizedPosition;
                    UIHelper.Remove(UIType.UIPanel_MonopolyTaskShop);
                }

                UIHelper.CreateAsync(UIType.UIPanel_MonopolyTaskShop, activityId, pos);
            }
        }

        async public void OnRollDiceResponse(object sender, WebMessageHandler.Execute e)
        {
            MonopolyActionInfo monopolyAction = new MonopolyActionInfo();
            monopolyAction.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            // var KMonoPoly_Touzi = GetFromReference(UIPanel_Activity_Monopoly.KMonoPoly_Touzi);
            //
            // while (KMonoPoly_Touzi.GameObject.activeSelf)
            // {
            //     await UniTask.Delay(200);
            // }

            if (!ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                    out var gameActivity))
            {
                Log.Error("没有大富翁的活动数据", Color.red);
                return;
            }

            Log.Debug(
                $"DiceNum{monopolyAction.DiceNum} GridIndex{monopolyAction.GridIndex} EventId{monopolyAction.EventId} TokenWeight{monopolyAction.TokenWeight} ",
                Color.green);
            if (!JiYuUIHelper.TryReduceReward(diceReward))
            {
                return;
            }

            var KMonoPoly_Touzi = GetFromReference(UIPanel_Activity_Monopoly.KMonoPoly_Touzi);
            KMonoPoly_Touzi.SetActive(true);
            KMonoPoly_Touzi.GetComponent<Animator>().speed = DiceRollSpeed;
            KMonoPoly_Touzi.GetComponent<Animator>().SetInteger("diceNum", monopolyAction.DiceNum);

            await UniTask.Delay((int)(1.5f * 1000));
            KMonoPoly_Touzi.SetActive(false);

            Refresh();
            gameActivity.MonopolyRecord.MonopolyGridNum = monopolyAction.GridIndex;
            var rewards = JiYuUIHelper.TurnStrReward2List(monopolyAction.ResourceList);
            var KText_Lotto = GetFromReference(UIPanel_Activity_Monopoly.KText_Lotto);
            var KImg_Lotto = GetFromReference(UIPanel_Activity_Monopoly.KImg_Lotto);
            var KBtn_Mask = GetFromReference(UIPanel_Activity_Monopoly.KBtn_Mask);

            // monopolyAction.EventId = 201;
            // gameActivity.MonopolyRecord.MonopolyGridNum = 119;
            //var lottoNum = 0;
            bool showAnim = true;
            switch (monopolyAction.EventId)
            {
                case 201:
                    await PlayerMoveAnim(lastGridIndex, gameActivity.MonopolyRecord.MonopolyGridNum);
                    showAnim = false;
                    KBtn_Mask.SetActive(true);
                    KImg_Lotto.SetActive(true);
                    var lotto = tbmonopoly_event_lotto.Get(monopolyAction.LotteryId);
                    KText_Lotto.GetTextMeshPro().SetTMPText("");
                    KBtn_Mask.GetButton().OnClick.Add(async () =>
                    {
                        if (KImg_Lotto.GameObject.activeSelf)
                        {
                            string str = $"";
                            if (lottoNum == 3)
                            {
                                str = $"{lotto.base0}";
                                lottoNum--;
                            }
                            else if (lottoNum == 2)
                            {
                                str = $"{lotto.base0} * {lotto.multiplier}";
                                lottoNum--;
                            }
                            else if (lottoNum == 1)
                            {
                                str = $"{lotto.base0} * {lotto.multiplier} = {lotto.base0 * lotto.multiplier}";
                                lottoNum--;
                            }
                            else if (lottoNum == 0)
                            {
                                lottoNum = 3;
                                KImg_Lotto.SetActive(false);
                                KBtn_Mask.GetButton().OnClick.RemoveAllListeners();
                                KBtn_Mask.SetActive(false);
                                if (rewards.Count > 0)
                                {
                                    var ui = await UIHelper.CreateAsync(UIType.UICommon_Reward, rewards);
                                    var KBtn_Close = ui.GetFromReference(UICommon_Reward.KBtn_Close);
                                    var KBg_Img = ui.GetFromReference(UICommon_Reward.KBg_Img);
                                    KBtn_Close.GetButton().OnClick.Add(() => { Refresh(); });
                                    KBg_Img.GetButton().OnClick.Add(() => { Refresh(); });
                                    return;
                                }
                            }

                            KText_Lotto.GetTextMeshPro().SetTMPText(str);
                        }
                    });
                    //showAnim = false;
                    // KText_Lotto.GetTextMeshPro().SetTMPText("");
                    // await UniTask.Delay(1000);
                    // string str = $"{lotto.base0}";
                    // KText_Lotto.GetTextMeshPro().SetTMPText(str);
                    // await UniTask.Delay(1000);
                    // str = $"{lotto.base0} * {lotto.multiplier}";
                    // KText_Lotto.GetTextMeshPro().SetTMPText(str);
                    // await UniTask.Delay(1000);
                    // str = $"{lotto.base0} * {lotto.multiplier} = {lotto.base0 * lotto.multiplier}";
                    // KText_Lotto.GetTextMeshPro().SetTMPText(str);
                    // await UniTask.Delay(1000);
                    //
                    // KImg_Lotto.SetActive(false);
                    // KBtn_Mask.SetActive(false);
                    break;
                case 301:
                    var group = tbmonopoly_cell.Get(gameActivity.MonopolyRecord.MonopolyGridNum).group;

                    var gridIndex = tbmonopoly_cell.DataList.Where((a) => a.group == group && a.monopolyEvent == 301)
                        .ToList()[0].id;
                    await PlayerMoveAnim(lastGridIndex, gridIndex);

                    await PlayerMoveAnim(gridIndex, gameActivity.MonopolyRecord.MonopolyGridNum, 1);

                    break;
                default:

                    await PlayerMoveAnim(lastGridIndex, gameActivity.MonopolyRecord.MonopolyGridNum);

                    break;
            }

            lastGridIndex = gameActivity.MonopolyRecord.MonopolyGridNum;
            var KText_EnergySum = GetFromReference(UIPanel_Activity_Monopoly.KText_EnergySum);
            string rewardStr = $"x{monopolyAction.TokenWeight}";
            KText_EnergySum.GetTextMeshPro().SetTMPText(rewardStr);

            Log.Error($"完成动画 资源是{rewards.Count}", Color.red);
            if (rewards.Count > 0 && showAnim)
            {
                var ui = await UIHelper.CreateAsync(UIType.UICommon_Reward, rewards);
                var KBtn_Close = ui.GetFromReference(UICommon_Reward.KBtn_Close);
                var KBg_Img = ui.GetFromReference(UICommon_Reward.KBg_Img);
                KBtn_Close.GetButton().OnClick.Add(async () =>
                {
                    await UniTask.Delay(1200);
                    Refresh();
                });
                KBg_Img.GetButton().OnClick.Add(async() =>
                {
                    await UniTask.Delay(1200);
                    Refresh();
                });
            }
        }


        protected override void OnClose()
        {
            JiYuUIHelper.DestoryAllTips();
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERTSINGLEACTIVITY, OnMonopolyResponse);
            WebMessageHandler.Instance.RemoveHandler(CMD.MONOPOLYACTION, OnRollDiceResponse);
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERYACTIVITYTASK, OnQueryMonopolyTaskResponse);
            RedDotManager.Instance.ClearChildrenListeners(m_RedDotName);
            RemoveTimer();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}