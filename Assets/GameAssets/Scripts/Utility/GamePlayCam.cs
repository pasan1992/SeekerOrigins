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

    public Image screenDamageFilter;

    private Camera m_camera;
    [SerializeField] AnimationCurve _shakeCurve;
    private bool isStaking;
    private float cam_fov;
    //public float speedMultiplayer;
    void Start()
    {
        offset = target.transform.position - this.transform.position;
        m_cameraAimOffset = Vector3.zero;
        var outlines = FindObjectsOfType<Outline>();
        m_camera = Camera.main;
        cam_fov = m_camera.fieldOfView;
        // foreach(Outline outline in outlines)
        // {
        //     outline.OutlineMode = Outline.Mode.SilhouetteOnly;
        // }
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

        // if(screenDamageFilter.color.a > 0)
        // {
        //     Color tmp_color = screenDamageFilter.color;
        //     tmp_color.a -= Time.deltaTime*3;
        //     screenDamageFilter.color= tmp_color;
        // }

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
            m_cameraAimOffset = Vector3.Lerp(m_cameraAimOffset,Vector3.ClampMagnitude((target.getTargetPosition() - target.transform.position)/2,getCalculatedMaxOffset() ),UtilityConstance.CAMERA_AIM_OFFSET_CHANGE_RATE);
            newCameraPosition = target.transform.position + Vector3.ClampMagnitude((target.getTargetPosition() - target.transform.position)/2,getCalculatedMaxOffset() ) - offset;
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

    private float getCalculatedMaxOffset()
    {
        var screen_y = Input.mousePosition.y;
        var gap = Screen.height/2 - screen_y;
        if(gap <0)
            gap = 0;
        
        var offset_multipler = gap/ (Screen.height/2);
        return UtilityConstance.CAMERA_AIM_OFFSET_MAX_DISTANCE +  offset_multipler*3.5f;

        // Vector3 p = Input.mousePosition;
        // p.z = 20;
        // var pointDirection = (Camera.main.ScreenToWorldPoint(p) - this.transform.position).normalized;
        // var direction = Vector3.Angle(this.transform.forward,pointDirection);

        // if(screen_y < Screen.height/2)
        // {
        // return UtilityConstance.CAMERA_AIM_OFFSET_MAX_DISTANCE + direction/2.5f;
        // }
        // return UtilityConstance.CAMERA_AIM_OFFSET_MAX_DISTANCE + direction/20;

    }

    public static bool IsVisibleToCamera(Transform transform)
    {
        Vector3 visTest = Camera.main.WorldToViewportPoint(transform.position);
        return (visTest.x >= 0.05f && visTest.y >= 0.05f) && (visTest.x <= 0.95f && visTest.y <= 0.95f) && visTest.z >= 0;
    }

    public IEnumerator cam_Shake(Vector3 direction,float magnitigue)
    {
        if(!isStaking)
        {
            isStaking = true;
            for (float t = 0; t < 0.1f; t += Time.deltaTime)
            {
                float y = _shakeCurve.Evaluate(t * 10);
                m_camera.fieldOfView = cam_fov + y/5;
                yield return null;
            }
            var tmp_camP_fov = m_camera.fieldOfView;
            for (float t = 0; t < 0.1f; t += Time.deltaTime)
            {
                float y = _shakeCurve.Evaluate(t * 10);
                m_camera.fieldOfView = tmp_camP_fov - y/5;
                yield return null;
            }
            m_camera.fieldOfView = cam_fov;
            isStaking= false;
        }   


        //transform.position = originalPose;
    }

    public void DamageEffect(float healthValue)
    {
        Color tmp_color = screenDamageFilter.color;
        tmp_color.a = (1 - healthValue)/4;
        // if(tmp_color.a > 0.5f)
        // {
        //     tmp_color.a = 0.5f;
        // }
        screenDamageFilter.color= tmp_color;
    }
}
