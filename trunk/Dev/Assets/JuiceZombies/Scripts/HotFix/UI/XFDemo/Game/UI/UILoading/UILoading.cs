using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UILoading)]
    internal sealed class UILoadingEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UILoading;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.High;

        public override UI OnCreate()
        {
            return UI.Create<UILoading>(true);
        }
    }

    public partial class UILoading : UI, IAwake<ILoading>, IWaitObject
    {
        private ILoading m_loading;

        /// <summary>
        /// �������ص�������
        /// </summary>
        private const int SceneMaxProgress = 10;

        /// <summary>
        /// �������ص�ǰ����
        /// </summary>
        private float sceneProgress;

        /// <summary>
        /// ��ǰ���ص���Դ��
        /// </summary>
        private int curCount;

        /// <summary>
        /// ��ǰ���ص��ܸ���
        /// </summary>
        private int totalCount;

        /// <summary>
        /// ��һ�θ��µĽ���
        /// </summary>
        private float beforeProgress;

        /// <summary>
        /// Ҫʵ������Ԥ�Ƶ�key
        /// </summary>
        private List<string> objKeys = new List<string>();

        private long timerId;

        private MiniTween tween;

        Dictionary<System.Type, object> IWaitObject.WaitDict { get; set; }

        public void Initialize(ILoading loadArg)
        {
            var KText_Progress = GetFromReference(UILoading.KText_Progress);
            var KImg_Filled = GetFromReference(UILoading.KImg_Filled);
            var KText_FilledRatios = GetFromReference(UILoading.KText_FilledRatios);

            this.m_loading = loadArg;
            loadArg.GetObjects(objKeys);
            this.totalCount = objKeys.Count + SceneMaxProgress;

            KImg_Filled.GetImage().SetFillAmount(0);
            KText_FilledRatios.GetTextMeshPro().SetTMPText(string.Empty);
            KText_Progress.GetTextMeshPro().SetTMPText("�������볡��...");
            //����һ��ÿִ֡�е������൱��Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.RepeatedFrameTimer(this.Update);

            //��ʼ������Դ
            this.LoadAssets().ToCoroutine();
        }

        private void Update()
        {
            float progress = this.m_loading.SceneProgress();
            this.sceneProgress = progress * SceneMaxProgress;
            this.DoFillAmount().ToCoroutine();

            if (this.sceneProgress >= SceneMaxProgress)
            {
                this.RemoveTimer();
            }
        }

        /// <summary>
        /// ����������Դ
        /// </summary>
        /// <returns></returns>
        private async UniTask LoadAssets()
        {
            //Log.Debug($"LoadAssets111");
            using var tasks = XList<UniTask>.Create();
            var timerMgr = TimerManager.Instance;
            var tagId = this.TagId;

            Transform parent = Common.Instance.Get<Global>().GameRoot;
            foreach (var key in this.objKeys)
            {
                tasks.Add(this.LoadObjectAsync(key, parent));
            }

            //�ȴ�������Դ�������

            await UniTask.WhenAll(tasks);
            //��Ϊ���첽�������������첽ʱ����౻�ͷ��ˣ�����Ҫ��tagId�ж�һ��
            //�����������Զ���أ���ôtagIdÿ��ȡ��������仯
            //������tagId�ж�������Ƿ���Ч�����׵ķ�ʽ
            if (tagId != this.TagId)
                return;

            //�ȴ������Ľ�����
            while (this.sceneProgress < SceneMaxProgress)
            {
                await timerMgr.WaitFrameAsync();
                if (tagId != this.TagId)
                    return;
            }

            if (this.tween != null)
            {
                //�ȴ��������������
                if (!this.tween.IsCompelted)
                {
                    await this.tween.Task;
                    if (tagId != this.TagId)
                        return;
                }
            }

            //�ӳ�50����
            await timerMgr.WaitAsync(50);
            if (tagId != this.TagId)
                return;

            //��Դ������ϣ��ر�Loading
            this.Close();
        }

        /// <summary>
        /// ʵ����GameObject
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private async UniTask LoadObjectAsync(string key, Transform parent)
        {
            //Log.Debug($"LoadAssets222 {key}");
            var tagId = this.TagId;
            GameObject obj = await ResourcesManager.InstantiateAsync(this, key, parent, true);
            ResourcesManager.ReleaseInstance(obj);
            if (tagId != this.TagId)
                return;

            ++curCount;
            this.DoFillAmount().ToCoroutine();
        }

        /// <summary>
        /// ˿���仯������
        /// </summary>
        /// <returns></returns>
        private async UniTask DoFillAmount()
        {
            var KText_Progress = GetFromReference(UILoading.KText_Progress);
            var KImg_Filled = GetFromReference(UILoading.KImg_Filled);
            var KText_FilledRatios = GetFromReference(UILoading.KText_FilledRatios);


            float count = this.curCount + this.sceneProgress;
            if (count == this.beforeProgress)
                return;
            this.beforeProgress = count;

            float t = this.beforeProgress / this.totalCount;
            this.tween?.Cancel(this);
            var image = KImg_Filled.GetImage();
            var txt = KText_FilledRatios.GetTextMeshPro();
            var tweenMgr = Common.Instance.Get<MiniTweenManager>();
            var miniTween = tweenMgr.To(this, image.GetFillAmount(), t, 10f, MiniTweenMode.Speed);
            miniTween.AddListener(v =>
            {
                image.SetFillAmount(v);
                txt.SetTextWithKey("{0:F0}%", v * 100);
            });

            this.tween = miniTween;
            await this.tween.Task;
        }

        /// <summary>
        /// �Ƴ���ʱ��
        /// </summary>
        private void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }

        protected override void OnClose()
        {
            this.m_loading = null;
            this.RemoveTimer();
            this.curCount = 0;
            this.totalCount = 0;
            this.beforeProgress = 0;
            this.sceneProgress = 0;
            this.objKeys.Clear();
            this.tween = null;
        }
    }
}