using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class JiYuPivotAuthoring : MonoBehaviour
{
    public float PivotY = 0.5f;

    class JiYuPivotAuthoringBaker : Baker<JiYuPivotAuthoring>
    {
        public override void Bake(JiYuPivotAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            //float pivotY = 0.5f;
            float pivotY = authoring.PivotY;
            //var renderer = ;
            // if (authoring.TryGetComponent<MeshRenderer>(out var renderer) && math.abs(pivotY - 0.5f) < math.EPSILON)
            // {
            //     Texture texture = renderer.sharedMaterial?.mainTexture;
            //
            //
            //     string path = AssetDatabase.GetAssetPath(texture); // 需要引入UnityEditor命名空间
            //     TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            //
            //
            //     if (importer != null && importer.spritePackingTag != null)
            //     {
            //         // 获取Sprite的Pivot值
            //         pivotY = importer.spritePivot.y;
            //         Debug.Log("Sprite PivotY: " + pivotY);
            //     }
            //     else
            //     {
            //         //Debug.Log("无法获取Sprite的Pivot值");
            //     }
            // }


            AddComponent(entity, new JiYuPivot
            {
                value = pivotY
            });
        }
    }
}

[Serializable]
[MaterialProperty("_JiYuPivot")]
public struct JiYuPivot : IComponentData
{
    /// <summary>
    /// y轴的排序轴点
    /// </summary>
    public float value;
}