//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Spine.Unity;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;
using Unity.Entities.UniversalDelegates;
using UnityEngine.SceneManagement;


namespace XFramework
{
    [UIEvent(UIType.UIPanel_Challege)]
    internal sealed class UIPanel_ChallegeEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Challege;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Challege>();
        }
    }

    public partial class UIPanel_Challege : UI, IAwake
    {
        private cfg.config.Tbtag_func tbTagFunc;
        private cfg.config.Tblanguage tbLanguage;
        private cfg.config.Tbchallenge tbChallenge;
        private cfg.config.Tbenvironment tbenvironment;
        private cfg.config.Tbscene tbscene;
        private cfg.config.Tblevel tblevel;
        private cfg.config.Tbmonster tbmonster;
        private cfg.config.Tbmonster_attr tbmonster_attr;
        private cfg.config.Tbmonster_model tbmonster_model;
        private cfg.config.Tbevent_0 tbevent_0;
        private cfg.config.Tbanecdote tbanecdote;
        private cfg.config.Tbblock tbblock;

        private int currentTag;

        private int currentSelectID;

        /// <summary>
        /// 当前显示的下三角
        /// </summary>
        private UI currentPlygon;

        private UIPanel_Challege_AreaInfo tipParentUI;

        private float tipStartPos;
        public int tagId = 4;

        /// <summary>
        /// 轶事描述的上下间距
        /// </summary>
        private float itemGap = 40f;

        public void Initialize()
        {
            tipParentUI = default;
            var maxChapterID = ResourcesSingleton.Instance.levelInfo.maxPassChapterID;
            if (maxChapterID < 2)
            {
                Log.Error($"没通关前两张{maxChapterID}");
                this.Close();
                return;
            }

            this.GetFromReference(KMain_RedDot)?.SetActive(false);
            this.GetFromReference(KArea_RedDot)?.SetActive(false);

            var redDotString1 = NodeNames.GetTagFuncRedDotName(4001);
            var redDotString2 = NodeNames.GetTagFuncRedDotName(4002);
            RedDotManager.Instance.AddListener(redDotString1, (num) =>
            {
                Log.Debug($"reddotnum11111111:{num}");
                this.GetFromReference(KMain_RedDot)?.SetActive(num > 0);
            });
            RedDotManager.Instance.AddListener(redDotString2, (num) =>
            {
                Log.Debug($"reddotnum222222:{num}");
                this.GetFromReference(KArea_RedDot)?.SetActive(num > 0);
            });


            RefreshMaxLockID();
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_MainTread), OnMainTreadClick);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_AreaThread), OnAreaTreadClick);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Description), OnTipTagClick);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Display_Root),
                () => OnChallengeButtonClick(currentSelectID, 1));
            this.GetFromReference(KBtn_Close).GetButton()?.OnClick.Add(OnTipClose);

            InitAllNeedHide();
            InitTable();

            currentTag = 4001;

            SetThreadTag(currentTag);
            //GetFromReference(KIcon_MainTread)?.GetComponent<Image>().SetSprite(ResourcesManager.LoadAsset<Sprite>(tbTagFunc.GetOrDefault(4001).icon), false);
            //GetFromReference(KIcon_AreaTread)?.GetComponent<Image>().SetSprite(ResourcesManager.LoadAsset<Sprite>(tbTagFunc.GetOrDefault(4002).icon), false);
            CreateMainTreadAreaInfo().Forget();


            int currentMainID = GetCurrentMainThreadID();
            int currentAreaID = GetCurrentAreaThreadID();

            UpdateFromCurrentMainID(currentMainID).Forget();

            CreateAreaTreadAreaInfo(currentAreaID);
            GetFromReference(KMainTreadRoot)?.SetActive(true);
            GetFromReference(KContainer_Switch)?.SetActive(true);
            GetFromReference(KMainTreadRoot)?.SetActive(true);
            currentPlygon = default;
            this.GetXButton(KBtn_Goto)?.OnClick.Add(OnTipGoto);

            GetFromReference(KInfoTip)?.SetActive(false);
            tipStartPos = (float)GetFromReference(KContainer_AreaInfo)?.GetRectTransform().AnchoredPosition().x;
            GetFromReference(KScroller_AreaInfo)?.GetScrollRect().OnValueChanged.Add((v) => DragToHidTip());

            //GetFromReference(KScroller_AreaInfo)?.GetXScrollRect().OnDrag.Add((v)
            //    => OnTipClose());
            //GetFromReference(KContainer_Info_Area)?.GetXScrollRect().OnDrag.Add((v)
            //    => OnTipClose());

            //this.GetTextMeshPro(KTxt_InfoTip)?.SetTMPText(tbLanguage.GetOrDefault().current);
        }

        public void DragToHidTip()
        {
            //Debug.LogError("TryGetUI");
            if (currentPlygon == default)
            {
                return;
            }

            //var scrollRect = GetFromReference(KScroller_AreaInfo)?.GetComponent<JiYuScrollRect>();
            float leftBorder = -Screen.width / 2f;
            float rightBorder = Screen.width / 2f;
            var currentX = JiYuUIHelper.GetUIPos(currentPlygon).x;
            GetFromReference(KInfoTip)?.SetActive(false);
            if (currentX < leftBorder || currentX > rightBorder)
            {
                GetFromReference(KInfoTip)?.SetActive(true);
            }

            //scrollRect.SetOnDragAction(() =>
            //{
            //    //GetFromReference(KTip_Polygon)?.SetActive(false);
            //});
        }


        private void OnTipGoto()
        {
            Debug.Log("goto");
            var scroll = GetFromReference(KScroller_AreaInfo)?.GetComponent<ScrollRect>();
            scroll.velocity = Vector2.zero;
            float needPos = GetFromReference(KContainer_AreaInfo).GetComponent<HorizontalLayoutGroup>().spacing + 280;
            needPos *= currentSelectID - 2000 - 1;
            GetFromReference(KContainer_AreaInfo)?.GetRectTransform().SetAnchoredPositionX(tipStartPos - needPos);
            GetFromReference(KInfoTip)?.SetActive(false);
        }

        public async void OnLoopClick()
        {
            int maxChallengeID = 0;
            for (int i = 0; i < tbTagFunc.DataList.Count; i++)
            {
                if (tbTagFunc.DataList[i].tagId == 4)
                {
                    //maxPosID = tbTagFunc.DataList[i].posType > maxPosID ? tbTagFunc.DataList[i].posType:maxPosID;
                    maxChallengeID = tbTagFunc.DataList[i].id > maxChallengeID
                        ? tbTagFunc.DataList[i].id
                        : maxChallengeID;
                    break;
                }
            }

            int id = currentTag + 1;
            if (id > maxChallengeID)
            {
                id = currentTag;
            }

            var posID = id > maxChallengeID ? 1 : 2;
            OnBtnClickEvent(posID);
        }

        public void OnBtnClickEvent(int posID)
        {
            if (posID == 1)
            {
                OnMainTreadClick();
            }
            else if (posID == 2)
            {
                OnAreaTreadClick();
            }
        }

        private static void RefreshMaxLockID()
        {
            //var dic = ResourcesSingleton.Instance.challengeInfo.challengeStateMap;
            //if (dic.Count <= 0) return;
            //int maxMainChallengID = dic.Where(x => x.Value != 1 && x.Key.ToString().StartsWith("2")).Max(x => x.Key);
            //int maxAreaChallengID = dic.Where(x => x.Value != 1 && x.Key.ToString().StartsWith("3")).Max(x => x.Key);
            //ResourcesSingleton.Instance.challengeInfo.maxMainChallengeID = maxMainChallengID;
            //ResourcesSingleton.Instance.challengeInfo.maxAreaChallengeID = maxAreaChallengID;
        }

        private void OnTipClose()
        {
            Log.Debug("OnTipClose");
            this.GetFromReference(KTip_Descript)?.SetActive(false);
            //this.GetFromReference(KBtn_Close)?.SetActive(false);
            this.GetFromReference(KCommon_ItemTips)?.SetActive(false);
            //this.GetFromReference(Ktip_Txt_Bg)?.SetActive(false);
            JiYuUIHelper.DestoryAllTips();


            this.GetFromReference(KImgArrow)?.SetActive(false);
        }

        private int GetCurrentAreaThreadID()
        {
            int id = ResourcesSingleton.Instance.challengeInfo.maxAreaChallengeID;
            return id;
        }

        private int GetCurrentMainThreadID()
        {
            int id = ResourcesSingleton.Instance.challengeInfo.maxMainChallengeID;
            return id;
        }

        private async void CreateAreaTreadAreaInfo(int currentAreaID)
        {
            int maxChallengeID = ResourcesSingleton.Instance.challengeInfo.maxAreaChallengeID;
            var scrollRect = this.GetFromReference(KContainer_Info_Area).GetXScrollRect();
            scrollRect.OnBeginDrag.Add(JiYuUIHelper.DestoryAllTips);
            var areaInfoList = scrollRect.Content.GetList();

            var challengeTable = tbChallenge.DataList;
            areaInfoList.Clear();
            for (int i = 0; i < challengeTable.Count; i++)
            {
                if (challengeTable[i].type == 3)
                {
                    var challengeID = challengeTable[i].id;
                    var ui = await areaInfoList.CreateWithUITypeAsync<int>(
                            UIType.UISubPanel_Challenge_Container_AreaDetails, challengeTable[i].id, false)
                        as UISubPanel_Challenge_Container_AreaDetails;
                    ui.index = i;

                    //ui.GetImage(UISubPanel_Challenge_Container_AreaDetails.KContainer_Area_Map)
                    //    ?.SetSprite(tbChallenge.Get(challengeTable[i].id).bg, false);
                    SetAreaAnimation(i, ui);

                    var titileName = challengeTable[i].blockId + "." +
                                     tbLanguage.Get(tbblock.Get(challengeTable[i].blockId).name).current;

                    ui.GetTextMeshPro(UISubPanel_Challenge_Container_AreaDetails.KTxt_AreaName)?.SetTMPText(titileName);
                    var anecdotes = tbChallenge.Get(challengeTable[i].id).anecdoteGroup;
                    CreateEventDes(anecdotes, 2, ui);

                    ui.GetTextMeshPro(UISubPanel_Challenge_Container_AreaDetails.KTxt_FinishTag)
                        ?.SetTMPText(tbLanguage.Get("text_successed").current);

                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KContainerFinish)?.SetActive(false);
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KImg_RedDot)?.SetActive(false);

                    var redString = NodeNames.GetTagFuncRedDotName(4002) + '|' + challengeID.ToString();
                    RedDotManager.Instance.InsterNode(redString);
                    RedDotManager.Instance.SetRedPointCnt(redString, 0);

                    if (challengeTable[i].id <= maxChallengeID)
                    {
                        if (ResourcesSingleton.Instance.challengeInfo.challengeStateMap[challengeID] > 2)
                        {
                            ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KContainerFinish)
                                ?.SetActive(true);
                        }

                        if (ResourcesSingleton.Instance.challengeInfo.challengeStateMap[challengeID] == 3)
                        {
                            ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KImg_RedDot)
                                ?.SetActive(true);
                            RedDotManager.Instance.SetRedPointCnt(redString, 1);
                        }
                    }

                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(
                        ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KBtn_Display_Root),
                        () => OnChallengeButtonClick(challengeID, 2), 4001);
                    SetFromCurrentAreaID(ui, challengeTable[i].id).Forget();


                    var mapName = challengeTable[i].blockId.ToString() + "-" + challengeTable[i].num + " " +
                                  challengeTable[i].name;
                    //ui.GetTextMeshPro(UIPanel_Challege_AreaInfo.KTxt_MapName)?.SetTMPText(mapName);
                    ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Mask)?.SetActive(true);
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KImg_Lock)?.SetActive(true);
                    if (challengeTable[i].id <= maxChallengeID)
                    {
                        ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Lock)?.SetActive(false);
                        ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Mask)?.SetActive(false);
                    }


                    // JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(ui.GetFromReference(UIPanel_Challege_AreaInfo.KBtn_AreaInfo), () => OnMainChallengeAreaClick(ui, challengeTable[i].id));
                }
            }

            areaInfoList.Sort((a, b) =>
            {
                var uia = a as UISubPanel_Challenge_Container_AreaDetails;
                var uib = b as UISubPanel_Challenge_Container_AreaDetails;
                return uia.index.CompareTo(uib.index);
            });

            SetAndoteHeight();
            //JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KContainer_Info_Area).GetScrollRect().Content);
        }

        private void SetAndoteHeight()
        {
            float height = LayoutUtility.GetPreferredHeight(GetFromReference(KDescription_Container).GameObject
                .GetComponent<RectTransform>());
            Log.Debug($"height:{height}", Color.cyan);
            //GetFromReference(KDescription_Container).GetRectTransform().SetOffsetWithLeft(itemGap);
            //GetFromReference(KDescription_Container).GetRectTransform().SetOffsetWithRight(itemGap);
            GetFromReference(KDescription_Container).GetRectTransform().SetHeight(height);
            var containerH = height + 267f;
            var offset = (containerH - 348) / 2f;
            GetFromReference(KContainer).GetRectTransform().SetHeight(containerH);
            GetFromReference(KContainer).GetRectTransform().SetAnchoredPositionY(325f + offset);
        }

        private void SetAreaAnimation(int i, UISubPanel_Challenge_Container_AreaDetails ui)
        {
            var challengeTable = tbChallenge.DataList;

            var bossIds = FilterBossIDs(challengeTable[i].id);
            if (bossIds.Count <= 0) return;

            var bookId = tbmonster_attr.Get(tbmonster.Get(bossIds[2]).monsterAttrId).bookId;
            var monsterPic = tbmonster_model.Get(bookId).thumbPic;
            ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KContainer_Area_Boss)
                .GetImage().SetSprite(monsterPic, true);
        }

        public async UniTask<int> UpdateFromCurrentMainID(int currentID)
        {
            currentSelectID = currentID;


            this.GetImage(KImg_Challege_BG)?.SetSprite(tbChallenge.Get(currentID).pic, false);
            SetBossModleAndAni(currentID);
            //for (int i = 1; i <= 3; i++)
            //{
            //   
            //}

            //this.GetFromReference(KBtn_Boss3)?.GetRectTransform()
            //        .SetSize(new Vector2(bossSkeleton.rectTransform.Width(), bossSkeleton.rectTransform.Height()));
            //this.GetButton(KBtn_Boss3)?.OnClick.Add(() => OnBossTipClick(bossIds[bossIndex - 1]));
            var anecdotes = tbChallenge.Get(currentID).anecdoteGroup;
            Log.Debug($"evnts{anecdotes.Count}", Color.cyan);
            CreateEventDes(anecdotes, 1, this);

            var rewards = await CreateReward(currentID, 1, this);

            int awardState = ResourcesSingleton.Instance.challengeInfo.challengeStateMap[currentID];

            //awardState = 1;
            DisplayRewardGetInfoForMain(awardState, rewards);
            return awardState;
        }


        private async UniTaskVoid SetFromCurrentAreaID(UISubPanel_Challenge_Container_AreaDetails ui, int currentID)
        {
            var rewards = await CreateReward(currentID, 2, ui);

            int awardState = ResourcesSingleton.Instance.challengeInfo.challengeStateMap[currentID];

            DisplayRewardGetInfoForArea(ui, awardState, rewards, currentID);
        }

        private void DisplayRewardGetInfoForArea(UISubPanel_Challenge_Container_AreaDetails ui, int awardState,
            List<UICommon_RewardItem> rewards, int currentID)
        {
            switch (awardState)
            {
                //未解锁
                case 1:
                    var str = string.Format(tbLanguage.Get("challenge_3_unlock_text").current,
                        tbChallenge.Get(currentID).blockId, tbChallenge.Get(currentID).num,
                        tbLanguage.Get(tbChallenge.Get(currentID).name).current);
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KBtn_Display_Root)?.SetActive(false);
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KTxt_Display_Root)?.SetActive(true);
                    ui.GetTextMeshPro(UISubPanel_Challenge_Container_AreaDetails.KTxt_Display)
                        ?.SetTMPText(str);
                    break;
                //已解锁可挑战
                case 2:
                    Debug.LogError("已解锁可挑战");
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KBtn_Display_Root)?.SetActive(true);
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KTxt_Display_Root)?.SetActive(false);
                    ui.GetTextMeshPro(UISubPanel_Challenge_Container_AreaDetails.KTxt_Btn_Display)
                        ?.SetTMPText(tbLanguage.Get("challenge_state_rush").current);
                    ui.GetImage(UISubPanel_Challenge_Container_AreaDetails.KBtn_Display_Root)
                        ?.SetSprite("common_yellow_button_5", false);
                    break;
                case 3:
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KBtn_Display_Root)?.SetActive(true);
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KTxt_Display_Root)?.SetActive(false);
                    ui.GetTextMeshPro(UISubPanel_Challenge_Container_AreaDetails.KTxt_Btn_Display)
                        ?.SetTMPText(tbLanguage.Get("common_state_gain").current);
                    ui.GetImage(UISubPanel_Challenge_Container_AreaDetails.KBtn_Display_Root)
                        ?.SetSprite("common_yellow_button_5", false);
                    foreach (var reward in rewards)
                    {
                        reward.GetImage(UICommon_RewardItem.KBg_Item)?.SetSprite("Rectangle 14781", false);
                    }

                    break;
                case 4:
                    ui.GetImage(UISubPanel_Challenge_Container_AreaDetails.KBtn_Display_Root)
                       ?.SetSprite("common_blue_button_8", false);
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KBtn_Display_Root)?.SetActive(true);
                    ui.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KTxt_Display_Root)?.SetActive(false);
                    ui.GetTextMeshPro(UISubPanel_Challenge_Container_AreaDetails.KTxt_Btn_Display)
                        ?.SetTMPText(tbLanguage.Get("challenge_state_rush_again").current);
                    //ui.GetImage(UISubPanel_Challenge_Container_AreaDetails.KBtn_Display_Root)
                    //    ?.SetColor(new Color(34 / 255f, 197 / 255f, 94 / 255f));
                    foreach (var reward in rewards)
                    {
                        reward.GetFromReference(UICommon_RewardItem.KImg_ItemMask)?.SetActive(true);
                        reward.GetTextMeshPro(UICommon_RewardItem.KText_ItemMask)
                            ?.SetTMPText(tbLanguage.Get("common_state_gained").current);
                    }

                    break;
                default: break;
            }
        }


        private async void OnChallengeButtonClick(int challengeID, int type)
        {
            ResourcesSingleton.Instance.levelInfo.levelId = tbChallenge.Get(challengeID).levelId;
            var state = ResourcesSingleton.Instance.challengeInfo.challengeStateMap[challengeID];
            switch (state)
            {
                case 2:
                case 4:
                    var global = Common.Instance.Get<Global>();
                    //PlayerPrefs.DeleteAll();
                    // if (global.isStandAlone)
                    // {
                    //     var sceneController = Common.Instance.Get<SceneController>();
                    //     var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime);
                    //     SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();
                    // }

                    WebMessageHandler.Instance.AddHandler(CMD.QUERYCANSTART, OnClickPlayBtnResponse);
                    var battleGain = new BattleGain
                    {
                        LevelId = ResourcesSingleton.Instance.levelInfo.levelId
                    };
                    NetWorkManager.Instance.SendMessage(CMD.QUERYCANSTART, battleGain);


                    // WebMessageHandler.Instance.AddHandler(2, 3, OnClickPlayBtnBeforeResponse0);
                    // WebMessageHandler.Instance.AddHandler(2, 5, OnClickPlayBtnResponse0);
                    // Log.Debug($"发送请求挑战", Color.green);
                    // NetWorkManager.Instance.SendMessage(2, 3);
                    break;
                case 3:

                    ResourcesSingleton.Instance.challengeInfo.challengeStateMap[currentSelectID] = 4;
                    NetWorkManager.Instance.SendMessage(CMD.CHALLENGECLAIM,
                        new IntValue { Value = ResourcesSingleton.Instance.levelInfo.levelId });
                    var rewards = tbChallenge.Get(currentSelectID).reward;

                    #region test

                    foreach (var reward in rewards)
                    {
                        Log.Debug($"1:{reward}", Color.cyan);
                    }

                    #endregion

                    #region test

                    foreach (var reward in rewards)
                    {
                        Log.Debug($"2:{reward}", Color.cyan);
                    }

                    #endregion

                    //UIHelper.CreateAsync(UIType.UICommon_Reward, rewards).Forget();
                    JiYuUIHelper.MergeRewardList(rewards);
                    JiYuUIHelper.SortRewards(rewards);
                    JiYuUIHelper.AddReward(rewards, true);
                    if (type == 1)
                    {
                        UpdateFromCurrentMainID(currentSelectID);


                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Talent, out UI ui))
                        {
                            Log.Debug("TryGetUI(UIType.UIPanel_Talent,out UI ui)", Color.cyan);
                            var talent = ui as UIPanel_Talent;
                            talent.UpdateResource();
                            talent.SetTalentSkill();
                        }
                    }
                    else if (type == 2)
                    {
                        CreateAreaTreadAreaInfo(currentSelectID);
                    }


                    break;
                default: break;
            }
        }

        void OnClickPlayBtnResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.QUERYCANSTART, OnClickPlayBtnResponse);
            var longValue = new LongValue();
            longValue.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                //return;
            }

            Log.Debug($"验证对局是否可以开始:{longValue.Value}", Color.green);
            if (longValue.Value != null && longValue.Value > 0)
            {
                ResourcesSingleton.Instance.battleData.battleId = longValue.Value;
                //this.GetParent<UIPanel_JiyuGame>().DestoryAllToggle();
                // entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                // switchSceneQuery = entityManager.CreateEntityQuery(typeof(SwitchSceneData));
                //
                // Log.Debug($"switch:{switchSceneQuery.CalculateEntityCount()}");

                //this.Close();
                //Close();
                var sceneController = Common.Instance.Get<SceneController>();
                var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime, LoadSceneMode.Additive);
                SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();
                JsonManager.Instance.userData.tagId = 4;
                JsonManager.Instance.SavePlayerData(JsonManager.Instance.userData);

                // var switchSceneData = switchSceneQuery.ToComponentDataArray<SwitchSceneData>(Allocator.Temp)[0];
                //  switchSceneData.mainScene.LoadAsync(new ContentSceneParameters()
                //  {
                //      loadSceneMode = LoadSceneMode.Single
                //  });
            }
            else
            {
                Log.Debug($"对局不可以开始{longValue.Value}", Color.green);
            }
        }
        // void OnClickPlayBtnBeforeResponse0(object sender, WebMessageHandler.Execute e)
        // {
        //     WebMessageHandler.Instance.RemoveHandler(2, 3, OnClickPlayBtnBeforeResponse0);
        //     var longValue = new LongValue();
        //     longValue.MergeFrom(e.data);
        //
        //
        //     if (e.data.IsEmpty)
        //     {
        //         Log.Debug("e.data.IsEmpty", Color.red);
        //         return;
        //     }
        //
        //     Log.Debug($"收到后端战局ID:{longValue.Value}", Color.green);
        //     var battleGain = new BattleGain
        //     {
        //         BattleId = longValue.Value,
        //         LevelId = ResourcesSingleton.Instance.levelInfo.levelId,
        //         Type = 0,
        //         RoleId = 0,
        //         PassStatus = "",
        //         LiveTime = 0,
        //         KillMobs = 0,
        //         KillElite = 0,
        //         KillBoss = 0
        //     };
        //
        //     //Log.Debug($"战局ID:{longValue.Value}", Color.green);
        //     NetWorkManager.Instance.SendMessage(2, 5, battleGain);
        // }
        //
        // void OnClickPlayBtnResponse0(object sender, WebMessageHandler.Execute e)
        // {
        //     WebMessageHandler.Instance.RemoveHandler(2, 5, OnClickPlayBtnResponse0);
        //     var longValue = new BoolValue();
        //     longValue.MergeFrom(e.data);
        //     if (e.data.IsEmpty)
        //     {
        //         Log.Debug("e.data.IsEmpty", Color.red);
        //         return;
        //     }
        //
        //     Log.Debug($"验证对局是否可以开始:{longValue.Value}", Color.green);
        //     if (longValue.Value)
        //     {
        //         //this.GetParent<UIPanel_JiyuGame>().DestoryAllToggle();
        //         var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //         var switchSceneQuery = entityManager.CreateEntityQuery(typeof(SwitchSceneData));
        //
        //         Log.Debug($"switch:{switchSceneQuery.CalculateEntityCount()}");
        //
        //         //this.Close();
        //         var sceneController = Common.Instance.Get<SceneController>();
        //         var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime);
        //         SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();
        //
        //         //TODO:ʹ��ecs���ݹ�����س���
        //         // var switchSceneData = switchSceneQuery.ToComponentDataArray<SwitchSceneData>(Allocator.Temp)[0];
        //         //  switchSceneData.mainScene.LoadAsync(new ContentSceneParameters()
        //         //  {
        //         //      loadSceneMode = LoadSceneMode.Single
        //         //  });
        //     }
        //     else
        //     {
        //         //TODO:ͨ����ʾ��
        //         Log.Debug("longValue is false,maybe resources are not enough", Color.red);
        //     }
        // }

        private void InitTable()
        {
            tbTagFunc = ConfigManager.Instance.Tables.Tbtag_func;
            tbLanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbChallenge = ConfigManager.Instance.Tables.Tbchallenge;
            tbenvironment = ConfigManager.Instance.Tables.Tbenvironment;
            tbscene = ConfigManager.Instance.Tables.Tbscene;
            tblevel = ConfigManager.Instance.Tables.Tblevel;
            tbmonster = ConfigManager.Instance.Tables.Tbmonster;
            tbmonster_attr = ConfigManager.Instance.Tables.Tbmonster_attr;
            tbevent_0 = ConfigManager.Instance.Tables.Tbevent_0;
            tbblock = ConfigManager.Instance.Tables.Tbblock;
            tbanecdote = ConfigManager.Instance.Tables.Tbanecdote;
            tbmonster_model = ConfigManager.Instance.Tables.Tbmonster_model;
        }

        private async UniTaskVoid CreateMainTreadAreaInfo()
        {
            var maxChallengeID = ResourcesSingleton.Instance.challengeInfo.maxMainChallengeID;

            var areaInfoParent = this.GetFromReference(KContainer_AreaInfo);

            var areaInfoList = areaInfoParent.GetList();
            areaInfoList.Clear();
            var challengeTable = tbChallenge.DataList;
            UIPanel_Challege_AreaInfo uiInit = default;
            for (int i = 0; i < challengeTable.Count; i++)
            {
                if (challengeTable[i].type == 2)
                {
                    var index = i;
                    var ui = await areaInfoList.CreateWithUITypeAsync<int>(UIType.UIPanel_Challege_AreaInfo,
                            challengeTable[i].id, false)
                        as UIPanel_Challege_AreaInfo;
                    ui.index = i;

                    ui.GetFromReference(UIPanel_Challege_AreaInfo.KBtn_AreaInfo)?.GetImage()
                        .SetSpriteAsync("bg_challeng_area", false);
                    //ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Panel_Select)?.SetActive(false);
                    ui.GetTextMeshPro(UIPanel_Challege_AreaInfo.KTxt_AreaName)
                        ?.SetTMPText(tbLanguage.Get(tbblock.Get(challengeTable[i].blockId).name).current);
                    var mapNumber = challengeTable[i].blockId.ToString() + "-" + challengeTable[i].num;
                    ui.GetTextMeshPro(UIPanel_Challege_AreaInfo.KTxt_MapNumber)?.SetTMPText(mapNumber);
                    ui.GetTextMeshPro(UIPanel_Challege_AreaInfo.KTxt_MapName)
                        ?.SetTMPText(tbLanguage.Get(challengeTable[i].name).current);
                    //ui.GetImage(UIPanel_Challege_AreaInfo.KImg_BG)?.SetSprite(challengeTable[i].bg, false);
                    ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Lock)?.SetActive(true);
                    ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Mask)?.SetActive(true);
                    ui.GetFromReference(UIPanel_Challege_AreaInfo.KTip_Polygon)?.SetActive(false);
                    ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_RedDot).SetActive(false);

                    var redString = NodeNames.GetTagFuncRedDotName(4001) + '|' + challengeTable[index].id.ToString();
                    RedDotManager.Instance.InsterNode(redString);
                    RedDotManager.Instance.RemoveAllListeners(redString);
                    RedDotManager.Instance.AddListener(redString,
                        (num) => { ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_RedDot).SetActive(num > 0); });
                    RedDotManager.Instance.SetRedPointCnt(redString, 0);

                    if (challengeTable[i].id <= maxChallengeID)
                    {
                        ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Lock)?.SetActive(false);
                        ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Mask)?.SetActive(false);
                    }

                    int finishState = ResourcesSingleton.Instance.challengeInfo.challengeStateMap[challengeTable[i].id];
                    if (finishState >= 3)
                    {
                        ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Finish).SetActive(true);
                        ui.GetFromReference(UIPanel_Challege_AreaInfo.KTxt_FinishTag).GetTextMeshPro()
                            ?.SetTMPText(tbLanguage.Get("text_successed").current);
                        if (finishState == 3)
                        {
                            RedDotManager.Instance.SetRedPointCnt(redString, 1);
                            ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_RedDot).SetActive(true);
                        }
                    }

                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(
                        ui.GetFromReference(UIPanel_Challege_AreaInfo.KBtn_AreaInfo),
                        () => { OnMainChallengeAreaClick(ui, challengeTable[index].id); });
                    if (index == 0)
                    {
                        uiInit = ui;
                    }
                }
            }

            areaInfoList.Sort((a, b) =>
            {
                var uia = a as UIPanel_Challege_AreaInfo;
                var uib = b as UIPanel_Challege_AreaInfo;
                return uia.index.CompareTo(uib.index);
            });
            OnMainChallengeAreaClick(uiInit, challengeTable[0].id);
            JiYuUIHelper.ForceRefreshLayout(areaInfoParent);
        }

        private async void OnMainChallengeAreaClick(UIPanel_Challege_AreaInfo ui, int id)
        {
            JiYuUIHelper.DestoryAllTips();
            var state = await UpdateFromCurrentMainID(id);
            if (currentPlygon != default)
            {
                currentPlygon.SetActive(false);
            }

            //var areaInfoParent = this.GetFromReference(KContainer_AreaInfo);

            //var areaInfoList = areaInfoParent.GetList();
            //foreach (var areaInfo in areaInfoList.Children)
            //{
            //    areaInfo.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Panel_Select)?.SetActive(false);
            //}

            //Log.Debug($"id:{id}", Color.cyan);
            //ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Panel_Select)?.SetActive(true);


            currentPlygon = ui.GetFromReference(UIPanel_Challege_AreaInfo.KTip_Polygon);
            currentPlygon?.SetActive(false);
            //this.GetTextMeshPro(KTxtInfo_levleID)?.SetTMPText((id - 2000).ToString());
            DragToHidTip();


            if (currentPlygon != default)
            {
                currentPlygon.SetActive(false);
            }

            var areaInfoParent = this.GetFromReference(KContainer_AreaInfo);

            var areaInfoList = areaInfoParent.GetList();
            foreach (var areaInfo in areaInfoList.Children)
            {
                areaInfo.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Select)?.SetActive(false);
            }

            //Log.Debug($"id:{id}", Color.cyan);
            ui.GetFromReference(UIPanel_Challege_AreaInfo.KImg_Select)?.SetActive(true);


            currentPlygon = ui.GetFromReference(UIPanel_Challege_AreaInfo.KTip_Polygon);
            currentPlygon?.SetActive(false);
            this.GetTextMeshPro(KTxtInfo_levleID)?.SetTMPText((id - 2000).ToString());
            //DragToHidTip();
        }


        //public void DragToHidTip()
        //{
        //    //Debug.LogError("TryGetUI");
        //    if (currentPlygon == default)
        //    {
        //        return;
        //    }

        //    //var scrollRect = GetFromReference(KScroller_AreaInfo)?.GetComponent<JiYuScrollRect>();
        //    float leftBorder = -Screen.width / 2f;
        //    float rightBorder = Screen.width / 2f;
        //    var currentX = JiYuUIHelper.GetUIPos(currentPlygon).x;
        //    GetFromReference(KInfoTip)?.SetActive(false);
        //    //GetFromReference(Ktip_Award_Bg)?.SetActive(false);
        //    if (currentX < leftBorder || currentX > rightBorder)
        //    {
        //        GetFromReference(KInfoTip)?.SetActive(true);
        //    }

        //    //scrollRect.SetOnDragAction(() =>
        //    //{
        //    //    //GetFromReference(KTip_Polygon)?.SetActive(false);
        //    //});
        //}

        private void ChangeMainChallengInfo(int id)
        {
        }

        private void DisplayRewardGetInfoForMain(int state, List<UICommon_RewardItem> rewards)
        {
            var redString = NodeNames.GetTagFuncRedDotName(4001) + '|' + currentSelectID.ToString();
            RedDotManager.Instance.SetRedPointCnt(redString, 0);


            switch (state)
            {
                //未解锁
                case 1:
                    GetFromReference(KBtn_Display_Root)?.SetActive(false);
                    GetFromReference(KTxt_Display_Root)?.SetActive(true);
                    this.GetTextMeshPro(KTxt_Display)?.SetTMPText(tbLanguage.Get("challenge_2_unlock_text").current);
                    break;
                //已解锁可挑战
                case 2:
                    GetFromReference(KBtn_Display_Root)?.SetActive(true);
                    GetFromReference(KTxt_Display_Root)?.SetActive(false);
                    this.GetTextMeshPro(KTxt_Btn_Display)?.SetTMPText(tbLanguage.Get("challenge_state_rush").current);
                    //this.GetImage(KBtn_Display_Root)?.SetColor(new Color(51 / 255f, 65 / 255f, 85 / 255f));
                    break;
                case 3:
                    RedDotManager.Instance.SetRedPointCnt(redString, 1);


                    GetFromReference(KBtn_Display_Root)?.SetActive(true);
                    GetFromReference(KTxt_Display_Root)?.SetActive(false);
                    this.GetTextMeshPro(KTxt_Btn_Display)?.SetTMPText(tbLanguage.Get("common_state_gain").current);
                    //this.GetImage(KBtn_Display_Root)?.SetColor(new Color(34 / 255f, 197 / 255f, 94 / 255f));
                    foreach (var reward in rewards)
                    {
                        reward.GetImage(UICommon_RewardItem.KBg_Item)?.SetSprite("Rectangle 14781", false);
                    }

                    break;
                case 4:
                    GetFromReference(KBtn_Display_Root)?.SetActive(true);
                    GetFromReference(KTxt_Display_Root)?.SetActive(false);
                    this.GetTextMeshPro(KTxt_Btn_Display)
                        ?.SetTMPText(tbLanguage.Get("challenge_state_rush_again").current);
                    //this.GetImage(KBtn_Display_Root)?.SetColor(new Color(34 / 255f, 197 / 255f, 94 / 255f));
                    foreach (var reward in rewards)
                    {
                        reward.GetFromReference(UICommon_RewardItem.KImg_ItemMask)?.SetActive(true);
                        reward.GetTextMeshPro(UICommon_RewardItem.KText_ItemMask)
                            ?.SetTMPText(tbLanguage.Get("common_state_gained").current);
                    }

                    break;
                default: break;
            }
        }

        private async void CreateEventDes(List<int> anecdotes, int type, UI uiParent)
        {
            UI descriptionParent = default;
            if (type == 1)
            {
                descriptionParent = this.GetFromReference(KDescription_Container);
            }
            else
            {
                descriptionParent =
                    uiParent.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KContainer_Description);
            }

            var descripList = descriptionParent.GetList();
            descripList.Clear();

            for (int i = 0; i < anecdotes.Count; i++)
            {
                if (tbanecdote.Get(anecdotes[i]).type == 3)
                {
                    continue;
                }

                var ui =
                    await descripList.CreateWithUITypeAsync<int>(UIType.UIPanel_Common_Events, anecdotes[i], false) as
                        UIPanel_Common_Events;

                switch (tbanecdote[anecdotes[i]].type)
                {
                    case 0:
                        ui.sortIndex = -tbanecdote[anecdotes[i]].sort * 100;
                        break;
                    case 2:
                        ui.sortIndex = -1 * tbanecdote[anecdotes[i]].sort;
                        break;
                    case 1:
                        ui.sortIndex = 20 - tbanecdote[anecdotes[i]].sort;
                        break;
                }

                //ui.index = i;
                var desStr = tbLanguage.Get(tbanecdote.Get(anecdotes[i]).name).current +
                             tbLanguage.Get(tbanecdote.Get(anecdotes[i]).desc).current;

                var text = ui.GetTextMeshPro(UIPanel_Common_Events.KTxt_Event);
                text?.SetTMPText(desStr);
                //if (type == 1)
                //{
                //    text?.SetColor(Color.black);
                //}
                //else if (type == 2)
                //{
                //    text?.SetColor(Color.white);
                //}

                ui.GetImage(UIPanel_Common_Events.KImg_Event)?.SetSprite(tbanecdote.Get(anecdotes[i]).icon, false);
            }

            descripList.Sort((a, b) =>
            {
                var uia = a as UIPanel_Common_Events;
                var uib = b as UIPanel_Common_Events;
                return uib.sortIndex.CompareTo(uia.sortIndex);
            });
            JiYuUIHelper.ForceRefreshLayout(descriptionParent);
        }

        private async UniTask<List<UICommon_RewardItem>> CreateReward(int currentID, int type, UI uiParent)
        {
            UI rewardParent = default;
            float itemSize = 156f;
            if (type == 1)
            {
                rewardParent = uiParent.GetFromReference(KContainer_Award);
                itemSize = 124.8f;
            }
            else
            {
                itemSize = 117f;
                rewardParent = uiParent.GetFromReference(UISubPanel_Challenge_Container_AreaDetails.KContainer_Award);
            }


            var rewardList = rewardParent.GetList();
            rewardList.Clear();

            var rewardArray = tbChallenge.Get(currentID).reward;
            JiYuUIHelper.MergeRewardList(rewardArray);

            var rewardItems = new List<UICommon_RewardItem>(rewardArray.Count);
            foreach (var reward in rewardArray)
            {
                var ui = await rewardList.CreateWithUITypeAsync(UIType.UICommon_RewardItem, reward, false) as
                    UICommon_RewardItem;
                rewardItems.Add(ui);
                ui.GetRectTransform().SetScale(new Vector2(itemSize / 156f, itemSize / 156f));

                JiYuUIHelper.SetRewardOnClick(reward, ui);
            }

            rewardList.Sort(JiYuUIHelper.RewardUIComparer);
            JiYuUIHelper.ForceRefreshLayout(rewardParent);
            return rewardItems;
        }


        private void InitAllNeedHide()
        {
            this.GetFromReference(KCommon_ItemTips)?.SetActive(false);
            this.GetFromReference(KTip_Descript)?.SetActive(false);
            this.GetFromReference(KAreaTreadRoot)?.SetActive(false);
            //this.GetFromReference(KBtn_Close)?.SetActive(false);
        }

        /// <summary>
        /// 设置boss图片和tip
        /// </summary>
        /// <param name="bossIndex">第几个boss</param>
        private void SetBossModleAndAni(int currentChallengID)
        {
            var bossIds = FilterBossIDs(currentChallengID);
            if (bossIds.Count <= 0) return;
            var bookId = tbmonster_attr.Get(tbmonster.Get(bossIds[2]).monsterAttrId).bookId;
            var monsterPic = tbmonster_model.Get(bookId).thumbPic;
            GetFromReference(KBtn_Boss3).GetImage().SetSprite(monsterPic, true);
            this.GetButton(KBtn_Boss3)?.OnClick.Add(() => OnBossTipClick(bossIds[2]));
            //SkeletonGraphic bossSkeleton = default;
            //switch (bossIndex)
            //{
            //    case 1:
            //        bossSkeleton = this.GetFromReference(KBoss1SkeletonGraphic).GetComponent<SkeletonGraphic>();
            //        break;
            //    case 2:
            //        bossSkeleton = this.GetFromReference(KBoss2SkeletonGraphic).GetComponent<SkeletonGraphic>();
            //        break;
            //    case 3:
            //        bossSkeleton = this.GetFromReference(KBoss3SkeletonGraphic).GetComponent<SkeletonGraphic>();
            //        break;
            //    default: break;
            //}


            //Log.Debug($"count:{bossIds.Count}, bossindex:{bossIndex - 1},", Color.cyan);

            //var attrId = tbmonster.Get(bossIds[2]).monsterAttrId;
            //string strSke = ;


            //if (strSke == "")
            //{
            //    strSke = "spine_monster_1501_Json_SkeletonData";
            //}
        }

        private void OnBossTipClick(int bossID)
        {
            Log.Debug($"OnBossTipClick", Color.cyan);
            var KCommon_ItemTips = this.GetFromReference(UIPanel_Challege.KCommon_ItemTips);
            //this.GetFromReference(KBtn_Close)?.SetActive(true);
            KCommon_ItemTips?.SetActive(true);

            var txtStr = tbmonster_model.Get((tbmonster_attr.Get(tbmonster.Get(bossID).monsterAttrId).bookId))
                .challengeText;
            var txtIndex = UnityEngine.Random.Range(0, txtStr.Count);

            this.GetTextMeshPro(KText_Des)?.SetTMPText(tbLanguage.Get(txtStr[txtIndex]).current);
            var height = this.GetTextMeshPro(KText_Des).Get().preferredHeight;
            height = height + 76 * 2;
            GetFromReference(KContentBoss).GetRectTransform().SetHeight(height);
            KCommon_ItemTips.GetRectTransform().SetHeight(height + 20);
        }

        private List<int> FilterBossIDs(int challengeID)
        {
            var monsters = tbscene.Get(tblevel.Get(tbChallenge.Get(challengeID).levelId).sceneId).monsterTemplatePara;
            List<int> filteredIds = new List<int>();
            foreach (int id in monsters)
            {
                if (id.ToString()[1] == '4' || id.ToString()[1] == '5')
                {
                    filteredIds.Add(id);
                }
            }

            // 反向排序
            filteredIds.Sort();
            filteredIds.Reverse();

            return filteredIds;
        }


        public void OnAreaTreadClick()
        {
            OnTipClose();
            currentTag = 4002;
            SetThreadTag(currentTag);
            GetFromReference(KMainTreadRoot)?.SetActive(false);
            GetFromReference(KAreaTreadRoot)?.SetActive(true);
        }


        private void SetThreadTag(int tagID)
        {
            var mainThreadText = GetFromReference(KTxt_MainTread)?.GetTextMeshPro();
            var areaThreadText = GetFromReference(KTxt_AreaThread)?.GetTextMeshPro();
            var btnMain = GetFromReference(KBtn_MainTread)?.GetImage();
            var btnArea = GetFromReference(KBtn_AreaThread)?.GetImage();

            if (tagID == 4001)
            {
                //mainThreadText.SetFontSize(55);
                btnMain.SetSpriteAsync("r1", false);
                mainThreadText.SetColor(Color.white);
                mainThreadText.SetTMPText(tbLanguage.Get(tbTagFunc.Get(4001).name).current);
                btnArea.SetSpriteAsync("r2", false);
                //areaThreadText.SetFontSize(50);
                areaThreadText.SetColor("857F78");
                areaThreadText.SetTMPText(tbLanguage.Get(tbTagFunc.Get(4002).name).current);
            }
            else if (tagID == 4002)
            {
                btnMain.SetSpriteAsync("r2", false);
                //areaThreadText.SetFontSize(55);
                areaThreadText.SetColor(Color.white);
                areaThreadText.SetTMPText(tbLanguage.Get(tbTagFunc.Get(4002).name).current);
                //mainThreadText.SetFontSize(50);
                btnArea.SetSpriteAsync("r1", false);
                mainThreadText.SetColor("857F78");
                mainThreadText.SetTMPText(tbLanguage.Get(tbTagFunc.Get(4001).name).current);
            }
        }

        public void OnMainTreadClick()
        {
            OnTipClose();
            currentTag = 4001;

            SetThreadTag(currentTag);
            GetFromReference(KMainTreadRoot)?.SetActive(true);
            GetFromReference(KAreaTreadRoot)?.SetActive(false);
        }

        private void OnTipTagClick()
        {
            this.GetFromReference(KTip_Descript)?.SetActive(true);
            var text = this.GetFromReference(KTxt_Challege_Des).GetTextMeshPro();
            text.SetTMPText(tbLanguage.GetOrDefault(tbTagFunc.Get(currentTag).desc).current);
            var height = text.Get().preferredHeight;
            this.GetFromReference(KTxt_Challege_Des).GetRectTransform().SetHeight(height);
            this.GetFromReference(KcontentDes)?.GetRectTransform().SetHeight(height + 76 * 2);
            this.GetFromReference(KTip_Descript)?.GetRectTransform().SetHeight(height + 76 * 2 + 20);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}