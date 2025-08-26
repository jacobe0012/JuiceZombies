// //---------------------------------------------------------------------
// // UnicornStudio
// // Author: jaco0012
// // Time: 2023-07-15 12:00:25
// //---------------------------------------------------------------------
//
// using Unity.Burst;
// using Unity.Entities;
// using UnityEngine.SceneManagement;
//
// namespace Main
// {
//     [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
//     [UpdateInGroup(typeof(PresentationSystemGroup))]
//     public partial struct StartToLoadSceneSystem : ISystem
//     {
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<StartToLoadSceneTag>();
//         }
//
//
//         public void OnUpdate(ref SystemState state)
//         {
//             foreach (var sceneData in SystemAPI.Query<RefRW<SwitchSceneData>>())
//             {
//                 if (!sceneData.ValueRO.startedLoad)
//                 {
//                     // SceneManager.LoadSceneAsync("RunTime",
//                     //     LoadSceneMode.Single);
//
//                     sceneData.ValueRW.scene.LoadAsync(new Unity.Loading.ContentSceneParameters()
//                     {
//                         loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single
//                     });
//                     sceneData.ValueRW.startedLoad = true;
//                 }
//             }
//         }
//     }
// }

