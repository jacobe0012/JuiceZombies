//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;
using System.Threading;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Collection_Item)]
    internal sealed class UISubPanel_Collection_ItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Collection_Item;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Collection_Item>();
        }
    }

    public partial class UISubPanel_Collection_Item : UI, IAwake<int>
    {
        /// <summary>
        /// 改为图鉴id
        /// </summary>
        public int monsterId;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public void Initialize(int args)
        {
            monsterId = args;


            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UISubPanel_Collection_Item.KMid), 0, 50,
                cts.Token, 0.35f, false, false);


            //JiYuUIHelper.ChangePaddingLR(this, 50, 0.2f);
            JiYuTweenHelper.ChangeSoftness(this, 300, 0.2f);
        }

        protected override void OnClose()
        {
            cts?.Cancel();
            base.OnClose();
        }
    }
}