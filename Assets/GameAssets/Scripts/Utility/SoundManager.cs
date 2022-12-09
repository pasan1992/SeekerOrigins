using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [System.Serializable]
    public struct SoundEffect
    {
        public AudioClip[] sound;
        public string soundName;
    }

    private static SoundManager this_ins;

    private bool dialogPlaying;


    public List<SoundEffect> SoundList = new List<SoundEffect>();
    private Dictionary<string,AudioClip[]> sound_dict = new Dictionary<string,AudioClip[]>(); 
    void Awake()
    {

        foreach(SoundEffect s in SoundList)
        {
            sound_dict.Add(s.soundName,s.sound);
        }
    }

    public void setDialogPlaying(bool isPlaying)
    {
        dialogPlaying = isPlaying;
    }

    public static SoundManager getInstance()
    {
        if(this_ins ==null)
        {
            this_ins = GameObject.FindObjectOfType<SoundManager>();
        }

        return this_ins;
    }
    public void setAudioVolume(AudioSource audioSource)
    {
        if(dialogPlaying)
        {
            audioSource.volume = 0.1f;
        }
        else
        {
            audioSource.volume = 0.8f;
        }
    }

    public AudioClip getSound(string soundName)
    {
        if(sound_dict.ContainsKey(soundName))
        {
            var clips = sound_dict[soundName];
            return clips[Random.Range(0,clips.GetLength(0))];
        }
        return null;
    }
}
