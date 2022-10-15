using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTimerExplodingObject : BasicExplodingObject
{
     [SerializeField] 
    private float m_explosionCountDown;
    private float m_currentCownDown = 0;
    private bool m_countDownStarted = false;

    public float ExplosionCountDown { get => m_explosionCountDown; set => m_explosionCountDown = value; }
    public bool CountDownStarted { get => m_countDownStarted; set => m_countDownStarted = value; }
    public float CurrentCownDown { get => m_currentCownDown; set => m_currentCownDown = value; }

    public GameObject indicator;

    private float blink_time = 0;

    public void Awake()
    {
        if(indicator)
        {
            indicator.SetActive(false);
        }
    }

    public void Update()
    {
        if(ExplosionCountDown < CurrentCownDown && CountDownStarted)
        {
            resetAll();
            explode();
        }

        if(CountDownStarted)
        {
            CurrentCownDown += (Time.deltaTime % 60);
        }

        if(indicator && CountDownStarted)
        {
            indicatorUpdate();
        }
    }

    public void startCountDown()
    {
        CountDownStarted = true;
    }

    public void resetAll()
    {
        CurrentCownDown = 0;
        CountDownStarted = false;
    }

    private void indicatorUpdate()
    {
        blink_time += Time.deltaTime;

        if (blink_time > ( (ExplosionCountDown -CurrentCownDown) / 10) )
        {
            StartCoroutine(blink());
            blink_time = 0;
        }

    }

    private IEnumerator blink()
    {
        indicator.SetActive(true);
        yield return new WaitForSeconds( (ExplosionCountDown - CurrentCownDown) / 20 );
        indicator.SetActive(false);
        yield return new WaitForSeconds( (ExplosionCountDown - CurrentCownDown) / 20 );
    }

    public override void activateExplosionMechanisum()
    {
        base.activateExplosionMechanisum();
        resetAll();
        startCountDown();
    }

}
