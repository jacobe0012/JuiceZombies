//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UICommon_Btn)]
    internal sealed class UICommon_BtnEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UICommon_Btn;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UICommon_Btn>();
        }
    }

    public partial class UICommon_Btn : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KText_Btn = GetFromReference(UICommon_Btn.KText_Btn);
			var KImg_RedDotRight = GetFromReference(UICommon_Btn.KImg_RedDotRight);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
