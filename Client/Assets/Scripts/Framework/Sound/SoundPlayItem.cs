using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

//using DG.Tweening;

public class SoundPlayItem
{
    public const string SOUND_PATH = "Sound";

    public string currentPlaySoundName = "";

    private AudioSource source;    
    private float currentVol = 1;
    private float inOrOutTime = 0;
    private bool loop;
    private string soundType;
   // private Tweener tween;

    public SoundPlayItem(AudioSource source)
    {
        this.source = source;
    }

    public bool IsPlaying()
    {
        return source.enabled;
    }

    public void Update()
    {
        if (source.enabled && !source.loop && (source.clip == null || source.time >= source.clip.length))
        {
            Stop();
        }
    }

    //播放声音
    public void PlaySound(AudioClip clip, bool loop, float vol = 1, float inOrOutTime = 1f)
    {
        if (clip == null) return;

       // tween.Kill();
        currentPlaySoundName = clip.name;        
        source.enabled = true;
        source.clip = clip;
        source.loop = loop;
        source.Play();
        if (inOrOutTime > 0)
        {
          //  tween = DOTween.To(x => source.volume = x, source.volume, vol, inOrOutTime);
        }
        else
        {
            source.volume = vol;
        }
    }

    //播放声音，会根据type类型和名字自动去加载声音
    public void PlaySound(string soundType,string soundName,bool loop, float vol=1, float inOrOutTime = 1f)
    {
        soundName = soundName.ToLower();
        this.currentPlaySoundName = soundName;
        this.currentVol = vol;
        this.loop = loop;
        this.inOrOutTime = inOrOutTime;
        this.soundType = soundType;

        string path;
        ResourceBase resourceBase = null;
        if (soundType == SoundType.BGM)
        {
            path = "Sound/BGM/" + soundName;
        }
        else if (soundType == SoundType.SE_UI)
        {
            path = "Sound/UI/" + soundName;
        }
        else
        {
            path = "Sound/SE/" + MonoSingleton<GameLoader>.GetInstance().CurSceneName + "/" + soundName;
        }
        resourceBase = Singleton<ResourceManager>.GetInstance().GetResource(path, typeof(AudioClip), enResourceType.Sound, false, false);

        if (resourceBase != null)
        {
            PlaySound(resourceBase.content as AudioClip, loop, currentVol, inOrOutTime);
        }
    }

    public void Stop(float inOrOutTime)
    {
        if (inOrOutTime > 0)
        {
         //   tween = DOTween.To(x => source.volume = x, source.volume, 0, inOrOutTime);
          //  tween.OnComplete(Stop);
        }
        else
        {
            Stop();
        }
    }

    public void Stop()
    {
      //  tween.Kill();
        source.Stop();
        source.clip = null;
        source.enabled = false;

        currentPlaySoundName = "";
        currentVol = 1;
        inOrOutTime = 0;
    }
}
