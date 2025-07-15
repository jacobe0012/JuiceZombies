//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using Main;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPlayerHp)]
    internal sealed class UIPlayerHpEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPlayerHp;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIPlayerHp>();
        }
    }

    public partial class UIPlayerHp : UI, IAwake
    {
        private const int updateInternal = 200;
        private EntityManager entityManager;
        private EntityQuery entityQuery;
        private long timerId;

        public void Initialize()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityQuery = entityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            StartTimer();
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(updateInternal, this.Update);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }

        void Update()
        {
            if (entityQuery.IsEmpty) return;
            var chaStats = entityQuery.ToComponentDataArray<ChaStats>(Allocator.Temp)[0];
            var playerData = entityQuery.ToComponentDataArray<PlayerData>(Allocator.Temp)[0];

            var hpRatios = (float)chaStats.chaResource.hp / (float)chaStats.chaProperty.maxHp;

            var hpBarImg = GetFromReference(KHpBarImg);
            hpBarImg.GetImage().DoFillAmount(hpRatios, 0.3f);


            var coolDownRatios =
                (playerData.playerOtherData.weaponSkillCoolDown - playerData.playerOtherData.curWeaponSkillCoolDown) /
                (float)playerData.playerOtherData.weaponSkillCoolDown;
            var coolDownBarImg = GetFromReference(KCoolDownBarImg);
            if (!float.IsNaN(coolDownRatios))
            {
                coolDownBarImg.GetImage().DoFillAmount(coolDownRatios, 0.15f);
            }
        }

        protected override void OnClose()
        {
            Log.Debug("RemoveTimer", Color.cyan);
            RemoveTimer();
            base.OnClose();
        }
    }
}