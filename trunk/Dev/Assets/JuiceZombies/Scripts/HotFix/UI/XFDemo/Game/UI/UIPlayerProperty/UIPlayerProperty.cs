//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using HotFix_UI;
using Main;

namespace XFramework
{
    [UIEvent(UIType.UIPlayerProperty)]
    internal sealed class UIPlayerPropertyEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPlayerProperty;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPlayerProperty>();
        }
    }

    public partial class UIPlayerProperty : UI, IAwake
    {
        public void Initialize()
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            this.GetButton(KCloseBack)?.OnClick.Add(Close);
            this.GetButton(KCloseBtn)?.OnClick.Add(Close);

            //��˶�ȡ
            var playerProperty = ResourcesSingletonOld.Instance.playerProperty;
            var battleVaribles = ConfigManager.Instance.Tables.Tbattr_variable.DataMap;
            int bvCount = battleVaribles.Count;
            //����һ�������õ�����
            int[] variblesHelp = new int[bvCount];
            variblesHelp = VariblesSorting(variblesHelp, battleVaribles, bvCount);
            int vCount = VariblesCount(variblesHelp);
            if (vCount > 20) vCount = 20;

            //����˳����ʾ����
            for (int i = 0; i < vCount; i++)
            {
                //1Ϊ���Σ�2Ϊ��ֱ�
                if (battleVaribles[variblesHelp[i]].type == 1)
                {
                    this.GetFromReference("PropertyTxt" + i.ToString()).GetTextMeshPro()
                        .SetTMPText(language[battleVaribles[variblesHelp[i]].name].current + ":" +
                                    UnityHelper.GetPlayerProperty(playerProperty.playerData,
                                        playerProperty.chaProperty, variblesHelp[i]).ToString());
                }

                if (battleVaribles[variblesHelp[i]].type == 2)
                {
                    this.GetFromReference("PropertyTxt" + i.ToString()).GetTextMeshPro()
                        .SetTMPText(language[battleVaribles[variblesHelp[i]].name].current + ":" +
                                    PercentageStr(UnityHelper.GetPlayerProperty(playerProperty.playerData,
                                        playerProperty.chaProperty, variblesHelp[i])));
                }
            }

            this.GetFromReference(KTitleText).GetTextMeshPro().SetTMPText(language["attr_title"].current);
        }

        /// <summary>
        /// ���ݶ������������Ե���ʾ˳��
        /// </summary>
        private int[] VariblesSorting(int[] variblesHelp, Dictionary<int, attr_variable> battleVaribles, int bvCount)
        {
            foreach (KeyValuePair<int, attr_variable> kvp in battleVaribles)
            {
                //�ж�����˳���Ƿ�Ϊ0
                if (kvp.Value.displayOrder != 0)
                {
                    variblesHelp[kvp.Value.displayOrder - 1] = kvp.Key;
                }
            }

            return variblesHelp;
        }

        /// <summary>
        /// �ж�Ŀǰ��ʾ��������Ҫ��ʾ��һ���м�λ
        /// </summary>
        private int VariblesCount(int[] variblesHelp)
        {
            for (int i = 0; i < variblesHelp.Length; i++)
            {
                if (variblesHelp[i] == 0)
                    return i;
            }

            return variblesHelp.Length;
        }

        /// <summary>
        /// �޸���ֱ���ֵ����ʾ�ַ���
        /// </summary>
        private string PercentageStr(int input)
        {
            string output = null;
            if (input == 0)
            {
                output = "0.00%";
            }

            if (input < 10 && input > 0)
            {
                output = "0.0" + input.ToString() + "%";
            }

            if (input < 100 && input >= 10)
            {
                output = "0." + input.ToString() + "%";
            }

            if (input < 1000 && input >= 100)
            {
                output = input.ToString()[0] + "." + input.ToString()[1] + input.ToString()[2] + "%";
            }

            if (input >= 1000)
            {
                output = input.ToString()[0] + input.ToString()[1] + "."
                         + input.ToString()[2] + input.ToString()[3] + "%";
            }

            return output;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}