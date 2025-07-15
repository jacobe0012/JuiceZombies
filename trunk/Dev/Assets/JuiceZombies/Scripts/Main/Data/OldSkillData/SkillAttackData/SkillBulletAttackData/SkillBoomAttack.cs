//using cfg.blobstruct;
//using System.Threading;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;

//namespace Main
//{
//    //炸弹爆炸
//    public partial struct SkillBoomAttack : ISkillAttack
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
//        /// 6 是否是持续性攻击
//        /// </summary>
//        public bool stayAttack;

//        /// <summary>
//        /// 7 持续性攻击间隔
//        /// </summary>
//        public float stayAttackInterval;

//        /// <summary>
//        /// 8 当前持续性攻击间隔
//        /// </summary>
//        public float curStayAttackInterval;


//        /// <summary>
//        /// 技能实体执行延时 单位s  9
//        /// </summary>
//        public float skillDelay;

//        /// <summary>
//        /// 实体的击中目标 10
//        /// </summary>
//        public Entity target;

//        public int speed;

//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            return refData.cdfeLocalTransform[inData.entity];
//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {


//            var buff3 = BuffHelper.UpdateParmeter(inData.config, id, 30000010);
//            var damageAddtion = buff3[2].x;

//            var targets = inData.otherEntities;
//            var currentLayer = 1;
//            for(int i=0;i<targets.Length;i++)
//            {
//                var buff= inData.bfeBuff[targets[i]];
//                for(int j = 0; j < buff.Length; j++)
//                {
//                    if (buff[j].Int32_0== 20003017)
//                    {
//                        currentLayer = buff[j].BuffStack_12.stack;
//                        refData.ecb.AppendToBuffer(inData.sortkey, targets[i], new Buff_30000003 { caster = inData.player, carrier = targets[i], buffArgs = new BuffArgs { args0 = damageAddtion*currentLayer } }.ToBuffOld());
//                        buff.RemoveAt(j);
//                    }
//                }
//            }
//        }


//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//    }
//}

