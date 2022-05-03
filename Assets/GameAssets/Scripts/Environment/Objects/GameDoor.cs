using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class GameDoor : MonoBehaviour
{
    public enum LoockStatus {LockedForPlayer,LockedForAll,OpenedForAll,KeepOpened}
    public LoockStatus lockStatus;
    Animator m_animator;
    private bool opened =false;
    public bool keepOpened = false;
    private NavMeshObstacle[] door_obsticles;

    // Start is called before the first frame update
    void Awake()
    {
        m_animator = this.GetComponent<Animator>();
        door_obsticles = this.GetComponentsInChildren<NavMeshObstacle>();
    }

    public void Start()
    {
        if(lockStatus == LoockStatus.KeepOpened)
        {
            m_animator.SetTrigger("Open");
            opened = true;          
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if(lockStatus == LoockStatus.KeepOpened)
        {
            return;
        }

        if (!opened)
        {
            if (other.tag == "Enemy" &&  lockStatus != LoockStatus.LockedForAll)
            {
                m_animator.SetTrigger("Open");
                opened = true;
                Invoke("closeDoor", 2);
                Debug.Log("Open");
                setNavMeshObsticleStatus(false);
            }
            else if (other.tag == "Player" &&  lockStatus != LoockStatus.LockedForAll && lockStatus!=LoockStatus.LockedForPlayer )
            {
                m_animator.SetTrigger("Open");
                opened = true;
                Invoke("closeDoor", 2);
                Debug.Log("Open");
                setNavMeshObsticleStatus(false);
            }
        }
        */
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void setNavMeshObsticleStatus(bool enabled)
    {
        foreach(NavMeshObstacle obs in door_obsticles)
        {
            obs.enabled=enabled;
        }
    }

    private void closeDoor()
    {
        opened = false;
        m_animator.SetTrigger("Close");
        setNavMeshObsticleStatus(true);
    }

    public void KeepOpened()
    {
        keepOpened = true;
        m_animator.SetTrigger("Open");
        opened = true;
    }

    public void RemoveKeepOpened()
    {
        lockStatus = LoockStatus.KeepOpened;
        m_animator.SetTrigger("Close");
        opened = false;       
        setNavMeshObsticleStatus(true);
    }

    public void activateForPlayer()
    {
        opened = true;
    }
}
