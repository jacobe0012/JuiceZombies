using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    public class JiYuFrameAnimLoopAuthoring : MonoBehaviour
    {
        public int JiYuFrameAnimLoop = 0;
        class JiYuFrameAnimLoopAuthoringBaker : Baker<JiYuFrameAnimLoopAuthoring>
        {
            public override void Bake(JiYuFrameAnimLoopAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new JiYuFrameAnimLoop
                {
                    value = authoring.JiYuFrameAnimLoop
                });
            }
        }
    }

    [MaterialProperty("_JiYuFrameAnimLoop")]
    public struct JiYuFrameAnimLoop : IComponentData
    {
        public int value;
    }
}