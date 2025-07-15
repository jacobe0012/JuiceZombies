//---------------------------------------------------------------------
// JiYuStudio
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
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_GuideTips)]
    internal sealed class UIPanel_GuideTipsEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_GuideTips;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.WorldSpace;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_GuideTips>();
        }
    }

    public partial class UIPanel_GuideTips : UI, IAwake<int>
    {
        private Tbskill_binding tbskill_binding;
        private Tblanguage tblanguage;
        private Tbguide tbguide;
        public int guideId;
        public bool closed;

        public void Initialize(int args)
        {
            guideId = args;
            InitJson();
            InitNode();
        }

        void InitJson()
        {
            tbskill_binding = ConfigManager.Instance.Tables.Tbskill_binding;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbguide = ConfigManager.Instance.Tables.Tbguide;
        }

        async void InitNode()
        {
            var KCommon_ItemTips = GetFromReference(UIPanel_GuideTips.KCommon_ItemTips);
            var KImg_Tips = GetFromReference(UIPanel_GuideTips.KImg_Tips);
            var KImg_Tips_4 = GetFromReference(UIPanel_GuideTips.KImg_Tips_4);
            var KImg_GuideAnim_10 = GetFromReference(UIPanel_GuideTips.KImg_GuideAnim_10);
            var KCommon_Dialog = GetFromReference(UIPanel_GuideTips.KCommon_Dialog);
            var KImg_Avator = KCommon_Dialog.GetFromReference(UICommon_Dialog.KImg_Avator);
            var KText_dialog = KCommon_Dialog.GetFromReference(UICommon_Dialog.KText_dialog);

            var guide = tbguide.Get(guideId);
            KImg_Tips_4.SetActive(false);
            KImg_Tips.SetActive(false);
            switch (guide.guideType)
            {
                case 313:
                    KImg_Tips_4.SetActive(true);
                    var para313_2 = guide.guidePara[0];
                    KText_dialog.GetTextMeshPro().SetTMPText(tblanguage.Get(para313_2).current);
                    await UniTask.Delay(5000);
                    Close();
                    break;
                case 323:
                    KImg_Tips.SetActive(true);
                    var para323_1 = int.Parse(guide.guidePara[0]);
                    var para323_2 = guide.guidePara[1];
                    var para323_3 = int.Parse(guide.guidePara[2]);

                    var KTxt_Des = KCommon_ItemTips.GetFromReference(UICommon_ItemTips.KTxt_Des);

                    KTxt_Des.GetTextMeshPro().SetTMPText(tblanguage.Get(para323_2).current);

                    Update().Forget();
                    await UniTask.Delay(para323_3);
                    Close();
                    break;
            }
        }

        async UniTaskVoid Update()
        {
            var rect = this.GetRectTransform();
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerquery = em.CreateEntityQuery(typeof(PlayerData));
            var player = playerquery.ToEntityArray(Allocator.Temp)[0];
            while (!closed)
            {
                var tran = em.GetComponentData<LocalTransform>(player);
                rect.SetAnchoredPosition(tran.Position.xy);
                await UniTask.Yield();
            }
        }

        protected override void OnClose()
        {
            closed = true;
            JiYuUIHelper.TryFinishGuide(guideId);
            base.OnClose();
        }
    }
}