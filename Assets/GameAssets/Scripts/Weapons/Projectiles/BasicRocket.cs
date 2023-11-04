using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRocket : MonoBehaviour
{
    public Vector3 m_targetLocation;

    private DamagableObject m_followingDamagableObject;
    private Transform m_target;

    public float Speed = 4;

    public float explosionTimeout = 20;

    public float rocketScale = 1;

    private BasicExplodingObject m_explodingObject;

    private bool move_up = true;
    private float m_current_speed = 0;

    private bool armed = false;

    public float upTime = 1;

    private AudioSource m_audioSource;

    public void Awake()
    {
        m_explodingObject = this.GetComponent<BasicExplodingObject>();
        
        var All_transfroms = this.GetComponentsInChildren<Transform>();

        foreach (Transform tf in All_transfroms)
        {
            tf.localScale = tf.localScale * rocketScale;   
        }
        m_audioSource = this.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if(armed)
        {
            Invoke("selfDestoryOnTimeOut",explosionTimeout);
        }
        
    }

    private void OnDisable()
    {
        CancelInvoke();
    }


    private void fireRocket()
    {
        armed = true;
        Invoke("selfDestoryOnTimeOut",explosionTimeout);
        Vector3 relativePosition = m_targetLocation - this.transform.position;

        var sound_clip = SoundManager.getInstance().getSound("Rocket");
        if(sound_clip)
        {
            m_audioSource.PlayOneShot(sound_clip);
        }
    }

    [ContextMenu("Fire")]
    public void fireFollowRocketAgent(DamagableObject followTarget)
    {
        m_followingDamagableObject = followTarget;
        fireRocket();
        move_up = true;
        StartCoroutine(waitToAim(0.1f));
        m_followingDamagableObject = null;
    }

    public void fireRocketLocation(Vector3 position)
    {
        var t = new GameObject();
        t.transform.position = position;
        m_target = t.transform;
        fireRocket();
        move_up = true;
        StartCoroutine(waitToAim(upTime));
    }

    public void fireRocketTransfrom(Transform transfrom)
    {
        m_target = transfrom;
        fireRocket();
        move_up = true;
        StartCoroutine(waitToAim(upTime));
    }

    private Vector3 getTarget()
    {
        if(m_followingDamagableObject!=null)
        {
            return m_followingDamagableObject.getTransfrom().position;
        }
        return m_target.transform.position;
    }



    public void Update()
    {
        if(!armed)
        {
            return;
        }
        // Enable follow target position
        if( (m_followingDamagableObject != null && !m_followingDamagableObject.isDestroyed()) || m_target !=null )
        {
            if(move_up)
            {
                m_targetLocation = this.transform.position + Vector3.up * 100;
            }
            else
            {
                m_targetLocation = getTarget();
            }
            
        }


        Vector3 moveStep = Vector3.MoveTowards(this.transform.position,m_targetLocation,Time.deltaTime*Speed);

        //this.transform.position = moveStep;
        //this.transform.LookAt(m_targetLocation,Vector3.up);
        m_current_speed = Mathf.Lerp(m_current_speed, Speed, 0.01f);
        this.transform.Translate(Vector3.forward*Time.deltaTime* m_current_speed);

        Vector3 direction = m_targetLocation - this.transform.position;
        Quaternion toRotation = Quaternion.FromToRotation(Vector3.forward, direction);

        if(Vector3.Distance(m_targetLocation,this.transform.position)<2)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 1 * Time.time);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 0.006f * Time.time);
        }

        

        checkExplodeCondition();
    }

    private IEnumerator waitToAim(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        move_up = false;
    }

    private void selfDestoryOnTimeOut()
    {
        m_explodingObject.explode(ProjectilePool.POOL_OBJECT_TYPE.RocketExplosionParticle);
    }

    private void checkExplodeCondition()
    {
        if(Vector3.Distance(this.transform.position,m_targetLocation)<0.2f)
        {
            m_explodingObject.explode(ProjectilePool.POOL_OBJECT_TYPE.RocketExplosionParticle);
        }
    }
}
