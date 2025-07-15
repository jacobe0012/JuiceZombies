using Common;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;
using XFramework;

namespace SuperScrollView
{
    [DefaultExecutionOrder(200)]
    public class SpecialGridViewFeatureDemoScript2 : MonoBehaviour
    {
        //public List<GameEquip> gameEquips1=new List<GameEquip>();


        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 200;
        private LoopListViewItem2 item2;

        public RectTransform ViewContent;

        //初始Content高度
        private float initHeight = -1;
        DataSourceMgr<ItemData> mDataSourceMgr;

        public static int mItemCountPerRow = 5; // how many items in one row

        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mAddButton;
        InputField mAddInput;
        Button mBackButton;

        int[] mFeatureArray = { 0 };
        string[] mFeaturePrefabs = { " " };


        // Use this for initialization
        void Start()
        {
            WebMessageHandler.Instance.RemoveHandler(9, 5, OnInitEquipmentResponse);


            WebMessageHandler.Instance.AddHandler(9, 5, OnInitEquipmentResponse);
            NetWorkManager.Instance.SendMessage(9, 5);
        }

        //[DefaultExecutionOrder(200)]
        private void Update()
        {
            if (ViewContent.sizeDelta.y != initHeight)
            {
                Vector2 temp2 = ViewContent.sizeDelta;
                //print(ViewContent.sizeDelta + "ViewContent.sizeDelta");
                if (item2 != null)
                {
                    item2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -temp2.y);
                    temp2 = new Vector2(temp2.x, temp2.y + item2.GetComponent<RectTransform>().rect.height + 300);
                }
                else
                {
                    //后续要调整
                    temp2 = new Vector2(temp2.x, temp2.y + 100);
                }

                ViewContent.sizeDelta = temp2;
                ViewContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewContent.sizeDelta.y);
                //c=false;
                initHeight = ViewContent.sizeDelta.y;
            }

            return;
        }


        public int InitView(int Count)
        {
            //GameObject ScrollViewObject = GameObject.Find("EquipScrollView(Clone)");
            //int count=ScrollViewObject.GetComponent<RectTransform>().GetChild(0).GetChild(1).childCount;
            //Debug.Log(count + "XJX123");
            //if (count != 0)
            //{
            //    for (int i = 0; i <count; i++)
            //    {
            //        DestroyImmediate(ScrollViewObject.GetComponent<RectTransform>().GetChild(0).GetChild(1).GetChild(0).gameObject);

            //    }
            //}


            //Count = 0;
            mDataSourceMgr = new DataSourceMgr<ItemData>(Count);
            int featureCount = GetFeatureItemCount(); //特殊数量
            //这里进行排序

            //计算行数
            int row = (mDataSourceMgr.TotalItemCount - featureCount) / mItemCountPerRow;
            if ((mDataSourceMgr.TotalItemCount - featureCount) % mItemCountPerRow > 0)
            {
                row++;
            }

            //count is the total row count          
            mLoopListView.InitListView(row + mFeatureArray.Length, OnGetItemByIndex);

            return row + mFeatureArray.Length;
        }


        /*when a row is getting show in the scrollrect viewport,
        this method will be called with the row’ rowIndex as a parameter,
        to let you create the row  and update its content.

        SuperScrollView uses single items with subitems that make up the columns in the row.
        so in fact, the GridView is ListView.
        if one row is make up with 3 subitems, then the GridView looks like:

            row0:  subitem0 subitem1 subitem2
            row1:  subitem3 subitem4 subitem5
            row2:  subitem6 subitem7 subitem8
            row3:  subitem9 subitem10 subitem11
            ...
        */

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex < 0)
            {
                return null;
            }

            LoopListViewItem2 item = null;
            if (rowIndex < mFeatureArray.Length)
            {
                //item = NewFeatureItems(listView, rowIndex);
            }
            else
            {
                item = NewMainItems(listView, rowIndex);
                //print(rowIndex + "LATER");
            }

            ;
            return item;
        }

        LoopListViewItem2 NewFeatureItems(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex >= mFeatureArray.Length)
            {
                return null;
            }

            LoopListViewItem2 item = listView.NewListViewItem(mFeaturePrefabs[rowIndex]);
            //BaseHorizontalItemList itemScript = item.GetComponent<BaseHorizontalItemList>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                //itemScript.Init();
            }

            int initItemIndex = GetInitItemIndex(rowIndex);
            for (int i = 0; i < mFeatureArray[rowIndex]; ++i)
            {
                //print(i+"i是多少？");
                //print(mFeatureArray[rowIndex] + " mFeatureArray[rowIndex]是多少？");
                //print(rowIndex);

                int itemIndex = i + initItemIndex;
                if (itemIndex >= mDataSourceMgr.TotalItemCount)
                {
                    //itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }

                ItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemIndex);
                //update the subitem content.
                if (itemData != null)
                {
                    //mItemList[i].gameObject.SetActive(true);
                    //itemScript.mItemList[i].SetItemData(itemData, itemIndex);
                }
                else
                {
                    //itemScript.mItemList[i].gameObject.SetActive(false);
                }
            }

            return item;
        }

        LoopListViewItem2 NewMainItems(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex < mFeatureArray.Length)
            {
                return null;
            }

            int initItemIndex = GetInitItemIndex(rowIndex);
            //print(initItemIndex + "initItemIndex");
            //if (initItemIndex == mTotalDataCount) { item = listView.NewListViewItem("DrawingText"); }
            //LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab3");

            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            BaseHorizontalItemList itemScript = item.GetComponent<BaseHorizontalItemList>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init(EquipitemCache.GameEquips);
                //Debug.Log(gameEquips1.Count);
            }

            int initRowIndex = GetInitRowIndex(rowIndex);
            //这个是对最后一行做处理
            for (int i = 0; i < mItemCountPerRow; ++i)
            {
                int itemIndex = initRowIndex * mItemCountPerRow + i + initItemIndex;
                //print(ConfigManager.Instance.Tables.Tbequip_data.DataList.Count);


                //print(itemIndex + "itemIndex");
                if (itemIndex >= mDataSourceMgr.TotalItemCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }

                ItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemIndex);
                //update the subitem content.
                if (itemData != null)
                {
                    itemScript.mItemList[i].gameObject.SetActive(true);
                    itemScript.mItemList[i].SetItemData(itemData, itemIndex);
                }
                else
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                }
            }

            return item;
        }

        int GetInitItemIndex(int rowIndex)
        {
            if (rowIndex == 0)
            {
                return 0;
            }

            int initItemIndex = 0;
            int maxLength = mFeatureArray.Length;
            if (rowIndex < mFeatureArray.Length)
            {
                maxLength = rowIndex;
            }

            for (int i = 0; i < maxLength; i++)
            {
                initItemIndex += mFeatureArray[i];
            }

            return initItemIndex;
        }

        int GetInitRowIndex(int rowIndex)
        {
            int initRowIndex = 0;
            if (rowIndex < mFeatureArray.Length)
            {
                initRowIndex = rowIndex;
            }
            else
            {
                initRowIndex = rowIndex - mFeatureArray.Length;
            }

            return initRowIndex;
        }

        int GetFeatureItemCount()
        {
            int tmpCount = 0;
            for (int i = 0; i < mFeatureArray.Length; i++)
            {
                tmpCount += mFeatureArray[i];
            }

            return tmpCount;
        }


        public void OnInitEquipmentResponse(object sender, WebMessageHandler.Execute e)
        {
            EquipitemCache.GameEquips.Clear();
            ByteValueList gameEquips = new ByteValueList();
            gameEquips.MergeFrom(e.data.ToByteArray());
            mTotalDataCount = gameEquips.Values.Count;
            BaseHorizontalItem.equipDataIndex = 0;
            for (int i = 0; i < gameEquips.Values.Count; i++)
            {
                GameEquip equip1 = new GameEquip();
                equip1.MergeFrom(gameEquips.Values[i]);

                EquipitemCache.GameEquips.Add(equip1);
            }


            //对缓存排序,让通用材料不显示
            EquipitemCache.GameEquips.Sort((obj1, obj2) =>
            {
                if (obj1.EquipId < obj2.EquipId)
                    return 1;
                else if (obj1.EquipId > obj2.EquipId)
                    return -1;
                else
                    return 0;
            });


            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", UnityEngine.Color.red);

                return;
            }


            int first = InitView(mTotalDataCount);
            //int TestNum = Random.Range(0, 3);
            //if (TestNum >= 0)
            //{
            //    //需要图纸等信息就开启
            //    item2 = mLoopListView.NewListViewItem("DrawingText");
            //    //这里的220指的是每一行的宽度
            //    item2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -770 + (-220 * (first - 1)));
            //}


            //Vector2 temp2 = ViewContent.sizeDelta;
            //if (item2 != null)
            //{
            //    temp2 = new Vector2(temp2.x, temp2.y + item2.GetComponent<RectTransform>().rect.height + 300);
            //}
            //else
            //{
            //    //后续要调整
            //    temp2 = new Vector2(temp2.x, temp2.y + 300);
            //}
            //ViewContent.sizeDelta = temp2;
            //ViewContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewContent.sizeDelta.y);
            //initHeight = ViewContent.sizeDelta.y;
        }
    }
}