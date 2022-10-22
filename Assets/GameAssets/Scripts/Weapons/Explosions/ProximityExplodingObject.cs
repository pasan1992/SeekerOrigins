using System.Collections;
using UnityEngine;


  public  enum BombState 
{
    STARTING,
    ACTIVE,
    ARMED
}

public class ProximityExplodingObject : BasicExplodingObject
{
    public float ProximityExplosionCountDown= 5f;

    public float IdleTime=10f;

    public BombState bombState;
    float _redius;

    public bool isExploded = false;

   
    float timer_value = 0f;

  

    public GameObject indicator;
    public GameObject pointLight;

    public void Awake()
    {
        if (indicator)
        {
            indicator.SetActive(false);
        }
    }

    public void Start()
    {
        
        bombState = BombState.STARTING;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isExploded)
        {

            explode();

        }

        switch(bombState)
        {
            case BombState.STARTING:
                indicator.SetActive(true);
                pointLight.SetActive(true);
                //pointlight color change to blue
                pointLight.GetComponent<Light>().color = Color.blue;

                timer_value += Time.deltaTime;
                //wait for 10 seconds
                if (timer_value >= IdleTime)
                {
                    bombState = BombState.ACTIVE;
                    timer_value = 0f;
                }
                break;
            case BombState.ACTIVE:
                //pointlight color change to red
                pointLight.GetComponent<Light>().color = Color.red;
                //check Enemy near
                OnEnemyNear();
                break;

            case BombState.ARMED:
                //pointlight color change to yellow
                
                //wait for 5 seconds
                timer_value += Time.deltaTime;
                if (timer_value < ProximityExplosionCountDown)
                {
                    StartCoroutine(blink());
                }
                if (timer_value >= ProximityExplosionCountDown)
                {
                     explode();
                    timer_value = 0f;
                }
                break;
        }


        


    }
     private IEnumerator blink()
    {
        indicator.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        indicator.SetActive(false);
        yield return new WaitForSeconds(0.1f);
    }



    // Cast a sphere wrapping character controller 10 meters forward
    // to see if it is about to hit anything.
    void OnEnemyNear()
    {
       
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        Collider[] hitColliders = Physics.OverlapSphere(pos, 2.5f);
        foreach (Collider hitCollider in hitColliders)
        {
            switch (hitCollider.tag)
            {
                case "Enemy":
                case "Player":
                case "Head":
                case "Chest":
                bombState = BombState.ARMED;
                    

                break;
            }
        }
    }

    
    

    


    




    

}




