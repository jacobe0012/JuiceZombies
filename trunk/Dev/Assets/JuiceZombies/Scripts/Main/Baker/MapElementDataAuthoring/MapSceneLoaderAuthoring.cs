// using System.Collections.Generic;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Mathematics;
// using Unity.Entities;
// using UnityEngine;
// using Unity.Transforms;
// using Unity.Entities.Serialization;
// /// <summary>
// /// ��ͼ���ص����ݺ決
// /// </summary>
//
//
// public class MapLoaderAuthoring : MonoBehaviour
// {
//     /// <summary>
//     /// �����ӳ�������mapSceneLoader������ӳ�������
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

