//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: 2025-08-23 21:20:59
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UICommonBtn)]
    internal sealed class UICommonBtnEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UICommonBtn;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UICommonBtn>();
        }
    }

    public partial class UICommonBtn : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KGreen = GetFromReference(UICommonBtn.KGreen);
			var KYellow = GetFromReference(UICommonBtn.KYellow);
			var KBlue = GetFromReference(UICommonBtn.KBlue);
			var KPink = GetFromReference(UICommonBtn.KPink);
			var KText = GetFromReference(UICommonBtn.KText);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
