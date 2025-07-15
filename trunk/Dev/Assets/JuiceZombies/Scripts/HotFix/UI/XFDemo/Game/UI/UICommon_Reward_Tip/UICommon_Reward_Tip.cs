//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Reward_Tip)]
    internal sealed class UICommon_Reward_TipEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_Reward_Tip;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Reward_Tip>();
        }
    }

    public partial class UICommon_Reward_Tip : UI, IAwake<List<Vector3>>
    {
        #region 参数

        int itemH = 156;
        int itemW = 348;

        #endregion

        public async void Initialize(List<Vector3> rewardList)
        {
            // this.GetFromReference(KImg_ArrowDownRight).SetActive(false);
            // this.GetRectTransform().SetHeight(itemH * rewardList.Count);
            // this.GetRectTransform().SetWidth(itemW);
            var Bglist = this.GetFromReference(KBg).GetList();
            Bglist.Clear();
            var count = rewardList.Count;
            for (int i = 0; i < count; i++)
            {
               Bglist.CreateWithUIType(UIType.UICommon_Reward_Tip_Item, rewardList[i], false);
            }
            var width=156*count;
            this.GetFromReference(KBg).GetRectTransform().SetWidth(width);
            this.GetRectTransform().SetWidth(width);
            JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KBg));
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}