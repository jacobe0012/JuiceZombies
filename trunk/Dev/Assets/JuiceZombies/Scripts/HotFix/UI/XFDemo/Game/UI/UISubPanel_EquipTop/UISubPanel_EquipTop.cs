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
	[UIEvent(UIType.UISubPanel_EquipTop)]
    internal sealed class UISubPanel_EquipTopEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UISubPanel_EquipTop;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_EquipTop>();
        }
    }

    public partial class UISubPanel_EquipTop : UI, IAwake
	{	
		public void Initialize()
		{
			 InitNode();
		}
		 void InitNode()
		{
			var KPlayerAnim = GetFromReference(UISubPanel_EquipTop.KPlayerAnim);
			var KText_PlayerLevel = GetFromReference(UISubPanel_EquipTop.KText_PlayerLevel);
			var KText_PlayerName = GetFromReference(UISubPanel_EquipTop.KText_PlayerName);
			var KBtn_PlayerInfo = GetFromReference(UISubPanel_EquipTop.KBtn_PlayerInfo);
			var KImg_LeftAtk = GetFromReference(UISubPanel_EquipTop.KImg_LeftAtk);
			var KText_MidAtk = GetFromReference(UISubPanel_EquipTop.KText_MidAtk);
			var KText_RightAtkNum = GetFromReference(UISubPanel_EquipTop.KText_RightAtkNum);
			var KImg_LeftHp = GetFromReference(UISubPanel_EquipTop.KImg_LeftHp);
			var KText_MidHp = GetFromReference(UISubPanel_EquipTop.KText_MidHp);
			var KText_RightHpNum = GetFromReference(UISubPanel_EquipTop.KText_RightHpNum);
			var KImg_Exp = GetFromReference(UISubPanel_EquipTop.KImg_Exp);
			var KRight = GetFromReference(UISubPanel_EquipTop.KRight);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
