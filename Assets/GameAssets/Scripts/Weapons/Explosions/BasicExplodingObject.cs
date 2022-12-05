using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicExplodingObject : MonoBehaviour
{
     [SerializeField] 
    protected float m_baseDamage;
    public float m_energyDaamge;

     [SerializeField] 
    protected float m_range = 7;

    public float BaseDamage { get => m_baseDamage; set => m_baseDamage = value; }
    public float Range { get => m_range; set => m_range = value; }

    public ProjectilePool.POOL_OBJECT_TYPE explosionType = ProjectilePool.POOL_OBJECT_TYPE.FireEXplosionParticle;

    private AudioSource m_audioSource;

    public void explode()
    {
        explode(explosionType);
    }   

    public virtual void explode(ProjectilePool.POOL_OBJECT_TYPE explosionType)
    {
       GameObject explostion = ProjectilePool.getInstance().getPoolObject(explosionType);
       explostion.transform.position = this.transform.position;
       explostion.SetActive(true);
       damgeAround();
       this.gameObject.SetActive(false);
    }

    public virtual void activateExplosionMechanisum()
    {
        
    }

    protected void damgeAround()
    {

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, m_range);
         

        for(int i =0; i<5 ;i++)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                // var explodingObj = hitCollider.GetComponentInParent<BasicExplodingObject>();
                // if (explodingObj !=null && explodingObj !=this)
                // {
                //     explodingObj.explode();
                // }
                
                switch (hitCollider.tag)
                {
                    case "Enemy":
                    case "Player":
                    case "Head":
                    case "Chest":
                        hitOnEnemy(hitCollider);
                        break;
                    case "Wall":
                        break;
                    case "Cover":
                        break;
                    case "Floor":

                        break;
                }
            }
        }

    }


    protected virtual void hitOnEnemy(Collider other)
    {
        
      DamagableObject damagableObject =  other.GetComponentInParent<DamagableObject>();
      Vector3 direction;
      float damagePropotion = DamageCalculator.getExplosionDamgage(this.transform.position,other.transform.position,m_range,out direction);
      if(damagePropotion > 0 && damagableObject !=null)
      {
        if(damagableObject.isDestroyed())
        { 


            Rigidbody rb = other.GetComponent<Rigidbody>();
            
            float chance = Random.value;

            if(rb && chance >0.5f)
            {
                rb.AddForce(direction*damagePropotion*BaseDamage*10,ForceMode.Impulse);
            }
        }

        if(other.tag =="Chest")
        {   
            if(!DamageCalculator.isSafeFromTarget(this.transform.position,other.transform.position,m_range))
            {

                // Neeed to improve it exploding objects must not have faction
                damagableObject.damage(new CommonFunctions.Damage(m_baseDamage*damagePropotion,m_energyDaamge*damagePropotion),other,direction,other.transform.position,AgentBasicData.AgentFaction.Neutral);
                var mvdamage = (MovingAgentDamagableObject)damagableObject;
                if(mvdamage !=null)
                {
                    mvdamage.Stun(3f,other.transform.position - this.transform.position);
                }

            }           
        }
      }
    }
}
