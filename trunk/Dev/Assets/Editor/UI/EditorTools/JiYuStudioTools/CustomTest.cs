using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DigitalOpus.MB.Core;
using DigitalOpus.MB.MBEditor;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using GPUECSAnimationBaker.Engine.Baker;
using GpuEcsAnimationBaker.Engine.Data;
using Main;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Stateful;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


public class CustomTest
{
    [Title("输出路径:(默认为存放预制件的目录)")] [FolderPath]
    private string outputPath = "Assets/ApesGang/Prefabs/Monster/Animator2GPUAnimTempFloder";

    [Title("预烘焙的预制件列表:")] public List<GameObject> GameObjects;

    private string prefabPath = "Assets/ApesGang/Prefabs/Monster/Prefab/Normal";

    private string sharedTexturePath = "Assets/ApesGang/Shaders/JiYuShaders/Texture/JiYuShaderTextureArray.png";
    private string disloveTexturePath = "Assets/ApesGang/Shaders/JiYuShaders/Texture/seamlessNoise.png";

    [Button(name: "一键烘焙")]
    public void Button()
    {
        if (GameObjects.Count < 1)
        {
            Debug.LogError($"没有要烘焙的预制件");
            return;
        }

        // if (outputPath == "")
        // {
        //     outputPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(skeletonDataAsset))
        //         .Replace('\\', '/') + "/Baked";
        //     System.IO.Directory.CreateDirectory(outputPath);
        // }
        // else
        // {
        // }
        //System.IO.Directory.CreateDirectory(outputPath);
        //System.IO.Directory.CreateDirectory(outputPath + "/mesh");
        // var meshBakerGO = MB3_MeshBakerEditor.CreateNewMeshBaker();
        // var textureBaker = meshBakerGO.GetComponent<MB3_TextureBaker>();
        // var meshBakerGrouper = meshBakerGO.GetComponent<MB3_MeshBakerGrouper>();
        // var meshBaker = meshBakerGO.GetComponentInChildren<MB3_MeshBaker>();
        // var list = textureBaker.GetObjectsToCombine();
        //
        // meshBakerGrouper.GetMeshBakerSettings().renderType = MB_RenderType.skinnedMeshRenderer;
        // meshBaker.meshCombiner.outputOption = MB2_OutputOptions.bakeIntoPrefab;
        // meshBaker.meshCombiner.settings.optimizeAfterBake = false;
        // meshBakerGrouper.GetMeshBakerSettings().clearBuffersAfterBake = true;

        foreach (var go in GameObjects)
        {
            //go.transform.GetChild(0).gameObject.AddComponent<RenderingEntityTagAuthoring>();
            // var ratios = go.transform.GetChild(0).transform.localScale.x / 0.5f;
            // go.transform.localScale = new Vector3(ratios, ratios, ratios);
            // go.transform.GetChild(0).transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(go) as GameObject;

            if (prefabInstance != null)
            {
                // 创建一个新的 GameObject 作为子节点
                // var child = new GameObject("RenderingEntityGO");
                // // 设置子节点为父节点的子对象
                // child.transform.SetParent(prefabInstance.transform);

                var phys = prefabInstance.transform.GetChild(0);
                // for (int i = 0; i < phys.childCount; i++)
                // {
                //     GameObject.DestroyImmediate(phys.GetChild(i).gameObject);
                // }

                for (int i = 0; i < 3; i++)
                {
                    GameObject child = new GameObject($"CustomPos_{i}");
                    child.transform.SetParent(phys);
                    child.transform.localPosition = Vector3.zero;
                    child.transform.localScale = Vector3.one;
                    child.transform.localRotation = Quaternion.identity;
                }


                //prefabInstance.AddComponent<StatefulCollisionEventAuthoring>();
                //var physicsShapeAuthoring = prefabInstance.GetComponent<PhysicsShapeAuthoring>();
                // var meshFilter = prefabInstance.GetComponent<MeshFilter>();
                // var jiYuSortAuthoring = prefabInstance.GetComponent<JiYuSortAuthoring>();
                // var jiYuPivotAuthoring = prefabInstance.GetComponent<JiYuPivotAuthoring>();


                // var meshRenderer = prefabInstance.GetComponent<MeshRenderer>();
                // var meshFilter = prefabInstance.GetComponent<MeshFilter>();
                // var jiYuSortAuthoring = prefabInstance.GetComponent<JiYuSortAuthoring>();
                // var jiYuPivotAuthoring = prefabInstance.GetComponent<JiYuPivotAuthoring>();


                //
                // var MeshRenderer1 = child.AddComponent<MeshRenderer>();
                // var MeshFilter1 = child.AddComponent<MeshFilter>();
                // var JiYuSortAuthoring1 = child.AddComponent<JiYuSortAuthoring>();
                // var JiYuPivotAuthoring1 = child.AddComponent<JiYuPivotAuthoring>();
                // child.AddComponent<RenderingEntityTagAuthoring>();

                // MeshRenderer1 = meshRenderer;
                // MeshFilter1 = meshFilter;
                // JiYuSortAuthoring1 = jiYuSortAuthoring;
                // JiYuPivotAuthoring1 = jiYuPivotAuthoring;
                // EditorUtility.CopySerialized(meshRenderer, MeshRenderer1);
                // EditorUtility.CopySerialized(meshFilter, MeshFilter1);
                // EditorUtility.CopySerialized(jiYuSortAuthoring, JiYuSortAuthoring1);
                // EditorUtility.CopySerialized(jiYuPivotAuthoring, JiYuPivotAuthoring1);
                //
                //GameObject.DestroyImmediate(child.gameObject);
                // GameObject.DestroyImmediate(meshFilter);
                // GameObject.DestroyImmediate(jiYuSortAuthoring);
                // GameObject.DestroyImmediate(jiYuPivotAuthoring);

                // 保存修改后的 Prefab 实例到 Prefab 资源
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, AssetDatabase.GetAssetPath(go));

                // 销毁实例（如果需要）
                GameObject.DestroyImmediate(prefabInstance);

                // var ratios = go.transform.GetChild(0).transform.localScale.x / 0.5f;
                // go.transform.localScale = new Vector3(ratios, ratios, ratios);
                // go.transform.GetChild(0).transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

        Debug.Log($"Finished");
        AssetDatabase.Refresh();
        //Object obj = AssetDatabase.LoadMainAssetAtPath(outputPath);
        //EditorGUIUtility.PingObject(obj);
    }
}