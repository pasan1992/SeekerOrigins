using System.Collections;
using UnityEngine;

public class ProximityExplodingObject : BasicExplodingObject
{
    float _redius; // check player is in the circil to explode
    // check aegnt faction if player not expolde
    //every 0.5sec run SphereCast 
    //check ditection of a new rigedtboday then check parent is a Damageble object

    [SerializeField] float m_explosionCountDown;
    float m_currentCownDown = 0;
    bool m_countDownStarted = false;

    public float ExplosionCountDown { get => m_explosionCountDown; set => m_explosionCountDown = value; }
    public bool CountDownStarted { get => m_countDownStarted; set => m_countDownStarted = value; }
    public float CurrentCownDown { get => m_currentCownDown; set => m_currentCownDown = value; }

    float blink_time = 0;

    public GameObject indicator;


    public void Awake()
    {
        if (indicator)
        {
            indicator.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ExplosionCountDown < CurrentCownDown && CountDownStarted)
        {
            resetAll();
            explode();
        }

        if (CountDownStarted)
        {
            CurrentCownDown += (Time.deltaTime % 60);
        }

        if (indicator && CountDownStarted)
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

        if (blink_time > ((ExplosionCountDown - CurrentCownDown) / 10))
        {
            StartCoroutine(blink());
            blink_time = 0;
        }

    }

    private IEnumerator blink()
    {
        indicator.SetActive(true);
        yield return new WaitForSeconds((ExplosionCountDown - CurrentCownDown) / 20);
        indicator.SetActive(false);
        yield return new WaitForSeconds((ExplosionCountDown - CurrentCownDown) / 20);
    }
}
