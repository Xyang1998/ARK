using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : SingletonMono<AudioSystem>
{
    public AudioSource voiceSource;
    public AudioSource bgmSource;
    private float bgmLength;
    
    public void PlayVoice(AudioClip clip)
    {
        if (clip != null)
        {
            if (!voiceSource.isPlaying)
            {
                voiceSource.clip = clip;
                voiceSource.Play();
            }
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip != null)
        {
            if (!bgmSource.isPlaying)
            {
                //TODO:修改为设置的值
                bgmSource.volume = 1.0f;
                bgmSource.loop = true;
                bgmSource.clip = clip;
                bgmLength = clip.length;
                bgmSource.Play();
            }
        }
    }

    public void FixedUpdate()
    {
        //TODO:需要优化
        if (bgmSource.clip)
        {
            if (bgmSource.time > bgmLength - 5)
            {
                float t=Mathf.Abs((bgmSource.time-bgmLength+5)/5);
                bgmSource.volume = 1 - t;
            }
            else
            {
                bgmSource.volume = 1.0f;
            }
        }
    }
}
