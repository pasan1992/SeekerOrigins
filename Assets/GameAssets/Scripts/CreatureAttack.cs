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
    bool isDie = false;

    // Start is called before the first frame update
    void Start()
    {
        _creatureAnim = GetComponentInChildren<Animator>();
        _navMeshAgent = transform.GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = Random.Range(2, 4);
        //_damagableObject = player.GetComponent<DamagableObject>();
        _damagableObject = gameObject.GetComponent<CreatureDamagableObject>();

        _damagableObject.setOnDestroyed(OnDestory);

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
        if (!isDie)
        {
            _navMeshAgent.SetDestination(player.transform.position);
            this.transform.LookAt(player.transform.position);

            if (Vector3.Distance(this.transform.position, player.transform.position) > biteDistance)
            {
                //_creatureAnim.SetBool("isWorking", true);
                _creatureAnim.speed = _navMeshAgent.speed;
                _creatureAnim.SetBool("Run Forward", true);
            }
            else
            {
                //_creatureAnim.SetBool("isWorking", false);
                _creatureAnim.SetBool("Run Forward", false);
            }
            Bite();
        }
    }

    void Bite()
    {
        _updateTime += Time.deltaTime;

        if (_updateTime > bitInterval)
        {
            if (Vector3.Distance(this.transform.position, player.transform.position) < biteDistance)
            {
                //_creatureAnim.SetTrigger("Attack");
                _creatureAnim.SetTrigger("Stab Attack");
                Invoke("Damaging", 1);
            }
            _updateTime = 0;
        }
    }

    void Damaging()
    {
        player.GetComponent<HumanoidDamagableObject>().damage(new CommonFunctions.Damage(creatureDamage,0), _legCollider, Random.insideUnitSphere, _legCollider.transform.position, agentBasicData.m_agentFaction);
    }

    public void OnDestory()
    {
        isDie = true;
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        _creatureAnim.SetTrigger("Die");

        StartCoroutine(DieCompete());
    }

    IEnumerator DieCompete()
    {
        yield return new WaitForSeconds(4);
        GameObject basicHitParticle = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.SmokeEffect);
        basicHitParticle.SetActive(true);
        basicHitParticle.transform.localScale = new Vector3(5f, 5f, 5f);
        basicHitParticle.transform.position = transform.position;

        yield return new WaitForSeconds(3);
        _damagableObject.damage_effect(transform);
        Destroy(this.gameObject);
    }
}
