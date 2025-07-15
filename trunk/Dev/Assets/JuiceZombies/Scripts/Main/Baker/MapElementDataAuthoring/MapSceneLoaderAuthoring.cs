// using System.Collections.Generic;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Mathematics;
// using Unity.Entities;
// using UnityEngine;
// using Unity.Transforms;
// using Unity.Entities.Serialization;
// /// <summary>
// /// 地图加载的数据烘焙
// /// </summary>
//
//
// public class MapLoaderAuthoring : MonoBehaviour
// {
//     /// <summary>
//     /// 保存子场景并在mapSceneLoader中添加子场景引用
//     /// </summary>
//     public UnityEditor.SceneAsset mapScene;
//     class MapLoaderBaker : Baker<MapLoaderAuthoring>
//     {
//         public override void Bake(MapLoaderAuthoring authoring)
//         {
//             var reference = new EntitySceneReference(authoring.mapScene);
//             var entity = GetEntity(TransformUsageFlags.None);
//             AddComponent(entity, new MapSceneLoader
//             {
//                 sceneRefer = reference
//             });
//         }
//     }
// }
//
//
// public struct MapSceneLoader : IComponentData
// {
//     public EntitySceneReference sceneRefer;
// }

