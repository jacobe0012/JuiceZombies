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

[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeSparseOctree<int>.GetCellsInRadiusJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeSparseOctree<int>.GetCellsInRadiiJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeDenseOctree<int>.GetCellsInRadiusJob))]
[assembly: RegisterGenericJobType(typeof(GimmeDOTSGeometry.NativeDenseOctree<int>.GetCellsInRadiiJob))]

namespace GimmeDOTSGeometry.Samples.ECS
{
    public partial class OctreeMovementECSSystem : MonoBehaviour
    {

        #region Public Fields

        public Bounds bounds;

        public Color insideColor;
        public Color outsideColor;

        public float pointRadius = 0.025f;
        public float searchRadius = 1.0f;
        public float initialMovingPercentage = 0.2f;

        public int initialNumberOfPoints = 1000;
        public int initialOctreeDepth = 3;

        public Material pointMaterial;
        public Material boxMaterial;
        public Material ringMaterial;

        public Vector2 velocityRange;

        #endregion

        #region Private Fields

        private bool useSparseOctree = true;
        private bool doMultiQuery = false;

        private EntityManager entityMgr;
        private Entity systemInfoEntity;

        private float movingPercentage = 0.0f;

        private GameObject boxGO;
        private GameObject ringGO;
        private GameObject[] multiQueryRings;

        private int currentOctreeDepth = 0;
        private int positionCount = 0;

        private IOctree<int> octree;

        private Mesh pointMesh = null;

        private RenderBounds localBounds;

        private RenderMeshDescription renderMeshDescription;
        private RenderMeshArray renderMeshArray;

        private NativeList<float3> searchPositions;
        private NativeReference<Unity.Mathematics.Random> rnd;

        #endregion

        public bool IsDoingMultiQueries() => this.doMultiQuery;
        public bool IsUsingSparseOctree() => this.useSparseOctree;

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
            get => this.currentOctreeDepth;
            set
            {
                this.currentOctreeDepth = value;
                this.octree.Dispose();
                this.RecreateOctree();
                this.UpdateSystemInfo();
            }
        }

        public float CurrentSearchPercentage
        {
            get => (this.searchRadius * 2.0f) / this.bounds.size.x;
            set
            {
                this.searchRadius = value * this.bounds.size.x * 0.5f;
                this.ScaleRings();
                this.UpdateSystemInfo();
            }
        }

        private void ScaleRings()
        {
            if(this.doMultiQuery)
            {
                var newRingMesh = MeshUtil.CreateRing(this.searchRadius * 0.5f, 0.2f);

                for(int i = 0; i < this.multiQueryRings.Length; i++)
                {
                    var meshFilter = this.multiQueryRings[i].GetComponent<MeshFilter>();
                    meshFilter.sharedMesh = newRingMesh;
                }
            } else
            {
                var newRingMesh = MeshUtil.CreateRing(this.searchRadius, 0.2f);

                var meshFilter = this.ringGO.GetComponent<MeshFilter>();
                meshFilter.sharedMesh = newRingMesh;
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
            this.searchPositions.Clear();
            if (this.doMultiQuery)
            {
                float halfSize = this.bounds.size.x * 0.5f;
                float3 pos0 = new float3(-halfSize * 0.5f, halfSize * 0.5f, -halfSize * 0.5f);
                float3 pos1 = new float3(-halfSize * 0.5f, halfSize * 0.5f, halfSize * 0.5f);
                float3 pos2 = new float3(halfSize * 0.5f, halfSize * 0.5f, -halfSize * 0.5f);
                float3 pos3 = new float3(halfSize * 0.5f, halfSize * 0.5f, halfSize * 0.5f);
                float3 pos4 = new float3(-halfSize * 0.5f, -halfSize * 0.5f, -halfSize * 0.5f);
                float3 pos5 = new float3(-halfSize * 0.5f, -halfSize * 0.5f, halfSize * 0.5f);
                float3 pos6 = new float3(halfSize * 0.5f, -halfSize * 0.5f, -halfSize * 0.5f);
                float3 pos7 = new float3(halfSize * 0.5f, -halfSize * 0.5f, halfSize * 0.5f);

                this.searchPositions.Add(pos0);
                this.searchPositions.Add(pos1);
                this.searchPositions.Add(pos2);
                this.searchPositions.Add(pos3);
                this.searchPositions.Add(pos4);
                this.searchPositions.Add(pos5);
                this.searchPositions.Add(pos6);
                this.searchPositions.Add(pos7);
            }
            else
            {
                this.searchPositions.Add(float3.zero);
            }

            this.UpdateSystemInfo();
        }

        [BurstCompile]
        public struct InsertEntityPositionsIntoSparseOctree : IJob
        {
            [NoAlias]
            public NativeSparseOctree<int> octree;

            [NoAlias, ReadOnly]
            public NativeArray<LocalToWorld> trs;

            [NoAlias, ReadOnly]
            public NativeArray<IndexComponent> indices;

            public void Execute()
            {
                for (int i = 0; i < this.trs.Length; i++)
                {
                    var pos = this.trs[i].Position;
                    var idx = this.indices[i];
                    this.octree.Insert(pos, idx.idx);
                }
            }
        }

        [BurstCompile]
        public struct InsertEntityPositionsIntoDenseOctree : IJob
        {
            [NoAlias]
            public NativeDenseOctree<int> octree;

            [NoAlias, ReadOnly]
            public NativeArray<LocalToWorld> trs;

            [NoAlias, ReadOnly]
            public NativeArray<IndexComponent> indices;

            public void Execute()
            {
                for (int i = 0; i < this.trs.Length; i++)
                {
                    var pos = this.trs[i].Position;
                    var idx = this.indices[i];
                    this.octree.Insert(pos, idx.idx);
                }
            }
        }

        private void RecreateOctree()
        {
            if(this.useSparseOctree)
            {

                this.octree = new NativeSparseOctree<int>(float3.zero, this.bounds.size, this.currentOctreeDepth,
                    this.positionCount * 2, Allocator.Persistent);

            } else
            {
                this.octree = new NativeDenseOctree<int>(float3.zero, this.bounds.size, this.currentOctreeDepth,
                    this.positionCount * 2, Allocator.Persistent);
            }

            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.TempJob);
            EntityQuery transformQuery = builder
                .WithAll<LocalToWorld>()
                .WithAll<IndexComponent>()
                .Build(this.entityMgr);
            var trs = transformQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
            var indices = transformQuery.ToComponentDataArray<IndexComponent>(Allocator.TempJob);

            if(this.useSparseOctree)
            {
                var insertJob = new InsertEntityPositionsIntoSparseOctree()
                {
                    indices = indices,
                    octree = (NativeSparseOctree<int>)this.octree,
                    trs = trs,
                };

                insertJob.Schedule().Complete();

            } else
            {
                var insertJob = new InsertEntityPositionsIntoDenseOctree()
                {
                    indices = indices,
                    octree = (NativeDenseOctree<int>)this.octree,
                    trs = trs,
                };

                insertJob.Schedule().Complete();
            }

            builder.Dispose();
            transformQuery.Dispose();
            trs.Dispose();
            indices.Dispose();
        }

        private void UpdateSystemInfo()
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            this.entityMgr.SetComponentData(this.systemInfoEntity,
                new OctreeMovementSystemInfoComponent()
                {
                    bounds = this.bounds,
                    pointRadius = this.pointRadius,
                    movingPercentage = this.movingPercentage,
                    rnd = this.rnd.Value,
                    denseOctree = this.useSparseOctree ? default : (NativeDenseOctree<int>)this.octree,
                    sparseOctree = this.useSparseOctree ? (NativeSparseOctree<int>)this.octree : default,
                    useSparseOctree = this.useSparseOctree,
                    searchRadius = this.searchRadius,
                    doMultiQuery = this.doMultiQuery,
                    insideColor = this.insideColor.ToFloat4(),
                    outsideColor = this.outsideColor.ToFloat4(),
                    searchPositions = this.searchPositions,
                });

            ecb.Playback(this.entityMgr);
            ecb.Dispose();
        }

        private void InitMultiQueries()
        {
            this.multiQueryRings = new GameObject[8];

            float halfSize = this.bounds.size.x * 0.5f;

            var ringMesh = MeshUtil.CreateRing(this.searchRadius * 0.4f, 0.2f);

            for(int i = 0; i < 8; i++)
            {
                this.multiQueryRings[i] = new GameObject($"Ring_{i}");
                this.multiQueryRings[i].transform.parent = this.transform;

                var ringMeshFilter = this.multiQueryRings[i].AddComponent<MeshFilter>();
                ringMeshFilter.mesh = ringMesh;

                var ringMeshRenderer = this.multiQueryRings[i].AddComponent<MeshRenderer>();
                ringMeshRenderer.material = this.ringMaterial;
                ringMeshRenderer.enabled = false;
            }

            this.multiQueryRings[0].transform.position = new Vector3(-halfSize * 0.5f, -halfSize * 0.5f, -halfSize * 0.5f);
            this.multiQueryRings[1].transform.position = new Vector3(-halfSize * 0.5f, -halfSize * 0.5f, halfSize * 0.5f);
            this.multiQueryRings[2].transform.position = new Vector3(halfSize * 0.5f, -halfSize * 0.5f, -halfSize * 0.5f);
            this.multiQueryRings[3].transform.position = new Vector3(halfSize * 0.5f, -halfSize * 0.5f, halfSize * 0.5f);
            this.multiQueryRings[4].transform.position = new Vector3(-halfSize * 0.5f, halfSize * 0.5f, -halfSize * 0.5f);
            this.multiQueryRings[5].transform.position = new Vector3(-halfSize * 0.5f, halfSize * 0.5f, halfSize * 0.5f);
            this.multiQueryRings[6].transform.position = new Vector3(halfSize * 0.5f, halfSize * 0.5f, -halfSize * 0.5f);
            this.multiQueryRings[7].transform.position = new Vector3(halfSize * 0.5f, halfSize * 0.5f, halfSize * 0.5f);
        }

        private void Start()
        {
            this.currentOctreeDepth = this.initialOctreeDepth;
            this.movingPercentage = this.initialMovingPercentage;

            var boxMesh = MeshUtil.CreateBoxOutline(this.bounds, 0.1f);
            var ringMesh = MeshUtil.CreateRing(this.searchRadius, 0.2f);

            this.boxGO = new GameObject("Box");
            this.boxGO.transform.parent = this.transform;
            this.boxGO.transform.position = Vector3.zero;

            var boxMeshFilter = this.boxGO.AddComponent<MeshFilter>();
            boxMeshFilter.mesh = boxMesh;

            var boxMeshRenderer = this.boxGO.AddComponent<MeshRenderer>();
            boxMeshRenderer.material = this.boxMaterial;

            this.ringGO = new GameObject("Ring");
            this.ringGO.transform.parent = this.transform;
            this.ringGO.transform.position = Vector3.zero;

            var ringMeshFilter = this.ringGO.AddComponent<MeshFilter>();
            ringMeshFilter.mesh = ringMesh;

            var ringMeshRenderer = this.ringGO.AddComponent<MeshRenderer>();
            ringMeshRenderer.material = this.ringMaterial;

            this.InitMultiQueries();

            this.octree = new NativeSparseOctree<int>(float3.zero,
                this.bounds.size,
                this.currentOctreeDepth,
                this.initialNumberOfPoints, Allocator.Persistent);

            var world = World.DefaultGameObjectInjectionWorld;
            this.entityMgr = world.EntityManager;
            this.pointMesh = MeshUtil.CreateUVSphere(this.pointRadius, 24, 24);

            var pointMeshBounds = this.pointMesh.bounds;

            this.localBounds = new RenderBounds();
            this.localBounds.Value = new AABB() { Center = pointMeshBounds.center, Extents = pointMeshBounds.extents };

            this.renderMeshDescription = new RenderMeshDescription(shadowCastingMode: ShadowCastingMode.Off);
            this.renderMeshArray = new RenderMeshArray(new Material[] { this.pointMaterial }, new Mesh[] { this.pointMesh });

            var newRnd = new Unity.Mathematics.Random();
            newRnd.InitState();
            this.rnd = new NativeReference<Unity.Mathematics.Random>(newRnd, Allocator.Persistent);

            this.searchPositions = new NativeList<float3>(Allocator.Persistent);
            this.searchPositions.Add(float3.zero);

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            this.systemInfoEntity = this.entityMgr.CreateEntity();

            this.entityMgr.AddComponentData(this.systemInfoEntity, new OctreeMovementSystemInfoComponent());

            ecb.Playback(this.entityMgr);
            ecb.Dispose();

            this.AddPoints(this.initialNumberOfPoints);
        }

        [BurstCompile]
        public struct AddPointsJob : IJob
        {
            public Entity prototype;
            public EntityCommandBuffer ecb;

            public float radius;
            public float2 velocityRange;

            public int nrOfPoints;
            public int offset;

            public RenderBounds localBounds;

            public Bounds bounds;

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

                    float3 velocity = rnd.NextFloat3Direction() * rnd.NextFloat(this.velocityRange.x, this.velocityRange.y);
                    this.ecb.SetComponent(e, new Velocity3DComponent() { velocity = velocity });
                }
                this.rnd.Value = rnd;
            }

            public float4x4 ComputeTransform(ref Unity.Mathematics.Random rnd)
            {
                var min = (float3)this.bounds.min + this.radius;
                var max = (float3)this.bounds.max - this.radius;

                float rndX = rnd.NextFloat(min.x, max.x);
                float rndY = rnd.NextFloat(min.y, max.y);
                float rndZ = rnd.NextFloat(min.z, max.z);

                var worldPos = new float3(rndX, rndY, rndZ);

                return float4x4.Translate(worldPos);
            }
        }

        public void ExchangeOctreeModel()
        {
            this.octree.Dispose();
            this.useSparseOctree = !this.useSparseOctree;

            this.RecreateOctree();
            this.UpdateSystemInfo();
        }

        public void AddPoints(int nrOfPoints)
        {
            var prototype = this.entityMgr.CreateEntity();

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            RenderMeshUtility.AddComponents(prototype, this.entityMgr, this.renderMeshDescription, this.renderMeshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

            this.entityMgr.AddComponentData(prototype, new LocalToWorld());
            this.entityMgr.AddComponentData(prototype, new IndexComponent());
            this.entityMgr.AddComponentData(prototype, new Velocity3DComponent());
            this.entityMgr.AddComponentData(prototype, new IsMovingComponent());
            this.entityMgr.AddComponentData(prototype, new ColorComponent());

            var addPointsJob = new AddPointsJob()
            {
                ecb = ecb,
                nrOfPoints = nrOfPoints,
                bounds = this.bounds,
                localBounds = this.localBounds,
                offset = this.positionCount,
                prototype = prototype,
                radius = this.pointRadius,
                rnd = this.rnd,
                velocityRange = this.velocityRange,
            };

            addPointsJob.Schedule().Complete();

            ecb.Playback(this.entityMgr);
            ecb.Dispose();

            this.entityMgr.DestroyEntity(prototype);

            this.positionCount += nrOfPoints;

            this.octree.Dispose();
            this.RecreateOctree();

            this.UpdateSystemInfo();
        }

        [BurstCompile]
        public partial struct UpdatePointJob : IJobEntity
        {

            public float deltaTime;
            public float movingPercentage;
            public float pointRadius;


            public Unity.Mathematics.Random rnd;

            [NoAlias, WriteOnly, NativeDisableParallelForRestriction]
            public NativeArray<float3> oldPositions;

            public Bounds bounds;

            public void Execute(IndexComponent index, ref LocalToWorld trs, ref Velocity3DComponent velocity, ref IsMovingComponent isMoving)
            {
                var pos = trs.Position;
                var vel = velocity.velocity;

                this.oldPositions[index.idx] = pos;

                if (this.rnd.NextFloat() < this.movingPercentage)
                {
                    float3 nextPos = math.mad(vel, this.deltaTime, pos);

                    float3 max = (float3)this.bounds.max - this.pointRadius;
                    float3 min = (float3)this.bounds.min + this.pointRadius;


                    if (Hint.Unlikely(nextPos.x > max.x))
                    {
                        vel = math.reflect(vel, Float3Extension.Left);
                        nextPos.x -= (nextPos.x - max.x) * 2.0f;
                    }
                    else if (Hint.Unlikely(nextPos.x < min.x))
                    {
                        vel = math.reflect(vel, Float3Extension.Right);
                        nextPos.x += (min.x - nextPos.x) * 2.0f;
                    }

                    if (Hint.Unlikely(nextPos.y > max.y))
                    {
                        vel = math.reflect(vel, Float3Extension.Down);
                        nextPos.y -= (nextPos.y - max.y) * 2.0f;
                    }
                    else if (Hint.Unlikely(nextPos.y < min.y))
                    {
                        vel = math.reflect(vel, Float3Extension.Up);
                        nextPos.y += (min.y - nextPos.y) * 2.0f;
                    }

                    if (Hint.Unlikely(nextPos.z > max.z))
                    {
                        vel = math.reflect(vel, Float3Extension.Back);
                        nextPos.z -= (nextPos.z - max.z) * 2.0f;
                    }
                    else if (Hint.Unlikely(nextPos.z < min.z))
                    {
                        vel = math.reflect(vel, Float3Extension.Forward);
                        nextPos.z += (min.z - nextPos.z) * 2.0f;
                    }

                    velocity.velocity = vel;
                    trs.Value.c3 = new float4(nextPos, 1.0f);
                    isMoving.isMoving = true;
                } else
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

            [NoAlias, ReadOnly]
            public NativeParallelHashSet<int> indices;

            [NoAlias, ReadOnly]
            public NativeList<float3> searchPositions;

            public void Execute(IndexComponent index, LocalToWorld trs, ref ColorComponent color)
            {
                bool contained = false;
                if (this.indices.Contains(index.idx))
                {
                    var pos = trs.Position;
                    for (int i = 0; i < this.searchPositions.Length; i++)
                    {
                        if (math.distancesq(pos, this.searchPositions[i]) < this.searchRadius * this.searchRadius)
                        {
                            color.value = this.insideColor;
                            contained = true;
                            break;
                        }
                    }
                }
                if (!contained)
                {
                    color.value = this.outsideColor;
                }
            }
        }

        [BurstCompile]
        public partial struct UpdateSparseOctreeJob : IJobEntity
        {
            [NoAlias, ReadOnly, DeallocateOnJobCompletion]
            public NativeArray<float3> oldPositions;

            [NoAlias]
            public NativeSparseOctree<int> octree;

            public void Execute(LocalToWorld trs, IndexComponent index, IsMovingComponent isMoving)
            {
                if (isMoving.isMoving)
                {
                    this.octree.Update(index.idx, this.oldPositions[index.idx], trs.Position);
                }
            }
        }



        [BurstCompile]
        public partial struct UpdateDenseOctreeJob : IJobEntity
        {
            [NoAlias, ReadOnly, DeallocateOnJobCompletion]
            public NativeArray<float3> oldPositions;

            [NoAlias]
            public NativeDenseOctree<int> octree;

            public void Execute(LocalToWorld trs, IndexComponent index, IsMovingComponent isMoving)
            {
                if (isMoving.isMoving)
                {
                    this.octree.Update(index.idx, this.oldPositions[index.idx], trs.Position);
                }
            }
        }

        [BurstCompile]
        private struct CollectMultiQueryIndicesJob : IJob
        {
            [NoAlias, ReadOnly]
            public NativeParallelMultiHashMap<uint, int> data;

            [NoAlias, ReadOnly]
            public NativeParallelHashSet<uint> cells;

            [NoAlias, WriteOnly]
            public NativeParallelHashSet<int> result;

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
        private struct CollectIndicesJob : IJob
        {
            [NoAlias, ReadOnly]
            public NativeParallelMultiHashMap<uint, int> data;

            [NoAlias, ReadOnly]
            public NativeList<uint> cells;

            [NoAlias, WriteOnly]
            public NativeParallelHashSet<int> result;

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


        public partial struct UpdateSystem : ISystem
        {

            private EntityQuery pointsQuery;

            [BurstCompile]
            public void OnCreate(ref SystemState state)
            {
                var queryBuilder = new EntityQueryBuilder(Allocator.Temp);
                this.pointsQuery = queryBuilder
                    .WithAll<LocalToWorld>()
                    .WithAll<Velocity3DComponent>()
                    .WithAll<IndexComponent>()
                    .WithAll<IsMovingComponent>()
                    .WithAll<ColorComponent>()
                    .Build(ref state);
            }

            [BurstCompile]
            public void OnUpdate(ref SystemState state)
            {
                bool foundSystemInfo = false;
                OctreeMovementSystemInfoComponent systemInfo = default;
                foreach(var sysInfo in SystemAPI.Query<OctreeMovementSystemInfoComponent>())
                {
                    systemInfo = sysInfo;
                    foundSystemInfo = true;
                    break;
                }

                if(foundSystemInfo)
                {
                    float delaTime = SystemAPI.Time.DeltaTime;
                    int entities = this.pointsQuery.CalculateEntityCount();
                    var oldPositions = new NativeArray<float3>(entities, Allocator.TempJob);

                    var updateJob = new UpdatePointJob()
                    {
                        deltaTime = delaTime,
                        bounds = systemInfo.bounds,
                        movingPercentage = systemInfo.movingPercentage,
                        oldPositions = oldPositions,
                        pointRadius = systemInfo.pointRadius,
                        rnd = systemInfo.rnd,
                    };
                    state.Dependency = updateJob.ScheduleParallel(this.pointsQuery, state.Dependency);

                    NativeList<uint> queryResults = new NativeList<uint>(Allocator.TempJob);
                    NativeParallelHashSet<uint> multiQueryResults = new NativeParallelHashSet<uint>(entities, Allocator.TempJob);
                    NativeParallelHashSet<int> allIndices = new NativeParallelHashSet<int>(8, Allocator.TempJob);
                    NativeArray<float> radii = new NativeArray<float>(systemInfo.searchPositions.Length, Allocator.TempJob);

                    if (systemInfo.useSparseOctree)
                    {

                        var updateOctreeJob = new UpdateSparseOctreeJob()
                        {
                            oldPositions = oldPositions,
                            octree = systemInfo.sparseOctree,
                        };
                        state.Dependency = updateOctreeJob.Schedule(this.pointsQuery, state.Dependency);

                        if(systemInfo.doMultiQuery)
                        {
                            for (int i = 0; i < systemInfo.searchPositions.Length; i++) radii[i] = systemInfo.searchRadius * 0.5f;

                            state.Dependency = systemInfo.sparseOctree.GetCellsInRadii(systemInfo.searchPositions.AsArray(), radii,
                                ref multiQueryResults, state.Dependency);

                            var collectIndicesJob = new CollectMultiQueryIndicesJob()
                            {
                                cells = multiQueryResults,
                                data = systemInfo.sparseOctree.GetDataBuckets(),
                                result = allIndices,
                            };

                            state.Dependency = collectIndicesJob.Schedule(state.Dependency);

                        } else
                        {
                            state.Dependency = systemInfo.sparseOctree.GetCellsInRadius(float3.zero, systemInfo.searchRadius,
                                ref queryResults, state.Dependency);

                            var collectIndicesJob = new CollectIndicesJob()
                            {
                                cells = queryResults,
                                data = systemInfo.sparseOctree.GetDataBuckets(),
                                result = allIndices,
                            };

                            state.Dependency = collectIndicesJob.Schedule(state.Dependency);
                        }

                    } else
                    {
                        var updateOctreeJob = new UpdateDenseOctreeJob()
                        {
                            oldPositions = oldPositions,
                            octree = systemInfo.denseOctree,
                        };
                        state.Dependency = updateOctreeJob.Schedule(this.pointsQuery, state.Dependency);

                        if(systemInfo.doMultiQuery)
                        {
                            for (int i = 0; i < systemInfo.searchPositions.Length; i++) radii[i] = systemInfo.searchRadius * 0.5f;

                            state.Dependency = systemInfo.denseOctree.GetCellsInRadii(systemInfo.searchPositions.AsArray(), radii,
                                ref multiQueryResults, state.Dependency);

                            var collectIndicesJob = new CollectMultiQueryIndicesJob()
                            {
                                cells = multiQueryResults,
                                data = systemInfo.denseOctree.GetDataBuckets(),
                                result = allIndices,
                            };

                            state.Dependency = collectIndicesJob.Schedule(state.Dependency);

                        }
                        else
                        {
                            state.Dependency = systemInfo.denseOctree.GetCellsInRadius(float3.zero, systemInfo.searchRadius,
                                ref queryResults, state.Dependency);

                            var collectIndicesJob = new CollectIndicesJob()
                            {
                                cells = queryResults,
                                data = systemInfo.denseOctree.GetDataBuckets(),
                                result = allIndices,
                            };

                            state.Dependency = collectIndicesJob.Schedule(state.Dependency);
                        }
                    }

                    state.Dependency = queryResults.Dispose(state.Dependency);
                    state.Dependency = multiQueryResults.Dispose(state.Dependency);
                    state.Dependency = radii.Dispose(state.Dependency);

                    var updateColorsJob = new ColorPointsJob()
                    {
                        indices = allIndices,
                        insideColor = systemInfo.insideColor,
                        outsideColor = systemInfo.outsideColor,
                        searchPositions = systemInfo.searchPositions,
                        searchRadius = systemInfo.doMultiQuery ? systemInfo.searchRadius * 0.5f : systemInfo.searchRadius,
                    };
                    state.Dependency = updateColorsJob.ScheduleParallel(this.pointsQuery, state.Dependency);
                    state.Dependency = allIndices.Dispose(state.Dependency);
                }
            }
        }

        private void FaceRingsTowardsCamera()
        {
            var camera = Camera.main;
            this.ringGO.transform.up = -camera.transform.forward;
            for(int i = 0; i < this.multiQueryRings.Length; i++)
            {
                this.multiQueryRings[i].transform.up = -camera.transform.forward;
            }
        }

        private void Update()
        {
            this.FaceRingsTowardsCamera();
        }

        private void Dispose()
        {
            this.rnd.DisposeIfCreated();
            this.searchPositions.DisposeIfCreated();
            if(this.octree.IsCreated)
            {
                this.octree.Dispose();
            }
        }

        private void OnDestroy()
        {
            this.Dispose();
        }
    }
}
