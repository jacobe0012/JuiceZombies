//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using Common;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_EquipLevelUpTips)]
    internal sealed class UICommon_EquipLevelUpTipsEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_EquipLevelUpTips;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_EquipLevelUpTips>();
        }
    }

    public partial class UICommon_EquipLevelUpTips : UI, IAwake<GameEquipStruct>
    {
        private Tblanguage tblanguage;
        private Tbequip_data tbequip_Data;
        private Tbattr_variable tbattr_variable;
        private Tbequip_quality tbequip_quality;
        private Tbquality tbquality;
        private Tbitem tb_item;
        private Tbequip_level tbequip_level;
        private Tbskill tbplayer_skill;
        private GameEquip equip = new GameEquip();
        private GameEquipStruct tempitem;

        public void Initialize(GameEquipStruct equipitem)
        {
            tempitem = equipitem;

            InitJson();

            InitPanelType(tempitem.Type);
            equip = tempitem.equip;
            //初始化装备信息
            InitEquipInfo(equip);

            RegisterWidgetAction(tempitem);
        }

        private void RegisterWidgetAction(GameEquipStruct equipitem)
        {
            UI levelupUI = GetFromReference(KBtnLevelUp);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(levelupUI, () => LevelupAction(equipitem));
            UI btnRapidLevelup = GetFromReference(KBtnRapidLevelup);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btnRapidLevelup, () => RapidLevelupAction(equipitem));
            UI kBtnEquip = GetFromReference(KBtnEquip);
            var childText = kBtnEquip.GetRectTransform().GetChild(0).GetComponent<XTextMeshProUGUI>();
            switch (equipitem.Type)
            {
                case 1: //装备
                {
                    childText.SetTMPText(tblanguage.Get("common_state_common").current);
                    childText.color = Color.white;
                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(kBtnEquip, () => LoadEquipItem(equipitem.equip));
                    break;
                }
                case 5: //卸下
                {
                    childText.SetTMPText(tblanguage.Get("common_state_remove").current);
                    childText.color = Color.red;
                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(kBtnEquip, () => UnLoadEquipItem(equipitem.equip));
                    break;
                }
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        private void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbequip_Data = ConfigManager.Instance.Tables.Tbequip_data;
            tbattr_variable = ConfigManager.Instance.Tables.Tbattr_variable;
            tbequip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            tbquality = ConfigManager.Instance.Tables.Tbquality;
            tb_item = ConfigManager.Instance.Tables.Tbitem;
            tbequip_level = ConfigManager.Instance.Tables.Tbequip_level;
            tbplayer_skill = ConfigManager.Instance.Tables.Tbskill;
        }


        //指定样式,和equipitem.Type有关
        public void InitPanelType(int type)
        {
            if (type.Equals(1))
            {
                GetFromReference(KImg_TopArraw).SetActive(true);
                GetFromReference(KImg_TopTitle).SetActive(false);
            }
            else if (type.Equals(2))
            {
                GetFromReference(KImg_TopArraw).SetActive(false);
                GetFromReference(KImg_TopTitle).SetActive(true);
                GetFromReference(KTxt_TitleInfo).GetTextMeshPro().SetTMPText(tblanguage.Get("func_2101_name").current);
                GetFromReference(KBtn_Close).GetButton().OnClick.Add(Close);
            }
            else
            {
            }
        }

        //装备信息
        public void InitEquipInfo(GameEquip equip)
        {
            int equipId = equip.EquipId * 100 + equip.Quality;
            var tbequip_data = tbequip_Data.Get(equipId);
            var name = tblanguage.Get(tbequip_data.name).current;
            var LevelName = tblanguage.Get("attr_info_level").current;
            var mainEntryId = tbequip_data.mainEntryId;
            var mainEntryIcon = tbattr_variable.Get(mainEntryId).icon;
            var mainEntryName = tblanguage.Get(tbattr_variable.Get(mainEntryId).name).current;
            var mainEntryInit = tbequip_data.mainEntryInit;
            var mainEntryGrow = tbequip_data.mainEntryGrow;

            GetFromReference(KTxt_EquipName).GetTextMeshPro().SetTMPText(name);
            //等级
            GetFromReference(KText_Mid).GetTextMeshPro().SetTMPText(LevelName);
            //装备等级
            GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(equip.Level.ToString());
            //主属性图
            GetFromReference(KImg_Left).GetComponent<XImage>()
                .SetSprite(ResourcesManager.LoadAsset<Sprite>(mainEntryIcon), false);
            //主属性类型
            GetFromReference(KText_Mid_2).GetTextMeshPro().SetTMPText(mainEntryName);
            //主属性值
            GetFromReference(KText_Right_2).GetTextMeshPro()
                .SetTMPText((mainEntryInit + (equip.Level - 1) * mainEntryGrow).ToString());

            #region 技能组

            //技能组
            var fatherContent = GetFromReference(KContent).GameObject;
            int Equip_type = tbequip_quality.Get(equip.Quality).type;
            var array2Skill = tbequip_data.minorSkillGroup;
            for (int i = 0; i < fatherContent.GetComponent<RectTransform>().childCount; i++)
            {
                var Content_skill = fatherContent.GetComponent<RectTransform>().GetChild(i);
                var Img_Unlock = Content_skill.GetChild(0);
                var Txt_Content = Content_skill.GetChild(1);
                if (Equip_type >= array2Skill[i].x)
                {
                    var unlockName = tbquality.Get(i + 2).unlock;
                    //解锁图片
                    Img_Unlock.GetComponent<XImage>().SetSprite(ResourcesManager.LoadAsset<Sprite>(unlockName), false);
                }
                else
                {
                    var lockName = tbquality.Get(i + 2).lock0;
                    Img_Unlock.GetComponent<XImage>().SetSprite(ResourcesManager.LoadAsset<Sprite>(lockName), false);
                    //加锁图
                }


                var SkillDesc = tbplayer_skill.Get((int)array2Skill[i].y).desc;
                var SkillDescStr = tblanguage.Get(SkillDesc).current;
                //技能描述
                Txt_Content.GetComponent<XTextMeshProUGUI>().SetTMPText(SkillDescStr);
            }

            #endregion


            //等级判断
            var levelMax = tbequip_quality.Get(equip.Quality).levelMax;
            GetFromReference(KBtn_Decrease).SetActive(equip.Level > 1 ? true : false);
            if (equip.Level >= levelMax)
            {
                var equip_levelup_max = tblanguage.Get("equip_levelup_max").current;
                GetFromReference(KNorPos).SetActive(false);
                GetFromReference(KTxt_LevelMaxInfo).SetActive(true);
                GetFromReference(KTxt_LevelMaxInfo).GetTextMeshPro().SetTMPText(equip_levelup_max);
                //失活对应的按钮
                GetFromReference(KBtnLevelUp).SetActive(false);
                GetFromReference(KBtnRapidLevelup).SetActive(false);
            }
            else
            {
                GetFromReference(KNorPos).SetActive(true);
                GetFromReference(KTxt_LevelMaxInfo).SetActive(false);

                //激活对应的按钮
                GetFromReference(KBtnLevelUp).SetActive(true);
                GetFromReference(KBtnRapidLevelup).SetActive(true);

                //升级消耗文字
                GetFromReference(KTxt_Levelup).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("equip_levelup_cost").current);


                long DrawNum = 0;
                long GoldNum = 0;
                int Drawid = 0;
                int Goldid = 0;
                switch (equip.PosId)
                {
                    case 1:
                    {
                        DrawNum = (long)tbequip_level.Get(equip.Level + 1).cost1[1].z;
                        GoldNum = (long)tbequip_level.Get(equip.Level + 1).cost1[0].z;
                        Drawid = (int)tbequip_level.Get(equip.Level + 1).cost1[1].y;
                        Goldid = (int)tbequip_level.Get(equip.Level + 1).cost1[0].x;
                        break;
                    }
                    case 2:
                    {
                        DrawNum = (long)tbequip_level.Get(equip.Level + 1).cost2[1].z;
                        GoldNum = (long)tbequip_level.Get(equip.Level + 1).cost2[0].z;
                        Drawid = (int)tbequip_level.Get(equip.Level + 1).cost2[1].y;
                        Goldid = (int)tbequip_level.Get(equip.Level + 1).cost2[0].x;
                        break;
                    }
                    case 3:
                    {
                        DrawNum = (long)tbequip_level.Get(equip.Level + 1).cost3[1].z;
                        GoldNum = (long)tbequip_level.Get(equip.Level + 1).cost3[0].z;
                        Drawid = (int)tbequip_level.Get(equip.Level + 1).cost3[1].y;
                        Goldid = (int)tbequip_level.Get(equip.Level + 1).cost3[0].x;
                        break;
                    }
                    case 4:
                    {
                        DrawNum = (long)tbequip_level.Get(equip.Level + 1).cost4[1].z;
                        GoldNum = (long)tbequip_level.Get(equip.Level + 1).cost4[0].z;
                        Drawid = (int)tbequip_level.Get(equip.Level + 1).cost4[1].y;
                        Goldid = (int)tbequip_level.Get(equip.Level + 1).cost4[0].x;
                        break;
                    }
                    case 5:
                    {
                        DrawNum = (long)tbequip_level.Get(equip.Level + 1).cost5[1].z;
                        GoldNum = (long)tbequip_level.Get(equip.Level + 1).cost5[0].z;
                        Drawid = (int)tbequip_level.Get(equip.Level + 1).cost5[1].y;
                        Goldid = (int)tbequip_level.Get(equip.Level + 1).cost5[0].x;
                        break;
                    }
                    case 6:
                    {
                        DrawNum = (long)tbequip_level.Get(equip.Level + 1).cost6[1].z;
                        GoldNum = (long)tbequip_level.Get(equip.Level + 1).cost6[0].z;
                        Drawid = (int)tbequip_level.Get(equip.Level + 1).cost6[1].y;
                        Goldid = (int)tbequip_level.Get(equip.Level + 1).cost6[0].x;
                        break;
                    }
                }

                //金币图
                switch (Goldid)
                {
                    case 0:
                    {
                        Log.Debug("error! Goldid", Color.red);
                        break;
                    }
                    case 1:
                    {
                        //金币图
                        GetFromReference(KImg_Gold).GetComponent<XImage>()
                            .SetSprite(ResourcesManager.LoadAsset<Sprite>("icon_energy"), false);
                        break;
                    }
                    case 2:
                    {
                        //金币图
                        GetFromReference(KImg_Gold).GetComponent<XImage>()
                            .SetSprite(ResourcesManager.LoadAsset<Sprite>("icon_diamond"), false);
                        break;
                    }
                    case 3:
                    {
                        //金币图
                        GetFromReference(KImg_Gold).GetComponent<XImage>()
                            .SetSprite(ResourcesManager.LoadAsset<Sprite>("icon_money"), false);

                        break;
                    }
                    case 4:
                    {
                        //金币图
                        GetFromReference(KImg_Gold).GetComponent<XImage>()
                            .SetSprite(ResourcesManager.LoadAsset<Sprite>("icon_exp"), false);

                        break;
                    }
                }

                //图纸图
                GetFromReference(KImg_Drawing).GetComponent<XImage>()
                    .SetSprite(ResourcesManager.LoadAsset<Sprite>(tb_item.Get(Drawid).icon), false);
                long itemNum = 0;
                if (ResourcesSingleton.Instance.items.ContainsKey(Drawid))
                    itemNum = ResourcesSingleton.Instance.items[Drawid];
                //拥有的图纸和升级消耗的图纸
                GetFromReference(KTxt_DrawConsume).GetTextMeshPro()
                    .SetTMPText(itemNum.ToString() + "/" + DrawNum.ToString());


                //拥有的金币和升级消耗的金币
                GetFromReference(KTxt_GoldConsume).GetTextMeshPro().SetTMPText(
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill.ToString() + "/"
                    + GoldNum.ToString());
                //var KBtnLevelUpUI=GetFromReference(KBtnLevelUp);
                //JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtnLevelUpUI,()=> LevelupAction(equip));
            }


            GetFromReference(KBtnEquip).GameObject.GetComponent<RectTransform>().GetChild(0)
                .GetComponent<XTextMeshProUGUI>().SetTMPText(tblanguage.Get("common_state_common").current);
            GetFromReference(KBtnLevelUp).GameObject.GetComponent<RectTransform>().GetChild(0)
                .GetComponent<XTextMeshProUGUI>().SetTMPText(tblanguage.Get("common_state_levelup").current);
            GetFromReference(KBtnRapidLevelup).GameObject.GetComponent<RectTransform>().GetChild(0)
                .GetComponent<XTextMeshProUGUI>().SetTMPText(tblanguage.Get("common_state_levelupclick").current);
        }

        //指定状态,和equipitem.type有关
        public void InitWidgetState(int type)
        {
            if (type.Equals(1))
            {
                GetFromReference(KBtnEquip).GameObject.GetComponent<RectTransform>().GetChild(0)
                    .GetComponent<XTextMeshProUGUI>().SetTMPText(tblanguage.Get("common_state_common").current);
            }
            else if (type.Equals(2))
            {
                GetFromReference(KBtnEquip).GameObject.GetComponent<RectTransform>().GetChild(0)
                    .GetComponent<XTextMeshProUGUI>().SetTMPText(tblanguage.Get("common_state_remove").current);
            }
        }

        //升级函数
        public void LevelupAction(GameEquipStruct equipStruct)
        {
            //先判断对应资源是否充足
            bool isGoldenough = CalcuatingGold(equipStruct.equip);
            if (!isGoldenough)
            {
                CreatePrompt(tblanguage.Get("equip_levelup_error").current);
                return;
            }

            //不足显示对应tip面板
            bool isItemnough = CalculatingItem(equipStruct.equip);
            if (!isItemnough)
            {
                CreatePrompt(tblanguage.Get("equip_levelup_error").current);
                return;
            }


            WebMessageHandlerOld.Instance.AddHandler(9, 3, OnResponseEquipLevelup);

            NetWorkManager.Instance.SendMessage(9, 3, equipStruct.equip);
        }

        //一键升级函数
        public void RapidLevelupAction(GameEquipStruct equipStruct)
        {
            //先判断对应资源是否充足
            bool isGoldenough = CalcuatingGold(equipStruct.equip);
            if (!isGoldenough)
            {
                CreatePrompt(tblanguage.Get("equip_levelup_error").current);
                return;
            }

            //不足显示对应tip面板
            bool isItemnough = CalculatingItem(equipStruct.equip);
            if (!isItemnough)
            {
                CreatePrompt(tblanguage.Get("equip_levelup_error").current);
                return;
            }

            WebMessageHandlerOld.Instance.AddHandler(CMD.EQUIPALLUPGRADE, OnResponseEquipLevelup);

            NetWorkManager.Instance.SendMessage(9, 7, equipStruct.equip);
        }


        //装备
        public void LoadEquipItem(GameEquip equip)
        {
            OnEquipmentLoad?.Invoke(equip);
        }

        //卸下
        public void UnLoadEquipItem(GameEquip equip)
        {
            OnEquipmentUnLoad?.Invoke(equip);
        }


        //计算金币是否足够
        public bool CalcuatingGold(GameEquip equip)
        {
            long result = ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill -
                          (long)tbequip_level.Get(equip.Level + 1).cost1[0].z;

            return result >= 0 ? true : false;
        }

        //计算图纸是否足够
        public bool CalculatingItem(GameEquip equip)
        {
            if (!ResourcesSingleton.Instance.items.ContainsKey(1020000 + equip.PosId)) return false;
            int result = (int)(ResourcesSingleton.Instance.items[1020000 + equip.PosId] -
                               (int)tbequip_level.Get(equip.Level + 1).cost1[1].z);

            return result >= 0 ? true : false;
        }

        private void CreatePrompt(string current)
        {
            JiYuUIHelper.ClearCommonResource();
            UIHelper.CreateAsync(UIType.UICommon_Resource, current);
        }


        //升级回调
        private void OnResponseEquipLevelup(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(9, 3, OnResponseEquipLevelup);
            this.tempitem.equip = new GameEquip();
            this.tempitem.equip.MergeFrom(e.data);
            //减少图纸和金币
            ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill -=
                (int)tbequip_level.Get(this.tempitem.equip.Level).cost1[0].z;
            //更新缓存中的金币
            ResourcesSingleton.Instance.UpdateResourceUI();
            //更新图纸数量
            List<BagItem> countItem = new List<BagItem>();
            BagItem bagItem = new BagItem();
            bagItem.Count = (int)(ResourcesSingleton.Instance.items[1020000 + this.tempitem.equip.PosId] -
                                  (int)tbequip_level.Get(this.tempitem.equip.Level).cost1[1].z);
            bagItem.ItemId = 1020000 + this.tempitem.equip.PosId;
            countItem.Add(bagItem);
            //ResourcesSingleton.Instance.UpdateItems(countItem);
            //更新缓存
            // if (ResourcesSingleton.Instance.equipmentData.equipments.ContainsKey(this.tempitem.equip.PartId))
            //     ResourcesSingleton.Instance.equipmentData.equipments[this.tempitem.equip.PartId] = this.tempitem.equip;
            // else
            // {
            //     Log.Debug("缓存不存在该装备,可能是后端返回装备错误,也可能是初始查询错位", Color.red);
            // }
            //TODO 升级动画


            //TODO
            //已装备装备升级,需要改变面板信息
            if (this.tempitem.Type.Equals(5))
            {
            }

            //重新初始化当前面板
            InitEquipInfo(this.tempitem.equip);

            //重新更新对应的item
            OnEquipmentUpgraded?.Invoke(this.tempitem.equip.PartId, this.tempitem.equip);
        }


        //一些事件
        //普通升级事件
        public static event System.Action<long, GameEquip> OnEquipmentUpgraded;
        public static event System.Action<GameEquip> OnEquipmentLoad;
        public static event System.Action<GameEquip> OnEquipmentUnLoad;
    }
}