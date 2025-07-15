//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UISubPanel_SelectBoxItem)]
    internal sealed class UISubPanel_SelectBoxItemEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UISubPanel_SelectBoxItem;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_SelectBoxItem>();
        }
    }

    public partial class UISubPanel_SelectBoxItem : UI, IAwake<Vector3>
	{	
		public Vector3 reward;
		public void Initialize(Vector3 args)
		{
			reward = args;
			InitNode();
		}
		 void InitNode()
		{
			var KCommon_RewardItem = GetFromReference(UISubPanel_SelectBoxItem.KCommon_RewardItem);
			var KBtn_Selected = GetFromReference(UISubPanel_SelectBoxItem.KBtn_Selected);
			var KImg_UnSelected = GetFromReference(UISubPanel_SelectBoxItem.KImg_UnSelected);
			var KImg_Selected = GetFromReference(UISubPanel_SelectBoxItem.KImg_Selected);
			KImg_UnSelected.SetActive(true);
			KImg_Selected.SetActive(false);
			JiYuUIHelper.SetRewardIconAndCountText(reward, KCommon_RewardItem);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
