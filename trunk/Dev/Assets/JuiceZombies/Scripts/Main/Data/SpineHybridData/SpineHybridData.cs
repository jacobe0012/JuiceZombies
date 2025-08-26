//---------------------------------------------------------------------
// UnicornStudio
// Author: huangjinguo
// Time: 2024-02-18 17:51:53
//---------------------------------------------------------------------


using System;
using Spine.Unity;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Main
{
    [Serializable]
    public struct SpineHybridData : IComponentData
    {
        public UnityObjectRef<GameObject> go;
        public UnityObjectRef<SkeletonAnimation> skeletonAnimation;
    }

    [Serializable]
    public struct TransformHybridUpdateData : IComponentData
    {
        public UnityObjectRef<GameObject> go;
        //public UnityObjectRef<Animation> animator;
    }
}