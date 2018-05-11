using System;
using UnityEngine;
using UnityEngine.EventSystems;

    public class UIEventScript : UIComponent, IPointerDownHandler, IEventSystemHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerUpHandler
    {
        public delegate void OnUIEventHandler(UIEvent uiEvent);

        private const float c_holdTimeValue = 1f;

        private const float c_clickAreaValue = 40f;

        [HideInInspector]
        public enUIEventID m_onDownEventID;

        [NonSerialized]
        public stUIEventParams m_onDownEventParams;

        [HideInInspector]
        public enUIEventID m_onUpEventID;

        [NonSerialized]
        public stUIEventParams m_onUpEventParams;

       // [HideInInspector]
        public enUIEventID m_onClickEventID;

        [NonSerialized]
        public stUIEventParams m_onClickEventParams;

        [HideInInspector]
        public enUIEventID m_onHoldStartEventID;

        [NonSerialized]
        public stUIEventParams m_onHoldStartEventParams;

        [HideInInspector]
        public enUIEventID m_onHoldEventID;

        [NonSerialized]
        public stUIEventParams m_onHoldEventParams;

        [HideInInspector]
        public enUIEventID m_onHoldEndEventID;

        [NonSerialized]
        public stUIEventParams m_onHoldEndEventParams;

        [HideInInspector]
        public enUIEventID m_onDragStartEventID;

        [NonSerialized]
        public stUIEventParams m_onDragStartEventParams;

        [HideInInspector]
        public enUIEventID m_onDragEventID;

        [NonSerialized]
        public stUIEventParams m_onDragEventParams;

        [HideInInspector]
        public enUIEventID m_onDragEndEventID;

        [NonSerialized]
        public stUIEventParams m_onDragEndEventParams;

        [HideInInspector]
        public enUIEventID m_onDropEventID;

        [NonSerialized]
        public stUIEventParams m_onDropEventParams;

        [HideInInspector]
        public bool m_closeFormWhenClicked;

        public string[] m_onDownWwiseEvents = new string[0];

        public string[] m_onClickedWwiseEvents = new string[0];

        private bool m_isDown;

        private bool m_isHold;

        private bool m_canClick;

        private float m_downTimestamp;

        private Vector2 m_downPosition;

        private PointerEventData m_holdPointerEventData;

        private bool m_needClearInputStatus;

        public UIEventScript.OnUIEventHandler onClick;

        public override void Initialize(UIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }
            base.Initialize(formScript);
        }

        protected override void OnDestroy()
        {
            m_holdPointerEventData = null;
            onClick = null;
            base.OnDestroy();
        }

        public override void Close()
        {
            ExecuteClearInputStatus();
        }

        public void SetUIEvent(enUIEventType eventType, enUIEventID eventID)
        {
            switch (eventType)
            {
                case enUIEventType.Down:
                {
                    m_onDownEventID = eventID;
                    break;
                }
                case enUIEventType.Click:
                {
                    m_onClickEventID = eventID;
                    break;
                }
                case enUIEventType.HoldStart:
                {
                    m_onHoldStartEventID = eventID;
                    break;
                }
                case enUIEventType.Hold:
                {
                    m_onHoldEventID = eventID;
                    break;
                }
                case enUIEventType.HoldEnd:
                {
                    m_onHoldEndEventID = eventID;
                    break;
                }
                case enUIEventType.DragStart:
                {
                    m_onDragStartEventID = eventID;
                    break;
                }
                case enUIEventType.Drag:
                {
                    m_onDragEventID = eventID;
                    break;
                }
                case enUIEventType.DragEnd:
                {
                    m_onDragEndEventID = eventID;
                    break;
                }
                case enUIEventType.Drop:
                {
                    m_onDropEventID = eventID;
                    break;
                }
                case enUIEventType.Up:
                {
                    m_onUpEventID = eventID;
                    break;
                }
            }
        }

        public void  SetUIEvent(enUIEventType eventType, enUIEventID eventID, stUIEventParams eventParams)
        {
            switch (eventType)
            {
                case enUIEventType.Down:
                {
                    m_onDownEventID = eventID;
                    m_onDownEventParams = eventParams;
                    break;
                }
                case enUIEventType.Click:
                {
                    m_onClickEventID = eventID;
                    m_onClickEventParams = eventParams;
                    break;
                }
                case enUIEventType.HoldStart:
                {
                    m_onHoldStartEventID = eventID;
                    m_onHoldStartEventParams = eventParams;
                    break;
                }
                case enUIEventType.Hold:
                {
                    m_onHoldEventID = eventID;
                    m_onHoldEventParams = eventParams;
                    break;
                }
                case enUIEventType.HoldEnd:
                {
                    m_onHoldEndEventID = eventID;
                    m_onHoldEndEventParams = eventParams;
                    break;
                }
                case enUIEventType.DragStart:
                {
                    m_onDragStartEventID = eventID;
                    m_onDragStartEventParams = eventParams;
                    break;
                }
                case enUIEventType.Drag:
                {
                    m_onDragEventID = eventID;
                    m_onDragEventParams = eventParams;
                    break;
                }
                case enUIEventType.DragEnd:
                {
                    m_onDragEndEventID = eventID;
                    m_onDragEndEventParams = eventParams;
                    break;
                }
                case enUIEventType.Drop:
                {
                    m_onDropEventID = eventID;
                    m_onDropEventParams = eventParams;
                    break;
                }
                case enUIEventType.Up:
                {
                    m_onUpEventID = eventID;
                    m_onUpEventParams = eventParams;
                    break;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_isDown = true;
            m_isHold = false;
            m_canClick = true;
            m_downTimestamp = Time.realtimeSinceStartup;
            m_downPosition = eventData.position;
            m_holdPointerEventData = eventData;
            m_needClearInputStatus = false;
            DispatchUIEvent(enUIEventType.Down, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_isHold && m_holdPointerEventData != null)
            {
                DispatchUIEvent(enUIEventType.HoldEnd, m_holdPointerEventData);
            }
            DispatchUIEvent(enUIEventType.Up, eventData);
            ClearInputStatus();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bool flag = true;
            if (belongedFormScript != null && !belongedFormScript.enableMultiClickedEvent)
            {
                flag = (belongedFormScript.clickedEventDispatchedCounter == 0);
                belongedFormScript.clickedEventDispatchedCounter++;
            }
            if (m_canClick && flag)
            {
                if (belongedListScript != null && indexInList >= 0)
                {
                    belongedListScript.SelectElement(indexInList, true);
                }
                DispatchUIEvent(enUIEventType.Click, eventData);
                if (m_closeFormWhenClicked && belongedFormScript != null)
                {
                    belongedFormScript.Close();
                }
            }
            ClearInputStatus();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (m_canClick && belongedFormScript != null && belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.position, m_downPosition)) > 40f)
            {
                m_canClick = false;
            }
            DispatchUIEvent(enUIEventType.DragStart, eventData);
            if (belongedListScript != null && belongedListScript.scrollRect != null)
            {
                belongedListScript.scrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_canClick && belongedFormScript != null && belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.position, m_downPosition)) > 40f)
            {
                m_canClick = false;
            }
            DispatchUIEvent(enUIEventType.Drag, eventData);
            if (belongedListScript != null && belongedListScript.scrollRect != null)
            {
                belongedListScript.scrollRect.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (m_canClick && belongedFormScript != null && belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.position, m_downPosition)) > 40f)
            {
                m_canClick = false;
            }
            DispatchUIEvent(enUIEventType.DragEnd, eventData);
            if (belongedListScript != null && belongedListScript.scrollRect != null)
            {
                belongedListScript.scrollRect.OnEndDrag(eventData);
            }
            ClearInputStatus();
        }

        public void OnDrop(PointerEventData eventData)
        {
            DispatchUIEvent(enUIEventType.Drop, eventData);
        }

        public bool ClearInputStatus()
        {
            m_needClearInputStatus = true;
            return m_isDown;
        }

        public void ExecuteClearInputStatus()
        {
            m_isDown = false;
            m_isHold = false;
            m_canClick = false;
            m_downTimestamp = 0f;
            m_downPosition = Vector2.zero;
            m_holdPointerEventData = null;
        }

        private void Update()
        {
            if (m_needClearInputStatus)
            {
                return;
            }
            if (m_isDown)
            {
                if (!m_isHold)
                {
                    if (Time.realtimeSinceStartup - m_downTimestamp >= 1f)
                    {
                        m_isHold = true;
                        m_canClick = false;
                        DispatchUIEvent(enUIEventType.HoldStart,m_holdPointerEventData);
                    }
                }
                else
                {
                    DispatchUIEvent(enUIEventType.Hold, m_holdPointerEventData);
                }
            }
        }

        private void LateUpdate()
        {
            if (m_needClearInputStatus)
            {
                ExecuteClearInputStatus();
                m_needClearInputStatus = false;
            }
        }

        private void DispatchUIEvent(enUIEventType eventType, PointerEventData pointerEventData)
        {
            UIEvent uiEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
            switch (eventType)
            {
                case enUIEventType.Down:
                    if (m_onDownEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uiEvent.eventID = m_onDownEventID;
                    uiEvent.eventParams = m_onDownEventParams;
                    break;
                case enUIEventType.Click:
                    PostWwiseEvent(m_onDownWwiseEvents);
                    PostWwiseEvent(m_onClickedWwiseEvents);
                    if (m_onClickEventID == enUIEventID.None)
                    {
                        if (onClick != null)
                        {
                            uiEvent.eventID = enUIEventID.None;
                            uiEvent.eventParams = m_onClickEventParams;
                            uiEvent.srcWidget = gameObject;
                            onClick(uiEvent);
                        }
                        return;
                    }
                    uiEvent.eventID = m_onClickEventID;
                    uiEvent.eventParams = m_onClickEventParams;
                    break;
                case enUIEventType.HoldStart:
                    if (this.m_onHoldStartEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uiEvent.eventID = m_onHoldStartEventID;
                    uiEvent.eventParams = m_onHoldStartEventParams;
                    break;
                case enUIEventType.Hold:
                    if (m_onHoldEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uiEvent.eventID = m_onHoldEventID;
                    uiEvent.eventParams = m_onHoldEventParams;
                    break;
                case enUIEventType.HoldEnd:
                    if (m_onHoldEndEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uiEvent.eventID = m_onHoldEndEventID;
                    uiEvent.eventParams = m_onHoldEndEventParams;
                    break;
                case enUIEventType.DragStart:
                    if (m_onDragStartEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uiEvent.eventID = m_onDragStartEventID;
                    uiEvent.eventParams = m_onDragStartEventParams;
                    break;
                case enUIEventType.Drag:
                    if (m_onDragEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uiEvent.eventID = m_onDragEventID;
                    uiEvent.eventParams = m_onDragEventParams;
                    break;
                case enUIEventType.DragEnd:
                    if (m_onDragEndEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uiEvent.eventID = m_onDragEndEventID;
                    uiEvent.eventParams = m_onDragEndEventParams;
                    break;
                case enUIEventType.Drop:
                    if (m_onDropEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uiEvent.eventID = m_onDropEventID;
                    uiEvent.eventParams = m_onDropEventParams;
                    break;
                case enUIEventType.Up:
                    if (m_onUpEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uiEvent.eventID = m_onUpEventID;
                    uiEvent.eventParams = m_onUpEventParams;
                    break;
            }
            uiEvent.srcFormScript = belongedFormScript;
            uiEvent.srcWidgetBelongedListScript = belongedListScript;
            uiEvent.SrcWidgetIndexInBelongedList = indexInList;
            uiEvent.srcWidget = gameObject;
            uiEvent.srcWidgetScript = this;
            uiEvent.pointerEventData = pointerEventData;
            if (eventType == enUIEventType.Click && onClick != null)
            {
                onClick(uiEvent);
            }
            base.DispatchUIEvent(uiEvent);
        }

        private void PostWwiseEvent(string[] wwiseEvents)
        {
            //for (int i = 0; i < wwiseEvents.Length; i++)
            //{
            //    if (!string.IsNullOrEmpty(wwiseEvents[i]))
            //    {
            //        Singleton<CSoundManager>.GetInstance().PostEvent(wwiseEvents[i], null);
            //    }
            //}
        }
    }
