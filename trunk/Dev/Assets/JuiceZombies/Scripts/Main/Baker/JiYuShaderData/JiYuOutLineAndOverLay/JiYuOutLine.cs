//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2024-03-12 14:27:55
//---------------------------------------------------------------------

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Main
{
    [MaterialProperty("_OverlayTexEnable")]
    public struct JiYuOverlayTexEnable : IComponentData
    {
        public int value;
    }
    [MaterialProperty("_OverlayIndex")]
    public struct JiYuOverlayIndex : IComponentData
    {
        public int value;
    }
    // [MaterialProperty("_OverlayTexEnable")]
    // public struct JiYuOverlayTexEnable : IComponentData
    // {
    //     public int value;
    // }
}