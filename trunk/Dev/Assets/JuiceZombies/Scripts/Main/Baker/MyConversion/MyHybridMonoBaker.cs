using System.Collections.Generic;
using Unity.Transforms;
using Unity.Entities;

namespace Main
{
    public class MyHybridMonoBaker : Baker<MyHybridMono>
    {
        public override void Bake(MyHybridMono authoring)
        {
            //BakingUtility.AddAdditionalCompanionComponentType(typeof(MyHybridMono));
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            //var skeletonAnimation = GetComponent<AnimationMono>();

            //AddComponent(entity, new SpineHybridData());
            AddComponentObject(entity, authoring);
        }
    }
}