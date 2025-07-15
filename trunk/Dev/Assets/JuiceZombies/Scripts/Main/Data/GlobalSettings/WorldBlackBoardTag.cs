using System;
using FMOD.Studio;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Main
{
    public struct WorldBlackBoardTag : IComponentData
    {
    }

    public struct GameSetUpData : IComponentData
    {
        public bool enableDamageNumber;
        public bool enableSoundEffects;
    }


    public struct GameEnviromentData : IComponentData
    {
        public Enviroment env;
    }

    public struct Enviroment
    {
        /// <summary>
        /// 100 - default
        /// 101 - wind
        /// 102 - rain
        /// 103 - snow
        /// 104 - lei
        /// 105 - rainbow
        /// </summary>
        public int weather;

        public int time;


        public int lastweather;
        public int lasttime;

        public int lastWeather;


        public int lastTime;
    }

    public struct GameOthersData : IComponentData, IDisposable
    {
        /// <summary>
        /// 音频映射
        /// </summary>
        public NativeHashMap<FixedString128Bytes, EventDescription> allAudioClips;

        /// <summary>
        /// 动画映射 弃用
        /// </summary>
        //public NativeHashMap<FixedString128Bytes, int> animations;

        /// <summary>
        /// 当前关卡id
        /// </summary>
        public int levelId;

        /// <summary>
        /// 当前场景id
        /// </summary>
        public int sceneId;

        /// <summary>
        /// 怪物刷新id
        /// </summary>
        public int monsterRefreshId;


        /// <summary>
        /// 拾取持续时间
        /// </summary>
        public int pickupDuration;

        /// <summary>
        /// 拾取时道具远离距离
        /// </summary>
        public int dropPoint1Offset;

        /// <summary>
        /// 碰撞最大数量
        /// </summary>
        //public int hitBackMaxCount;

        //
        public MapData mapData;

        //Test
        //public bool enableEnemyWeapon;
        public bool enableTest1;
        public bool enableTest2;


        /// <summary>
        /// 当前正在打的bossEntity
        /// </summary>
        public Entity BossEntity;

        /// <summary>
        /// CameraTarget
        /// </summary>
        public Entity CameraTarget;

        /// <summary>
        /// 当前战斗商店阶段数
        /// </summary>
        public int battleShopStage;

        public GameOtherParas gameOtherParas;

        public struct GameOtherParas
        {
            public float getHitDuration;
            public float getHitOffset;

            public float alphaSpeed;
            public float dissolveSpeed;

            public float dropAnimedDuration;
            public float dropAnimedHeight;
        }

        public void Dispose()
        {
            allAudioClips.Dispose();
        }
    }

    public struct GameRandomData : IComponentData
    {
        public Random rand;

        /// <summary>
        /// 
        /// </summary>
        public uint seed;
    }

    /// <summary>
    /// 全局的只读map数据 一个关卡的mapData是固定不变的
    /// </summary>
    public struct MapData
    {
        public int mapID;
        public int mapType;
        public float2 mapSize;
    }

    public struct JiYuTime
    {
        /// <summary>
        /// 此时时间 单位:s
        /// </summary>
        public float elapsedTime;

        /// <summary>
        /// 此时时间 单位:帧
        /// </summary>
        public int tick;

        /// <summary>
        /// 此时时间流速 
        /// </summary>
        public float gameTimeScale;

        /// <summary>
        /// 默认时间流速 
        /// </summary>
        public float defaultGameTimeScale;
    }

    public struct GameTimeData : IComponentData
    {
        /// <summary>
        /// 不可变时间(全局不变)
        /// </summary>
        public JiYuTime unScaledTime;

        /// <summary>
        /// 逻辑时间(技能/速度受时间流速影响)
        /// </summary>
        public JiYuTime logicTime;

        /// <summary>
        /// 刷怪时间(刷怪/障碍物受时间流速影响)
        /// </summary>
        public JiYuTime refreshTime;
    }

    public struct GameCameraSizeData : IComponentData
    {
        public float width;
        public float height;
    }

    /// <summary>
    /// 全局的地图数据 只有一份 用来地图刷新
    /// </summary>
    public struct MapRefreshData : IComponentData
    {
        public bool isMapInit;

        /// <summary>
        /// 最大左上角值
        /// </summary>
        public float3 maxPosUpleft;

        /// <summary>
        /// 最小左下角值
        /// </summary>
        public float3 minPosBottomLeft;

        /// <summary>
        /// 最大地图序号
        /// </summary>
        public int maxIndex;

        /// <summary>
        /// 最小地图序号
        /// </summary>
        public int minIndex;
    }

    /// <summary>
    /// 加到每一张地图上的数据 生成地图时add 为可改的
    /// </summary>
    public struct MapBaseData : IComponentData
    {
        /// <summary>
        /// 第几张地图
        /// </summary>
        public int mapIndex;
    }
}