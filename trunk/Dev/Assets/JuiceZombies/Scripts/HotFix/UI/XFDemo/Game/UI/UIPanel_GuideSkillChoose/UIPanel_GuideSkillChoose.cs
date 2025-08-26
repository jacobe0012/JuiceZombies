//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Main;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_GuideSkillChoose)]
    internal sealed class UIPanel_GuideSkillChooseEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_GuideSkillChoose;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.WorldSpace;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_GuideSkillChoose>();
        }
    }

    public partial class UIPanel_GuideSkillChoose : UI, IAwake
    {
        private Tbskill_binding tbskill_binding;
        private Tblanguage tblanguage;
        private const float Offset = 25f;
        private const float Radius = 20f;
        private bool close = false;

        public void Initialize()
        {
            InitJson();
            InitNode();
        }

        void InitJson()
        {
            tbskill_binding = ConfigManager.Instance.Tables.Tbskill_binding;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        }

        void InitNode()
        {
            var KText_Title = GetFromReference(UIPanel_GuideSkillChoose.KText_Title);
            var KIcon_0 = GetFromReference(UIPanel_GuideSkillChoose.KIcon_0);
            var KText_0 = GetFromReference(UIPanel_GuideSkillChoose.KText_0);
            var KIcon_1 = GetFromReference(UIPanel_GuideSkillChoose.KIcon_1);
            var KText_1 = GetFromReference(UIPanel_GuideSkillChoose.KText_1);
            var KIcon_2 = GetFromReference(UIPanel_GuideSkillChoose.KIcon_2);
            var KText_2 = GetFromReference(UIPanel_GuideSkillChoose.KText_2);
            var KIcon_3 = GetFromReference(UIPanel_GuideSkillChoose.KIcon_3);
            var KText_3 = GetFromReference(UIPanel_GuideSkillChoose.KText_3);
            KText_Title.GetTextMeshPro().SetTMPText(tblanguage.Get("guide_binding_choose_1").current);
            for (int i = 0; i < tbskill_binding.DataList.Count; i++)
            {
                var skillBinding = tbskill_binding.DataList[i];
                var index = i;
                var icon = GetFromReference($"Icon_{index}");
                var text = GetFromReference($"Text_{index}");
                icon.GetImage().SetSpriteAsync(skillBinding.pic, false);
                text.GetTextMeshPro().SetTMPText(tblanguage.Get(skillBinding.name).current);
            }

            UIHelper.CreateAsync(UIType.UIPanel_GuideSkillConfirm, 1);

            ChoseOnSkillGroup().Forget();
        }

        public async UniTaskVoid ChoseOnSkillGroup()
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = em.CreateEntityQuery(typeof(PlayerData), typeof(LocalTransform));
            if (query.IsEmpty)
            {
                Log.Error($"PlayerData query.IsEmpty");
                return;
            }

            var KText_Title = GetFromReference(UIPanel_GuideSkillChoose.KText_Title);

            var KImg_Choose0 = GetFromReference(UIPanel_GuideSkillChoose.KImg_Choose0);
            var KImg_Choose1 = GetFromReference(UIPanel_GuideSkillChoose.KImg_Choose1);
            var KImg_Choose2 = GetFromReference(UIPanel_GuideSkillChoose.KImg_Choose2);
            var KImg_Choose3 = GetFromReference(UIPanel_GuideSkillChoose.KImg_Choose3);
            string str = default;
            string picStr0 = "pic_introguide_skillchoose1";
            string picStr1 = "pic_introguide_skillchoose2";
            int skillBindingId = 1;
            while (!close)
            {
                var tran = query.ToComponentDataArray<LocalTransform>(Allocator.Temp)[0];
                float3 point1 = new float3(-Offset, Offset, 0);
                float3 point2 = new float3(Offset, Offset, 0);
                float3 point3 = new float3(-Offset, -Offset, 0);
                float3 point4 = new float3(Offset, -Offset, 0);


                if (math.length(tran.Position - point1) < Radius)
                {
                    skillBindingId = 1;
                    str = string.Format(tblanguage.Get("guide_binding_choose_2").current,
                        tblanguage.Get(tbskill_binding.Get(skillBindingId).name).current);
                    KText_Title.GetTextMeshPro().SetTMPText(str);

                    KImg_Choose0.GetImage().SetSpriteAsync(picStr1, false);
                    //UIHelper.CreateAsync(UIType.UIPanel_GuideSkillConfirm, skillBindingId);
                    if (UnicornUIHelper.TryGetUI(UIType.UIPanel_GuideSkillConfirm, out var ui))
                    {
                        var uis = ui as UIPanel_GuideSkillConfirm;
                        uis.Refresh(skillBindingId);
                        uis.SetActive(true);
                        
                    }
                }
                else if (math.length(tran.Position - point2) < Radius)
                {
                    skillBindingId = 2;
                    str = string.Format(tblanguage.Get("guide_binding_choose_2").current,
                        tblanguage.Get(tbskill_binding.Get(skillBindingId).name).current);
                    KText_Title.GetTextMeshPro().SetTMPText(str);

                    KImg_Choose1.GetImage().SetSpriteAsync(picStr1, false);
                    //UIHelper.CreateAsync(UIType.UIPanel_GuideSkillConfirm, skillBindingId);
                    if (UnicornUIHelper.TryGetUI(UIType.UIPanel_GuideSkillConfirm, out var ui))
                    {
                        var uis = ui as UIPanel_GuideSkillConfirm;
                        uis.Refresh(skillBindingId);
                        uis.SetActive(true);
                        
                    }
                }
                else if (math.length(tran.Position - point3) < Radius)
                {
                    skillBindingId = 3;
                    str = string.Format(tblanguage.Get("guide_binding_choose_2").current,
                        tblanguage.Get(tbskill_binding.Get(skillBindingId).name).current);
                    KText_Title.GetTextMeshPro().SetTMPText(str);

                    KImg_Choose2.GetImage().SetSpriteAsync(picStr1, false);
                    //UIHelper.CreateAsync(UIType.UIPanel_GuideSkillConfirm, skillBindingId);
                    if (UnicornUIHelper.TryGetUI(UIType.UIPanel_GuideSkillConfirm, out var ui))
                    {
                        var uis = ui as UIPanel_GuideSkillConfirm;
                        uis.Refresh(skillBindingId);
                        uis.SetActive(true);
                        
                    }
                }
                else if (math.length(tran.Position - point4) < Radius)
                {
                    skillBindingId = 4;
                    str = string.Format(tblanguage.Get("guide_binding_choose_2").current,
                        tblanguage.Get(tbskill_binding.Get(skillBindingId).name).current);
                    KText_Title.GetTextMeshPro().SetTMPText(str);

                    KImg_Choose3.GetImage().SetSpriteAsync(picStr1, false);
                    //UIHelper.CreateAsync(UIType.UIPanel_GuideSkillConfirm, skillBindingId);
                    if (UnicornUIHelper.TryGetUI(UIType.UIPanel_GuideSkillConfirm, out var ui))
                    {
                        var uis = ui as UIPanel_GuideSkillConfirm;
                        uis.Refresh(skillBindingId);
                        uis.SetActive(true);
                        
                    }
                }
                else
                {
                    str = tblanguage.Get("guide_binding_choose_1").current;
                    KText_Title.GetTextMeshPro().SetTMPText(str);
                    KImg_Choose0.GetImage().SetSpriteAsync(picStr0, false);
                    KImg_Choose1.GetImage().SetSpriteAsync(picStr0, false);
                    KImg_Choose2.GetImage().SetSpriteAsync(picStr0, false);
                    KImg_Choose3.GetImage().SetSpriteAsync(picStr0, false);
                    if (UnicornUIHelper.TryGetUI(UIType.UIPanel_GuideSkillConfirm, out var ui))
                    {
                        var uis = ui as UIPanel_GuideSkillConfirm;
                        uis.SetActive(false);
                    }
                    //UIHelper.Remove(UIType.UIPanel_GuideSkillConfirm);
                }

                await UniTask.Yield();
            }
        }

        protected override void OnClose()
        {
            close = true;
            base.OnClose();
        }
    }
}