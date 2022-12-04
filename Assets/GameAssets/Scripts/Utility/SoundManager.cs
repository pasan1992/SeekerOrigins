using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioClip m_laserPistol;
    private AudioClip m_laserRifal;
    private AudioClip m_droneExplosion;
    private AudioClip m_emptyGun;
    private AudioClip m_buletHitMetal;

    [System.Serializable]
    public struct SoundEffect
    {
        public AudioClip[] sound;
        public string soundName;
    }

    public string pistolSoundFile;
    public string emptyGunSoundFile;
    public string rifleSoundFile;
    public string bulletHitMetal;
    private static SoundManager this_ins;
    private string sound_location ="Sounds/";


    public List<SoundEffect> SoundList = new List<SoundEffect>();
    private Dictionary<string,AudioClip[]> sound_dict = new Dictionary<string,AudioClip[]>(); 
    void Awake()
    {
        m_droneExplosion = Resources.Load<AudioClip>(sound_location+"/droneExplosion");
        m_emptyGun = Resources.Load<AudioClip>(sound_location + emptyGunSoundFile);

        foreach(SoundEffect s in SoundList)
        {
            sound_dict.Add(s.soundName,s.sound);
        }
    }

    public static SoundManager getInstance()
    {
        if(this_ins ==null)
        {
            this_ins = GameObject.FindObjectOfType<SoundManager>();
        }

        return this_ins;
    }

    public AudioClip getDroneExplosion()
    {
        return m_droneExplosion;
    }

    public AudioClip getEmptyGunSound()
    {
        return m_emptyGun;
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
