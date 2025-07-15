using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    public class EnemyAttackDataAuthoring : MonoBehaviour
    {
        class EnemyAttackDataBaker : Baker<EnemyAttackDataAuthoring>
        {
            public override void Bake(EnemyAttackDataAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new EnemyAttackData
                {
                });
            }
        }
    }

    public struct HitBackData : IComponentData
    {
        /// <summary>
        /// id  由什么技能实体造成的推力
        /// </summary>
        public int id;

        public int hitTimes;
        public Entity attacker;
        public bool isLittleEnemyAttack;
        public float3 dirPushforce;
    }

    /// <summary>
    /// 无视击退 tag(不无视伤害) 效果类似于质量无限大
    /// </summary>
    public struct IgnoreHitBackData : IComponentData
    {
        /// <summary>
        /// 0:无视碰撞和伤害 1:无视伤害 2：无视碰撞 
        /// </summary>
        public int IgnoreType;
    }


    public struct EnemyAttackData : IComponentData
    {
        public int pushForce;
        public int atk;
    }
}