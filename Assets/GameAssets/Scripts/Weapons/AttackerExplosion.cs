using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerExplosion : BasicExplodingObject
{
    private DamagableObject m_selfDamagable;

    public void Start()
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

    protected override void hitOnEnemy(Collider other)
    {
        DamagableObject damagableObject =  other.GetComponentInParent<DamagableObject>();
        if(damagableObject == m_selfDamagable)
        {
            return;
        }
        base.hitOnEnemy(other);
    }
}
