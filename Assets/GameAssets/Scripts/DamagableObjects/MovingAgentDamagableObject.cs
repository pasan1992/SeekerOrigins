using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class MovingAgentDamagableObject : MonoBehaviour,DamagableObject
{
    protected ICyberAgent m_movingAgent;

    protected GameEvents.BasicNotifactionEvent onDamagedEvent;
    protected GameEvents.BasicNotifactionEvent onDestroyedEvent;
    protected bool destroyedEventCalled = false;
    public ProjectilePool.POOL_OBJECT_TYPE particleEffectOnDamage = ProjectilePool.POOL_OBJECT_TYPE.HitBasicParticle;
    protected AudioSource m_audioSource;
    protected SoundManager m_soundManager;
    protected ObjectUI m_objectUI;

    public void Awake()
    {
        m_movingAgent = this.GetComponent<ICyberAgent>();
        m_audioSource = this.GetComponent<AudioSource>();
        m_soundManager = GameObject.FindObjectOfType<SoundManager>();
        m_objectUI = this.GetComponent<ObjectUI>();
    }
    public virtual bool damage(float damageValue,Collider collider, Vector3 force, Vector3 point, AgentBasicData.AgentFaction fromFaction)
    {
        /*
        if (!GamePlayCam.IsVisibleToCamera(m_movingAgent.getTransfrom()))
        {

            Debug.Log("Not damaging");
            return false;
        }*/

        m_movingAgent.damageAgent(damageValue);
        
        // If functional after damaged, then retrun false and fire on damaged event
        if(m_movingAgent.IsFunctional())
        {
            if(onDamagedEvent !=null)
            {
                onDamagedEvent();
            }
            
            return false;
        }

        // Not functional - If the Object is destroyed 
        if (!destroyedEventCalled & getRemaningHealth() == 0)
        {           
            destroyedEventCalled = true;

            if(onDestroyedEvent !=null)
            {
                onDestroyedEvent();
            }
           StartCoroutine(waitAndDestory(2));
            
        }  
        return true;  
    }

    public virtual float getArmor()
    {
        return 0;
    }

    public virtual float getRemaningHealth()
    {
       return m_movingAgent.GetAgentData().Health;
    }

    public virtual float getTotalHealth()
    {
        return m_movingAgent.GetAgentData().MaxHealth;
    }

    public virtual Transform getTransfrom()
    {
        return m_movingAgent.getTransfrom();
    }

    public virtual bool isDestroyed()
    {
       return !m_movingAgent.IsFunctional();
    }

    public virtual bool isDamagable(AgentBasicData.AgentFaction fromFaction)
    {
        var notFromPlayer = fromFaction != AgentBasicData.AgentFaction.Player;
        var inScreen = GamePlayCam.IsVisibleToCamera(m_movingAgent.getTransfrom());
        return (m_movingAgent.getFaction() != fromFaction) && (inScreen | notFromPlayer);
    }

    public void setOnDamaged(GameEvents.BasicNotifactionEvent onDamaged)
    {
        onDamagedEvent +=onDamaged;
    }

    public void setOnDestroyed(GameEvents.BasicNotifactionEvent onDestroyed)
    {
        onDestroyedEvent += onDestroyed;
    }

    public void resetObject()
    {
        destroyedEventCalled= false;
        // Reset Health logic
        m_movingAgent.GetAgentData().Health = m_movingAgent.GetAgentData().MaxHealth;
    }

    public void damage_effect(Transform hit_transfrom)
    {
        /*
        GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(particleEffectOnDamage);
        if (basicHitParticle == null)
        {
            Debug.LogWarning("Not enough particle effects");
            return;
        }
        basicHitParticle.SetActive(true);
        basicHitParticle.transform.position = hit_transfrom.position;
        basicHitParticle.transform.LookAt(Vector3.up);
        */

    }

    protected IEnumerator waitAndDestory(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

}
