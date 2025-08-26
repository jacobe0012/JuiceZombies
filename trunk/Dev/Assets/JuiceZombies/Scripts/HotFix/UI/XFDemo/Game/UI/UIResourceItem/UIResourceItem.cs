//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIResourceItem)]
    internal sealed class UIResourceItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIResourceItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;


        public override UI OnCreate()
        {
            return UI.Create<UIResourceItem>();
        }
    }

    public partial class UIResourceItem : UI, IAwake<Vector3>
    {
        public void Initialize(Vector3 reward)
        {
            var tag_func = ConfigManager.Instance.Tables.Tbtag_func;
            var icon = GetFromReference(KResPic);
            if (reward.x == 0)
            {
                icon.GetImage().SetSpriteAsync("icon_func_3606", false).Forget();
                return;
            }

            #region �ƽ�����

            if (reward.x == -1)
            {
                icon.GetImage().SetSpriteAsync("icon_score_star", false).Forget();
            }

            #endregion

            UnicornUIHelper.SetIconOnly(reward, icon);
        }

        // public void OnDestroyUI()
        // {
        //     UnityEngine.GameObject.Destroy(this.GameObject);
        // }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}