//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Main;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_BattleInfo)]
    internal sealed class UIPanel_BattleInfoEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_BattleInfo;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_BattleInfo>();
        }
    }

    public partial class UIPanel_BattleInfo : UI, IAwake
    {
        private Tbskill tbskill;
        private Tbequip_data tbequip_data;
        private Tblanguage tblanguage;
        private Tbskill_binding tbskill_binding;
        private Tbskill_binding_rank tbskill_binding_rank;
        private Tbskill_quality tbskill_quality;
        private Tbbattletech tbbattletech;

        private EntityQuery playerQuery;
        private EntityManager entityManager;

        private UI panelRunTimeHUD = null;

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var parent))
            {
                panelRunTimeHUD = parent;
            }

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));

            JiYuUIHelper.StartStopTime(false);
            InitJson();
            InitNode();
        }

        void InitJson()
        {
            tbequip_data = ConfigManager.Instance.Tables.Tbequip_data;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbskill_binding = ConfigManager.Instance.Tables.Tbskill_binding;
            tbskill = ConfigManager.Instance.Tables.Tbskill;
            tbskill_binding_rank = ConfigManager.Instance.Tables.Tbskill_binding_rank;
            tbskill_quality = ConfigManager.Instance.Tables.Tbskill_quality;
            tbbattletech = ConfigManager.Instance.Tables.Tbbattletech;
        }

        void InitNode()
        {
            var KText_SumMoney = GetFromReference(UIPanel_BattleInfo.KText_SumMoney);
            var KText_KillEnemy = GetFromReference(UIPanel_BattleInfo.KText_KillEnemy);
            var KText_Hp = GetFromReference(UIPanel_BattleInfo.KText_Hp);
            var KContainer_Binding = GetFromReference(UIPanel_BattleInfo.KContainer_Binding);
            var KImg_CurWeapon = GetFromReference(UIPanel_BattleInfo.KImg_CurWeapon);

            var KText_WeaponSkillDes = GetFromReference(UIPanel_BattleInfo.KText_WeaponSkillDes);
            var KBtn_Continue = GetFromReference(UIPanel_BattleInfo.KBtn_Continue);
            var KText_Continue = GetFromReference(UIPanel_BattleInfo.KText_Continue);
            var KBtn_Home = GetFromReference(UIPanel_BattleInfo.KBtn_Home);
            var KBtn_Volume = GetFromReference(UIPanel_BattleInfo.KBtn_Volume);
            var KBtn_DamageInfo = GetFromReference(UIPanel_BattleInfo.KBtn_DamageInfo);
            var KBtn_TestArgs = GetFromReference(UIPanel_BattleInfo.KBtn_TestArgs);
            var KBtn_Mask = GetFromReference(UIPanel_BattleInfo.KBtn_Mask);
            var KContainer = GetFromReference(UIPanel_BattleInfo.KContainer);
            var KBottom = GetFromReference(UIPanel_BattleInfo.KBottom);
            var KBtn_VolumeIcon = GetFromReference(UIPanel_BattleInfo.KBtn_VolumeIcon);


            SetImgAndText();

            //var RenderHeight = KContainer_Top.GetRectTransform().Height() +
            //                   KContainer_Binding.GetRectTransform().Height() +
            //                   KContainer_Mid.GetRectTransform().Height() +
            //                   KBottom.GetRectTransform().Height();
            //var upOffset = (Screen.height - RenderHeight) / 2f;

            //KContainer.GetRectTransform().SetAnchoredPositionY(-upOffset);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Continue, () =>
            {
                JiYuUIHelper.StartStopTime(true);
                this.Close();
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Home,
                () => { UIHelper.CreateAsync(UIType.UIPanel_ReturnConfirm); });

            var volumeImg = ResourcesSingleton.Instance.settingData.EnableBgm ? "icon_voice" : "pic_setting_1_2";
            KBtn_VolumeIcon.GetImage().SetSpriteAsync(volumeImg, false);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Volume, () =>
            {
                ResourcesSingleton.Instance.settingData.EnableBgm =
                    !ResourcesSingleton.Instance.settingData.EnableBgm;
                AudioManager.Instance.SetFModBgmMute(!ResourcesSingleton.Instance.settingData.EnableBgm);
                AudioManager.Instance.SetFModSFXMute(!ResourcesSingleton.Instance.settingData.EnableBgm);
                var volumeImg = ResourcesSingleton.Instance.settingData.EnableBgm
                    ? "icon_voice"
                    : "pic_setting_1_2";
                KBtn_VolumeIcon.GetImage().SetSpriteAsync(volumeImg, false);
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_DamageInfo,
                () => { UIHelper.CreateAsync(UIType.UIPanel_BattleDamageInfo); });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_TestArgs,
                () => { UIHelper.CreateAsync(UIType.UIPanel_ParasTest); });
            KBtn_Mask.GetButton().OnClick.Add(() =>
            {
                JiYuUIHelper.DestoryAllTips();

                //if (KContainer_Selcted.GameObject.activeSelf)
                //{
                //    KContainer_Selcted.SetActive(false);
                //    DisableAllStageIsSelected();
                //}
            });
        }

        private void SetImgAndText()
        {
            var KText_SumMoney = GetFromReference(UIPanel_BattleInfo.KText_SumMoney);
            var KText_KillEnemy = GetFromReference(UIPanel_BattleInfo.KText_KillEnemy);
            var KText_Hp = GetFromReference(UIPanel_BattleInfo.KText_Hp);
            var KContainer_Binding = GetFromReference(UIPanel_BattleInfo.KContainer_Binding);
            var KImg_CurWeapon = GetFromReference(UIPanel_BattleInfo.KImg_CurWeapon);

            var KText_WeaponSkillDes = GetFromReference(UIPanel_BattleInfo.KText_WeaponSkillDes);
            var KBtn_Continue = GetFromReference(UIPanel_BattleInfo.KBtn_Continue);
            var KText_Continue = GetFromReference(UIPanel_BattleInfo.KText_Continue);
            var KBtn_Home = GetFromReference(UIPanel_BattleInfo.KBtn_Home);
            var KBtn_Volume = GetFromReference(UIPanel_BattleInfo.KBtn_Volume);
            var KBtn_DamageInfo = GetFromReference(UIPanel_BattleInfo.KBtn_DamageInfo);
            var KBtn_TestArgs = GetFromReference(UIPanel_BattleInfo.KBtn_TestArgs);
            var KBtn_Mask = GetFromReference(UIPanel_BattleInfo.KBtn_Mask);
            var KContainer = GetFromReference(UIPanel_BattleInfo.KContainer);
            var KBottom = GetFromReference(UIPanel_BattleInfo.KBottom);
            var KBtn_VolumeIcon = GetFromReference(UIPanel_BattleInfo.KBtn_VolumeIcon);
            var KText_DamagTittle = GetFromReference(UIPanel_BattleInfo.KText_DamagTittle);
            var KText_TecTittle = GetFromReference(UIPanel_BattleInfo.KText_TecTittle);

            var KText_TittleWeapon = GetFromReference(UIPanel_BattleInfo.KText_TittleWeapon);
            var KText_SumDamge = GetFromReference(UIPanel_BattleInfo.KText_SumDamge);
            var playerData = GetPlayerData();

            KText_DamagTittle.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_statistics_title").current);
            KText_TecTittle.GetTextMeshPro().SetTMPText(tblanguage.Get("battletech_learnt").current);
            KText_TittleWeapon.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_weapon_title").current);

            KText_SumMoney.GetTextMeshPro().SetTMPText(JiYuUIHelper.GetRewardTextIconName("icon_battle_") +
                                                       playerData.playerData.exp.ToString());
            KText_KillEnemy.GetTextMeshPro().SetTMPText(JiYuUIHelper.GetRewardTextIconName("icon_battle_kill_num") +
                                                        playerData.playerData.killEnemy.ToString());
            KText_Hp.GetTextMeshPro().SetTMPText(JiYuUIHelper.GetRewardTextIconName("icon_item_1010002") +
                                                 JiYuUIHelper.GetRewardCount(new Vector3(5, 1010002, 0)).ToString());
            JiYuUIHelper.ForceRefreshLayout(GetFromReference(KContainer_PropBar));

            CreateBindingItem().Forget();


            var equipid = playerData.playerOtherData.weaponId;
            KImg_CurWeapon.GetImage().SetSpriteAsync(tbequip_data.Get(equipid).icon, false);
            var weaponSkillID = playerData.playerOtherData.weaponSkillId;
            Log.Debug($"weaponSkillID:{weaponSkillID}");
            if (weaponSkillID == 0)
            {
                this.GetFromReference(KSkillIconGet).SetActive(false);
                this.GetFromReference(KSkillIconNot).SetActive(true);
            }
            else
            {
                var weaponSkill = tbskill.Get(weaponSkillID);
                if (weaponSkill.desc.Contains("desc"))
                {
                    KText_WeaponSkillDes.GetTextMeshPro().SetTMPText(tblanguage.Get(weaponSkill.desc).current);
                }


                if (weaponSkill.icon.Contains("icon"))
                {
                    this.GetFromReference(KSkillIconGet).SetActive(true);
                    this.GetFromReference(KSkillIconNot).SetActive(false);
                    this.GetImage(KImgSkillIcon).SetSpriteAsync(weaponSkill.icon, false);
                    this.GetTextMeshPro(KText_SkillName).SetTMPText(tblanguage.Get(weaponSkill.name).current);
                }
                else
                {
                    this.GetFromReference(KSkillIconGet).SetActive(false);
                    this.GetFromReference(KSkillIconNot).SetActive(true);
                }
            }

            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            var bindingDic = parentUI.bindingsDic;
            bool isHaveBinding = false;
            foreach (var binding in bindingDic)
            {
                if (binding.Value != 0)
                {
                    isHaveBinding = true;
                    break;
                }
            }

            if (isHaveBinding)
            {
                this.GetFromReference(KImg_Binding).SetActive(true);
                var sortedDict = bindingDic.OrderByDescending(x => x.Value);
                var maxID = sortedDict.First().Key;
                this.GetTextMeshPro(KText_BindingDes)
                    ?.SetTMPText(tblanguage.Get(tbskill_binding.Get(maxID).desc).current);
                this.GetImage(KImg_Binding)?.SetSpriteAsync(tbskill_binding.Get(maxID).pic, false);
            }
            else
            {
                this.GetFromReference(KImg_Binding).SetActive(false);
                this.GetTextMeshPro(KText_BindingDes)?.SetTMPText(tblanguage.Get("battle_binding_empty").current);
            }


            var sumDamage = playerData.playerOtherData.playerDamageSumInfo.weaponDamage +
                            playerData.playerOtherData.playerDamageSumInfo.collideDamage +
                            playerData.playerOtherData.playerDamageSumInfo.areaDamage;
            sumDamage = Mathf.Max(0, sumDamage);
            KText_SumDamge.GetTextMeshPro()
                .SetTMPText(tblanguage.Get("battle_statistics_damage").current + sumDamage.ToString());

            CreateDamageBar(sumDamage).Forget();
            KText_Continue.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_continuebattle").current);
            var tecs = parentUI.displaySelectedTechs;
            GetFromReference(KTec1).SetActive(false);
            GetFromReference(KTec2).SetActive(false);
            GetFromReference(KTec3).SetActive(false);
            if (tecs.Count > 0)
            {
                if (tecs.Count == 1)
                {
                    GetFromReference(KTec1).SetActive(true);
                }
                else if (tecs.Count == 2)
                {
                    GetFromReference(KTec1).SetActive(true);
                    GetFromReference(KTec2).SetActive(true);
                }
                else
                {
                    GetFromReference(KTec1).SetActive(true);
                    GetFromReference(KTec2).SetActive(true);
                    GetFromReference(KTec3).SetActive(true);
                }
            }

            for (int i = 0; i < tecs.Count; i++)
            {
                var techID = tecs[i];
                var iconTec = tbbattletech.Get(techID).icon;
                var strTec = tbbattletech.Get(techID).name;
                var tecImg = "ImgTec" + (i + 1).ToString();
                this.GetImage(tecImg).SetSprite(iconTec, false);
                var tecText = "TextTec" + (i + 1).ToString();
                this.GetTextMeshPro(tecText).SetTMPText(tblanguage.Get(strTec).current);
            }
        }

        public float DivideToPercentage(double numerator, double denominator)
        {
            if (denominator == 0)
            {
                return 0; // ±ÜÃâ³ýÒÔÁã
            }

            int percentage = (int)Math.Round((numerator / denominator) * 100);
            return percentage;
        }

        private async UniTask CreateDamageBar(float sumDamage)
        {
            var playerData = GetPlayerData();

            int count = 3;

            var damageDic = new Dictionary<int, float>()
            {
                { 1, playerData.playerOtherData.playerDamageSumInfo.weaponDamage },
                { 2, playerData.playerOtherData.playerDamageSumInfo.collideDamage },
                { 3, playerData.playerOtherData.playerDamageSumInfo.areaDamage }
            };
            var sortedDict = damageDic.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            //var damageList = new List<float>(){ playerData.playerOtherData.playerDamageSumInfo.weaponDamage, playerData.playerOtherData.playerDamageSumInfo.collideDamage, playerData.playerOtherData.playerDamageSumInfo.areaDamage };
            float lastValue = 0, midValue = 0, maxValue = 0;
            if (sumDamage > 0)
            {
                lastValue = sortedDict[sortedDict.Keys.Last()] = math.ceil(sortedDict.Values.Last() / sumDamage * 100);
                midValue = sortedDict[sortedDict.Keys.ToList()[1]] =
                    math.ceil(sortedDict.Values.ToList()[1] / sumDamage * 100);
                maxValue = sortedDict[sortedDict.Keys.First()] = 100 - lastValue - midValue;
            }


            var container = GetFromReference(KBarContainer);
            container.GetRectTransform().GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>()
                .SetTMPText(tblanguage.Get("battle_statistics_name_1").current);
            container.GetRectTransform().GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>()
                .SetTMPText(tblanguage.Get("battle_statistics_name_2").current);
            container.GetRectTransform().GetChild(2).GetChild(0).GetChild(1).GetComponent<TMP_Text>()
                .SetTMPText(tblanguage.Get("battle_statistics_name_3").current);
            var text1 = container.GetRectTransform().GetChild(0).GetChild(0).GetChild(1).GetChild(0);
            var textP1 = container.GetRectTransform().GetChild(0).GetChild(1).GetChild(0);

            var text2 = container.GetRectTransform().GetChild(1).GetChild(0).GetChild(1).GetChild(0);
            var textP2 = container.GetRectTransform().GetChild(1).GetChild(1).GetChild(0);

            var text3 = container.GetRectTransform().GetChild(2).GetChild(0).GetChild(1).GetChild(0);
            var textP3 = container.GetRectTransform().GetChild(2).GetChild(1).GetChild(0);
            int index = 0;
            foreach (var damage in sortedDict)
            {
                index++;
                var strCircle = "";
                if (index == 1)
                {
                    strCircle = KCircleMax;
                    GetFromReference(strCircle)?.GetImage().SetFillAmount((maxValue - 1f) / 100f);
                    ;
                }
                else if (index == 2)
                {
                    strCircle = KCircleMid;
                    GetFromReference(strCircle)?.GetImage().SetFillAmount((midValue - 1f) / 100f);
                    GetFromReference(strCircle)?.GetRectTransform().SetLocalEulerAnglesZ(360 * (midValue / 100f));
                }
                else
                {
                    strCircle = KCircleMin;
                    GetFromReference(strCircle)?.GetImage().SetFillAmount((lastValue - 1f) / 100f);
                    GetFromReference(strCircle)?.GetRectTransform().SetLocalEulerAnglesZ(360 * (lastValue / 100f));
                }

                switch (damage.Key)
                {
                    case 1:
                        GetFromReference(strCircle).GetImage().SetColor("CB2449");
                        text1.GetComponent<TMP_Text>()
                            .SetTMPText($"({playerData.playerOtherData.playerDamageSumInfo.weaponDamage.ToString()})");
                        textP1.GetComponent<TMP_Text>().SetTMPText($"{damage.Value.ToString()}%");
                        break;
                    case 2:
                        GetFromReference(strCircle).GetImage().SetColor("F4704A");
                        text2.GetComponent<TMP_Text>()
                            .SetTMPText($"({playerData.playerOtherData.playerDamageSumInfo.collideDamage.ToString()})");
                        textP2.GetComponent<TMP_Text>().SetTMPText($"{damage.Value.ToString()}%");
                        break;
                    case 3:
                        GetFromReference(strCircle).GetImage().SetColor("72AF1C");
                        text3.GetComponent<TMP_Text>()
                            .SetTMPText($"({playerData.playerOtherData.playerDamageSumInfo.areaDamage.ToString()})");
                        textP3.GetComponent<TMP_Text>().SetTMPText($"{damage.Value.ToString()}%");
                        break;
                }

                if (lastValue == 1 && midValue == 1)
                {
                    GetFromReference(KCircleMax).GetImage().SetFillAmount(0.95f);
                    GetFromReference(KCircleMid).GetImage().SetFillAmount(0.01f);
                    GetFromReference(KCircleMin).GetImage().SetFillAmount(0.01f);
                    GetFromReference(KCircleMid)?.GetRectTransform().SetLocalEulerAnglesZ(7.2f);
                    GetFromReference(KCircleMin)?.GetRectTransform().SetLocalEulerAnglesZ(7.2f);
                }
                else if (lastValue == 1 && midValue != 1)
                {
                    GetFromReference(KCircleMax).GetImage().SetFillAmount((maxValue - 2) / 100f);
                    GetFromReference(KCircleMid).GetImage().SetFillAmount((midValue - 1) / 100f);
                    GetFromReference(KCircleMin).GetImage().SetFillAmount(0.01f);
                    GetFromReference(KCircleMid)?.GetRectTransform().SetLocalEulerAnglesZ(360 * (midValue / 100f));
                    GetFromReference(KCircleMin)?.GetRectTransform().SetLocalEulerAnglesZ(7.2f);
                }
                else if (lastValue == midValue && lastValue == 0)
                {
                    GetFromReference(KCircleMax).GetImage().SetFillAmount(maxValue / 100f);
                }

                JiYuUIHelper.ForceRefreshLayout(GetFromReference(KLeft1));
                JiYuUIHelper.ForceRefreshLayout(GetFromReference(KLeft2));
                JiYuUIHelper.ForceRefreshLayout(GetFromReference(KLeft3));
            }


            //for (int i = 0; i < count; i++)
            //{
            //    var index = i;
            //    var ui =
            //        await list.CreateWithUITypeAsync(UIType.UISubPanel_DamageInfoItem, index, false) as
            //            UISubPanel_DamageInfoItem;
            //    var KImg_WeaponIcon = ui.GetFromReference(UISubPanel_DamageInfoItem.KImg_WeaponIcon);
            //    var KText_Name = ui.GetFromReference(UISubPanel_DamageInfoItem.KText_Name);
            //    var KText_Num = ui.GetFromReference(UISubPanel_DamageInfoItem.KText_Num);
            //    var KImg_Filled = ui.GetFromReference(UISubPanel_DamageInfoItem.KImg_Filled);
            //    var KText_Ratios = ui.GetFromReference(UISubPanel_DamageInfoItem.KText_Ratios);

            //    float ratios = default;
            //    switch (index)
            //    {
            //        case 0:

            //            ratios = playerData.playerOtherData.playerDamageSumInfo.weaponDamage / sumDamage;
            //            KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_statistics_name_1").current);
            //            KImg_WeaponIcon.GetImage().SetSpriteAsync(equip_data.icon, false);
            //            KText_Num.GetTextMeshPro()
            //                .SetTMPText(playerData.playerOtherData.playerDamageSumInfo.weaponDamage.ToString());
            //            KImg_Filled.GetImage()
            //                .SetFillAmount(ratios);
            //            KText_Ratios.GetTextMeshPro()
            //                .SetTMPText($"{Mathf.FloorToInt(ratios * 100)}%");
            //            break;
            //        case 1:
            //            ratios = playerData.playerOtherData.playerDamageSumInfo.collideDamage / sumDamage;
            //            KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_statistics_name_2").current);
            //            KImg_WeaponIcon.GetImage().SetSpriteAsync("pic_collide_damage", false);
            //            KText_Num.GetTextMeshPro()
            //                .SetTMPText(playerData.playerOtherData.playerDamageSumInfo.collideDamage.ToString());
            //            KImg_Filled.GetImage()
            //                .SetFillAmount(ratios);
            //            KText_Ratios.GetTextMeshPro()
            //                .SetTMPText($"{Mathf.FloorToInt(ratios * 100)}%");
            //            break;
            //        case 2:
            //            ratios = playerData.playerOtherData.playerDamageSumInfo.areaDamage / sumDamage;
            //            KText_Name.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_statistics_name_3").current);
            //            KImg_WeaponIcon.GetImage().SetSpriteAsync("pic_area_damage", false);
            //            KText_Num.GetTextMeshPro()
            //                .SetTMPText(playerData.playerOtherData.playerDamageSumInfo.areaDamage.ToString());
            //            KImg_Filled.GetImage()
            //                .SetFillAmount(ratios);
            //            KText_Ratios.GetTextMeshPro()
            //                .SetTMPText($"{Mathf.FloorToInt(ratios * 100)}%");

            //            break;
            //    }

            //    if (ratios >= 0 && ratios < 1f / count)
            //    {
            //        KImg_Filled.GetImage().SetColor("2E88F6");
            //    }
            //    else if (ratios >= 1f / count && ratios < (1f / count) * 2)
            //    {
            //        KImg_Filled.GetImage().SetColor("7167FF");
            //    }
            //    else
            //    {
            //        KImg_Filled.GetImage().SetColor("EF4444");
            //    }
            //}

            //list.Sort((a, b) =>
            //{
            //    var uia = a as UISubPanel_DamageInfoItem;
            //    var uib = b as UISubPanel_DamageInfoItem;
            //    return uia.index.CompareTo(uib.index);
            //});
        }

        PlayerData GetPlayerData()
        {
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            return playerData;
        }

        private async UniTaskVoid CreateBindingItem()
        {
            var KContainer_Binding = GetFromReference(UIPanel_BattleInfo.KContainer_Binding);

            var bindingList = KContainer_Binding.GetList();
            bindingList.Clear();
            var tbBindingList = tbskill_binding.DataList;

            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            List<int> bindingNums = new List<int> { 0, 0, 0, 0, 0 };
            foreach (var skillkv in parentUI.skillsDic)
            {
                var skillId = skillkv.Key;
                var skillLevel = skillkv.Value;

                var curSkill = tbskill.Get(skillId + skillLevel - 1);

                //var playerSkillGroup = player_skill.Get(skillId);
                var bindingId = curSkill.skillBindingId[0];
                Log.Debug($"skillbindingid:{bindingId}");
                bindingNums[bindingId]++;
            }

            for (int i = 1; i < bindingNums.Count; i++)
            {
                var key = "Text" + i.ToString();
                GetFromReference(key).GetTextMeshPro().SetTMPText(bindingNums[i].ToString());
            }
        }

        private async UniTaskVoid OnClickBindings(int bindId)
        {
            // this.GetFromReference(UIPanel_BattleInfo.KContainer_Skill)?.SetActive(false);
            // this.GetFromReference(UIPanel_BattleInfo.KBottom)?.SetActive(false);

            var playerSkillBinding = tbskill_binding.Get(bindId);
            var KImg_Icon_Binding = GetFromReference(UIPanel_BattleShop.KImg_Icon_Binding);
            var KTxt_Name_Binding = GetFromReference(UIPanel_BattleShop.KTxt_Name_Binding);
            var KTxt_Description_Binding = GetFromReference(UIPanel_BattleShop.KTxt_Description_Binding);

            KImg_Icon_Binding.GetImage().SetSpriteAsync(playerSkillBinding.pic, false).Forget();
            KTxt_Name_Binding.GetTextMeshPro().SetTMPText(tblanguage.Get(playerSkillBinding.name).current);

            KTxt_Description_Binding.GetTextMeshPro().SetTMPText(tblanguage.Get(playerSkillBinding.desc).current);
            var KContainer_SelectedItem = GetFromReference(UIPanel_BattleShop.KContainer_SelectedItem);
            var scrollRect = KContainer_SelectedItem.GetScrollRect();

            const float SelectedItemInterval = 10f;
            scrollRect.Content.GetRectTransform().SetWidth(Screen.width);
            var gridLayoutGroup = scrollRect.Content.GetComponent<GridLayoutGroup>();
            //var cellsezeX = (Screen.width - (3 * SelectedItemInterval)) / 2f;
            //gridLayoutGroup.cellSize = new Vector2(cellsezeX, gridLayoutGroup.cellSize.y);

            var list = scrollRect.Content.GetList();
            list.Clear();
            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            foreach (var skillkv in parentUI.skillsDic)
            {
                var skillId = skillkv.Key;
                var skillLevel = skillkv.Value;

                var curSkill = tbskill.Get(skillId + skillLevel - 1);

                //var playerSkillGroup = player_skill.Get(skillId);
                var bindingId = curSkill.skillBindingId[0];
                if (bindingId != bindId)
                {
                    continue;
                }

                //var playerSkill = player_skill.Get(playerSkillGroup.default0);
                var playerSkillQuality = tbskill_quality.Get(curSkill.skillQualityId);
                //var playerSkillBinding = player_skill_binding.Get(bindingId);

                var ui =
                    await list.CreateWithUITypeAsync(UIType.UIPanel_SelectItem, skillId, false) as
                        UIPanel_SelectItem;
                var KImg_bg_quality = ui.GetFromReference(UIPanel_SelectItem.KImg_bg_quality);
                var KImg_icon_skill = ui.GetFromReference(UIPanel_SelectItem.KImg_icon_skill);
                var KTxt_name_skill = ui.GetFromReference(UIPanel_SelectItem.KTxt_name_skill);
                var KTxt_Descripiton = ui.GetFromReference(UIPanel_SelectItem.KTxt_Descripiton);
                var KContainer_Stars = ui.GetFromReference(UIPanel_SelectItem.KContainer_Stars);
                //TODO:

                var qulityStr = playerSkillQuality.pic + "_tech";

                KImg_bg_quality.GetImage().SetSpriteAsync(playerSkillQuality.pic, false).Forget();

                KImg_icon_skill.GetImage().SetSpriteAsync(curSkill.icon, false).Forget();
                KTxt_name_skill.GetTextMeshPro().SetTMPText(tblanguage.Get(curSkill.name).current);
                string desc = string.Format(tblanguage.Get(curSkill.desc).current,
                    curSkill.descPara.ToArray());

                KTxt_Descripiton.GetTextMeshPro().SetTMPText(desc);

                var itemList = KContainer_Stars.GetList();
                SetStars(itemList, playerSkillQuality.levelUpperlimit, skillLevel);
            }

            list.Sort((a, b) =>
            {
                var uia = a as UIPanel_SelectItem;
                var uib = b as UIPanel_SelectItem;

                var playerSkilla = tbskill.Get(uia.skillId);
                var playerSkillb = tbskill.Get(uib.skillId);
                return playerSkillb.skillQualityId.CompareTo(playerSkilla.skillQualityId);
            });
            JiYuUIHelper.ForceRefreshLayout(scrollRect.Content);
        }

        void SetStars(UIListComponent itemList, int levelUpperlimit, int currentLevel)
        {
            const int MaxStars = 5;
            for (int j = 0; j < MaxStars; j++)
            {
                var index = j;
                Transform child = itemList.Get().GetChild(index);
                var uiItem = itemList.Create(child.gameObject, true);
                uiItem.SetActive(false);
            }

            for (int j = 0; j < levelUpperlimit; j++)
            {
                var index = j;
                Transform child = itemList.Get().GetChild(index);
                var uiItem = itemList.Create(child.gameObject, true);
                uiItem.SetActive(true);
                if (index <= currentLevel - 1)
                {
                    uiItem.GetImage().SetSpriteAsync("icon_whiteStar", false).Forget();
                }
                else
                {
                    uiItem.GetImage().SetSpriteAsync("icon_hollowStar", false).Forget();
                }
            }
        }

        void DisableAllStageIsSelected()
        {
            var KContainer_Binding = GetFromReference(UIPanel_BattleInfo.KContainer_Binding);
            var list = KContainer_Binding.GetList();
            int i = 0;
            foreach (var ui in list.Children)
            {
                i++;
                var uiS = ui as UIPanel_BindingItem;
                var isSelected = uiS.GetFromReference(UIPanel_BindingItem.KBg_IsSelected);

                isSelected.SetActive(false);
                var strImg = tbskill_binding.Get(i).pic + "_outline";
                uiS.GetFromReference(UIPanel_BindingItem.KImg_BindingIcon).GetImage().SetSpriteAsync(strImg, false)
                    .Forget();
            }
        }

        void SetStagsColor(int exp, UIListComponent itemList)
        {
            Transform child0 = itemList.Get().GetChild(0);
            Transform child1 = itemList.Get().GetChild(1);
            Transform child2 = itemList.Get().GetChild(2);
            Transform child3 = itemList.Get().GetChild(3);
            var uiItem0 = itemList.Create(child0.gameObject, true).GetImage();
            var uiItem1 = itemList.Create(child1.gameObject, true).GetImage();
            var uiItem2 = itemList.Create(child2.gameObject, true).GetImage();
            var uiItem3 = itemList.Create(child3.gameObject, true).GetImage();
            //5E6B68 466B9E 815586 9E7749 975950

            uiItem0.SetColor("5E6B68");
            uiItem1.SetColor("5E6B68");
            uiItem2.SetColor("5E6B68");
            uiItem3.SetColor("5E6B68");

            var index = GetBindingIndex(exp);

            if (index == -1)
            {
                uiItem0.SetColor("975950");
                uiItem1.SetColor("975950");
                uiItem2.SetColor("975950");
                uiItem3.SetColor("975950");
                return;
            }

            switch (index - 1)
            {
                case 0:

                    break;
                case 1:
                    uiItem0.SetColor("466B9E");

                    break;
                case 2:
                    uiItem0.SetColor("815586");
                    uiItem1.SetColor("815586");

                    break;
                case 3:
                    uiItem0.SetColor("9E7749");
                    uiItem1.SetColor("9E7749");
                    uiItem2.SetColor("9E7749");
                    break;
            }
        }

        int GetBindingIndex(int exp)
        {
            int index = -1;
            foreach (var item in tbskill_binding_rank.DataList)
            {
                if (exp < item.exp)
                {
                    index = item.id;
                    break;
                }
            }

            return index;
        }


        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}