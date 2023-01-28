using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon
{
    private GameObject m_tempGrenede;
    private bool m_pinPulled = false;
    public int count = 5;

    [System.Serializable]
    public class GrenadeType
    {
        public GrenadeType(ProjectilePool.POOL_OBJECT_TYPE objectType,float damage,string name)
        {
            this.damage = damage;
            this.objectType = objectType;
            this.name = name;
        }
        public ProjectilePool.POOL_OBJECT_TYPE objectType;
        public float damage;

        public string name;
    }

    public IDictionary<string,GrenadeType> GrenadeTypes;
    private GrenadeType currentGrenadeType;

    public void Awake()
    {
        base.Awake();
        GrenadeTypes = new Dictionary<string,GrenadeType>();
        GrenadeTypes.Add(AmmoTypeEnums.Grenade.Regular_Grenade.ToString(),new GrenadeType(ProjectilePool.POOL_OBJECT_TYPE.Grenade,40,AmmoTypeEnums.Grenade.Regular_Grenade.ToString()));
        currentGrenadeType = GrenadeTypes[AmmoTypeEnums.Grenade.Regular_Grenade.ToString()];
    }

    public void getGrenateCount()
    {
        
    }

    public override WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.grenede;
    }

    public void pullGrenedePin()
    {
        // if (count == 0)
        //     return;
        // if (count < 0)
        //     Debug.LogError("GRENATE COUNT LESS THANT ZERO");
        // count -= 1;



        m_tempGrenede = ProjectilePool.getInstance().getPoolObject(currentGrenadeType.objectType);
        m_tempGrenede.SetActive(true);

        // var basic_timer = m_tempGrenede.GetComponent<BasicTimerExplodingObject>();
        // basic_timer.resetAll();
        // basic_timer.BaseDamage = damage;
        // Rigidbody rb = m_tempGrenede.GetComponent<Rigidbody>();
        // rb.isKinematic = true;
        // rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        // m_tempGrenede.transform.parent = this.transform;
        // m_tempGrenede.transform.localPosition =Vector3.zero;
        // basic_timer.startCountDown();
        // m_pinPulled = true;


        m_pinPulled = true;


        // reset rigidbody
        Rigidbody rb = m_tempGrenede.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // reset transform
        m_tempGrenede.transform.parent = this.transform;
        m_tempGrenede.transform.localPosition =Vector3.zero;

        // reset exploding object
        var baseGrenade = m_tempGrenede.GetComponent<BasicExplodingObject>();
        baseGrenade.BaseDamage = currentGrenadeType.damage;
        baseGrenade.activateExplosionMechanisum();

    }

    public void ThrowGrenede()
    {
        if(m_pinPulled)
        {
            //throwGenade();
        }

    }

    public void ThrowGrenadeQuick(Transform location)
    {
            pullGrenedePin();
            throwGenade(location);
    }

    private void throwGenade(Transform location)
    {
        m_pinPulled = false;
        m_tempGrenede.transform.parent = null;
        Rigidbody rb = m_tempGrenede.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        m_tempGrenede.transform.position = location.position;
        Vector3 throwVeclocity = calculateThrowVelocity(m_target.transform.position - this.transform.position - Vector3.up*1.5f);
        m_tempGrenede.GetComponent<Rigidbody>().AddForce(throwVeclocity,ForceMode.VelocityChange);

        // Temparary Disable Grenade
        nonFunctionalProperties.magazineObjProp.SetActive(false);
        Invoke("EnableGrenadeProp",0.5f);
        

        // if(count <0)
        // {
        //     Debug.LogError("G count less than zero");
        // }
    }

    private void EnableGrenadeProp()
    {
        nonFunctionalProperties.magazineObjProp.SetActive(true);
    }

    private Vector3 calculateThrowVelocity(Vector3 relativePosition)
    {
        if(relativePosition.magnitude > 12)
        {
            relativePosition = relativePosition.normalized*12;
        }

        float throwTime = 1f;
        float X_velocity = relativePosition.x/throwTime;
        float Z_velocity = relativePosition.z/throwTime;
        float Y_velocity = (2*relativePosition.y + Physics.gravity.magnitude*throwTime*throwTime)/(2*throwTime);
        return new Vector3(X_velocity,Y_velocity,Z_velocity);
    }

    public bool isPinPulled()
    {
        return m_pinPulled;
    }

    public string getCurrentGrenateType()
    {
        return currentGrenadeType.name;
    }

    public void SwitchGrenadeType(string type)
    {
        currentGrenadeType = GrenadeTypes[type];
    }
}
