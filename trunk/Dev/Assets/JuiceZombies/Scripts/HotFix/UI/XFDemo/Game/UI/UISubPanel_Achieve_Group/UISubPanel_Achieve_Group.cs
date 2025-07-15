//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Achieve_Group)]
    internal sealed class UISubPanel_Achieve_GroupEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Achieve_Group;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Achieve_Group>();
        }
    }

    public partial class UISubPanel_Achieve_Group : UI, IAwake<int>
    {
        public int id;

        public void Initialize(int id)
        {
            this.id = id;
            //RedPointMgr.instance.Init(UIPanel_Battle.Battle_Red_Point_Root, "achieve_group" + id.ToString(),
            //    (RedPointState state, int data) =>
            //    {
            //        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Achieve, out UI uuii))
            //        {
            //            this.GetFromReference(KImg_RedPoint).SetActive(state == RedPointState.Show);
            //        }
            //        //Debug.Log("task group" + id.ToString() + " state is " + state.ToString());
            //    });
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}