using Unity.Entities;

namespace Main
{
    public struct MapElementData : IComponentData
    {
        public int elementID;
        public bool isActiveWeather;
    }

    public struct AreaTag : IComponentData
    {
    }

    public struct ObstacleTag : IComponentData
    {
    }

    public struct AreaTempHp : IComponentData {
        public int maxHp;
    }

}