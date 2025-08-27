//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: 2025-08-27 23:22:57
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UIJuiceCommonBtn)]
    internal sealed class UIJuiceCommonBtnEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UIJuiceCommonBtn;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UIJuiceCommonBtn>();
        }
    }

    public partial class UIJuiceCommonBtn : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KGreen = GetFromReference(UIJuiceCommonBtn.KGreen);
			var KYellow = GetFromReference(UIJuiceCommonBtn.KYellow);
			var KBlue = GetFromReference(UIJuiceCommonBtn.KBlue);
			var KPink = GetFromReference(UIJuiceCommonBtn.KPink);
			var KText = GetFromReference(UIJuiceCommonBtn.KText);
			var KRed = GetFromReference(UIJuiceCommonBtn.KRed);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
