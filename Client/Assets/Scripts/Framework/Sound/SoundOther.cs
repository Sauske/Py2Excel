using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//最多同时播放12个音效

public class SoundOther:MonoBehaviour
{

    private int sourceCount = 12;

    private List<SoundPlayItem> soundPlayItems = new List<SoundPlayItem>();

    void Start()
    {
        for (int i = 0; i < sourceCount; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = SoundManager.AMB_VOLUME;
            source.enabled = false;
            source.loop = true;
            SoundPlayItem soundItem = new SoundPlayItem(source);
            soundPlayItems.Add(soundItem);
        }
    }

    void Update()
    {
        for (int i = 0; i < soundPlayItems.Count; i++)
        {
            soundPlayItems[i].Update();
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1, bool isLoop = false)
    {
        Stop();

        for (int i = 0; i < soundPlayItems.Count; i++)
        {
            if (!soundPlayItems[i].IsPlaying())
            {
                soundPlayItems[i].PlaySound(clip, isLoop, volume, 0);
                break;
            }
        }
    }

    public void PlaySound(string soundType, string soundName, float volume = 1, bool isLoop = false)
    {
        Stop();

        for (int i = 0; i < soundPlayItems.Count; i++)
        {
            if (!soundPlayItems[i].IsPlaying())
            {
                soundPlayItems[i].PlaySound(soundType, soundName, isLoop, volume, 0);
                break;
            }
        }
    }

    public void Stop()
    {
        for (int i = 0; i < soundPlayItems.Count; i++)
        {
            if (soundPlayItems[i].IsPlaying())
            {
                soundPlayItems[i].Stop();
                break;
            }
        }
    }
    public void Stop(string soundName)
    {
        for (int i = 0; i < soundPlayItems.Count; i++)
        {
            if (soundPlayItems[i].currentPlaySoundName == soundName)
            {
                soundPlayItems[i].Stop();
                break;
            }
        }
    }

    void OnDestroy()
    {
        Stop();
        soundPlayItems.Clear();
        soundPlayItems = null;
    }

}
