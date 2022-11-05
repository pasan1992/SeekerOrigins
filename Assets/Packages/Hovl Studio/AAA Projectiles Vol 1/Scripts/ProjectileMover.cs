using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    public float speed = 15f;
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public GameObject hit;
    public GameObject flash;
    private Rigidbody rb;
    public GameObject[] Detached;

    public ParticleSystem m_particleSystem;
    private RaycastHit targetPos;
    private bool active_disable = false;
    private float travelled_time = 0;
    private float real_speed = 100;
    private float max_distance = 0;
    private Vector3 start_pos = Vector3.zero;
    public DamageCalculator.DamageFuture damageFuture;

    void Awake()
    {
        real_speed = speed;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
        
        //Destroy(gameObject,5);
	}

    void FixedUpdate ()
    {
		if (speed != 0)
        {
            //rb.velocity = transform.forward * speed;
            transform.position += transform.forward * (speed * Time.deltaTime);   
                
        }
        travelled_time += Time.deltaTime;  

        if(Vector3.Distance(this.transform.position,start_pos) > max_distance)
        {
            onHit(targetPos);
        }

        if(travelled_time > 1)
        {
            this.gameObject.SetActive(false);
        }
	}

    //https ://docs.unity3d.com/ScriptReference/Rigidbody.OnCollisionEnter.html
    void OnCollisionEnter(Collision collision)
    {
        //Lock all axes movement and rotation
        rb.constraints = RigidbodyConstraints.FreezeAll;
        speed = 0;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }
        //Destroy(gameObject);
        this.gameObject.SetActive(false);
    }

    private void onHit(RaycastHit hit_obj)
    {
        //Lock all axes movement and rotation
        //rb.constraints = RigidbodyConstraints.FreezeAll;
        speed = 0;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit_obj.normal);
        Vector3 pos = hit_obj.point + hit_obj.normal * hitOffset;

        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(hit_obj.point + hit_obj.normal); }

            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }
        
        if(damageFuture !=null)
        {
            switch(damageFuture.damageLocation)
            {
                case DamageCalculator.DamageFuture.DamageLocation.Enemy:
                DamageCalculator.onHitEnemy(damageFuture.other,damageFuture.m_fireFrom,damageFuture.hitDirection,damageFuture.damage,damageFuture.dot_time);
                break;
                case DamageCalculator.DamageFuture.DamageLocation.Item:
                DamageCalculator.onHitDamagableItem(damageFuture.other,damageFuture.m_fireFrom,damageFuture.hitDirection);
                break;
                case DamageCalculator.DamageFuture.DamageLocation.Wall:
                DamageCalculator.hitOnWall(damageFuture.other,damageFuture.hitpoint);
                break;
            }
        }



        //Destroy(gameObject);
        this.gameObject.SetActive(false);
    }

    public void StartFire(RaycastHit hit_obj)
    {
        speed = real_speed;
        m_particleSystem.Play();
        targetPos = hit_obj;
        travelled_time = 0;

        if(hit_obj.point != Vector3.zero)
        {
            max_distance = Vector3.Distance(this.transform.position,hit_obj.point);
        }
        else
        {
            max_distance = float.MaxValue;
        }
        
        start_pos = this.transform.position;
        
        //rb.constraints = RigidbodyConstraints.None;
    }
}
