using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GimmeDOTSGeometry.Samples.ECS
{
    public struct QuadtreeMovementSystemInfoComponent : IComponentData
    {
        public float pointRadius;
        public float yOffset;
        public float movingPercentage;
        public float searchRadius;

        public float4 insideColor;
        public float4 outsideColor;

        public Rect bounds;

        public Unity.Mathematics.Random rnd;

        public NativeList<float2> searchPositions;
        public NativeSparseQuadtree<int> sparseQuadtree;
        public NativeDenseQuadtree<int> denseQuadtree;

        public bool useSparseQuadtree;
        public bool doMultiQuery;
    }
}
