using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GimmeDOTSGeometry.Samples.ECS
{
    public struct OctreeMovementSystemInfoComponent : IComponentData
    {
        public Bounds bounds;

        public float pointRadius;
        public float movingPercentage;
        public float searchRadius;

        public float4 insideColor;
        public float4 outsideColor;

        public Unity.Mathematics.Random rnd;

        public NativeList<float3> searchPositions;
        public NativeSparseOctree<int> sparseOctree;
        public NativeDenseOctree<int> denseOctree;

        public bool useSparseOctree;
        public bool doMultiQuery;

    }
}
