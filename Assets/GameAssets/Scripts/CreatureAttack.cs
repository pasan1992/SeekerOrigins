using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAttack : MonoBehaviour
{
    public int bitInterval;
    public int biteDistance;
    public int creatureDamage;

    public AgentBasicData agentBasicData;

    DamagableObject _damagableObject;
    Collider _legCollider;

    NavMeshAgent _navMeshAgent;

    public GameObject player;

    float _updateTime = 0;
    //public static GameObject player;
    //public static Collider playerLeftLeg;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = transform.GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = Random.Range(2, 4);
        _damagableObject = player.GetComponent<DamagableObject>();

        Debug.Log(_damagableObject);

        

        Component[] colliders = player.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.name == "LowerLeg_L")
            {
                _legCollider = collider;
                Debug.Log(_legCollider.name);

            }
        }
        //if (player.GetComponentsInChildren<GameObject>())
        //{
        //    Debug.Log("having");

        //}
        //else
        //{
        //    Debug.Log("nothing");

        //}
        //Debug.Log(player.GetComponentInChildren<Collider>().name);
        //if (player.GetComponentInChildren<Collider>().name == "LowerLeg_L")
        //{
        //    playerLeftLeg = player.GetComponentInChildren<Collider>();
        //}


        //_agentBasicData = gameObject.GetComponent<AgentBasicData>();
    }

    // Update is called once per frame
    void Update()
    {
        //is ready
        //if (_navMeshAgent.isOnNavMesh)
        //{
        _navMeshAgent.SetDestination(player.transform.position);
        //}
        this.transform.LookAt(player.transform.position);
        Bite();
    }

    void Bite()
    {
        _updateTime += Time.deltaTime;

        if (_updateTime > bitInterval)
        {
            if (Vector3.Distance(this.transform.position, player.transform.position) < biteDistance)
            {
                player.GetComponent<HumanoidDamagableObject>().damage(creatureDamage, _legCollider, Random.insideUnitSphere, _legCollider.transform.position, agentBasicData.m_agentFaction);
            }
            _updateTime = 0;
        }
    }
}
