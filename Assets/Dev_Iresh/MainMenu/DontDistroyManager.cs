using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDistroyManager : MonoBehaviour
{
    public bool isGameRunning = false;

    void Awake()
    {
        GameObject[] dontDistroyManagers = GameObject.FindGameObjectsWithTag("DontDistroyManager");

        if (dontDistroyManagers.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
