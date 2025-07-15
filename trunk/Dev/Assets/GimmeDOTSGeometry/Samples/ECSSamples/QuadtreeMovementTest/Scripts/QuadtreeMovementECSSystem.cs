using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

//These types have to be registered in order for us to be able to call and use them in a (non-generic) Entity System (or rather another Burst-compiled code)

[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeSparseQuadtree<Entity>.GetCellsInRadiusJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeSparseQuadtree<Entity>.GetCellsInRadiiJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeDenseQuadtree<Entity>.GetCellsInRadiusJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeDenseQuadtree<Entity>.GetCellsInRadiiJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeSparseQuadtree<int>.GetCellsInRadiusJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeSparseQuadtree<int>.GetCellsInRadiiJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeDenseQuadtree<int>.GetCellsInRadiusJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeDenseQuadtree<int>.GetCellsInRadiiJob))]

namespace GimmeDOTSGeometry.Samples.ECS
{
    public struct GameQuadtreeData : IComponentData
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
        public NativeSparseQuadtree<Entity> sparseQuadtree;
    }

    public partial class QuadtreeMovementECSSystem : MonoBehaviour
    {
        #region Public Fields

        public Color insideColor;
        public Color outsideColor;

        public float pointRadius = 0.025f;
        public float yOffset = 0.01f;
        public float searchRadius = 1.0f;
        public float initialMovingPercentage = 0.2f;

        public int initialNumberOfPoints = 1000;
        public int initialQuadtreeDepth = 3;

        public Material pointMaterial;
        public Material rectMaterial;
        public Material ringMaterial;

        public Rect bounds;

        public Vector2 velocityRange;

        #endregion

        #region Private Fields

        private bool useSparseQuadtree = true;
        private bool doMultiQuery = true;

        private EntityManager entityMgr;
        private Entity systemInfoEntity;

        private float movingPercentage = 0.0f;

        private GameObject rectangleGO;
        private GameObject ringGO;
        private GameObject[] multiQueryRings;

        private IQuadtree<Entity> quadtree;

        private int currentQuadtreeDepth = 0;
        private int positionCount = 0;

        private Mesh pointMesh = null;

        private RenderBounds localBounds;

        private RenderMeshDescription renderMeshDescription;
        private RenderMeshArray renderMeshArray;

        private NativeList<float2> searchPositions;
        private NativeReference<Unity.Mathematics.Random> rnd;

        #endregion

        public bool IsDoingMultiQueries() => this.doMultiQuery;
        public bool IsUsingSparseQuadtree() => this.useSparseQuadtree;

        public int GetNrOfPoints() => this.positionCount;

        public float CurrentMovingPercentage
        {
            get => this.movingPercentage;
            set
            {
                this.movingPercentage = value;
                this.UpdateSystemInfo();
            }
        }

        public int CurrentSearchDepth
        {
            get => this.currentQuadtreeDepth;
            set
            {
                this.currentQuadtreeDepth = value;
                this.quadtree.Dispose();
                this.RecreateQuadtree();
                this.UpdateSystemInfo();
            }
        }

        public float CurrentSearchPercentage
        {
            get => (this.searchRadius * 2.0f) / this.bounds.width;
            set
            {
                this.searchRadius = value * this.bounds.width * 0.5f;
                this.ScaleRings();
                this.UpdateSystemInfo();
            }
        }

        public void EnableMultiQuery(bool enable)
        {
            this.doMultiQuery = enable;

            var mr = this.ringGO.GetComponent<MeshRenderer>();
            mr.enabled = !this.doMultiQuery;

            for (int i = 0; i < this.multiQueryRings.Length; i++)
            {
                mr = this.multiQueryRings[i].GetComponent<MeshRenderer>();
                mr.enabled = this.doMultiQuery;
            }

            this.searchPositions.Clear();
            if (this.doMultiQuery)
            {
                float halfSize = this.bounds.width * 0.5f;
                float2 pos0 = new float2(-halfSize * 0.5f, -halfSize * 0.5f);
                float2 pos1 = new float2(-halfSize * 0.5f, halfSize * 0.5f);
                float2 pos2 = new float2(halfSize * 0.5f, -halfSize * 0.5f);
                float2 pos3 = new float2(halfSize * 0.5f, halfSize * 0.5f);

                this.searchPositions.Add(pos0);
                this.searchPositions.Add(pos1);
                this.searchPositions.Add(pos2);
                this.searchPositions.Add(pos3);
            }
            else
            {
                this.searchPositions.Add(float2.zero);
            }

            this.UpdateSystemInfo();
        }

        private void InitMultiQueries()
        {
            this.multiQueryRings = new GameObject[4];

            float halfSize = this.bounds.width * 0.5f;

            var ringMesh = MeshUtil.CreateRing(this.searchRadius * 0.5f, 0.2f);

            for (int i = 0; i < 4; i++)
            {
                this.multiQueryRings[i] = new GameObject($"Ring_{i}");
                this.multiQueryRings[i].transform.parent = this.transform;

                var ringMeshFilter = this.multiQueryRings[i].AddComponent<MeshFilter>();
                ringMeshFilter.mesh = ringMesh;

                var ringMeshRenderer = this.multiQueryRings[i].AddComponent<MeshRenderer>();
                ringMeshRenderer.material = this.ringMaterial;
                ringMeshRenderer.enabled = false;
            }

            this.multiQueryRings[0].transform.position =
                new Vector3(-halfSize * 0.5f, 2 * this.yOffset, -halfSize * 0.5f);
            this.multiQueryRings[1].transform.position =
                new Vector3(-halfSize * 0.5f, 2 * this.yOffset, halfSize * 0.5f);
            this.multiQueryRings[2].transform.position =
                new Vector3(halfSize * 0.5f, 2 * this.yOffset, -halfSize * 0.5f);
            this.multiQueryRings[3].transform.position =
                new Vector3(halfSize * 0.5f, 2 * this.yOffset, halfSize * 0.5f);
        }

        private void Start()
        {
            this.currentQuadtreeDepth = this.initialQuadtreeDepth;
            this.movingPercentage = this.initialMovingPercentage;

            var rectMesh = MeshUtil.CreateRectangleOutline(this.bounds, 0.1f);
            var ringMesh = MeshUtil.CreateRing(this.searchRadius, 0.2f);

            this.rectangleGO = new GameObject("Rectangle");
            this.rectangleGO.transform.parent = this.transform;
            this.rectangleGO.transform.position = new Vector3(0.0f, -this.yOffset, 0.0f);

            var rectMeshFilter = this.rectangleGO.AddComponent<MeshFilter>();
            rectMeshFilter.mesh = rectMesh;

            var rectMeshRenderer = this.rectangleGO.AddComponent<MeshRenderer>();
            rectMeshRenderer.material = this.rectMaterial;

            this.ringGO = new GameObject("Ring");
            this.ringGO.transform.parent = this.transform;
            this.ringGO.transform.position = new Vector3(0.0f, 2 * this.yOffset, 0.0f);

            var ringMeshFilter = this.ringGO.AddComponent<MeshFilter>();
            ringMeshFilter.mesh = ringMesh;

            var ringMeshRenderer = this.ringGO.AddComponent<MeshRenderer>();
            ringMeshRenderer.material = this.ringMaterial;

            this.InitMultiQueries();

            this.quadtree = new NativeSparseQuadtree<Entity>(float3.zero,
                new float2(this.bounds.width, this.bounds.height),
                this.currentQuadtreeDepth,
                this.initialNumberOfPoints, Allocator.Persistent);

            var world = World.DefaultGameObjectInjectionWorld;
            this.entityMgr = world.EntityManager;
            this.pointMesh = MeshUtil.CreateCircle(this.pointRadius, 8);

            var pointMeshBounds = this.pointMesh.bounds;

            this.localBounds = new RenderBounds();
            this.localBounds.Value = new AABB() { Center = pointMeshBounds.center, Extents = pointMeshBounds.extents };

            this.renderMeshDescription = new RenderMeshDescription(shadowCastingMode: ShadowCastingMode.Off);
            this.renderMeshArray =
                new RenderMeshArray(new Material[] { this.pointMaterial }, new Mesh[] { this.pointMesh });

            var newRnd = new Unity.Mathematics.Random();
            newRnd.InitState();
            this.rnd = new NativeReference<Unity.Mathematics.Random>(newRnd, Allocator.Persistent);

            this.searchPositions = new NativeList<float2>(Allocator.Persistent);
            this.searchPositions.Add(float2.zero);

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            this.systemInfoEntity = this.entityMgr.CreateEntity();

            this.entityMgr.AddComponentData(systemInfoEntity, new GameQuadtreeData());

            ecb.Playback(this.entityMgr);
            ecb.Dispose();

            this.AddPoints(this.initialNumberOfPoints);
        }

        private void ScaleRings()
        {
            if (this.doMultiQuery)
            {
                var newRingMesh = MeshUtil.CreateRing(this.searchRadius * 0.5f, 0.2f);

                for (int i = 0; i < this.multiQueryRings.Length; i++)
                {
                    var meshFilter = this.multiQueryRings[i].GetComponent<MeshFilter>();
                    meshFilter.sharedMesh = newRingMesh;
                }
            }
            else
            {
                var newRingMesh = MeshUtil.CreateRing(this.searchRadius, 0.2f);

                var meshFilter = this.ringGO.GetComponent<MeshFilter>();
                Destroy(meshFilter.sharedMesh);
                meshFilter.sharedMesh = newRingMesh;
            }
        }

        private void UpdateSystemInfo()
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            this.entityMgr.SetComponentData(this.systemInfoEntity,
                new GameQuadtreeData()
                {
                    bounds = this.bounds,
                    pointRadius = this.pointRadius,
                    yOffset = this.yOffset,
                    movingPercentage = this.movingPercentage,
                    rnd = this.rnd.Value,
                    sparseQuadtree = this.useSparseQuadtree ? (NativeSparseQuadtree<Entity>)this.quadtree : default,
                    searchRadius = this.searchRadius,
                    insideColor = this.insideColor.ToFloat4(),
                    outsideColor = this.outsideColor.ToFloat4(),
                    searchPositions = this.searchPositions,
                });

            ecb.Playback(this.entityMgr);
            ecb.Dispose();
        }

        public void ExchangeQuadtreeModel()
        {
            this.quadtree.Dispose();
            this.useSparseQuadtree = !this.useSparseQuadtree;

            this.RecreateQuadtree();
            this.UpdateSystemInfo();
        }

        private void RecreateQuadtree()
        {
            //We need to allocate more memory to be safe inorder to use insert in a job (there are problems otherwise)

            this.quadtree = new NativeSparseQuadtree<Entity>(float3.zero,
                new float2(this.bounds.width, this.bounds.height),
                this.currentQuadtreeDepth, this.positionCount, Allocator.Persistent);
            //quadtree.

            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.TempJob);
            EntityQuery transformQuery = builder
                .WithAll<LocalToWorld>()
                .WithAll<IndexComponent>()
                .Build(this.entityMgr);
            var trs = transformQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
            var indices = transformQuery.ToComponentDataArray<IndexComponent>(Allocator.TempJob);
            var entities = transformQuery.ToEntityArray(Allocator.TempJob);


            var insertJob = new InsertEntityPositionsIntoSparseQuadtree()
            {
                quadtree = (NativeSparseQuadtree<Entity>)this.quadtree,
                indices = indices,
                trs = trs,
                entities = entities
            };

            insertJob.Schedule().Complete();


            builder.Dispose();
            transformQuery.Dispose();
            trs.Dispose();
            indices.Dispose();
        }

        public void AddPoints(int nrOfPoints)
        {
            var prototype = this.entityMgr.CreateEntity();

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            RenderMeshUtility.AddComponents(prototype, this.entityMgr, this.renderMeshDescription, this.renderMeshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

            this.entityMgr.AddComponentData(prototype, new LocalToWorld());
            this.entityMgr.AddComponentData(prototype, new IndexComponent());
            this.entityMgr.AddComponentData(prototype, new Velocity2DComponent());
            this.entityMgr.AddComponentData(prototype, new IsMovingComponent());
            this.entityMgr.AddComponentData(prototype, new ColorComponent());

            var addPointsJob = new AddPointsJob()
            {
                ecb = ecb,
                nrOfPoints = nrOfPoints,
                prototype = prototype,
                bounds = this.bounds,
                localBounds = this.localBounds,
                radius = this.pointRadius,
                rnd = this.rnd,
                yOffset = this.yOffset,
                offset = this.positionCount,
                velocityRange = this.velocityRange,
            };

            addPointsJob.Schedule().Complete();


            ecb.Playback(this.entityMgr);
            ecb.Dispose();

            this.entityMgr.DestroyEntity(prototype);

            this.positionCount += nrOfPoints;

            this.quadtree.Dispose();
            this.RecreateQuadtree();


            this.UpdateSystemInfo();
        }

        [BurstCompile]
        public struct InsertEntityPositionsIntoSparseQuadtree : IJob
        {
            [NoAlias] public NativeSparseQuadtree<Entity> quadtree;

            [NoAlias, ReadOnly] public NativeArray<LocalToWorld> trs;

            [NoAlias, ReadOnly] public NativeArray<IndexComponent> indices;
            [NoAlias, ReadOnly] public NativeArray<Entity> entities;

            public void Execute()
            {
                for (int i = 0; i < this.trs.Length; i++)
                {
                    var pos = this.trs[i].Position;
                    //var idx = this.indices[i];
                    this.quadtree.Insert(pos, entities[i]);
                }
            }
        }


        [BurstCompile]
        public struct AddPointsJob : IJob
        {
            public Entity prototype;
            public EntityCommandBuffer ecb;

            public float radius;
            public float yOffset;
            public float2 velocityRange;

            public int nrOfPoints;
            public int offset;

            public RenderBounds localBounds;

            public Rect bounds;

            public NativeReference<Unity.Mathematics.Random> rnd;

            public void Execute()
            {
                var rnd = this.rnd.Value;
                for (int i = 0; i < this.nrOfPoints; i++)
                {
                    var e = this.ecb.Instantiate(this.prototype);
                    this.ecb.SetComponent(e, new LocalToWorld { Value = ComputeTransform(ref rnd) });
                    this.ecb.SetComponent(e, this.localBounds);
                    this.ecb.SetComponent(e, new IndexComponent() { idx = this.offset + i });

                    float2 velocity = rnd.NextFloat2Direction() *
                                      rnd.NextFloat(this.velocityRange.x, this.velocityRange.y);
                    this.ecb.SetComponent(e, new Velocity2DComponent() { velocity = velocity });
                }

                this.rnd.Value = rnd;
            }

            public float4x4 ComputeTransform(ref Unity.Mathematics.Random rnd)
            {
                float rndX = rnd.NextFloat(this.bounds.xMin + this.radius, this.bounds.xMax - this.radius);
                float rndY = rnd.NextFloat(this.bounds.yMin + this.radius, this.bounds.yMax - this.radius);

                var worldPos = new float3(rndX, this.yOffset, rndY);

                return float4x4.Translate(worldPos);
            }
        }

        [BurstCompile]
        private struct CollectMultiQueryIndicesJob : IJob
        {
            [NoAlias, ReadOnly] public NativeParallelMultiHashMap<uint, Entity> data;

            [NoAlias, ReadOnly] public NativeParallelHashSet<uint> cells;

            [NoAlias, WriteOnly] public NativeParallelHashSet<Entity> result;

            public void Execute()
            {
                foreach (var cell in this.cells)
                {
                    if (this.data.TryGetFirstValue(cell, out var idx, out var it))
                    {
                        this.result.Add(idx);

                        while (this.data.TryGetNextValue(out idx, ref it))
                        {
                            this.result.Add(idx);
                        }
                    }
                }
            }
        }

        [BurstCompile]
        private struct CollectIndicesJob : IJob
        {
            [NoAlias, ReadOnly] public NativeParallelMultiHashMap<uint, int> data;

            [NoAlias, ReadOnly] public NativeList<uint> cells;

            [NoAlias, WriteOnly] public NativeParallelHashSet<int> result;

            public void Execute()
            {
                foreach (var cell in this.cells)
                {
                    if (this.data.TryGetFirstValue(cell, out int idx, out var it))
                    {
                        this.result.Add(idx);

                        while (this.data.TryGetNextValue(out idx, ref it))
                        {
                            this.result.Add(idx);
                        }
                    }
                }
            }
        }

        [BurstCompile]
        public partial struct UpdateSparseQuadtreeJob : IJobEntity
        {
            [NoAlias, ReadOnly, DeallocateOnJobCompletion]
            public NativeArray<float3> oldPositions;

            [NoAlias] public NativeSparseQuadtree<Entity> quadtree;

            public void Execute(Entity entity, LocalToWorld trs, IndexComponent index, IsMovingComponent isMoving)
            {
                if (isMoving.isMoving)
                {
                    this.quadtree.Update(entity, this.oldPositions[index.idx], trs.Position);
                }
            }
        }

        [BurstCompile]
        public partial struct UpdateDenseQuadtreeJob : IJobEntity
        {
            [NoAlias, ReadOnly, DeallocateOnJobCompletion]
            public NativeArray<float3> oldPositions;

            [NoAlias] public NativeDenseQuadtree<int> quadtree;

            public void Execute(LocalToWorld trs, IndexComponent index, IsMovingComponent isMoving)
            {
                if (isMoving.isMoving)
                {
                    this.quadtree.Update(index.idx, this.oldPositions[index.idx], trs.Position);
                }
            }
        }

        [BurstCompile]
        public partial struct UpdatePointJob : IJobEntity
        {
            public float deltaTime;
            public float movingPercentage;
            public float pointRadius;
            public float yOffset;


            public Unity.Mathematics.Random rnd;

            [NoAlias, WriteOnly, NativeDisableParallelForRestriction]
            public NativeArray<float3> oldPositions;

            public Rect bounds;

            public void Execute(IndexComponent index, ref LocalToWorld trs, ref Velocity2DComponent velocity,
                ref IsMovingComponent isMoving)
            {
                var pos = trs.Position;
                var vel = velocity.velocity;

                this.oldPositions[index.idx] = pos;

                if (this.rnd.NextFloat() < this.movingPercentage)
                {
                    float nextPosX = math.mad(vel.x, this.deltaTime, pos.x);
                    float nextPosY = math.mad(vel.y, this.deltaTime, pos.z);

                    float xMax = this.bounds.xMax - this.pointRadius;
                    float xMin = this.bounds.xMin + this.pointRadius;
                    float yMax = this.bounds.yMax - this.pointRadius;
                    float yMin = this.bounds.yMin + this.pointRadius;

                    if (Hint.Unlikely(nextPosX > xMax))
                    {
                        vel = math.reflect(vel, Float2Extension.Left);
                        nextPosX -= (nextPosX - xMax) * 2.0f;
                    }
                    else if (Hint.Unlikely(nextPosX < xMin))
                    {
                        vel = math.reflect(vel, Float2Extension.Right);
                        nextPosX += (xMin - nextPosX) * 2.0f;
                    }

                    if (Hint.Unlikely(nextPosY > yMax))
                    {
                        vel = math.reflect(vel, Float2Extension.Down);
                        nextPosY -= (nextPosY - yMax) * 2.0f;
                    }
                    else if (Hint.Unlikely(nextPosY < yMin))
                    {
                        vel = math.reflect(vel, Float2Extension.Up);
                        nextPosY += (yMin - nextPosY) * 2.0f;
                    }

                    velocity.velocity = vel;
                    trs.Value.c3 = new float4(nextPosX, this.yOffset, nextPosY, 1.0f);
                    isMoving.isMoving = true;
                }
                else
                {
                    isMoving.isMoving = false;
                }
            }
        }

        [BurstCompile]
        public partial struct ColorPointsJob : IJobEntity
        {
            public float4 insideColor;
            public float4 outsideColor;

            public float searchRadius;

            [NoAlias, ReadOnly] public NativeParallelHashSet<Entity> indices;

            [NoAlias, ReadOnly] public NativeList<float2> searchPositions;

            public void Execute(Entity entity, IndexComponent index, LocalToWorld trs, ref ColorComponent color)
            {
                bool contained = false;
                if (this.indices.Contains(entity))
                {
                    
                    var pos = trs.Position;
                    for (int i = 0; i < this.searchPositions.Length; i++)
                    {
                        
                        // if (math.distancesq(pos.xz, this.searchPositions[i]) < this.searchRadius * this.searchRadius)
                        // {
                            color.value = this.insideColor;
                            contained = true;
                            break;
                        //}
                    }
                }

                if (!contained)
                {
                    color.value = this.outsideColor;
                }
            }
        }

        [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
        public partial struct QuadtreeUpdateSystem : ISystem
        {
            private EntityQuery pointsQuery;

            [BurstCompile]
            public void OnCreate(ref SystemState state)
            {
                pointsQuery = SystemAPI.QueryBuilder().WithAll<LocalToWorld>()
                    .WithAll<Velocity2DComponent>()
                    .WithAll<IndexComponent>()
                    .WithAll<IsMovingComponent>()
                    .WithAll<ColorComponent>()
                    .Build();
            }

            [BurstCompile]
            public void OnUpdate(ref SystemState state)
            {
                bool foundSystemInfo = false;
                GameQuadtreeData systemInfo = default;
                foreach (var sysInfo in SystemAPI.Query<GameQuadtreeData>())
                {
                    systemInfo = sysInfo;
                    foundSystemInfo = true;
                    break;
                }

                if (foundSystemInfo)
                {
                    float deltaTime = SystemAPI.Time.DeltaTime;
                    int entities = this.pointsQuery.CalculateEntityCount();
                    var oldPositions = new NativeArray<float3>(entities, Allocator.TempJob);

                    //RecreateQuadtree()

                    var updateJob = new UpdatePointJob()
                    {
                        deltaTime = deltaTime,
                        bounds = systemInfo.bounds,
                        movingPercentage = systemInfo.movingPercentage,
                        oldPositions = oldPositions,
                        pointRadius = systemInfo.pointRadius,
                        rnd = systemInfo.rnd,
                        yOffset = systemInfo.yOffset,
                    };
                    state.Dependency = updateJob.ScheduleParallel(this.pointsQuery, state.Dependency);

                    NativeList<uint> queryResults = new NativeList<uint>(Allocator.TempJob);
                    NativeParallelHashSet<uint> multiQueryResults =
                        new NativeParallelHashSet<uint>(entities, Allocator.TempJob);
                    NativeParallelHashSet<Entity> allIndices = new NativeParallelHashSet<Entity>(8, Allocator.TempJob);
                    NativeArray<float> radii =
                        new NativeArray<float>(systemInfo.searchPositions.Length, Allocator.TempJob);

                    //Because we cannot schedule from managed code in an ISystem (as opposed to a SystemBase),
                    //this is a little bit elaborate in the if-else structures because we do not use an interface.
                    //As it is only meant to test which branch is optimal for your use-cases it is fine though for this
                    //sample scene

                    // if (systemInfo.useSparseQuadtree)
                    // {
                    var updateQuadtreeJob = new UpdateSparseQuadtreeJob()
                    {
                        oldPositions = oldPositions,
                        quadtree = systemInfo.sparseQuadtree,
                    };
                    state.Dependency = updateQuadtreeJob.Schedule(this.pointsQuery, state.Dependency);

                    // if (systemInfo.doMultiQuery)
                    // {
                    for (int i = 0; i < systemInfo.searchPositions.Length; i++)
                        radii[i] = systemInfo.searchRadius * 0.5f;

                    state.Dependency = systemInfo.sparseQuadtree.GetCellsInRadii(
                        systemInfo.searchPositions.AsArray(), radii,
                        ref multiQueryResults, state.Dependency);

                    var collectIndicesJob = new CollectMultiQueryIndicesJob()
                    {
                        cells = multiQueryResults,
                        data = systemInfo.sparseQuadtree.GetDataBuckets(),
                        result = allIndices,
                    };

                    state.Dependency = collectIndicesJob.Schedule(state.Dependency);
                    // }
                    // else
                    // {
                    //     state.Dependency = systemInfo.sparseQuadtree.GetCellsInRadius(float2.zero,
                    //         systemInfo.searchRadius,
                    //         ref queryResults, state.Dependency);
                    //
                    //     var collectIndicesJob = new CollectIndicesJob()
                    //     {
                    //         cells = queryResults,
                    //         data = systemInfo.sparseQuadtree.GetDataBuckets(),
                    //         result = allIndices,
                    //     };
                    //
                    //     state.Dependency = collectIndicesJob.Schedule(state.Dependency);
                    // }
                    //}
                    // else
                    // {
                    //     var updateQuadtreeJob = new UpdateDenseQuadtreeJob()
                    //     {
                    //         oldPositions = oldPositions,
                    //         quadtree = systemInfo.denseQuadtree,
                    //     };
                    //     state.Dependency = updateQuadtreeJob.Schedule(this.pointsQuery, state.Dependency);
                    //
                    //     if (systemInfo.doMultiQuery)
                    //     {
                    //         for (int i = 0; i < systemInfo.searchPositions.Length; i++)
                    //             radii[i] = systemInfo.searchRadius * 0.5f;
                    //
                    //         state.Dependency = systemInfo.denseQuadtree.GetCellsInRadii(
                    //             systemInfo.searchPositions.AsArray(), radii,
                    //             ref multiQueryResults, state.Dependency);
                    //
                    //         var collectIndicesJob = new CollectMultiQueryIndicesJob()
                    //         {
                    //             cells = multiQueryResults,
                    //             data = systemInfo.denseQuadtree.GetDataBuckets(),
                    //             result = allIndices,
                    //         };
                    //
                    //         state.Dependency = collectIndicesJob.Schedule(state.Dependency);
                    //     }
                    //     else
                    //     {
                    //         state.Dependency = systemInfo.denseQuadtree.GetCellsInRadius(float2.zero,
                    //             systemInfo.searchRadius,
                    //             ref queryResults, state.Dependency);
                    //
                    //         var collectIndicesJob = new CollectIndicesJob()
                    //         {
                    //             cells = queryResults,
                    //             data = systemInfo.denseQuadtree.GetDataBuckets(),
                    //             result = allIndices,
                    //         };
                    //
                    //
                    //         state.Dependency = collectIndicesJob.Schedule(state.Dependency);
                    //     }
                    // }

                    state.Dependency = queryResults.Dispose(state.Dependency);
                    state.Dependency = multiQueryResults.Dispose(state.Dependency);
                    state.Dependency = radii.Dispose(state.Dependency);

                    var updateColorsJob = new ColorPointsJob()
                    {
                        indices = allIndices,
                        insideColor = systemInfo.insideColor,
                        outsideColor = systemInfo.outsideColor,
                        searchPositions = systemInfo.searchPositions,
                        searchRadius = systemInfo.searchRadius * 0.5f
                    };
                    state.Dependency = updateColorsJob.ScheduleParallel(this.pointsQuery, state.Dependency);
                    state.Dependency = allIndices.Dispose(state.Dependency);
                }
            }

            public void OnDestroy(ref SystemState state)
            {
            }
        }


        private void Dispose()
        {
            this.rnd.DisposeIfCreated();
            this.searchPositions.DisposeIfCreated();
            if (this.quadtree.IsCreated)
            {
                this.quadtree.Dispose();
            }
        }

        private void OnDestroy()
        {
            this.Dispose();
        }
    }
}