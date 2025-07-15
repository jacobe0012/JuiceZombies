//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UISubPanel_TTITWithBg)]
    internal sealed class UISubPanel_TTITWithBgEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UISubPanel_TTITWithBg;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_TTITWithBg>();
        }
    }

    public partial class UISubPanel_TTITWithBg : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KText_Left = GetFromReference(UISubPanel_TTITWithBg.KText_Left);
			var KText_Mid = GetFromReference(UISubPanel_TTITWithBg.KText_Mid);
			var KImg_Mid = GetFromReference(UISubPanel_TTITWithBg.KImg_Mid);
			var KText_Right = GetFromReference(UISubPanel_TTITWithBg.KText_Right);
			var KHorizontal = GetFromReference(UISubPanel_TTITWithBg.KHorizontal);
			var KText_AllRow = GetFromReference(UISubPanel_TTITWithBg.KText_AllRow);
			var KText_AllRowBg = GetFromReference(UISubPanel_TTITWithBg.KText_AllRowBg);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
