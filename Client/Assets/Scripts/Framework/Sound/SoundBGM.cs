using UnityEngine;
using System.Collections;
using System.IO;

//两个SoundPlayItem，是为了在切换背景音乐的时候，有的渐隐和渐现的过程。

public class SoundBGM : MonoBehaviour
{
    private SoundPlayItem soundItem1;
    private SoundPlayItem soundItem2;
    private SoundPlayItem mainSoundItem;


    void Start()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.volume = SoundManager.BGM_VOLUME;
        source.enabled = false;
        source.loop = true;
        soundItem1 = new SoundPlayItem(source);

        source = gameObject.AddComponent<AudioSource>();
        source.volume = SoundManager.BGM_VOLUME;
        source.enabled = false;
        source.loop = true;
        soundItem2 = new SoundPlayItem(source);
    }
    void Update()
    {
        if (soundItem1 != null)
        {
            soundItem1.Update();
        }
        if (soundItem2 != null)
        {
            soundItem2.Update();
        }
    }
    public void PlaySound(AudioClip clip, float vol=1, float inTime = 1f)
    {
        if (mainSoundItem != null && mainSoundItem.currentPlaySoundName == clip.name)
        {
            return;
        }
        if (mainSoundItem == soundItem1)
        {
            mainSoundItem = soundItem2;
            soundItem1.Stop(inTime);
            soundItem2.PlaySound(clip, true, SoundManager.BGM_VOLUME * vol, inTime);
        }
        else
        {
            mainSoundItem = soundItem1;
            soundItem1.PlaySound(clip, true, SoundManager.BGM_VOLUME * vol, inTime);
            soundItem2.Stop(inTime);
        }
    }
    public void PlaySound(string soundName, float vol = 1, float inTime = 0)
    {
        if (mainSoundItem != null && mainSoundItem.currentPlaySoundName == soundName)
        {
            return;
        }
        if (mainSoundItem == soundItem1)
        {
            mainSoundItem = soundItem2;
            soundItem1.Stop(inTime);
            soundItem2.PlaySound(SoundType.BGM, soundName,true ,SoundManager.BGM_VOLUME * vol, inTime);
        }
        else
        {
            mainSoundItem = soundItem1;
            soundItem2.Stop();
            soundItem1.PlaySound(SoundType.BGM, soundName,true, SoundManager.BGM_VOLUME * vol, inTime);           
        }
    }
    
    //停止背景声音
    public void Stop(float outTime)
    {
        soundItem1.Stop(outTime);
        soundItem2.Stop(outTime);
        mainSoundItem = null;
    }

    public void Stop()
    {
        soundItem1.Stop();
        soundItem2.Stop();
        mainSoundItem = null;
    }
    void OnDestroy()
    {
        if (soundItem1 != null)
        {
            soundItem1.Stop();
        }

        if (soundItem2 != null)
        {
            soundItem2.Stop();
        }
        mainSoundItem = null;
        soundItem1 = null;
        soundItem2 = null;
    }
}
