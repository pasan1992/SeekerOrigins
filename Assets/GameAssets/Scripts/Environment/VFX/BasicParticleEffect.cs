using UnityEngine;

public class BasicParticleEffect : MonoBehaviour
{
    ParticleSystem m_selfParticleSystem;
    private Transform m_parent;

    private void Awake()
    {
        m_selfParticleSystem = this.GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        m_selfParticleSystem.Play();
        this.Invoke("resetAll",1.5f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void resetAll()
    {
        this.transform.parent = m_parent;
        m_selfParticleSystem.Stop();
        this.gameObject.SetActive(false);
        Debug.Log(m_parent);
        
    }

    public void SetParent(Transform parent)
    {
        m_parent = parent;
    }
}
