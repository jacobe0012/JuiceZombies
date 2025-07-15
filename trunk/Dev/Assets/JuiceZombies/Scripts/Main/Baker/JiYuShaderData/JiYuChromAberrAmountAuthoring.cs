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
    public class JiYuChromAberrAmountAuthoring : MonoBehaviour
    {
        public class JiYuChromAberrAmountAuthoringBaker : Baker<JiYuChromAberrAmountAuthoring>
        {
            public override void Bake(JiYuChromAberrAmountAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuChromAberrAmount
                {
                    value = 0
                });
            }
        }
    }

    /// <summary>
    /// 启用残影
    /// </summary>
    [MaterialProperty("_ChromAberrAmount")]
    public struct JiYuChromAberrAmount : IComponentData
    {
        public float value;
    }
}