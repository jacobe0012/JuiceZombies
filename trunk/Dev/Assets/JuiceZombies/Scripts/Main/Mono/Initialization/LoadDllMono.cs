//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-12 12:15:10
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

// using VoxelBusters.CoreLibrary;
// using VoxelBusters.EssentialKit;

namespace Main
{
    //cdn资源加载初始化&&热更初始化脚本，放到AOT层
    public class LoadDllMono : MonoBehaviour
    {
        // 资源系统运行模式
        private EPlayMode PlayMode = EPlayMode.HostPlayMode;

        private bool localTest = true;

        //public static bool isStandAlone = false;

        //MyCDN
        //CDN地址
        private string DefaultHostServer = "https://gleen-test-0012.oss-cn-beijing.aliyuncs.com/Dev/Android";

        //
        private string FallbackHostServer = "https://gleen-test-0012.oss-cn-beijing.aliyuncs.com/Dev/Android";


        private string LocalServer = "http://192.168.2.112/Dev";


        //弹窗对象,此对象当前为AOT层中的预制体对象，不放入热更新
        private GameObject InitCover;
        public static Version Verson;

        //热更新dll的列表，Yooasset中不需要带后缀
        public static List<string> HotDllNames { get; } = new List<string>()
        {
            "ApesGang.HotFix_Logic.dll",
            "ApesGang.HotFix_UI.dll"
        };

        public static List<System.Reflection.Assembly> AssList { get; } = new List<System.Reflection.Assembly>();

        //补充元数据dll的列表，Yooasset中不需要带后缀
        // public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
        // {
        //     "mscorlib.dll",
        //     "System.dll",
        //     "System.Core.dll",
        //     "UnityEngine.JSONSerializeModule.dll",
        //     "UniTask.dll",
        //     "Unity.Entities.dll",
        //     "Unity.Collections.dll",
        //     "Unity.Transforms.dll",
        //     "Unity.Mathematics.dll",
        //     "Unity.Burst.dll"
        //};


        //获取资源二进制
        private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

        public static byte[] GetAssetData(string dllName)
        {
            return s_assetDatas[dllName];
        }

        public void AOTGenericMethod()
        {
            var renderMeshArray = Unity.Properties.TypeUtility.Instantiate<Unity.Rendering.RenderMeshArray>();
        }

        async void Start()
        {
            Init();
            await DownloadAssetsAndStartGame();
        }

        void Init()
        {
#if UNITY_EDITOR
            PlayMode = EPlayMode.EditorSimulateMode;
#else
            PlayMode = EPlayMode.HostPlayMode;
#endif
            //PlayMode = EPlayMode.HostPlayMode;
        }

        async UniTask DownloadAssetsAndStartGame()
        {
            Debug.Log($"[{GetType().FullName}] StartLoadDll.cs!");
            InitHostServerURL();
            //初始化BetterStreamingAssets插件
            BetterStreamingAssets.Initialize();
            await DownLoadAssetsByYooAssets();
        }

        private void InitHostServerURL()
        {
            FallbackHostServer = DefaultHostServer;


#if UNITY_STANDALONE
            if (localTest)
            {
                DefaultHostServer = $"{LocalServer}/Win64";
                FallbackHostServer = $"{LocalServer}/Win64";
            }
            else
            {
                DefaultHostServer = "https://gleen-test-0012.oss-cn-beijing.aliyuncs.com/Dev/StandaloneWin64";
                FallbackHostServer = "https://gleen-test-0012.oss-cn-beijing.aliyuncs.com/Dev/StandaloneWin64";
            }

#elif UNITY_ANDROID
            if (localTest)
            {
                DefaultHostServer = $"{LocalServer}/Android";
                FallbackHostServer = $"{LocalServer}/Android";
            }
            else
            {
                DefaultHostServer = "https://gleen-test-0012.oss-cn-beijing.aliyuncs.com/Dev/Android";
                FallbackHostServer = "https://gleen-test-0012.oss-cn-beijing.aliyuncs.com/Dev/Android";
            }

#elif UNITY_IOS
            if (localTest)
            {
                DefaultHostServer = $"{LocalServer}/IOS";
                FallbackHostServer = $"{LocalServer}/IOS";
            }
            else
            {
                DefaultHostServer = "https://gleen-test-0012.oss-cn-beijing.aliyuncs.com/Dev/IOS";
                FallbackHostServer = "https://gleen-test-0012.oss-cn-beijing.aliyuncs.com/Dev/IOS";
            }
#endif
        }


        #region Yooasset下载

        /// <summary>
        /// 获取下载信息
        /// </summary>
        /// <param name="onDownloadComplete"></param>
        /// <returns></returns>
        async UniTask DownLoadAssetsByYooAssets()
        {
            InitCover = UnityEngine.GameObject.Find("InitCover");
            // 1.初始化资源系统
            if (!YooAssets.Initialized)
            {
                YooAssets.Initialize();
            }

            ResourcePackage package = YooAssets.TryGetPackage("DefaultPackage");
            bool containsPackage = true;
            if (!YooAssets.ContainsPackage("DefaultPackage"))
            {
                // 创建默认的资源包
                package = YooAssets.CreatePackage("DefaultPackage");
                // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
                YooAssets.SetDefaultPackage(package);
                containsPackage = false;
            }

            if (package.InitializeStatus == EOperationStatus.None)
            {
                if (PlayMode == EPlayMode.EditorSimulateMode)
                {
                    //编辑器模拟模式
                    var initParameters = new EditorSimulateModeParameters();
                    initParameters.SimulateManifestFilePath =
                        EditorSimulateModeHelper.SimulateBuild("BuiltinBuildPipeline",
                            "DefaultPackage");
                    //package.InitializeStatus == EOperationStatus.Succeed
                    await package.InitializeAsync(initParameters).ToUniTask();
                }
                else if (PlayMode == EPlayMode.HostPlayMode)
                {
                    var initParameters = new HostPlayModeParameters();
                    initParameters.BuildinQueryServices =
                        new GameQueryServices(); //太空战机DEMO的脚本类，详细见StreamingAssetsHelper
                    // initParameters.DeliveryQueryServices = new DeliveryQueryServices();
                    //initParameters.DecryptionServices = new GameDecryptionServices();
                    initParameters.RemoteServices = new RemoteServices(DefaultHostServer, FallbackHostServer);


                    // var initParameters = new HostPlayModeParameters();
                    // initParameters.BuildinQueryServices = new GameQueryServices(); //太空战机DEMO的脚本类，详细见StreamingAssetsHelper
                    // initParameters.DecryptionServices = new GameDecryptionServices();
                    // initParameters.RemoteServices = new RemoteServices(DefaultHostServer, FallbackHostServer);

                    await package.InitializeAsync(initParameters).ToUniTask();
                }
                else if (PlayMode == EPlayMode.OfflinePlayMode)
                {
                    //单机模式
                    var initParameters = new OfflinePlayModeParameters();


                    await package.InitializeAsync(initParameters).ToUniTask();
                }
            }

            //2.获取资源版本

            string oldVersion = "";

            //Debug.LogError($"{YooAssets.ContainsPackage()}");
            if (containsPackage)
            {
                oldVersion = package.GetPackageVersion();
            }

            var operation = package.UpdatePackageVersionAsync();

            await operation.ToUniTask();

            if (operation.Status != EOperationStatus.Succeed)
            {
                //TODO:更新失败
                //更新失败、显示弹窗窗口
#if !UNITY_EDITOR
                Application.Quit();
#else
                EditorApplication.isPlaying = false;
#endif

                Debug.LogError(operation.Error);
                return;
            }

            string packageVersion = operation.PackageVersion;
            Debug.Log($"cnd版本号:{packageVersion}");
            if (PlayMode == EPlayMode.HostPlayMode)
            {
                string dirUrl = $"{DefaultHostServer}/Version/Version.json";
                bool isForceUpdate = await TryForceUpdateAndLoadJSONFromUrl(dirUrl, oldVersion);

                if (isForceUpdate)
                    return;
            }


            //3.更新补丁清单
            var operation2 = package.UpdatePackageManifestAsync(packageVersion);
            await operation2.ToUniTask();
            if (operation2.Status != EOperationStatus.Succeed)
            {
                //更新失败
                Debug.LogError(operation2.Error);
                //TODO:
                return;
            }

            //4.下载补丁包信息，反馈到弹窗
            Download().Forget();
        }

        public static Transform FindGrandchild(Transform parent, string grandchildName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == grandchildName)
                {
                    return child;
                }

                Transform result = FindGrandchild(child, grandchildName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static void ShowProgressUI(GameObject InitCover, float ratios, string titleStr)
        {
            var Text_Progress = FindGrandchild(InitCover.transform, "Text_Progress");
            var Img_Filled = FindGrandchild(InitCover.transform, "Img_Filled");
            var Text_FilledRatios = FindGrandchild(InitCover.transform, "Text_FilledRatios");

            Img_Filled.GetComponent<Image>().DOFillAmount(ratios, 0.2f);
            var str = $"{(ratios * 100).ToString("F1")}%";
            Text_FilledRatios.GetComponent<TextMeshProUGUI>().text = str;

            Text_Progress.GetComponent<TextMeshProUGUI>().text = titleStr;
        }

        /// <summary>
        /// 获取下载的信息大小，显示弹窗上
        /// </summary>
        /// <returns></returns>
        async UniTask Download()
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            int timeout = 60;
            var package = YooAssets.GetPackage("DefaultPackage");
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain, timeout);
            //没有需要下载的资源
            if (downloader.TotalDownloadCount == 0)
            {
                Debug.Log($"没有资源更新，直接进入游戏加载环节");
                ShowProgressUI(InitCover, 1, "正在下载资源...");
                await GotoStart();
                return;
            }

            //需要下载的文件总数和总大小
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;
            Debug.Log($"文件总数:{totalDownloadCount}:::总大小:{totalDownloadBytes}");
            GetDownload();
            //显示更新提示UI界面 
            // tx = Instantiate(Resources.Load<GameObject>("ShowMsgBox"));
            // // tx.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text =
            // //     $"文件总数:{totalDownloadCount},总大小:{totalDownloadBytes}KB";
            // tx.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
            //     "检测到版本更新,点击进入appStore";
            // tx.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick
            //     .AddListener(() => GetDownload());
        }

        /// <summary>
        /// 按键回调下载
        /// </summary>
        /// <returns></returns>
        async UniTask GetDownload()
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            int timeout = 60;
            var package = YooAssets.GetPackage("DefaultPackage");
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain, timeout);
            //注册回调方法
            downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
            downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
            downloader.OnDownloadOverCallback = OnDownloadOverFunction;
            downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;
            //开启下载
            downloader.BeginDownload();
            await downloader.ToUniTask();
            //检测下载结果
            if (downloader.Status == EOperationStatus.Succeed)
            {
                //下载成功
                Debug.Log($"更新完成!");
                //DestroyImmediate(tx);
                await GotoStart();
            }
            else
            {
                //下载失败
                Debug.LogError($"更新失败！");
                //DestroyImmediate(tx);
                return;
                //TODO:
            }
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sizeBytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnStartDownloadFileFunction(string fileName, long sizeBytes)
        {
            Debug.Log(string.Format("开始下载：文件名：{0}, 文件大小：{1}", fileName, sizeBytes));
        }

        /// <summary>
        /// 下载完成
        /// </summary>
        /// <param name="isSucceed"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnDownloadOverFunction(bool isSucceed)
        {
            Debug.Log("下载" + (isSucceed ? "成功" : "失败"));
        }

        /// <summary>
        /// 更新中
        /// </summary>
        /// <param name="totalDownloadCount"></param>
        /// <param name="currentDownloadCount"></param>
        /// <param name="totalDownloadBytes"></param>
        /// <param name="currentDownloadBytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount,
            long totalDownloadBytes, long currentDownloadBytes)
        {
            ShowProgressUI(InitCover, currentDownloadBytes / (float)totalDownloadBytes, "正在下载资源...");

            //TODO:进度条
            // tx.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = string.Format(
            //     "文件总数：{0}, 已下载文件数：{1}, 下载总大小：{2}, 已下载大小：{3}", totalDownloadCount, currentDownloadCount,
            //     totalDownloadBytes,
            //     currentDownloadBytes);
            Debug.Log(string.Format("文件总数：{0}, 已下载文件数：{1}, 下载总大小：{2}, 已下载大小：{3}", totalDownloadCount,
                currentDownloadCount,
                totalDownloadBytes, currentDownloadBytes));
        }

        /// <summary>
        /// 下载出错
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="error"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnDownloadErrorFunction(string fileName, string error)
        {
            Debug.LogError(string.Format("下载出错：文件名：{0}, 错误信息：{1}", fileName, error));
        }

        /// <summary>
        /// 完成下载验证开始进入游戏
        /// </summary>
        /// <returns></returns>
        async UniTask GotoStart()
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            //热更新Dll名称
            //var Allassets = HotDllNames.Concat(AOTMetaAssemblyNames);

            foreach (var asset in HotDllNames)
            {
                // RawFileHandle handle =
                //     package.LoadRawFileAsync("Assets/ApesGang/HotFixDlls/" + asset + ".bytes");
                AssetHandle handle =
                    package.LoadAssetAsync<TextAsset>("Assets/ApesGang/HotFixDlls/" + asset + ".bytes");

                await handle.ToUniTask();
                var textAsset = handle.AssetObject as TextAsset;

                byte[] fileData = textAsset.bytes;

                s_assetDatas[asset] = fileData;
                Debug.Log($"[{GetType().FullName}] dll:{asset} size:{fileData.Length}");
            }

            //DestroyImmediate(tx);
            await StartGame();
        }

        // 内置文件查询服务类
        // private class QueryStreamingAssetsFileServices : IQueryServices
        // {
        //     public bool QueryStreamingAssets(string fileName)
        //     {
        //         // 注意：使用了BetterStreamingAssets插件，使用前需要初始化该插件！
        //         string buildinFolderName = YooAssets.GetStreamingAssetBuildinFolderName();
        //         return BetterStreamingAssets.FileExists($"{buildinFolderName}/{fileName}");
        //     }
        // }

        #endregion

        async UniTask StartGame()
        {
            //LoadMetadataForAOTAssemblies();
            AOTGenericMethod();
#if !UNITY_EDITOR
           foreach (var HotDll in HotDllNames)
            {
                AssList.Add(System.Reflection.Assembly.Load(GetAssetData(HotDll)));
            }
#endif
            //委托加载方式，加载prefab
            var EventSystem = UnityEngine.GameObject.Find("EventSystem");
            EventSystem?.SetActive(false);

            var package = YooAssets.GetPackage("DefaultPackage");
            AssetHandle handle =
                package.LoadAssetAsync<GameObject>(
                    "Assets/ApesGang/Scripts/HotFix/HotFixPrefab/HotUpdatePrefab.prefab");
            await handle.ToUniTask();

            GameObject go = handle.AssetObject as GameObject;
            Instantiate(go);
            Debug.Log($"[{GetType().FullName}] Prefab name is {go.name}");
            Debug.Log($"[{GetType().FullName}] FinishLoadDll.cs!");
        }

        void OpenURL(string url)
        {
            Debug.Log($"强更跳转");

            var Btn_ForceUpdate = FindGrandchild(InitCover.transform, "Btn_ForceUpdate");
            var Text_ForceUpdate = FindGrandchild(InitCover.transform, "Text_ForceUpdate");

            Btn_ForceUpdate.gameObject.SetActive(true);

            Text_ForceUpdate.GetComponent<TextMeshProUGUI>().text = "检测到版本更新,点击进入appStore";
            Btn_ForceUpdate.GetComponent<Button>().onClick
                .AddListener(() =>
                {
#if UNITY_EDITOR
                    Application.OpenURL(url);
#elif UNITY_ANDROID
                    Application.Quit();
                    // WebView webview = WebView.CreateInstance();
                    // webview.Frame = new Rect(0f, 0f, Screen.width, Screen.height);
                    // webview.AutoShowOnLoadFinish = true;
                    // webview.Style = WebViewStyle.Popup;
                    // webview.LoadURL(URLString.URLWithPath("https://www.baidu.com"));
#endif
                });
        }

        async UniTask<bool> TryForceUpdateAndLoadJSONFromUrl(string url, string oldVersion)
        {
            using (WWW www = new WWW(url))
            {
                await UniTask.WaitUntil(() => www.isDone);

                if (string.IsNullOrEmpty(www.error))
                {
                    string jsonText = www.text;
                    Debug.Log("JSON data: " + jsonText);

                    // 解析 JSON
                    Verson =
                        JsonConvert.DeserializeObject<Version>(jsonText);
                    if (oldVersion == "")
                    {
                        oldVersion = Verson.majorVerson;
                    }

                    if (CompareVersion(Verson.majorVerson, oldVersion))
                    {
                        OpenURL(Verson.forceUpdateURL);
                        return true;
                        //Application.Quit();
                    }

                    // if (jsonData.ContainsKey(fieldName))
                    // {
                    //     Debug.Log("Field value: " + jsonData[fieldName]);
                    // }
                    // else
                    // {
                    //     Debug.Log("Field not found: " + fieldName);
                    // }
                }
                else
                {
                    //TODO：超时处理
                    Debug.Log("Error loading JSON: " + www.error);
                }
            }

            return false;
        }

        bool CompareVersion(string version1, string version2)
        {
            string[] versionParts1 = version1.Split('.');
            string[] versionParts2 = version2.Split('.');

            if (versionParts1.Length < 2 || versionParts2.Length < 2)
            {
                Debug.LogError($"版本号格式错误 ");
                return false; // 格式错误，无法比较
            }

            var versionParts11 = int.Parse(versionParts1[0]);
            var versionParts21 = int.Parse(versionParts2[0]);

            var versionParts12 = int.Parse(versionParts1[1]);
            var versionParts22 = int.Parse(versionParts2[1]);

            return versionParts11 != versionParts21 || versionParts12 != versionParts22;
        }
    }

    //版本号文件
    public struct Version
    {
        /// <summary>
        /// 主包版本号
        /// </summary>
        public string majorVerson;

        /// <summary>
        /// 资源版本号
        /// </summary>
        public string subVerson;

        /// <summary>
        /// 强更的跳转链接
        /// </summary>
        public string forceUpdateURL;


        public bool isLock;
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }

    /// <summary>
    /// 资源文件解密服务类
    /// </summary>
// class GameDecryptionServices : IDecryptionServices
// {
//     public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
//     {
//         return 32;
//     }
//
//     public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Stream LoadFromStream(DecryptFileInfo fileInfo)
//     {
//         BundleStream bundleStream =
//             new BundleStream(fileInfo.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
//         return bundleStream;
//     }
//
//     public uint GetManagedReadBufferSize()
//     {
//         return 1024;
//     }
// }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
// private static void LoadMetadataForAOTAssemblies()
// {
//     /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
//     /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
//     HomologousImageMode mode = HomologousImageMode.SuperSet;
//     foreach (var aotDllName in AOTMetaAssemblyNames)
//     {
//         byte[] dllBytes = GetAssetData(aotDllName);
//         // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
//         LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
//         Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
//     }
// }
}