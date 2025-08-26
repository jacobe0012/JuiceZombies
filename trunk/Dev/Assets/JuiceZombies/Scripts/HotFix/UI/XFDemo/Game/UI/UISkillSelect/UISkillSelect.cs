// //---------------------------------------------------------------------
// // UnicornStudio
// // Author: xxx
// // Time: #CreateTime#
// //---------------------------------------------------------------------
//
// using cfg.config;
// using HotFix_UI;
// using Main;
// using ProtoBuf;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using TMPro;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Entities.UniversalDelegates;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.UI;
// using static UnityEngine.GraphicsBuffer;
//
// namespace XFramework
// {
//     [UIEvent(UIType.UISkillSelect)]
//     internal sealed class UISkillSelectEvent : AUIEvent, IUILayer
//     {
//         public override string Key => UIPathSet.UISkillSelect;
//
//         public override bool IsFromPool => true;
//
//         public override bool AllowManagement => true;
//
//         public UILayer Layer => UILayer.Mid;
//
//         public override UI OnCreate()
//         {
//             return UI.Create<UISkillSelect>();
//         }
//     }
//
//     public partial class UISkillSelect : UI, IAwake
//     {
//         private long timerId;
//
//         public float waitTime = 1f;
//         float elapseTime = 0f;
//         private EntityManager manager;
//         private EntityQuery query;
//
//         private DynamicBuffer<Skill> skills;
//         private DynamicBuffer<Buff> buffs;
//         private Entity player;
//
//         /// <summary>
//         /// ��ѡ��ļ�����
//         /// </summary>
//         private List<int> availableSkills;
//
//         /// <summary>
//         /// ��ѡ��ļ�����
//         /// </summary>
//         private List<int> selectedSkills;
//
//         private List<UISkillItem> skillItems;
//
//         int activeSkillCount;
//         int passiveSkillCount;
//         string avaliableCount = "availableCount";
//
//         int weaponSkillID = -1;
//
//         public void Initialize()
//         {
//             var languageTabel = ConfigManager.Instance.Tables.Tblanguage;
//             GetFromReference(KText_Refresh)?.GetTextMeshPro()
//                 .SetTMPText(languageTabel.Get("battle_button_refresh").current);
//             GetFromReference(KTxt_select)?.SetActive(true);
//             this.GetTextMeshPro(KTxt_select).SetTMPText(languageTabel.Get("battle_skill_select_text").current);
//             GetFromReference(KBtn_Refresh)?.SetActive(false);
//             skillItems = new List<UISkillItem>(3);
//             manager = World.DefaultGameObjectInjectionWorld.EntityManager;
//             query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GameRandomData));
//             player = manager.CreateEntityQuery(typeof(PlayerData)).ToEntityArray(Allocator.Temp)[0];
//             if (manager.HasBuffer<Skill>(player))
//             {
//                 skills = manager.GetBuffer<Skill>(player);
//             }
//
//             if (manager.HasBuffer<Buff>(player))
//             {
//                 buffs = manager.GetBuffer<Buff>(player);
//             }
//
//             var playerProp = manager.GetComponentData<PlayerData>(player);
//             if (playerProp.playerData.skillRefreshCount > 0)
//             {
//                 GetFromReference(KTxt_select)?.SetActive(false);
//                 GetFromReference(KBtn_Refresh)?.SetActive(true);
//                 this.GetButton(KBtn_Refresh)?.OnClick.Add(() => RefreshAvailableSkill(availableSkills));
//             }
//
//             //GetFromReference(KBtn_Refresh)?.SetActive(true);
//             //this.GetButton(KBtn_Refresh)?.OnClick.Add(() => RefreshAvailableSkill(availableSkills));
//             UnityHelper.StopTime();
//
//             var timerMgr = TimerManager.Instance;
//             timerId = timerMgr.RepeatedFrameTimer(this.Update);
//
//
//             GetFromReference(KTitleText).GetComponent<TMP_Text>()
//                 .SetText(languageTabel["battle_skill_select_title"].current);
//
//
//             var skillGroupTable = ConfigManager.Instance.Tables.Tbplayer_skill_group.DataMap;
//             var skillTable = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
//             //HidSupplyImage();
//             selectedSkills = new List<int>(20);
//             availableSkills = new List<int>(20);
//
//
//             if (PlayerPrefs.GetInt("isInit") == 1)
//             {
//                 int avaliableNum = PlayerPrefs.GetInt(avaliableCount);
//                 for (int i = 0; i < avaliableNum; i++)
//                 {
//                     string skillKey = "availableSkill" + (i + 1).ToString();
//                     if (PlayerPrefs.GetInt(skillKey) != 0)
//                     {
//                         Log.Debug(
//                             $"availableSkill--------------skillKey:{skillKey},skillValue{PlayerPrefs.GetInt(skillKey)}",
//                             Color.blue);
//                         availableSkills.Add(PlayerPrefs.GetInt(skillKey));
//                     }
//                 }
//
//                 for (int i = 0; i < selectedSkills.Capacity; i++)
//                 {
//                     string skillKey = "selectSkill" + (i + 1).ToString();
//                     if (PlayerPrefs.GetInt(skillKey) != 0)
//                     {
//                         selectedSkills.Add(PlayerPrefs.GetInt(skillKey));
//                         Log.Debug(
//                             $"selectedSkills-------------skillKey:{skillKey},skillValue{PlayerPrefs.GetInt(skillKey)}",
//                             Color.blue);
//                     }
//                 }
//             }
//             else
//             {
//                 HidSupplyImage();
//                 activeSkillCount = 0;
//                 passiveSkillCount = 0;
//
//                 GetSkillsID();
//                 //��ʼ�����ܳ�
//
//                 PlayerPrefs.SetInt("weaponSkillID", weaponSkillID);
//
//                 var skillList = ConfigManager.Instance.Tables.Tbplayer_skill.DataList;
//
//                 for (int i = 0; i < skillList.Count; i++)
//                 {
//                     if (skillList[i].type == 2 && skillList[i].level == 1)
//                     {
//                         availableSkills.Add(skillList[i].id);
//                     }
//                 }
//
//                 PlayerPrefs.SetInt(avaliableCount, availableSkills.Count);
//
//
//                 for (int i = 0; i < availableSkills.Count; i++)
//                 {
//                     string skillKey = "availableSkill" + (i + 1).ToString();
//                     PlayerPrefs.SetInt(skillKey, availableSkills[i]);
//                 }
//
//                 PlayerPrefs.SetInt("isInit", 1);
//             }
//
//             SetWeaponInfo();
//
//
//             //Log.Debug($"{weaponSkillID}6666666666", Color.blue);
//             SetSkillContainer(in availableSkills);
//         }
//
//         private void RefreshAvailableSkill(List<int> availableSkills)
//         {
//             int count = this.GetFromReference(KSkillContainer).GetRectTransform().ChildCount;
//             for (int i = 0; i < skillItems.Count; i++)
//             {
//                 skillItems[i].RemoveTimer();
//             }
//
//             for (int i = 0; i < count; i++)
//             {
//                 GameObject.Destroy(this.GetFromReference(KSkillContainer).GetRectTransform().GetChild(i).gameObject);
//             }
//
//             SetSkillContainer(in availableSkills);
//             var playerProp = manager.GetComponentData<PlayerData>(player);
//             playerProp.playerData.skillRefreshCount--;
//             manager.SetComponentData(player, playerProp);
//             if (playerProp.playerData.skillRefreshCount <= 0)
//             {
//                 GetFromReference(KBtn_Refresh)?.SetActive(false);
//                 GetFromReference(KTxt_select)?.SetActive(true);
//             }
//
//             Log.Debug($"{playerProp.playerData.skillRefreshCount}");
//         }
//
//         private int GetTotalWeight()
//         {
//             var userSkillTable = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
//             int totalWeight = 0;
//             for (int i = 0; i < availableSkills.Count; i++)
//             {
//                 int weight = userSkillTable[availableSkills[i]].power;
//                 totalWeight += weight;
//             }
//
//             return totalWeight;
//         }
//
//         private void SetSkillContainer(in List<int> availableSkills)
//         {
//             //ѡ��ļ���id-index
//             Dictionary<int, int> selectedIndex = new Dictionary<int, int>(3);
//             int weights = GetTotalWeight();
//             int count = 0;
//             int index = 0;
//             int needCount;
//             if (availableSkills.Count >= 3)
//             {
//                 needCount = 3;
//             }
//             else
//             {
//                 needCount = availableSkills.Count;
//             }
//
//             while (count < needCount)
//             {
//                 //ѡ��ɹ�
//                 if (SelectOneNormalSkill(availableSkills[index % availableSkills.Count], weights))
//                 {
//                     //id�Ƿ��ظ�
//                     if (selectedIndex.ContainsKey(availableSkills[index % availableSkills.Count]))
//                     {
//                         index++;
//                         continue;
//                     }
//
//                     selectedIndex.Add(availableSkills[index % availableSkills.Count], index % availableSkills.Count);
//                     count++;
//                 }
//
//                 index++;
//                 //selectedSkills.Add(availableSkills[index % availableSkills.Count]);
//             }
//
//             foreach (var item in selectedIndex)
//             {
//                 Log.Debug(item.Key.ToString(), Color.green);
//                 //selectedSkills.Add(item.Value);
//                 SetSkillContainerDetails(item.Key);
//             }
//         }
//
//         private void SetSkillContainerDetails(int value)
//         {
//             var parentUI = GetFromReference(KSkillContainer);
//             var item =
//                 UIHelper.Create<int>(UIType.UISkillItem, value, parentUI.GetComponent<Transform>()) as UISkillItem;
//             item.SetParent(this, false);
//             skillItems.Add(item);
//         }
//
//
//         public void SelectButtonClicked(int id)
//         {
//             Log.Debug($"�㵽��{id}");
//
//             var userSkillTable = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
//
//             //����ͼ��
//             bool isUpdateIcon = true;
//             if (PlayerPrefs.GetInt("passiveSkillCount") == 5 || PlayerPrefs.GetInt("activeSkillCount") == 5)
//             {
//                 isUpdateIcon = false;
//             }
//
//
//             for (int i = 0; i < selectedSkills.Count; i++)
//             {
//                 if (userSkillTable[selectedSkills[i]].group == userSkillTable[id].group)
//                 {
//                     isUpdateIcon = false;
//                     break;
//                 }
//             }
//
//             if (isUpdateIcon)
//             {
//                 int skillType = (int)userSkillTable[id].attribute[0].x;
//                 if (skillType == 1 || skillType == 2)
//                 {
//                     Log.Debug($"activeSkillCount:{PlayerPrefs.GetInt("activeSkillCount")}");
//                     int actIndex = PlayerPrefs.GetInt("activeSkillCount") + 1;
//                     string actKey = "activeSkillImage" + actIndex.ToString();
//                     //��������
//
//                     GetFromReference(actKey).GetComponent<Image>().sprite =
//                         ResourcesManager.Instance.Loader.LoadAsset<Sprite>(userSkillTable[id].pic);
//                     GetFromReference(actKey).GetImage().SetAlpha(1);
//                     //GetFromReference(actKey).GetComponent<Image>().SetAlpha(1f);
//                     PlayerPrefs.SetInt("activeSkillCount", actIndex);
//                 }
//                 else if (skillType == 3 || skillType == 4)
//                 {
//                     Log.Debug($"passiveSkillCount:{PlayerPrefs.GetInt("passiveSkillCount")}");
//                     int passiveIndex = PlayerPrefs.GetInt("passiveSkillCount") + 1;
//                     string passiveKey = "passiveSkillImage" + passiveIndex.ToString();
//                     //��������
//                     GetFromReference(passiveKey).GetComponent<Image>().sprite =
//                         ResourcesManager.Instance.Loader.LoadAsset<Sprite>(userSkillTable[id].pic);
//                     GetFromReference(passiveKey).GetImage().SetAlpha(1);
//                     // GetFromReference(passiveKey).GetComponent<Image>().SetAlpha(1f);
//                     Log.Debug($"{passiveIndex}");
//                     PlayerPrefs.SetInt("passiveSkillCount", passiveIndex);
//                 }
//             }
//
//
//             for (int i = 0; i < selectedSkills.Count; i++)
//             {
//                 if (userSkillTable[selectedSkills[i]].group == userSkillTable[id].group)
//                 {
//                     selectedSkills.Remove(selectedSkills[i]);
//                     break;
//                 }
//             }
//
//             //��Ӽ���
//             selectedSkills.Add(id);
//
//             for (int i = 0; i < selectedSkills.Count; i++)
//             {
//                 string skillKey = "selectSkill" + (i + 1).ToString();
//                 PlayerPrefs.SetInt(skillKey, selectedSkills[i]);
//             }
//
//
//             UpdatePlayerSkill(id);
//
//
//             //����ѡ����
//             UpadateAvailbaleSkills(ref availableSkills, id);
//
//             int count = this.GetFromReference(KSkillContainer).GetRectTransform().ChildCount;
//
//             for (int i = 0; i < skillItems.Count; i++)
//             {
//                 skillItems[i].RemoveTimer();
//             }
//
//             for (int i = 0; i < count; i++)
//             {
//                 GameObject.Destroy(this.GetFromReference(KSkillContainer).GetRectTransform().GetChild(i).gameObject);
//             }
//
//             if (UnicornUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
//             {
//                 var parent = ui as UIPanel_RunTimeHUD;
//                 parent.EnableInputBar();
//                 parent.StartTimer();
//                 parent.ReduceLevelUpPoint();
//                 UnityHelper.BeginTime();
//             }
//
//
//             Close();
//         }
//
//         private void UpdatePlayerSkill(int id)
//         {
//             var userSkillTable = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
//             var skillTable = ConfigManager.Instance.Tables.Tbskill.DataMap;
//             var skillEffectTable = ConfigManager.Instance.Tables.Tbskill_effect.DataMap;
//             int skillEffectID = skillTable[id].skillEffectArray[0];
//
//
//             //Log.Debug(buff1Args.ToString(), Color.green);
//             float coldDown = skillTable[id].cd / 1000f;
//
//
//             if (manager.HasBuffer<Skill>(player))
//             {
//                 skills = manager.GetBuffer<Skill>(player);
//             }
//
//             for (int i = 0; i < skills.Length; i++)
//             {
//                 if (!userSkillTable.ContainsKey(skills[i].Int32_0)) continue;
//                 if (userSkillTable[skills[i].Int32_0].group == userSkillTable[id].group)
//                 {
//                     var temp = skills[i];
//                     //temp.CurrentTypeId = (SkillOld.TypeId)id;
//                     temp.Int32_0 = id;
//                     temp.Int32_13 = 0;
//                     skills[i] = temp;
//                     return;
//                     //�滻
//                 }
//             }
//
//             if (id == 240201 || id == 220101 || id == 210101 || id == 210201 || id == 210301 || id == 210401 ||
//                 id == 210601 || id == 210701 || id == 220301)
//             {
//                 skills.Add(new SkillOld { CurrentTypeId = (SkillOld.TypeId)id, Entity_5 = player });
//
//
//                 //skills.Add(new Skill_240201
//                 //{
//                 //    caster = default,
//                 //    duration = coldDown,
//                 //    cooldown = coldDown,
//                 //    target = default,
//                 //    tick = 0,
//                 //    timeScale = 1,
//                 //    id = id,
//                 //    level = 1
//                 //}.ToSkillOld());
//             }
//
//             //���һ�����ܻ���buff
//             //else if (id == 220101)
//             //{
//             //    skills.Add(new SkillOld { CurrentTypeId = (SkillOld.TypeId)240201, Entity_5 = player });
//             //    //skills.Add(new Skill_220101
//             //    //{
//             //    //    caster = default,
//             //    //    duration = coldDown,
//             //    //    cooldown = coldDown,
//             //    //    target = default,
//             //    //    tick = 0,
//             //    //    timeScale = 1,
//             //    //    id = id,
//             //    //    level = 1
//             //    //}.ToSkillOld());
//             //}
//             //else if (id == 220201)
//             //{
//             //    skills.Add(new Skill_220201
//             //    {
//             //        caster = default,
//             //        duration = coldDown,
//             //        cooldown = coldDown,
//             //        target = default,
//             //        tick = 0,
//             //        timeScale = 1,
//             //        id = id,
//             //        level = 1
//             //    }.ToSkillOld());
//             //}
//             //else if (id == 210101)
//             //{
//             //    skills.Add(new Skill_210101
//             //    {
//             //        caster = default,
//             //        duration = coldDown,
//             //        cooldown = coldDown,
//             //        target = default,
//             //        tick = 0,
//             //        timeScale = 1,
//             //        id = id,
//             //        level = 1
//             //    }.ToSkillOld());
//             //}
//             //else if (id == 210201)
//             //{
//             //    skills.Add(new Skill_210201
//             //    {
//             //        caster = default,
//             //        duration = coldDown,
//             //        cooldown = coldDown,
//             //        target = default,
//             //        tick = 0,
//             //        timeScale = 1,
//             //        id = id,
//             //        level = 1
//             //    }.ToSkillOld());
//             //}
//
//
//             if (manager.HasBuffer<Buff>(player))
//             {
//                 buffs = manager.GetBuffer<Buff>(player);
//             }
//
//             for (int i = 0; i < buffs.Length; i++)
//             {
//                 if (buffs[i].Int32_0 < 1)
//                 {
//                     continue;
//                 }
//
//                 if (!userSkillTable.ContainsKey(buffs[i].Int32_0)) continue;
//                 if (userSkillTable[buffs[i].Int32_0].group == userSkillTable[id].group)
//                 {
//                     buffs.RemoveAt(i);
//                     buffs = manager.GetBuffer<Buff>(player);
//                 }
//             }
//
//             var group = userSkillTable[id].group;
//             if (group == 2001)
//             {
//                 int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//                 int buff2Args = skillEffectTable[skillEffectID].buff1Para[1];
//                 buffs.Add(new Buff_30000020
//                 {
//                     id = id,
//                     carrier = player,
//                     duration = -1,
//                     permanent = true,
//                     buffArgs = new BuffArgs
//                     {
//                         args0 = buff1Args,
//                         args1 = buff2Args,
//                         args2 = 0,
//                         args3 = 0,
//                         args4 = 0
//                     }
//                 }.ToBuffOld());
//             }
//
//
//             if (group == 2301)
//             {
//                 int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//                 buffs.Add(new Buff_10203020
//                 {
//                     id = id,
//                     carrier = player,
//                     duration = -1,
//                     permanent = true,
//                     buffArgs = new BuffArgs { args0 = buff1Args }
//                 }.ToBuffOld());
//             }
//
//             if (group == 2302)
//             {
//                 int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//                 buffs.Add(new Buff_10206220
//                 {
//                     id = id,
//                     carrier = player,
//                     duration = -1,
//                     permanent = true,
//                     buffArgs = new BuffArgs { args0 = buff1Args }
//                 }.ToBuffOld());
//             }
//
//             if (group == 2303)
//             {
//                 int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//                 buffs.Add(new Buff_10207020
//                 {
//                     id = id,
//                     carrier = player,
//                     duration = -1,
//                     permanent = true,
//                     buffArgs = new BuffArgs { args0 = buff1Args }
//                 }.ToBuffOld());
//             }
//
//             if (group == 2401)
//             {
//                 int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//                 buffs.Add(new Buff_20001002
//                 {
//                     id = id,
//                     carrier = player,
//                     duration = -1,
//                     permanent = true,
//                     buffArgs = new BuffArgs { args0 = buff1Args / 10000 }
//                 }.ToBuffOld());
//             }
//             //throw new NotImplementedException();
//         }
//
//         private void SetBlinkEffect(GameObject skill, int skillLevel)
//         {
//             skill.GetComponent<UnityEngine.Transform>().Find("StarGrop").GetChild(skillLevel - 1).GetComponent<Image>()
//                 .sprite = ResourcesManager.Instance.Loader.LoadAsset<Sprite>("star_normal");
//         }
//
//
//         private void Update()
//         {
//             elapseTime += Time.deltaTime;
//             if (elapseTime > waitTime)
//             {
//             }
//         }
//
//
//         bool SelectOneNormalSkill(int id, int totalWeight)
//         {
//             var userSkillTable = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
//             var rand = query.ToComponentDataArray<GameRandomData>(Allocator.Temp)[0];
//             int randomValue = rand.rand.NextInt(1, totalWeight + 1);
//
//             var entity = query.ToEntityArray(Allocator.Temp)[0];
//             World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<GameRandomData>(entity, rand);
//
//             // ������ɵ������С�ڵ��ڸ����ĸ��ʣ���ѡ��
//             if (randomValue <= userSkillTable[id].power)
//             {
//                 return true;
//             }
//
//             return false;
//         }
//
//         void SetWeaponInfo()
//         {
//             weaponSkillID = PlayerPrefs.GetInt("weaponSkillID");
//             Log.Debug($"weaponSkillID{weaponSkillID}", Color.yellow);
//             var skillsMap = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
//             // var equipMap = ConfigManager.Instance.Tables.Tbequip_data;
//             //��������������
//             var skillGroupTable = ConfigManager.Instance.Tables.Tbplayer_skill_group.DataMap;
//             var fillImage = GetFromReference(KWeaponRoot).GetRectTransform().Find("WeaponBar").Find("FillImage")
//                 .GetComponent<Image>();
//             float currentValue = 0f;
//             for (int i = 0; i < selectedSkills.Count; i++)
//             {
//                 if (skillsMap[selectedSkills[i]].attribute.Count > 0)
//                 {
//                     currentValue += (int)skillsMap[selectedSkills[i]].attribute[0].y;
//                 }
//                 else
//                     continue;
//             }
//
//             float current = currentValue / skillGroupTable[skillsMap[weaponSkillID].group].displayValue;
//             Log.Debug($"currentWeaponValue{currentValue}");
//             fillImage.fillAmount = current;
//
//             UpdateWeaponSkllLevel(currentValue);
//
//             var weapon = GetFromReference(KWeaponRoot).GetRectTransform().Find("Weapon").Find("WeaponImage")
//                 .GetComponent<Image>();
//             //��������ͼƬ
//             string weaponPicStr = skillsMap[weaponSkillID].pic;
//             weapon.sprite = ResourcesManager.Instance.Loader.LoadAsset<UnityEngine.Sprite>(weaponPicStr);
//         }
//
//         private void UpdateWeaponSkllLevel(float current)
//         {
//             int oldWeaponSkillID = weaponSkillID;
//             var skillMap = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
//             var skillDic = ConfigManager.Instance.Tables.Tbplayer_skill.DataList;
//             var skillGruopMap = ConfigManager.Instance.Tables.Tbplayer_skill_group.DataMap;
//             int initWeaponID = 0;
//             List<int> skillSList = new List<int>(5);
//             int skillGroup = skillMap[weaponSkillID].group;
//             for (int i = 0; i < skillDic.Count; i++)
//             {
//                 if (skillDic[i].group != skillGroup)
//                 {
//                     continue;
//                 }
//
//                 if (skillDic[i].level == 1)
//                 {
//                     initWeaponID = skillDic[i].id;
//                 }
//
//                 if (skillDic[i].level == 6)
//                 {
//                     skillSList.Add(skillDic[i].id);
//                 }
//             }
//
//             var stageList = skillGruopMap[skillMap[weaponSkillID].group].unlockValue;
//             int skillLevel = 1;
//             //int times = stageList.Count;
//             //1��
//             if (current < stageList[0])
//                 return;
//             if (current > stageList[0] && current < stageList[1])
//             {
//                 weaponSkillID = initWeaponID + 1;
//                 skillLevel += 1;
//                 Log.Debug($"weaponSkillID{weaponSkillID}");
//             }
//
//             if (current > stageList[1] && current < stageList[2])
//             {
//                 weaponSkillID = initWeaponID + 2;
//                 skillLevel += 2;
//                 Log.Debug($"weaponSkillID{weaponSkillID}");
//             }
//
//             if (current > stageList[2] && current < stageList[3])
//             {
//                 weaponSkillID = initWeaponID + 3;
//                 skillLevel += 3;
//                 Log.Debug($"weaponSkillID{weaponSkillID}");
//             }
//
//             if (current > stageList[3] && current < stageList[4])
//             {
//                 weaponSkillID = initWeaponID + 4;
//                 skillLevel += 4;
//                 Log.Debug($"weaponSkillID{weaponSkillID}");
//             }
//
//             //s��
//             if (current > stageList[4])
//             {
//                 skillLevel += 5;
//                 if (skillSList.Count > 0)
//                 {
//                     int weaponCount = 0;
//                     for (int i = 0; i < skillSList.Count; ++i)
//                     {
//                         weaponCount += (int)skillMap[skillSList[i]].equipPower[0].y;
//                     }
//
//                     var rand = query.ToComponentDataArray<GameRandomData>(Allocator.Temp)[0];
//                     int S1 = (int)skillMap[skillSList[0]].equipPower[0].y;
//                     Log.Debug(S1.ToString(), Color.blue);
//                     if (rand.rand.NextInt(0, weaponCount + 1) > S1)
//                     {
//                         weaponSkillID = initWeaponID + 6;
//                     }
//                     else
//                     {
//                         weaponSkillID = initWeaponID + 5;
//                     }
//                 }
//
//                 Log.Debug($"weaponSkillID{weaponSkillID}", Color.red);
//             }
//
//             //������������
//             for (int i = 0; i < skills.Length; i++)
//             {
//                 if (skills[i].Int32_0 == oldWeaponSkillID)
//                 {
//                     var temp = skills[i];
//                     temp.Int32_0 = weaponSkillID;
//                     skills[i] = temp;
//                 }
//             }
//
//             PlayerPrefs.SetInt("weaponSkillID", weaponSkillID);
//         }
//
//         void HidSupplyImage()
//         {
//             GetFromReference(KactiveSkillImage1).GetImage().SetAlpha(0);
//             GetFromReference(KactiveSkillImage2).GetImage().SetAlpha(0);
//             GetFromReference(KactiveSkillImage3).GetImage().SetAlpha(0);
//             GetFromReference(KactiveSkillImage4).GetImage().SetAlpha(0);
//             GetFromReference(KactiveSkillImage5).GetImage().SetAlpha(0);
//             GetFromReference(KpassiveSkillImage1).GetImage().SetAlpha(0);
//             GetFromReference(KpassiveSkillImage2).GetImage().SetAlpha(0);
//             GetFromReference(KpassiveSkillImage3).GetImage().SetAlpha(0);
//             GetFromReference(KpassiveSkillImage4).GetImage().SetAlpha(0);
//             GetFromReference(KpassiveSkillImage5).GetImage().SetAlpha(0);
//             GetFromReference(KactiveSkillImage1).GetImage().SetAlpha(0);
//             GetFromReference(KactiveSkillImage2).GetComponent<Image>().sprite = null;
//             GetFromReference(KactiveSkillImage3).GetComponent<Image>().sprite = null;
//             GetFromReference(KactiveSkillImage4).GetComponent<Image>().sprite = null;
//             GetFromReference(KactiveSkillImage5).GetComponent<Image>().sprite = null;
//             GetFromReference(KpassiveSkillImage1).GetComponent<Image>().sprite = null;
//             GetFromReference(KpassiveSkillImage2).GetComponent<Image>().sprite = null;
//             GetFromReference(KpassiveSkillImage3).GetComponent<Image>().sprite = null;
//             GetFromReference(KpassiveSkillImage4).GetComponent<Image>().sprite = null;
//             GetFromReference(KpassiveSkillImage5).GetComponent<Image>().sprite = null;
//         }
//
//         private void GetSkillsID()
//         {
//             weaponSkillID = 100101;
//         }
//
//         private void UpadateAvailbaleSkills(ref List<int> availableSkills, int currentID)
//         {
//             var skillHasMap = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
//             if (isSpecialSkill(currentID))
//             {
//                 return;
//             }
//
//             for (int i = 0; i < availableSkills.Count; ++i)
//             {
//                 if (skillHasMap[availableSkills[i]].group == skillHasMap[currentID].group)
//                 {
//                     if (skillHasMap[availableSkills[i]].attribute[0].x == 3 ||
//                         skillHasMap[availableSkills[i]].attribute[0].x == 4)
//                     {
//                         if (availableSkills[i] % 10 == 5)
//                         {
//                             Log.Debug("23333333333333333333333333333333333333");
//                             availableSkills.Remove(availableSkills[i]);
//                             break;
//                         }
//
//                         availableSkills[i] += 1;
//                     }
//                     else if (skillHasMap[availableSkills[i]].attribute[0].x == 1 ||
//                              skillHasMap[availableSkills[i]].attribute[0].x == 2)
//                     {
//                         if (availableSkills[i] % 10 == 6)
//                         {
//                             availableSkills.Remove(availableSkills[i]);
//                             break;
//                         }
//
//                         availableSkills[i] += 1;
//                     }
//
//                     break;
//                 }
//             }
//
//             if (availableSkills.Count <= 0)
//             {
//                 Log.Debug("211111111111111111111111111111111");
//                 var lastAvalible = GetOnlySelectSkills();
//                 for (int i = 0; i < lastAvalible.Count; i++)
//                 {
//                     availableSkills.Add(lastAvalible[i]);
//                 }
//             }
//
//             PlayerPrefs.SetInt(avaliableCount, availableSkills.Count);
//
//             for (int i = 0; i < availableSkills.Count; i++)
//             {
//                 string skillKey = "availableSkill" + (i + 1).ToString();
//                 PlayerPrefs.SetInt(skillKey, availableSkills[i]);
//                 Log.Debug($"availableSkill{i}:{availableSkills[i]}", Color.green);
//             }
//         }
//
//         List<int> GetOnlySelectSkills()
//         {
//             var skillHasMap = ConfigManager.Instance.Tables.Tbplayer_skill.DataList;
//             List<int> onlys = new List<int>(3);
//             for (int i = 0; i < skillHasMap.Count; i++)
//             {
//                 if (isSpecialSkill(skillHasMap[i].id))
//                 {
//                     Log.Debug($"{skillHasMap[i].id}");
//                     onlys.Add(skillHasMap[i].id);
//                 }
//             }
//
//             return onlys;
//         }
//
//         bool isSpecialSkill(int id)
//         {
//             var skillHasMap = ConfigManager.Instance.Tables.Tbplayer_skill.DataList;
//             var temp = id.ToString();
//             if (temp[0] == '2' && temp[1] == '0')
//             {
//                 return true;
//             }
//
//             return false;
//         }
//
//         protected override void OnClose()
//         {
//             //HidSupplyImage();
//             base.OnClose();
//         }
//     }
// }

