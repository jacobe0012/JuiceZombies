//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UICommon_EquipItemWeapon)]
    internal sealed class UICommon_EquipItemWeaponEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UICommon_EquipItemWeapon;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UICommon_EquipItemWeapon>();
        }
    }

    public partial class UICommon_EquipItemWeapon : UI, IAwake<MyGameEquip, UIPanel_Equipment.EquipPanelType>
	{	
		public long uid;

		public MyGameEquip equip;
		//Type表示装备在装备界面还是合成界面 1装备界面 2合成界面 3合成界面点击一次 4在装备界面的通用合成材料 5已装备的装备

		public void Initialize(MyGameEquip myGameEquip, UIPanel_Equipment.EquipPanelType panelType)
		{
			uid = myGameEquip.equip.PartId;
			equip = myGameEquip;
			UnicornUIHelper.SetEquipIcon(myGameEquip, this, panelType);
		}

		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
