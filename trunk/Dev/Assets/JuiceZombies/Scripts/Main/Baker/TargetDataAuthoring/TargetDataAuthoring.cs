using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace Main
{
    /// <summary>
    /// 所有可作为目标的实体组件 根据target表读取
    /// </summary>
    public struct TargetData : IComponentData
    {
        /// <summary>
        /// 当前存在tick 帧
        /// </summary>
        public int tick;

        /// <summary>
        /// 代表它是什么目标
        /// </summary>
        public uint BelongsTo;

        /// <summary>
        /// 攻击类(技能实体)时代表可以攻击哪些目标(目标1+目标2...)
        /// </summary>
        public uint AttackWith;


        /// <summary>
        /// 分裂子弹击中后为了后续同时刻同技能失效的数据 技能id
        /// </summary>
        public int skillId;

        /// <summary>
        /// 分裂子弹击中后为了后续同时刻同技能失效的数据 添加时间
        /// </summary>
        public int addTime;
    }


    public class TargetDataAuthoring : MonoBehaviour
    {
        //public TargetEnum target;

        class TargetDataBaker : Baker<TargetDataAuthoring>
        {
            public override void Bake(TargetDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<TargetData>(entity);
            }
        }
    }
}