using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static bool muted = false;
    public static AudioManager Instance;
    public List<AudioSource> audioSources = new List<AudioSource>();
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        if(muted == false)
        {
            print("Audio Played");
            AllAudios(true);
        }
        else
        {
            print("Audio Stopped");
            AllAudios(false);
        }
    }
    public void AddAudio(AudioSource audioSource)
    {
        audioSources.Add(audioSource);
        if (muted == false)
        {
            print("Audio Played");
            audioSource.mute = false;
        }
        else
        {
            print("Audio Stopped");
            audioSource.mute = true;
        }
    }
    public void StopAudio()
    {
        print("Stop sound");
        AllAudios(false);
    }
    public void PlaySound()
    {
        if (muted == false)
        {
            AllAudios(true);
        }
        else
        {
            AllAudios(false);
        }
    }
    public void enableSound()
    {
        muted = false;
        print("Sound enabled");
        AllAudios(true);
    }
    public void disableSound()
    {
        muted = true;
        print("Sound disabled");
        AllAudios(false);

    }
    private void AllAudios(bool status)
    {
        foreach (AudioSource audio in audioSources)
        {
            if(audio !=null)
            {
                if (!status)
                {
                    audio.Pause();
                }
                else { 
                    audio.UnPause();
                }
            }
            else
            {
                audioSources.Remove(audio);
            }
            
        }
        print("Current Audio status" + status + audioSources.Count);
    }
}
