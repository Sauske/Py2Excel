using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMiniEventScript : UIComponent, IPointerDownHandler, IEventSystemHandler, IPointerClickHandler, IPointerUpHandler
{
    public delegate void OnUIEventHandler(UIEvent uiEvent);

    [HideInInspector]
    public enUIEventID onDownEventID;

    [NonSerialized]
    public stUIEventParams onDownEventParams;

    [HideInInspector]
    public enUIEventID onUpEventID;

    [NonSerialized]
    public stUIEventParams onUpEventParams;

    [HideInInspector]
    public enUIEventID onClickEventID;

    [NonSerialized]
    public stUIEventParams onClickEventParams;

    public bool closeFormWhenClicked;

    public string[] onDownWwiseEvents = new string[0];

    public string[] onClickedWwiseEvents = new string[0];

    public UIMiniEventScript.OnUIEventHandler onClick;

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
        onClick = null;
        base.OnDestroy();
    }

    public void SetUIEvent(enUIEventType eventType, enUIEventID eventID)
    {
        if (eventType != enUIEventType.Down)
        {
            if (eventType != enUIEventType.Click)
            {
                if (eventType == enUIEventType.Up)
                {
                  onUpEventID = eventID;
                }
            }
            else
            {
              onClickEventID = eventID;
            }
        }
        else
        {
          onDownEventID = eventID;
        }
    }

    public void SetUIEvent(enUIEventType eventType, enUIEventID eventID, stUIEventParams eventParams)
    {
        if (eventType != enUIEventType.Down)
        {
            if (eventType != enUIEventType.Click)
            {
                if (eventType == enUIEventType.Up)
                {
                  onUpEventID = eventID;
                  onUpEventParams = eventParams;
                }
            }
            else
            {
              onClickEventID = eventID;
              onClickEventParams = eventParams;
            }
        }
        else
        {
          onDownEventID = eventID;
          onDownEventParams = eventParams;
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
      DispatchUIEvent(enUIEventType.Down, eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
      DispatchUIEvent(enUIEventType.Up, eventData);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        bool flag = true;
        if (belongedFormScript != null && !belongedFormScript.enableMultiClickedEvent)
        {
            flag = (belongedFormScript.clickedEventDispatchedCounter == 0);
            belongedFormScript.clickedEventDispatchedCounter++;
        }
        if (flag)
        {
            if (belongedListScript != null && indexInList >= 0)
            {
                belongedListScript.SelectElement(indexInList, true);
            }
            DispatchUIEvent(enUIEventType.Click, eventData);
            if (closeFormWhenClicked && belongedFormScript != null)
            {
              belongedFormScript.Close();
            }
        }
    }

    private void Update()
    {
    }

    private void DispatchUIEvent(enUIEventType eventType, PointerEventData pointerEventData)
    {
        UIEvent uIEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
        if (eventType != enUIEventType.Down)
        {
            if (eventType != enUIEventType.Click)
            {
                if (eventType == enUIEventType.Up)
                {
                    if (onUpEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uIEvent.eventID = onUpEventID;
                    uIEvent.eventParams = onUpEventParams;
                }
            }
            else
            {
              PostWwiseEvent(onClickedWwiseEvents);
                if (onClickEventID == enUIEventID.None)
                {
                    if (onClick != null)
                    {
                        uIEvent.eventID = enUIEventID.None;
                        uIEvent.eventParams = onClickEventParams;
                        onClick(uIEvent);
                    }
                    return;
                }
                uIEvent.eventID = onClickEventID;
                uIEvent.eventParams = onClickEventParams;
            }
        }
        else
        {
            PostWwiseEvent(onDownWwiseEvents);
            if (onDownEventID == enUIEventID.None)
            {
                return;
            }
            uIEvent.eventID = onDownEventID;
            uIEvent.eventParams = onDownEventParams;
        }
        uIEvent.srcFormScript = belongedFormScript;
        uIEvent.srcWidgetBelongedListScript = belongedListScript;
        uIEvent.SrcWidgetIndexInBelongedList =indexInList;
        uIEvent.srcWidget = gameObject;
        uIEvent.srcWidgetScript = this;
        uIEvent.pointerEventData = pointerEventData;
        if (eventType == enUIEventType.Click && onClick != null)
        {
          onClick(uIEvent);
        }
        DispatchUIEvent(uIEvent);
    }

    protected void PostWwiseEvent(string[] wwiseEvents)
    {
        for (int i = 0; i < wwiseEvents.Length; i++)
        {
            if (!string.IsNullOrEmpty(wwiseEvents[i]))
            {
             //   Singleton<CSoundManager>.GetInstance().PostEvent(wwiseEvents[i], null);
            }
        }
    }
}
