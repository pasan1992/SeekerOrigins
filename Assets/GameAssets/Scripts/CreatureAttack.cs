using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAttack : MonoBehaviour
{
    private Animator _creatureAnim;
    public GameObject player;

    public int bitInterval;
    public int biteDistance;
    public int creatureDamage;

    public AgentBasicData agentBasicData;

    NavMeshAgent _navMeshAgent;

    DamagableObject _damagableObject;
    Collider _legCollider;

    float _updateTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        _creatureAnim = GetComponentInChildren<Animator>();
        _navMeshAgent = transform.GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = Random.Range(2, 4);
        _damagableObject = player.GetComponent<DamagableObject>();

        Component[] colliders = player.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.name == "LowerLeg_L")
            {
                _legCollider = collider;
                Debug.Log(_legCollider.name);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _navMeshAgent.SetDestination(player.transform.position);
        this.transform.LookAt(player.transform.position);

        if (Vector3.Distance(this.transform.position, player.transform.position) > biteDistance)
        {
            _creatureAnim.SetBool("isWorking", true);
        }
        else
        {
            _creatureAnim.SetBool("isWorking", false);
        }
        Bite();
    }

    void Bite()
    {
        _updateTime += Time.deltaTime;

        if (_updateTime > bitInterval)
        {
            if (Vector3.Distance(this.transform.position, player.transform.position) < biteDistance)
            {
                _creatureAnim.SetTrigger("Attack");
                Invoke("Damaging", 1);
            }
            _updateTime = 0;
        }
    }

    void Damaging()
    {
        player.GetComponent<HumanoidDamagableObject>().damage(creatureDamage, _legCollider, Random.insideUnitSphere, _legCollider.transform.position, agentBasicData.m_agentFaction);
    }

}
