using Unity.Entities;
using UnityEngine;

namespace Main
{
    public class BanBulletTriggerBufferAuthoring : MonoBehaviour
    {
        class BanBulletTriggerBufferAuthoringBaker : Baker<BanBulletTriggerBufferAuthoring>
        {
            public override void Bake(BanBulletTriggerBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddBuffer<BanBulletTriggerBuffer>(entity);
            }
        }
    }

    public struct BanBulletTriggerBuffer : IBufferElementData
    {
        public int id;
        public int curTick;
        public float duration;
    }
}