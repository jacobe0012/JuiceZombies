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
	[UIEvent(UIType.UIPanel_GuideBoom)]
    internal sealed class UIPanel_GuideBoomEvent : AUIEvent, IUILayer
    {
	    public override string Key => UIPathSet.UIPanel_GuideBoom;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => true;
		
		public UILayer Layer => UILayer.Low;
		
        public override UI OnCreate()
        {
            return UI.Create<UIPanel_GuideBoom>();
        }
    }

    public partial class UIPanel_GuideBoom : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KImg_Bg = GetFromReference(UIPanel_GuideBoom.KImg_Bg);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
