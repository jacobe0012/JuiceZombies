using System.Collections.Generic;

namespace XFramework
{
    public interface ILoading
    {
        /// <summary>
        /// 要预先加载的场景对象
        /// </summary>
        /// <param name="objKeys"></param>
        void GetObjects(ICollection<string> objKeys);

        /// <summary>
        /// 场景加载进度
        /// </summary>
        /// <returns></returns>
        float SceneProgress();
    }
}