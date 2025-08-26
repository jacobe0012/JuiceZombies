//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_BindingItem)]
    internal sealed class UIPanel_BindingItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPanel_BindingItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_BindingItem>();
        }
    }

    public partial class UIPanel_BindingItem : UI, IAwake<int>
    {
        public int id;

        public void Initialize(int args)
        {
            // var tbPlayer_skill_binding = ConfigManager.Instance.Tables.Tbplayer_skill_binding;
            // this.GetImage(KImg_icon_binding)?.SetSprite(tbPlayer_skill_binding[bindingID].pic, false);
            // SetQueenState();
        }

        /// <summary>
        /// 是否显示流派上的王冠
        /// </summary>
        /// <param name="state"></param>
        public void SetQueenState(bool state = false)
        {
            //this.GetFromReference(KImg_queen)?.SetActive(state);
        }

        /// <summary>
        /// 设置流派进度数字
        /// </summary>
        /// <param name="num"></param>
        public void SetStagNumberAndColor(int num = 0)
        {
            //         NativeHashMap<int,int> ExpToRank = new NativeHashMap<int,int>();
            //         var TbPlayer_skill_binding_rank = ConfigManager.Instance.Tables.Tbplayer_skill_binding_rank.DataList;
            //         int maxExp = int.Parse( TbPlayer_skill_binding_rank[TbPlayer_skill_binding_rank.Count - 1].exp);
            //         int tempRank = 0;
            //         for(int i = 0;i< maxExp; i++)
            //         {
            //             if (tempRank<=TbPlayer_skill_binding_rank[i].exp)
            //             ExpToRank
            //         }
            //var str=num.ToString()+"/40";
            //this.GetTextMeshPro(KTxt_Stag)?.SetTMPText(str);
            //         int level = num / 10;
            //         var stag1 = GetFromReference(KStag1)?.GetComponent<XImage>();
            //         var stag2 = GetFromReference(KStag2)?.GetComponent<XImage>();
            //         var stag3 = GetFromReference(KStag3)?.GetComponent<XImage>();
            //         var stag4 = GetFromReference(KStag4)?.GetComponent<XImage>();
            //         var color = Color.white;
            //         switch (level)
            //         {
            //             case 0:
            //                 stag1.fillAmount = num / 10f;
            //                 stag2.fillAmount = 0;
            //                 stag3.fillAmount = 0;
            //                 stag4.fillAmount = 0;
            //                 color = UnityHelper.HexRGB2Color("#1055EA");

            //                 SetColorForStag(color);
            //                 break;
            //             case 1:
            //                 color = UnityHelper.HexRGB2Color("#A31DAF");

            //                 break;
            //             case 2:
            //                 break;
            //                 color = UnityHelper.HexRGB2Color("#F67416");
            //             case 3: break;

            //             default:
            //                 color = UnityHelper.HexRGB2Color("CA7D71");
            //                 break;
            //         }
        }

        public void SetStagBar(int num = 0)
        {
        }

        private void SetColorForStag(Color color)
        {
            for (int i = 1; i <= 4; i++)
            {
                string key = "stag" + i.ToString();
                this.GetImage(key)?.SetColor(color);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}