using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTextShower : MonoBehaviour
{
    private static CombatTextShower _instance;
    public static CombatTextShower Instance { get { return _instance; } }
    public FloatingInfoText yellText;
    public float yellTextOffset;

    public float TimePerYell;
    private float timeFromLastYell;
    private bool yellActive = false;

    private ICyberAgent currentAgent;
    private string currentMessage;
    


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        yellIntervalUpdate();
        updateYellMessage();
    }

    public void yellMessage(string message,ICyberAgent agent)
    {
        if(!yellActive)
        {
            currentAgent = agent;
            currentMessage = message;
            yellText.gameObject.SetActive(true);
            yellText.setTarget(currentAgent.getTransfrom(), Vector3.up * 1);
            yellText.setText(message);
            yellActive = true;
        }
    }

    public void yellMessage(string message, ICyberAgent agent,float time,float prob)
    {
        if(Random.value < prob)
        {
            yellMessage(message, agent);
            TimePerYell = time;
        }

    }

    private void updateYellMessage()
    {
        if(yellActive)
        {
            if(currentAgent !=null & !currentAgent.IsFunctional())
            {
                stopYell();
            }
        }
    }

    private void stopYell()
    {
        yellText.gameObject.SetActive(false);
        timeFromLastYell = 0;
        yellActive = false;
    }

    private void yellIntervalUpdate()
    {
        if(yellActive)
        {
            timeFromLastYell += Time.deltaTime;
            if (timeFromLastYell > TimePerYell)
            {
                timeFromLastYell = 0;
                yellActive = false;
            }
        }
    }
}
