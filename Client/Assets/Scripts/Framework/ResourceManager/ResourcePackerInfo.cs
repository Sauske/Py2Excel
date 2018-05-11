using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourcePackerInfo 
{
    private bool m_isAssetBundle;

    private bool m_resident;

    private bool m_useAsyncLoadingData;

    private string m_pathInIFS;

    private enAssetBundleState m_assetBundleState;

    private AssetBundle m_assetBundle;

    private ResourcePackerInfo m_parent;

    private List<int> m_resourceInfos = new List<int>();

    private List<ResourcePackerInfo> m_children = new List<ResourcePackerInfo>();

    public bool isAssetBundle
    {
        get
        {
            return m_isAssetBundle;
        }
    }

    public AssetBundle assetBundle
    {
        get
        {
            return m_assetBundle;
        }
    }

    public ResourcePackerInfo dependency
    {
        get
        {
            return m_parent;
        }
        set
        {
            m_parent = value;
            m_children.Add(this);
        }
    }

    public List<ResourcePackerInfo> childrens
    {
        get
        {
            return m_children;
        }
    }

    public bool IsResident()
    {
        return dependency == null && m_resident;
    }

    public bool IsAssetBundleLoaded()
    {
        return m_isAssetBundle && m_assetBundleState == enAssetBundleState.Loaded;
    }

    public void AddToResourceMap(Dictionary<int, ResourcePackerInfo> map)
    {
        for (int i = 0; i < m_resourceInfos.Count; ++i)
        {
            if (!map.ContainsKey(m_resourceInfos[i]))
            {
                map.Add(m_resourceInfos[i], this);
            }
        }
    }

    public void LoadAssetBundle(string ifsExtractPath)
    {
        if (!m_isAssetBundle)
        {
            return;
        }
        if (dependency != null && dependency.m_isAssetBundle && !dependency.IsAssetBundleLoaded())
        {
            dependency.LoadAssetBundle(ifsExtractPath);
        }
        if (m_assetBundleState != enAssetBundleState.Unload)
        {
            return;
        }
        m_useAsyncLoadingData = false;
        string filePath = FileManager.CombinePath(ifsExtractPath,m_pathInIFS);
        
        if (FileManager.IsFileExist(filePath))
		{
		    int num = 0;
			while (true)
			{
				try
				{
				    m_assetBundle = AssetBundle.LoadFromFile(filePath);
				}
				catch (Exception)
				{
					m_assetBundle = null;
				}
				if (this.m_assetBundle != null)
				{
					break;
				}
				num++;
				if (num >= 3)
				{
					break;
				}
			}
					
			if (m_assetBundle == null) 
            {
				Debug.LogError("Load AssetBundle " + filePath + " Error!!!");
			}
		}
		else
		{
			Debug.LogError("File " + filePath + " can not be found!!!");
		}
		m_assetBundleState = enAssetBundleState.Loaded;
    }

    public IEnumerator AsyncLoadAssetBundle(string ifsExtractPath)
    {
        if (m_isAssetBundle)
        {
            yield break;
        }
        
        m_useAsyncLoadingData = true;
        m_assetBundleState = enAssetBundleState.Loading;
        
        AssetBundleCreateRequest assetBundleLoader = AssetBundle.LoadFromFileAsync(FileManager.CombinePath(ifsExtractPath,m_pathInIFS));
        yield return assetBundleLoader;
        
        if (m_useAsyncLoadingData)
        {
            m_assetBundle = assetBundleLoader.assetBundle;
        }
        m_assetBundleState = enAssetBundleState.Loaded;
    }

    public void UnloadAssetBundle(bool force = false)
    {
        if (m_isAssetBundle && (!IsResident() || force))
        {
            if (m_assetBundleState == enAssetBundleState.Loaded)
            {
                if (m_assetBundle != null)
                {
                    m_assetBundle.Unload(false);
                    m_assetBundle = null;
                }
                m_assetBundleState = enAssetBundleState.Unload;
            }
            else if (m_assetBundleState == enAssetBundleState.Loading)
            {
                m_useAsyncLoadingData = false;
            }
            if (dependency != null)
            {
                dependency.UnloadAssetBundle(force);
            }
        }
    }
}
