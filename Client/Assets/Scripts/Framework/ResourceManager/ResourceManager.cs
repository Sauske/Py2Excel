using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enResourceType
{
    Numeric,
    UIForm,
    UIPrefab,
    Sound,
    ScenePrefab,
    UISprite,
    Scene,
    Material,
    Effect,
}

public enum enResourceState
{
    Unload,
    Loading,
    Loaded
}

public enum enAssetBundleState
{
    Unload,
    Loading,
    Loaded
}

public class ResourceManager : Singleton<ResourceManager>
{
    public delegate void OnResourceLoaded(ResourceBase resource);

    public static bool isBattleState;

    private static int s_frameCounter;

    private ResourcePackerInfoSet m_resourcePackerInfoSet;

    private Dictionary<int, ResourceBase> m_cachedResourceMap;

    private bool m_clearUnusedAssets;

    private int m_clearUnusedAssetsExecuteFrame;

    public override void Init()
    {
        base.Init();
        m_resourcePackerInfoSet = null;
        m_cachedResourceMap = new Dictionary<int, ResourceBase>();
    }

    public Dictionary<int, ResourceBase> GetCachedResourceMap()
    {
        return m_cachedResourceMap;
    }

    public bool CheckCachedResource(string fullPathInResources)
    {
        string s = FileManager.EraseExtension(fullPathInResources);
        ResourceBase resourceInfo = null;
        return m_cachedResourceMap.TryGetValue(s.JavaHashCodeIgnoreCase(), out resourceInfo);
    }

    public void CustomUpdate()
    {
        ResourceManager.s_frameCounter++;
        if (m_clearUnusedAssets && m_clearUnusedAssetsExecuteFrame == ResourceManager.s_frameCounter)
        {
            ExecuteUnloadUnusedAssets();
            m_clearUnusedAssets = false;
        }
    }

    /// <summary>
    ///  获取资源基本数据结构对象
    /// </summary>
    /// <param name="fullPathInResources">资源在Resource文件夹下完整路径</param>
    /// <param name="resourceContentType">数据类型</param>
    /// <param name="resourceType">资源类型</param>
    /// <param name="needCached">是否需要缓存</param>
    /// <param name="unloadBelongedAssetBundleAfterLoaded">加载资源后是否卸载资源所在的AB包</param>
    /// <returns>资源基本数据</returns>
    public ResourceBase GetResource(string fullPathInResources, Type resourceContentType, enResourceType resourceType, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
    {
        if (string.IsNullOrEmpty(fullPathInResources))
        {
            return new ResourceBase(0, string.Empty, null, resourceType, unloadBelongedAssetBundleAfterLoaded);
        }
        string s = FileManager.EraseExtension(fullPathInResources);
        int key = s.JavaHashCodeIgnoreCase();
        ResourceBase resourceBase = null;
        if (m_cachedResourceMap.TryGetValue(key, out resourceBase))
        {
            if (resourceBase.resourceType != resourceType)
            {
                resourceBase.resourceType = resourceType;
            }
            return resourceBase;
        }
        resourceBase = new ResourceBase(key, fullPathInResources, resourceContentType, resourceType, unloadBelongedAssetBundleAfterLoaded);
        try
        {
            LoadResource(resourceBase);
        }
        catch (Exception exception)
        {
            object[] inParameters = new object[] { s };
            Debug.AssertFormat(false, "Failed Load Resource {0}", inParameters);
            throw exception;
        }
        if (needCached)
        {
           m_cachedResourceMap.Add(key, resourceBase);
        }
        return resourceBase;
    }

    public ResourcePackerInfo GetResourceBelongedPackerInfo(string fullPathInResources)
    {
        if (string.IsNullOrEmpty(fullPathInResources))
        {
            return null;
        }
        if (m_resourcePackerInfoSet != null)
        {
            return m_resourcePackerInfoSet.GetResourceBelongedPackerInfo(FileManager.EraseExtension(fullPathInResources).JavaHashCodeIgnoreCase());
        }
        return null;
    }

    private ResourcePackerInfo GetResourceBelongedPackerInfo(ResourceBase resourceBase)
    {
        if (m_resourcePackerInfoSet != null)
        {
            ResourcePackerInfo resourceBelongedPackerInfo = m_resourcePackerInfoSet.GetResourceBelongedPackerInfo(resourceBase.key);
            return resourceBelongedPackerInfo;
        }
        return null;
    }

    public Type GetResourceContentType(string extension)
    {
        Type result = null;
        if (string.Equals(extension, ".prefab", StringComparison.OrdinalIgnoreCase))
        {
            result = typeof(GameObject);
        }
        else
        {
            if (string.Equals(extension, ".bytes", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase))
            {
                result = typeof(TextAsset);
            }
            else
            {
                if (string.Equals(extension, ".asset", StringComparison.OrdinalIgnoreCase))
                {
                    result = typeof(ScriptableObject);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 加载资源,优先保证从AB包里面(Resources文件夹外部),其次是通过C#IO流加
    /// 载(Resources文件夹外部),最后通过Resources加载((Resources文件夹内存)).
    /// </summary>
    /// <param name="resourceBase">资源基本数据</param>
    private void LoadResource(ResourceBase resourceBase)
    {
        ResourcePackerInfo resourceBelongedPackerInfo = GetResourceBelongedPackerInfo(resourceBase);
        if (resourceBelongedPackerInfo != null)
        {
            if (resourceBelongedPackerInfo.isAssetBundle)
            {
                if (!resourceBelongedPackerInfo.IsAssetBundleLoaded())
                {
                    resourceBelongedPackerInfo.LoadAssetBundle(FileManager.GetIFSExtractPath());
                }
                
                resourceBase.LoadFromAssetBundle(resourceBelongedPackerInfo);
                if (resourceBase.unloadBelongedAssetBundleAfterLoaded)
                {
                    resourceBelongedPackerInfo.UnloadAssetBundle(false);
                }
            }
            else
            {
                resourceBase.Load(FileManager.GetIFSExtractPath());
            }
        }
        else
        {
            resourceBase.Load();
        }
    }

    public void RemoveAllCachedResources()
    {
        this.RemoveCachedResources((enResourceType[])Enum.GetValues(typeof(enResourceType)));
    }

    public void RemoveCachedResource(string fullPathInResources)
    {
        string s = FileManager.EraseExtension(fullPathInResources);
        int key = s.JavaHashCodeIgnoreCase();
        ResourceBase resourceBase = null;
        if (m_cachedResourceMap.TryGetValue(key, out resourceBase))
        {
            resourceBase.Unload();
            this.m_cachedResourceMap.Remove(key);
        }
    }

    public void RemoveCachedResources(enResourceType[] resourceTypes)
    {
        for (int i = 0; i < resourceTypes.Length; i++)
        {
            RemoveCachedResources(resourceTypes[i], false);
        }
        UnloadAllAssetBundles();
        UnloadUnusedAssets();
    }

    public void RemoveCachedResources(enResourceType resourceType, bool clearImmediately = true)
    {
        List<int> list = new List<int>();
        Dictionary<int, ResourceBase>.Enumerator enumerator = m_cachedResourceMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<int, ResourceBase> current = enumerator.Current;
            ResourceBase value = current.Value;
            if (value.resourceType == resourceType)
            {
                value.Unload();
                list.Add(value.key);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            m_cachedResourceMap.Remove(list[i]);
        }
        if (clearImmediately)
        {
            UnloadAllAssetBundles();
            UnloadUnusedAssets();
        }
    }

    private void ExecuteUnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    private void UnloadAllAssetBundles()
    {
        if (m_resourcePackerInfoSet == null)
        {
            return;
        }
        for (int i = 0; i < m_resourcePackerInfoSet.resourcePackerInfosAll.Count; i++)
        {
            ResourcePackerInfo resourcePackerInfo = m_resourcePackerInfoSet.resourcePackerInfosAll[i];
            if (resourcePackerInfo.IsAssetBundleLoaded())
            {
                resourcePackerInfo.UnloadAssetBundle(false);
            }
        }
    }

    public void UnloadBelongedAssetbundle(string fullPathInResources)
    {
        ResourcePackerInfo resourceBelongedPackerInfo = GetResourceBelongedPackerInfo(fullPathInResources);
        if (resourceBelongedPackerInfo != null && resourceBelongedPackerInfo.IsAssetBundleLoaded())
        {
            resourceBelongedPackerInfo.UnloadAssetBundle(false);
        }
    }

    public void UnloadUnusedAssets()
    {
        m_clearUnusedAssets = true;
        m_clearUnusedAssetsExecuteFrame = ResourceManager.s_frameCounter + 1;
    }
}
