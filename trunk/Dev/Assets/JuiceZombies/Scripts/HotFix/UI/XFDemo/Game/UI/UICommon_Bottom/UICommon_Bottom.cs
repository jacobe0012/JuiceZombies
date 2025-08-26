//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Bottom)]
    internal sealed class UICommon_BottomEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_Bottom;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Bottom>();
        }
    }

    public partial class UICommon_Bottom : UI, IAwake
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        public void Initialize()
        {
            InitNode();
            UnicornTweenHelper.SetEaseAlphaAndPosUtoB(this.GetFromReference(UICommon_Bottom.KScrollView_Item0), 0, 50f,cts.Token);
        }

        void InitNode()
        {
            var KScrollView_Item0 = GetFromReference(UICommon_Bottom.KScrollView_Item0);
            var KBtn_Close = GetFromReference(UICommon_Bottom.KBtn_Close);
            var KText_BottomTitle = GetFromReference(UICommon_Bottom.KText_BottomTitle);
            var KBtn_TitleInfo = GetFromReference(UICommon_Bottom.KBtn_TitleInfo);
            KText_BottomTitle.SetActive(false);
            KBtn_TitleInfo.SetActive(false);
        }

        protected override void OnClose()
        {
            cts.Cancel();
            base.OnClose();
        }
    }
}