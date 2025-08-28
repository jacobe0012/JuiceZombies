//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using HotFix_UI;
using System;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_ResourceNotEnough)]
    internal sealed class UICommon_ResourceNotEnoughEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_ResourceNotEnough;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_ResourceNotEnough>();
        }
    }

    public partial class UICommon_ResourceNotEnough : UI, IAwake<Vector3>
    {
        private Tblanguage tblanguage;
        private Tbuser_variable tbuser_Variable;
        private TbitemOld tbItem;

        public void Initialize(Vector3 v3)
        {
            InitJson();
            Init(v3);
        }

        private void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbuser_Variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbItem = tbItem = ConfigManager.Instance.Tables.TbitemOld;
        }

        private void SetText(Vector3 v3)
        {
            //this.GetFromReference(KText).GetTextMeshPro().SetTMPText("");
            //this.GetFromReference(KText_Num).GetTextMeshPro().SetTMPText(v3.z.ToString());

            //if (v3.x != 2 && v3.x != 3)
            //{
            //    Debug.Log("input is wrong");
            //    Close();
            //}
            this.GetFromReference(KSubPanel_CommonBtn).GetFromReference(UISubPanel_CommonBtn.KText_Mid).SetActive(true);
            string str1 = tblanguage.Get("common_state_buy").current;
            var str = UnicornUIHelper.GetRewardTextIconName(v3);
            if (v3.x == 2)
            {
                string str2 = tblanguage.Get(tbuser_Variable.Get(2).name).current;
                this.GetFromReference(KSubPanel_CommonBtn).GetFromReference(UISubPanel_CommonBtn.KText_Mid)
                    .GetTextMeshPro().SetTMPText(str1 + str2);
                string notEnoughStr = tblanguage.Get("common_lack_2_desc").current;
                var neStr = notEnoughStr.Replace("{0}", str + v3.z.ToString());
                this.GetFromReference(KText).GetTextMeshPro().SetTMPText(neStr);
            }

            else if (v3.x == 3)
            {
                string str3 = tblanguage.Get(tbuser_Variable.Get(3).name).current;
                this.GetFromReference(KSubPanel_CommonBtn).GetFromReference(UISubPanel_CommonBtn.KText_Mid)
                    .GetTextMeshPro().SetTMPText(str1 + str3);
                string notEnoughStr = tblanguage.Get("common_lack_3_desc").current;
                var neStr = notEnoughStr.Replace("{0}", str + v3.z.ToString());
                this.GetFromReference(KText).GetTextMeshPro().SetTMPText(neStr);
            }
            else
            {
                int itemId = (int)v3.y;
                string str3 = tblanguage.Get(tbItem.Get(itemId).name).current;
                this.GetFromReference(KSubPanel_CommonBtn).GetFromReference(UISubPanel_CommonBtn.KText_Mid)
                    .GetTextMeshPro().SetTMPText(str1 + str3);
                string notEnoughStr = tblanguage.Get("common_lack_3_desc").current;
                var neStr = notEnoughStr.Replace("{0}", str + v3.z.ToString());
                this.GetFromReference(KText).GetTextMeshPro().SetTMPText(neStr);
            }

            this.GetFromReference(KSubPanel_CommonBtn).GetFromReference(UISubPanel_CommonBtn.KText_Mid)
                .SetActive(true);
        }

        private void SetImage(Vector3 v3)
        {
            //if (v3.x != 2 && v3.x != 3)
            //{
            //    Debug.Log("input is wrong");
            //    Close();
            //}

            //if (v3.x == 2)
            //{
            //    //this.GetFromReference(KImg_Resource).GetXImage().SetSprite(tbuser_Variable.Get(2).icon, false);
            //}

            //if (v3.x == 3)
            //{
            //    //this.GetFromReference(KImg_Resource).GetXImage().SetSprite(tbuser_Variable.Get(3).icon, false);
            //}

           
            this.GetFromReference(KSubPanel_CommonBtn).GetFromReference(UISubPanel_CommonBtn.KImg_RedDot)
                .SetActive(false);
        }

        private void BtnInit(Vector3 v3)
        {
            if (v3.x == 2 || v3.x == 3)
            {
                var btn = this.GetFromReference(KSubPanel_CommonBtn).GetFromReference(UISubPanel_CommonBtn.KBtn_Common);
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () => { GoToChongZhi(); });
            }
            else
            {
                this.GetFromReference(KSubPanel_CommonBtn).SetActive(false);
            }
        }

        public void Init(Vector3 v3)
        {
            SetText(v3);
            SetImage(v3);
            BtnInit(v3);
        }

        private void GoToChongZhi()
        {
            var str = "type=1;para=[141]";
            UnicornUIHelper.GoToSomePanel(str);
            Close();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}