using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UITools
{
    public static GameObject FindChild(this UIFormScript form, string path)
    {
        if (form == null) return null;

        Transform mTran = form.transform.Find(path);

        if (mTran == null)
        {
            Debug.LogError("没有找到该对象:" + path);
            return null;
        }

        return mTran.gameObject;
    }

    public static T FindChild<T>(this Component com,string path) where T : MonoBehaviour
    {
        if(com == null)
        {
            Debug.Log("This component is Null. path: " + path);
            return default(T);
        }

        Transform tran = com.transform.Find(path);
        if(tran == null)
        {
            Debug.Log("This component child is Null. path: " + path);
            return default(T);
        }

        T component = tran.GetComponent<T>();
        return component;
    }

    public static void NewSetActive(this GameObject go,bool isActive)
    {
        if (go == null) return;

        go.SetActive(isActive);
    }
}
