//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using HotFix_UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISkillItem)]
    internal sealed class UISkillItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISkillItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISkillItem>();
        }
    }

    public partial class UISkillItem : UI, IAwake<int>
    {
        public bool isTimer;
        private long timerId;
        public int maxLevel;

        public void Initialize(int skillID)
        {
            isTimer = false;
            maxLevel = 6;
            SetSkillItem(skillID);
            SetMatchInfo(skillID);
        }

        private void SetMatchInfo(int skillID)
        {
            GetFromReference(KMatchText).SetActive(false);
            GetFromKeyOrPath(KMatchImage).SetActive(false);
            var langTable = ConfigManager.Instance.Tables.Tblanguage;
            GetFromReference(KMatchText).GetTextMeshPro().SetTMPText(langTable.Get("battle_skill_select_evo").current);
            var userSkillList = ConfigManager.Instance.Tables.Tbskill.DataList;
            var userSkillMap = ConfigManager.Instance.Tables.Tbskill.DataMap;
            // if (userSkillMap[skillID].recommend.Count > 0)
            // {
            //     int recommendID = userSkillMap[skillID].recommend[0];
            //     if (recommendID != 0)
            //     {
            //         GetFromReference(KMatchText).SetActive(true);
            //         GetFromKeyOrPath(KMatchImage).SetActive(true);
            //         for (int i = 0; i < userSkillList.Count; i++)
            //         {
            //             // if (userSkillList[i].group == recommendID)
            //             // {
            //             //     GetFromReference(KMatchImage).GetImage().SetSprite(userSkillList[i].pic, false);
            //             //     break;
            //             // }
            //         }
            //     }
            // }
        }

        private void SetSkillItem(int skillID)
        {
            var userSkillTable = ConfigManager.Instance.Tables.Tbskill.DataMap;
            var langTable = ConfigManager.Instance.Tables.Tblanguage;
            var str = skillID.ToString();
            GameObject blinkStar = null;

            if (str[0] == '2' && str[1] == '0')
            {
                GetFromReference(KIsNew)?.GameObject.SetActive(false);
                //终极技能 比如汉堡等等
                GetFromReference(KStarGrop).SetActive(false);

                GetFromReference(KStarSuper).GetComponent<Image>().sprite =
                    ResourcesManager.Instance.Loader.LoadAsset<Sprite>("Star_Normal");
                blinkStar = GetFromReference(KStarSuper).GameObject;
            }
            else
            {
                //判断是否为新技能
                GetFromReference(KIsNew)?.GameObject.SetActive(false);
                bool isNew = isSkillNew(skillID);
                if (isNew)
                {
                    GetFromReference(KIsNew)?.GameObject.SetActive(true);
                    //GetFromReference(KIsNew)?.GetComponent<TMP_Text>().SetTMPText()
                }

                if (isSkillPassive(skillID))
                {
                    this.GetComponent<Image>().sprite =
                        ResourcesManager.Instance.Loader.LoadAsset<Sprite>("icon_supplyItembg_green");
                }

                int skillLevel = userSkillTable[skillID].level;


                //设置星星和闪烁
                if (skillLevel == 6)
                {
                    GetFromReference(KStarGrop).SetActive(false);
                    GetFromReference(KStarSuper).SetActive(true);
                    GetFromReference(KStarSuper).GetComponent<Image>().sprite =
                        ResourcesManager.Instance.Loader.LoadAsset<Sprite>("icon_star_red");
                    blinkStar = GetFromReference(KStarSuper).GameObject;
                }
                else
                {
                    for (int i = 0; i < skillLevel; i++)
                    {
                        GetFromReference(KStarGrop).GetRectTransform().GetChild(i).GetComponent<Image>().sprite =
                            ResourcesManager.Instance.Loader.LoadAsset<Sprite>("icon_star_normal");
                    }

                    blinkStar = GetFromReference(KStarGrop).GetRectTransform().GetChild(skillLevel - 1).gameObject;
                }
            }

            string skillName = langTable.Get(userSkillTable[skillID].name).current;
            // string skillDiscription = langTable.Get(userSkillTable[skillID].content).current;
            // string skillPic = userSkillTable[skillID].pic;


            // GetFromReference(KSkillImage).GetComponent<Image>().sprite =
            //      ResourcesManager.Instance.Loader.LoadAsset<Sprite>(skillPic);
            GetFromReference(KSkillName).GetComponent<TMP_Text>().text = skillName;
            //GetFromReference(KSkillDescripitonText).GetComponent<TMP_Text>().text = skillDiscription;
            this.GetButton(KSelectButton)?.OnClick.Add(() => OnSelectButtonClicked(skillID));
            if (!isTimer && blinkStar != null)
            {
                StartTimer(blinkStar);
            }
        }

        private void OnSelectButtonClicked(int skillID)
        {
            //this.GetParent<UISkillSelect>().SelectButtonClicked(skillID);
            RemoveTimer();
        }

        private void StartTimer(GameObject blinkStar)
        {
            isTimer = true;
            //开启一个每n秒执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(1000, () => this.SetBlinkEffect(blinkStar));
        }

        private void SetBlinkEffect(GameObject blinkStar)
        {
            blinkStar.GetComponent<Image>().DoFade(this, 1f, 0f, 1f);
        }

        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
            isTimer = false;
        }


        private bool isSkillPassive(int skillID)
        {
            var userSkillTable = ConfigManager.Instance.Tables.Tbskill.DataMap;
            //int type =(int) userSkillTable[skillID].attribute[0].x;
            int type = 0;
            if (type == 1 || type == 2)
            {
                return false;
            }

            return true;
        }

        private bool isSkillNew(int skillID)
        {
            var userSkillTable = ConfigManager.Instance.Tables.Tbskill.DataMap;
            var selectedSkills = new List<int>(20);
            for (int i = 0; i < selectedSkills.Capacity; i++)
            {
                string skillKey = "selectSkill" + (i + 1).ToString();
                if (PlayerPrefs.GetInt(skillKey) != 0)
                {
                    selectedSkills.Add(PlayerPrefs.GetInt(skillKey));
                }
            }

            for (int i = 0; i < selectedSkills.Count; i++)
            {
                // if (userSkillTable[selectedSkills[i]].group == userSkillTable[skillID].group) {
                //     return false;
                // }
            }

            return true;
        }

        protected override void OnClose()
        {
            RemoveTimer();
            base.OnClose();
        }
    }
}