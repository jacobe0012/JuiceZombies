//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Collection_Unlock)]
    internal sealed class UIPanel_Collection_UnlockEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Collection_Unlock;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Collection_Unlock>();
        }
    }

    public partial class UIPanel_Collection_Unlock : UI, IAwake<int>
    {
        public int monsterId;
        private Tbmonster tbmonster;
        private Tblanguage tblanguage;
        private Tbpower tbpower;
        private Tbweapon tbweapon;
        private Tbracist tbracist;
        private Tbmonster_attr tbmonster_attr;

        public bool playAnim = false;

        public void Initialize(int args)
        {
            monsterId = args;
            //PlayAnim().Forget();
            // InitJson();
            // var KRewardRoot = GetFromReference(UIPanel_Collection_Unlock.KRewardRoot);
            // var KAnimalRoot = GetFromReference(UIPanel_Collection_Unlock.KAnimalRoot);
            //
            //
            // var monster = tbmonster.Get(monsterId);
            //
            // var skeletonGraphic = KAnimalRoot.GetComponent<SkeletonGraphic>();
            // skeletonGraphic.skeletonDataAsset =
            //     await ResourcesManager.LoadAssetAsync<SkeletonDataAsset>("WOLF_monster_1013_SkeletonData");
            //
            // skeletonGraphic.Initialize(true);
            // skeletonGraphic.Skeleton.SetSkin(tbmonster_attr.Get(monster.monsterAttrId).model);
            // skeletonGraphic.Skeleton.SetSlotsToSetupPose(); // 重置插槽姿势
            // skeletonGraphic.Skeleton.SetAttachment("weapon", monster.monsterWeaponId.ToString());
            // this.SetActive(true);
            // while (true)
            // {
            //     int index = Random.Range(0, skeletonGraphic.SkeletonData.Animations.Items.Length);
            //     string randomAnimationName =
            //         skeletonGraphic.SkeletonData.Animations.Items[index].Name;
            //     float duration =
            //         skeletonGraphic.SkeletonData.Animations.Items[index].Duration;
            //     skeletonGraphic.AnimationState.SetAnimation(0, randomAnimationName, false);
            //
            //     //var skeletonData = skeletonGraphic.skeletonDataAsset.GetSkeletonData(false);
            //     // 获取动画信息
            //     //var animation = skeletonData.FindAnimation(randomAnimationName);
            //     //var time = skeletonGraphic.AnimationState.GetCurrent(0).AnimationTime;
            //
            //     await UniTask.Delay((int)(duration * 1000) + 500);
            //     Log.Error($"{randomAnimationName} finished");
            //     //skeletonGraphic.AnimationState.SetAnimation(0, "Player_Stand", true);
            // }
        }

        public async UniTaskVoid PlayAnim()
        {
            var KImg_Icon = this.GetFromReference(UIPanel_Collection_Unlock.KImg_Icon);
            var ImgRec = KImg_Icon.GetRectTransform();
            const float StartY = -120f;

            const float OffsetY = 50f;
            //const float EndY = 100-200;

            const float Duration = 1;
            while (playAnim)
            {
                ImgRec.DoAnchoredPositionY(StartY, StartY - OffsetY, Duration);
                await UniTask.Delay((int)(Duration * 1000));
                ImgRec.DoAnchoredPositionY(StartY - OffsetY, StartY, Duration);
                await UniTask.Delay((int)(Duration * 1000));
            }
        }

        void InitJson()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbpower = ConfigManager.Instance.Tables.Tbpower;
            tbmonster = ConfigManager.Instance.Tables.Tbmonster;
            tbweapon = ConfigManager.Instance.Tables.Tbweapon;
            tbracist = ConfigManager.Instance.Tables.Tbracist;
            tbmonster_attr = ConfigManager.Instance.Tables.Tbmonster_attr;
        }

        protected override void OnClose()
        {
            playAnim = false;
            base.OnClose();
        }
    }
}