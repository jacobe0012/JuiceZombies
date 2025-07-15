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
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


public class Animator2GPUAnim
{
    [Title("输出路径:(默认为存放预制件的目录)")] [FolderPath]
    public string outputPath = "Assets/JuiceZombies/Prefabs/Monster/Animator2GPUAnimTempFloder";

    [Title("预烘焙的预制件列表:")] public List<GameObject> GameObjects;

    private string prefabPath = "Assets/JuiceZombies/Prefabs/Monster/Prefab/Normal";

    private string sharedTexturePath = "Assets/JuiceZombies/Shaders/JiYuShaders/Texture/JiYuShaderTextureArray.png";
    private string disloveTexturePath = "Assets/JuiceZombies/Shaders/JiYuShaders/Texture/seamlessNoise.png";

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
        System.IO.Directory.CreateDirectory(outputPath);
        //System.IO.Directory.CreateDirectory(outputPath + "/mesh");
        var meshBakerGO = MB3_MeshBakerEditor.CreateNewMeshBaker();
        var textureBaker = meshBakerGO.GetComponent<MB3_TextureBaker>();
        var meshBakerGrouper = meshBakerGO.GetComponent<MB3_MeshBakerGrouper>();
        var meshBaker = meshBakerGO.GetComponentInChildren<MB3_MeshBaker>();
        var list = textureBaker.GetObjectsToCombine();

        meshBakerGrouper.GetMeshBakerSettings().renderType = MB_RenderType.skinnedMeshRenderer;
        meshBaker.meshCombiner.outputOption = MB2_OutputOptions.bakeIntoPrefab;
        meshBaker.meshCombiner.settings.optimizeAfterBake = false;
        meshBakerGrouper.GetMeshBakerSettings().clearBuffersAfterBake = true;


        foreach (var go in GameObjects)
        {
            list.Clear();
            var renderers = go.GetComponentsInChildren<Renderer>(true);
            var renderList = renderers.ToList();
            renderList.Sort((a, b) => { return a.sortingOrder.CompareTo(b.sortingOrder); });
            foreach (var renderer in renderList)
            {
                Debug.Log($"name:{renderer.gameObject.name} sortingOrder:{renderer.sortingOrder}");
                list.Add(renderer.gameObject);
            }


            var newgo = new GameObject();
            var prefabpath = $"{outputPath}/{go.name}_Clone.prefab";
            var matpath = $"{outputPath}/{go.name}_Clone_Mat.asset";
            var assetprefab = PrefabUtility.SaveAsPrefabAsset(newgo, prefabpath);

            meshBaker.resultPrefab = assetprefab;


            SerializedObject so = null;
            bool createdDummyTextureBakeResults;
            foreach (var VARIABLE in textureBaker.GetObjectsToCombine())
            {
                Debug.Log($"name11:{VARIABLE.gameObject.name} ");
            }

            MB3_MeshBakerEditorFunctions.BakeIntoCombined(meshBaker, out createdDummyTextureBakeResults,
                ref so);


            var ins = GameObject.Instantiate(assetprefab);
            var animator = ins.GetComponentInChildren<Animator>();
            for (int i = 0; i < animator.gameObject.transform.childCount; i++)
            {
                var child = animator.gameObject.transform.GetChild(i);
                child.SetParent(ins.transform);
            }

            ComponentUtility.CopyComponent(animator);
            ComponentUtility.PasteComponentAsNew(ins);
            GameObject.DestroyImmediate(animator.gameObject);

            var newAnimator = ins.GetComponent<Animator>();
            var gpuEcsAnimationBakerBehaviour = ins.AddComponent<GpuEcsAnimationBakerBehaviour>();
            var clips = newAnimator.runtimeAnimatorController.animationClips;
            //排序动画
            clips.Sort((a, b) =>
            {
                string outputA = char.ToUpper(a.name[0]) + a.name.Substring(1);
                string outputB = char.ToUpper(b.name[0]) + b.name.Substring(1);
                var enumValueA = (int)(AnimationEnum)Enum.Parse(typeof(AnimationEnum), outputA);
                var enumValueB = (int)(AnimationEnum)Enum.Parse(typeof(AnimationEnum), outputB);

                return enumValueA.CompareTo(enumValueB);
            });

            gpuEcsAnimationBakerBehaviour.bakerData.animations = new AnimationData[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                gpuEcsAnimationBakerBehaviour.bakerData.animations[i] = new AnimationData
                {
                    animationID = $"{go.name}_{clips[i].name}",
                    animatorStateName = clips[i].name,
                    animationType = AnimationTypes.SingleClip,
                    singleClipData = new SingleClipData
                    {
                        animationClip = clips[i]
                    },
                    dualClipBlendData = default,
                    loop = !(clips[i].name.Contains("die") || clips[i].name.Contains("getHit") ||
                             clips[i].name.Contains("skill")),
                    additionalAnimatorParameterValues = new AnimatorParameter[]
                    {
                    },
                    additionalAnimatorStatesPerLayer = new AnimatorState[]
                    {
                    }
                };
            }


            var skinnedMeshRenderer = ins.GetComponentInChildren<SkinnedMeshRenderer>();

            var ecsGpuMaterial = new Material(Shader.Find("JiYuStudio/JiYuGPUAnimationOutlineTest"));
            var skinrenderer = go.GetComponentsInChildren<Renderer>(true);
            // foreach (var VARIABLE in skinrenderer)
            // {
            //     Debug.LogError($"{VARIABLE.name}");
            // }
            //Debug.LogError($"{skinrenderer[0].sharedMaterial.mainTexture.name}");
            var sharedtexture = AssetDatabase.LoadMainAssetAtPath(sharedTexturePath) as Texture2D;
            var disTexture2D = AssetDatabase.LoadMainAssetAtPath(disloveTexturePath) as Texture2D;

            ecsGpuMaterial.SetTexture("_MainTex", skinrenderer[0].sharedMaterial.mainTexture);

            //ecsGpuMaterial.SetFloat("_JiYuSort", 5);
            //var sharedtexturearray = sharedtexture as Texture2D;
            //ecsGpuMaterial.SetTexture("_JiYuTextureArray", sharedtexture);
            ecsGpuMaterial.SetTexture("_JiYuDissolveThreshold", disTexture2D);

            AssetDatabase.CreateAsset(ecsGpuMaterial, matpath);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
            var mat = AssetDatabase.LoadMainAssetAtPath(matpath) as Material;
            skinnedMeshRenderer.sharedMaterial = mat;

            var finalGo = PrefabUtility.SaveAsPrefabAsset(ins, prefabpath);

            //AssetDatabase.
            bool showPrefabError;
            string path = AssetDatabase.GetAssetPath(finalGo);
            if (string.IsNullOrEmpty(path) || !PrefabUtility.IsPartOfAnyPrefab(finalGo))
                showPrefabError = true;
            else
            {
                string folder = Path.GetDirectoryName(path);
                string subFolder = $"BakedAssets_{finalGo.name}";
                string generatedAssetsFolder = Path.Combine(folder, subFolder);
                if (!AssetDatabase.IsValidFolder(generatedAssetsFolder))
                    generatedAssetsFolder = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(
                        folder, subFolder));
                string animatorName = finalGo.name;
                var baker = finalGo.GetComponent<GpuEcsAnimationBakerBehaviour>();

                GpuEcsAnimationBakerData bakerData = (GpuEcsAnimationBakerData)baker.bakerData;
                GameObject newGpuEcsAnimator = GpuEcsAnimationBakerServices.GenerateAnimationObject(path, bakerData,
                    animatorName, generatedAssetsFolder);

                baker.gpuEcsAnimator = newGpuEcsAnimator;
                showPrefabError = false;

                var meshRenderer = newGpuEcsAnimator.GetComponentInChildren<MeshRenderer>();
                var meshFilter = newGpuEcsAnimator.GetComponentInChildren<MeshFilter>();
                var gpuEcsAnimatorBehaviour = newGpuEcsAnimator.GetComponentInChildren<GpuEcsAnimatorBehaviour>();


                var assetPath = AssetDatabase.GetAssetPath(newGpuEcsAnimator);
                Debug.Log($"f {assetPath}");
                string pattern = @"\d{4,}";
                MatchCollection matches = Regex.Matches(path, pattern);
                var id = int.Parse(matches[0].Value);
                string prefabName = $"{prefabPath}/model_monster_{id}.prefab";
                string prefabName0 = $"{prefabPath}/model_monster_temp.prefab";

                var orimonster = AssetDatabase.LoadMainAssetAtPath(prefabName0) as GameObject;

                var oldmeshRenderer = orimonster.GetComponentInChildren<MeshRenderer>();
                var oldmeshFilter = orimonster.GetComponentInChildren<MeshFilter>();
                var oldgpuEcsAnimatorBehaviour = orimonster.GetComponentInChildren<GpuEcsAnimatorBehaviour>();

                oldmeshRenderer.sharedMaterial = meshRenderer.sharedMaterial;
                oldmeshFilter.sharedMesh = meshFilter.sharedMesh;
                oldgpuEcsAnimatorBehaviour.animations = gpuEcsAnimatorBehaviour.animations;
                oldgpuEcsAnimatorBehaviour.totalNbrOfFrames = gpuEcsAnimatorBehaviour.totalNbrOfFrames;
                oldgpuEcsAnimatorBehaviour.transformUsageFlags = gpuEcsAnimatorBehaviour.transformUsageFlags;
                oldgpuEcsAnimatorBehaviour.animationEventOccurences = gpuEcsAnimatorBehaviour.animationEventOccurences;
                oldgpuEcsAnimatorBehaviour.attachmentAnchorData = gpuEcsAnimatorBehaviour.attachmentAnchorData;
                oldgpuEcsAnimatorBehaviour.nbrOfAttachmentAnchors = gpuEcsAnimatorBehaviour.nbrOfAttachmentAnchors;

                PrefabUtility.SaveAsPrefabAsset(orimonster, prefabName);
            }


            //Debug.Log($"f {matches[0].Value}");
            GameObject.DestroyImmediate(ins);
            GameObject.DestroyImmediate(newgo);
        }

        GameObject.DestroyImmediate(meshBakerGO);
        // 标记场景为已修改
        Scene activeScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(activeScene);

        // 保存场景
        EditorSceneManager.SaveScene(activeScene);


        AssetDatabase.Refresh();
        Object obj = AssetDatabase.LoadMainAssetAtPath(outputPath);
        EditorGUIUtility.PingObject(obj);
    }
}