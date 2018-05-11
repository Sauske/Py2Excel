using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode 
{
    private string m_name;

    private string m_path;

    private bool m_isAssetBundle;

    private bool m_isResident;

    private List<ResourceNode> m_childs;

    public string Name
    {
        get
        {
            return m_name;
        }
    }

    public string Path
    {
        get
        {
            return m_path;
        }
    }

    public bool IsAssetBundle
    {
        get
        {
            return m_isAssetBundle;
        }
    }

    public bool IsResident
    {
        get
        {
            return m_isResident;
        }
    }

    public List<ResourceNode> childs
    {
        get
        {
            return m_childs;
        }
    }
}
