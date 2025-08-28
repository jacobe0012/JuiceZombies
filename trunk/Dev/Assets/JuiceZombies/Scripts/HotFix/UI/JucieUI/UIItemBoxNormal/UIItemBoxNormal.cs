//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: 2025-08-26 22:44:06
//---------------------------------------------------------------------

using cfg.config;
using HotFix_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
	[UIEvent(UIType.UIItemBoxNormal)]
    internal sealed class UIItemBoxNormalEvent : AUIEvent
    {
	    public override string Key => UIPathSet.UIItemBoxNormal;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => false;
		
		// 此UI是不受UIManager管理的
		
        public override UI OnCreate()
        {
            return UI.Create<UIItemBoxNormal>();
        }
    }

    public partial class UIItemBoxNormal : UI, IAwake<int>
	{

		public int currentBoxId;
        private cfg.config.TbHeroBox tbheroBox;
        private Tblanguage tblanguage;

        public void Initialize(int boxId)
		{
            currentBoxId = boxId;
            InitConfig();
            InitNode();
            InitView();
		}

        private void InitConfig()
        {
            tbheroBox=ConfigManager.Instance.Tables.TbHeroBox;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        }

        private void InitView()
        {
            var KBg_Mask = GetFromReference(UIItemBoxNormal.KBg_Mask);
            var KBorder = GetFromReference(UIItemBoxNormal.KBorder);
            var KText_Title = GetFromReference(UIItemBoxNormal.KText_Title);
            var KText_Description = GetFromReference(UIItemBoxNormal.KText_Description);
            var KImg_Box = GetFromReference(UIItemBoxNormal.KImg_Box);
            KText_Title.GetTextMeshPro().SetTMPText(tblanguage.Get(tbheroBox.Get(currentBoxId).name).current);
            KText_Description.GetTextMeshPro().SetTMPText(tblanguage.Get(tbheroBox.Get(currentBoxId).desc).current);
            KImg_Box.GetImage().SetSpriteAsync(tbheroBox.Get(currentBoxId).icon, false);
            switch (currentBoxId)
			{
				case 1:
                    KBg_Mask.GetImage().SetColor("14D492");
                    KBorder.GetImage().SetColor("46FFA1");
                    //KImg_Box.sprite = ResManager.Instance.Load<Sprite>("Textures/ItemBoxes/BeginnerBox");
                    break;
				case 2:
                    KBg_Mask.GetImage().SetColor("14D492");
                    KBorder.GetImage().SetColor("46FFA1");
                    break;
				case 3:
                    KBg_Mask.GetImage().SetColor("FFDD00");
                    KBorder.GetImage().SetColor("FDF9DD");
                    break;
				default:
				break;
            }
        }

        void InitNode()
		{
			var KBg_Mask = GetFromReference(UIItemBoxNormal.KBg_Mask);
			var KBorder = GetFromReference(UIItemBoxNormal.KBorder);
			var KText_Title = GetFromReference(UIItemBoxNormal.KText_Title);
			var KText_Description = GetFromReference(UIItemBoxNormal.KText_Description);
			var KImg_Box = GetFromReference(UIItemBoxNormal.KImg_Box);
			var KGroup_Btn = GetFromReference(UIItemBoxNormal.KGroup_Btn);
		}
		protected override void OnClose()
		{
			base.OnClose();
		}
	}
}
