using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    public sealed class UIManager : CommonObject, IUpdate
    {
        private Dictionary<string, UI> allUIs = new Dictionary<string, UI>();

        private Dictionary<int, Transform> allLayers = new Dictionary<int, Transform>();

        private Stack<string> uiStack = new Stack<string>();

        /// <summary>
        /// 独立UI并且需要高斯模糊的UI栈
        /// </summary>
        private Stack<string> uiBlurStack = new Stack<string>();

        //private const string CancelKeyCode = "Cancel";

        public Transform BlurVolume { get; private set; }
        public Transform LoadingPanel { get; private set; }

        public Transform Img_Loading { get; private set; }
        public Transform Img_TranstionFX_Tuxture { get; private set; }
        public Transform Img_TranstionFX_Shutters { get; private set; }

        protected override void Init()
        {
            Transform ui = Common.Instance.Get<Global>().UI;
            var reference = ui.Reference();
            allLayers.Add((int)UILayer.Low, reference.GetChild<Transform>("Low"));
            allLayers.Add((int)UILayer.Mid, reference.GetChild<Transform>("Mid"));
            allLayers.Add((int)UILayer.High, reference.GetChild<Transform>("High"));
            allLayers.Add((int)UILayer.Overlay, reference.GetChild<Transform>("Overlay"));
            allLayers.Add((int)UILayer.OverlayTop, reference.GetChild<Transform>("OverlayTop"));
            allLayers.Add((int)UILayer.WorldSpace, reference.GetChild<Transform>("WorldSpace"));
            BlurVolume = reference.GetChild<Transform>("BlurVolume").transform;
            //GameObject.Find("BlurVolume").transform;
            LoadingPanel = reference.GetChild<Transform>("LoadingPanel").transform;
            var loadingRef = LoadingPanel.Reference();
            Img_Loading = loadingRef.GetChild<Transform>("Img_Loading");
            Img_TranstionFX_Tuxture = loadingRef.GetChild<Transform>("Img_TranstionFX_Tuxture");
            Img_TranstionFX_Shutters = loadingRef.GetChild<Transform>("Img_TranstionFX_Shutters");
            BlurVolume?.SetViewActive(false);
            LoadingPanel?.SetViewActive(false);
            Img_Loading?.SetViewActive(false);
            Img_TranstionFX_Tuxture?.SetViewActive(false);
            Img_TranstionFX_Shutters?.SetViewActive(false);
        }

        protected override void Destroy()
        {
            Clear();
            allLayers.Clear();
        }

        public void DestroyAllSubPanel()
        {
            List<string> uiList = new List<string>();
            uiList.Add(UIType.UISubPanel_RawBackground);
            uiList.Add(UIType.UIPanel_JiyuGame);
            uiList.Add(UIType.UIPanel_Shop);
            uiList.Add(UIType.UISubPanel_Equipment);
            uiList.Add(UIType.UIPanel_Main);
            uiList.Add(UIType.UIPanel_Equipment);
            uiList.Add(UIType.UIPanel_Challege);
            uiList.Add(UIType.UIPanel_Person);

            Dictionary<string, UI> tempDic = new Dictionary<string, UI>();
            foreach (var kv in allUIs)
            {
                if (uiList.Contains(kv.Key))
                {
                    continue;
                }

                tempDic.Add(kv.Key, kv.Value);
            }

            foreach (var kv in tempDic)
            {
                kv.Value?.Dispose();
            }

            uiList.Clear();
            tempDic.Clear();
        }


        #region LoadingAndFx

        public enum LoadingType
        {
            Loading,
            TranstionFXEnter,
            TranstionFXExit,
            TranstionShattersEnter,
            TranstionShattersExit
        }

        public async UniTask EnableLoading(bool enable = true,
            LoadingType loadingType = LoadingType.Loading)
        {
            DisableAllLoading();

            switch (loadingType)
            {
                case LoadingType.Loading:
                    Img_Loading?.SetViewActive(enable);
                    LoadingPanel?.SetViewActive(enable);
                    break;
                case LoadingType.TranstionFXEnter:
                    LoadingPanel?.SetViewActive(true);
                    Img_TranstionFX_Tuxture?.SetViewActive(true);

                    await PlayTranstionFX(LoadingType.TranstionFXEnter);
                    break;
                case LoadingType.TranstionFXExit:
                    LoadingPanel?.SetViewActive(true);
                    Img_TranstionFX_Tuxture?.SetViewActive(true);

                    await PlayTranstionFX(LoadingType.TranstionFXExit);
                    break;
                case LoadingType.TranstionShattersEnter:
                    LoadingPanel?.SetViewActive(true);
                    Img_TranstionFX_Shutters?.SetViewActive(true);

                    await PlayTranstionFX(LoadingType.TranstionShattersEnter);
                    break;
                case LoadingType.TranstionShattersExit:
                    LoadingPanel?.SetViewActive(true);
                    Img_TranstionFX_Shutters?.SetViewActive(true);

                    await PlayTranstionFX(LoadingType.TranstionShattersExit);
                    break;
            }
        }

        public enum EaseType
        {
            EaseInQuad,
            EaseOutQuad,
            EaseInOut,
            EaseOut,
            EaseIn,
            Linear
        }


        private async UniTask PlayTranstionFX(LoadingType type)
        {
            const float Duration = 1f;
            var t = 0f;
            float startValue = 0;
            float endValue = 0;
            var easeType = EaseType.EaseInOut;
            Image img = Img_TranstionFX_Tuxture.GetComponent<Image>();
            switch (type)
            {
                case LoadingType.TranstionFXEnter:
                    endValue = 1;

                    break;
                case LoadingType.TranstionFXExit:
                    startValue = 1;

                    break;

                case LoadingType.TranstionShattersEnter:
                    endValue = 0.2f;
                    img = Img_TranstionFX_Shutters.GetComponent<Image>();
                    break;
                case LoadingType.TranstionShattersExit:
                    startValue = 0.2f;
                    img = Img_TranstionFX_Shutters.GetComponent<Image>();
                    break;
            }

            while (t <= 1.0f)
            {
                t += 0.02f / Duration;
                var _step = JiYuTweenHelper.EaseFromTo(startValue, endValue, t, easeType);
                img.material.SetFloat("_Step", _step);
                await UniTask.Delay(1, true);
            }

            if (type == LoadingType.TranstionFXExit)
            {
                Img_TranstionFX_Tuxture?.SetViewActive(false);
            }

            if (type == LoadingType.TranstionShattersExit)
            {
                Img_TranstionFX_Shutters?.SetViewActive(false);
            }

            //
        }

        private void DisableAllLoading()
        {
            Img_Loading?.SetViewActive(false);
            Img_TranstionFX_Tuxture?.SetViewActive(false);
            LoadingPanel?.SetViewActive(false);
        }

        #endregion

        public void PrintAll()
        {
            Log.Debug($"allUIs");
            foreach (var VARIABLE in allUIs)
            {
                Log.Debug($"{VARIABLE.Key}");
            }

            Log.Debug($"uiStack");
            foreach (var VARIABLE in uiStack)
            {
                Log.Debug($"{VARIABLE}");
            }

            Log.Debug($"uiBlurStack");
            foreach (var VARIABLE in uiBlurStack)
            {
                Log.Debug($"{VARIABLE}");
            }
        }

        private void InitUI(string uiType, UI ui, UILayer layer)
        {
            UILayer layerBlur = default;
            if (ui is null)
                return;

            ui.SetLayer(layer);
            UI child = GetTop();
            if (child != null && !child.IsDisposed)
            {
                child.OnBlur();
            }

            uiStack.Push(ui.Name);

            if (GetUIEventManager()?.Get(uiType) is IUILayer uiLayer)
            {
                layerBlur = uiLayer.Layer;
            }

            if (layerBlur == UILayer.Overlay)
            {
                UI childOverlay = GetTopOverlay();
                if (childOverlay != null && !childOverlay.IsDisposed)
                {
                    //childOverlay.SetLayer(UILayer.Mid);
                    childOverlay.GameObject?.transform?.SetParent(GetUILayer(UILayer.Mid), false);
                }

                uiBlurStack.Push(ui.Name);
            }

            BlurVolume?.SetViewActive(uiBlurStack.Count > 0);
        }

        /// <summary>
        /// 拿到顶层的overlay层级的UI
        /// </summary>
        /// <returns></returns>
        public UI GetTopOverlay()
        {
            if (uiBlurStack.Count == 0)
                return null;

            string uiType = uiBlurStack.Peek();

            return Get(uiType);
        }

        private Transform GetParentObj(AUIEvent uiEvent)
        {
            UILayer layer = UILayer.Low;
            if (uiEvent is IUILayer uiLayer)
            {
                layer = uiLayer.Layer;
            }

            return GetUILayer(layer);
        }

        /// <summary>
        /// 创建UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <param name="parentObj"></param>
        /// <param name="allowManagement">允许UIManager进行管理</param>
        /// <returns></returns>
        private UI CreateInner(string uiType, Transform parentObj, bool allowManagement)
        {
            Remove(uiType);
            var uiEvent = GetUIEventManager()?.Get(uiType);
            if (uiEvent is null)
                return null;

            if (allowManagement)
            {
                if (!uiEvent.AllowManagement)
                {
                    Log.Error($"尝试创建UIType为{uiType}的UI到UIManager，因为{uiType}是不受管理的");
                    return null;
                }

                //AudioManager.Instance.PlayFModAudio(1104);
                //TODO:
            }

            UI ui = uiEvent.OnCreate();
            if (ui is null)
                return null;

            GameObject obj = GetGameObject(ui, uiEvent, parentObj);
            ui.Bind(obj, uiType);

            if (allowManagement)
            {
                if (uiEvent.AllowManagement)
                {
                    allUIs.TryAdd(uiType, ui);
                }
            }

            return ui;
        }

        public bool TryAddAllUIs(string uiType, UI ui)
        {
            if (allUIs.TryAdd(uiType, ui))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 创建一个GameObject对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="uiEvent"></param>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        private GameObject GetGameObject(XObject parent, AUIEvent uiEvent, Transform parentObj)
        {
            if (uiEvent is null)
                return null;

            string key = uiEvent.Key;
            if (key.IsNullOrEmpty())
                return null;

            if (!parentObj)
            {
                parentObj = GetParentObj(uiEvent);
            }

            bool isFromPool = uiEvent.IsFromPool;
            GameObject obj = ResourcesManager.Instantiate(parent, key, parentObj, isFromPool);

            return obj;
        }

        /// <summary>
        /// 异步创建UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <param name="parentObj"></param>
        /// <param name="allowManagement">允许UIManager进行管理</param>
        /// <returns></returns>
        private async UniTask<UI> CreateInnerAsync(string uiType, Transform parentObj, bool allowManagement,
            CancellationToken cct = default)
        {
            Remove(uiType);
            var uiEvent = GetUIEventManager()?.Get(uiType);

            if (uiEvent is null)
                return null;

            if (allowManagement)
            {
                if (!uiEvent.AllowManagement)
                {
                    Log.Error($"尝试创建UIType为{uiType}的UI到UIManager，因为{uiType}是不受管理的");
                    return null;
                }

                //AudioManager.Instance.PlayFModAudio(1104);
                //LoadingPanel?.SetViewActive(false);
            }

            UI ui = uiEvent.OnCreate();
            if (ui is null)
                return null;

            long tagId = this.TagId;

            GameObject obj = await GetGameObjectAsync(ui, uiEvent, parentObj, cct);
            if (tagId != this.TagId)
            {
                ui?.Dispose();
                return null;
            }

            ui.Bind(obj, uiType);

            if (allowManagement)
            {
                if (uiEvent.AllowManagement)
                {
                    allUIs.TryAdd(uiType, ui);
                }
            }

            return ui;
        }


        /// <summary>
        /// 异步创建UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <param name="parentObj"></param>
        /// <param name="allowManagement">允许UIManager进行管理</param>
        /// <returns></returns>
        private async UniTask<UI> CreateInnerAsync(string uiType, string prefabKey, Transform parentObj,
            bool allowManagement,
            CancellationToken cct = default)
        {
            Remove(uiType);
            var uiEvent = GetUIEventManager()?.Get(uiType);

            if (uiEvent is null)
                return null;

            if (allowManagement)
            {
                if (!uiEvent.AllowManagement)
                {
                    Log.Error($"尝试创建UIType为{uiType}的UI到UIManager，因为{uiType}是不受管理的");
                    return null;
                }

                //AudioManager.Instance.PlayFModAudio(1104);
                //LoadingPanel?.SetViewActive(false);
            }

            UI ui = uiEvent.OnCreate();
            if (ui is null)
                return null;

            long tagId = this.TagId;

            GameObject obj = await GetGameObjectAsync(ui, uiEvent, prefabKey, parentObj, cct);
            if (tagId != this.TagId)
            {
                ui?.Dispose();
                return null;
            }

            ui.Bind(obj, uiType);

            if (allowManagement)
            {
                if (uiEvent.AllowManagement)
                {
                    allUIs.TryAdd(uiType, ui);
                }
            }

            return ui;
        }

        /// <summary>
        /// 创建一个GameObject对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="uiEvent"></param>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        private async UniTask<GameObject> GetGameObjectAsync(XObject parent, AUIEvent uiEvent, Transform parentObj,
            CancellationToken cct = default)
        {
            if (uiEvent is null)
                return null;

            string key = uiEvent.Key;
            if (key.IsNullOrEmpty())
                return null;

            if (!parentObj)
            {
                parentObj = GetParentObj(uiEvent);
            }

            bool isFromPool = uiEvent.IsFromPool;
            GameObject obj = await ResourcesManager.InstantiateAsync(parent, key, parentObj, isFromPool, cct);

            return obj;
        }

        /// <summary>
        /// 创建一个GameObject对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="uiEvent"></param>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        private async UniTask<GameObject> GetGameObjectAsync(XObject parent, AUIEvent uiEvent, string prefabKey,
            Transform parentObj,
            CancellationToken cct = default)
        {
            if (uiEvent is null)
                return null;

            //string key = uiEvent.Key;
            string key = prefabKey;
            if (key.IsNullOrEmpty())
                return null;

            if (!parentObj)
            {
                parentObj = GetParentObj(uiEvent);
            }

            bool isFromPool = uiEvent.IsFromPool;
            GameObject obj = await ResourcesManager.InstantiateAsync(parent, key, parentObj, isFromPool, cct);

            return obj;
        }

        private UIEventManager GetUIEventManager()
        {
            return Common.Instance.Get<UIEventManager>();
        }

        #region Create Sync

        public UI Create(string uiType, Transform parentObj, bool awake = true)
        {
            UI ui = CreateInner(uiType, parentObj, false);
            if (awake)
                ObjectHelper.Awake(ui);

            return ui;
        }

        public UI Create<P1>(string uiType, P1 p1, Transform parentObj)
        {
            UI ui = CreateInner(uiType, parentObj, false);
            ObjectHelper.Awake(ui, p1);

            return ui;
        }

        public UI Create(string uiType, UILayer layer)
        {
            Transform parentObj = GetUILayer(layer, UILayer.Low);
            UI ui = CreateInner(uiType, parentObj, true);

            InitUI(uiType, ui, layer);
            ObjectHelper.Awake(ui);

            return ui;
        }

        public UI Create<P1>(string uiType, P1 p1, UILayer layer)
        {
            Transform parentObj = GetUILayer(layer, UILayer.Low);
            UI ui = CreateInner(uiType, parentObj, true);

            InitUI(uiType, ui, layer);

            ObjectHelper.Awake(ui, p1);

            return ui;
        }

        public UI Create(string uiType)
        {
            UI ui = CreateInner(uiType, null, true);
            if (ui is null)
                return null;

            UILayer layer = UILayer.Low;
            if (GetUIEventManager()?.Get(uiType) is IUILayer uiLayer)
            {
                layer = uiLayer.Layer;
            }

            InitUI(uiType, ui, layer);
            ObjectHelper.Awake(ui);

            return ui;
        }

        public UI Create<P1>(string uiType, P1 p1)
        {
            UI ui = CreateInner(uiType, null, true);
            if (ui is null)
                return null;

            UILayer layer = UILayer.Low;
            if (GetUIEventManager()?.Get(uiType) is IUILayer uiLayer)
            {
                layer = uiLayer.Layer;
            }

            InitUI(uiType, ui, layer);
            ObjectHelper.Awake(ui, p1);

            return ui;
        }

        #endregion

        #region Create Async

        /// <summary>
        /// 格伦新增
        /// 新加的创建一个有父节点的独立UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <param name="parentObj"></param>
        /// <param name="awake"></param>
        /// <returns></returns>
        public async UniTask<UI> CreateAsyncNew<P1>(string uiType, P1 p1, Transform parentObj,
            CancellationToken cct = default)
        {
            UI ui = await CreateInnerAsync(uiType, parentObj, true, cct);
            if (ui is null)
                return null;

            UILayer layer = UILayer.Low;
            if (GetUIEventManager()?.Get(uiType) is IUILayer uiLayer)
            {
                layer = uiLayer.Layer;
            }

            InitUI(uiType, ui, layer);
            ObjectHelper.Awake(ui, p1);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        /// <summary>
        /// 格伦新增
        /// 新加的创建一个有父节点的独立UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <param name="parentObj"></param>
        /// <param name="awake"></param>
        /// <returns></returns>
        public async UniTask<UI> CreateAsyncNew(string uiType, Transform parentObj,
            CancellationToken cct = default)
        {
            UI ui = await CreateInnerAsync(uiType, parentObj, true, cct);
            if (ui is null)
                return null;

            UILayer layer = UILayer.Low;
            if (GetUIEventManager()?.Get(uiType) is IUILayer uiLayer)
            {
                layer = uiLayer.Layer;
            }

            InitUI(uiType, ui, layer);
            ObjectHelper.Awake(ui);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }


        public async UniTask<UI> CreateAsync(string uiType, Transform parentObj, bool awake = true,
            CancellationToken cct = default)
        {
            UI ui = await CreateInnerAsync(uiType, parentObj, false, cct);
            if (awake)
                ObjectHelper.Awake(ui);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        public async UniTask<UI> CreateAsync<P1>(string uiType, P1 p1, Transform parentObj,
            CancellationToken cct = default)
        {
            UI ui = await CreateInnerAsync(uiType, parentObj, false, cct);
            ObjectHelper.Awake(ui, p1);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        public async UniTask<UI> CreateAsync<P1, P2>(string uiType, P1 p1, P2 p2, Transform parentObj,
            CancellationToken cct = default)
        {
            UI ui = await CreateInnerAsync(uiType, parentObj, false, cct);
            ObjectHelper.Awake(ui, p1, p2);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        public async UniTask<UI> CreateAsync<P1, P2>(string uiType, string prefabKey, P1 p1, P2 p2, Transform parentObj,
            CancellationToken cct = default)
        {
            UI ui = await CreateInnerAsync(uiType, prefabKey, parentObj, false, cct);
            ObjectHelper.Awake(ui, p1, p2);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        public async UniTask<UI> CreateAsync(string uiType, UILayer layer,
            CancellationToken cct = default)
        {
            Transform parentObj = GetUILayer(layer, UILayer.Low);
            UI ui = await CreateInnerAsync(uiType, parentObj, true, cct);

            InitUI(uiType, ui, layer);
            ObjectHelper.Awake(ui);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        public async UniTask<UI> CreateAsync<P1>(string uiType, P1 p1, UILayer layer,
            CancellationToken cct = default)
        {
            Transform parentObj = GetUILayer(layer, UILayer.Low);
            UI ui = await CreateInnerAsync(uiType, parentObj, true, cct);

            InitUI(uiType, ui, layer);

            ObjectHelper.Awake(ui, p1);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        public async UniTask<UI> CreateAsync<P1, P2>(string uiType, P1 p1, P2 p2, UILayer layer,
            CancellationToken cct = default)
        {
            Transform parentObj = GetUILayer(layer, UILayer.Low);
            UI ui = await CreateInnerAsync(uiType, parentObj, true, cct);

            InitUI(uiType, ui, layer);

            ObjectHelper.Awake(ui, p1, p2);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        public async UniTask<UI> CreateAsync(string uiType,
            CancellationToken cct = default)
        {
            UI ui = await CreateInnerAsync(uiType, null, true, cct);
            if (ui is null)
                return null;

            UILayer layer = UILayer.Low;
            if (GetUIEventManager()?.Get(uiType) is IUILayer uiLayer)
            {
                layer = uiLayer.Layer;
            }

            InitUI(uiType, ui, layer);

            ObjectHelper.Awake(ui);
            //Log.Error($"Awake: {uiType}");
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        public async UniTask<UI> CreateAsync<P1>(string uiType, P1 p1,
            CancellationToken cct = default)
        {
            UI ui = await CreateInnerAsync(uiType, null, true, cct);
            if (ui is null)
                return null;

            UILayer layer = UILayer.Low;
            if (GetUIEventManager()?.Get(uiType) is IUILayer uiLayer)
            {
                layer = uiLayer.Layer;
            }

            InitUI(uiType, ui, layer);
            ObjectHelper.Awake(ui, p1);
            //LoadingPanel?.SetViewActive(false);
            return ui;
        }

        public async UniTask<UI> CreateAsync<P1, P2>(string uiType, P1 p1, P2 p2,
            CancellationToken cct = default)
        {
            UI ui = await CreateInnerAsync(uiType, null, true, cct);
            if (ui is null)
                return null;

            UILayer layer = UILayer.Low;
            if (GetUIEventManager()?.Get(uiType) is IUILayer uiLayer)
            {
                layer = uiLayer.Layer;
            }

            InitUI(uiType, ui, layer);
            ObjectHelper.Awake(ui, p1, p2);

            return ui;
        }

        #endregion

        /// <summary>
        /// 获取UI层级对象
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public Transform GetUILayer(UILayer layer, UILayer defaultLayer = UILayer.Low)
        {
            allLayers.TryGetValue((int)layer, out Transform parent);
            return parent != null ? parent : allLayers[(int)defaultLayer];
        }

        /// <summary>
        /// 移除UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public bool Remove(string uiType)
        {
            if (allUIs.TryRemove(uiType, out UI ui))
            {
                GetUIEventManager()?.OnRemove(ui);
                ui?.Dispose();

                if (uiStack.Count > 0)
                {
                    string type = uiStack.Peek();
                    if (type == uiType)
                    {
                        uiStack.Pop();
                        while (uiStack.Count > 0)
                        {
                            type = uiStack.Peek();
                            var child = Get(type);
                            if (child != null && !child.IsDisposed)
                            {
                                child.OnFocus();
                                break;
                            }
                            else
                            {
                                uiStack.Pop();
                            }
                        }
                    }
                }

                UILayer layerBlur = default;
                if (GetUIEventManager()?.Get(uiType) is IUILayer uiLayer)
                {
                    layerBlur = uiLayer.Layer;
                }

                if (layerBlur == UILayer.Overlay && uiBlurStack.Count > 0)
                {
                    string type = uiBlurStack.Peek();
                    if (type == uiType)
                    {
                        uiBlurStack.Pop();
                        while (uiBlurStack.Count > 0)
                        {
                            type = uiBlurStack.Peek();
                            var child = Get(type);
                            if (child != null && !child.IsDisposed)
                            {
                                //child.OnFocus();
                                child.GameObject?.transform?.SetParent(GetUILayer(UILayer.Overlay), false);

                                break;
                            }
                            else
                            {
                                uiBlurStack.Pop();
                            }
                        }

                        BlurVolume?.SetViewActive(uiBlurStack.Count > 0);
                    }
                }

                if (uiBlurStack.Count <= 0)
                {
                    BlurVolume?.SetViewActive(false);
                }


                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public UI Get(string uiType)
        {
            TryGet(uiType, out UI ui);
            return ui;
        }

        /// <summary>
        /// 获取顶层UI
        /// </summary>
        /// <returns></returns>
        public UI GetTop()
        {
            if (uiStack.Count == 0)
                return null;

            string uiType = uiStack.Peek();
            return Get(uiType);
        }

        /// <summary>
        /// 尝试获取一个UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <param name="ui"></param>
        /// <returns></returns>
        public bool TryGet(string uiType, out UI ui)
        {
            return allUIs.TryGetValue(uiType, out ui);
        }

        /// <summary>
        /// 是否包含UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public bool Contain(string uiType)
        {
            return allUIs.ContainsKey(uiType);
        }

        /// <summary>
        /// 清除所有的UI
        /// </summary>
        public void Clear()
        {
            using var list = XList<string>.Create();
            list.AddRange(allUIs.Keys);
            foreach (var uiType in list)
            {
                Remove(uiType);
            }

            allUIs.Clear();
            uiStack.Clear();
            uiBlurStack.Clear();
        }

        public void Update()
        {
            // if (Input.GetButtonDown(CancelKeyCode))
            // {
            //     UI child = GetTop();
            //     if (child != null && !child.IsDisposed)
            //     {
            //         child.OnCancel();
            //     }
            // }
        }
    }
}