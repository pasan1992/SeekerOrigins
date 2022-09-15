using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioClip m_laserPistol;
    private AudioClip m_laserRifal;
    private AudioClip m_droneExplosion;
    private AudioClip m_emptyGun;
    private AudioClip m_buletHitMetal;



    public string pistolSoundFile;
    public string emptyGunSoundFile;
    public string rifleSoundFile;
    public string bulletHitMetal;

    void Awake()
    {
        m_laserPistol = Resources.Load<AudioClip>("Sounds/" + pistolSoundFile);
        m_laserRifal = Resources.Load<AudioClip>("Sounds/" + rifleSoundFile);
        m_droneExplosion = Resources.Load<AudioClip>("Sounds/droneExplosion");
        m_emptyGun = Resources.Load<AudioClip>("Sounds/" + emptyGunSoundFile);
        m_buletHitMetal = Resources.Load<AudioClip>("Sounds/" + bulletHitMetal);
    }

    public AudioClip getLaserPistolAudioClip()
    {
        return m_laserPistol;
    }

    public AudioClip getLaserRifalAudioClip()
    {
        return m_laserRifal;
    }

    public AudioClip getDroneExplosion()
    {
        return m_droneExplosion;
    }

    public AudioClip getEmptyGunSound()
    {
        return m_emptyGun;
    }

    public AudioClip getBulletHitMetal()
    {
        return m_buletHitMetal;
    }
}
