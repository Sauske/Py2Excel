using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableProto;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        chapter_info info = TableResMgr.GetInstance().GetRecordKey<chapter_info>(TableResMgr.IdxHint,2);
        if(info != null)
        {
            Debug.Log(info.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
