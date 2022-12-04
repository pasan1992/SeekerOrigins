using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPack : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> Locations;
    private int m_current_transfrom = 0;
    public FloatingGameUI targetIcon;

    private AmmoTypeEnums.Missile currentRocketName = AmmoTypeEnums.Missile.DroneBusters_Missile;

    private Queue<GameObject> rockets = new Queue<GameObject>();
    private float currentTIme;
    public float RocketSpawnTime = 0.5f;
    public HumanoidMovingAgent agent; 
    private bool rocket_reload = false;
    
    public void FireMissleLocation(Vector3 location, AgentData agentData)
    {


        if(rockets.Peek())
        {
            // GameObject basicRocketObj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicRocket);
            // var fire_transfrom = getFireTransfrom();
            // basicRocketObj.transform.position = fire_transfrom.transform.position + new Vector3(0, 0, 0);
            // basicRocketObj.transform.rotation = fire_transfrom.transform.rotation;
            // basicRocketObj.SetActive(true);
            var rocketObj = rockets.Dequeue();
            rocketObj.transform.parent = null;
            BasicRocket rocket = rocketObj.GetComponent<BasicRocket>();
            rocket.fireRocketLocation(location);
        }

    }

    IEnumerator fireRocket(DamagableObject obj)
    {
        Debug.Log(obj);
        bool wait_to_fire = true;
        while(wait_to_fire)
        {
                var rocketObj = rockets.Dequeue();
                BasicRocket rocket = rocketObj.GetComponent<BasicRocket>();
                rocketObj.transform.parent = null;
                rocket.fireFollowRocketAgent(obj);
                wait_to_fire = false;
                if(rockets.Count==0)
                {
                    rocket_reload = false;
                }
            
            yield return null;
        }
    }

    public void Update()
    {
        currentTIme +=Time.deltaTime;
        if(RocketSpawnTime < currentTIme)
        {
            currentTIme =0;
            
            if(rockets.Count < 4)
            {
                var ammo_count = agent.GetAgentData().useAmmoCount(getCurrentRokectName(),1);
                if(ammo_count > 0)
                {
                    var loc = getFireTransfrom();
                    GameObject basicRocketObj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicRocket);
                    basicRocketObj.transform.position = loc.transform.position + new Vector3(0, 0, 0);
                    basicRocketObj.transform.rotation = loc.transform.rotation;
                    basicRocketObj.transform.parent = loc;
                    basicRocketObj.SetActive(true);
                    rockets.Enqueue(basicRocketObj);
                }
                else
                {
                    if(rockets.Count> 0)
                    {
                        rocket_reload = true;
                    }
                    
                }

            }
            else{
                rocket_reload = true;
            }
        }
    }

    public string getCurrentRokectName()
    {
        return currentRocketName.ToString();
    }

    public void FireMissiles(AgentData agentData)
    {
        switch(currentRocketName)
        {
            case AmmoTypeEnums.Missile.DroneBusters_Missile:
            

            StartCoroutine(fireDroneBusters(agentData));
            break;
            case AmmoTypeEnums.Missile.MiniNuke_Missile:
            var target = getTransfrom();
            if(target)
            {
                //FireMissleTransfrom(target,agentData);
            }
            break;
        }
    }

    IEnumerator fireDroneBusters(AgentData agentData)
    {
        var agents =  FindObjectsOfType<AgentController>();
            foreach(AgentController agent in agents)
            {

                    if(GamePlayCam.IsVisibleToCamera(agent.transform) && agent.getICyberAgent().GetAgentData().m_agentFaction != agentData.m_agentFaction && agent.getICyberAgent().IsFunctional())
                    {
                        if(rocket_reload)
                        {
                            StartCoroutine(fireRocket(agent.GetComponent<DamagableObject>()));
                        }
                        
                        yield return new WaitForSeconds(0.5f);
                    }
                    
                
            }
    }

    private Transform getTransfrom()
    {
            RaycastHit m_raycastHit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_raycastHit, 100, LayerMask.GetMask("Enemy")))
            {
                return m_raycastHit.transform;
            }
            return null;
    }

    public void FireMissleTransfrom(Transform transfrom, AgentData agentData)
    {
        Debug.Log(transform.position);
        //StartCoroutine(fireRocket(transfrom.gameObject.transform));
        
    }

    private Transform getFireTransfrom()
    {
        m_current_transfrom += 1;
        if(m_current_transfrom > Locations.Count-1)
        {
            m_current_transfrom = 0;
        }
        return Locations[m_current_transfrom];
    }

    IEnumerator disableTarget(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        targetIcon.setMainImageStatus(false);
    }


}
