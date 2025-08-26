//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2024-06-12 11:10:33
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    [MaterialProperty("_WarningData")]
    public struct JiYuATKRangeWarningData : IComponentData
    {
        /// <summary>
        /// x: 角度
        /// y: 开始时间 单位s
        /// z: 总持续时间
        /// w: 同时存在几个波
        /// </summary>
        public float4 value;
    }


    public class JiYuATKRangeWarningDataAuthoring : MonoBehaviour
    {
        //public float4 value;

        public class JiYuATKRangeWarningDataAuthoringBaker : Baker<JiYuATKRangeWarningDataAuthoring>
        {
            public override void Bake(JiYuATKRangeWarningDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new JiYuATKRangeWarningData
                {
                    value = new float4(100, 0, 3, 1)
                });
                // AddComponent(entity, new JiYuStartTime()
                //     { });
                // AddComponent(entity, new JiYuPivot
                // {
                //     value = 0.5f
                // });
            }
        }
    }
}