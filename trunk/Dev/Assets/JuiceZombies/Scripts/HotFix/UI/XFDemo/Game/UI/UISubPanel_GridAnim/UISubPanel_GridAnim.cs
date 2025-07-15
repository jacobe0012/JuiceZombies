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
    [UIEvent(UIType.UISubPanel_GridAnim)]
    internal sealed class UISubPanel_GridAnimEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_GridAnim;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_GridAnim>();
        }
    }

    public partial class UISubPanel_GridAnim : UI, IAwake
    {
        public void Initialize()
        {
            InitNode();
        }

        void InitNode()
        {
            var KAnim_Dust = GetFromReference(UISubPanel_GridAnim.KAnim_Dust);
            var KAnim_Shining = GetFromReference(UISubPanel_GridAnim.KAnim_Shining);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}