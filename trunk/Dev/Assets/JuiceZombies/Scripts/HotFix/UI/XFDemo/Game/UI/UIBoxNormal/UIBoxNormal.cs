//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2025-07-19 21:42:10
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UIBoxNormal)]
    internal sealed class UIBoxNormalEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UIBoxNormal;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UIBoxNormal>();
        }
    }

    public partial class UIBoxNormal : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KBg_Mask = GetFromReference(UIBoxNormal.KBg_Mask);
			var KBorder = GetFromReference(UIBoxNormal.KBorder);
			var KText_Title = GetFromReference(UIBoxNormal.KText_Title);
			var KText_Description = GetFromReference(UIBoxNormal.KText_Description);
			var KImg_Box = GetFromReference(UIBoxNormal.KImg_Box);
			var KGradient_Bottom = GetFromReference(UIBoxNormal.KGradient_Bottom);
			var KGradient_Top = GetFromReference(UIBoxNormal.KGradient_Top);
			var KGroup_Btn = GetFromReference(UIBoxNormal.KGroup_Btn);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
