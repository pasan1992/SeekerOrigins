using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class CreatureDamagableObject : MonoBehaviour, DamagableObject
{
    public float Total_Health;
    private float m_remaning_Health;
    public ProjectilePool.POOL_OBJECT_TYPE particleEffectOnDestroy;
    public Color color;

    private GameEvents.BasicNotifactionEvent onDamagedEvent;
    private GameEvents.BasicNotifactionEvent onDestroyedEvent;

    // This is to make sure the on destroyed event won't get called multiple times.
    protected bool m_ondestroyEventCalled = false;
    private AudioSource m_audioSource;

    public UnityEvent m_onDestroyEvent;

    public void Awake()
    {
        m_remaning_Health = Total_Health;
        m_audioSource = this.GetComponent<AudioSource>();
    }
    public bool damage(CommonFunctions.Damage damageValue, Collider collider, Vector3 force, Vector3 point, AgentBasicData.AgentFaction fromFaction ,float dot_time = 0)
    {
        if (!isDestroyed())
        {
            m_remaning_Health -= damageValue.healthDamage;

            if (m_remaning_Health <= 0)
            {
                m_remaning_Health = 0;
                GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(particleEffectOnDestroy);
                if (basicHitParticle)
                {
                    basicHitParticle.SetActive(true);
                    basicHitParticle.transform.position = point;
                    basicHitParticle.transform.LookAt(Vector3.up);
                    basicHitParticle.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    //basicHitParticle.transform.localScale = new Vector3(1f,1f,1f);
                }


                // ParticleSystem.MainModule main = basicHitParticle.GetComponent<ParticleSystem>().main;
                // main.startColor = color; // <- or whatever color you want to assign
                // basicHitParticle.GetComponent<ParticleSystem>().Play();

                if (!m_ondestroyEventCalled)
                {
                    m_ondestroyEventCalled = true;
                    if (onDestroyedEvent != null)
                    {
                        onDestroyedEvent();
                    }
                    if (m_onDestroyEvent != null)
                    {
                        m_onDestroyEvent.Invoke();
                    }
                }
                //Destroy(this.gameObject);
                return true;
            }
            onDamagedEvent?.Invoke();
        }
        return false;
    }

    public float getArmor()
    {
        return 0;
    }

    public float getRemaningHealth()
    {
        return m_remaning_Health;
    }

    public float getTotalHealth()
    {
        return Total_Health;
    }

    public Transform getTransfrom()
    {
        return this.transform;
    }

    public bool isDestroyed()
    {
        return m_remaning_Health == 0;
    }

    public void setOnDamaged(GameEvents.BasicNotifactionEvent onDamaged)
    {
        this.onDamagedEvent += onDamaged;
    }

    public void setOnDestroyed(GameEvents.BasicNotifactionEvent onDestroyed)
    {
        this.onDestroyedEvent += onDestroyed;
    }

    public void resetObject()
    {
        m_ondestroyEventCalled = true;
        m_remaning_Health = Total_Health;
    }

    public void damage_effect(Transform hit_transfrom)
    {
        GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BloodSplatterEffect);
        basicHitParticle.SetActive(true);
        basicHitParticle.transform.position = hit_transfrom.position;
        basicHitParticle.transform.LookAt(Vector3.up);
    }
}
