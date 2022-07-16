using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public List<AudioClip> SFXAudioClips;
    public List<AudioClip> MusicAudioClips;
    
    public enum SFXType
    {
        Button = 0,
        StartGame = 1,
        Wick = 2,
        Explosion = 3,
        Cannon = 4,
        TurretShot = 5
    }

    public enum MusicType
    {
        Background = 0,
        Wind = 1
    }
    
    public void SetSfxVolume(AudioSource SFXAudioSources, float value)
    {
        SFXAudioSources.volume = value;
    }
    
    public void SetMusicVolume(AudioSource MusicAudioSources, float value)
    {
        MusicAudioSources.volume = value;
    }
    
    public void PlaySfx(AudioSource SFXAudioSource, SFXType type)
    {
        float sfxVolume = GameManager.instance.playerPrefsManager.GetFloat(PlayerPrefsManager.PlayerPrefsKeys.sfx, 0.5f);
        SFXAudioSource.clip = SFXAudioClips[(int)type];
        SFXAudioSource.volume = sfxVolume;
        SFXAudioSource.Play();
    }

    public void PlayMusic(AudioSource MusicAudioSource, MusicType type, bool islooping)
    {
        float musicVolume = GameManager.instance.playerPrefsManager.GetFloat(PlayerPrefsManager.PlayerPrefsKeys.music, 0.3f);
        MusicAudioSource.Stop();
        MusicAudioSource.clip = MusicAudioClips[(int)type];
        MusicAudioSource.volume = musicVolume;
        MusicAudioSource.Play();
        if (islooping)
        {
            StopCoroutine(LoopWaiter(MusicAudioSource.clip.length, MusicAudioSource, type));   
        }
    }

    IEnumerator LoopWaiter(float time, AudioSource MusicAudioSources, MusicType type)
    {
        yield return new WaitForSeconds(time);
        PlayMusic(MusicAudioSources, type, true);
    }
}
