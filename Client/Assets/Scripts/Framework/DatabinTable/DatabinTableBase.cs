using System;
using ProtoBuf;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DatabinTableBase
{
    protected Dictionary<long, object> m_mapItems = new Dictionary<long, object>();

    protected string m_dataName;

    protected string m_keyName;

    protected bool m_bLoaded;

    protected bool m_bAllowUnLoad = true;

    protected Type m_valueType;

    protected bool m_bSimple;

    public string Name
    {
        get
        {
            return m_dataName;
        }
    }

    public bool isLoaded
    {
        get
        {
            return m_bLoaded;
        }
    }

    public bool isAllowUnLoad
    {
        set
        {
            m_bAllowUnLoad = value;
        }
    }

    public int count
    {
        get
        {
            return Count();
        }
    }

    public DatabinTableBase(Type inValueType)
    {
        m_valueType = inValueType;
    }

    public Dictionary<long, object>.Enumerator GetEnumerator()
    {
        return m_mapItems.GetEnumerator();
    }

    public void Unload()
    {
        if (!m_bAllowUnLoad)
        {
            return;
        }
        m_bLoaded = false;
        m_bSimple = false;
        m_mapItems.Clear();
    }

    public virtual void LoadTdrBin(byte[] rawData, Type inValueType)
    {

    }

    protected object GetDataByKeyInner(long key)
    {
        object result;
        if (m_bLoaded && m_mapItems.TryGetValue(key, out result))
        {
            return result;
        }
        return null;
    }

    protected long GetDataKey(object data, Type inValueType)
    {
        Type type = data.GetType();
        long result;
        
        FieldInfo field3 = type.GetField(m_keyName);
        object value3 = field3.GetValue(data);
        try
        {
            result = ((value3 == null) ? 0L : Convert.ToInt64(value3));
        }
        catch (Exception ex2)
        {
            #if UNITY_EDITOR
            Debug.LogError("Exception in Databin Get Key :" + ex2);
            #endif
            result = 0L;
        }
        return result;
    }

    public void Reload()
    {
        if (isLoaded)
        {
            return;
        }
        Singleton<ResourceLoader>.GetInstance().LoadDatabin(Name, new ResourceLoader.BinLoadCompletedDelegate(OnRecordLoaded));
    }

    public int Count()
    {
        Reload();
        return m_mapItems.Count;
    }

    protected void OnRecordLoaded(ref byte[] rawData)
    {
        LoadTdrBin(rawData, m_valueType);
        m_bLoaded = true;
    }
}
