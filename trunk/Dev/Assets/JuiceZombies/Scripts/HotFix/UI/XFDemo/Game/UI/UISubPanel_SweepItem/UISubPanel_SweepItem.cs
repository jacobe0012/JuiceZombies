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
	[UIEvent(UIType.UISubPanel_SweepItem)]
    internal sealed class UISubPanel_SweepItemEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UISubPanel_SweepItem;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_SweepItem>();
        }
    }

    public partial class UISubPanel_SweepItem : UI, IAwake
	{
		public int index;
		public void Initialize()
		{
			 InitNode();
			JiYuUIHelper.ForceRefreshLayout(GetFromReference(Kcontainer));
		}
		 void InitNode()
		{
			var KImgMonster = GetFromReference(UISubPanel_SweepItem.KImgMonster);
			var KText_EnemyName = GetFromReference(UISubPanel_SweepItem.KText_EnemyName);
			var KText_EnemyCount = GetFromReference(UISubPanel_SweepItem.KText_EnemyCount);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
