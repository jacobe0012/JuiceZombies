//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2024-03-12 14:27:55
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    public class JiYuChromAberrEnableAuthoring : MonoBehaviour
    {
        public class JiYuChromAberrEnableAuthoringBaker : Baker<JiYuChromAberrEnableAuthoring>
        {
            public override void Bake(JiYuChromAberrEnableAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuChromAberrEnable
                {
                    value = 0
                });
            }
        }
    }

    /// <summary>
    /// 启用残影
    /// </summary>
    [MaterialProperty("_ChromAberrEnable")]
    public struct JiYuChromAberrEnable : IComponentData
    {
        public int value;
    }
}