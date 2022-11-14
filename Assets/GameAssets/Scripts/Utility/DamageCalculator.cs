using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    public class DamageFuture
    {
        public DamageFuture(Collider other, AgentBasicData.AgentFaction m_fireFrom, CommonFunctions.Damage damage, float dot_time, DamageLocation dl, Vector3 hitpoint,Vector3 hitDirection)
        {
            this.other = other;
            this.m_fireFrom = m_fireFrom;
            this.damage = damage;
            this.dot_time = dot_time;
            this.damageLocation = dl;
            this.hitpoint = hitpoint;
            this.hitDirection = hitDirection;
        }
        public Collider other;
        public AgentBasicData.AgentFaction m_fireFrom;
        public Vector3 hitDirection;
        public CommonFunctions.Damage damage;
        public  float dot_time=0;

        public enum DamageLocation {Wall,Enemy,Item};
        public Vector3 hitpoint;

        public DamageLocation damageLocation;
    }
    private static readonly float s_maximumHitReactionValue = 2.5f;
    // public static void SetDamageFromExplosion(ICyberAgent agent,BasicExplodingObject explosionObject,Collider hitObject)
    // {
    //     Vector3 forceDirection = (agent.getCurrentPosition() - explosionObject.transform.position);
    //     float distance = forceDirection.magnitude;
    //     forceDirection = forceDirection.normalized;

    //     if(distance < explosionObject.Range)
    //     {
    //         float damageProposion =  (1- (distance/explosionObject.Range));
    //         agent.damageAgent(explosionObject.BaseDamage*damageProposion);
    //         agent.reactOnHit(hitObject, forceDirection*s_maximumHitReactionValue*damageProposion,hitObject.transform.position);
    //     }
    // }

    public static float getExplosionDamgage(Vector3 explosionPositon,Vector3 targetPosition,float explosionMaxRange , out Vector3 direction)
    {
        
        direction = targetPosition - explosionPositon;
        float distance = direction.magnitude;
        direction = direction.normalized;

        if(distance < explosionMaxRange)
        {
            //return  (1- (distance/explosionMaxRange));
            return 1;
        }

        return 0;
               
    }

    public static bool isSafeFromTarget(Vector3 explosionPosition, Vector3 target, float range)
    {
        RaycastHit hit;
        string[] layerMaskNames = { "HalfCoverObsticles","FullCoverObsticles" };
        if (Physics.Raycast(explosionPosition, target - explosionPosition, out hit,range, LayerMask.GetMask(layerMaskNames)))
        {
            if(hit.transform.tag =="Cover" || hit.transform.tag == "Wall")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public static void hitOnWall(Collider wall,Vector3 hitPositon)
    {
            /*
            GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.HitBasicParticle);
            if(basicHitParticle !=null)
            {
                basicHitParticle.SetActive(true);
                basicHitParticle.transform.position = hitPositon;
                basicHitParticle.transform.LookAt(Vector3.up);
            }
            */

    }
    // public static void onHitEnemy(Collider other,AgentBasicData.AgentFaction m_fireFrom,Vector3 hitDirection)
    // {
    //     AgentController agentController = other.transform.GetComponentInParent<AgentController>();
    //     if (agentController != null)
    //     {
    //         ICyberAgent cyberAgent = agentController.getICyberAgent();
    //         if (cyberAgent !=null && !m_fireFrom.Equals(cyberAgent.getFaction()))
    //         {

    //             //cyberAgent.reactOnHit(other, (hitDirection) * 3f, other.transform.position);
    //             //cyberAgent.damageAgent(1);
    //             agentController.GetComponent<HumanoidDamagableObject>().damage(1,other,(hitDirection) * 3f,other.transform.position);
            
    //             GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.HitBasicParticle);
    //             basicHitParticle.SetActive(true);
    //             basicHitParticle.transform.position = other.transform.position;
    //             basicHitParticle.transform.LookAt(Vector3.up);
 
    //             if (!cyberAgent.IsFunctional())
    //             {
    //                 HumanoidMovingAgent movingAgent = cyberAgent as HumanoidMovingAgent;
    //                 if(movingAgent !=null)
    //                 {
    //                     Rigidbody rb = other.transform.GetComponent<Rigidbody>();

    //                     if (rb != null)
    //                     {
    //                         rb.isKinematic = false;
    //                         rb.AddForce((hitDirection) * 150, ForceMode.Impulse);
    //                     }

    //                     Rigidbody hitRb =  movingAgent.getChestTransfrom().GetComponent<Rigidbody>();

    //                     if(hitRb)
    //                     {
    //                         hitRb.AddForce((hitDirection) * 2 + Random.insideUnitSphere*2, ForceMode.Impulse);
    //                     }

    //                 }
    //                 else
    //                 {
    //                    basicHitParticle.transform.position = cyberAgent.getTopPosition();
    //                 }
    //             }
    //         }

    //     }
    // }

    public static void onHitDamagableItem(Collider other,AgentBasicData.AgentFaction m_fireFrom,Vector3 hitDirection)
    {
        DamagableObject damagableObject = other.transform.GetComponentInParent<DamagableObject>();
        
        if(damagableObject != null)
        {
            damagableObject.damage(new CommonFunctions.Damage(1,0),other,hitDirection,other.transform.position,m_fireFrom);
        }
    }

    public static void onHitEnemy(Collider other,AgentBasicData.AgentFaction m_fireFrom,Vector3 hitDirection,CommonFunctions.Damage damage,float stunPrecentage=0)
    {
        if(other == null)
        {
            return;
        }
        DamagableObject damagableObject = other.transform.GetComponentInParent<DamagableObject>();
        if (damagableObject != null)
        {
            MovingAgentDamagableObject movingDamagableObject = (MovingAgentDamagableObject)damagableObject;
            if (movingDamagableObject !=null && movingDamagableObject.isDamagable(m_fireFrom))
            {

               // cyberAgent.reactOnHit(other, (hitDirection) * 3f, other.transform.position);
                //cyberAgent.damageAgent(1);
               movingDamagableObject.damage(damage, other,(hitDirection) * 1f,other.transform.position,m_fireFrom,stunPrecentage);
            
                //GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.HitBasicParticle);
                //basicHitParticle.SetActive(true);
                //basicHitParticle.transform.position = other.transform.position;
                //basicHitParticle.transform.LookAt(Vector3.up);
                damagableObject.damage_effect(other.transform);
 
                if (movingDamagableObject.isDestroyed())
                {
                    Rigidbody rb = other.transform.GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.AddForce((hitDirection) * 150, ForceMode.Impulse);
                    }
                }
            }

        }
    }

    public static RaycastHit 
    checkFire(Vector3 startPositon, Vector3 targetPositon, AgentBasicData.AgentFaction ownersFaction,CommonFunctions.Damage weapon_damage,ProjectileMover pmover,float dot_time=0)
    {
        RaycastHit hit = new RaycastHit();
        RaycastHit acualHit = new RaycastHit();
        Vector3 hitPos = Vector3.zero;
        string[] layerMaskNames = { "HalfCoverObsticles","FullCoverObsticles","Enemy","Floor", "IgnoreNavMesh","Environment"};
        bool hitOnEnemy = false;

        // To stop firing through walls when too close
        if (checkIfFireThroughWall(startPositon, targetPositon,out acualHit))
        {
            return acualHit;
        }

        // Give offset to starting postion to avoid bullets colliding in own covers
        Vector3 offsetTargetPositon =  (targetPositon - startPositon).normalized + startPositon;
        if (Physics.Raycast(offsetTargetPositon, targetPositon - startPositon, out hit,100, LayerMask.GetMask(layerMaskNames)))
        {
            acualHit = hit;
            switch(hit.transform.tag)
            {
                case "Floor":
                    acualHit.point = Vector3.zero;
                break;
                case "Cover":
                case "Wall":
                //DamageCalculator.hitOnWall(hit.collider,hit.point);
                pmover.damageFuture = new DamageCalculator.DamageFuture(hit.collider,ownersFaction,null,0,DamageFuture.DamageLocation.Wall,hit.point,Vector3.zero);
                break;
                case "Enemy":
                case "Player":
                case "Head":
                case "Chest":
                //DamageCalculator.onHitEnemy(hit.collider,ownersFaction,(targetPositon-startPositon).normalized,weapon_damage,dot_time);
                pmover.damageFuture = new DamageCalculator.DamageFuture(hit.collider,ownersFaction,weapon_damage,dot_time,DamageFuture.DamageLocation.Enemy,Vector3.zero,(targetPositon-startPositon).normalized);
                hitOnEnemy = true;
                break;
                case "Item":
                hitOnEnemy = true;
                //DamageCalculator.onHitDamagableItem(hit.collider,ownersFaction,(targetPositon-startPositon).normalized);
                pmover.damageFuture = new DamageCalculator.DamageFuture(hit.collider,ownersFaction,weapon_damage,dot_time,DamageFuture.DamageLocation.Item,hit.point,(targetPositon-startPositon).normalized);
                break;       
            } 

        }
        
        bool importantHit = false;
        for (int i =-3; i <3;i++)
        {
            // Check fire for the second time for find crouched enemies.
            if(!hitOnEnemy && Physics.Raycast(offsetTargetPositon, targetPositon + new Vector3(0,i/3,0) - startPositon, out hit,100, LayerMask.GetMask(layerMaskNames)))
            {
                switch(hit.transform.tag)
                {
                    case "Wall":
                        //DamageCalculator.hitOnWall(hit.collider,hit.point);
                        pmover.damageFuture = new DamageCalculator.DamageFuture(hit.collider,ownersFaction,weapon_damage,dot_time,DamageFuture.DamageLocation.Wall,Vector3.zero,(targetPositon-startPositon).normalized);
                        acualHit = hit;
                    break;
                    case "Cover":
                        //DamageCalculator.hitOnWall(hit.collider,hit.point);
                        pmover.damageFuture = new DamageCalculator.DamageFuture(hit.collider,ownersFaction,weapon_damage,dot_time,DamageFuture.DamageLocation.Wall,Vector3.zero,(targetPositon-startPositon).normalized);
                        importantHit = true;
                        acualHit = hit;
                    break;
                    case "Enemy":
                    case "Player":
                    case "Head":
                    case "Chest":
                        //DamageCalculator.onHitEnemy(hit.collider,ownersFaction,(targetPositon-startPositon).normalized, weapon_damage,dot_time);
                        pmover.damageFuture = new DamageCalculator.DamageFuture(hit.collider,ownersFaction,weapon_damage,dot_time,DamageFuture.DamageLocation.Enemy,Vector3.zero,(targetPositon-startPositon).normalized);
                        importantHit = true;
                        acualHit = hit;
                    break; 
                    case "Floor":
                        acualHit = hit;
                        acualHit.point = Vector3.zero;
                    break;
                }                 
            }
            if(importantHit)
            {
                break;
            }
        }
        return acualHit;
        
    }

    public static bool checkIfFireThroughWall(Vector3 startPositon, Vector3 targetPositon, out RaycastHit actualHit)
    {
        // To stop firing through walls when too close
        RaycastHit hit;
        // Can't shoot through these layered objects when too close
        string[] layerMaskNames = { "HalfCoverObsticles", "FullCoverObsticles", "Enemy", "Floor","IgnoreNavMesh","Environment" };

        // Give offset to starting postion to avoid bullets colliding in own covers
        Vector3 offsetTargetPositon = startPositon - (targetPositon - startPositon).normalized/2;
        if (Physics.Raycast(offsetTargetPositon, targetPositon - startPositon, out hit, 2, LayerMask.GetMask(layerMaskNames)))
        {
            switch (hit.transform.tag)
            {
                
                case "Wall":
                case "Environment":
                case "IgnoreNavMesh":
                    actualHit = hit;
                    return true;  
            }
        }
        actualHit = new RaycastHit();
        return false;
    }
}
