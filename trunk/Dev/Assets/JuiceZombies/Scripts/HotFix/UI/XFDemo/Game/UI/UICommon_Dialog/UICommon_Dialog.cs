//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using cfg.config;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Dialog)]
    internal sealed class UICommon_DialogEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_Dialog;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Dialog>();
        }
    }

    public partial class UICommon_Dialog : UI, IAwake<int>
    {
        private Tbguide tbguide;
        private Tblanguage tblanguage;
        private CancellationTokenSource cts = new CancellationTokenSource();

        private bool isClick = false;

        public int guideId;


        public void Initialize(int args1)
        {
            guideId = args1;
            tbguide = ConfigManager.Instance.Tables.Tbguide;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            InitNode();
        }

        async void InitNode()
        {
            var KMid = GetFromReference(UICommon_Dialog.KMid);
            var KBg_Img = GetFromReference(UICommon_Dialog.KBg_Img);
            var KImg_Avator = GetFromReference(UICommon_Dialog.KImg_Avator);
            var KText_dialog = GetFromReference(UICommon_Dialog.KText_dialog);
            var KBtn_Mask = GetFromReference(UICommon_Dialog.KBtn_Mask);
            var paras = tbguide.Get(guideId).guidePara;
            var para1 = int.Parse(paras[0]);
            KImg_Avator.GetImage().SetSpriteAsync($"pic_introguide_frame_{para1}",false);
            var str =
                $"guide.id{guideId} Ĭ�����ı�Ĭ�����ı�Ĭ�����ı�Ĭ�����ı�Ĭ�����ı�";
            if (paras.Count >= 2)
            {
                str = tblanguage.Get($"{paras[1]}").current;
            }

            KBtn_Mask.GetButton().OnClick.Add(() =>
            {
                if (!isClick)
                {
                    isClick = true;
                    cts.Cancel();
                    var tmp = KText_dialog.GetTextMeshPro();
                    tmp.SetTMPText(str);
                    return;
                }

                Close();
            });
            await UnicornUIHelper.TypeWriteEffect(KText_dialog, str, cts.Token);
            isClick = true;
        }

        protected override void OnClose()
        {
            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out UI ui))
            {
                var uis = ui as UIPanel_RunTimeHUD;
                uis.OnGuideOrderFinished(guideId);
            }

            base.OnClose();
        }
    }
}