//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Spine.Unity;
using TMPro;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;
using static XFramework.UIContainer_Bar;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Talent)]
    internal sealed class UIPanel_TalentEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPanel_Talent;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;


        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Talent>();
        }
    }


    public partial class UIPanel_Talent : UI, IAwake
    {
        public delegate void UpdatePropContainer();

        public static event UpdatePropContainer UpdateUI;
        private bool isTimer;
        private long timerId;

        List<UI> uiTalentProps;
        //public float idleTimeThreshold = 10f; // 无操作的时间阈值，单位为秒

        private float lastInputTime;
        bool isLockSucess;
        public int nextTalentSkillID;
        private Tbtalent talentMap;
        private List<talent> talentList;
        private List<talent_level> talentLevelList;
        private Tbtalent_level tanlentLevelMap;
        private Tblanguage lang;
        private Tbitem itemMap;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public struct Parameter
        {
            public int id;
            public UI parentUI;
        };

        public void Initialize()
        {
            InitConfig();
            isLockSucess = false;
            UpdateContainer();
            UpdateResource();
            this.GetButton(KBtn_CloseDetails)?.OnClick.Add(CloseDeatails);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Prop),
                () => { OpenPropPanel(1); });
            //this.GetButton(KBtn_Prop)?.OnClick.Add(() => OpenPropPanel(1));
            SetTalentSkill();
            Anim().Forget();
        }

        async UniTaskVoid Anim()
        {
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(
                this.GetFromReference(UIPanel_Talent.KContainer_Prop_Root), 0, cancellationToken: cts.Token);
            await UniTask.Delay(50, cancellationToken: cts.Token);
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UIPanel_Talent.KContainer_Prop), 0,
                100, cts.Token, 0.2f);
            JiYuTweenHelper.SetEaseAlphaAndPosRtoL(
                this.GetFromReference(UIPanel_Talent.KContainer_Tanlent_Skill), -75, cancellationToken: cts.Token);
        }

        //public void RefreshResourceUI()
        //{
        //    var KImg_FilledImgExp = GetFromReference(UIPanel_Main.KImg_FilledImgExp);
        //    var KText_PlayerLevel = GetFromReference(UIPanel_Main.KText_PlayerLevel);
        //    var KText_Diamond = GetFromReference(UIPanel_Main.KText_Diamond);
        //    var KText_Money = GetFromReference(UIPanel_Main.KText_Money);
        //    var KText_Energy = GetFromReference(UIPanel_Main.KText_Energy);

        //    var icon = JiYuUIHelper.GetRewardTextIconName(JiYuUIHelper.GetVector3(JiYuUIHelper.Vector3Type.BITCOIN));
        //    // icon= UnityHelper.RichTextSize(icon, 50);
        //    KText_Gem.GetTextMeshPro().SetTMPText(icon + JiYuUIHelper.ReturnFormatResourceNum(1));

        //    icon = JiYuUIHelper.GetRewardTextIconName(JiYuUIHelper.GetVector3(JiYuUIHelper.Vector3Type.DOLLARS));
        //    /// icon = UnityHelper.RichTextSize(icon, 50);
        //    KText_Gold.GetTextMeshPro().SetTMPText(icon + JiYuUIHelper.ReturnFormatResourceNum(2));

        //    icon = JiYuUIHelper.GetRewardTextIconName(JiYuUIHelper.GetVector3(JiYuUIHelper.Vector3Type.ENEERGY));
        //    // icon = UnityHelper.RichTextSize(icon, 50);
        //    KText_Energy.GetTextMeshPro()
        //        .SetTMPText(icon + JiYuUIHelper.ReturnFormatResourceNum(3) + "/" +
        //                    $"{ResourcesSingleton.Instance.UserInfo.RoleAssets.EnergyMax}");

        //    //ResourcesSingleton.Instance.UserInfo.RoleAssets.Level
        //}

        public void UpdateResource()
        {
            this.GetFromReference(KContainer_Resouce)?.SetActive(true);
            this.GetImage(KImgGold)?.SetSprite("icon_money", false);
            int resoveID = 1030001;

            this.GetImage(KImgResove)?.SetSprite(itemMap[resoveID].icon, false);

            this.GetTextMeshPro(KTxtGold)?.SetTMPText(JiYuUIHelper.ReturnFormatResourceNum(2));
            //this.GetTextMeshPro(KTxtGold)?.SetTMPText(ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill.ToString());

            var resovleNum = ResourcesSingleton.Instance.items.ContainsKey(resoveID)
                ? ResourcesSingleton.Instance.items[resoveID]
                : 0;

            Log.Debug($"resovleNum:{resovleNum}");

            this.GetTextMeshPro(KTxtResove)?.SetTMPText(resovleNum.ToString());

            JiYuTweenHelper.SetEaseAlphaAndPosRtoL(GetFromReference(KContainer_Resouce), 45f,
                cancellationToken: cts.Token);
        }

        private void InitConfig()
        {
            talentMap = ConfigManager.Instance.Tables.Tbtalent;
            talentList = ConfigManager.Instance.Tables.Tbtalent.DataList;
            talentLevelList = ConfigManager.Instance.Tables.Tbtalent_level.DataList;
            tanlentLevelMap = ConfigManager.Instance.Tables.Tbtalent_level;
            lang = ConfigManager.Instance.Tables.Tblanguage;
            itemMap = ConfigManager.Instance.Tables.Tbitem;
        }

        public void UpdateContainer()
        {
            Log.Debug("UpdateContainer");
            SetTalentPropTitle();
            SetTalentPropContainer().Forget();
            ActPlyarAnimation();
            SetResourceNum();
        }

        private void SetResourceNum()
        {
            var itemID = (int)talentMap.Get(20001).cost[0].y;
            long itemCount = 0;
            if (ResourcesSingleton.Instance.items.ContainsKey(itemID))
            {
                itemCount = ResourcesSingleton.Instance.items[itemID];
            }

            this.GetTextMeshPro(KTxtResove)?.SetTMPText(itemCount.ToString());
            this.GetTextMeshPro(KTxtGold)
                ?.SetTMPText(ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill.ToString());
        }

        private async void ActPlyarAnimation()
        {
            var playerSkeleton = this.GetFromReference(UIPanel_Talent.KPlayerSkeletonGraphic)
                .GetComponent<SkeletonGraphic>();
            playerSkeleton.AnimationState.ClearTracks();
            if (this.cts != null)
            {
                this.cts.Cancel();
                this.cts.Dispose();
                cts = new CancellationTokenSource();
            }

            playerSkeleton.Initialize(true);
            playerSkeleton.MatchRectTransformWithBounds();

            int currentLockPropId = ResourcesSingleton.Instance.talentID.talentPropID;
            if (currentLockPropId == 0)
            {
                playerSkeleton.AnimationState.SetAnimation(0, "idle", true);
            }
            else
            {
                var animState = talentMap.GetOrDefault(currentLockPropId).animation;

                var strAnim = "";

                if (animState.Contains("1"))
                {
                    strAnim = "skill1";
                    AudioManager.Instance.PlayFModAudio(1223);
                }
                else if (animState.Contains("2"))
                {
                    strAnim = "skill2";
                    AudioManager.Instance.PlayFModAudio(1224);
                }
                else if (animState.Contains("3"))
                {
                    strAnim = "skill3";
                    AudioManager.Instance.PlayFModAudio(1225);
                }

                playerSkeleton.AnimationState.SetAnimation(0, strAnim, false);
                await UniTask.Delay(JiYuUIHelper.GetAnimaionDuration(playerSkeleton, strAnim),
                    cancellationToken: cts.Token);
                playerSkeleton.AnimationState.SetAnimation(0, "idle", true);
            }
        }

        private void JudegeLockTalentSkill(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.LOCKTALENT, JudegeLockTalentSkill);
            var value = new StringValue();
            value.MergeFrom(e.data);
            Log.Debug(value.Value, Color.cyan);
            if (value.Value != "true")
            {
                nextTalentSkillID = talentMap.Get(nextTalentSkillID).preId;
                return;
            }

            isLockSucess = true;
            if (isLockSucess)
            {
                Log.Debug("56666666666666");

                var cost = new Vector3(talentMap[nextTalentSkillID].cost[0].x, talentMap[nextTalentSkillID].cost[0].y,
                    talentMap[nextTalentSkillID].cost[0].z);
                JiYuUIHelper.TryReduceReward(cost);
                //技能解锁
                ResourcesSingleton.Instance.talentID.talentSkillID = nextTalentSkillID;
                int nexID = 0;
                for (int i = 0; i < talentList.Count; i++)
                {
                    if (talentList[i].preId == nextTalentSkillID)
                    {
                        nexID = talentList[i].id;
                        break;
                    }
                }

                SetTalentSkill(true);

                if (talentMap[nexID].level >= talentLevelList[talentLevelList.Count - 1].id)
                {
                    DisplaySkillDetail(nexID, true);

                    return;
                }

                DisplaySkillDetail(nexID, false);
                UpdateResource();
            }
        }

        public void OpenPropPanel(int type)
        {
            UIHelper.Create<int>(UIType.UIPanel_Talent_Prop, type);
            //打开天赋属性面板
        }

        private void SetTalentSkillDetail(int skillID)
        {
            GetFromReference(KContainer_SkillWarn).SetActive(false);
            GetFromReference(KContainer_SkillTip).SetActive(false);
            GetFromReference(KBtn_Resovle).SetActive(false);
            GetFromReference(KImg_Tanlet_Skill).GetImage().SetSprite(talentMap[skillID].icon, false);
            GetFromReference(KText_Prop_Name).GetTextMeshPro().SetTMPText(lang.Get(talentMap[skillID].name).current);
            GetFromReference(KText_Prop_Descript).GetTextMeshPro()
                .SetTMPText(lang.Get(talentMap[skillID].desc).current);
            var KBtn_Add = GetFromReference(UIPanel_Talent.KBtn_Add);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Add, () => onSkillAddButtonClicked());
        }

        private void onSkillAddButtonClicked()
        {
            OpenPropPanel(2);
        }

        private void DisplaySkillDetail(int nextTalentSkillID, bool isLimit)
        {
            GetFromReference(KContainer_Tanlent_Skill).SetActive(false);

            this.GetXButton(KBtn_Resovle).RemoveAllListeners();
            GetFromReference(KContainer_Tanlent_Details).SetActive(true);
            int playerLevel = ResourcesSingleton.Instance.UserInfo.RoleAssets.Level;

            SetTalentSkillDetail(nextTalentSkillID);
            if (isLimit)
            {
                //达到版本上限 没有可解锁的技能
                DisplayInfoOfLimit();
                return;
            }

            if (playerLevel < talentMap[nextTalentSkillID].level)
            {
                DisplayInfoOfSkillLevel(talentMap[nextTalentSkillID].level);
                return;
            }

            GetFromReference(KBtn_Resovle).SetActive(true);
            long marginCount = 0;
            if (isCanLock(nextTalentSkillID, ref marginCount))
            {
                DisplayEvolustion(nextTalentSkillID);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Resovle),
                    () => OnClickEvolustion(nextTalentSkillID));
            }
            else
            {
                ///资源不够解锁

                GetFromReference(KImg_Left).SetActive(false);
                GetFromReference(KText_Mid).SetActive(true);
                GetFromReference(KText_Right).SetActive(false);


                GetFromReference(KText_Mid).GetTextMeshPro().SetTMPText(lang.Get("goto_tag_4").current);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Resovle), () => GotoTag4());
                GetFromReference(KContainer_SkillWarn).SetActive(true);
                GetFromReference(KContainer_SkillWarn).GetRectTransform().GetChild(0).GetComponent<TMP_Text>()
                    .SetTMPText(lang.Get("common_lack_5_title").current + lang.Get("common_demand").current);
                GetFromReference(KContainer_SkillWarn).GetRectTransform().GetChild(1).GetChild(0).GetComponent<Image>()
                    .SetSprite(
                        ResourcesManager.LoadAsset<Sprite>(itemMap[(int)talentMap[nextTalentSkillID].cost[0].y].icon),
                        false);
                GetFromReference(KContainer_SkillWarn).GetRectTransform().GetChild(1).GetChild(1)
                    .GetComponent<TMP_Text>()
                    .SetTMPText("x" + marginCount.ToString());
            }
        }

        private void CloseDeatails()
        {
            GetFromReference(KContainer_Tanlent_Details).SetActive(false);
            GetFromReference(KContainer_Tanlent_Skill).SetActive(true);
        }

        private void StartTimer()
        {
            isTimer = true;
            //开启一个任务 判断鼠标或者触屏的状态是否改变
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.RepeatedFrameTimer(this.WaitToShrink);
        }

        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
            isTimer = false;
        }

        private void WaitToShrink()
        {
            //// 检查鼠标按下或触屏触摸的状态
            //bool isInputDetected = Input.GetMouseButtonDown(0) || Input.touchCount > 0;

            //if (isInputDetected)
            //{
            //    // 重置最后输入时间
            //    lastInputTime = Time.time;
            //}
            //else
            //{
            //    // 检查是否达到无操作的时间阈值
            //    float idleTime = Time.time - lastInputTime;

            //    if (idleTime >= idleTimeThreshold)
            //    {
            //        // 鼠标或触屏无操作
            //        Debug.Log("No input detected");
            //        GetFromReference(KContainer_Tanlent_Details).SetActive(false);
            //        GetFromReference(KContainer_Tanlent_Skill).SetActive(true);
            //        if (isTimer)
            //        {
            //            RemoveTimer();
            //        }
            //    }
            //}
        }


        private void GotoTag4()
        {
            if (isTimer)
            {
                RemoveTimer();
            }

            var str = $"type=1;para=[411]";
            JiYuUIHelper.GoToSomePanel(str);
            //跳转页签4
            // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out UI jiyu))
            // {
            //     var ui = jiyu as UIPanel_JiyuGame;
            //     var tagMap = ConfigManager.Instance.Tables.Tbtag;
            //     ui.OnBtnClickEvent(tagMap[4].sort);
            // }
        }

        private void OnClickEvolustion(int nextId)
        {
            Log.Debug($"talentID:{nextId}", Color.cyan);
            nextTalentSkillID = nextId;
            WebMessageHandlerOld.Instance.AddHandler(CMD.LOCKTALENT, JudegeLockTalentSkill);
            NetWorkManager.Instance.SendMessage(CMD.LOCKTALENT, new IntValue { Value = nextId });
        }

        private void DisplayEvolustion(int nextTalentSkillID)
        {
            Debug.Log($"$nextTalentSkillID:{nextTalentSkillID}");
            GetFromReference(KImg_Left).SetActive(true);
            GetFromReference(KText_Mid).SetActive(true);
            GetFromReference(KText_Right).SetActive(true);


            GetFromReference(KImg_Left).GetImage()
                .SetSprite(itemMap[(int)talentMap[nextTalentSkillID].cost[0].y].icon, false);
            GetFromReference(KText_Mid).GetTextMeshPro().SetTMPText(talentMap[nextTalentSkillID].cost[0].z.ToString());
            GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(lang.Get("talent_skill_unlock").current);
        }

        public void SetTalentSkill(bool isDisplayDetail = false)
        {
            GetFromReference(KContainer_Tanlent_Skill).SetActive(!isDisplayDetail);
            GetFromReference(KContainer_Tanlent_Details).SetActive(isDisplayDetail);
            GetFromReference(KImg_Up).SetActive(false);
            int currentLockSkillId = ResourcesSingleton.Instance.talentID.talentSkillID;
            bool isLimit = false;
            Log.Debug(currentLockSkillId.ToString(), Color.cyan);
            if (currentLockSkillId < 20001)
            {
                //初始默认的技能id
                for (int i = 0; i < talentList.Count; i++)
                {
                    if (talentList[i].preId == currentLockSkillId && talentList[i].type == 2)
                    {
                        nextTalentSkillID = talentList[i].id;
                        break;
                    }
                }
            }
            else if (talentMap[currentLockSkillId].level >= talentLevelList[talentLevelList.Count - 1].id)
            {
                //达到上限了
                nextTalentSkillID = currentLockSkillId;
                isLimit = true;
            }
            else
            {
                nextTalentSkillID = currentLockSkillId + 1;
            }

            long temp = 0;
            if (isCanLock(nextTalentSkillID, ref temp))
            {
                GetFromReference(KImg_Up).SetActive(true);
            }

            GetFromReference(KBtn_Tanlet_Skill).GetImage().SetSprite(talentMap[nextTalentSkillID].icon, false);
            this.GetButton(KBtn_Tanlet_Skill)?.OnClick.RemoveAllListeners();
            this.GetButton(KBtn_Tanlet_Skill)?.OnClick.Add(() => DisplaySkillDetail(nextTalentSkillID, isLimit));
        }

        private async UniTaskVoid SetTalentPropContainer()
        {
            GetFromReference(KContainer_Tip).SetActive(false);

            uiTalentProps = new List<UI>(5);
            int nextTalentPropID = 0;
            int playerLevel = ResourcesSingleton.Instance.UserInfo.RoleAssets.Level;

            int currentLockPropId = ResourcesSingleton.Instance.talentID.talentPropID;

            int maxPropID = GetPropMaxID(currentLockPropId);
            if (currentLockPropId == 0)
            {
                //初始默认的天赋id
                nextTalentPropID = talentList[0].id;
            }
            else if (currentLockPropId >= maxPropID)
            {
                //达到当前版本上限
                DisplayInfoOfLimit();
                return;
            }
            else
            {
                for (int i = 0; i < talentList.Count; i++)
                {
                    if (talentList[i].preId == currentLockPropId)
                    {
                        nextTalentPropID = talentList[i].id;
                        break;
                    }
                }
                //nextTalentPropID = tanlentMap[currentLockPropId] + 1;
            }

            if (playerLevel < talentMap[nextTalentPropID].level)
            {
                DisplayInfoOfLevel(talentMap[nextTalentPropID].level);
                return;
            }

            Log.Debug(nextTalentPropID.ToString());
            SetTanlentPropDetails(nextTalentPropID);
        }

        private int GetPropMaxID(int currentLockPropId)
        {
            var tanlentLevelList = ConfigManager.Instance.Tables.Tbtalent.DataList;
            int maxPropID = 0;
            for (int i = 0; i < tanlentLevelList.Count; i++)
            {
                if (tanlentLevelList[i].type == 1)
                {
                    maxPropID = tanlentLevelList[i].id > maxPropID ? tanlentLevelList[i].id : maxPropID;
                }
            }

            return maxPropID;
        }

        private bool isCanLock(int talentSkillId, ref long marginCount)
        {
            var tanlentMap = ConfigManager.Instance.Tables.Tbtalent.DataMap;
            var costs = tanlentMap[talentSkillId].cost;
            if (!JudgeCosts(costs[0], ref marginCount))
            {
                return false;
            }

            return true;
        }

        private bool JudgeCosts(Vector3 vector3, ref long marginCount)
        {
            switch (vector3.x)
            {
                case 1:
                    return ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy >= vector3.z ? true : false;
                case 2:
                    return ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin >= vector3.z ? true : false;
                case 3:

                    return ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill >= vector3.z ? true : false;
                case 5:
                    return isItemEnough((int)vector3.y, (int)vector3.z, ref marginCount);
            }

            return false;
        }

        private bool isItemEnough(int itemID, int needCount, ref long marginCount)
        {
            if (!ResourcesSingleton.Instance.items.ContainsKey(itemID))
            {
                marginCount = needCount;
                return false;
            }

            if (ResourcesSingleton.Instance.items[itemID] < needCount)
            {
                marginCount = needCount - ResourcesSingleton.Instance.items[itemID];
                return false;
            }

            return true;
        }

        /// <summary>
        /// 版本上限信息显示
        /// </summary>
        private void DisplayInfoOfLimit()
        {
            GetFromReference(KContainer_Prop).SetActive(false);
            GetFromReference(KContainer_Tip).SetActive(true);
            GetFromReference(KImg_Icon).GetImage().SetSprite("icon_warning", false);
            GetFromReference(KText_Tip).GetTextMeshPro().SetTMPText(lang.Get("talent_max_text_1").current);

            GetFromReference(KContainer_SkillTip).SetActive(true);
            GetFromReference(KContainer_SkillTip).GetRectTransform().GetChild(1).GetComponent<TMP_Text>()
                .SetTMPText(lang.Get("talent_max_text_2").current);
            //throw new NotImplementedException();
            GetFromReference(KContainer_SkillTip).GetRectTransform().GetChild(0).GetComponent<Image>()
                .SetSprite(ResourcesManager.LoadAsset<Sprite>("icon_warning_white"), false);
        }

        /// <summary>
        /// 显示未达到相应等级的界面 
        /// </summary>
        /// <param name="level"></param>
        private void DisplayInfoOfLevel(int level)
        {
            GetFromReference(KContainer_Prop).SetActive(false);
            GetFromReference(KContainer_Tip).SetActive(true);
            //var lang = ConfigManager.Instance.Tables.Tblanguage;
            GetFromReference(KImg_Icon).GetImage().SetSprite("icon_lock_yellow", false);
            string temp = lang.Get("talent_limit_text").current;
            //string levelRed = "<color=red>" + level.ToString() + "</color>";
            var descStr = string.Format(temp, UnityHelper.RichTextColor(level.ToString(), "d95648"));
            GetFromReference(KText_Tip).GetTextMeshPro().SetTMPText(descStr);
            //GetFromReference(KText_Tip).GetTextMeshPro().SetTMPText(ReplaceBetweenBraces(temp, levelRed));
        }

        /// <summary>
        /// 显示未达到相应技能等级的界面 
        /// </summary>
        /// <param name="level"></param>
        private void DisplayInfoOfSkillLevel(int skillLevel)
        {
            GetFromReference(KContainer_SkillTip).SetActive(true);
            var lang = ConfigManager.Instance.Tables.Tblanguage;
            string temp = lang.Get("talent_limit_text").current;
            GetFromReference(KContainer_SkillTip).SetActive(true);
            Log.Debug(GetFromReference(KContainer_SkillTip).GetRectTransform().GetChild(0).GetComponent<XImage>().name);
            GetFromReference(KContainer_SkillTip).GetRectTransform().GetChild(0).GetComponent<XImage>().sprite =
                ResourcesManager.LoadAsset<Sprite>("icon_lock_yellow");
            var descStr = string.Format(temp, UnityHelper.RichTextColor(skillLevel.ToString(), "d95648"));
            //GetFromReference(KText_Tip).GetTextMeshPro().SetTMPText(descStr);
            GetFromReference(KContainer_SkillTip).GetRectTransform().GetChild(1).GetComponent<TMP_Text>()
                .SetTMPText(descStr);
        }


        string ReplaceBetweenBraces(string input, string replacement)
        {
            Regex regex = new Regex("{(.*?)}");
            MatchCollection matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                string content = match.Groups[1].Value;
                input = input.Replace(match.Value, replacement);
            }

            return input;
        }

        /// <summary>
        /// 设置具体的tanlent图标
        /// </summary>
        /// <param name="nextTalentPropID"></param>
        private async void SetTanlentPropDetails(int nextTalentPropID)
        {
            int needLevel = talentMap[nextTalentPropID].level;
            var containerList = GetFromReference(KContainer_Prop).GetList();
            containerList.Clear();
            for (int i = 0; i < talentList.Count; i++)
            {
                if (talentList[i].level == needLevel && talentList[i].type == 1)
                {
                    int index = i;
                    // Log.Debug($"tanlentList[i]:{tanlentList[i]}");
                    ParamterBtn paramterBtn;
                    paramterBtn.talentID = talentList[i].id;
                    paramterBtn.isDisplayArrow = false;
                    paramterBtn.isOnlyDisplay = false;
                    var ui = await containerList.CreateWithUITypeAsync<UIContainer_Bar.ParamterBtn>(
                        UIType.UICommon_Btn1, paramterBtn, false) as UICommon_Btn1;
                    ui.index = index;
                }
            }

            containerList.Sort((a, b) =>
            {
                var uia = a as UICommon_Btn1;
                var uib = b as UICommon_Btn1;
                return uia.index.CompareTo(uib.index);
            });
        }

        private void SetTalentPropTitle()
        {
            int currentLockPropId = ResourcesSingleton.Instance.talentID.talentPropID;
            int nextId = 0;
            foreach (var item in talentList)
            {
                if (item.preId == currentLockPropId)
                {
                    nextId = item.id;
                    break;
                }
            }

            if (nextId == 0) return;
            GetFromReference(KText_Level).GetTextMeshPro()
                .SetTMPText(talentMap[nextId].level.ToString());
            GetFromReference(KText_Level_Description).GetTextMeshPro()
                .SetTMPText(lang.Get(tanlentLevelMap[talentMap[nextId].level].name).current);
            //if (currentLockPropId == 0)
            //{
            //    //一个天赋都没解锁 默认一级
            //    int defaultLevel = 1;
            //    GetFromReference(KText_Level).GetTextMeshPro().SetTMPText(defaultLevel.ToString());
            //    GetFromReference(KText_Level_Description).GetTextMeshPro()
            //        .SetTMPText(lang.Get(tanlentLevelMap[defaultLevel].name).current);
            //}
            //else
            //{

            //}
        }

        protected override void OnClose()
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.LOCKTALENT, JudegeLockTalentSkill);
            if (uiTalentProps.Count > 0)
            {
                foreach (var child in uiTalentProps)
                {
                    child.Dispose();
                }
            }

            if (isTimer)
            {
                RemoveTimer();
            }

            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}