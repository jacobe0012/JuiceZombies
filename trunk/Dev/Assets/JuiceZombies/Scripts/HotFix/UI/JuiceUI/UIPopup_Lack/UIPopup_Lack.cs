//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2025-08-30 20:10:00
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UIPopup_Lack)]
    internal sealed class UIPopup_LackEvent : AUIEvent, IUILayer
    {
	    public override string Key => UIPathSet.UIPopup_Lack;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => true;
		
		public UILayer Layer => UILayer.Low;
		
        public override UI OnCreate()
        {
            return UI.Create<UIPopup_Lack>();
        }
    }

    public partial class UIPopup_Lack : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KButton_Close = GetFromReference(UIPopup_Lack.KButton_Close);
			var KText_Title = GetFromReference(UIPopup_Lack.KText_Title);
			var KImage_Icon = GetFromReference(UIPopup_Lack.KImage_Icon);
			var KText_Insufficient = GetFromReference(UIPopup_Lack.KText_Insufficient);
			var KText_Get = GetFromReference(UIPopup_Lack.KText_Get);
			var KGetFuncContaienr = GetFromReference(UIPopup_Lack.KGetFuncContaienr);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
