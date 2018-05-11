using System;
using System.Collections.Generic;


public class UIEventManager : Singleton<UIEventManager>
{
    public delegate void OnUIEventHandler(UIEvent uiEvent);

    private UIEventManager.OnUIEventHandler[] m_uiEventHandlerMap = new UIEventManager.OnUIEventHandler[15522];

    private List<object> m_uiEvents = new List<object>();

    public bool HasUIEventListener(enUIEventID eventID)
    {
        return m_uiEventHandlerMap[(int)((UIntPtr)eventID)] != null;
    }

    public void AddUIEventListener(enUIEventID eventID, UIEventManager.OnUIEventHandler onUIEventHandler)
    {
        if (m_uiEventHandlerMap[(int)((UIntPtr)eventID)] == null)
        {
            m_uiEventHandlerMap[(int)((UIntPtr)eventID)] = delegate
            {
            };
            UIEventManager.OnUIEventHandler[] onUIEventHandlerMap = m_uiEventHandlerMap;
            UIntPtr intPtr = (UIntPtr)eventID;
            onUIEventHandlerMap[(int)intPtr] = (UIEventManager.OnUIEventHandler)Delegate.Combine(onUIEventHandlerMap[(int)intPtr], onUIEventHandler);
        }
        else
        {
            UIEventManager.OnUIEventHandler[] onUIEventHandlerMap1 = m_uiEventHandlerMap;
            UIntPtr intPtr1 = (UIntPtr)eventID;
            onUIEventHandlerMap1[(int)intPtr1] = (UIEventManager.OnUIEventHandler)Delegate.Remove(onUIEventHandlerMap1[(int)intPtr1], onUIEventHandler);
            UIEventManager.OnUIEventHandler[] onUIEventHandlerMap2 = m_uiEventHandlerMap;
            UIntPtr intPtr2 = (UIntPtr)eventID;
            onUIEventHandlerMap2[(int)intPtr2] = (UIEventManager.OnUIEventHandler)Delegate.Combine(onUIEventHandlerMap2[(int)intPtr2], onUIEventHandler);
        }
    }

    public void RemoveUIEventListener(enUIEventID eventID, UIEventManager.OnUIEventHandler onUIEventHandler)
    {
        if (m_uiEventHandlerMap[(int)((UIntPtr)eventID)] != null)
        {
            UIEventManager.OnUIEventHandler[] onUIEventHandlerMap = m_uiEventHandlerMap;
            UIntPtr ptr = (UIntPtr)eventID;
            onUIEventHandlerMap[(int)ptr] = (UIEventManager.OnUIEventHandler)Delegate.Remove(onUIEventHandlerMap[(int)ptr], onUIEventHandler);
        }
    }

    public void DispatchUIEvent(UIEvent uiEvent)
    {
        uiEvent.inUse = true;
        UIEventManager.OnUIEventHandler onUIEventHandler = m_uiEventHandlerMap[(int)((UIntPtr)uiEvent.eventID)];
        if (onUIEventHandler != null)
        {
            onUIEventHandler(uiEvent);
        }
        uiEvent.Clear();
    }

    public void DispatchUIEvent(enUIEventID eventID)
    {
        UIEvent uiEvent = GetUIEvent();
        uiEvent.eventID = eventID;
        DispatchUIEvent(uiEvent);
    }

    public void DispatchUIEvent(enUIEventID eventID, stUIEventParams par)
    {
        UIEvent uiEvent = GetUIEvent();
        uiEvent.eventID = eventID;
        uiEvent.eventParams = par;
        DispatchUIEvent(uiEvent);
    }

    public UIEvent GetUIEvent()
    {
        for (int i = 0; i < m_uiEvents.Count; i++)
        {
            UIEvent uiEvent = (UIEvent)m_uiEvents[i];
            if (!uiEvent.inUse)
            {
                return uiEvent;
            }
        }
        UIEvent uiEvent2 = new UIEvent();
        m_uiEvents.Add(uiEvent2);
        return uiEvent2;
    }
}

