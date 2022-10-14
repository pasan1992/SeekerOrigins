using UnityEngine;

public class BasicParticleEffect : MonoBehaviour
{
    ParticleSystem m_selfParticleSystem;
    private Transform m_parent;

    public float ResetTime = 1.5f;
    public bool defaultOnEnable = true;

    private void Awake()
    {
        m_selfParticleSystem = this.GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        if(defaultOnEnable)
        {
            _onEnable(ResetTime);
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
