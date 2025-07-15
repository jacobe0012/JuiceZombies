//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.config;
using HotFix_UI;
using Main;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_GuideSkillConfirm)]
    internal sealed class UIPanel_GuideSkillConfirmEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_GuideSkillConfirm;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_GuideSkillConfirm>();
        }
    }

    public partial class UIPanel_GuideSkillConfirm : UI, IAwake<int>
    {
        private Tbskill_binding tbskill_binding;
        private Tblanguage tblanguage;
        private Tbguide tbguide;
        public int id;

        public void Initialize(int args)
        {
            id = args;
            InitJson();
            InitNode();
        }

        void InitJson()
        {
            tbskill_binding = ConfigManager.Instance.Tables.Tbskill_binding;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbguide = ConfigManager.Instance.Tables.Tbguide;
        }

        void InitNode()
        {
            var skillBinding = tbskill_binding.Get(id);
            var KImg_Bg = GetFromReference(UIPanel_GuideSkillConfirm.KImg_Bg);
            var KText_Title = GetFromReference(UIPanel_GuideSkillConfirm.KText_Title);
            var KText_Content = GetFromReference(UIPanel_GuideSkillConfirm.KText_Content);
            var KCommon_Btn = GetFromReference(UIPanel_GuideSkillConfirm.KCommon_Btn);

            KText_Title.GetTextMeshPro().SetTMPText(tblanguage.Get(skillBinding.name).current);
            KText_Content.GetTextMeshPro().SetTMPText(tblanguage.Get(skillBinding.desc).current);
            var KText_Btn = KCommon_Btn.GetFromReference(UICommon_Btn.KText_Btn);
            var KImg_RedDotRight = KCommon_Btn.GetFromReference(UICommon_Btn.KImg_RedDotRight);
            KText_Btn.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_confirm").current);
            KCommon_Btn.GetXButton().RemoveAllListeners();
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KCommon_Btn, () =>
            {
                var em = World.DefaultGameObjectInjectionWorld.EntityManager;
                var query = em.CreateEntityQuery(typeof(PlayerData), typeof(Skill));
                if (query.IsEmpty)
                {
                    Log.Error($"PlayerData query.IsEmpty");
                    return;
                }

                var player = query.ToEntityArray(Allocator.Temp)[0];
                var skillBuffer = em.GetBuffer<Skill>(player);
                foreach (var skill in skillBinding.skillGuide)
                {
                    skillBuffer.Add(new Skill
                    {
                        CurrentTypeId = (Skill.TypeId)1,
                        Int32_0 = skill,
                        Entity_5 = player,
                    });
                }

                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_GuideSkillChoose, out var ui))
                {
                    ui.Dispose();
                }

                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui0))
                {
                    var ui0s = ui0 as UIPanel_RunTimeHUD;
                    var guide = tbguide.DataList.Where(a => a.guideType == 902).FirstOrDefault();
                    ui0s.OnGuideOrderFinished(guide.id);
                }

                Close();
            });
        }

        public void Refresh(int groupId)
        {
            id = groupId;
            InitNode();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}