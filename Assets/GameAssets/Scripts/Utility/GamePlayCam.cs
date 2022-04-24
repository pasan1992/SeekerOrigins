using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayCam : MonoBehaviour
{
    // Start is called before the first frame update

    public HumanoidMovingAgent target;
    public float rotationSpeed = 1;
    public bool roataionEnabled = true;
    private Vector3 offset;

    private Vector3 m_cameraAimOffset;

    private Vector3 aimedPlayerPositon;
    Vector3 newCameraPosition;

    public bool maintainAimedOffset = false;

    public float shake_magnitue = 1;
    public float shake_count_start = 1;
    public float shake_count_end = 1;
    public float shake_rate = 1;
    public float shake_lerp_start = 1;
    public float shake_lerp_end = 1;

    public Image screenDamageFilter;

    //public float speedMultiplayer;
    void Start()
    {
        offset = target.transform.position - this.transform.position;
        m_cameraAimOffset = Vector3.zero;
        var outlines = FindObjectsOfType<Outline>();
        foreach(Outline outline in outlines)
        {
            outline.OutlineMode = Outline.Mode.SilhouetteOnly;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(roataionEnabled)
        {
            //this.transform.position = Vector3.Lerp(this.transform.position, target.transform.position - offset, Time.deltaTime * 5);
            //speedMultiplayer =Mathf.Lerp(speedMultiplayer, (target.getCurrentVelocity().normalized).magnitude,0.1f);
            this.transform.position = Vector3.Lerp(this.transform.position, calcualteCameraAimPositon(), Time.deltaTime * UtilityConstance.CAMERA_VIEW_FOLLOW_RATE);
            //this.transform.LookAt(target.transform);

           // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.transform.position - this.transform.position), Time.deltaTime * 1);
        }
        else
        {
            Vector3 newPostion = target.transform.position - offset;
            //newPostion = new Vector3(newPostion.x, this.transform.position.y, target.transform.position.z);
            this.transform.position = newPostion;
        }

        if(screenDamageFilter.color.a > 0)
        {
            Color tmp_color = screenDamageFilter.color;
            tmp_color.a -= Time.deltaTime*3;
            screenDamageFilter.color= tmp_color;
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            target.GetAgentData().MaxHealth = 1000;
            target.GetAgentData().Health = 1000;
        }
    }

    private Vector3 calcualteCameraAimPositon()
    {
        if(target.isAimed() 
        // To Avoid unplesent behavior of the camera when dodging + aimed 
        || (HumanoidMovingAgent.CharacterMainStates.Dodge.Equals(target.getCharacterMainStates()) && target.isArmed() && Input.GetMouseButton(1) ) )
        {
            // Smooth the motion of the camera aim offset
            m_cameraAimOffset = Vector3.Lerp(m_cameraAimOffset,Vector3.ClampMagnitude((target.getTargetPosition() - target.transform.position)/2,UtilityConstance.CAMERA_AIM_OFFSET_MAX_DISTANCE ),UtilityConstance.CAMERA_AIM_OFFSET_CHANGE_RATE);
            newCameraPosition = target.transform.position + Vector3.ClampMagnitude((target.getTargetPosition() - target.transform.position)/2,UtilityConstance.CAMERA_AIM_OFFSET_MAX_DISTANCE ) - offset;
            aimedPlayerPositon = target.transform.position;
            return newCameraPosition;
        }
        else if(maintainAimedOffset && (Vector3.Distance(aimedPlayerPositon,target.transform.position) <0.1f || (target.isArmed() && Vector3.Angle(m_cameraAimOffset,target.getMovmentDirection()) < 120 )))
        {
            return target.transform.position + m_cameraAimOffset - offset;
        }
        else
        {
            // To smoothly return the camera to its original position
           m_cameraAimOffset = Vector3.Lerp(m_cameraAimOffset,Vector3.zero,UtilityConstance.CAMERA_AIM_OFFSET_CHANGE_RATE);
           return  target.transform.position - offset + m_cameraAimOffset; 
        }
    }

    public static bool IsVisibleToCamera(Transform transform)
    {
        Vector3 visTest = Camera.main.WorldToViewportPoint(transform.position);
        return (visTest.x >= 0.05f && visTest.y >= 0.05f) && (visTest.x <= 0.95f && visTest.y <= 0.95f) && visTest.z >= 0;
    }

    public IEnumerator cam_Shake(Vector3 direction,float magnitigue)
    {
        /*
        direction.y = 0;
        direction = Vector3.left * shake_magnitue;
        for (int i=0;i<shake_count_start;i++)
        {
            yield return new WaitForSeconds(Time.deltaTime* shake_rate);
            this.transform.position = Vector3.Lerp(this.transform.position, calcualteCameraAimPositon() + direction, shake_lerp_start);
        }

        for (int i = 0; i < shake_count_start; i++)
        {
            yield return new WaitForSeconds(Time.deltaTime * shake_rate);
            this.transform.position = Vector3.Lerp(this.transform.position, calcualteCameraAimPositon(), shake_lerp_end);
        }
        */
        yield return null;

        //transform.position = originalPose;
    }

    public void DamageEffect(int damageType)
    {
        Color tmp_color = screenDamageFilter.color;
        tmp_color.a += 0.2f;
        if(tmp_color.a > 0.5f)
        {
            tmp_color.a = 0.5f;
        }
        screenDamageFilter.color= tmp_color;
    }
}
