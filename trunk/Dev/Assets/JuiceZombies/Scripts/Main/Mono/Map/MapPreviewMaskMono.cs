//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-31 10:50:10
//---------------------------------------------------------------------


using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //TODO: 地图遮罩
    public class MapPreviewMaskMono : MonoBehaviour
    {
        private EntityManager entityManager;
        private EntityQuery elementQuery;
        private EntityQuery globalQuery;
        private EntityQuery prefabMapQuery;

        public bool isActive;

        private void Start()
        {
            isActive = false;
        }

        public void InitMask()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            elementQuery = entityManager.CreateEntityQuery(typeof(MapElementData), typeof(LocalTransform));

            // globalQuery = entityManager.CreateEntityQuery(typeof(GlobalConfigData));
            prefabMapQuery = entityManager.CreateEntityQuery(typeof(PrefabMapData));
            //var element = elementQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var entities = elementQuery.ToEntityArray(Allocator.Temp);
            if (entities.Length <= 0)
            {
                return;
            }

            // var configEntity = globalQuery.ToEntityArray(Allocator.Temp)[0];
            var temp = prefabMapQuery.ToEntityArray(Allocator.Temp);
            if (temp.Length <= 0)
            {
                return;
            }

            var prefabEntity = temp[0];
            var prefabMap = entityManager.GetComponentData<PrefabMapData>(prefabEntity);


            foreach (var entity in entities)
            {
                Debug.Log($"InitMask{entity}");
                var loc = entityManager.GetComponentData<LocalTransform>(entity);
                var id = entityManager.GetComponentData<MapElementData>(entity).elementID;
                int type = id / 1000;

                // ref var refreshArray = ref config.value.Value.configTbscene_modules.configTbscene_modules;

                Entity mask = default;
                if (type == 1)
                {
                    mask = entityManager.Instantiate(prefabMap.prefabHashMap["mask_green"]);
                }
                else if (type == 2)
                {
                    mask = entityManager.Instantiate(prefabMap.prefabHashMap["mask_red"]);
                }

                if (mask != default)
                {
                    entityManager.SetComponentData<LocalTransform>(mask, loc);
                }
            }

            isActive = false;
        }

        private void LateUpdate()
        {
            if (isActive)
            {
                Debug.Log("isActive");
                InitMask();
            }
        }
    }
}