using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDoor : MonoBehaviour
{
    public enum LoockStatus {LockedForPlayer,LockedForAll,OpenedForAll}
    public LoockStatus lockStatus;
    Animator m_animator;
    private bool opened =false;


    private bool keepOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = this.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(keepOpened)
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
        keepOpened = true;
        m_animator.SetTrigger("Close");
        opened = false;       
    }

    public void activateForPlayer()
    {
        opened = true;
        //m_animator.SetBool("Opened", true);
    }
}
