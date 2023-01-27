using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton pattern
    public static AudioManager instance;

    // Audio sources
    public GameObject sfxSourcePf;
    public AudioSource musicSource;
    public AudioLowPassFilter musicLowPass;

    // Audio clips
    public CustomizableAudioClip[] sfxClips;
    public AudioClip[] musicClips;

    private bool muteSFX = false;

    void Awake()
    {
        // Implementaci�n del patr�n singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // M�todo para reproducir un efecto de sonido
    public void PlaySFX(string clipName)
    {
        if(muteSFX) return;
        // Buscar el clip de audio en el array de clips
        CustomizableAudioClip customAudioClip = Array.Find(sfxClips, audio => audio.clip.name == clipName);

        // Comprobar si se ha encontrado el clip
        if (customAudioClip != null)
        {
            // Reproducir el clip
            GameObject audioSource = Instantiate(sfxSourcePf, transform.position, Quaternion.identity, transform);
            audioSource.name = "SFX (" + clipName +")";
            AudioSource source = audioSource.GetComponent<AudioSource>();
            source.volume = customAudioClip.volume;
            source.PlayOneShot(customAudioClip.clip);
            Destroy(audioSource, customAudioClip.clip.length);
        }
        else
        {
            Debug.LogError("Clip de audio no encontrado: " + clipName);
        }
    }

    public void MuteSFX(bool mute)
    {
        muteSFX = mute;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (muteSFX) return;
        GameObject audioSource = Instantiate(sfxSourcePf, transform.position, Quaternion.identity, transform);
        audioSource.name = "SFX (" + clip.name + ")";
        audioSource.GetComponent<AudioSource>().PlayOneShot(clip);
        Destroy(audioSource, clip.length);
    }

    // M�todo para reproducir m�sica
    public void PlayMusic(string clipName)
    {
        // Buscar el clip de audio en el array de clips
        AudioClip clip = Array.Find(musicClips, audio => audio.name == clipName);

        // Comprobar si se ha encontrado el clip
        if (clip != null)
        {
            // Reproducir el clip
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogError("Clip de audio no encontrado: " + clipName);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void MuteMusic(bool value)
    {
        musicSource.mute = value;
    }

    public void SetCutOffFrequencyMusic(float value)
    {
        musicLowPass.cutoffFrequency = value;
    }

    public bool IsSFXMuted()
    {
        return muteSFX;
    }

    public bool IsMusicMuted()
    {
        return musicSource.mute;
    }
}

[System.Serializable]
public class CustomizableAudioClip
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1;
}