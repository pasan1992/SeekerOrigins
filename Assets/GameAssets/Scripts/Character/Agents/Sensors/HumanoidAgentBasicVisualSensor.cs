using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class HumanoidAgentBasicVisualSensor : AgentBasicSensor
{
    public enum VISUAL_STATUS { IDLE,ALERT,COMBAT}

    ICyberAgent m_agent;
    GameEvents.BasicAgentCallback onEnemyDetection;
    GameEvents.BasicNotifactionEvent onAllClear;

    private float allClearCount = 0;
    private float maximummAllClearCount = 50;


    public static string[] layerMaskNames ={"FullCOverObsticles","HalfCoverObsticles","Floor"};
    public static string[] wallLayer = { "Floor" };
    
    private RaycastHit hit;

    float closestDistance;

    ICyberAgent targetAgent;

    FakeMovingAgent m_fakeAgent;

    private float VISUAL_CONE_ANGLE = 90;

    private float VISUAL_DISTANCE = 12;

    private float MINIMUM_CLOSE_SENSTIVITY_DISTANCE = 0.2f;

    private bool normalUpdate = false;

    private Color m_temp_color = Color.red;
    private VISUAL_STATUS m_currentVisualStatus;
    public LookAtIK m_look_IK;

    private GameObject eye_pos_obj;

    // Idle state paramteres
    private float look_angle;

    private HashSet<ICyberAgent> m_rememberEnemyList = new HashSet<ICyberAgent>();
    

    public HumanoidAgentBasicVisualSensor(ICyberAgent agent) : base(agent)
    {
        m_agent = agent;
        m_fakeAgent = new FakeMovingAgent(Vector3.zero);
        m_look_IK = agent.getGameObject().GetComponent<LookAtIK>();
        eye_pos_obj = new GameObject();
        VISUAL_DISTANCE = agent.GetAgentData().VisualDistance;
        VISUAL_CONE_ANGLE = agent.GetAgentData().VisualAngle;

        if(m_look_IK)
        {
            m_look_IK.solver.target = eye_pos_obj.transform;
        }
    }

    protected void onVisualSensorUpdate()
    {
        switch(m_currentVisualStatus)
        {
            case VISUAL_STATUS.ALERT:
                break;
            case VISUAL_STATUS.COMBAT:
                onCombatUpdate();
                break;
            case VISUAL_STATUS.IDLE:
                idleUpdate();
                break;
        }
    }

    private void idleUpdate()
    {
        find_target();
        if(targetAgent !=null)
        {
            switch_to_combat();
        }

        if(m_look_IK)
        {
            if (!m_agent.isInteracting() && !m_agent.isHidden())
            {

                if (look_angle > 180)
                {
                    look_angle = 0;
                }
                look_angle += 0.5f;

                eye_pos_obj.transform.position = calculate_look_position(look_angle, m_agent.getTopPosition(), 50);
                m_look_IK.solver.eyesWeight = 1;
                m_look_IK.solver.bodyWeight = 1;
            }
            else
            {
                m_look_IK.solver.eyesWeight = 0;
                m_look_IK.solver.bodyWeight = 0;
            }
        }


    }

    private void switch_to_combat()
    {
        

        if(targetAgent == null)
        {
            Debug.LogError("Switching to visual combat state but no target");
            return;
        }

        m_currentVisualStatus = VISUAL_STATUS.COMBAT;
        onEnemyDetection(targetAgent);

        if(m_look_IK)
        {
            m_look_IK.enabled = false;
        }

    }

    public void disableLook()
    {
         if(m_look_IK)
        {
            m_look_IK.enabled = false;
        }       
    }

    private Vector3 calculate_look_position(float angle,Vector3 headPos,float look_distance)
    {
        float x = Mathf.Cos(angle) * look_distance + headPos.x;
        float y = headPos.y;
        float z = Mathf.Sin(angle) * look_distance + headPos.z;
        return new Vector3(x, y, z);
    }

    private void find_target()
        // End of this loop, we must have targetAgent set, if null no target found
    {
        Collider[] hitColliders = Physics.OverlapSphere(m_agent.getCurrentPosition(), VISUAL_DISTANCE);
        closestDistance = float.MaxValue;
        targetAgent = null;

        foreach (Collider hitCollider in hitColliders)
        {
            switch (hitCollider.tag)
            {
                case "Chest":
                    onHumanAgentDetected(hitCollider.GetComponentInParent<HumanoidMovingAgent>());
                    onDroneDetected(hitCollider.GetComponentInParent<FlyingAgent>());
                    break;
                case "Wall":
                    break;
                case "Cover":
                    break;
                case "Floor":
                    break;
            }
        }
    }

    protected override void onSensorUpdate()
    {
        onVisualSensorUpdate();
    }


    private void onCombatUpdate()
    {
        ICyberAgent previousAgent = targetAgent;
        find_target();
        //Debug.Log(targetAgent);

        // Check for all clear
        if (targetAgent == null)
        {
            
            //allClearCount +=1;


            //if (allClearCount > maximummAllClearCount || (agentList.Count == 0 && allClearCount > maximummAllClearCount/20))
            // If not seen for a long time, assume a closed location
            if (allClearCount > maximummAllClearCount)
           {
                //Debug.Log("wait countdown " +allClearCount);
                //Debug.Log("Agent Count " + agentList.Count);
                 //allClearCount = 0;
                previousAgent = null;
                 onAllClear();
                allClearCount = 0;
                // m_temp_color = Color.green;


                //var random_pos = Random.insideUnitSphere * Random.value * 10;
                //random_pos.z = 0;
                //m_fakeAgent.moveCharacter(m_fakeAgent.getTopPosition() + random_pos);
                //m_fakeAgent.setActualAgent(previousAgent);
                //onEnemyDetection(m_fakeAgent);
                //allClearCount = 0;
                //eye_pos_obj.transform.position = m_fakeAgent.getTopPosition();

                // allClearCount = 0;
            }
           // Else use the previously seen agent as a fake agent
            else if(previousAgent != null)
            {
                
                m_fakeAgent.moveCharacter(previousAgent.getTopPosition());
                m_fakeAgent.setActualAgent(previousAgent);
                onEnemyDetection(m_fakeAgent);
                m_temp_color = Color.blue;
                eye_pos_obj.transform.position = m_fakeAgent.getTopPosition();
                

                //var random_pos = Random.insideUnitSphere * Random.value * 100;
                //random_pos.z = 0;
                //m_fakeAgent.moveCharacter(m_agent.getTopPosition() + random_pos);
                //m_fakeAgent.setActualAgent(previousAgent);
                //onEnemyDetection(m_fakeAgent);
                //allClearCount = 0;
                //eye_pos_obj.transform.position = m_fakeAgent.getTopPosition();

            }
            else
            {
                //forceUpdateSneosr();
                //var random_pos = Random.insideUnitSphere * Random.value * 10;
                //random_pos.y = 0;
               // m_fakeAgent.moveCharacter(m_agent.getTopPosition() + random_pos);
                //onEnemyDetection(m_fakeAgent);
                //allClearCount = 0;
                //eye_pos_obj.transform.position = m_fakeAgent.getTopPosition();
            }
        }
        else
        {
             m_temp_color = Color.red;
            onEnemyDetection(targetAgent);
            eye_pos_obj.transform.position = targetAgent.getTopPosition();
            allClearCount = 0;
        }
    }

    public override void forceUpdateSneosr()
    {
        VISUAL_CONE_ANGLE = 360;
        VISUAL_DISTANCE = 40;
        normalUpdate = false;
        onSensorUpdate();
        VISUAL_CONE_ANGLE = 85;
        VISUAL_DISTANCE = 20;
        normalUpdate = true;
    }

    public void forceCombatMode(Vector3 position)
    {
        m_fakeAgent.moveCharacter(position);
        onEnemyDetection(m_fakeAgent);
        m_temp_color = Color.blue;
        eye_pos_obj.transform.position = m_fakeAgent.getTopPosition();
        maximummAllClearCount = Mathf.Infinity;
        targetAgent = m_fakeAgent;
    }
    
    public void forceGussedTargetLocation(Vector3 position)
    {
        // If there is no current target, set the given location as the new target location.
        if(targetAgent == null)
        {
            m_fakeAgent.moveCharacter(position);
            onEnemyDetection(m_fakeAgent);
        }
    }

    private void onHumanAgentDetected(HumanoidMovingAgent detectedAgent)
    {
        if(detectedAgent != null)
        {
            if(detectedAgent !=null && detectedAgent.IsFunctional())
            {
                if(IsKnownEnemy(detectedAgent))
                {
                    _onHumanAgentDetected(detectedAgent,false);
                    return;
                }
                if(!CommonFunctions.isAllies(detectedAgent, m_agent))
                {
                   _onHumanAgentDetected(detectedAgent,true);
                    return;
                }
                
            }           
        }
       //HumanoidMovingAgent detectedAgent =  collider.GetComponentInParent<HumanoidMovingAgent>();
    }

    public bool IsKnownEnemy(HumanoidMovingAgent detectedAgent)
    {
        foreach(HumanoidMovingAgent mv in m_rememberEnemyList)
        {
            if(mv == detectedAgent)
            {
                return true;
            }
        }

        return false;
    }

    private void _onHumanAgentDetected(HumanoidMovingAgent detectedAgent,bool new_enemy)
    {

        Vector3 direction = (detectedAgent.getCurrentPosition() - m_agent.getCurrentPosition());
        Vector3 current_looking_direction = eye_pos_obj.transform.position - m_agent.getTopPosition();
        float distance = direction.magnitude;
        direction = direction.normalized;

        if (Vector3.Angle(direction, current_looking_direction) < VISUAL_CONE_ANGLE && distance < closestDistance && detectedAgent.IsFunctional() && isVisibleHumanoid(detectedAgent))
        {
            closestDistance = distance;
            targetAgent = detectedAgent;
            if(new_enemy)
            {
                m_rememberEnemyList.Add(detectedAgent);
            }
            
        }
    }

    private void onDroneDetected(FlyingAgent detectedAgent)
    {
        if(detectedAgent !=null)
        {
            if(detectedAgent !=null && detectedAgent.IsFunctional() && !CommonFunctions.isAllies(detectedAgent,m_agent))
            {
                Vector3 direction = (detectedAgent.getCurrentPosition() -  m_agent.getCurrentPosition());
                float distance = direction.magnitude;
                direction = direction.normalized;

                if( Vector3.Angle(direction,m_agent.getGameObject().transform.forward)< VISUAL_CONE_ANGLE && distance < closestDistance && detectedAgent.IsFunctional())
                { 
                    closestDistance = distance;
                    targetAgent = detectedAgent;  
                }
            }           
        }
    }

    private bool isVisibleHumanoid(HumanoidMovingAgent detectedAgent)
    {
        Vector3 direction =  (m_agent.getCurrentPosition() - detectedAgent.getCurrentPosition());
        float distance = direction.magnitude;
        
        if(distance > MINIMUM_CLOSE_SENSTIVITY_DISTANCE && detectedAgent.isCrouched() && !detectedAgent.isAimed())
        {
            direction = direction.normalized;
            if (Physics.Raycast(detectedAgent.getCurrentPosition() + Vector3.up*0.5f,direction, out hit, 3, LayerMask.GetMask(layerMaskNames)))
            {
                return false;
            }
        }

        if(Physics.Raycast(detectedAgent.getCurrentPosition() + Vector3.up * 0.5f, direction, out hit, distance, LayerMask.GetMask(wallLayer)))
        {
            return false;
        }
        return true;
    }

    public void setOnEnemyDetectionEvent(GameEvents.BasicAgentCallback callback)
    {
        onEnemyDetection = callback;
    }

    public void setOnAllClear(GameEvents.BasicNotifactionEvent onAllClear)
    {
        this.onAllClear = onAllClear;
    }

    public void onEnemeyDestoryed()
    {
    }

    public void setVisualDistance(float distance)
    {
        VISUAL_DISTANCE = distance;
    }


    private Material lineMaterial;
    private void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void OnPostRender()
    {
        /*
        Vector3 front_direction = m_agent.getGameObject().transform.forward.normalized*VISUAL_DISTANCE;
        Vector3 front_position = m_agent.getGameObject().transform.forward.normalized*VISUAL_DISTANCE + m_agent.getGameObject().transform.position;
        Vector3 side1 = Quaternion.AngleAxis(-VISUAL_CONE_ANGLE/2, Vector3.up) * front_direction + m_agent.getGameObject().transform.position;
        Vector3 side2 = Quaternion.AngleAxis(VISUAL_CONE_ANGLE/2, Vector3.up) * front_direction  + m_agent.getGameObject().transform.position;

        CreateLineMaterial();
        lineMaterial.SetPass(0);

        //GL.PushMatrix();
        GL.Begin(GL.LINES);
        GL.Color(m_temp_color);
        GL.Vertex3(m_agent.getGameObject().transform.position.x, m_agent.getGameObject().transform.position.y, m_agent.getGameObject().transform.position.z);
        GL.Vertex3(front_position.x, front_position.y, front_position.z);
        GL.End();


        GL.Begin(GL.LINES);
        GL.Color(m_temp_color);
        GL.Vertex3(m_agent.getGameObject().transform.position.x, m_agent.getGameObject().transform.position.y, m_agent.getGameObject().transform.position.z);
        GL.Vertex3(side1.x, side1.y, side1.z);
        GL.End();

        GL.Begin(GL.LINES);
        GL.Color(m_temp_color);
        GL.Vertex3(m_agent.getGameObject().transform.position.x, m_agent.getGameObject().transform.position.y, m_agent.getGameObject().transform.position.z);
        GL.Vertex3(side2.x, side2.y, side2.z);
        GL.End();
        */

    }
}
