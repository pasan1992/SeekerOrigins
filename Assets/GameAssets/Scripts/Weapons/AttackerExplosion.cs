using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerExplosion : BasicTimerExplodingObject
{
    protected DamagableObject m_selfDamagable;
    public bool damageSelf = true;
    private bool exploded = false;

    public bool enableTimerFromStart;

    public virtual void Start()
    {
        m_selfDamagable = this.GetComponent<DamagableObject>();
    }


    public override void explode(ProjectilePool.POOL_OBJECT_TYPE explosionType)
    {
       GameObject explostion = ProjectilePool.getInstance().getPoolObject(explosionType);
       explostion.transform.position = this.transform.position;
       explostion.SetActive(true);
       damgeAround();
    }

    public virtual void performAttack(float duration,Vector3 direction)
    {
        if(!exploded)
        {
            exploded = true;
            activateExplosionMechanisum();
        }

    }

    protected override void hitOnEnemy(Collider other)
    {
        if(!damageSelf)
        {
            DamagableObject damagableObject =  other.GetComponentInParent<DamagableObject>();
            if(damagableObject == m_selfDamagable)
            {
                return;
            }
        }

        base.hitOnEnemy(other);
    }
}
