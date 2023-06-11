using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public enum POOL_OBJECT_TYPE {
        FireEXplosionParticle,

        RocketExplosionParticle,
        DroneExplosion,
        RepairDroneExplosionParticle,
        HumanoidExplosion,
        BasicProjectile,

        IncendearyProjectile,
        ElectricProjectile,
        ElectricParticleEffect,
        SmokeEffect,
        PistolAmmo,
        RifleAmmo,
        Grenade,
        BasicRocket,
        DroidExplosionParticleEffect,
        GlassParticleEffect,
        BloodSplatterEffect,
        DamageText,
        Obj_Indicator,
        SmallFireEffect,
        Granade_Explosion,

        AttackDroneExplosion,
    }

    // Basic Projectile
    public int maxBulletCount = 20;

    // Explosion


    public int maxExplosions = 10;
    public int donreExplosions = 10;

    public int GameIndicatorCount = 20;

    private static ProjectilePool thisProjectilePool;

    public int ammoCount = 10;

    public int minorPartilceCount = 5;


    private Dictionary<POOL_OBJECT_TYPE,List<GameObject>> prjectile_dict = new Dictionary<POOL_OBJECT_TYPE, List<GameObject>>();

    private GameObject m_player;

    #region initialize

    public void Awake()
    {
        foreach (POOL_OBJECT_TYPE type in System.Enum.GetValues(typeof(POOL_OBJECT_TYPE)))
        {
            initalziePool(type);
        }
    }
    void Start()
    {
        m_player = GameObject.FindObjectOfType<PlayerController>().gameObject;
    }

    public List<GameObject> get_list(POOL_OBJECT_TYPE type)
    {
        if (prjectile_dict.ContainsKey(type))
        {
            return prjectile_dict[type];
        }
        var newList = new List<GameObject>();
        prjectile_dict[type] = newList;
        return newList;
    }

    private void initalziePool(POOL_OBJECT_TYPE typeofEffect)
    {
        string resourcePath = "";
        List<GameObject> effectList = null;
        int count =0;

        effectList = get_list(typeofEffect);

        switch (typeofEffect)
        {
            case POOL_OBJECT_TYPE.FireEXplosionParticle:
                //resourcePath = "ParticleEffects/Explosion_fire";
                resourcePath = "ParticleEffects/Grenade_Explosive";
                count = maxExplosions;
                // basicFireExplosionParticlesList = new List<GameObject>();
                break;
            case POOL_OBJECT_TYPE.RocketExplosionParticle:
                //resourcePath = "ParticleEffects/Grenade_Explosive";
                resourcePath = "ParticleEffects/Grenade_Explosive";
                count = maxExplosions;
                break;
            case POOL_OBJECT_TYPE.DroneExplosion:
                resourcePath = "Explosions/BasicDroneExplosion";
                count = donreExplosions;
                break;
            case POOL_OBJECT_TYPE.RepairDroneExplosionParticle:
                resourcePath = "Explosions/RepairDroneExplosion";
                count = minorPartilceCount;
                break;    
            case POOL_OBJECT_TYPE.AttackDroneExplosion:
                resourcePath = "Explosions/AttackDroneExplosion";
                count = donreExplosions;
                break;
            case POOL_OBJECT_TYPE.HumanoidExplosion:
                count = maxExplosions;
                resourcePath = "ParticleEffects/BodyExplosion";
                break;
            case POOL_OBJECT_TYPE.BasicProjectile:
                count = maxBulletCount;
                resourcePath = "Prefab/RegularProjectile";
                break;
            case POOL_OBJECT_TYPE.IncendearyProjectile:
                count = maxBulletCount;
                resourcePath = "Prefab/FireProjectile";
                break;
            case POOL_OBJECT_TYPE.ElectricProjectile:
                count = maxBulletCount;
                resourcePath = "Prefab/ElectricProjectile";
                break;
            case POOL_OBJECT_TYPE.ElectricParticleEffect:
                count = maxExplosions;
                resourcePath = "ParticleEffects/ElectricShock";
                break;
            case POOL_OBJECT_TYPE.SmokeEffect:
                count = maxExplosions;
                resourcePath = "ParticleEffects/SmokeParticleEffect";
                break;
            case POOL_OBJECT_TYPE.RifleAmmo:
                count = ammoCount;
                resourcePath = "Drops/Rifle_Mag";
                break;
            case POOL_OBJECT_TYPE.PistolAmmo:
                count = ammoCount;
                resourcePath = "Drops/Pistol_Mag";
                break;
            case POOL_OBJECT_TYPE.Grenade:
                count = ammoCount;
                resourcePath = "Prefab/Grenede_throwObject";
                break;
            case POOL_OBJECT_TYPE.BasicRocket:
                count = ammoCount;
                resourcePath = "Prefab/BasicRocket";   
                break;
            case POOL_OBJECT_TYPE.DroidExplosionParticleEffect:
                resourcePath = "Explosions/BasicDroidExplosion";
                count = donreExplosions;
                break;
            case POOL_OBJECT_TYPE.GlassParticleEffect:
                resourcePath = "ParticleEffects/GlassBreak";
                count = minorPartilceCount;      
                break;
            case POOL_OBJECT_TYPE.BloodSplatterEffect:
                resourcePath = "ParticleEffects/BulletHitHbodyParticle";
                count = ammoCount;
                break;

            case POOL_OBJECT_TYPE.DamageText:
                resourcePath = "Prefab/DamageText";
                count = maxBulletCount;
                break;  
            case POOL_OBJECT_TYPE.Obj_Indicator:
                 resourcePath = "Prefab/Indicator_General";
                count = GameIndicatorCount;
                break;  
            case POOL_OBJECT_TYPE.Granade_Explosion:
                //resourcePath = "ParticleEffects/Explosion_fire";
                resourcePath = "ParticleEffects/Grenade_Explosive";
                count = maxExplosions;
                break;    

            case POOL_OBJECT_TYPE.SmallFireEffect:
                //resourcePath = "ParticleEffects/Explosion_fire";
                resourcePath = "ParticleEffects/FireEffect";
                count = maxExplosions;
                break;                     

        }

        GameObject bulletHitBasicParticlePrefab = Resources.Load<GameObject>(resourcePath);
        
        for (int i = 0; i < count; i++)
        {
            GameObject bulletHitParticle = GameObject.Instantiate(bulletHitBasicParticlePrefab);
            bulletHitParticle.AddComponent<PoolObject>();
            bulletHitParticle.GetComponent<PoolObject>().poolObjectList = effectList;
            bulletHitParticle.transform.parent = this.transform;
            bulletHitParticle.name = typeofEffect.ToString() + i.ToString();
            var basic_particle = bulletHitParticle.GetComponent<BasicParticleEffect>();

            if (basic_particle)
            {
                basic_particle.SetParent(this.transform);
            }
            bulletHitParticle.SetActive(false);
            effectList.Add(bulletHitParticle);
        }
    }

    #region Not Using
    #endregion

    #endregion


    #region getters and setters

    public GameObject getPoolObject(POOL_OBJECT_TYPE type)
    {
        List<GameObject> effectList = get_list(type);

        if (effectList.Count == 0)
        {
            initalziePool(type);
            Debug.Log("re-initializing pool");
        }

        foreach (GameObject projectile in effectList)
        {
            if (projectile == null)
            {

            }
            if (!projectile.activeInHierarchy)
            {
                return projectile;
            }
        }

        return null;
    }


    public static ProjectilePool getInstance()
    {
        if(thisProjectilePool ==null)
        {
            thisProjectilePool = GameObject.FindObjectOfType<ProjectilePool>();
        }

        return thisProjectilePool;
    }

    public GameObject getPlayer()
    {
        return m_player;
    }
    #endregion
}
