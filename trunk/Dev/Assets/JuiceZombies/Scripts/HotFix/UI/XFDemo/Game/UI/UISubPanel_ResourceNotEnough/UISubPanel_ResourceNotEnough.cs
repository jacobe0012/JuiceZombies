//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using cfg.config;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_ResourceNotEnough)]
    internal sealed class UISubPanel_ResourceNotEnoughEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_ResourceNotEnough;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_ResourceNotEnough>();
        }
    }

    public partial class UISubPanel_ResourceNotEnough : UI, IAwake<Vector3>
    {
        private Tblanguage tblanguage;
        private Tbuser_variable tbuser_Variable;
        private Tbitem tbItem;

        public void Initialize(Vector3 v3)
        {
            InitJson();
            Init(v3);
        }

        private void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbuser_Variable = ConfigManager.Instance.Tables.Tbuser_variable;
            tbItem = tbItem = ConfigManager.Instance.Tables.Tbitem;
        }

        private void SetText(Vector3 v3)
        {

            string str1 = tblanguage.Get("common_state_buy").current;
            var str = JiYuUIHelper.GetRewardTextIconName(v3);
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
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () => { GoToChongZhi(); });
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
            JiYuUIHelper.GoToSomePanel(str);
            Close();
            JiYuUIHelper.DestroyAllSubPanel();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}