using System;
using System.IO;
using cfg;
using cfg.config;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using XFramework;
using YooAsset;

namespace HotFix_UI
{
    public sealed class ConfigManager : Singleton<ConfigManager>, IDisposable
    {
        private Tables tables;

        public Tables Tables => tables;

        public void Dispose()
        {
            tables = null;
            Instance = null;
        }

        public void InitTables(Tables newTables)
        {
            tables = newTables;
        }

        public async UniTask InitTables()
        {
            var tables = new Tables();
            await tables.LoadAsync(ConfigManager.Instance.Loader);
            
            InitTables(tables);
        }

        public async UniTask<JSONNode> Loader(string name)
        {
#if UNITY_EDITOR
            var textAsset = await File.ReadAllTextAsync("Assets/ApesGang/ConfigJsonData/" + name + ".json");

            string fileText = textAsset;
            return JSON.Parse(fileText);
#else
           var package = YooAssets.GetPackage("DefaultPackage");

            AssetHandle handle =
                package.LoadAssetAsync<TextAsset>("Assets/ApesGang/ConfigJsonData/" + name + ".json");

            await handle.ToUniTask();
            var textAsset = handle.AssetObject as TextAsset;

            string fileText = textAsset.text;

            return JSON.Parse(fileText);

#endif
        }

        public void SwitchLanguages(Tblanguage.L10N l10N)
        {
            tables.Tblanguage.SwitchL10N(l10N);
        }

        public void SwitchLanguages(int l10N)
        {
            l10N = Mathf.Max(1, l10N);
            tables.Tblanguage.SwitchL10N((Tblanguage.L10N)l10N);
        }
    }
}