using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPackExplosionObject : BasicExplodingObject
{
    private BasicRocket[] m_rockets;
    private Queue<BasicRocket> rockets = new Queue<BasicRocket>();
    private HashSet<DamagableObject> attacked_value = new HashSet<DamagableObject>();
    public AgentBasicData.AgentFaction faction;
    void Start()
    {
        m_rockets = GetComponentsInChildren<BasicRocket>();
        foreach(BasicRocket rocket in m_rockets)
        {
            rockets.Enqueue(rocket);
        }
    }

    // Update is called once per frame
    public override void explode(ProjectilePool.POOL_OBJECT_TYPE explosionType)
    {
       //GameObject explostion = ProjectilePool.getInstance().getPoolObject(explosionType);
       //explostion.transform.position = this.transform.position;
       //explostion.SetActive(true);
       fireAllRockets();
    }

    private void fireAllRockets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, m_range);
            foreach (Collider hitCollider in hitColliders)
            {
                // var explodingObj = hitCollider.GetComponentInParent<BasicExplodingObject>();
                // if (explodingObj !=null && explodingObj !=this)
                // {
                //     explodingObj.explode();
                // }
                if(rockets.Count == 0)
                {
                    break;
                }
                
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
        
        for(int i =0;i<rockets.Count ;i++)
        {
            var random_location = new Vector3(Random.Range(5,-5),Random.Range(0,2),Random.Range(-5,5));
            var rocket = rockets.Dequeue();
            rocket.transform.parent = null;
            rocket.fireRocketLocation(random_location+ this.transform.position);
        }

    }

    protected override void hitOnEnemy(Collider other)
    {
        
      DamagableObject damagableObject =  other.GetComponentInParent<DamagableObject>();
      var agent = other.GetComponentInParent<ICyberAgent>();
      if(agent == null || agent.getFaction() == faction || !agent.IsFunctional())
      {
        return;
      }


      Vector3 direction;
      float damagePropotion = DamageCalculator.getExplosionDamgage(this.transform.position,other.transform.position,m_range,out direction);
      if(damagePropotion > 0 && damagableObject !=null && !attacked_value.Contains(damagableObject))
      {
        attacked_value.Add(damagableObject);
        var rocket = rockets.Dequeue();
        rocket.fireRocketTransfrom(damagableObject.getTransfrom());
        rocket.transform.parent = null;
      }
    }
}
