//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UICommon_Reward_TipWithTitle)]
    internal sealed class UICommon_Reward_TipWithTitleEvent : AUIEvent, IUILayer
    {
	    public override string Key => UIPathSet.UICommon_Reward_TipWithTitle;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => true;
		
		public UILayer Layer => UILayer.Low;
		
        public override UI OnCreate()
        {
            return UI.Create<UICommon_Reward_TipWithTitle>();
        }
    }

    public partial class UICommon_Reward_TipWithTitle : UI, IAwake<List<Vector3>>
	{	
		public void Initialize(List<Vector3> rewardList)
		{
			 InitNode();
            var Bglist = this.GetFromReference(Kcontent).GetList();
            Bglist.Clear();
            var count = rewardList.Count;
            for (int i = 0; i < count; i++)
            {
                Bglist.CreateWithUIType(UIType.UICommon_Reward_Tip_Item, rewardList[i], false);
            }
            var width = 156 * count;
            this.GetFromReference(Kcontent).GetRectTransform().SetWidth(width);

            this.GetFromReference(KBg).GetRectTransform().SetWidth(width + 40);
            this.GetRectTransform().SetWidth(width + 40);
            JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(Kcontent));
        }
		 void InitNode()
		{
			var KBg = GetFromReference(UICommon_Reward_TipWithTitle.KBg);
			var KImg_ArrowDownLeft = GetFromReference(UICommon_Reward_TipWithTitle.KImg_ArrowDownLeft);
			var KImg_ArrowDownUp = GetFromReference(UICommon_Reward_TipWithTitle.KImg_ArrowDownUp);
			var KImg_ArrowDownRight = GetFromReference(UICommon_Reward_TipWithTitle.KImg_ArrowDownRight);
			var Kcontent = GetFromReference(UICommon_Reward_TipWithTitle.Kcontent);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
