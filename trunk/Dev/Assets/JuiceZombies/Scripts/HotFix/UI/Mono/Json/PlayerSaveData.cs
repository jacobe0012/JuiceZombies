using System;
using System.Collections.Generic;

namespace HotFix_UI
{
    [Serializable]
    public class AllPlayerSharedData
    {
        public long lastLoginUserId;
        public List<long> quickLoginUserIds = new List<long>();
        public MyNoticeList noticesList;

        public int l10N = 2;

     
    }

    [Serializable]
    public class PlayerSaveData
    {
        public long userId;
        public string nickName;

        /// <summary>
        /// 登录私钥
        /// </summary>
        public string privateKey;

        public int lastChapterId;
        public int chapterId;
        public int blockId;
        public int tagId = 3;
    }


    [Serializable]
    public class MyNoticeList : IDisposable
    {
        public List<NoticeMulti> notices = new List<NoticeMulti>();

        public void Dispose()
        {
            foreach (var notice in notices)
            {
                notice.Dispose();
            }

            notices.Clear();
        }
    }

    [Serializable]
    public class NoticeSingle
    {
        /// <summary>
        /// L10N  语言类型
        /// </summary>
        public int l10N;

        /**
         * 公告内容（uri）
         */
        public string link;

        /**
        * 文本参数
        */
        public string para;

        /**
         *  公告icon图片
         */
        public string icon;

        /**
         *  公告图片
         */
        public string pic;

        /**
         *  公告标题
         */
        public string title;

        /**
         *  公告内容
         */
        public string content;

        /**
         * 发件人
         */
        public string fromPerson;
    }

    [Serializable]
    public class NoticeMulti : IDisposable
    {
        /// <summary>
        /// 公告id 
        /// </summary>
        public int id;

        public List<NoticeSingle> notices = new List<NoticeSingle>();

        /// <summary>
        /// 版本号
        /// </summary>
        public int version;

        /**
                * 有效期（天数）
                */
        public int? valid;

        /**
         *  公告状态 0 失效 1有效
         */
        public int? noticeStatus;

        /**
         *  阅读状态 0 未读 1已读
         */
        public int? readStatus;

        /**
         * 初始化时间
         */
        public long? initTime;

        public void Dispose()
        {
            notices.Clear();
        }
    }
}