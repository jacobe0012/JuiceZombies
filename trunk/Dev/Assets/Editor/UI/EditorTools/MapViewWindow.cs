using HotFix_UI;
using System;
using System.IO;
using Main;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using XFramework;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class MapViewWindow : EditorWindow
{
    private Texture2D mapTexture;
    private Texture2D playerViewTexture;
    string mapPath = "D:\\jiyu\\mapPreview";
    string playerViewPath = "D:\\jiyu\\playerPreview";
    string inputText = "";

    private EntityManager entityManager;
    private EntityQuery entityQuery;
    private UIManager uiManager;
    private bool isReady = false;
    private bool isMapView = false;
    private Vector2 scrollPosition = Vector2.zero;

    //[MenuItem("UnicornStudio Tools/地图预览")]
    public static void ShowWindow()
    {
        MapViewWindow window = GetWindow<MapViewWindow>("地图预览");
        window.Show();
    }

    async Task<bool> ExitScene()
    {
        uiManager = XFramework.Common.Instance.Get<UIManager>();

        if (uiManager.TryGet(UIType.UIPanel_RunTimeHUD, out var ui))
        {
            ui.GameObject.SetActive(true);
            var uiscript = ui as UIPanel_RunTimeHUD;

            uiscript.Dispose();

            var sceneController = XFramework.Common.Instance.Get<SceneController>();
            var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu, LoadSceneMode.Single);
            await SceneResManager.WaitForCompleted(sceneObj);
            return true;
        }

        return false;
    }

    async void OnInspectorGUI()
    {
        //DisablePlayer();
        inputText = EditorGUILayout.TextField("???:", inputText);

        if (GUILayout.Button("??????"))
        {
            Debug.Log("?????????????? " + inputText);
            if (ReadyToShotAsync())
            {
                isReady = true;
                var script = GameObject.Find("MainCamera")?.GetComponent<MapPreviewMaskMono>();
                script.isActive = true;
            }
        }

        if (GUILayout.Button("???????"))
        {
            Debug.Log("???????");
            if (await ExitScene())
            {
                var script = GameObject.Find("MainCamera")?.GetComponent<MapPreviewMaskMono>();
                script.isActive = false;
                script.enabled = false;
            }
        }

        //entityManager.AddComponent<Disabled>(player);
    }

    private void InitBool()
    {
        isReady = false;
        isMapView = false;
    }

    private void GetRunTimeUI()
    {
        uiManager = XFramework.Common.Instance.Get<UIManager>();

        if (uiManager.TryGet(UIType.UIPanel_RunTimeHUD, out var ui) && !isReady)
        {
            if (ui.GameObject.activeSelf)
            {
                ui.GameObject.SetActive(false);
            }
        }
    }

    void DisablePlayer()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
        if (entityQuery.IsEmpty)
        {
            return;
        }

        var player = entityQuery.ToEntityArray(Allocator.Temp)[0];
        var skills = entityManager.GetBuffer<Skill>(player);
        skills.Clear();
        // var playerHybridData = entityManager.GetComponentObject<PlayerHybridData>(player);
        // playerHybridData.player.SetActive(false);
    }

    private bool ReadyToShotAsync()
    {
        GetRunTimeUI();
        if (uiManager.TryGet(UIType.UIPanel_JiyuGame, out var ui))
        {
            SwitchScene(ui);
            //SceneResManager.WaitForCompleted(sceneObj);
            // await UniTask.Delay(1000 * 3);
            DisalbeSpawnSystem();

            AjustCamera();
            //  await UniTask.Delay(1000 * 3);
            return true;
        }

        return false;
    }

    private static void AjustCamera()
    {
        var script = GameObject.Find("MainCamera")?.GetComponent<MapPreviewMaskMono>();
        script.enabled = true;
        var camera = GameObject.Find("MainCamera")?.GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 250;
    }

    private static void DisalbeSpawnSystem()
    {
        var spawnEnemySystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SpawnEnemySystem>();
        ref var state = ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(spawnEnemySystem);
        state.Enabled = false;
    }

    private void SwitchScene(UI ui)
    {
        //?л?????
        ResourcesSingletonOld.Instance.levelInfo.levelId = int.Parse(inputText);

        var sceneController = XFramework.Common.Instance.Get<SceneController>();
        var sceneObj = sceneController.LoadSceneAsync<RunTimeScene>(SceneName.RunTime);
    }

    private async UniTask StartShotsAsync()
    {
        //string mapPath = Path.Combine(Application.dataPath, screenshotPath);
        if (!Directory.Exists(mapPath))
        {
            Directory.CreateDirectory(mapPath);
        }

        string fileName = string.Format("MapView_{0}.png", System.DateTime.Now.ToString("yy-MM-dd_HH-mm-ss"));
        string fullPath = Path.Combine(mapPath, fileName);

        await UniTask.Delay(1000 * 1);
        ScreenCapture.CaptureScreenshot(fullPath);
        LoadMap();
        isReady = false;
        // Debug.Log("Screenshot saved: " + fullPath);
    }


    void OnGUI()
    {
        OnInspectorGUI();

        if (isReady)
        {
            StartShotsAsync();
        }

        if (mapTexture == null)
        {
            GUILayout.Label("??п???????");
            return;
        }
        //if (mapTexture == null || playerViewTexture == null)
        //{
        //    GUILayout.Label("??п???????");
        //    return;
        //}

        GUILayout.Label("mapTexture:", EditorStyles.boldLabel);
        //mapTexture = (Texture2D)EditorGUILayout.ObjectField(mapTexture, typeof(Texture2D), false);

        //GUILayout.Label("Right Image:", EditorStyles.boldLabel);
        //playerViewTexture = (Texture2D)EditorGUILayout.ObjectField(playerViewTexture, typeof(Texture2D), false);
        GUILayout.Space(20);

        //// ??????????
        //GUILayout.Box(mapTexture, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        //// ??????????
        //GUILayout.Box(playerViewTexture, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width),
            GUILayout.Height(position.height));

        Rect rect = GUILayoutUtility.GetRect(position.width, position.width * 2);
        if (mapTexture != null)
        {
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, rect.height), mapTexture);
        }

        if (playerViewTexture != null)
        {
            GUI.DrawTexture(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, rect.height), playerViewTexture);
        }

        EditorGUILayout.EndScrollView();
    }


    void LoadTotalMap(string mapPath)
    {
        byte[] bytes = File.ReadAllBytes(mapPath);
        mapTexture = new Texture2D(2, 2);
        mapTexture.LoadImage(bytes);
    }

    void LodaPlayView(string mapPath)
    {
        byte[] bytes = File.ReadAllBytes(mapPath);
        playerViewTexture = new Texture2D(2, 2);
        playerViewTexture.LoadImage(bytes);
    }

    //[MenuItem("UnicornStudio Tools/??????")]
    void LoadMap()
    {
        string[] mapFiles =
            Directory.GetFiles(mapPath, "*.png");
        if (mapFiles.Length > 0)
        {
            string latestScreenshot = mapFiles[mapFiles.Length - 1];
            LoadTotalMap(latestScreenshot);
        }

        //string[] playerviewfiles =
        //    Directory.GetFiles(playerViewPath, "*.png");
        //if (playerviewfiles.Length > 0)
        //{
        //    string latestmapview = playerviewfiles[playerviewfiles.Length - 1];
        //    MapViewWindow window = GetWindow<MapViewWindow>("??????");
        //    window.LodaPlayView(latestmapview);
        //}
    }
}