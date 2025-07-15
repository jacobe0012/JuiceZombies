//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;

//namespace Main
//{
//    //生成火焰地形的attack
//    public partial struct SkillAttack_800001 : ISkillAttack
//    {
//        /// <summary>
//        /// 0
//        /// </summary>
//        public int id;

//        /// <summary>
//        /// 1 一次性释放的技能实体 值=0
//        /// </summary>
//        public float duration;

//        /// <summary>
//        /// 2
//        /// </summary>
//        public int tick;

//        /// <summary>
//        /// 3 技能实体释放者
//        /// </summary>
//        public Entity caster;

//        /// <summary>
//        /// 4 是否是弹幕
//        /// </summary>
//        public bool isBullet;

//        /// <summary>
//        /// 5 弹幕hp
//        /// </summary>
//        public int hp;

//        /// <summary>
//        /// 实体的击中目标 6
//        /// </summary>
//        public Entity target;

//        /// <summary>
//        /// 7
//        /// </summary>
//        public int4 args;

//        /// <summary>
//        /// 弹幕速度 8
//        /// </summary>
//        public float speed;

//        /// <summary>
//        /// 弹幕用 触发器id 9
//        /// </summary>
//        public int triggerID;

//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //UnityEngine.Debug.LogError("DoSkillMove");
//            //var c = refData.cdfePhysicsCollider[inData.entity];

//            //BuffHelper.SetSkillAttackTarget(refData.ecb, inData.sortkey, inData.entity, inData.config, id, c);


//            return refData.cdfeLocalTransform[inData.entity];
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {


//           // UnityEngine.Debug.LogError("OnDestroy");
//            var buff = BuffHelper.UpdateParmeter(inData.config, id, 30000014);
//            var skillDuration = buff[0].x/1000f;
//            var areaId = buff[1].x;
//            var width = buff[2].x;
//            var height = buff[3].x;


//            ref var mapModuleArray = ref inData.config.value.Value.configTbscene_modules.configTbscene_modules;
//            FixedString128Bytes prefabName = "";
//            var temp = new BlobArray<int>();
//            ref BlobArray<int> currentEvent = ref temp;
//            for (int i = 0; i < mapModuleArray.Length; ++i)
//            {
//                if (mapModuleArray[i].id == areaId)
//                {
//                    prefabName = mapModuleArray[i].pic;
//                    currentEvent =ref  mapModuleArray[i].event0;
//                    break;
//                }
//            }

//            var prefab = inData.prefabMapData.prefabHashMap[prefabName];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);


//            var newpos = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.entity].Position,
//                //TODO:大小*10
//                Scale = refData.cdfeLocalTransform[prefab].Scale,
//                Rotation = refData.cdfeLocalTransform[prefab].Rotation,
//            };

//            refData.ecb.SetComponent(inData.sortkey, ins, newpos);

//            refData.ecb.AddComponent(inData.sortkey, ins, new PostTransformMatrix
//            {
//                Value = float4x4.Scale(width, height, 1)
//            });


//            refData.ecb.AddComponent(inData.sortkey, ins,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_0
//                    {
//                        id = id,
//                        duration = skillDuration,
//                        tick = 0,
//                        caster = inData.entity,
//                        isBullet = false,
//                        hp = 0,
//                    }.ToSkillAttack()
//                });

//            refData.ecb.AddComponent(inData.sortkey, ins,
//                                new MapElementData { elementID = areaId });
//            refData.ecb.RemoveComponent<MapElementData>(inData.sortkey, ins);
//            refData.ecb.AddComponent(inData.sortkey, ins, new AreaTag());

//            ref var events = ref inData.config.value.Value.configTbevent_0s.configTbevent_0s;

//            for (int i = 0; i < currentEvent.Length; i++)
//            {
//                for (int j = 0; j < events.Length; j++)
//                {
//                    if (currentEvent[i] == events[j].id)
//                    {
//                        int tempEvent = events[j].id;
//                        AddEventTo(tempEvent, ins, refData.ecb, inData.config, inData.sortkey);
//                        break;
//                    }

//                }
//            }
//        }
//        private void AddEventTo(int eventID, Entity entity, EntityCommandBuffer.ParallelWriter ecb, GlobalConfigData globalConfig, int sortkey)
//        {
//            ref var eventsTable = ref globalConfig.value.Value.configTbevent_0s.configTbevent_0s;
//            int triggerType = 0, type = 0;
//            for (int i = 0; i < eventsTable.Length; i++)
//            {
//                if (eventsTable[i].id == eventID)
//                {
//                    triggerType = eventsTable[i].triggerType;
//                    type = eventsTable[i].type;
//                }
//            }
//            ecb.AddBuffer<GameEvent>(sortkey, entity);

//            switch (eventID)
//            {
//                case 1001:
//                    ecb.AppendToBuffer<GameEvent>(sortkey, entity, new GameEvent_1001
//                    {
//                        id = eventID,
//                        triggerType = triggerType,
//                        eventType = type,
//                        triggerGap = 0,
//                        remainTime = 1f,
//                        duration = 0,
//                        isPermanent = true,
//                        target = default,
//                        onceTime = 0,
//                        colliderScale = 0,
//                        delayTime = 1f
//                    }.ToGameEvent());

//                    break;
//                case 1002:
//                    ecb.AppendToBuffer<GameEvent>(sortkey, entity, new GameEvent_1002 { id = eventID, triggerType = triggerType, eventType = type, triggerGap = 0, remainTime = 1f, duration = 0, isPermanent = true, target = default, onceTime = 0, colliderScale = 0, delayTime = 1f }.ToGameEvent());

//                    break;
//                case 1004:
//                    ecb.AppendToBuffer<GameEvent>(sortkey, entity, new GameEvent_1004 { id = eventID, triggerType = triggerType, eventType = type, triggerGap = 0, remainTime = 1f, duration = 0, isPermanent = true, target = default, onceTime = 0, colliderScale = 0, delayTime = 1f }.ToGameEvent());

//                    break;
//                case 1005:
//                    ecb.AppendToBuffer<GameEvent>(sortkey, entity, new GameEvent_1005 { id = eventID, triggerType = triggerType, eventType = type, triggerGap = 0, remainTime = 1f, duration = 0, isPermanent = true, target = default, onceTime = 0, colliderScale = 0, delayTime = 1f }.ToGameEvent());

//                    break;
//                //case 1006:
//                //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                //    GameEvent.Add(new GameEvent_1001
//                //    {

//                //        id = 1006,

//                //        isPermanent = true,
//                //        remainTime = 1,
//                //        target = default,

//                //    }.ToGameEvent());
//                //    break;
//                case 1007:
//                    ecb.AppendToBuffer<GameEvent>(sortkey, entity, new GameEvent_1007 { id = eventID, eventType = type, triggerType = triggerType, isPermanent = true, duration = 0f, remainTime = 1f, target = default, triggerGap = 0f }.ToGameEvent());
//                    break;
//                case 1008:
//                    ecb.AppendToBuffer<GameEvent>(sortkey, entity, new GameEvent_1008 { id = eventID, eventType = type, triggerType = triggerType, isPermanent = true, duration = 0f, remainTime = 1f, target = default, triggerGap = 2f, delayTime = 0 }.ToGameEvent());

//                    break;
//                //case 1009:
//                //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                //    GameEvent.Add(new GameEvent_1009
//                //    {
//                //        id = 1009,
//                //        isPermanent = false,
//                //        remainTime = 1,
//                //        target = default,
//                //    }.ToGameEvent());
//                //    break;
//                //case 1010:
//                //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                //    GameEvent.Add(new GameEvent_1010
//                //    {
//                //        id = 1010,
//                //        isPermanent = true,
//                //        remainTime = 1,
//                //        target = default,
//                //    }.ToGameEvent());
//                //    break;
//                //case 2002:
//                //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                //    GameEvent.Add(new GameEvent_2002
//                //    {

//                //        id = 2002,
//                //        isPermanent = true,
//                //        remainTime = 1,
//                //        target = default,
//                //    }.ToGameEvent());
//                //    break;
//                //case 2003:
//                //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                //    GameEvent.Add(new GameEvent_2003
//                //    {

//                //        id = 2003,

//                //        isPermanent = true,
//                //        remainTime = 1,
//                //        target = default,

//                //    }.ToGameEvent());
//                //    break;
//                //case 2004:
//                //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                //    GameEvent.Add(new GameEvent_2004
//                //    {

//                //        id = 2004,
//                //        isPermanent = true,
//                //        remainTime = 1,
//                //        target = default,
//                //    }.ToGameEvent());
//                //    break;
//                case 2005:
//                    ecb.AppendToBuffer<GameEvent>(sortkey, entity, new GameEvent_2005
//                    {
//                        id = 2005,
//                        triggerType = triggerType,
//                        eventType = type,
//                        triggerGap = 0,
//                        remainTime = 0,
//                        duration = 0,
//                        isPermanent = true,
//                        target = default,
//                        onceTime = 0,
//                        colliderScale = 0,
//                        delayTime = 0
//                    }.ToGameEvent());

//                    break;
//            }
//        }
//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }
//    }
//}

