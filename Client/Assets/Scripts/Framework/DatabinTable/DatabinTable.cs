using System;
using ProtoBuf;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class DatabinTable<T, K> : DatabinTableBase
{
    public DatabinTable(string inName, string inKey)
        : base(typeof(T))
    {
        m_dataName = inName;
        m_keyName = inKey;
        m_mapItems.Clear();
        m_bLoaded = false;
        Singleton<ResourceLoader>.GetInstance().LoadDatabin(inName, new ResourceLoader.BinLoadCompletedDelegate(base.OnRecordLoaded));
    }

    public override void LoadTdrBin(byte[] rawData, Type inValueType)
    {
        try
        {
            using (Stream stream = new MemoryStream(rawData))
            {
                T list = ProtoBuf.Serializer.Deserialize<T>(stream);
                PropertyInfo listProp = list.GetType().GetProperty("items");
                List<K> recordList = (List<K>)listProp.GetGetMethod().Invoke(list, null);
                if (recordList != null)
                {
                    for (int i = 0; i < recordList.Count; i++)
                    {
                        K item = (K)recordList[i];
                        PropertyInfo itemProp = recordList[i].GetType().GetProperty(m_keyName);
                        int id = System.Convert.ToInt32(itemProp.GetGetMethod().Invoke(recordList[i], null));
                        if (!m_mapItems.ContainsKey(id))
                        {
                            m_mapItems.Add(id, item as ProtoBuf.IExtensible);
                        }
                        else
                        {
                            #if UNITY_EDITOR
                            Debug.LogError(string.Format("加载表{0}时出现重复的资源ID：{1}", typeof(T).ToString(), id));
                            #endif
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            #if UNITY_EDITOR
            Debug.LogError("LoadError type is: " + typeof(T).ToString());
            #endif
        }
    }

    public void CopyTo(ref K[] inArrayRef)
    {
        base.Reload();
        int num = 0;
        Dictionary<long, object>.Enumerator enumerator = m_mapItems.GetEnumerator();
        while (enumerator.MoveNext())
        {
            K[] tarrs = inArrayRef;
            int flag = num++;
            KeyValuePair<long, object> current = enumerator.Current;
            tarrs[flag] = (K)((object)current.Value);
        }
    }

    public K GetDataByKey(int key)
    {
        return GetDataByKey((long)key);
    }

    public K GetDataByKey(long key)
    {
        Reload();
        K k = (K)((object)GetDataByKeyInner(key));
        if (k == null && m_bSimple && key != 0L)
        {
            Unload();
            Reload();
            m_bSimple = false;
            k = (K)((object)GetDataByKeyInner(key));
        }
        return k;
    }

    public void UpdataData(uint key, K data)
    {
        UpdateData((long)((ulong)key), data);
    }

    private void UpdateData(long key, K data)
    {
        Reload();
        if (m_mapItems.ContainsKey(key))
        {
            m_mapItems[key] = data;
        }
        else
        {
            m_mapItems.Add(key, data);
        }
    }

    public K FindIf(Func<K, bool> InFunc)
    {
        Reload();
        if (isLoaded)
        {
            Dictionary<long, object>.Enumerator enumerator = m_mapItems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                if (InFunc((K)((object)current.Value)))
                {
                    KeyValuePair<long, object> current2 = enumerator.Current;
                    return (K)((object)current2.Value);
                }
            }
        }
        return default(K);
    }

    public void Accept(Action<K> InVisitor)
    {
        Reload();
        if (isLoaded)
        {
            Dictionary<long, object>.Enumerator enumerator = m_mapItems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                InVisitor((K)((object)current.Value));
            }
        }
    }

    public K GetDataByIndex(int id)
    {
        Reload();
        if (isLoaded)
        {
            Dictionary<long, object>.Enumerator enumerator = m_mapItems.GetEnumerator();
            int num = 0;
            while (enumerator.MoveNext())
            {
                if (num == id)
                {
                    KeyValuePair<long, object> current = enumerator.Current;
                    return (K)((object)current.Value);
                }
                num++;
            }
        }
        return default(K);
    }

    public K GetAnyData()
    {
        Reload();
        if (isLoaded && m_mapItems.Count > 0)
        {
            Dictionary<long, object>.Enumerator enumerator = m_mapItems.GetEnumerator();
            enumerator.MoveNext();
            KeyValuePair<long, object> current = enumerator.Current;
            return (K)((object)current.Value);
        }
        return default(K);
    }

    public void ReduceDatabin(List<long> dataList)
    {
        Reload();
        if (isLoaded)
        {
            Dictionary<long, object>.Enumerator enumerator = m_mapItems.GetEnumerator();
            List<long> list = new List<long>();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                if (!dataList.Contains(current.Key))
                {
                    List<long> list2 = list;
                    KeyValuePair<long, object> current2 = enumerator.Current;
                    list2.Add(current2.Key);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                m_mapItems.Remove(list[i]);
            }
            m_bSimple = true;
        }
    }
}
