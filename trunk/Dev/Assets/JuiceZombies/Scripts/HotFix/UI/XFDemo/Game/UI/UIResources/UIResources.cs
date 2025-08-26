//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2025-08-19 00:30:10
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UIResources)]
    internal sealed class UIResourcesEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UIResources;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UIResources>();
        }
    }

    public partial class UIResources : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KTextEnergy = GetFromReference(UIResources.KTextEnergy);
			var KTextGem = GetFromReference(UIResources.KTextGem);
			var KTextGold = GetFromReference(UIResources.KTextGold);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
