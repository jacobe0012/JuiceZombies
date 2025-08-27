//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2025-08-23 22:43:11
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UIItemMonCardIDes)]
    internal sealed class UIItemMonCardIDesEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UIItemMonCardIDes;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UIItemMonCardIDes>();
        }
    }

    public partial class UIItemMonCardIDes : UI, IAwake<int>
	{
		public int index;
		public void Initialize(int i)
		{
			index = i;
		}
		protected override void OnClose()
		{
			base.OnClose();
		}


    }
}
