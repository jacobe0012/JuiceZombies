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
    public class JiYuOutlineEnableAuthoring : MonoBehaviour
    {
        public class JiYuOutlineEnableAuthoringBaker : Baker<JiYuOutlineEnableAuthoring>
        {
            public override void Bake(JiYuOutlineEnableAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuOutlineEnable
                {
                    value = 0
                });
            }
        }
    }

    /// <summary>
    /// 启用描边
    /// </summary>
    [MaterialProperty("_OutlineEnable")]
    public struct JiYuOutlineEnable : IComponentData
    {
        public int value;
    }
}