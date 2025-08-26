//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;
using TMPro;

namespace XFramework
{
    [UIEvent(UIType.UILockNewChapter)]
    internal sealed class UILockNewChapterEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UILockNewChapter;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;


        public UILayer Layer => UILayer.High;

        public override UI OnCreate()
        {
            return UI.Create<UILockNewChapter>();
        }
    }

    public partial class UILockNewChapter : UI, IAwake<int>
    {
        //private string savePath = "Assets/Resources/levelJson.json";

        public int currentID;

        public void Initialize(int levelID)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            currentID = levelID;
            this.GetFromReference(KCloseText).GetComponent<TMP_Text>().SetText(language["text_window_close"].current);
            this.GetFromReference(KTextInfo).GetComponent<TMP_Text>().SetText(language["text_level_unlock"].current);
            LevelInfoShow();
            this.GetButton(KCloseBack)?.OnClick.Add(Close);
            UpdateJsonData(levelID + 1);
        }

        private void UpdateJsonData(int popID)
        {
            JsonManager.Instance.userData.chapterId = popID;
            JsonManager.Instance.SavePlayerData(JsonManager.Instance.userData);

            // JSONNode jsonNode = new JSONObject();
            // jsonNode["PopLevelID"] = popID.ToString();
            // File.WriteAllText(savePath, jsonNode.ToString());
        }

        private void LevelInfoShow()
        {
            InitLevelNameText(currentID);
            SetLevelImage(currentID);
        }


        private void InitLevelNameText(int levelID)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var chapterTable = ConfigManager.Instance.Tables.Tbchapter.DataMap;
            int chapterID = levelID - 10000;

            this.GetTextMeshPro(KTextInfo).SetTMPText(language.Get("text_chapter_unlock").current);
            this.GetTextMeshPro(KCloseText).SetTMPText(language.Get("text_window_close").current);

            var levelNameText = this.GetFromReference(KChapterTitleText).GetComponent<TMP_Text>();
            var levelDescription = this.GetFromReference(KDescritptText).GetComponent<TMP_Text>();
            int levelNum = 0;
            string levelName = "";
            if (chapterTable.ContainsKey(chapterID))
            {
                levelNum = chapterTable[chapterID].num;
                levelName = chapterTable[chapterID].name;
            }


            levelNameText.SetText(levelNum.ToString() + "." + language[levelName].current);
            levelDescription.SetText(language[chapterTable[chapterID].desc].current);
        }

        private void SetLevelImage(int levelID)
        {
            int chapterID = levelID - 10000;
            var chapterTable = ConfigManager.Instance.Tables.Tbchapter.DataMap;

            //this.GetImage(KChapterImage).SetSprite(chapterTable[chapterID].iconUnlock, false);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}