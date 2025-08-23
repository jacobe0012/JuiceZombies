//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HotFix_UI;
using Main;
using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using BoxCollider = Unity.Physics.BoxCollider;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Spine.Unity;
using UnityEngine.SceneManagement;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_RunTimeHUD)]
    internal sealed class UIPanel_RunTimeHUDEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_RunTimeHUD;

        public override bool IsFromPool => false;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_RunTimeHUD>();
        }
    }


    public partial class UIPanel_RunTimeHUD : UI, IAwake<int>, IAwake
    {
        private long timerId;
        private EntityManager entityManager;
        private EntityQuery wbeQuery;
        private EntityQuery playerQuery;
        private EntityQuery enemyQuery;
        private EntityQuery gameRandomQuery;

        //private Tbbattle_exp battle_exp;
        private Tblanguage tblanguage;
        private Tbbattleshop_stage battleshop_stage;
        private Tbskill_binding player_skill_binding;
        private Tbskill_binding_rank player_skill_binding_rank;
        private Tblevel levelConfig;
        private Tbuser_variable user_variable;
        private Tbbattle_constant constant;
        private Tbskill tbskill;
        private Tbskill_quality tbskill_quality;
        private Tbguide tbguide;
        private Tbmonster tbmonster;
        private Tbmonster_attr tbmonster_attr;
        private Tbmonster_model tbmonster_model;
        private Tbmonster_template tbmonster_template;
        private Tbscene tbscene;
        private int currentLevel = 1;
        private int levelUpPoint;

        //public UI touchui;
        //public UI playerBarUI;
        private const int updateInternal = 200;
        private int livesucTime;
        public int livefailTime;
        private List<Vector3> sucList;
        //private List<Vector3> failList;

        private Dictionary<int, bool> sucBoolDic;
        private float time;
        private float frameCount;

        private float fps;


        /// <summary>
        /// 羁绊id 羁绊经验
        /// </summary>
        public Dictionary<int, int> bindingsDic = new Dictionary<int, int>();

        /// <summary>
        /// 已选技能id 等级
        /// </summary>
        public Dictionary<int, int> skillsDic = new Dictionary<int, int>();

        //大阶段
        public int bossState = 1;

        //小阶段
        public int state = 1;

        //小阶段
        private int shopId;
        public bool isLocked;

        //锁定的技能组
        public Dictionary<int, int> numIndexSkillIdDic = new Dictionary<int, int>();

        public List<int> displaySelectedTechs = new List<int>();


        /// <summary>
        /// 学习技能的总次数
        /// </summary>
        public int buySkillCount;

        /// <summary>
        /// 刷新技能的总次数
        /// </summary>
        public int refreshSkillCount;

        /// <summary>
        /// 武器技能是否进化
        /// </summary>
        public bool isEvolve;

        /// <summary>
        /// 最大经验羁绊id
        /// </summary>
        public int maxExpBindingId;

        /// <summary>
        /// 复活次数
        /// </summary>
        public int reBirthCount;

        public int panelId = 613;
        public Entity bossEntity;
        public bool isClose;

        /// <summary>
        /// 0正常 1新手序章
        /// </summary>
        public int type = 0;

        public int order = 0;

        private float3 itemDropPos;

        public async void Initialize(int args)
        {
            Log.Debug($"UIPanel_RunTimeHUD IntroGuide");
            //type = args;
            //PlayerPrefs.DeleteAll();
            InitJson();
            InitBindingsData();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag), typeof(GameTimeData),
                typeof(GameEnviromentData));
            playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
            gameRandomQuery = entityManager.CreateEntityQuery(typeof(GameRandomData));
            enemyQuery = entityManager.CreateEntityQuery(typeof(EnemyData));

            //UnityHelper.BeginTime();


            var KBtn_Pause = this.GetFromReference(UIPanel_RunTimeHUD.KBtn_Pause);

            var playerInput = this.GetFromReference(UIPanel_RunTimeHUD.KPlayerInput);
            var KBossHp = this.GetFromReference(UIPanel_RunTimeHUD.KBossHp);
            var KUISpecialEffect = this.GetFromReference(UIPanel_RunTimeHUD.KUISpecialEffect);
            KUISpecialEffect.SetActive(false);
            KBossHp.SetActive(false);
            playerInput.SetActive(true);

            var touchui = await UIHelper.CreateAsync(playerInput, UIType.UITouchController,
                playerInput.GameObject.transform, true);
            var KBase = touchui.GetFromReference(UITouchController.KBase);
            var KStick = touchui.GetFromReference(UITouchController.KStick);

            playerInput.AddChild(touchui);

            if (ResourcesSingleton.Instance.settingData.EnableShowStick)
            {
                KBase.GetImage().SetEnabled(true);
                KStick.GetImage().SetEnabled(true);
            }
            else
            {
                KBase.GetImage().SetEnabled(false);
                KStick.GetImage().SetEnabled(false);
            }


            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Pause, () =>
            {
                //JiYuUIHelper.StartStopTime(false);
                UIHelper.CreateAsync(UIType.UIPanel_BattleInfo);
            });
            UnityHelper.EnableTime(true);
            EnableInputBar(false);
            //JiYuUIHelper.StartStopTime(true);
            var order = tbguide.DataList.Where(a => a.group == args).OrderBy(a => a.order).ToList()[0];
            Log.Debug($"enter runtimeHud group:{args} order:{order}");
            IntroGuideOrder(order.order).Forget();
            //await UniTask.Delay(3000);

            //Guide_4();

            //SetEnv().Forget();
            // var playerBarUI = await UIHelper.CreateAsync(this, UIType.UIPlayerHp,
            //     this.GetComponent<RectTransform>(),
            //     true);
            // playerBarUI.GetRectTransform().SetAnchoredPositionY(-55);
            //
            // this.AddChild(playerBarUI);
        }

        public async UniTask PlaySpineUIFX(string spineName, float time)
        {
            Log.Error($"PlaySpineUIFX {spineName} {time}");
            var KUISpecialEffect = this.GetFromReference(UIPanel_RunTimeHUD.KUISpecialEffect);
            KUISpecialEffect.SetActive(true);
            var spineUI = this.GetFromReference(spineName);
            var sg = spineUI.GetComponent<SkeletonGraphic>();
            var duration = JiYuUIHelper.GetAnimaionDuration(sg, spineName);
            float scale = (duration / 1000f) / time;
            sg.timeScale = scale;
            sg.Initialize(true);
            spineUI.SetActive(true);
            await UniTask.Delay((int)(1000 * time));
            spineUI.SetActive(false);
        }

        public async UniTask<bool> CheckEnemyClear()
        {
            var enemyQuery = entityManager.CreateEntityQuery(typeof(EnemyData), typeof(ChaStats));

            while (true)
            {
                var chas = enemyQuery.ToComponentDataArray<ChaStats>(Allocator.Temp);
                bool isEmpty = true;
                foreach (var cha in chas)
                {
                    if (cha.chaResource.hp > 0)
                    {
                        isEmpty = false;
                    }
                }

                if (isEmpty)
                {
                    return true;
                }

                await UniTask.Delay(1000);
            }
        }

        public async UniTask<bool> CheckQueryExist(int id)
        {
            var enemyQuery = entityManager.CreateEntityQuery(typeof(IntroGuideItemData));

            while (true)
            {
                if (!enemyQuery.IsEmpty)
                {
                    var entites = enemyQuery.ToEntityArray(Allocator.Temp);
                    foreach (var entity in entites)
                    {
                        var itemData = entityManager.GetComponentData<IntroGuideItemData>(entity);
                        if (itemData.id == id)
                        {
                            return true;
                            break;
                        }
                    }
                }

                await UniTask.Delay(1000);
            }
        }

        async void InitGuidScene()
        {
            var global = Common.Instance.Get<Global>();

            var boss1Pos = new float3(0, 120, 0);
            SpawnEnemy(9, boss1Pos, out var monsterId);
            global.SetCameraPos(boss1Pos.xy);
            var obsPos = new float3(-25f, boss1Pos.y + 30, 0);
            var obsPos1 = new float3(25f, boss1Pos.y + 30, 0);
            var obsPos2 = new float3(0, boss1Pos.y + 10, 0);

            JiYuUIHelper.SpawnMapElement(20001, obsPos);
            JiYuUIHelper.SpawnMapElement(20001, obsPos1);

            obsPos2.x -= 15;
            JiYuUIHelper.SpawnMapElement("pic_guide_obstancle1", obsPos2, 15);
            obsPos2.x -= 10;
            JiYuUIHelper.SpawnMapElement("pic_guide_obstancle1", obsPos2, 15);
            obsPos2.x -= 10;
            JiYuUIHelper.SpawnMapElement("pic_guide_obstancle1", obsPos2, 15);
            obsPos2.x = 0;
            obsPos2.x += 15;
            JiYuUIHelper.SpawnMapElement("pic_guide_obstancle1", obsPos2, 15);
            obsPos2.x += 10;
            JiYuUIHelper.SpawnMapElement("pic_guide_obstancle1", obsPos2, 15);
            obsPos2.x += 10;
            JiYuUIHelper.SpawnMapElement("pic_guide_obstancle1", obsPos2, 15);


            while (!TryGetMonster(monsterId, out var e1, out var chaStats1))
            {
                await UniTask.Delay(100);
            }

            TryGetMonster(monsterId, out var e, out var chaStats);
            chaStats.chaControlState.cantWeaponAttack = true;
            chaStats.chaControlState.cantMove = true;
            entityManager.SetComponentData(e, chaStats);
            //await UniTask.Delay(2000);

            //JiYuUIHelper.TypeWriteEffect()
        }

        bool TryGetMonster(int monsterId, out Entity entity, out ChaStats chaStats)
        {
            chaStats = default;
            entity = Entity.Null;
            var query = entityManager.CreateEntityQuery(typeof(EnemyData));
            var qEntities = query.ToEntityArray(Allocator.Temp);
            foreach (var e in qEntities)
            {
                var enemyData = entityManager.GetComponentData<EnemyData>(e);
                if (enemyData.enemyID == monsterId)
                {
                    chaStats = entityManager.GetComponentData<ChaStats>(e);
                    entity = e;
                    return true;
                }
            }

            return false;
        }

        public async void OnGuideOrderFinished(int guideId)
        {
            JiYuUIHelper.TryFinishGuide(guideId);

            await UniTask.Delay(500);
            var order = tbguide.Get(guideId).order;
            if (order == 0)
            {
                return;
            }

            var last = tbguide.DataList.Where(a => a.order == order).OrderByDescending(s => s.id)
                .ToList()[0];
            if (last.id == guideId)
            {
                IntroGuideOrder(order + 1);
                Log.Debug($"开始order:{order + 1}");
            }
        }

        async UniTask IntroGuideOrder(int order)
        {
            //ResourcesSingleton.Instance.equipmentData.
            const int Normal = 90;
            const int Special = 70;
            var global = Common.Instance.Get<Global>();
            var orderList = tbguide.DataList.Where(a => a.order == order).ToList();
            int para1, para2, para3 = 0;
            var enemyquery = entityManager.CreateEntityQuery(typeof(EnemyData));
            var playerquery = entityManager.CreateEntityQuery(typeof(PlayerData));
            foreach (var guide in orderList)
            {
                switch (guide.guideType)
                {
                    case 101:
                        JiYuUIHelper.EnableGuide(false);
                        await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionShattersEnter);
                        OnGuideOrderFinished(guide.id);
                        break;
                    case 102:
                        JiYuUIHelper.EnableGuide(true);
                        await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionShattersExit);
                        OnGuideOrderFinished(guide.id);
                        break;
                    case 103:
                        JiYuUIHelper.EnableGuide(false);
                        //await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionShattersExit);
                        OnGuideOrderFinished(guide.id);
                        break;
                    case 104:
                        JiYuUIHelper.EnableGuide(true);
                        //await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionShattersExit);
                        OnGuideOrderFinished(guide.id);
                        break;
                    case 900:
                        JiYuUIHelper.EnableGuide(false);
                        InitGuidScene();
                        await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionShattersEnter);
                        OnGuideOrderFinished(guide.id);
                        break;
                    case 301:

                        var ui = await UIHelper.CreateAsync(UIType.UICommon_Dialog, guide.id);
                        ui.GetRectTransform().SetAnchoredPositionY(255f);
                        break;
                    case 201:

                        para1 = int.Parse(guide.guidePara[0]);
                        para2 = int.Parse(guide.guidePara[1]);
                        para3 = int.Parse(guide.guidePara[2]);
                        var fov201 = para1 == 2 ? Normal : Special;
                        global.DoCameraFOV(fov201, true, para3 / 1000f);
                        Entity target = Entity.Null;
                        //玩家
                        if (para2 == 1)
                        {
                            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
                            target = playerQuery.ToEntityArray(Allocator.Temp)[0];

                            var mySkeleton = entityManager.GetComponentData<TransformHybridUpdateData>(target);
                            var trans = mySkeleton.go.Value.GetComponent<Transform>();
                            global?.SetCameraTarget(trans);
                        }
                        //怪物
                        else if (para2 >= 1000000 && para2 <= 9999999)
                        {
                            var playerQuery0 = entityManager.CreateEntityQuery(typeof(EnemyData), typeof(ChaStats));
                            var enemies = playerQuery0.ToEntityArray(Allocator.Temp);
                            foreach (var enemy in enemies)
                            {
                                var enemyData = entityManager.GetComponentData<EnemyData>(enemy);
                                if (enemyData.enemyID == para2)
                                {
                                    target = enemy;
                                    break;
                                }
                            }

                            global.SetCameraTarget(target);
                        }
                        else
                        {
                            var playerQuery0 = entityManager.CreateEntityQuery(typeof(DropsData));
                            var enemies = playerQuery0.ToEntityArray(Allocator.Temp);
                            foreach (var enemy in enemies)
                            {
                                var enemyData = entityManager.GetComponentData<DropsData>(enemy);
                                if (enemyData.id == para2)
                                {
                                    target = enemy;
                                    break;
                                }
                            }

                            global.SetCameraTarget(target);
                        }

                        await UniTask.Delay(para3);

                        OnGuideOrderFinished(guide.id);
                        break;
                    case 202:


                        para1 = int.Parse(guide.guidePara[0]);
                        para2 = int.Parse(guide.guidePara[1]);
                        para3 = int.Parse(guide.guidePara[2]);
                        var fov202 = para1 == 2 ? Normal : Special;
                        global.DoCameraFOV(fov202, true, para3 / 1000f);
                        global.DoCameraPosOffset(new Vector2(0, para2 / 1000f), true, para3 / 1000f);
                        OnGuideOrderFinished(guide.id);
                        break;
                    case 321:

                        UIHelper.CreateAsync(UIType.UIPanel_GuideTips, guide.id);
                        OnGuideOrderFinished(guide.id);
                        break;
                    case 105:
                        para1 = int.Parse(guide.guidePara[0]);
                        para2 = int.Parse(guide.guidePara[1]);
                        para3 = int.Parse(guide.guidePara[2]);
                        //玩家
                        if (para1 == 1)
                        {
                            await JiYuUIHelper.DoPlayerPos(new float2(0, para2 / 1000f), para3 / 1000f);
                        }
                        //怪物
                        else if (para1 >= 1000000 && para1 <= 9999999)
                        {
                            if (para1 == 1500001)
                            {
                                var player = playerquery.ToEntityArray(Allocator.Temp)[0];
                                var tran = entityManager.GetComponentData<LocalTransform>(player);
                                await JiYuUIHelper.DoMonsterPos(para1, new float2(0, tran.Position.y + 60),
                                    para3 / 1000f);
                            }
                            else
                            {
                                await JiYuUIHelper.DoMonsterPos(para1, new float2(0, para2 / 1000f), para3 / 1000f);
                            }
                        }


                        OnGuideOrderFinished(guide.id);
                        break;
                    case 413:
                        para1 = int.Parse(guide.guidePara[0]);
                        //玩家
                        if (para1 == 1)
                        {
                        }
                        //怪物
                        else if (para1 >= 1000000 && para1 <= 9999999)
                        {
                            JiYuUIHelper.DestroyMonster(para1);
                        }


                        OnGuideOrderFinished(guide.id);
                        break;
                    // case 107:
                    //     para1 = int.Parse(guide.guidePara[0]);
                    //     para2 = int.Parse(guide.guidePara[1]);
                    //     para3 = int.Parse(guide.guidePara[2]);
                    //     //玩家
                    //     if (para1 == 1)
                    //     {
                    //         await JiYuUIHelper.DoPlayerPos(new float2(0, para2 / 1000f), para3 / 1000f);
                    //     }
                    //     //怪物
                    //     else if (para1 >= 1000000 && para1 <= 9999999)
                    //     {
                    //         var enemy = await JiYuUIHelper.DoMonsterPos(para1, new float2(0, para2 / 1000f),
                    //             para3 / 1000f);
                    //         entityManager.DestroyEntity(enemy);
                    //     }
                    //
                    //
                    //     OnGuideOrderFinished(guide.id);
                    //     break;
                    //锁定区域

                    //移动提示
                    case 311:
                        // para1 = int.Parse(guide.guidePara[0]);
                        // var key = guide.guidePara[1];
                        // para3 = int.Parse(guide.guidePara[2]);
                        await UIHelper.CreateAsync(this, UIType.UISubPanel_Guid, guide.id,
                            this.GameObject.transform);


                        break;

                    case 431:
                        para1 = int.Parse(guide.guidePara[0]);
                        para2 = int.Parse(guide.guidePara[1]);
                        itemDropPos = new float3(0, para2 / 1000f, 0);
                        JiYuUIHelper.SpawnDropItem(para1, new float3(0, para2 / 1000f, 0));
                        OnGuideOrderFinished(guide.id);
                        break;
                    case 203:
                        // Log.Debug($"empty203");
                        // var empty203 = await CheckQueryExist(3001);
                        // if (empty203)
                        // {
                        //     Log.Debug($"empty203:{empty203}");
                        //     global.SetCameraTarget(null);
                        //     global.SetCameraPos(float2.zero);
                        //     OnGuideOrderFinished(guide.id);
                        // }

                        break;
                    case 901:
                        Log.Debug($"empty11:");
                        para1 = int.Parse(guide.guidePara[0]);
                        var enemies901 = enemyquery.ToEntityArray(Allocator.Temp);
                        foreach (var e in enemies901)
                        {
                            var chaStats = entityManager.GetComponentData<ChaStats>(e);
                            chaStats.chaControlState.cantWeaponAttack = false;
                            chaStats.chaControlState.cantMove = false;
                            entityManager.SetComponentData(e, chaStats);
                        }

                        var empty = await CheckEnemyClear();
                        if (empty)
                        {
                            Log.Debug($"empty22:{empty}");
                            if (para1 == 2)
                            {
                                float3 enemiesPosBoss1 = new float3(0, 250, 0);
                                SpawnEnemy(9, enemiesPosBoss1, out var _);
                                await UniTask.DelayFrame(1);

                                var enemies = enemyquery.ToEntityArray(Allocator.Temp);
                                foreach (var e in enemies)
                                {
                                    var chaStats = entityManager.GetComponentData<ChaStats>(e);
                                    chaStats.chaControlState.cantWeaponAttack = true;
                                    chaStats.chaControlState.cantMove = true;
                                    entityManager.SetComponentData(e, chaStats);
                                }
                            }
                            else if (para1 == 3)
                            {
                                var player = playerquery.ToEntityArray(Allocator.Temp)[0];
                                var tran = entityManager.GetComponentData<LocalTransform>(player);
                                float3 enemiesPosBoss1 = new float3(0, tran.Position.y + 120, 0);
                                SpawnEnemy(11, enemiesPosBoss1, out var _);
                                await UniTask.DelayFrame(1);

                                var enemies = enemyquery.ToEntityArray(Allocator.Temp);
                                foreach (var e in enemies)
                                {
                                    var chaStats = entityManager.GetComponentData<ChaStats>(e);
                                    chaStats.chaControlState.cantWeaponAttack = true;
                                    chaStats.chaControlState.cantMove = true;
                                    entityManager.SetComponentData(e, chaStats);
                                }
                            }

                            OnGuideOrderFinished(guide.id);
                        }

                        break;
                    case 902:


                        var empty902 = await CheckQueryExist(3001);
                        if (empty902)
                        {
                            Log.Debug($"guide902:");
                            global.SetCameraTarget(null);
                            global.SetCameraPos(float2.zero);
                            UIHelper.CreateAsync(UIType.UIPanel_GuideSkillChoose);
                            var enemiesPos2 = new float3(0, 120, 0);
                            for (int i = 0; i < 3; i++)
                            {
                                SpawnEnemy(5, enemiesPos2, out var _);
                                SpawnEnemy(6, enemiesPos2, out var _);
                                SpawnEnemy(7, enemiesPos2, out var _);
                                SpawnEnemy(8, enemiesPos2, out var _);
                            }

                            await UniTask.DelayFrame(1);

                            var enemies902 = enemyquery.ToEntityArray(Allocator.Temp);
                            foreach (var e in enemies902)
                            {
                                var chaStats = entityManager.GetComponentData<ChaStats>(e);
                                chaStats.chaControlState.cantWeaponAttack = true;
                                chaStats.chaControlState.cantMove = true;
                                entityManager.SetComponentData(e, chaStats);
                            }

                            //OnGuideOrderFinished(guide.id);
                        }


                        break;
                    //丢C4
                    case 903:

                        var enemy903 = enemyquery.ToEntityArray(Allocator.Temp)[0];
                        var player903 = playerquery.ToEntityArray(Allocator.Temp)[0];
                        var enemy903Tran = entityManager.GetComponentData<LocalTransform>(enemy903);
                        var player903Tran = entityManager.GetComponentData<LocalTransform>(player903);
                        var c4 = ResourcesManager.Instantiate("Item_C4");
                        c4.transform.position = enemy903Tran.Position;

                        c4.transform.DOMove(player903Tran.Position, 3);
                        global.SetCameraTarget(c4.transform);
                        await UniTask.Delay(3000);
                        UIHelper.CreateAsync(UIType.UIPanel_GuideBoom);
                        // enemy903Tran.Position;
                        //Log.Debug($"guide903:");

                        OnGuideOrderFinished(guide.id);
                        break;
                    //切第一关场景并且掉落棒球棍
                    case 904:

                        Log.Debug($"guide904:");
                        var enemy904 = enemyquery.ToEntityArray(Allocator.Temp)[0];
                        var player904 = playerquery.ToEntityArray(Allocator.Temp)[0];
                        var enemy904Tran = entityManager.GetComponentData<LocalTransform>(enemy904);
                        var player904Tran = entityManager.GetComponentData<LocalTransform>(player904);
                        //JiYuUIHelper.DestoryWbe();
                        JiYuUIHelper.DestoryWbe();
                        WebMessageHandlerOld.Instance.Clear();
                        var sceneController = XFramework.Common.Instance.Get<SceneController>();
                        var sceneObj0 = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
                        await SceneResManager.WaitForCompleted(sceneObj0);
                        JiYuUIHelper.EnterChapter(1);
                        WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYCANSTART, OnClick1PlayBtnResponse);
                        var battleGain = new BattleGain
                        {
                            LevelId = ResourcesSingleton.Instance.levelInfo.levelId
                        };

                        NetWorkManager.Instance.SendMessage(CMD.QUERYCANSTART, battleGain);

                        //var sceneController = Common.Instance.Get<SceneController>();
                        // var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime);
                        // await SceneResManager.WaitForCompleted(sceneObj);


                        break;
                    //锁定
                    //TODO：
                    case 401:
                        para1 = int.Parse(guide.guidePara[0]);

                        OnGuideOrderFinished(guide.id);
                        break;
                    //解除锁定
                    case 402:

                        para1 = int.Parse(guide.guidePara[0]);
                        OnGuideOrderFinished(guide.id);

                        break;
                    //周身特效
                    case 106:

                        //para1 = int.Parse(guide.guidePara[0]);
                        OnGuideOrderFinished(guide.id);

                        break;
                }
            }
        }

        void OnClick1PlayBtnResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYCANSTART, OnClick1PlayBtnResponse);
            var longValue = new LongValue();
            longValue.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                //return;
            }

            Log.Debug($"验证对局是否可以开始:{longValue.Value}", Color.green);
            if (longValue.Value != null && longValue.Value > 0)
            {
                ResourcesSingleton.Instance.battleData.battleId = longValue.Value;
                //this.GetParent<UIPanel_JiyuGame>().DestoryAllToggle();
                //this.Close();
                //Close();
                var sceneController = Common.Instance.Get<SceneController>();
                var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime,LoadSceneMode.Additive);
                SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();
                var guide = tbguide.DataList.Where(a => a.guideType == 904).FirstOrDefault();
                OnGuideOrderFinished(guide.id);
                // var switchSceneData = switchSceneQuery.ToComponentDataArray<SwitchSceneData>(Allocator.Temp)[0];
                //  switchSceneData.mainScene.LoadAsync(new ContentSceneParameters()
                //  {
                //      loadSceneMode = LoadSceneMode.Single
                //  });
            }
            else
            {
                Log.Debug($"对局不可以开始{longValue.Value}", Color.green);
            }
        }

        public static void SpawnEnemy(int ruleId, float3 pos, out int monsterId)
        {
            var tblevel = ConfigManager.Instance.Tables.Tblevel;
            var tbscene = ConfigManager.Instance.Tables.Tbscene;
            var tbmonster_template = ConfigManager.Instance.Tables.Tbmonster_template;
            var scene = tbscene.Get(tblevel.Get(10000).sceneId);
            monsterId = 0;
            var template = tbmonster_template.DataList.Where(a => a.id == scene.monsterTemplateId && a.ruleId == ruleId)
                .FirstOrDefault();
            if (template == null)
            {
                Log.Error($"tbmonster_template.ruleId:{ruleId} 不存在 scene{scene.id}");
                return;
            }

            SpawnEnemy(template.id, ruleId, template.monsterId, pos);
            monsterId = template.monsterId;
        }

        public async static UniTask SpawnEnemy(int templateId, int ruleId, int monsterId, float3 pos)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag));

            while (query.IsEmpty)
            {
                await UniTask.Delay(200);
            }

            var wbe = query.ToEntityArray(Allocator.Temp)[0];
            var gameEvents = entityManager.GetBuffer<GameEvent>(wbe);
            gameEvents.Add(new GameEvent
            {
                CurrentTypeId = (GameEvent.TypeId)GameEvent.TypeId.GameEvent_999026,
                Int32_0 = 0,
                GameEventTriggerType_1 = GameEventTriggerType.ImmediateEffect,
                Single_2 = 0,
                Single_3 = 0,
                Single_4 = 0,
                Int32_5 = 0,
                Boolean_6 = false,
                int3_7 = new int3(monsterId, templateId, ruleId),
                int3_8 = (int3)pos,
                Int32_9 = 0,
                Int32_10 = 0,
                Boolean_11 = false,
                Boolean_12 = true,
                Boolean_13 = false,
                int3_14 = default,
                Int32_15 = 0,
                Int32_16 = 0,
                Int32_17 = 0
            });
        }


        public async void Initialize()
        {
            //type = args;
            //PlayerPrefs.DeleteAll();
            InitJson();
            InitBindingsData();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            sucList = levelConfig.Get(ResourcesSingleton.Instance.levelInfo.levelId).success;
            //failList = levelConfig.Get(ResourcesSingleton.Instance.levelInfo.levelId).fail;

            sucBoolDic = new Dictionary<int, bool>(sucList.Count);
            for (int i = 0; i < sucList.Count; i++)
            {
                sucBoolDic.Add(i, false);
            }

            wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag), typeof(GameTimeData),
                typeof(GameEnviromentData));
            playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
            gameRandomQuery = entityManager.CreateEntityQuery(typeof(GameRandomData));
            enemyQuery = entityManager.CreateEntityQuery(typeof(EnemyData));

            //UnityHelper.BeginTime();

            Log.Debug($"UIPanel_RunTimeHUD");
            var KBtn_Pause = this.GetFromReference(UIPanel_RunTimeHUD.KBtn_Pause);

            var playerInput = this.GetFromReference(UIPanel_RunTimeHUD.KPlayerInput);
            var KBossHp = this.GetFromReference(UIPanel_RunTimeHUD.KBossHp);
            KBossHp.SetActive(false);
            playerInput.SetActive(true);

            var touchui = await UIHelper.CreateAsync(playerInput, UIType.UITouchController,
                playerInput.GameObject.transform, true);
            playerInput.AddChild(touchui);
            var KBase = touchui.GetFromReference(UITouchController.KBase);
            var KStick = touchui.GetFromReference(UITouchController.KStick);
            if (ResourcesSingleton.Instance.settingData.EnableShowStick)
            {
                KBase.GetImage().SetEnabled(true);
                KStick.GetImage().SetEnabled(true);
            }
            else
            {
                KBase.GetImage().SetEnabled(false);
                KStick.GetImage().SetEnabled(false);
            }


            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Pause, () =>
            {
                //JiYuUIHelper.StartStopTime(false);
                UIHelper.CreateAsync(UIType.UIPanel_BattleInfo);
            });
            JiYuUIHelper.StartStopTime(true);

            //await UniTask.Delay(3000);

            //Guide_4();

            SetEnv().Forget();
            // var playerBarUI = await UIHelper.CreateAsync(this, UIType.UIPlayerHp,
            //     this.GetComponent<RectTransform>(),
            //     true);
            // playerBarUI.GetRectTransform().SetAnchoredPositionY(-55);
            //
            // this.AddChild(playerBarUI);
        }

        public void SetBossEntity(Entity entity)
        {
            bossEntity = entity;
        }

        public void SetBossText(int monsterId)
        {
            var KBossHp = this.GetFromReference(UIPanel_RunTimeHUD.KBossHp);
            var KText_BossHp = this.GetFromReference(UIPanel_RunTimeHUD.KText_BossHp);
            var KSlider_BossHp = this.GetFromReference(UIPanel_RunTimeHUD.KSlider_BossHp);

            KBossHp.SetActive(true);
            var monster = tbmonster.Get(monsterId);
            var monsterAttr = tbmonster_attr.GetOrDefault(monster.monsterAttrId);
            var monsterModel = tbmonster_model.GetOrDefault(monsterAttr.bookId);
            var closeStr = JiYuUIHelper.GetBulletTypeStr(tblanguage.Get(monsterModel.name).current);
            KText_BossHp.GetTextMeshPro().SetTMPText(closeStr);
        }

        public async UniTaskVoid SetBossHp(Entity entity)
        {
            var KBossHp = this.GetFromReference(UIPanel_RunTimeHUD.KBossHp);
            var KText_BossHp = this.GetFromReference(UIPanel_RunTimeHUD.KText_BossHp);
            var KSlider_BossHp = this.GetFromReference(UIPanel_RunTimeHUD.KSlider_BossHp);

            if (!KBossHp.GameObject.activeSelf)
            {
                return;
            }

            if (!entityManager.Exists(entity))
            {
                return;
            }

            //KBossHp.SetActive(true);
            var chaStats = entityManager.GetComponentData<ChaStats>(entity);
            var ratios = chaStats.chaResource.hp / (float)chaStats.chaProperty.maxHp;
            KSlider_BossHp.GetImage().DoFillAmount(ratios, 0.2f);

            if (chaStats.chaResource.hp <= 0)
            {
                //await UniTask.Delay(1000);
                KBossHp.SetActive(false);
                //return;
            }
        }

        public async UniTaskVoid Guide_4()
        {
            // foreach (var guide in tbguide.DataList)
            // {
            //     if (guide.targetId == panelId)
            //     {
            //         if (guide.triggerType == 5 &&
            //             guide.triggerPara[0] == ResourcesSingleton.Instance.levelInfo.chapterID)
            //         {
            //             if (ResourcesSingleton.Instance.settingData.GuideList.Contains(guide.group))
            //             {
            //                 await UIHelper.CreateAsync(this, UIType.UISubPanel_Guid, guide.id,
            //                     this.GameObject.transform);
            //                 //break;
            //             }
            //         }
            //     }
            // }
        }
        // async UniTask CreateWeather()
        // {
        //     if (expr)
        //     {
        //         
        //     }
        // }

        public async UniTaskVoid SetEnv()
        {
            while (wbeQuery.IsEmpty)
            {
                await UniTask.Delay(200);
            }

            var entity = wbeQuery.ToEntityArray(Allocator.Temp)[0];
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var env = entityManager.GetComponentData<GameEnviromentData>(entity);

            var tbenvironment = ConfigManager.Instance.Tables.Tbenvironment;
            var KPanel_Battle_Environment = this.GetFromReference(UIPanel_RunTimeHUD.KPanel_Battle_Environment);
            var KDefault = KPanel_Battle_Environment.GetFromReference(UIPanel_Battle_Environment.KDefault);
            var KRain = KPanel_Battle_Environment.GetFromReference(UIPanel_Battle_Environment.KRain);
            var KSnow = KPanel_Battle_Environment.GetFromReference(UIPanel_Battle_Environment.KSnow);
            var KRainbow = KPanel_Battle_Environment.GetFromReference(UIPanel_Battle_Environment.KRainbow);
            var KWind = KPanel_Battle_Environment.GetFromReference(UIPanel_Battle_Environment.KWind);
            var KNight = this.GetFromReference(UIPanel_RunTimeHUD.KNight);


            KDefault.SetActive(false);
            KRain.SetActive(false);
            KSnow.SetActive(false);
            KRainbow.SetActive(false);
            KWind.SetActive(false);
            switch (env.env.weather)
            {
                case 100:
                    KDefault.SetActive(true);
                    break;
                case 101:
                    KWind.SetActive(true);
                    break;
                case 102:
                    KRain.SetActive(true);
                    break;
                case 103:
                    KSnow.SetActive(true);
                    break;
                case 104:

                    break;
                case 105:
                    var environment = tbenvironment.Get(105);
                    int delay = environment.para[0];
                    int duration = environment.para[2];
                    int eventId = environment.para[1];
                    delay = Mathf.Max(0, delay - 2000);
                    while (!isClose)
                    {
                        await UniTask.Delay(delay);

                        //事件
                        var gameEvent = entityManager.GetBuffer<GameEvent>(player);
                        gameEvent.Add(new GameEvent
                        {
                            Int32_0 = eventId,
                            Boolean_6 = true
                        });

                        KRainbow.SetActive(true);
                        await UniTask.Delay(duration);
                        KRainbow.SetActive(false);
                    }


                    break;
                default:
                    KDefault.SetActive(true);
                    break;
            }

            KNight.SetActive(false);

            switch (env.env.time)
            {
                case 200:

                    break;
                case 201:
                    var nigthRect = KNight.GetRectTransform();
                    nigthRect.SetWidth(1);
                    KNight.GetRectTransform().SetHeight(1);
                    const float OriginWidth = 50000f;
                    var ratios = tbenvironment.Get(201).para[0] / 90f;

                    KNight.GetRectTransform().SetWidth(OriginWidth * ratios);
                    KNight.GetRectTransform().SetHeight(OriginWidth * ratios);

                    //KNight.GetRectTransform().SetScale2(ratios);
                    KNight.SetActive(true);
                    break;
            }
        }

        public void InitJson()
        {
            //battle_exp = ConfigManager.Instance.Tables.Tbbattle_exp;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            battleshop_stage = ConfigManager.Instance.Tables.Tbbattleshop_stage;
            player_skill_binding = ConfigManager.Instance.Tables.Tbskill_binding;
            player_skill_binding_rank = ConfigManager.Instance.Tables.Tbskill_binding_rank;
            levelConfig = ConfigManager.Instance.Tables.Tblevel;
            user_variable = ConfigManager.Instance.Tables.Tbuser_variable;
            constant = ConfigManager.Instance.Tables.Tbbattle_constant;
            tbskill = ConfigManager.Instance.Tables.Tbskill;
            tbskill_quality = ConfigManager.Instance.Tables.Tbskill_quality;
            tbguide = ConfigManager.Instance.Tables.Tbguide;
            tbmonster = ConfigManager.Instance.Tables.Tbmonster;
            tbmonster_attr = ConfigManager.Instance.Tables.Tbmonster_attr;
            tbmonster_model = ConfigManager.Instance.Tables.Tbmonster_model;
            tbscene = ConfigManager.Instance.Tables.Tbscene;
            tbmonster_template = ConfigManager.Instance.Tables.Tbmonster_template;
        }


        /// <summary>
        /// 获取一个真实技能id
        /// </summary>
        /// <param name="skillId">一级技能id</param>
        public int GetTrueSkillId(int skillId)
        {
            int trueSkillId = skillId;
            if (this.skillsDic.ContainsKey(skillId))
            {
                trueSkillId = skillId - 1 + this.skillsDic[skillId];
            }

            return trueSkillId;
        }

        /// <summary>
        /// 增加或升级一个羁绊技能
        /// </summary>
        /// <param name="skillId"></param>
        public void AddSkill(int skillId)
        {
            var curSkill = tbskill.Get(skillId);
            if (curSkill.type != 2)
            {
                Log.Error($"此技能非羁绊技能");
                return;
            }

            var bindingId = curSkill.skillBindingId[0];
            //var playerSkill = player_skill.Get(playerSkillGroup.default0);
            var playerSkillQuality = tbskill_quality.Get(curSkill.skillQualityId);
            var playerSkillBinding = player_skill_binding.Get(bindingId);
            var bindingExp = playerSkillQuality.bindingExp;

            if (this.skillsDic.ContainsKey(skillId))
            {
                this.skillsDic[skillId]++;
            }
            else
            {
                this.skillsDic.Add(skillId, 1);
            }

            if (this.bindingsDic.ContainsKey(bindingId))
            {
                this.bindingsDic[bindingId] += bindingExp;

                if (this.bindingsDic.ContainsKey(this.maxExpBindingId))
                {
                    if (this.bindingsDic[bindingId] > this.bindingsDic[this.maxExpBindingId])
                    {
                        this.maxExpBindingId = bindingId;
                    }
                }
                else
                {
                    this.maxExpBindingId = bindingId;
                }
            }
        }

        public void InitBindingsData()
        {
            foreach (var item in player_skill_binding.DataList)
            {
                bindingsDic.TryAdd(item.id, 0);
            }

            int num = constant.Get("battleshop_refresh_num").constantValue;

            for (int i = 0; i < num; i++)
            {
                numIndexSkillIdDic.TryAdd(i, 0);
            }


            //Log.Error($"bindingsDic{bindingsDic.Count}");
        }

        public bool JudgeNewSkill()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
            if (entityQuery.IsEmpty) return false;
            var player = entityQuery.ToEntityArray(Allocator.Temp)[0];
            var buffer = entityManager.GetBuffer<BuffOld>(player);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Int32_0 == 101824)
                {
                    return true;
                }
            }

            return false;
        }

        // void AddInvincibleBuffToPlayer()
        // {
        //     var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //     var entityQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
        //     if (entityQuery.IsEmpty) return;
        //     var player = entityQuery.ToEntityArray(Allocator.Temp)[0];
        //     var buffer = entityManager.GetBuffer<BuffOld>(player);
        //     // buffer.Add(new Buff_20002001
        //     // {
        //     //     id = 20002001,
        //     //     priority = 0,
        //     //     maxStack = 0,
        //     //     tags = 0,
        //     //     tickTime = 0,
        //     //     timeElapsed = 0,
        //     //     duration = 10,
        //     //     permanent = false,
        //     //     caster = default,
        //     //     carrier = player,
        //     //     canBeStacked = false,
        //     //     buffStack = default,
        //     //     buffArgs = default
        //     // }.ToBuffOld());
        // }


        /// <summary>
        /// 启用输入摇杆
        /// </summary>
        public void EnableInputBar(bool enable)
        {
            var playerInput = this?.GetFromReference(KPlayerInput);
            playerInput?.SetActive(enable);
            // var pause = this.GetFromReference(KPauseBG);
            // pause.SetActive(true);
        }


        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer(bool enable)
        {
            if (enable)
            {
                //开启一个每帧执行的任务，相当于Update
                var timerMgr = TimerManager.Instance;
                //timerId = timerMgr.StartRepeatedTimer(updateInternal, this.Update);
                timerId = timerMgr.RepeatedFrameTimer(this.Update);
            }
            else
            {
                var timerMgr = TimerManager.Instance;
                timerMgr?.RemoveTimerId(ref this.timerId);
                this.timerId = 0;
            }
        }

        /// <summary>
        /// 减少一点技能点
        /// </summary>
        public void ReduceLevelUpPoint()
        {
            if (levelUpPoint > 0)
            {
                levelUpPoint--;
            }
        }


        public async void Update()
        {
            if (wbeQuery.IsEmpty) return;
            var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
            var timeData = entityManager.GetComponentData<GameTimeData>(wbe);
            var enviromentData = entityManager.GetComponentData<GameEnviromentData>(wbe);
            var prefabMapData = entityManager.GetComponentData<PrefabMapData>(wbe);
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var tran = entityManager.GetComponentData<LocalTransform>(player);
            var chaStats = entityManager.GetComponentData<ChaStats>(player);

            if ((int)timeData.refreshTime.elapsedTime == 1)
            {
                Guide_4();
            }

            if (sucBoolDic == null || sucBoolDic?.Count <= 0)
            {
                return;
            }

            SetBossHp(bossEntity);

            var playerData = entityManager.GetComponentData<PlayerData>(player);

            var KText_Time = this.GetFromReference(UIPanel_RunTimeHUD.KText_Time);
            var KText_KillEnemy = this.GetFromReference(UIPanel_RunTimeHUD.KText_KillEnemy);
            var KText_Money = this.GetFromReference(UIPanel_RunTimeHUD.KText_Money);


            // level.GetTextMeshPro().SetTMPText(playerData.playerData.level.ToString());
            KText_Money?.GetTextMeshPro()?.SetTMPText(playerData.playerData.exp.ToString());
            KText_KillEnemy?.GetTextMeshPro()?.SetTMPText(playerData.playerData.killEnemy.ToString());
            KText_Time?.GetTextMeshPro()?.SetTMPText(UnityHelper.ToTimeFormat(timeData.refreshTime.elapsedTime));

            //winCondition
            var enemies = enemyQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < sucList.Count; i++)
            {
                if (sucBoolDic[i])
                {
                    continue;
                }

                if (sucList[i].x == 1)
                {
                    sucBoolDic[i] = true;
                }
                else if (sucList[i].x == 2)
                {
                    foreach (var enemy in enemies)
                    {
                        var enemyData = entityManager.GetComponentData<EnemyData>(enemy);
                        if (enemyData.enemyID == sucList[i].z)
                        {
                            var enemyChaStats = entityManager.GetComponentData<ChaStats>(enemy);
                            if (enemyChaStats.chaResource.hp <= 0)
                            {
                                sucBoolDic[i] = true;
                                break;
                            }
                        }
                    }
                }
                else if (sucList[i].x == 3)
                {
                    if (timeData.refreshTime.elapsedTime > sucList[i].z)
                    {
                        sucBoolDic[i] = true;
                    }
                }
            }

            bool suc = true;
            foreach (var sucBool in sucBoolDic)
            {
                if (!sucBool.Value || chaStats.chaResource.hp <= 0)
                {
                    suc = false;
                    break;
                }
            }

            if (suc && !JiYuUIHelper.TryGetUI(UIType.UIPanel_Success, out var _))
            {
                UIHelper.Remove(UIType.UIPanel_Fail);
                UIHelper.Remove(UIType.UIPanel_Rebirth);
                //JiYuUIHelper.StartStopTime(false);
                UIHelper.Create(UIType.UIPanel_Success);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                UIHelper.CreateAsync(UIType.UIPanel_GuideSkillChoose);
            }
            //failCondition
            /*
            if (chaStats.chaResource.hp <= 0 && !JiYuUIHelper.TryGetUI(UIType.UIPanel_Rebirth, out var _) &&
                !JiYuUIHelper.TryGetUI(UIType.UIPanel_Fail, out var _) &&
                !JiYuUIHelper.TryGetUI(UIType.UIPanel_Success, out var _) &&
                !JiYuUIHelper.TryGetUI(UIType.UIPanel_BattleInfo, out var _))
            {

                if (chaStats.chaProperty.rebirthCount1 > 0)
                {
                    chaStats.chaProperty.rebirthCount1--;
                    entityManager.SetComponentData(player, chaStats);
                    JiYuUIHelper.RebirthPlayer();
                }
                else if (chaStats.chaProperty.rebirthCount > 0)
                {
                    chaStats.chaProperty.rebirthCount--;
                    entityManager.SetComponentData(player, chaStats);
                    JiYuUIHelper.RebirthPlayer();
                }
                else
                {
                    //JiYuUIHelper.StartStopTime(false);
                    UIHelper.Create(UIType.UIPanel_Rebirth);
                }
            }
            */

            #region FPSShow

            var KEnemyNum = GetFromReference(UIPanel_RunTimeHUD.KEnemyNum);
            var KFPS = GetFromReference(UIPanel_RunTimeHUD.KFPS);
            var count = entityManager.CreateEntityQuery(typeof(PhysicsCollider)).CalculateEntityCount();
            //statematchine show

            //entities counts
            KEnemyNum.GetTextMeshPro().SetTMPText($"Colliders: {count}");
            //Fps
            if (time >= 1 && frameCount >= 1)
            {
                fps = frameCount / time;

                time = 0;

                frameCount = 0;
            }

            KFPS.GetTextMeshPro().SetColor(fps >= 60
                ? Color.white
                : (fps > 40 ? Color.yellow : Color.red));
            KFPS.GetTextMeshPro().SetTMPText("FPS:" + fps.ToString("f2"));

            time += Time.deltaTime;

            frameCount++;

            #endregion
        }


        protected override void OnClose()
        {
            JiYuUIHelper.StartStopTime(false);
            isClose = true;
            sucBoolDic.Clear();
            bindingsDic.Clear();
            skillsDic.Clear();
            numIndexSkillIdDic.Clear();
            base.OnClose();
        }
    }
}