//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using Cysharp.Threading.Tasks;
using dnlib.Threading;
using HotFix_UI;
using System;
using System.Threading;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Gift_Item)]
    internal sealed class UISubPanel_Shop_Gift_ItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Gift_Item;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Gift_Item>();
        }
    }

    public partial class UISubPanel_Shop_Gift_Item : UI, IAwake
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        public void Initialize()
        {
            InitEffect().Forget();
        }

        private async UniTaskVoid InitEffect()
        {
            await UniTask.Delay(200, cancellationToken: cts.Token);
            UnicornTweenHelper.PlayUIImageSweepFX(this.GetFromReference(KBtn), cts.Token);
        }

        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}