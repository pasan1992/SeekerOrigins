using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    
    public bool KeepOnDestory = false;


    private Outline m_outline;

    private GameObject fireEffect;
    private int dotCount = 0;

    public UnityEvent m_onDestroyEvent;

    public void Awake()
    {
        m_movingAgent = this.GetComponent<ICyberAgent>();
        m_audioSource = this.GetComponent<AudioSource>();
        m_soundManager = GameObject.FindObjectOfType<SoundManager>();
        m_objectUI = this.GetComponent<ObjectUI>();
        m_outline = this.GetComponentInChildren<Outline>();
        StartCoroutine(waitAndEnable());
    }

    IEnumerator waitAndEnable()
    {
        if(m_outline)
        {
            m_outline.enabled = false;
            yield return new WaitForSeconds(6);
            m_outline.enabled = true;
        }
        

    }

    public IEnumerator DotDamage(CommonFunctions.Damage damageValue,Collider collider, Vector3 force, Vector3 point, AgentBasicData.AgentFaction fromFaction,int duration)
    {
        float damage_frequancy_seconds = 0.5f;
        float damage_count = duration / damage_frequancy_seconds;
        damageValue.healthDamage = damageValue.healthDamage / damage_count;
        damageValue.energyDamage = damageValue.energyDamage / damage_count;


        if(dotCount == 0)
        {
            SetFireEffect(true,collider.transform);
        }
        dotCount +=1;
        

        for (int i=0;i<damage_count; i++)
        {
            this.damage(damageValue,collider,force,point,fromFaction,0);
            yield return new WaitForSeconds(damage_frequancy_seconds);
        }


        dotCount -=1;
        if(dotCount == 0)
        {
            SetFireEffect(false,null);
        }
    }

    public virtual bool damage(CommonFunctions.Damage damageValue,Collider collider, Vector3 force, Vector3 point, AgentBasicData.AgentFaction fromFaction ,float dot_time = 0)
    {
        /*
        if (!GamePlayCam.IsVisibleToCamera(m_movingAgent.getTransfrom()))
        {

            Debug.Log("Not damaging");
            return false;
        }*/

        if (dot_time > 0)
        {
            StartCoroutine(DotDamage(damageValue,collider,force,point,fromFaction,(int)dot_time));
            return false;
        }

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


            if(m_onDestroyEvent !=null)
            {
                m_onDestroyEvent.Invoke();
            }
            else{
                StartCoroutine(waitAndDestory(2));
            }

            if(onDestroyedEvent !=null)
            {
                onDestroyedEvent();
            }

           
            
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

    public void Stun(float duration)
    {
        m_movingAgent.setStunned(duration);
    }

    protected IEnumerator waitAndDestory(float time)
    {
        if(!KeepOnDestory)
        {
            yield return new WaitForSeconds(time);
            SetFireEffect(false,null);
            Destroy(this.gameObject);


        }

    }

    
    private void SetFireEffect(bool stats,Transform location)
    {

        if (stats)
        {
        Debug.Log("making fire");
        GameObject fireEffet = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.SmallFireEffect);
        fireEffet.SetActive(true);
        fireEffet.transform.position = location.position;
        fireEffet.transform.parent = location.transform;  
        fireEffect = fireEffet; 
        }
        else
        {
            if(fireEffect !=null)
            {
            Debug.Log("removing fire");
            fireEffect.GetComponent<BasicParticleEffect>().resetAll();
            fireEffect = null;
            }

        }

    }

}
