using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class PlayerDataAuthoring : MonoBehaviour
    {
        //[SerializeField] public GameObject hybridGO;
        [SerializeField] public ChaStats chaStats;

        [SerializeField] public PlayerData playerData;
        //public Transform playerTransform;

        public class PlayerDataBaker : Baker<PlayerDataAuthoring>
        {
            public override void Bake(PlayerDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, authoring.playerData);
                // AddBuffer<State>(entity);
                // AddComponent<StateMachine>(entity);
                AddComponent<InputData>(entity);
                AddComponent(entity, authoring.chaStats);

                //AddBuffer<BuffOld>(entity);
                AddBuffer<Buff>(entity);
                AddBuffer<BanBulletTriggerBuffer>(entity);
                //AddBuffer<PlayerEquipSkillBuffer>(entity);
                AddComponent<BulletSonData>(entity);
                AddBuffer<Skill>(entity);
                AddBuffer<Trigger>(entity);
                AddBuffer<GameEvent>(entity);
                AddBuffer<PlayerProps>(entity);
                AddBuffer<BulletCastData>(entity);
                AddComponent<FlipData>(entity);
                AddComponent<EntityGroupData>(entity);
                AddComponent(entity, new TargetData
                {
                    tick = 0,
                    BelongsTo = (uint)BuffHelper.TargetEnum.Player,
                    AttackWith = 0
                });
            }
        }
    }

    public struct SpiritData : IComponentData
    {
    }
}