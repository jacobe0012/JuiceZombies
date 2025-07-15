using System;
using System.Collections.Generic;
using System.IO;
using cfg.config;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using XFramework;

namespace HotFix_UI
{
    public sealed class JsonManager : Singleton<JsonManager>, IDisposable
    {
        /// <summary>
        /// 共享数据
        /// </summary>
        public AllPlayerSharedData sharedData = new AllPlayerSharedData();

        /// <summary>
        /// 单个用户数据
        /// </summary>
        public PlayerSaveData userData = new PlayerSaveData();

        private string sharedDataName = "Shared";

#if UNITY_EDITOR
        public string userDataSavePath = "Assets/Resources/UserData";
        public string sharedDataSavePath = "Assets/Resources/SharedData";
#else
        public string userDataSavePath = $"{Application.persistentDataPath}/UserData";
        public string sharedDataSavePath = $"{Application.persistentDataPath}/SharedData";
#endif
        public string noticeUrl = "http://192.168.2.112/Notice";
        public string shareUrl = "http://192.168.2.112/Share";

        public void Init()
        {
            //生成假的公告json
            // var noticeList = new MyNoticeList();
            //
            // for (int i = 0; i < 5; i++)
            // {
            //     var enmuStr = Enum.GetValues(typeof(Tblanguage.L10N));
            //     var id = 100 + i;
            //     var multi = new NoticeMulti
            //     {
            //         id = id,
            //         notices = new List<NoticeSingle>(enmuStr.Length),
            //         version = 900 + i,
            //         valid = 2,
            //         noticeStatus = 1,
            //         readStatus = 0,
            //         initTime = 66
            //     };
            //
            //     for (int j = 1; j < enmuStr.Length + 1; j++)
            //     {
            //         multi.notices.Add(new NoticeSingle
            //         {
            //             l10N = j,
            //             link = $"link{id} ",
            //             para = $"para{id}",
            //             icon = $"http://192.168.187.1/Notice/notice_icon{id}.png",
            //             pic = $"http://192.168.187.1/Notice/notice_pic{id}.png",
            //             title = $"title{id}",
            //             content = $"content{id}",
            //             fromPerson = $"fromPerson{id}",
            //         });
            //     }
            //
            //     noticeList.notices.Add(multi);
            // }
            //
            // var json = JsonConvert.SerializeObject(noticeList);
            // File.WriteAllTextAsync(sharedDataSavePath + "/Notice.json", json);
        }

        /// <summary>
        /// 保存玩家存档数据
        /// </summary>
        /// <param name="data">玩家存档数据</param>
        /// <param name="userId">userID</param>
        public async UniTask SavePlayerData(PlayerSaveData data)
        {
            if (data == null)
            {
                Debug.LogError($"data is null");
                return;
            }

            userData = data;

            string folderPath = userDataSavePath; // 指定存档文件夹的路径
            string filePath = Path.Combine(folderPath, userData.userId + ".json"); // 使用玩家账号名拼接文件名
            string jsonData = JsonConvert.SerializeObject(data); // 将数据转换为JSON格式的字符串
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 将JSON字符串写入文件
            await File.WriteAllTextAsync(filePath, jsonData).AsUniTask();
        }

        /// <summary>
        /// 根据userID读取玩家存档数据
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>玩家存档数据</returns>
        public async UniTask<PlayerSaveData> LoadPlayerData(long userId = default, string privateKey = "",
            string nickName = "")
        {
            if (userId == default)
            {
                userId = userData.userId;
            }

            if (nickName == "")
            {
                nickName = userData.nickName;
            }

            if (privateKey == "")
            {
                privateKey = userData.privateKey;
            }

            string folderPath = userDataSavePath; // 指定存档文件夹的路径
            string filePath = Path.Combine(folderPath, userId + ".json"); // 使用玩家账号名拼接文件名
            //PlayerSaveData data = new PlayerSaveData();
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (File.Exists(filePath))
            {
                string jsonData = await File.ReadAllTextAsync(filePath).AsUniTask();
                userData = JsonConvert.DeserializeObject<PlayerSaveData>(jsonData);
            }
            else
            {
                userData.tagId = 2;
                userData.userId = userId;
                userData.chapterId = 0;
                userData.blockId = 1;
                userData.lastChapterId = 0;
                userData.nickName = nickName;
                userData.privateKey = privateKey;
                string json = JsonConvert.SerializeObject(userData);
                await File.WriteAllTextAsync(filePath, json).AsUniTask();
            }


            if (sharedData.lastLoginUserId != userId)
            {
                string sharedDatafilePath = Path.Combine(sharedDataSavePath, sharedDataName + ".json"); // 使用玩家账号名拼接文件名

                sharedData.lastLoginUserId = userId;
                if (!sharedData.quickLoginUserIds.Contains(userId))
                {
                    sharedData.quickLoginUserIds.Add(userId);
                }

                string sharedDataJson = JsonConvert.SerializeObject(sharedData);
                await File.WriteAllTextAsync(sharedDatafilePath, sharedDataJson).AsUniTask();
            }


            return userData;
        }

        /// <summary>
        /// 根据userID读取玩家存档数据
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>玩家存档数据</returns>
        public async UniTask<PlayerSaveData> LoadPlayerData(long userId)
        {
            string folderPath = userDataSavePath; // 指定存档文件夹的路径
            string filePath = Path.Combine(folderPath, userId + ".json"); // 使用玩家账号名拼接文件名
            //PlayerSaveData data = new PlayerSaveData();
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!File.Exists(filePath))
            {
                Log.Error($"userId:{userId} filePath:{filePath} is not exist!");
                return null;
            }

            string jsonData = await File.ReadAllTextAsync(filePath).AsUniTask();
            userData = JsonConvert.DeserializeObject<PlayerSaveData>(jsonData);


            return userData;
        }

        /// <summary>
        /// 读取共享存档数据
        /// </summary>
        /// <returns>AllPlayerSharedData</returns>
        public async UniTask<AllPlayerSharedData> LoadSharedData()
        {
            string folderPath = sharedDataSavePath; // 指定存档文件夹的路径
            //string noticePath = sharedDataSavePath + "/Notice"; // 指定存档文件夹的路径
            string filePath = Path.Combine(folderPath, sharedDataName + ".json"); // 使用玩家账号名拼接文件名
            //string noticefilePath = Path.Combine(noticePath, "Notice" + ".json"); // 使用玩家账号名拼接文件名

            //PlayerSaveData data = new PlayerSaveData();
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            if (!File.Exists(filePath))
            {
                sharedData.lastLoginUserId = 0;
                sharedData.l10N = 2;
                sharedData.quickLoginUserIds = new List<long>();
                string json = JsonConvert.SerializeObject(sharedData);
                await File.WriteAllTextAsync(filePath, json).AsUniTask();
            }
            else
            {
                string jsonData = await File.ReadAllTextAsync(filePath).AsUniTask();
                sharedData = JsonConvert.DeserializeObject<AllPlayerSharedData>(jsonData);
            }

            return sharedData;
        }


        /// <summary>
        /// 保存玩家存档数据
        /// </summary>
        /// <param name="data">玩家存档数据</param>
        /// <param name="userId">userID</param>
        public async UniTask SaveSharedData(AllPlayerSharedData data)
        {
            if (data == null)
            {
                Debug.LogError($"data is null");
                return;
            }

            sharedData = data;

            string sharedDatafilePath = Path.Combine(sharedDataSavePath, sharedDataName + ".json"); // 使用玩家账号名拼接文件名

            string sharedDataJson = JsonConvert.SerializeObject(sharedData);
            await File.WriteAllTextAsync(sharedDatafilePath, sharedDataJson).AsUniTask();
        }

        public void Dispose()
        {
            sharedData?.quickLoginUserIds?.Clear();
            sharedData = null;
            sharedData?.noticesList?.notices?.Clear();
            userData = null;
        }
    }
}