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
	[UIEvent(UIType.UICommonDamageBar)]
    internal sealed class UICommonDamageBarEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UICommonDamageBar;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UICommonDamageBar>();
        }
    }

    public partial class UICommonDamageBar : UI, IAwake<int>
	{
		public int index;
		public void Initialize()
		{
			
			 InitNode();
		}
		 void InitNode()
		{
			var KImgBar = GetFromReference(UICommonDamageBar.KImgBar);
			var KTextBar = GetFromReference(UICommonDamageBar.KTextBar);
			var KText_Number = GetFromReference(UICommonDamageBar.KText_Number);
			var KText_Percent = GetFromReference(UICommonDamageBar.KText_Percent);
			var KImgColor = GetFromReference(UICommonDamageBar.KImgColor);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}

        public void Initialize(int v)
        {
			index = v;
			
        }
    }
}
