//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

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
    [UIEvent(UIType.UIPanel_TechnologyItem)]
    internal sealed class UIPanel_TechnologyItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPanel_TechnologyItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_TechnologyItem>();
        }
    }

    public partial class UIPanel_TechnologyItem : UI, IAwake<int>
    {
        public int tecId;
        private Tbbattletech tbBattletech;
        private Tblanguage tbLanguage;
        private Tbskill_quality tbSkill_quality;

        public void Initialize(int technologyID)
        {
            tecId = technologyID;
            Debug.Log($"technologyID:{tecId}");
            tbBattletech = ConfigManager.Instance.Tables.Tbbattletech;
            tbLanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbSkill_quality = ConfigManager.Instance.Tables.Tbskill_quality;
            this.GetImage(KImg_bg_quality)
                ?.SetSpriteAsync(tbSkill_quality.GetOrDefault(tbBattletech.Get(tecId).quality).pic, false);
            this.GetTextMeshPro(KTxt_name_Technology)
                ?.SetTMPText(tbLanguage.Get(tbBattletech.Get(tecId).name).current);
            this.GetTextMeshPro(KTxt_Descrip_Technology)
                ?.SetTMPText(tbLanguage.Get(tbBattletech.Get(tecId).desc).current);
            this.GetImage(KImg_icon_technology)?.SetSprite(tbBattletech.Get(tecId).icon, false);
            this.GetImage(KImg_icon_type)?.SetSprite(tbBattletech.Get(tecId).typeIcon, false);
            //this.GetTextMeshPro(KTxt_typeName)?.SetTMPText(tbLanguage.Get("common_free_refresh_text").current);
            this.GetTextMeshPro(KTxt_typeName)?.SetTMPText(tbLanguage.Get("text_type").current +
                                                           tbLanguage.Get(tbBattletech.Get(technologyID).typeName)
                                                               .current);
            GetFromReference(KBtn_Refresh)?.SetActive(true);
            this.GetTextMeshPro(KTxt_Refresh)?.SetTMPText(tbLanguage.Get("common_free_refresh_text").current);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Refresh),
                () => OnRefreshBtnClick(tecId, this));
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Item),
                () => OnSelectBtnClick(tecId));
        }

        private void OnRefreshBtnClick(int technologyID, UIPanel_TechnologyItem self)
        {
            //Debug.LogError($"technologyID:{tecId}");
            if (UnicornUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui1))
            {
                  UnicornUIHelper.DestoryAllTips();;
            }

            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_BattleTecnology, out UI ui))
            {
                var uiBattleTechnology = ui as UIPanel_BattleTecnology;
                uiBattleTechnology?.RefreshTechItem(technologyID, self);
            }
        }

        public void UpdateItem(int technologyID)
        {
            tecId = technologyID;
            Debug.LogError($"technologyID:{tecId}");
            GetFromReference(KBtn_Refresh)?.SetActive(false);
            this.GetImage(KImg_bg_quality)
                ?.SetSpriteAsync(tbSkill_quality.GetOrDefault(tbBattletech.Get(technologyID).quality).pic, false);
            this.GetTextMeshPro(KTxt_name_Technology)
                ?.SetTMPText(tbLanguage.Get(tbBattletech.Get(technologyID).name).current);
            this.GetTextMeshPro(KTxt_Descrip_Technology)
                ?.SetTMPText(tbLanguage.Get(tbBattletech.Get(technologyID).desc).current);
            this.GetImage(KImg_icon_technology)?.SetSprite(tbBattletech.Get(technologyID).icon, false);
            this.GetTextMeshPro(KTxt_typeName)?.SetTMPText(tbLanguage.Get("text_type").current +
                                                           tbLanguage.Get(tbBattletech.Get(technologyID).typeName)
                                                               .current);
            //this.GetFromReference(KBtn_Item).re
            //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Item),
            // () => OnSelectBtnClick(technologyID));
        }

        private void OnSelectBtnClick(int technologyID)
        {
            Debug.Log($"technologyID:{technologyID}");
            if (UnicornUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui))
            {
                  UnicornUIHelper.DestoryAllTips();;
            }

            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui1))
            {
                var currentUI = ui1 as UIPanel_RunTimeHUD;
                var temp = currentUI.displaySelectedTechs;
                temp.Add(technologyID);
                Log.Debug($"添加科技,id:{technologyID}", Color.red);
                currentUI.displaySelectedTechs = temp;
            }

            AddTecSkillToPlayer(technologyID);

            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_BattleTecnology, out UI ui2))
            {
                var ui2s = ui2 as UIPanel_BattleTecnology;
                ui2s.GuideOnClick();
                ui2.Dispose();
            }
        }

        private void AddTecSkillToPlayer(int technologyID)
        {
            var skillID = tbBattletech.Get(technologyID).skillId;
            Debug.Log($"technologyID:{technologyID},skillid:{skillID}");
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            BuffHelper.AddSkillByDcb(entityManager, skillID, player, 1);
        }

        private string SetColorForQuality(int quality)
        {
            string color = "";
            switch (quality)
            {
                case 1:
                    color = "#357AEA";
                    break;
                case 2:
                    color = "#E7B008";
                    break;
                case 3:
                    color = "#9234EA";
                    break;
            }

            return color;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}