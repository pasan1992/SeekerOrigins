using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDoor : MonoBehaviour
{
    public enum LoockStatus {LockedForPlayer,LockedForAll,OpenedForAll,KeepOpened}
    public LoockStatus lockStatus;
    Animator m_animator;
    private bool opened =false;
    public bool keepOpened = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_animator = this.GetComponent<Animator>();
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
            }
            else if (other.tag == "Player" &&  lockStatus != LoockStatus.LockedForAll && lockStatus!=LoockStatus.LockedForPlayer )
            {
                m_animator.SetTrigger("Open");
                opened = true;
                Invoke("closeDoor", 2);
                Debug.Log("Open");

            }
        }
    }

    private void closeDoor()
    {
        opened = false;
        m_animator.SetTrigger("Close");
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
    }

    public void activateForPlayer()
    {
        opened = true;
        //m_animator.SetBool("Opened", true);
    }
}
