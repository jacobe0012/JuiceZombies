//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Threading;
using Cysharp.Threading.Tasks;
using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Fund_List)]
    internal sealed class UISubPanel_Shop_Fund_ListEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Fund_List;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Fund_List>();
        }
    }

    public partial class UISubPanel_Shop_Fund_List : UI, IAwake
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        public void Initialize()
        {
            InitEffect().Forget();
        }

        private async UniTask InitEffect()
        {
            await UniTask.Delay(600, cancellationToken: cts.Token);
            JiYuTweenHelper.PlayUIImageSweepFX(this.GetFromReference(KImg_Btn_Mid), cancellationToken: cts.Token);
            await UniTask.Delay(100, cancellationToken: cts.Token);
            JiYuTweenHelper.PlayUIImageSweepFX(this.GetFromReference(KImg_Btn_Right), cancellationToken: cts.Token);
        }

        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}