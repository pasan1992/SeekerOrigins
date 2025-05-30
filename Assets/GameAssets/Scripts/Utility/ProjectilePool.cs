﻿using System.Collections;
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
    private List<GameObject> basicProjectilesList;

    private List<GameObject> IncendiaryProjectilesList;

    private List<GameObject> smallFireEffect;
    public int maxBulletCount = 20;

    // Explosions
    private List<GameObject> basicDroneExplosionList;
    private List<GameObject> repairDroneExplosionList;

    // Particle effects
    private List<GameObject> basicFireExplosionParticlesList;

    private List<GameObject> basicGrenadeExplosionParticlesList;

    private List<GameObject> basicRocketExplosionParticleList;
    private List<GameObject> bulletHitBasicParticleList;
    private List<GameObject> electricParticleEffectList;
    private List<GameObject> smokeEffectList;

    private List<GameObject> basicRocketList;

    private List<GameObject> droidExplosionsList;

    private List<GameObject> attackDroneExplosion;


    public int maxExplosions = 10;
    public int donreExplosions = 10;

    public int GameIndicatorCount = 5;

    private static ProjectilePool thisProjectilePool;

    public int ammoCount = 5;
    private List<GameObject> pistolAmmoList;
    private List<GameObject> rifleAmmoList;

    public List<GameObject> grenadeList;

    public int minorPartilceCount = 5;

    private List<GameObject> glassParticles;
    private List<GameObject> blood_splatter_particles;

    private List<GameObject> damage_text;

    private List<GameObject> inicator_list;

    private List<GameObject> electric_projectile;

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
        //initalizeBulletHitBasicParticleList();
        //initalizeBasicProjectile();
        //initalizeBasicExplosionParticle();
        //initalizeDroneExplosions();



        m_player = GameObject.FindObjectOfType<PlayerController>().gameObject;
    }

    private void initalziePool(POOL_OBJECT_TYPE typeofEffect)
    {
        string resourcePath = "";
        List<GameObject> effectList = null;
        int count =0;

        switch (typeofEffect)
        {
            case POOL_OBJECT_TYPE.FireEXplosionParticle:
                //resourcePath = "ParticleEffects/Explosion_fire";
                resourcePath = "ParticleEffects/Grenade_Explosive";
                count = maxExplosions;
                basicFireExplosionParticlesList = new List<GameObject>();
                effectList = basicFireExplosionParticlesList;
                break;
            case POOL_OBJECT_TYPE.RocketExplosionParticle:
                //resourcePath = "ParticleEffects/Grenade_Explosive";
                resourcePath = "ParticleEffects/Grenade_Explosive";
                count = maxExplosions;
                basicRocketExplosionParticleList = new List<GameObject>();
                effectList = basicRocketExplosionParticleList;
                break;
            case POOL_OBJECT_TYPE.DroneExplosion:
                resourcePath = "Explosions/BasicDroneExplosion";
                count = donreExplosions;
                basicDroneExplosionList = new List<GameObject>();
                effectList = basicDroneExplosionList;
                break;
            case POOL_OBJECT_TYPE.RepairDroneExplosionParticle:
                resourcePath = "Explosions/RepairDroneExplosion";
                count = minorPartilceCount;
                repairDroneExplosionList = new List<GameObject>();
                effectList = repairDroneExplosionList;
                break;    
            case POOL_OBJECT_TYPE.AttackDroneExplosion:
                resourcePath = "Explosions/AttackDroneExplosion";
                count = donreExplosions;
                attackDroneExplosion = new List<GameObject>();
                effectList = attackDroneExplosion;
                break;
            case POOL_OBJECT_TYPE.HumanoidExplosion:
                count = maxExplosions;
                resourcePath = "ParticleEffects/BodyExplosion";
                bulletHitBasicParticleList = new List<GameObject>();
                effectList = bulletHitBasicParticleList;
                break;
            case POOL_OBJECT_TYPE.BasicProjectile:
                count = maxBulletCount;
                resourcePath = "Prefab/RegularProjectile";
                basicProjectilesList = new List<GameObject>();
                effectList = basicProjectilesList;
                break;
            case POOL_OBJECT_TYPE.IncendearyProjectile:
                count = maxBulletCount;
                resourcePath = "Prefab/FireProjectile";
                IncendiaryProjectilesList = new List<GameObject>();
                effectList = IncendiaryProjectilesList;
                break;
            case POOL_OBJECT_TYPE.ElectricProjectile:
                count = maxBulletCount;
                resourcePath = "Prefab/ElectricProjectile";
                electric_projectile = new List<GameObject>();
                effectList = electric_projectile;
                break;
            case POOL_OBJECT_TYPE.ElectricParticleEffect:
                count = maxExplosions;
                resourcePath = "ParticleEffects/ElectricShock";
                electricParticleEffectList = new List<GameObject>();
                effectList = electricParticleEffectList;
                break;
            case POOL_OBJECT_TYPE.SmokeEffect:
                count = maxExplosions;
                resourcePath = "ParticleEffects/SmokeParticleEffect";
                smokeEffectList = new List<GameObject>();
                effectList = smokeEffectList;
                break;
            case POOL_OBJECT_TYPE.RifleAmmo:
                count = ammoCount;
                resourcePath = "Drops/Rifle_Mag";
                rifleAmmoList = new List<GameObject>();
                effectList = rifleAmmoList;
                break;
            case POOL_OBJECT_TYPE.PistolAmmo:
                count = ammoCount;
                resourcePath = "Drops/Pistol_Mag";
                pistolAmmoList = new List<GameObject>();
                effectList = pistolAmmoList;
                break;
            case POOL_OBJECT_TYPE.Grenade:
                count = ammoCount;
                resourcePath = "Prefab/Grenede_throwObject";
                grenadeList = new List<GameObject>();
                effectList = grenadeList;  
                break;
            case POOL_OBJECT_TYPE.BasicRocket:
                count = ammoCount;
                resourcePath = "Prefab/BasicRocket"; 
                basicRocketList = new List<GameObject>();
                effectList = basicRocketList;       
                break;
            case POOL_OBJECT_TYPE.DroidExplosionParticleEffect:
                resourcePath = "Explosions/BasicDroidExplosion";
                count = donreExplosions;
                droidExplosionsList = new List<GameObject>();
                effectList = droidExplosionsList;
                break;
            case POOL_OBJECT_TYPE.GlassParticleEffect:
                resourcePath = "ParticleEffects/GlassBreak";
                count = minorPartilceCount;
                glassParticles = new List<GameObject>();
                effectList = glassParticles;         
                break;
            case POOL_OBJECT_TYPE.BloodSplatterEffect:
                resourcePath = "ParticleEffects/BulletHitHbodyParticle";
                count = ammoCount;
                blood_splatter_particles = new List<GameObject>();
                effectList = blood_splatter_particles;
                break;

            case POOL_OBJECT_TYPE.DamageText:
                resourcePath = "Prefab/DamageText";
                count = maxBulletCount;
                damage_text = new List<GameObject>();
                effectList = damage_text;
                break;  
            case POOL_OBJECT_TYPE.Obj_Indicator:
                 resourcePath = "Prefab/Indicator_General";
                count = GameIndicatorCount;
                inicator_list = new List<GameObject>();
                effectList = inicator_list;
                break;  
            case POOL_OBJECT_TYPE.Granade_Explosion:
                //resourcePath = "ParticleEffects/Explosion_fire";
                resourcePath = "ParticleEffects/Grenade_Explosive";
                count = maxExplosions;
                basicGrenadeExplosionParticlesList = new List<GameObject>();
                effectList = basicGrenadeExplosionParticlesList;
                break;    

            case POOL_OBJECT_TYPE.SmallFireEffect:
                //resourcePath = "ParticleEffects/Explosion_fire";
                resourcePath = "ParticleEffects/FireEffect";
                count = maxExplosions;
                smallFireEffect = new List<GameObject>();
                effectList = smallFireEffect;
                break;                     

        }

        GameObject bulletHitBasicParticlePrefab = Resources.Load<GameObject>(resourcePath);
        
        for (int i = 0; i < count; i++)
        {
            GameObject bulletHitParticle = GameObject.Instantiate(bulletHitBasicParticlePrefab);
            bulletHitParticle.transform.parent = this.transform;
            var basic_particle = bulletHitParticle.GetComponent<BasicParticleEffect>();
            if (basic_particle)
            {
                basic_particle.SetParent(this.transform);
            }
            // if (bulletHitParticle is BasicParticleEffect)
            // {

            // }
            bulletHitParticle.SetActive(false);
            effectList.Add(bulletHitParticle);
        }
    }

    #region Not Using
    // private void initalizeBulletHitBasicParticleList()
    // {
    //     GameObject bulletHitBasicParticlePrefab = Resources.Load<GameObject>("ParticleEffects/BulletHitBasicParticle");
    //     bulletHitBasicParticleList = new List<GameObject>();

    //     for (int i = 0; i < maxBulletCount; i++)
    //     {
    //         GameObject bulletHitParticle = GameObject.Instantiate(bulletHitBasicParticlePrefab);
    //         bulletHitParticle.transform.parent = this.transform;
    //         bulletHitParticle.SetActive(false);
    //         bulletHitBasicParticleList.Add(bulletHitParticle);
    //     }
    // }

    // private void initalizeDroneExplosions()
    // {
    //     GameObject basicDroneExplosionPrefab = Resources.Load<GameObject>("Explosions/BasicDroneExplosion");
    //     basicDroneExplosionList = new List<GameObject>();

    //     for (int i = 0; i < donreExplosions; i++)
    //     {
    //         GameObject explosion = GameObject.Instantiate(basicDroneExplosionPrefab);
    //         explosion.transform.parent = this.transform;
    //         explosion.SetActive(false);
    //         basicDroneExplosionList.Add(explosion);
    //     }
    // }

    // private void initalizeBasicExplosionParticle()
    // {
    //     GameObject basicExplosionParticlePrefab = Resources.Load<GameObject>("ParticleEffects/Explosion_fire");

    //     basicFireExplosionParticlesList = new List<GameObject>();

    //     for (int i = 0; i < maxExplosions; i++)
    //     {
    //         GameObject explosion = GameObject.Instantiate(basicExplosionParticlePrefab);
    //         explosion.transform.parent = this.transform;
    //         explosion.SetActive(false);
    //         basicFireExplosionParticlesList.Add(explosion);
    //     }
    // }

    // private void initalizeBasicProjectile()
    // {
    //     GameObject basicProjectilePrefab = Resources.Load<GameObject>("Prefab/LaserBeamProjectile");
    //     basicProjectilesList = new List<GameObject>();

    //     for (int i = 0; i < maxBulletCount; i++)
    //     {
    //         GameObject projectile = GameObject.Instantiate(basicProjectilePrefab);
    //         projectile.transform.parent = this.transform;
    //         projectile.SetActive(false);
    //         basicProjectilesList.Add(projectile);
    //     }
    // }
    #endregion

    #endregion


    #region getters and setters

    public GameObject getPoolObject(POOL_OBJECT_TYPE type)
    {
        List<GameObject> effectList = null;

        switch (type)
        {
            case POOL_OBJECT_TYPE.FireEXplosionParticle:
                effectList = basicFireExplosionParticlesList;
                break;
            case POOL_OBJECT_TYPE.RocketExplosionParticle:
                effectList = basicRocketExplosionParticleList;
                break;
            case POOL_OBJECT_TYPE.DroneExplosion:
                effectList = basicDroneExplosionList;
                break;
            case POOL_OBJECT_TYPE.HumanoidExplosion:
                effectList = bulletHitBasicParticleList;
                break;
            case POOL_OBJECT_TYPE.BasicProjectile:
                effectList = basicProjectilesList;
                break;
            case POOL_OBJECT_TYPE.ElectricParticleEffect:
                effectList = electricParticleEffectList;
                break;
            default:
                effectList = null;
                break;
            case POOL_OBJECT_TYPE.SmokeEffect:
                effectList = smokeEffectList;
                break;
            case POOL_OBJECT_TYPE.RifleAmmo:
                effectList = rifleAmmoList;
                break;
            case POOL_OBJECT_TYPE.PistolAmmo:
                effectList = pistolAmmoList;
                break;
            case POOL_OBJECT_TYPE.Grenade:
                effectList = grenadeList;
                break;
            case POOL_OBJECT_TYPE.BasicRocket:
                effectList = basicRocketList;
                break;
            case POOL_OBJECT_TYPE.DroidExplosionParticleEffect:
                effectList = droidExplosionsList;
                break;
            case POOL_OBJECT_TYPE.GlassParticleEffect:
                effectList = glassParticles;
                break;
            case POOL_OBJECT_TYPE.BloodSplatterEffect:
                effectList = blood_splatter_particles;
                break;
            case POOL_OBJECT_TYPE.DamageText:
                effectList = damage_text;
                break;
            case POOL_OBJECT_TYPE.Obj_Indicator:
                effectList = inicator_list;
                break;
            case POOL_OBJECT_TYPE.Granade_Explosion:
                effectList = basicGrenadeExplosionParticlesList;
                break;
            case POOL_OBJECT_TYPE.IncendearyProjectile:
                effectList = IncendiaryProjectilesList;
                break;
            case POOL_OBJECT_TYPE.SmallFireEffect:
                effectList = smallFireEffect;
                break;
            case POOL_OBJECT_TYPE.ElectricProjectile:
                effectList = electric_projectile;
                break;
            case POOL_OBJECT_TYPE.AttackDroneExplosion:
                effectList = attackDroneExplosion;
                break;
            case POOL_OBJECT_TYPE.RepairDroneExplosionParticle:
                effectList = repairDroneExplosionList;
                break;
        }

        foreach (GameObject projectile in effectList)
        {
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
