using UnityEngine;
public class DroneDamageModule : DamageModule
{
    private ProjectilePool.POOL_OBJECT_TYPE explosionParticle;
    public DroneDamageModule(AgentBasicData basicData,Outline outline,OnDestoryDeligate onDestroyCallback,Transform agentTransfrom,ProjectilePool.POOL_OBJECT_TYPE exp_p): base(basicData, onDestroyCallback,outline)
    {
        m_basicData.setAgentTransfrom(agentTransfrom);
        explosionParticle = exp_p;
    }

    public void ExplosionEffect(Vector3 position)
    {
        //.getBasicDroneExplosion()
        BasicExplosion droneExplosion = ProjectilePool.getInstance().getPoolObject(explosionParticle).GetComponent<BasicExplosion>();
        droneExplosion.gameObject.SetActive(true);
        droneExplosion.gameObject.transform.position = position;
        droneExplosion.GetComponent<BasicExplosion>().exploade();
    }

    public void DisableDrone(Vector3 position)
    {
        GameObject electricParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.ElectricParticleEffect);
        electricParticle.gameObject.SetActive(true);
        electricParticle.gameObject.transform.position = position;
    }
}
