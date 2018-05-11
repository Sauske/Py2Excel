using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using TableProto;
using UnityEngine.SceneManagement;

public class GameLoader : MonoSingleton<GameLoader>
{
    public delegate void LoadCompleteDelegate();

    public bool isLoadStart;

    private int m_resCount = 0;

    private int m_curResCount = 0;

    private string m_curSceneName;

    public Dictionary<string, int> m_scenePartInfos = new Dictionary<string, int>();

    public List<string> m_soundInfos = new List<string>();

    private GameLoader.LoadCompleteDelegate m_loadCompleteEvent;

    private Coroutine m_handle_CoroutineLoad;

    private Coroutine m_scenePart_CoroutineLoad;

    private Coroutine m_sound_CoroutineLoad;

    private AsyncOperation m_asyncOperation;

    public string CurSceneName
    {
        get
        {
            return m_curSceneName;
        }
    }

    public void SceneResInfos(string sceneName)
    {
        m_curSceneName = sceneName;
        switch (sceneName)
        {
            case "HomeSafe":
            {
                GameDataMgr.chapterInfoDatabin.Accept(new Action<chapter_info>(ScenePartData));
                GameDataMgr.soundInfoDatabin.Accept(new Action<sound_info>(SceneSoundData));
                break;
            }
            case "Main":
            {
                break;
            }
        }
    }

    public void ScenePartData(chapter_info chapterInfoData)
    {
        if (chapterInfoData == null) return;

        int count = chapterInfoData.sprite.Count;

        for (int i = 0;i < count; i++)
        {
            sprite_info spriteInfoData = GameDataMgr.spriteInfoDatabin.GetDataByKey(chapterInfoData.sprite[i]);
            if (spriteInfoData == null) continue;

            string path = spriteInfoData.path + spriteInfoData.name;

            if (m_scenePartInfos.ContainsKey(path))
            {
                m_scenePartInfos[path]++;
            }
            else
            {
                m_scenePartInfos.Add(path, 1);
            }
            
        }
       
        m_resCount++;
    }


    public void SceneSoundData(sound_info soundInfoData)
    {
        if (soundInfoData == null) return;
        if (soundInfoData.affiliation_scene == 2 || soundInfoData.affiliation_scene == 0)
        {
            string path = soundInfoData.path + "/" + soundInfoData.name;
            if (!m_soundInfos.Contains(path))
            {
                m_soundInfos.Add(path);
            }
        }
        m_resCount++;
    }

    public void ResetLoader()
    {
        if (isLoadStart)
        {
            if (m_handle_CoroutineLoad != null)
                StopCoroutine(m_handle_CoroutineLoad);
            if (m_scenePart_CoroutineLoad != null)
                StopCoroutine(m_scenePart_CoroutineLoad);
            if (m_sound_CoroutineLoad != null)
                StopCoroutine(m_sound_CoroutineLoad);
            isLoadStart = false;
        }
        m_resCount = 0;
        m_curResCount = 0;
        m_curSceneName = null;
        m_scenePartInfos.Clear();
        m_soundInfos.Clear();
    }

    public void Load(GameLoader.LoadCompleteDelegate finish)
    {
        if (isLoadStart)
        {
            return;
        }
        isLoadStart = true;

        m_loadCompleteEvent = finish;

        m_handle_CoroutineLoad = StartCoroutine(CoroutineLoad());
    }

    private IEnumerator CoroutineLoad()
    {
        Singleton<UIManager>.GetInstance().CloseAllForm();

        Singleton<GameObjectPool>.GetInstance().ClearPooledObjects();

        enResourceType[] resourceTypes = new enResourceType[5];
        resourceTypes[1] = enResourceType.ScenePrefab;
        resourceTypes[2] = enResourceType.UIForm;
        resourceTypes[3] = enResourceType.UIPrefab;
        resourceTypes[4] = enResourceType.UISprite;
        Singleton<ResourceManager>.GetInstance().RemoveCachedResources(resourceTypes);
        yield return new WaitForEndOfFrame();

        Singleton<ResourceLoader>.GetInstance().LoadScene("Empty",null);

        yield return null;

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        yield return new WaitForEndOfFrame();

        m_asyncOperation = SceneManager.LoadSceneAsync(m_curSceneName);

        yield return m_asyncOperation;

        yield return m_scenePart_CoroutineLoad = StartCoroutine(LoadScenePart());

        yield return m_sound_CoroutineLoad = StartCoroutine(LoadSound());

        if (m_loadCompleteEvent != null)
        {
            m_asyncOperation.allowSceneActivation = true;
            m_loadCompleteEvent();
          
        }
    }

    public IEnumerator LoadScenePart()
    {
        foreach (KeyValuePair<string, int> kvp in m_scenePartInfos)
        {
            Singleton<GameObjectPool>.GetInstance().PrepareGameObject(kvp.Key, enResourceType.ScenePrefab, kvp.Value);

            m_curResCount += kvp.Value;

            if (m_curResCount % 3 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent<float>(EventID.LOADING_UPDATE_PROGRESS, (float)m_curResCount / (float)m_resCount);
        }
    }

    public IEnumerator LoadSound()
    {
        yield return null;
        for (int i = 0; i < m_soundInfos.Count; ++i)
        {
            Singleton<SoundManager>.GetInstance().AddSoundEffect(m_soundInfos[i]);

            m_curResCount += 1;

       //     yield return new WaitForEndOfFrame();

            Singleton<EventRouter>.GetInstance().BroadCastEvent<float>(EventID.LOADING_UPDATE_PROGRESS, (float)m_curResCount / (float)m_resCount);
        }
    }
}
