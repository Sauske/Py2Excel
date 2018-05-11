using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameObjectPool : Singleton<GameObjectPool>
{
    private class stDelayRecycle
    {
        public GameObject recycleObj;

        public int recycleTime;

        public GameObjectPool.OnDelayRecycleDelegate callback;
    }

    public class ParticleSystemCache
    {
        public ParticleSystem par;

        public bool emmitState;
    }

    public delegate void OnDelayRecycleDelegate(GameObject recycleObj);

    private Dictionary<int, Queue<PooledGameObjectScript>> m_pooledGameObjectMap = new Dictionary<int, Queue<PooledGameObjectScript>>();

    private Dictionary<int, Component> m_componentMap = new Dictionary<int, Component>();

    private LinkedList<GameObjectPool.stDelayRecycle> m_delayRecycle = new LinkedList<GameObjectPool.stDelayRecycle>();

    private GameObject m_poolRoot;

    private bool m_clearPooledObjects;

    private int m_clearPooledObjectsExecuteFrame;

    private static int s_frameCounter;

    public override void Init()
    {
        this.m_poolRoot = new GameObject("GameObjectPool");
        GameObject gameObject = GameObject.Find("BootObj");
        if (gameObject != null)
        {
            this.m_poolRoot.transform.SetParent(gameObject.transform);
        }
    }

    public override void UnInit()
    {
    }

    public void Update()
    {
        GameObjectPool.s_frameCounter++;
        UpdateDelayRecycle();
        if (m_clearPooledObjects && m_clearPooledObjectsExecuteFrame == GameObjectPool.s_frameCounter)
        {
            ExecuteClearPooledObjects();
            m_clearPooledObjects = false;
        }
    }

    public void ClearPooledObjects()
    {
        m_clearPooledObjects = true;
        m_clearPooledObjectsExecuteFrame = GameObjectPool.s_frameCounter + 1;
    }

    public void ExecuteClearPooledObjects()
    {
        for (LinkedListNode<GameObjectPool.stDelayRecycle> linkedListNode = m_delayRecycle.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
            if (null != linkedListNode.Value.recycleObj)
            {
                RecycleGameObject(linkedListNode.Value.recycleObj);
            }
        }
        m_delayRecycle.Clear();
        m_componentMap.Clear();
        Dictionary<int, Queue<PooledGameObjectScript>>.Enumerator enumerator = m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<int, Queue<PooledGameObjectScript>> current = enumerator.Current;
            Queue<PooledGameObjectScript> value = current.Value;
            while (value.Count > 0)
            {
                PooledGameObjectScript cPooledGameObjectScript = value.Dequeue();
                if (cPooledGameObjectScript != null && cPooledGameObjectScript.gameObject != null)
                {
                    UnityEngine.Object.Destroy(cPooledGameObjectScript.gameObject);
                }
            }
        }
        m_pooledGameObjectMap.Clear();
    }

    public void UpdateParticleChecker(int maxNum)
    {
    }

    private void UpdateDelayRecycle()
    {
        LinkedListNode<GameObjectPool.stDelayRecycle> linkedListNode = m_delayRecycle.First;
        int num = (int)(Time.time * 1000f);
        while (linkedListNode != null)
        {
            LinkedListNode<GameObjectPool.stDelayRecycle> linkedListNode2 = linkedListNode;
            linkedListNode = linkedListNode.Next;
            if (null == linkedListNode2.Value.recycleObj)
            {
                m_delayRecycle.Remove(linkedListNode2);
            }
            else
            {
                if (linkedListNode2.Value.recycleTime > num)
                {
                    break;
                }
                if (linkedListNode2.Value.callback != null)
                {
                    linkedListNode2.Value.callback(linkedListNode2.Value.recycleObj);
                }
                RecycleGameObject(linkedListNode2.Value.recycleObj);
                m_delayRecycle.Remove(linkedListNode2);
            }
        }
    }

    public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType)
    {
        bool flag = false;
        return GetGameObject(prefabFullPath, pos, rot, true, resourceType, out flag);
    }

    public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType, out bool isInit)
    {
        return GetGameObject(prefabFullPath, pos, rot, true, resourceType, out isInit);
    }

    public GameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType)
    {
        bool flag = false;
        return GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out flag);
    }

    public GameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType, out bool isInit)
    {
        return GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out isInit);
    }

    public GameObject GetGameObject(string prefabFullPath, enResourceType resourceType)
    {
        bool flag = false;
        return GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out flag);
    }

    public GameObject GetGameObject(string prefabFullPath, enResourceType resourceType, out bool isInit)
    {
        return GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out isInit);
    }

    public T GetCachedComponent<T>(GameObject go, bool autoAdd = false) where T : Component
    {
        if (null == go)
        {
            return (T)((object)null);
        }
        Component component = null;
        if (m_componentMap.TryGetValue(go.GetInstanceID(), out component) && (!autoAdd || component != null))
        {
            return component as T;
        }
        component = go.GetComponent<T>();
        if (autoAdd && component == null)
        {
            component = go.AddComponent<T>();
        }
        m_componentMap[go.GetInstanceID()] = component;
        if (null == component)
        {
            return (T)((object)null);
        }
        return component as T;
    }

    private GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, out bool isInit)
    {
        string text = FileManager.EraseExtension(prefabFullPath);
        Queue<PooledGameObjectScript> queue = null;
        if (!m_pooledGameObjectMap.TryGetValue(text.JavaHashCodeIgnoreCase(), out queue))
        {
            queue = new Queue<PooledGameObjectScript>();
            m_pooledGameObjectMap.Add(text.JavaHashCodeIgnoreCase(), queue);
        }
        PooledGameObjectScript pooledGameObjectScript = null;
        while (queue.Count > 0)
        {
            pooledGameObjectScript = queue.Dequeue();
            if (pooledGameObjectScript != null && pooledGameObjectScript.gameObject != null)
            {
                pooledGameObjectScript.gameObject.transform.SetParent(null, true);
                pooledGameObjectScript.gameObject.transform.position = pos;
                pooledGameObjectScript.gameObject.transform.rotation = rot;
                pooledGameObjectScript.gameObject.transform.localScale = pooledGameObjectScript.defaultScale;
                break;
            }
            pooledGameObjectScript = null;
        }
        if (pooledGameObjectScript == null)
        {
            pooledGameObjectScript = CreateGameObject(prefabFullPath, pos, rot, useRotation, resourceType, text);
        }
        if (pooledGameObjectScript == null)
        {
            isInit = false;
            return null;
        }
        isInit = pooledGameObjectScript.isInit;
        pooledGameObjectScript.OnGet();
        return pooledGameObjectScript.gameObject;
    }

    public void RecycleGameObjectDelay(GameObject pooledGameObject, int delayMillSeconds, GameObjectPool.OnDelayRecycleDelegate callback = null)
    {
        GameObjectPool.stDelayRecycle stDelayRecycle = new GameObjectPool.stDelayRecycle();
        stDelayRecycle.recycleObj = pooledGameObject;
        stDelayRecycle.recycleTime = (int)(Time.time * 1000f) + delayMillSeconds;
        stDelayRecycle.callback = callback;
        if (m_delayRecycle.Count == 0)
        {
            m_delayRecycle.AddLast(stDelayRecycle);
            return;
        }
        for (LinkedListNode<GameObjectPool.stDelayRecycle> linkedListNode = m_delayRecycle.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
        {
            if (linkedListNode.Value.recycleTime < stDelayRecycle.recycleTime)
            {
                m_delayRecycle.AddAfter(linkedListNode, stDelayRecycle);
                return;
            }
        }
        m_delayRecycle.AddFirst(stDelayRecycle);
    }

    public void RecycleGameObject(GameObject pooledGameObject)
    {
        RecycleGameObject(pooledGameObject, false);
    }

    public void RecyclePreparedGameObject(GameObject pooledGameObject)
    {
        RecycleGameObject(pooledGameObject, true);
    }

    private void RecycleGameObject(GameObject pooledGameObject, bool setIsInit)
    {
        if (pooledGameObject == null)
        {
            return;
        }
        PooledGameObjectScript component = pooledGameObject.GetComponent<PooledGameObjectScript>();
        if (component != null)
        {
            Queue<PooledGameObjectScript> queue = null;
            if (this.m_pooledGameObjectMap.TryGetValue(component.prefabKey.JavaHashCodeIgnoreCase(), out queue))
            {
                queue.Enqueue(component);
                component.OnRecycle();
                component.transform.SetParent(this.m_poolRoot.transform, true);
                component.isInit = setIsInit;
                return;
            }
        }
        UnityEngine.Object.Destroy(pooledGameObject);
    }

    public void PrepareGameObject(string prefabFullPath, enResourceType resourceType, int amount, bool assertNull = true)
    {
        string text = FileManager.EraseExtension(prefabFullPath);
        Queue<PooledGameObjectScript> queue = null;
        if (!m_pooledGameObjectMap.TryGetValue(text.JavaHashCodeIgnoreCase(), out queue))
        {
            queue = new Queue<PooledGameObjectScript>();
            m_pooledGameObjectMap.Add(text.JavaHashCodeIgnoreCase(), queue);
        }
        if (queue.Count >= amount)
        {
            return;
        }
        amount -= queue.Count;
        for (int i = 0; i < amount; i++)
        {
            PooledGameObjectScript pooledGameObjectScript = CreateGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, text);
            if (assertNull)
            {
                //DebugHelper.Assert(cPooledGameObjectScript != null, "Failed Create Game object from \"{0}\"", new object[]
                //{
                //    prefabFullPath
                //});
            }
            if (pooledGameObjectScript != null)
            {
                queue.Enqueue(pooledGameObjectScript);
                pooledGameObjectScript.gameObject.transform.SetParent(m_poolRoot.transform, true);
                pooledGameObjectScript.OnPrepare();
            }
        }
    }

    private PooledGameObjectScript CreateGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, string prefabKey)
    {
        bool needCached = false;// resourceType == enResourceType.BattleScene;
        GameObject gameObject = Singleton<ResourceManager>.GetInstance().GetResource(prefabFullPath, typeof(GameObject), resourceType, needCached, false).content as GameObject;
        if (gameObject == null)
        {
            return null;
        }
        GameObject gameObject2;
        if (useRotation)
        {
            gameObject2 = (UnityEngine.GameObject.Instantiate(gameObject, pos, rot) as GameObject);
        }
        else
        {
            gameObject2 = (UnityEngine.GameObject.Instantiate(gameObject) as GameObject);
            gameObject2.transform.position = pos;
        }
       // DebugHelper.Assert(gameObject2 != null);
        PooledGameObjectScript pooledGameObjectScript = gameObject2.GetComponent<PooledGameObjectScript>();
        if (pooledGameObjectScript == null)
        {
            pooledGameObjectScript = gameObject2.AddComponent<PooledGameObjectScript>();
        }
        pooledGameObjectScript.Initialize(prefabKey);
        pooledGameObjectScript.OnCreate();
        return pooledGameObjectScript;
    }
}
