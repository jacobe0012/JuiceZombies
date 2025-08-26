//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2024-03-12 14:27:55
//---------------------------------------------------------------------

using Spine.Unity;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Video;

namespace Main
{
    // public class HybridSpineDataAuthoringBaker : Baker<SkeletonAnimation>
    // {
    //     public override void Bake(SkeletonAnimation authoring)
    //     {
    //         BakingUtility.AddAdditionalCompanionComponentType(typeof(SkeletonAnimation));
    //         var entity = GetEntity(TransformUsageFlags.Dynamic);
    //         //var skeletonAnimation = GetComponent<AnimationMono>();
    //
    //         // AddComponent(entity, new SpineHybridData
    //         // {
    //         //     go = authoring.go,
    //         //     skeletonAnimation = authoring.skeletonAnimation
    //         // });
    //         AddComponentObject(entity, authoring);
    //         // AddComponentObject(entity, authoring.GetComponent<MeshFilter>());
    //         // AddComponentObject(entity, authoring.GetComponent<MeshRenderer>());
    //     }
    // }

   

    // public class HybridSpineDataAuthoring : MonoBehaviour
    // {
    //     //[SerializeField] public SpineHybridData spineHybridData;
    //     public GameObject go;
    //     public SkeletonAnimation skeletonAnimation;
    //
    //     public class HybridSpineDataAuthoringBaker : Baker<HybridSpineDataAuthoring>
    //     {
    //         public override void Bake(HybridSpineDataAuthoring authoring)
    //         {
    //             var entity = GetEntity(TransformUsageFlags.Dynamic);
    //             //var skeletonAnimation = GetComponent<AnimationMono>();
    //             BakingUtility.AddAdditionalCompanionComponentType(typeof(SkeletonAnimation));
    //             // AddComponent(entity, new SpineHybridData
    //             // {
    //             //     go = authoring.go,
    //             //     skeletonAnimation = authoring.skeletonAnimation
    //             // });
    //             AddComponentObject(entity, authoring.skeletonAnimation);
    //         }
    //     }
    // }
}