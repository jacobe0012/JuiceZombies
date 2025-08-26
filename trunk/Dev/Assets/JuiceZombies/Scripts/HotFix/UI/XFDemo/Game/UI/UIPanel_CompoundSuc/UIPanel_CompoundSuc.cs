//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_CompoundSuc)]
    internal sealed class UIPanel_CompoundSucEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_CompoundSuc;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_CompoundSuc>();
        }
    }

    public struct CompoundSucData
    {
        public bool isAllCompound;
        public List<MyGameEquip> Equips;
        public MyGameEquip firstEquips;
    }

    public partial class UIPanel_CompoundSuc : UI, IAwake<CompoundSucData>
    {
        public void Initialize(CompoundSucData args)
        {
            Init(args);
        }

        void AddOnClick()
        {
            var KBg_Mask = GetFromReference(UIPanel_CompoundSuc.KBg_Mask);
            KBg_Mask.GetXButton().OnClick.Add(() =>
            {
                Close();
                //Log.Error($"Closesuc");
            });
        }

        async UniTaskVoid Init(CompoundSucData args)
        {
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
            var qualityConfig = ConfigManager.Instance.Tables.Tbquality;
            var player_skillConfig = ConfigManager.Instance.Tables.Tbskill;
            var KBg_Mask = GetFromReference(UIPanel_CompoundSuc.KBg_Mask);
            var KEquipPos = GetFromReference(UIPanel_CompoundSuc.KEquipPos);
            var KCompoundResult = GetFromReference(UIPanel_CompoundSuc.KCompoundResult);
            var KText_SucInfo = GetFromReference(UIPanel_CompoundSuc.KText_SucInfo);
            var KText_EquipName = GetFromReference(UIPanel_CompoundSuc.KText_EquipName);

            KText_SucInfo.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_success").current);

            AudioManager.Instance.PlayFModAudio(1106);
            if (args.isAllCompound)
            {
                KEquipPos.SetActive(false);
                KCompoundResult.SetActive(false);
                KText_SucInfo.SetActive(true);
                KText_EquipName.SetActive(false);
                AddOnClick();

                return;
            }

            KText_SucInfo.SetActive(false);
            KText_EquipName.SetActive(true);
            var myGameEquip = args.firstEquips;
            var equip0 = myGameEquip.equip;
            long equipUid0 = equip0.PartId;
            int equipId0 = equip0.EquipId;
            int equipQuality0 = equip0.Quality;
            int equipLevel0 = equip0.EquipLevel;
            equipId0 = equip0.EquipId * 100 + equip0.Quality;
            //Log.Error($"selectedNum{selectedNum}selectedEquips{selectedEquips.Count}");

            var equip_data0 = equip_dataConfig.Get(equipId0);
            var equip_quality0 = equip_qualityConfig.Get(equip_data0.quality);
            var quality0 = qualityConfig.Get(equip_quality0.type);

            var needList0 = equip_quality0.mergeRule;


            var newequip = new EquipDto()
            {
                PartId = myGameEquip.equip.PartId,
                //RoleId = myGameEquip.equip.RoleId,
                EquipId = myGameEquip.equip.EquipId,
                //PosId = myGameEquip.equip.PosId,
                Quality = myGameEquip.equip.Quality,
                EquipLevel = myGameEquip.equip.EquipLevel,
                //MainEntryId = myGameEquip.equip.MainEntryId,
                //MainEntryInit = myGameEquip.equip.MainEntryInit,
                //MainEntryGrow = myGameEquip.equip.MainEntryGrow,
            };
            var upQuaEquip = new MyGameEquip
            {
                equip = newequip,
                isWearing = myGameEquip.isWearing,
                canCompound = myGameEquip.canCompound,
            };
            upQuaEquip.equip.Quality++;

            var newEquip = await UIHelper.CreateAsync(KEquipPos, UIType.UICommon_EquipItem, upQuaEquip,
                    UIPanel_Equipment.EquipPanelType.ComposeSelected, KEquipPos.GameObject.transform) as
                UICommon_EquipItem;
            KEquipPos.AddChild(newEquip);

            var name = UnicornUIHelper.GetRewardName(new Vector3(11,
                upQuaEquip.equip.EquipId * 100 + upQuaEquip.equip.Quality, 1));
            KText_EquipName.GetTextMeshPro().SetTMPText(name);


            var rect = newEquip.GetRectTransform();
            rect.SetAnchoredPosition(0, 0);
            rect.SetPivot(new Vector2(0.5f, 0.5f));
            rect.SetAnchorMax(Vector2.one);
            rect.SetAnchorMin(Vector2.zero);
            rect.SetOffset(0, 0, 0, 0);

            var resultList = KCompoundResult.GetList();
            resultList.Clear();
            var item = await resultList.CreateWithUITypeAsync(UIType.UISubPanel_TTITWithBg, false);
            var KText_Left = item.GetFromReference(UISubPanel_TTIT.KText_Left);
            var KText_Mid = item.GetFromReference(UISubPanel_TTIT.KText_Mid);
            //var KImg_Mid = item.GetFromReference(UISubPanel_TTIT.KImg_Mid);
            var KText_Right = item.GetFromReference(UISubPanel_TTIT.KText_Right);
            var KHorizontal = item.GetFromReference(UISubPanel_TTIT.KHorizontal);
            var KText_AllRowBG = item.GetFromReference(UISubPanel_TTIT.KText_AllRowBg);

            KHorizontal.SetActive(true);
            KText_AllRowBG.SetActive(false);

            var item1 = await resultList.CreateWithUITypeAsync(UIType.UISubPanel_TTITWithBg, false);
            var KText_Left1 = item1.GetFromReference(UISubPanel_TTIT.KText_Left);
            var KText_Mid1 = item1.GetFromReference(UISubPanel_TTIT.KText_Mid);
            //var KImg_Mid1 = item1.GetFromReference(UISubPanel_TTIT.KImg_Mid);
            var KText_Right1 = item1.GetFromReference(UISubPanel_TTIT.KText_Right);
            var KHorizontal1 = item1.GetFromReference(UISubPanel_TTIT.KHorizontal);
            var KText_AllRowBg1 = item1.GetFromReference(UISubPanel_TTIT.KText_AllRowBg);

            KHorizontal1.SetActive(true);
            KText_AllRowBg1.SetActive(false);
            var item2 = await resultList.CreateWithUITypeAsync(UIType.UISubPanel_TTITWithBg, false);
            var KHorizontal2 = item2.GetFromReference(UISubPanel_TTIT.KHorizontal);
            var KText_AllRowBG2 = item2.GetFromReference(UISubPanel_TTIT.KText_AllRowBg);
            var KText_AllRow = item2.GetFromReference(UISubPanel_TTIT.KText_AllRow);

            KHorizontal2.SetActive(false);
            KText_AllRowBG2.SetActive(true);


            var skillGroup = equip_data0.minorSkillGroup;

            int skillid = -1;
            for (int i = 0; i < skillGroup.Count; i++)
            {
                if ((int)skillGroup[i].x == quality0.id)
                {
                    skillid = (int)skillGroup[i].y;
                    break;
                }
            }

            var playerSkill = player_skillConfig.GetOrDefault((int)skillid);


            string skillDes = default;
            if (playerSkill == null)
            {
                skillDes = "key is not exist";
            }
            else
            {
                skillDes = string.Format(languageConfig.Get(playerSkill.desc).current,
                    playerSkill.descPara.ToArray());
            }

            KText_AllRow.GetTextMeshPro().SetTMPText(languageConfig.Get(skillDes).current);
            var height = KText_AllRow.GetTextMeshPro().Get().preferredHeight;
            KText_AllRow.GetRectTransform().SetHeight(height);
            height += 10;
            KText_AllRowBG2.GetRectTransform().SetHeight(height);
            item.GetRectTransform().SetHeight(height);
            item1.GetRectTransform().SetHeight(height);
            item2.GetRectTransform().SetHeight(height);

            switch (equip_data0.mainEntryId)
            {
                case 301:
                    KText_Left.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_atk").current);
                    //KImg_Mid.GetImage().SetSpriteAsync("icon_equip_tips_atk", false).Forget();
                    break;
                case 201:
                    KText_Left.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_hp").current);
                    //KImg_Mid.GetImage().SetSpriteAsync("icon_equip_tips_heart", false).Forget();
                    break;
                case 402:
                    KText_Left.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_speed").current);
                    //KImg_Mid.GetImage().SetSpriteAsync("icon_equip_tips_move", false).Forget();
                    break;
            }

            var atkNum0 = equip_data0.mainEntryInit + (equipLevel0 - 1) * equip_data0.mainEntryGrow;
            KText_Mid.GetTextMeshPro().SetTMPText(atkNum0.ToString());

            var newequip_data = equip_dataConfig.Get(upQuaEquip.equip.EquipId * 100 + upQuaEquip.equip.Quality);

            var newequip_quality = equip_qualityConfig.Get(newequip_data.quality);

            var atkNum = newequip_data.mainEntryInit +
                         (upQuaEquip.equip.EquipLevel - 1) * newequip_data.mainEntryGrow;
            KText_Right.GetTextMeshPro().SetTMPText(atkNum.ToString());

            KText_Left1.GetTextMeshPro()
                .SetTMPText(languageConfig.Get("attr_info_level_upperlimit").current);
            KText_Mid1.GetTextMeshPro()
                .SetTMPText(equip_quality0.levelMax.ToString());
            KText_Right1.GetTextMeshPro()
                .SetTMPText(newequip_quality.levelMax.ToString());


            AddOnClick();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}