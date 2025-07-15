using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace XFramework
{
    /// <summary>
    /// 如果需要在加载场景时显示进度条，则继承这个类
    /// <para>进度条逻辑在UILoading.cs</para>
    /// </summary>
    public abstract class LoadingScene : Scene, ILoading
    {
        private struct OnResourcesLoaded : IWaitType
        {
            public int Error { get; set; }
        }

        public virtual void GetObjects(ICollection<string> objKeys)
        {
        }

        public float SceneProgress()
        {
            return SceneResManager.Progress(this.sceneObject);
        }

        protected sealed override async UniTask WaitForCompleted()
        {
            //return;
            var tag = this.TagId;
            var ui = await UIHelper.CreateAsync<ILoading>(UIType.UILoading, this); // 打开UILoading
            var ret = await ((IWaitObject)ui).Wait<OnResourcesLoaded>(); // 显示进度条并等待资源加载完毕
            if (tag != this.TagId)
                return;

            // 如果 ret.Error == WaitTypeError.Destroy 则说明UILoading被释放掉了，默认当场景资源加载完毕就会释放掉UILoading
            if (ret.Error == WaitTypeError.Destroy)
            {
                this.isCompleted = true;
                this.OnCompleted();
            }
        }
    }
}