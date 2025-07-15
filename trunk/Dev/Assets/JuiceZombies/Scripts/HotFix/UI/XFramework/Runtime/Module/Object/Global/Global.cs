using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    public sealed class Global : CommonObject
    {
        public bool isStandAlone;
        public bool isIntroGuide;
        public Transform GameRoot { get; private set; }

        public Transform UI { get; private set; }

        public Camera UICamera { get; private set; }
        public Camera BlurCamera { get; private set; }

        public Camera MainCamera { get; private set; }

        public CinemachineCamera VirtualCamera { get; private set; }
        public Transform CameraBounds { get; private set; }

        public GlobalGameObjects GameObjects = new GlobalGameObjects();

        protected override void Init()
        {
            GameRoot = GameObject.Find("/GameRoot(Clone)").transform;
            //Log.Debug($"{GameRoot.name}");
            UI = GameRoot.Find("UI");
            UICamera = UI.Find("UICamera")?.GetComponent<Camera>();
            BlurCamera = UI.Find("BlurCamera")?.GetComponent<Camera>();
            MainCamera = GameRoot.Find("MainCamera")?.GetComponent<Camera>();
            VirtualCamera = GameRoot.Find("Virtual Camera")?.GetComponent<CinemachineCamera>();
            CameraBounds = GameRoot.Find("CameraBounds")?.GetComponent<Transform>();


            GameObjects.Cover = UnityEngine.GameObject.Find("Cover");
            GameObjects.InitBackGround = UnityEngine.GameObject.Find("InitBackGround");
            GameObjects.Reporter = UnityEngine.GameObject.Find("Reporter");
            GameObjects.RollBackground = UnityEngine.GameObject.Find("RollBackground");
            GameObjects.Cover?.SetViewActive(true);
            GameObjects.InitBackGround.transform.Find("EventSystem")?.gameObject?.SetActive(false);
            //GameObjects.InitBackGround?.SetActive(false);
            GameObjects.Reporter?.SetActive(true);
            GameObjects.BagPos = new Vector3(Screen.width / 2f, 0, 0);
            SetResolution();
            InitUnitySetting();
        }

        public async UniTaskVoid CameraShake(float duration = 0.25f, float amplitude = 10f, float frequency = 5f)
        {
            var shake = VirtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (shake != null)
            {
                shake.ReSeed();
                shake.AmplitudeGain = amplitude;
                shake.FrequencyGain = frequency;
                await UniTask.Delay((int)(duration * 1000f));
                shake.AmplitudeGain = 0;
                shake.FrequencyGain = 0;
            }
        }

        public async UniTaskVoid DoCameraFOV(float fov, bool animate = false, float duration = 2f)
        {
            if (animate)
            {
                //const float MaxDuration = 2f;
                const float Internal = 0.02f;
                var offset = fov - VirtualCamera.Lens.OrthographicSize;

                var num = duration / Internal;
                while (UnityEngine.Mathf.Abs(fov - VirtualCamera.Lens.OrthographicSize) > 0.01f)
                {
                    VirtualCamera.Lens.OrthographicSize += offset / num;
                    RefreshCameraBounds();
                    await UniTask.Delay((int)(Internal * 1000));
                }
            }

            VirtualCamera.Lens.OrthographicSize = fov;
        }

        public async UniTaskVoid DoCameraPos(Vector2 pos, bool animate = false, float duration = 2f)
        {
            if (animate)
            {
                // const float MaxDuration = 2f;
                // const float Internal = 0.02f;
                //var offset = math.length(pos) - math.length(VirtualCamera.transform.localPosition);
                var z = VirtualCamera.transform.localPosition.z;
                Log.Debug($"pos:{pos}");
                VirtualCamera.transform.DOLocalMove(new Vector3(pos.x, pos.y, z), duration);
                // var num = MaxDuration / Internal;
                // while (UnityEngine.Mathf.Abs(fov - VirtualCamera.Lens.OrthographicSize) > 0.01f)
                // {
                //     VirtualCamera.Lens.OrthographicSize += offset / num;
                //     RefreshCameraBounds();
                //     await UniTask.Delay((int)(Internal * 1000));
                // }
            }
        }

        public async UniTaskVoid DoCameraPosOffset(Vector2 pos, bool animate = false, float duration = 2f)
        {
            if (animate)
            {
                // const float MaxDuration = 2f;
                // const float Internal = 0.02f;
                //var offset = math.length(pos) - math.length(VirtualCamera.transform.localPosition);
                var z = VirtualCamera.transform.localPosition.z;
                Log.Debug($"pos:{pos}");
                var endValue = new Vector3(VirtualCamera.transform.localPosition.x + pos.x,
                    VirtualCamera.transform.localPosition.y + pos.y, z);
                VirtualCamera.transform.DOLocalMove(endValue, duration);
                // var num = MaxDuration / Internal;
                // while (UnityEngine.Mathf.Abs(fov - VirtualCamera.Lens.OrthographicSize) > 0.01f)
                // {
                //     VirtualCamera.Lens.OrthographicSize += offset / num;
                //     RefreshCameraBounds();
                //     await UniTask.Delay((int)(Internal * 1000));
                // }
            }
        }
        // public async UniTaskVoid DoCameraFOV(float fov, bool animate = false)
        // {
        //     if (animate)
        //     {
        //         const float MaxDuration = 2f;
        //         const float Internal = 0.02f;
        //         var offset = fov - VirtualCamera.m_Lens.FieldOfView;
        //
        //         var num = MaxDuration / Internal;
        //         while (UnityEngine.Mathf.Abs(fov - VirtualCamera.m_Lens.FieldOfView) > 0.01f)
        //         {
        //             VirtualCamera.m_Lens.FieldOfView += offset / num;
        //             RefreshCameraBounds();
        //             await UniTask.Delay((int)(Internal * 1000));
        //         }
        //     }
        //
        //
        //     VirtualCamera.m_Lens.FieldOfView = fov;
        // }

        private void RefreshCameraBounds()
        {
            var cinemachineConfiner2D = VirtualCamera.GetComponent<CinemachineConfiner2D>();
            cinemachineConfiner2D.BoundingShape2D = CameraBounds.gameObject.GetComponent<Collider2D>();
            cinemachineConfiner2D.InvalidateCache();
            //float cameraAspectRatio = Screen.height / (float)Screen.width;
            //cinemachineConfiner2D.   ValidateCache(cameraAspectRatio);
        }

        public void InitCameraBounds(Vector3 pos, int mapType)
        {
            var cinemachineConfiner2D = VirtualCamera.GetComponent<CinemachineConfiner2D>();
            cinemachineConfiner2D.BoundingShape2D = CameraBounds.gameObject.GetComponent<Collider2D>();
            // CinemachineFramingTransposer transposer =
            //     VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            Vector3 bounds = default;

            //1-左右封闭
            //2-上下封闭
            //3-全封闭
            //4-全开放
            cinemachineConfiner2D.enabled = true;
            switch (mapType)
            {
                case 1:
                    bounds = new Vector3(97, 1000, 1);
                    //transposer.m_TrackedObjectOffset = new Vector3(0, 0, -200);
                    pos.x = 0;
                    pos.z = 0;
                    break;
                case 2:
                    //TODO:
                    bounds = new Vector3(1000, 97, 1);
                    //transposer.m_TrackedObjectOffset = new Vector3(0, 0, -200);
                    pos.y = 0;
                    pos.z = 0;
                    break;
                case 3:

                    bounds = new Vector3(155, 170, 1);
                    //transposer.m_TrackedObjectOffset = new Vector3(0, 0, -200);
                    pos = new Vector3(0, -10, 0);
                    break;
                case 4:

                    //bounds = new Vector3(float.MaxValue, float.MaxValue, 1);
                    //transposer.m_TrackedObjectOffset = new Vector3(0, -50, -200);
                    //cinemachineConfiner2D = null;
                    cinemachineConfiner2D.enabled = false;
                    break;
            }

            CameraBounds.SetPosition(pos);
            CameraBounds.SetScale(bounds);

            RefreshCameraBounds();
        }


        public void SetCameraBounds(Vector3 bounds, Vector3 pos, float fov = 60)
        {
            VirtualCamera.Lens.FieldOfView = fov;
            var cinemachineConfiner2D = VirtualCamera.GetComponent<CinemachineConfiner2D>();
            cinemachineConfiner2D.BoundingShape2D = CameraBounds.gameObject.GetComponent<Collider2D>();
            // CinemachineFramingTransposer transposer =
            //     VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            //
            // transposer.m_TrackedObjectOffset = new Vector3(0, 0, -200);

            CameraBounds.SetPosition(pos);
            CameraBounds.SetScale(bounds);
            RefreshCameraBounds();
        }

        public void SetCameraTarget(Transform tran)
        {
            VirtualCamera.Follow = tran;
        }

        public async void SetCameraTarget(Entity tran)
        {
            var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = EntityManager.CreateEntityQuery(typeof(GameOthersData));
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var gameOther = EntityManager.GetComponentData<GameOthersData>(player);
            gameOther.CameraTarget = tran;
            EntityManager.SetComponentData(player, gameOther);
            await UniTask.DelayFrame(2);
            VirtualCamera.Follow = GameObjects.TargetCamera.transform;
            //VirtualCamera.Follow = tran;
        }

        public void SetCameraPos(float2 pos)
        {
            VirtualCamera.Follow = null;
            var temp = VirtualCamera.transform.localPosition;
            temp.x = pos.x;
            temp.y = pos.y;
            VirtualCamera.transform.localPosition = temp;
        }

        public void SetResolution()
        {
            const float defualtResolutionX = 1170;
            const float defualtResolutionY = 2532;
            var componentsInChildren = UI.gameObject.GetComponentsInChildren<CanvasScaler>();
#if UNITY_STANDALONE
            foreach (var canvasScaler in componentsInChildren)
            {
                var referenceResolution = canvasScaler.referenceResolution;
                referenceResolution.x = defualtResolutionX;
                referenceResolution.y = defualtResolutionY;
                canvasScaler.referenceResolution = referenceResolution;
            }
#else
            foreach (var canvasScaler in componentsInChildren)
            {
                var referenceResolution = canvasScaler.referenceResolution;
                referenceResolution.x = Screen.width;
                referenceResolution.y = Screen.height;
                canvasScaler.referenceResolution = referenceResolution;
            }
#endif
        }

        void InitUnitySetting()
        {
            Input.multiTouchEnabled = false;
            Application.runInBackground = false;
#if UNITY_EDITOR
            Application.runInBackground = true;
#endif
            //QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
#if UNITY_ANDROID || UNITY_IOS
            int maxRefreshRate = Screen.currentResolution.refreshRate;
            Application.targetFrameRate = maxRefreshRate;
#endif
        }

        protected override void Destroy()
        {
            GameRoot = null;
            UI = null;
            UICamera = null;
            GameObjects.Dispose();
            GameObjects = null;
        }

        public class GlobalGameObjects : IDisposable
        {
            public GameObject Cover;
            public GameObject InitBackGround;
            public GameObject Reporter;
            public GameObject RollBackground;
            public GameObject TargetCamera;
            public Vector3 BagPos;

            public void Dispose()
            {
                GameObject.Destroy(Cover);
                GameObject.Destroy(InitBackGround);
                GameObject.Destroy(Reporter);
                GameObject.Destroy(RollBackground);
                GameObject.Destroy(TargetCamera);
                // InitBackGround = null;
                // Reporter = null;
                // RollBackground = null;
            }
        }
    }
}