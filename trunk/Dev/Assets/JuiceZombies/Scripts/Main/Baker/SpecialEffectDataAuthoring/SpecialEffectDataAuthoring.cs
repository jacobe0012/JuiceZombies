//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: 2024-03-12 14:27:55
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    public class SpecialEffectDataAuthoring : MonoBehaviour
    {
        public class SpecialEffectDataAuthoringBaker : Baker<SpecialEffectDataAuthoring>
        {
            public override void Bake(SpecialEffectDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SpecialEffectData());
            }
        }
    }

    public struct SpecialEffectChainBuffer : IBufferElementData
    {
        public Entity Entity;
        //public float3 pos;
    }

    public struct SpecialEffectData : IComponentData
    {
        /// <summary>
        /// 0:默认读表 1:程序自行处理不读表/部分读表生成特殊类型的特效 2:闪电链生成请求
        /// </summary>
        public int type;

        //特效表id
        public int id;

        //状态特效
        public int stateId;
        public int groupId;
        public int sort;
        public Entity caster;
        public int tick;
        public int skillId;
        public float3 startPos;
        public float3 targetPos;

        public float duration;

        /// <summary>
        /// 闪电链的中心目标
        /// </summary>
        public Entity chainCenterEntity;

        /// <summary>
        /// 闪电链的前一个目标
        /// </summary>
        public Entity chainLastEntity;

        /// <summary>
        /// 闪电链的后一个目标
        /// </summary>
        public Entity chainNextEntity;

        public int startTick;
    }
}