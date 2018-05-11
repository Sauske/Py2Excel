using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using TableProto;

public class SoundManager : Singleton<SoundManager>
{
    //全局开关
    public static bool IsPlayBGM = true;
    //全局开关
    public static bool IsPlaySE = true;

    //如果此路径不为空就加载本地资源(千万不要动，编辑器代码会 O来更改这句)
    public static float BGM_VOLUME = 1f;
    public static float AMB_VOLUME = 1f;
    public static float OTHER_VOLUME = 0.7f;


    private Dictionary<string, AudioClip> m_seSoundClips = new Dictionary<string, AudioClip>();

    //bgm声音播放器
    public SoundBGM bgmSound;
 
    //SE声音播放器
    public SoundOther otherSound;

    private GameObject m_soundRoot;

    public override void Init()
    {
        base.Init();

        m_soundRoot = new GameObject("SoundManager");
        GameObject go = GameObject.Find("BootObj");
        if (go != null)
        {
            m_soundRoot.transform.parent = go.transform;
        }

        GameObject bgmObj = new GameObject();
        bgmObj.name = "SoundBGM";
        bgmObj.transform.parent = m_soundRoot.transform;
        bgmSound = bgmObj.AddComponent<SoundBGM>();

        GameObject otherObj = new GameObject();
        otherObj.name = "SoundOther";
        otherObj.transform.parent = m_soundRoot.transform;
        otherSound = otherObj.AddComponent<SoundOther>();
    }

    public override void UnInit()
    {
        base.UnInit();
    }

    public float GetLength(int resId)
    {
        sound_info info = GameDataMgr.soundInfoDatabin.GetDataByKey(resId);
        if (info == null)
        {
            Debug.Log("声音配置表里没有配置: " + resId);
            return 0;
        }
        return GetLength(SoundType.SE_MAIN, info.name);
    }

    public float GetLength(string soundType, string name)
    {
        AudioClip clip = null;
        if (m_seSoundClips.TryGetValue(name,out clip))
        {
            return clip.length;
        }
        else
        {
            string path = "";
            if (soundType == SoundType.BGM)
            {
                path = "Sound/BGM/" + name;
            }
            else if (soundType == SoundType.SE_UI)
            {
                path = "Sound/UI/" + name;
            }
            else
            {
                path = "Sound/SE/" + MonoSingleton<GameLoader>.GetInstance().CurSceneName + "/" + name;
            }
            ResourceBase resourceBase = Singleton<ResourceManager>.GetInstance().GetResource(path, typeof(AudioClip), enResourceType.Sound, false, false);

            return (resourceBase.content as AudioClip).length;
        }
    }

    public void AddSoundEffect(string path)
    {
        ResourceBase resourceBase = Singleton<ResourceManager>.GetInstance().GetResource(path, typeof(AudioClip), enResourceType.Sound, false, false);
        string name = resourceBase.Name;
        if (!m_seSoundClips.ContainsKey(name))
        {
            m_seSoundClips.Add(name, resourceBase.content as AudioClip);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundName">背景音乐名字</param>
    /// <param name="vol">音量大小</param>
    /// <returns></returns>
    public bool PlayBGM(string soundName, float vol = 1)
    {
        if (string.IsNullOrEmpty(soundName) || !IsPlayBGM)
        {
            return false;
        }
        return Play(soundName, SoundType.BGM, vol);
    }

    public bool PlayBGM(string soundName,bool loop = false,float vol = 1)
    {
        if (string.IsNullOrEmpty(soundName) || !IsPlayBGM)
        {
            return false;
        }
        return Play(soundName, SoundType.BGM, vol, loop);
    }

    public bool PlaySE(int resId)
    {
        if (!IsPlaySE)
        {
            return false;
        }

        sound_info info = GameDataMgr.soundInfoDatabin.GetDataByKey(resId);
        if(info == null)
        {
            Debug.Log("声音配置表里没有配置: " + resId);
            return false;
        }

        return Play(info.name,SoundType.SE_MAIN);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="soundType">音效名</param>
    /// <param name="soundName">声音类型SoundType</param>
    /// <param name="isAutoLoad">如果没有加载到缓存是否加载完后进行播放</param>
    /// <returns></returns>
    public bool PlaySE(string soundType, string soundName, float volume = 1, bool isLoop = false)
    {
        if (!IsPlaySE)
        {
            return false;
        }
        if (string.IsNullOrEmpty(soundType) || string.IsNullOrEmpty(soundName))
        {
            return false;
        }
        return Play(soundName, soundType, volume, isLoop);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="soundType"></param>
    /// <param name="soundNames"></param>
    /// <param name="rateList"></param>
    /// <param name="isAutoLoad"></param>
    /// <returns></returns>
    public bool PlaySE(string[] soundTypes, string[] soundNames, string[] rateList = null, float volume = 1)
    {
        if (!IsPlaySE)
        {
            return false;
        }
        if (soundNames.Length != soundTypes.Length && soundNames.Length > 0)
        {
            return false;
        }
        string soundName = "";
        string soundType = "";
        if (rateList == null || rateList.Length != soundNames.Length)
        {
            soundName = soundNames[0];
            soundType = soundTypes[0];
        }
        else if (soundNames.Length == 1)
        {
            if (string.IsNullOrEmpty(rateList[0]))
            {
                soundName = soundNames[0];
                soundType = soundTypes[0];
            }
            else
            {
                int rate = int.Parse(rateList[0]);
                if (rate != 0)
                {
                    if (rate >= Random.Range(1, 101))
                    {
                        soundName = soundNames[0];
                        soundType = soundTypes[0];
                    }
                }
                else
                {
                    soundName = soundNames[0];
                    soundType = soundTypes[0];
                }
            }
        }
        else
        {
            int range = 0;
            int[] rates = new int[rateList.Length];
            for (int i = 0; i < rateList.Length; i++)
            {
                if (string.IsNullOrEmpty(rateList[i]))
                {
                    rates[i] = 0;
                }
                else
                {
                    rates[i] = int.Parse(rateList[i]);
                }
                range += rates[i];
            }
            int rate = Random.Range(0, range > 100 ? range : 100);
            for (int i = rates.Length - 1; i >= 0; i--)
            {
                range -= rates[i];
                if (rate >= range)
                {
                    soundName = soundNames[i];
                    soundType = soundTypes[i];
                    break;
                }
            }
        }
        return Play(soundName, soundType, volume);
    }

    /// <summary>
    /// 根据声音名称，清除声音clip
    /// </summary>
    /// <param name="soundType"></param>
    /// <param name="soundName">如果声音为空，清除此类型所有的声音</param>
    public void UnLoadSound(string soundName)
    {
        if (!m_seSoundClips.ContainsKey(soundName)) return;
        
        m_seSoundClips.Remove(soundName);
    }

    public void StopAllSound()
    {
        bgmSound.Stop();
        otherSound.Stop();
    }

    public void StopOtherSound()
    {
        //bgmSound.Stop();
        otherSound.Stop();
    }

    public void StopBgmSound()
    {
        bgmSound.Stop();
    }

    /// <summary>
    /// 清除所有声音clip
    /// </summary>
    public void UnLoadAllSound()
    {
        StopAllSound();
        m_seSoundClips.Clear();
    }

    private bool Play(string soundName, string soundType, float volume = 1, bool isLoop = false)
    {
        if (volume <= 0)
        {
            return true;
        }
        AudioClip clip = null;
        if (soundType == SoundType.BGM)
        {
            bgmSound.PlaySound(soundName, volume);
        }
        else
        {
            if (m_seSoundClips.ContainsKey(soundName))
            {
                clip = m_seSoundClips[soundName];
                otherSound.PlaySound(clip, volume);
            }
            else
            {
                otherSound.PlaySound(soundType,soundName, volume, isLoop);
                return false;
            }
        }
        return true;
    }
}