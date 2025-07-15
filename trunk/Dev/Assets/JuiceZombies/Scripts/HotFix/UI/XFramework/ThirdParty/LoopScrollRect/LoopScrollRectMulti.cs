using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public abstract class LoopScrollRectMulti : LoopScrollRectBase
    {
        public UnityEvent<Vector2> m_OnDrag = new UnityEvent<Vector2>();
        public UnityEvent m_OnEndDrag = new UnityEvent();
        public UnityEvent m_OnBeginDrag = new UnityEvent();

        public override void OnEndDrag(PointerEventData eventData)
        {
            m_OnEndDrag.Invoke();
            base.OnEndDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_OnBeginDrag.Invoke();
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            m_OnDrag.Invoke(eventData.delta);
            base.OnDrag(eventData);
        }

        [HideInInspector] [NonSerialized] public LoopScrollMultiDataSource dataSource = null;

        protected override void ProvideData(Transform transform, int index)
        {
            dataSource.ProvideData(transform, index);
        }

        // Multi Data Source cannot support TempPool
        protected override RectTransform GetFromTempPool(int itemIdx)
        {
            RectTransform nextItem = prefabSource.GetObject(itemIdx).transform as RectTransform;
            nextItem.transform.SetParent(m_Content, false);
            nextItem.gameObject.SetActive(true);

            ProvideData(nextItem, itemIdx);
            return nextItem;
        }

        protected override void ReturnToTempPool(bool fromStart, int count)
        {
            Debug.Assert(m_Content.childCount >= count);
            if (fromStart)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    prefabSource.ReturnObject(m_Content.GetChild(i));
                }
            }
            else
            {
                int t = m_Content.childCount - count;
                for (int i = m_Content.childCount - 1; i >= t; i--)
                {
                    prefabSource.ReturnObject(m_Content.GetChild(i));
                }
            }
        }

        protected override void ClearTempPool()
        {
        }
    }
}