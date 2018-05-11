using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePackerInfoSet 
{
    public List<ResourcePackerInfo> resourcePackerInfosAll = new List<ResourcePackerInfo>();

    private Dictionary<int, ResourcePackerInfo> m_resourceMap = new Dictionary<int, ResourcePackerInfo>();

    public void AddResourcePackerInfo(ResourcePackerInfo resourceInfo)
    {
        resourcePackerInfosAll.Add(resourceInfo);
        for (int i = 0; i < resourceInfo.childrens.Count; ++i)
        {
            AddResourcePackerInfoAll(resourceInfo.childrens[i]);
        }
    }

    private void AddResourcePackerInfoAll(ResourcePackerInfo resourceInfo)
    {
        resourcePackerInfosAll.Add(resourceInfo);
        for (int i = 0; i < resourceInfo.childrens.Count; i++)
        {
            AddResourcePackerInfoAll(resourceInfo.childrens[i]);
        }
    }

    public ResourcePackerInfo GetResourceBelongedPackerInfo(int resourceKeyHash)
    {
        ResourcePackerInfo info = null;
        if (m_resourceMap.TryGetValue(resourceKeyHash, out info))
        {
            return info;
        }
        return null;
    }

    public void CreateResourceMap()
    {
        for (int i = 0; i < resourcePackerInfosAll.Count; ++i)
        {
            resourcePackerInfosAll[i].AddToResourceMap(m_resourceMap);
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < resourcePackerInfosAll.Count; ++i)
        {
            if (resourcePackerInfosAll[i].IsAssetBundleLoaded())
            {
                resourcePackerInfosAll[i].UnloadAssetBundle(false);
            }
            resourcePackerInfosAll[i] = null;
        }
        resourcePackerInfosAll.Clear();
        m_resourceMap.Clear();
    }

}
