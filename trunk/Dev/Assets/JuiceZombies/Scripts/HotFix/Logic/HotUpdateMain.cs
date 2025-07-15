//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-18 16:15:10
//---------------------------------------------------------------------

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HybridCLR;
using Main;
using UnityEngine;
using YooAsset;

namespace HotFix_Logic
{
    public class HotUpdateMain : MonoBehaviour
    {
        public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll",
            "UniTask.dll",
            "Unity.Entities.dll",
            "Unity.Collections.dll",
            "Unity.Transforms.dll",
            "Unity.Mathematics.dll",
            "Unity.Burst.dll",
            "ApesGang.ConfigGen.dll",
            "UnityEngine.ContentLoadModule.dll",
            "UnityEngine.CoreModule.dll",
            "DOTween.dll",
            "DOTween.Modules.dll",
            "DOTweenPro.Scripts.dll",
            "Google.Protobuf.dll",
            "Newtonsoft.Json.dll",
            "YooAsset.dll",
            "protobuf-net.Core.dll",
            "protobuf-net.dll",
            "spine-unity.dll",
            "FMODUnity.dll",
            "ProjectDawn.Navigation.dll",
            "Unity.Physics.Stateful.dll",
            "UnityEngine.PropertiesModule.dll",
            // "VoxelBusters.CoreLibrary.dll",
            // "VoxelBusters.EssentialKit.dll",
        };

        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        private async UniTask LoadMetadataForAOTAssemblies()
        {
            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            var package = YooAssets.GetPackage("DefaultPackage");
            var InitCover = UnityEngine.GameObject.Find("InitCover");
            float sum = 0;
            foreach (var aotDllName in AOTMetaAssemblyNames)
            {
                sum++;
                float ratios = sum / AOTMetaAssemblyNames.Count;
                if (sum == AOTMetaAssemblyNames.Count)
                {
                    ratios = 1;
                }

                LoadDllMono.ShowProgressUI(InitCover, ratios, "正在载入场景...");

                AssetHandle handle =
                    package.LoadAssetAsync<TextAsset>("Assets/ApesGang/HotFixDlls/" + aotDllName + ".bytes");

                await handle.ToUniTask();
                var textAsset = handle.AssetObject as TextAsset;

                byte[] dllBytes = textAsset.bytes;

                // var handle = package.LoadRawFileAsync("Assets/ApesGang/HotFixDlls/" + aotDllName + ".bytes");
                // await handle.Task;
                // byte[] dllBytes = handle.GetRawFileData();

                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                Debug.Log($"[{GetType().FullName}] LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
            }
        }

        /// <summary>
        /// 给AOT层补充元数据 
        /// </summary>
        private async UniTask InitTypeAndMetaData()
        {
#if !UNITY_EDITOR
            //TypeManager.InitializeAssembliesTypes(LoadDllMono.AssList.ToArray());
            //TypeManager.Shutdown();
            //TypeManager.Initialize();
#endif
            // TypeManager.Shutdown();
            // TypeManager.Initialize();
            //TypeManager.InitializeAssembliesTypes(LoadDllMono.AssList.ToArray());
            await LoadMetadataForAOTAssemblies();
        }


        private void Awake()
        {
            Debug.Log($"[{GetType().FullName}] InitTypeAndMetaData!");
            //InitTypeAndMetaData();
        }

        async UniTaskVoid Start()
        {
            await InitTypeAndMetaData();
            Debug.Log($"[{GetType().FullName}] LoadMetadataForAOTAssemblies!");

            //InitUnitySetting();
            await GoToUIScene();

            Debug.Log($"[{GetType().FullName}] Finished");
        }

        async UniTask GoToUIScene()
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var handle =
                package.LoadAssetAsync<GameObject>(
                    "Assets/ApesGang/Scripts/HotFix/HotFixPrefab/GameRoot.prefab");
            await handle.ToUniTask();

            GameObject go = handle.AssetObject as GameObject;

            Instantiate(go);
        }
    }
}