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
    public ProjectilePool.POOL_OBJECT_TYPE particleEffectOnDamage = ProjectilePool.POOL_OBJECT_TYPE.SmokeEffect;
    protected AudioSource m_audioSource;
    protected SoundManager m_soundManager;
    protected ObjectUI m_objectUI;
    
    public bool KeepOnDestory = false;


    public Outline m_outline;

    private GameObject fireEffect;
    private int dotCount = 0;

    public UnityEvent m_onDestroyEvent;
    private Color m_original_outline;

    public void Awake()
    {
        m_movingAgent = this.GetComponent<ICyberAgent>();
        m_audioSource = this.GetComponent<AudioSource>();
        m_soundManager = GameObject.FindObjectOfType<SoundManager>();
        m_objectUI = this.GetComponent<ObjectUI>();
        if(m_outline == null)
        {
            m_outline = this.GetComponentInChildren<Outline>();
        }
        
        if(m_outline)
        m_original_outline = m_outline.OutlineColor;
        
    }

    public void Start()
    {
            StartCoroutine(waitAndEnable());
    }

    IEnumerator waitAndEnable()
    {
        if(m_outline)
        {
            m_outline.OutlineWidth = 0.3f;
            yield return new WaitForSeconds(5);
            m_outline.OutlineWidth = 0.3f;
            Debug.Log("HERE");
        }
        

    }

    protected IEnumerator onDamageEffect()
    {
        if(m_outline)
        {
            m_outline.OutlineColor = Color.red;
            yield return new WaitForSeconds(0.5f);
            m_outline.OutlineColor = m_original_outline;
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

    public virtual bool damage(CommonFunctions.Damage damageValue,Collider collider, Vector3 force, Vector3 point, AgentBasicData.AgentFaction fromFaction ,float stunPrecentage = 0)
    {
        /*
        if (!GamePlayCam.IsVisibleToCamera(m_movingAgent.getTransfrom()))
        {

            Debug.Log("Not damaging");
            return false;
        }*/

        // if (stunPrecentage > Random.value)
        // {
        //     //StartCoroutine(DotDamage(damageValue,collider,force,point,fromFaction,(int)stunPrecentage));
        //     //this.m_movingAgent.setStunned(3);
        // }

        m_movingAgent.damageAgent(damageValue);
        StartCoroutine(onDamageEffect());
        
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

    public void Stun(CommonFunctions.Damage damage, float duration,Vector3 direction)
    {
        if(damage.energyDamage > damage.healthDamage && m_movingAgent.GetAgentData().AgentNature == AgentBasicData.AGENT_NATURE.DROID &&
        Random.value > 0.5f)
        {
            m_movingAgent.setShocked(4,direction);
            return;
        }
        m_movingAgent.setStunned(duration,direction);
    }

    protected IEnumerator waitAndDestory(float time)
    {
        if(!KeepOnDestory)
        {
            yield return new WaitForSeconds(time);
            CommonFunctions.ResetParticles(this.gameObject);
            SetFireEffect(false,null);
            Destroy(this.gameObject);


        }

    }

    
    protected void SetFireEffect(bool stats,Transform location)
    {

        if (stats)
        {
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
            fireEffect.GetComponent<BasicParticleEffect>().resetAll();
            fireEffect = null;
            }

        }

    }

}
