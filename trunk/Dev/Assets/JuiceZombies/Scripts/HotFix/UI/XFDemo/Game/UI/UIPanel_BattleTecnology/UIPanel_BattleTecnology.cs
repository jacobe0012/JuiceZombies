//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using cfg.config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HotFix_UI;
using Main;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_BattleTecnology)]
    internal sealed class UIPanel_BattleTecnologyEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_BattleTecnology;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_BattleTecnology>();
        }
    }

    public partial class UIPanel_BattleTecnology : UI, IAwake<int>
    {
        private Tblanguage tbLanguage;
        private Tbbattletech_drop tbBattletech_drop;
        private EntityManager entityManager;
        private Tbbattletech tbBattletech;
        private Tbguide tbguide;
        private Entity player;
        private List<int> displaySelectedTechs;
        private List<int> selectedTechs;
        private Dictionary<int, int> availableTechDic;
        private int currentStag;
        private bool isGuide;

        public async void Initialize(int args)
        {
            await UnicornUIHelper.InitBlur(this);
            UnicornUIHelper.StartStopTime(false);

            currentStag = args;
            InitTables();


            var money = entityManager.GetComponentData<PlayerData>(player).playerData.exp;
            availableTechDic = new Dictionary<int, int>();
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
            {
                var currentUI = ui as UIPanel_RunTimeHUD;
                displaySelectedTechs = new List<int>(currentUI.displaySelectedTechs);
                selectedTechs = new List<int>(displaySelectedTechs);
            }

            Debug.Log($"displaySelectedTechs{displaySelectedTechs.Count}");

            #region ����

            for (int i = 0; i < displaySelectedTechs.Count; i++)
            {
                Debug.Log($"displaySelectedTechs:{displaySelectedTechs[i]}");
            }

            #endregion

            if (displaySelectedTechs.Count > 0)
            {
                SetTechBtnItem().Forget();
            }
            else
            {
                GetFromReference(KContainer_Selcted)?.SetActive(false);
                GetFromReference(KContainer_Selcted_details)?.SetActive(false);
            }

            currentStag = UpdateCurrentStag();
            this.GetTextMeshPro(KTxt_SumMoney)?.SetTMPText(money.ToString());
            this.GetTextMeshPro(KTxt_TecnologyName)?.SetTMPText(tbLanguage.Get("battletech_name").current);
            this.GetTextMeshPro(KTxt_Name_Selected)?.SetTMPText(tbLanguage.Get("battletech_learnt").current);
            this.GetTextMeshPro(KTxt_Name_Selected1)?.SetTMPText(tbLanguage.Get("battletech_learnt").current);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_CloseTip), () =>
            {
                if (GetFromReference(KContainer_Selcted_details).GameObject.activeSelf)
                {
                    GetFromReference(KContainer_Selcted_details).SetActive(false);
                    GetFromReference(KContainer_Selcted).SetActive(true);
                }

                if (UnicornUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui1))
                {
                      UnicornUIHelper.DestoryAllTips();;
                }
            });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_TecnologyDes), async () =>
            {
                UnicornUIHelper.DestoryAllTips();
                var KText_Des = GetFromReference(UIPanel_Pass.KText_Des);
                var desc = tbLanguage.Get("battletech_tips").current;
                GetFromReference(KCommon_ItemTips).SetActive(!GetFromReference(KCommon_ItemTips).GameObject.activeSelf);
                KText_Des.GetTextMeshPro().SetTMPText(desc);
                var height = KText_Des.GetTextMeshPro().Get().preferredHeight;
                var width = KText_Des.GetTextMeshPro().Get().preferredWidth;
                KText_Des.GetRectTransform().SetHeight(height);
                GetFromReference(KContent).GetRectTransform().SetHeight(height + 76 * 2);

            });
            InitTechItem();

            //UpdateTechItem();
            //UpdateTechAvailable(stag);
            Guide().Forget();
        }


        public void GuideOnClick()
        {
            Log.Debug($"isGuide{isGuide}");
            if (isGuide)
            {
                if (UnicornUIHelper.TryGetUI(UIType.UISubPanel_Guid,out var ui))
                {
                    ui.Dispose();
                }
            }
        }
        async UniTaskVoid Guide()
        {
            var KContainer_Tecnology = this.GetFromReference(UIPanel_BattleTecnology.KContainer_Tecnology);
            var guide = tbguide.DataList.Where(a => a.guideType == 312).FirstOrDefault();
            if (ResourcesSingletonOld.Instance.settingData.GuideList.Contains(guide.group))
            {
                isGuide = true;
                var guideUI = await UIHelper.CreateAsync(UIType.UISubPanel_Guid, guide.id);
               
                var KImg_Bg = guideUI.GetFromReference(UISubPanel_Guid.KImg_Bg);
                UnicornUIHelper.SetForceGuideRectUI(KContainer_Tecnology, KImg_Bg);
            }
        }

        private void InitTables()
        {
            tbLanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbBattletech_drop = ConfigManager.Instance.Tables.Tbbattletech_drop;
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            tbBattletech = ConfigManager.Instance.Tables.Tbbattletech;
            tbguide = ConfigManager.Instance.Tables.Tbguide;
            //wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag), typeof(GameTimeData));
            player = entityManager.CreateEntityQuery(typeof(PlayerData)).ToEntityArray(Allocator.Temp)[0];
            //tbPlayer_skill_binding = ConfigManager.Instance.Tables.Tbplayer_skill_binding;
        }

        protected override void OnClose()
        {
            UnicornUIHelper.DestoryAllTips();
            UnicornUIHelper.StartStopTime(true);

            base.OnClose();
        }


        // public void CloseThisPanel()
        // {
        //     if (UnicornUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui1))
        //     {
        //           UnicornUIHelper.DestoryAllTips();;
        //     }
        //
        //     UnicornUIHelper.StartStopTime(true);
        //     if (UnicornUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
        //     {
        //         var uiRuntime = ui as UIPanel_RunTimeHUD;
        //         uiRuntime.EnableInputBar();
        //         uiRuntime.StartTimer();
        //         UnityHelper.BeginTime();
        //     }
        //
        //     Close();
        // }


        private void SetTipPos(UI itemUI, UI tipUI, string contentKey, string arrowDownKey, string arrowUpKey,
            float contentGap = 30f)
        {
            var arrowDown = tipUI.GetFromReference(arrowDownKey);
            var arrowUp = tipUI.GetFromReference(arrowUpKey);


            var itemRect = itemUI.GetRectTransform();
            var tipRect = tipUI.GetRectTransform();


            ScrollRect scrollRect = itemUI.GameObject.transform.GetComponentInParent<ScrollRect>();

            Canvas canvas = itemUI.GameObject.transform.GetComponentInParent<Canvas>();

            // var rewardPosX = itemRect.AnchoredPosition().x;
            // var parentPos = scrollRect.content.anchoredPosition;


            RectTransform canvasRect = canvas.transform.GetComponent<RectTransform>();
            Vector3 loadpos = canvas.transform.InverseTransformPoint(itemUI.GameObject.transform.position);


            if (scrollRect != null)
            {
                Vector3 scrollRectPos =
                    canvas.transform.InverseTransformPoint(scrollRect.transform.position);
                var scrollRectWidth = scrollRect.transform.GetComponent<RectTransform>().rect.width;
                var scrollRectLeftPos = scrollRectPos.x - scrollRectWidth / 2f;
                var scrollRectRightPos = scrollRectPos.x + scrollRectWidth / 2f;

                loadpos.x = math.clamp(loadpos.x, scrollRectLeftPos, scrollRectRightPos);
            }

            var itemUpOffset = itemRect.Width() / 2f * itemRect.Scale().x;
            var tipUpOffset = tipRect.Height() / 2f * tipRect.Scale().y;

            float offsetY = itemUpOffset + tipUpOffset;
            offsetY -= itemRect.Width() * itemRect.Scale().x + tipRect.Height() * tipRect.Scale().y;
            arrowDown.SetActive(false);
            arrowUp.SetActive(true);


            var tipPos = new Vector3(loadpos.x, loadpos.y + offsetY + 100);


            tipUI.GetRectTransform().SetAnchoredPosition(tipPos);
            var tipWidth = tipRect.Width() * tipRect.Scale().x;

            var arrow = tipUI.GetFromReference(arrowDownKey).GetRectTransform();
            var arrowWidth = arrow.Width() * arrow.Scale().x;


            var screenPosL = -(Screen.width / 2f);
            var screenPosR = Screen.width / 2f;
            var tipPosL = loadpos.x - tipWidth / 2;
            var tipPosR = loadpos.x + tipWidth / 2;
            var contentRect = tipUI.GetFromReference(contentKey).GetRectTransform();

            if (tipPosL < screenPosL + contentGap)
            {
                var contentPos = math.length(tipPosL) - math.length(screenPosL + contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);

                contentRect.SetAnchoredPosition(new Vector2(contentPos, 0));
            }
            else if (tipPosR > screenPosR - contentGap)
            {
                var contentPos = math.length(tipPosR) - math.length(screenPosR - contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);

                contentRect.SetAnchoredPosition(new Vector2(-contentPos, 0));
            }
            else
            {
                contentRect.SetAnchoredPosition(Vector2.zero);
            }
        }

        private int UpdateCurrentStag()
        {
            int bigState = 1, smallState = 1;
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
            {
                var uiRuntime = ui as UIPanel_RunTimeHUD;
                bigState = uiRuntime.bossState;
                smallState = uiRuntime.state;
            }

            this.GetTextMeshPro(KTxt_StagName)?.SetTMPText(tbLanguage.GetOrDefault("common_stage").current +
                                                           bigState.ToString() + "-" + smallState.ToString());
            return bigState;
        }

        private void InitTechItem()
        {
            var tbConstant = ConfigManager.Instance.Tables.Tbbattle_constant;

            //ѡ��ļ���id-index
            Dictionary<int, int> selectedIndex =
                new Dictionary<int, int>(tbConstant.Get("battletech_refresh_num").constantValue);
            int weights = UpdateTechAvailable(currentStag);
            int count = 0;
            int index = 0;
            int needCount = tbConstant.Get("battletech_refresh_num").constantValue;


            var availableTechs = availableTechDic.Keys.ToList();

            while (count < needCount)
            {
                //ѡ��ɹ�
                if (SelectOneNormalSkill(availableTechs[index % availableTechDic.Count], weights))
                {
                    //id�Ƿ��ظ�
                    if (selectedIndex.ContainsKey(availableTechs[index % availableTechDic.Count]))
                    {
                        index++;
                        continue;
                    }

                    selectedTechs.Add(availableTechs[index % availableTechDic.Count]);
                    selectedIndex.Add(availableTechs[index % availableTechDic.Count], index % availableTechDic.Count);
                    count++;
                }

                index++;
                //selectedSkills.Add(availableSkills[index % availableSkills.Count]);
            }

            SetTechItemUI(selectedIndex).Forget();
        }

        public void RefreshTechItem(int technoloId, UIPanel_TechnologyItem ui)
        {
            //selectedTechs.Add(technoloId);
            int weights = UpdateTechAvailable(currentStag);
            int availableCount = availableTechDic.Count;
            int randomIndex = UnityEngine.Random.Range(0, availableCount);
            var newTech = availableTechDic.Keys.ToList()[randomIndex];
            selectedTechs.Add(newTech);
            ui.UpdateItem(newTech);
        }


        private int UpdateTechAvailable(int stag)
        {
            availableTechDic.Clear();
            int sumPower = 0;
            var availableTechs = new List<int>();
            var tech = tbBattletech_drop.DataList;
            for (int i = 0; i < tech.Count; i++)
            {
                if (tech[i].id == stag)
                {
                    availableTechs.Add(tech[i].battletechId);
                }
            }

            if (selectedTechs.Count > 0)
            {
                availableTechs = availableTechs.Where(item => !selectedTechs.Contains(item)).ToList();
            }

            for (int i = 0; i < availableTechs.Count; i++)
            {
                for (int j = 0; j < tech.Count; j++)
                {
                    if (availableTechs[i] == tech[j].battletechId)
                    {
                        if (!availableTechDic.ContainsKey(availableTechs[i]))
                        {
                            availableTechDic.Add(availableTechs[i], tech[j].power);
                            sumPower += tech[j].power;
                        }
                    }
                }
            }

            return sumPower;
        }

        bool SelectOneNormalSkill(int id, int totalWeight)
        {
            var dropList = tbBattletech_drop.DataList;
            var currentPower = 0;
            //var userSkillTable = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
            var randomValue = UnityEngine.Random.Range(1, totalWeight + 1);
            //int randomValue = rand.NextInt();

            //var entity = query.ToEntityArray(Allocator.Temp)[0];
            //World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<GameRandomData>(entity, rand);
            for (int i = 0; i < dropList.Count; i++)
            {
                if (dropList[i].battletechId == id)
                {
                    currentPower = dropList[i].power;
                    break;
                }
            }

            // ������ɵ������С�ڵ��ڸ����ĸ��ʣ���ѡ��
            if (randomValue <= currentPower)
            {
                return true;
            }

            return false;
        }


        private async UniTaskVoid SetTechBtnItem()
        {
            GetFromReference(KContainer_Selcted)?.SetActive(true);
            GetFromReference(KContainer_Selcted_details)?.SetActive(false);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(GetFromReference(KContainer_Selcted), OnClickContainer);


            for (int i = 0; i < displaySelectedTechs.Count; i++)
            {
                var techID = displaySelectedTechs[i];
                Image img = default;
                if (i == 0)
                {
                    img = this.GetFromReference(Ktag1).GetRectTransform().GetChild(0)
                        .GetComponent<Image>();
                }
                else if (i == 1)
                {
                    img = this.GetFromReference(Ktag2).GetRectTransform().GetChild(0)
                        .GetComponent<Image>();
                }
                else
                {
                    img = this.GetFromReference(Ktag3).GetRectTransform().GetChild(0)
                        .GetComponent<Image>();
                }

                var icon = ResourcesManager.Instance.Loader.LoadAsset<Sprite>(tbBattletech.Get(techID).icon);
                img.SetSprite(icon, false);
            }
            //List<int> numList = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 };
            //for (int i = 0; i < displaySelectedTechs.Count; i++)
            //{
            //    var techID = displaySelectedTechs[i];
            //    int count = numList[tbBattletech.Get(techID).type];
            //    count++;
            //    numList[tbBattletech.Get(techID).type] = count;
            //}

            //int number = 0;
            //for (int i = 0; i < numList.Count; i++)
            //{
            //    if (numList[i] > 0)
            //    {
            //        number++;
            //        foreach (var tech in tbBattletech.DataList)
            //        {
            //            if (tech.type == i)
            //            {
            //                var icon = ResourcesManager.Instance.Loader.LoadAsset<Sprite>(tech.typeIcon);
            //                if (number == 1)
            //                {
            //                    this.GetFromReference(Ktag1).SetActive(true);
            //                    this.GetFromReference(KtagS1).SetActive(true);
            //                    var img = this.GetFromReference(Ktag1).GetRectTransform().GetChild(0)
            //                        .GetComponent<Image>();
            //                    var img1 = this.GetFromReference(KtagS1).GetRectTransform().GetChild(0)
            //                        .GetComponent<Image>();
            //                    var text = this.GetFromReference(Ktag1).GetRectTransform().GetChild(0).GetChild(0)
            //                        .GetComponent<TMP_Text>();
            //                    var text1 = this.GetFromReference(KtagS1).GetRectTransform().GetChild(0).GetChild(0)
            //                        .GetComponent<TMP_Text>();
            //                    img.SetSprite(icon, false);
            //                    img1.SetSprite(icon, false);
            //                    text.SetTMPText(numList[i].ToString());
            //                    text1.SetTMPText(numList[i].ToString());
            //                }
            //                else if (number == 2)
            //                {
            //                    this.GetFromReference(Ktag2).SetActive(true);
            //                    this.GetFromReference(KtagS2).SetActive(true);
            //                    var img = this.GetFromReference(Ktag2).GetRectTransform().GetChild(0)
            //                        .GetComponent<Image>();
            //                    img.SetSprite(icon, false);
            //                    var img1 = this.GetFromReference(KtagS2).GetRectTransform().GetChild(0)
            //                        .GetComponent<Image>();
            //                    img1.SetSprite(icon, false);

            //                    var text = this.GetFromReference(Ktag2).GetRectTransform().GetChild(0).GetChild(0)
            //                        .GetComponent<TMP_Text>();
            //                    var text1 = this.GetFromReference(KtagS2).GetRectTransform().GetChild(0).GetChild(0)
            //                        .GetComponent<TMP_Text>();
            //                    text.SetTMPText(numList[i].ToString());
            //                    text1.SetTMPText(numList[i].ToString());
            //                }
            //                else if (number == 3)
            //                {
            //                    this.GetFromReference(Ktag3).SetActive(true);
            //                    this.GetFromReference(KtagS3).SetActive(true);
            //                    var img = this.GetFromReference(Ktag3).GetRectTransform().GetChild(0)
            //                        .GetComponent<Image>();
            //                    img.SetSprite(icon, false);
            //                    var img1 = this.GetFromReference(KtagS3).GetRectTransform().GetChild(0)
            //                        .GetComponent<Image>();
            //                    img1.SetSprite(icon, false);

            //                    var text = this.GetFromReference(Ktag3).GetRectTransform().GetChild(0).GetChild(0)
            //                        .GetComponent<TMP_Text>();
            //                    var text1 = this.GetFromReference(KtagS3).GetRectTransform().GetChild(0).GetChild(0)
            //                        .GetComponent<TMP_Text>();
            //                    text.SetTMPText(numList[i].ToString());
            //                    text1.SetTMPText(numList[i].ToString());
            //                }

            //                break;
            //            }
            //        }
            //    }
            //}


            SetAndoteWidth();

            //DestroyAllItemBtn();
            var tecnologyBtnList = GetFromReference(KContent).GetList();
            tecnologyBtnList.Clear();
            for (int i = 0; i < displaySelectedTechs.Count; i++)
            {
                var techID = displaySelectedTechs[i];
                await tecnologyBtnList.CreateWithUITypeAsync<int>(UIType.UIPanel_TechnologyIemBtn, techID, false);
            }
        }

        private void OnClickContainer()
        {
            GetFromReference(KContainer_Selcted_details)?.SetActive(true);
            GetFromReference(KContainer_Selcted)?.SetActive(false);
            Log.Debug($"count:{displaySelectedTechs.Count}");
            if (displaySelectedTechs.Count == 1)
            {
                Log.Debug($"11111count:{displaySelectedTechs.Count}");
                GetFromReference(KContainer_Selcted_details)?.GetRectTransform().SetAnchoredPositionX(213f);
            }
            else if (displaySelectedTechs.Count == 2)
            {
                Log.Debug($"22222count:{displaySelectedTechs.Count}");
                GetFromReference(KContainer_Selcted_details)?.GetRectTransform().SetAnchoredPositionX(-100f);
            }
        }

        private void SetAndoteWidth()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetFromReference(KContainer_Tag).GetComponent<RectTransform>());
            var width = GetFromReference(KContainer_Tag).GetRectTransform().Width();
            Log.Debug($"width:{width}", Color.cyan);
            GetFromReference(KContainer_Selcted).GetRectTransform().SetWidth(width + 230);
            GetFromReference(KContainer_Selcted).GetRectTransform().SetAnchoredPositionX(30);
        }

        private async UniTaskVoid SetTechItemUI(Dictionary<int, int> selectedIndex)
        {
            var refreshTech = selectedIndex.Keys.ToList();

            var tecnologyList = GetFromReference(KContainer_Tecnology).GetList();
            tecnologyList.Clear();
            for (int i = 0; i < refreshTech.Count; i++)
            {
                var techID = refreshTech[i];
                var ui = await tecnologyList.CreateWithUITypeAsync(UIType.UIPanel_TechnologyItem, techID, false);
                ui.GetFromReference(UIPanel_TechnologyItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 0;

            }

            tecnologyList.Sort((a, b) =>
            {
                var uia = a as UIPanel_TechnologyItem;
                var uib = b as UIPanel_TechnologyItem;
                return uia.tecId.CompareTo(uib.tecId);
            });

            var childs = tecnologyList.Children;
            foreach ( var child in childs)
            {
                //UnicornTweenHelper.SetEaseAlphaAndPosB2U(child.GetFromReference(UIPanel_TechnologyItem.KBtn_Refresh),0);
                UnicornTweenHelper.SetBookmarkSwing(child.GetFromReference(UIPanel_TechnologyItem.KBtn_Item).GetComponent<RectTransform>(),1,5,0.8f);
                child.GetFromReference(UIPanel_TechnologyItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 0;
                child.GetFromReference(UIPanel_TechnologyItem.KBtn_Item).GetComponent<CanvasGroup>().DOFade(1f, 0.5f).SetEase(Ease.InQuad).SetUpdate(true);
                await UniTask.Delay(50,true);
            }
            await UniTask.Delay(100, true);
            foreach (var child in childs)
            {
                UnicornTweenHelper.SetEaseAlphaAndPosB2U(child.GetFromReference(UIPanel_TechnologyItem.KBtn_Refresh), 0);
            }

            }
    }
}