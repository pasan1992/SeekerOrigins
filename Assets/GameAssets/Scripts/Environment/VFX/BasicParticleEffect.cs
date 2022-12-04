using UnityEngine;

public class BasicParticleEffect : MonoBehaviour
{
    ParticleSystem m_selfParticleSystem;
    private Transform m_parent;

    public float ResetTime = 1.5f;
    public bool defaultOnEnable = true;

    public string ParticleSound = "";
    private AudioSource m_baseAudioSource;

    private void Awake()
    {
        m_selfParticleSystem = this.GetComponent<ParticleSystem>();

        if(ParticleSound !="")
        {
            m_baseAudioSource = this.GetComponent<AudioSource>();
            if(m_baseAudioSource == null)
            {
                this.gameObject.AddComponent<AudioSource>();
                m_baseAudioSource = this.GetComponent<AudioSource>();
            }
        }

    }

    private void OnEnable()
    {
        if(defaultOnEnable)
        {
            _onEnable(ResetTime);
            CommonFunctions.PlaySound(ParticleSound,m_baseAudioSource);
        }
    }

    public void _onEnable(float restTime)
    {
        m_selfParticleSystem.Play();
        this.Invoke("resetAll",restTime);


    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void resetAll()
    {
        this.transform.parent = m_parent;
        m_selfParticleSystem.Stop();
        this.gameObject.SetActive(false);
    }

    public void SetParent(Transform parent)
    {
        m_parent = parent;
    }
}
