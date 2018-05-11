using System;
using UnityEngine;

public class PooledGameObjectScript : MonoBehaviour
{
    public string prefabKey;

    public bool isInit;

    public Vector3 defaultScale;

    private IPooledMonoBehaviour[] m_cachedIPooledMonos;

    private bool m_inUse;

    public void Initialize(string tprefabKey)
    {
        MonoBehaviour[] componentsInChildren = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
        if (componentsInChildren != null && componentsInChildren.Length > 0)
        {
            int num = 0;
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i] is IPooledMonoBehaviour)
                {
                    num++;
                }
            }
            m_cachedIPooledMonos = new IPooledMonoBehaviour[num];
            int num2 = 0;
            for (int j = 0; j < componentsInChildren.Length; j++)
            {
                if (componentsInChildren[j] is IPooledMonoBehaviour)
                {
                    m_cachedIPooledMonos[num2] = (componentsInChildren[j] as IPooledMonoBehaviour);
                    num2++;
                }
            }
        }
        else
        {
            m_cachedIPooledMonos = new IPooledMonoBehaviour[0];
        }
        prefabKey = tprefabKey;
        defaultScale = gameObject.transform.localScale;
        isInit = true;
        m_inUse = false;
    }

    public void AddCachedMono(MonoBehaviour mono, bool defaultEnabled)
    {
        if (mono == null)
        {
            return;
        }
        if (mono is IPooledMonoBehaviour)
        {
            IPooledMonoBehaviour[] array = new IPooledMonoBehaviour[m_cachedIPooledMonos.Length + 1];
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                array[i] = m_cachedIPooledMonos[i];
            }
            array[m_cachedIPooledMonos.Length] = (mono as IPooledMonoBehaviour);
            m_cachedIPooledMonos = array;
        }
    }

    public void OnCreate()
    {
        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                if (m_cachedIPooledMonos[i] != null)
                {
                    m_cachedIPooledMonos[i].OnCreate();
                }
            }
        }
    }

    public void OnGet()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                if (m_cachedIPooledMonos[i] != null)
                {
                    m_cachedIPooledMonos[i].OnGet();
                }
            }
        }
        m_inUse = true;
    }

    public void OnRecycle()
    {
        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
        {
            for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
            {
                if (m_cachedIPooledMonos[i] != null)
                {
                    m_cachedIPooledMonos[i].OnRecycle();
                }
            }
        }
        gameObject.SetActive(false);
        m_inUse = false;
    }

    public void OnPrepare()
    {
        gameObject.SetActive(false);
    }
}
