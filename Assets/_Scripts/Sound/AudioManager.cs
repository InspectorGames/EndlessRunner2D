using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton pattern
    public static AudioManager instance;

    // Audio sources
    public AudioSource sfxSource;
    public AudioSource musicSource;

    // Audio clips
    public AudioClip[] sfxClips;
    public AudioClip[] musicClips;

    void Awake()
    {
        // Implementación del patrón singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para reproducir un efecto de sonido
    public void PlaySFX(string clipName)
    {
        // Buscar el clip de audio en el array de clips
        AudioClip clip = Array.Find(sfxClips, audio => audio.name == clipName);

        // Comprobar si se ha encontrado el clip
        if (clip != null)
        {
            // Reproducir el clip
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("Clip de audio no encontrado: " + clipName);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        // Reproducir el clip
        sfxSource.PlayOneShot(clip);
    }

    // Método para reproducir música
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
}
