//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2025-08-30 20:03:56
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UIItemGetFuc)]
    internal sealed class UIItemGetFucEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UIItemGetFuc;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UIItemGetFuc>();
        }
    }

    public partial class UIItemGetFuc : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KText = GetFromReference(UIItemGetFuc.KText);
			var KButton_Ok = GetFromReference(UIItemGetFuc.KButton_Ok);
			var KText_Ok = GetFromReference(UIItemGetFuc.KText_Ok);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
