using System;
using System.Collections.Generic;

public class EventRouter : Singleton<EventRouter>
{
    public Dictionary<string, Delegate> eventTables = new Dictionary<string, Delegate>();

    public void AddEventHandler(string eventType, Action handler)
    {
        if (OnHandlerAdding(eventType, handler))
        {
            eventTables[eventType] = (Action)Delegate.Combine((Action)eventTables[eventType], handler);
        }
    }

    public void AddEventHandler<T1>(string eventType, Action<T1> handler)
    {
        if (OnHandlerAdding(eventType, handler))
        {
            eventTables[eventType] = (Action<T1>)Delegate.Combine((Action<T1>)eventTables[eventType], handler);
        }
    }

    public void AddEventHandler<T1, T2>(string eventType, Action<T1, T2> handler)
    {
        if (OnHandlerAdding(eventType, handler))
        {
            eventTables[eventType] = (Action<T1, T2>)Delegate.Combine((Action<T1, T2>)eventTables[eventType], handler);
        }
    }

    public void AddEventHandler<T1, T2, T3>(string eventType, Action<T1, T2, T3> handler)
    {
        if (OnHandlerAdding(eventType, handler))
        {
            eventTables[eventType] = (Action<T1, T2, T3>)Delegate.Combine((Action<T1, T2, T3>)eventTables[eventType], handler);
        }
    }

    public void AddEventHandler<T1, T2, T3, T4>(string eventType, Action<T1, T2, T3, T4> handler)
    {
        if (OnHandlerAdding(eventType, handler))
        {
            eventTables[eventType] = (Action<T1, T2, T3, T4>)Delegate.Combine((Action<T1, T2, T3, T4>)eventTables[eventType], handler);
        }
    }

    public void RemoveEventHandler(string eventType, Action handler)
    {
        if (OnHandlerRemoving(eventType, handler))
        {
            eventTables[eventType] = (Action)Delegate.Remove((Action)eventTables[eventType], handler);
        }
    }

    public void RemoveEventHandler<T1>(string eventType, Action<T1> handler)
    {
        if (OnHandlerRemoving(eventType, handler))
        {
            eventTables[eventType] = (Action<T1>)Delegate.Remove((Action<T1>)eventTables[eventType], handler);
        }
    }

    public void RemoveEventHandler<T1, T2>(string eventType, Action<T1, T2> handler)
    {
        if (OnHandlerRemoving(eventType, handler))
        {
            eventTables[eventType] = (Action<T1, T2>)Delegate.Remove((Action<T1, T2>)eventTables[eventType], handler);
        }
    }

    public void RemoveEventHandler<T1, T2, T3>(string eventType, Action<T1, T2, T3> handler)
    {
        if (OnHandlerRemoving(eventType, handler))
        {
            eventTables[eventType] = (Action<T1, T2, T3>)Delegate.Remove((Action<T1, T2, T3>)eventTables[eventType], handler);
        }
    }

    public void RemoveEventHandler<T1, T2, T3, T4>(string eventType, Action<T1, T2, T3, T4> handler)
    {
        if (OnHandlerRemoving(eventType, handler))
        {
            eventTables[eventType] = (Action<T1, T2, T3, T4>)Delegate.Remove((Action<T1, T2, T3, T4>)eventTables[eventType], handler);
        }
    }

    public void BroadCastEvent(string eventType)
    {
        if (OnBroadCasting(eventType))
        {
            Action action = eventTables[eventType] as Action;
            if (action != null)
            {
                action();
            }
        }
    }

    public void BroadCastEvent<T1>(string eventType, T1 arg1)
    {
        if (OnBroadCasting(eventType))
        {
            Action<T1> action = eventTables[eventType] as Action<T1>;
            if (action != null)
            {
                action(arg1);
            }
        }
    }

    public void BroadCastEvent<T1, T2>(string eventType, T1 arg1, T2 arg2)
    {
        if (OnBroadCasting(eventType))
        {
            Action<T1, T2> action = eventTables[eventType] as Action<T1, T2>;
            if (action != null)
            {
                action(arg1, arg2);
            }
        }
    }

    public void BroadCastEvent<T1, T2, T3>(string eventType, T1 arg1, T2 arg2, T3 arg3)
    {
        if (OnBroadCasting(eventType))
        {
            Action<T1, T2, T3> action = eventTables[eventType] as Action<T1, T2, T3>;
            if (action != null)
            {
                action(arg1, arg2, arg3);
            }
        }
    }

    public void BroadCastEvent<T1, T2, T3, T4>(string eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (this.OnBroadCasting(eventType))
        {
            Action<T1, T2, T3, T4> action = eventTables[eventType] as Action<T1, T2, T3, T4>;
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4);
            }
        }
    }

    private bool OnHandlerAdding(string eventType, Delegate handler)
    {
        bool result = true;
        if (!eventTables.ContainsKey(eventType))
        {
            eventTables.Add(eventType, null);
        }
        Delegate @delegate = eventTables[eventType];
        if (@delegate != null && @delegate.GetType() != handler.GetType())
        {
            result = false;
        }
        return result;
    }

    private bool OnHandlerRemoving(string eventType, Delegate handler)
    {
        bool result = true;
        if (eventTables.ContainsKey(eventType))
        {
            Delegate @delegate = eventTables[eventType];
            if (@delegate != null)
            {
                if (@delegate.GetType() != handler.GetType())
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
        }
        else
        {
            result = false;
        }
        return result;
    }

    private bool OnBroadCasting(string eventType)
    {
        return eventTables.ContainsKey(eventType);
    }

    public void ClearAllEvents()
    {
        eventTables.Clear();
    }
}
