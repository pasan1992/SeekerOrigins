using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HumanoidMovingAgent))]
public class PlayerController : AgentController
{
    private bool m_enabled;
    protected HumanoidMovingAgent m_movingAgent;

    private LayerMask enemyHitLayerMask;
    private LayerMask targetHitLayerMask;

    private LayerMask floorHitLayerMask;
    
    private float speedModifyVale;
    private float verticleSpeed;
    private float horizontalSpeed;
    private HealthBar m_healthBar;


    private NavMeshAgent agent;

    private Vector3 m_currentTargetPosition;

    private bool was_hidden_before_running = false;

    private bool m_trigger_pulled = false;

    private CoverPoint[] coverpoints;

    private bool incover = false;

    private RocketPack m_rocket_pack;

    GamePlayCam camplayer;

    #region Initialize
    private void Start()
    {
        m_movingAgent = this.GetComponent<HumanoidMovingAgent>();
        enemyHitLayerMask = LayerMask.GetMask("Enemy");
        targetHitLayerMask = LayerMask.GetMask("Target");
        floorHitLayerMask = LayerMask.GetMask("Floor");
        m_healthBar = this.GetComponentInChildren<HealthBar>();

        createTargetPlane();
        m_movingAgent.enableTranslateMovment(true);
        intializeAgentCallbacks(m_movingAgent);
        agent = this.GetComponent<NavMeshAgent>();
        MouseCurserSystem.getInstance().setTargetLineTrasforms(m_movingAgent.AgentComponents.weaponAimTransform.transform,m_movingAgent.getTargetTransfrom());
        coverpoints = GameObject.FindObjectsOfType<CoverPoint>();

        m_rocket_pack = GetComponentInChildren<RocketPack>();
        camplayer = GameObject.FindObjectOfType<GamePlayCam>();
    }

    private void createTargetPlane()
    {
        // Create target support Plane.
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = this.transform;
        plane.transform.localPosition = new Vector3(0, 1.24f, 0);
        plane.transform.localScale = new Vector3(7, 7, 7);
        plane.layer = 10; // Target layer 
        plane.GetComponent<MeshFilter>().mesh = null;
        plane.name = "TargetPlane";
    }
    #endregion

    #region Updates

    private void updateNavMesgAgnet()
    {
        if(Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorHitLayerMask))
            {
                agent.isStopped = true;
                agent.SetDestination(hit.point);
                agent.isStopped = false;
            }
        }
    }

    private void controllerUpdate()
    {

        if(m_movingAgent.isInteracting())
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                var int_type = m_movingAgent.getCurrentInteractionType();
                if(Interactable.IsValidIntearction(int_type))
                {
                    if(int_type == Interactable.InteractableProperties.InteractableType.DialogInteraction)
                    {
                        m_movingAgent.skipInteraction();
                    }
                    else
                    {
                        m_movingAgent.cancleInteraction();
                    }
                    
                }
                
            }
            return;
        }

        bool crouch_pressed = Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(1);
        verticleSpeed = Mathf.Lerp(verticleSpeed, Input.GetAxis("Vertical"),1);
        horizontalSpeed = Mathf.Lerp(horizontalSpeed, Input.GetAxis("Horizontal"), 1);

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(m_movingAgent.isInteracting())
            {
                m_movingAgent.cancleInteraction();
            }
            else
            {
                m_movingAgent.stopAiming();
                m_movingAgent.Interact();
            }
        }

        // Setting Character Aiming.
        if ( /*Input.GetMouseButton(1) &&*/!crouch_pressed && !Input.GetKey(KeyCode.LeftShift) && !m_movingAgent.isEquipingWeapon() 
            && m_movingAgent.isReadyToAim()
            && !m_movingAgent.isInteracting())
        {
            m_movingAgent.aimWeapon();
        }
        else
        {
            if(m_movingAgent.isReadyToAim() && m_movingAgent.isEquipingWeapon())
            {
                Debug.Log("Conflict");
            }
            m_movingAgent.stopAiming();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_movingAgent.togglepSecondaryWeapon();

            // Set accuracy for ui
            if (m_movingAgent.getCurrentWeapon())
            {
                MouseCurserSystem.getInstance().set_current_weapon_accuracy((int)m_movingAgent.getCurrentWeapon().accuracy_rating);
            }
            else
            {
                MouseCurserSystem.getInstance().set_current_weapon_accuracy(-1);
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_movingAgent.togglePrimaryWeapon();

            // Set accuracy for ui
            if (m_movingAgent.getCurrentWeapon())
            {
                MouseCurserSystem.getInstance().set_current_weapon_accuracy((int)m_movingAgent.getCurrentWeapon().accuracy_rating);
            }
            else
            {
                MouseCurserSystem.getInstance().set_current_weapon_accuracy(-1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_movingAgent.toggleGrenede();

            // Set accuracy for ui
            if (m_movingAgent.getCurrentWeapon())
            {
                MouseCurserSystem.getInstance().set_current_weapon_accuracy((int)m_movingAgent.getCurrentWeapon().accuracy_rating);
            }
            else
            {
                MouseCurserSystem.getInstance().set_current_weapon_accuracy(-1);
            }
        }

        if (crouch_pressed)
        {
            /*
            if(m_movingAgent.isHidden())
            {
                was_hidden_before_running = false;
            }
            */

            if(!m_movingAgent.isHidden())
                m_movingAgent.toggleHide();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_movingAgent.dodgeAttack(getDirectionRelativeToCamera( new Vector3(verticleSpeed, 0, -horizontalSpeed)));
        }

        if(Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(verticleSpeed) > 0 || Mathf.Abs(horizontalSpeed) > 0))
        {
            speedModifyVale = Mathf.Lerp(speedModifyVale, 1.5f, 0.1f);
            if(m_movingAgent.isHidden())
            {
                m_movingAgent.toggleHide();
                was_hidden_before_running = true;
            }
        }
        else
        {
            speedModifyVale = Mathf.Lerp(speedModifyVale, 1f, 0.1f);

            if(was_hidden_before_running)
            {
                m_movingAgent.toggleHide();
                was_hidden_before_running = false;
            }
        }
        m_movingAgent.moveCharacter(getDirectionRelativeToCamera((new Vector3(verticleSpeed, 0, -horizontalSpeed)).normalized*speedModifyVale));

        if(Input.GetKeyDown(KeyCode.R))
        {
            m_movingAgent.reloadCurretnWeapon();
        }



        RaycastHit m_raycastHit;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_raycastHit, 100, LayerMask.GetMask("Enemy")))
            {
                if(m_rocket_pack)
                {
                    m_rocket_pack.FireMissleTransfrom(m_raycastHit.transform);
                }
            }
        }


        if (crouch_pressed)
        {
            if(incover)
            {
                m_healthBar.set_full_cover();
            }
            else
            {
                m_healthBar.set_no_cover();
            }
        }
        else
        {
            if(incover && m_movingAgent.isAimed())
            {
                m_healthBar.set_half_cover();
            }
            else if(incover)
            {
                m_healthBar.set_full_cover();
            }
            else
            {
                m_healthBar.set_no_cover();
            }
        }


        if(Input.GetKeyDown(KeyCode.G))
        {
            if(m_movingAgent.isAimed() & m_movingAgent.isReadyToAim())
            {
                m_movingAgent.Throw();
            }
            
        }


        UpdateShooting();

        UpdateTargetPoint();
    }

    private void UpdateShooting()
    {
        bool crouch_pressed = Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(1);
        if (Input.GetMouseButtonDown(0) /*&& Input.GetMouseButton(1)*/ && !Input.GetKey(KeyCode.LeftShift) && !crouch_pressed)
        { 
            m_movingAgent.pullTrigger();
            MouseCurserSystem.getInstance().onBullet_Fire();

            m_trigger_pulled = true;
        }

        if(Input.GetMouseButtonUp(0) /* && Input.GetMouseButton(1) */ || crouch_pressed || Input.GetKey(KeyCode.LeftShift))
        {
            m_movingAgent.releaseTrigger();
            m_trigger_pulled = false;
        }
    }

    private void UpdateTargetPoint()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        Vector3 targetPosition = Vector3.zero;

        bool found = false;

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, enemyHitLayerMask))
        {
            //targetPosition = setTargetHeight(hit.point, hit.transform.tag);
            MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.ONTARGET);
            targetPosition = hit.point;
            found = true;

            if(m_movingAgent.isAimed())
            {
                MouseCurserSystem.getInstance().enableTargetLine(true);

                if (m_trigger_pulled && !m_movingAgent.is_currentWeaponEmpty())
                {
                    MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.ONTARET_FIRE);
                }
                else
                {
                    MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.ONTARGET);
                }
            }
        }

        if (!found && Physics.Raycast(castPoint, out hit, Mathf.Infinity, targetHitLayerMask))
        {
            if(m_movingAgent.isAimed())
            {
                if (m_trigger_pulled & !m_movingAgent.is_currentWeaponEmpty())
                {
                    MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.FIRE);
                }
                else
                {
                    MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.AIMED);
                }
                
                MouseCurserSystem.getInstance().enableTargetLine(true);
            }
            else
            {
                MouseCurserSystem.getInstance().setMouseCurserState(MouseCurserSystem.CURSOR_STATE.IDLE);
                MouseCurserSystem.getInstance().enableTargetLine(false);
            }

            targetPosition = hit.point;
        }

        m_movingAgent.setTargetPoint(targetPosition);
        m_currentTargetPosition = targetPosition;
    }

    private void Update()
    {
        controllerUpdate();
        //CoverFinder();

        if (m_healthBar)
        {
            //m_healthBar.setHealthPercentage(m_movingAgent.AgentData.Health/m_movingAgent.AgentData.MaxHealth);
            m_healthBar.setHealthPercentage(m_movingAgent.AgentData);
        }
    }

    private void FixedUpdate()
    {
        CoverFinder();
    }

    private void CoverFinder()
    {
        bool crouch_pressed = Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(1);
        bool found_cover = false;
        foreach(CoverPoint cp in coverpoints)
        {
            cp.setCoverHighlightStatus(false);
            if (Vector3.Distance(cp.transform.position,this.transform.position)<2f && m_movingAgent.getMovmentDirection() == Vector3.zero)
            {
                if(!m_movingAgent.isHidden())
                {
                    m_movingAgent.toggleHide();
                }
                incover = true;
                cp.setCoverHighlightStatus(true);
                found_cover = true;
                break;
            }
        }
        if (!found_cover && !crouch_pressed)
        {
            if(m_movingAgent.isHidden())
            {
                m_movingAgent.toggleHide();
            }
            incover = false;

        }
    }
    #endregion

    #region Commands

    private void summonRockets(Vector3 position)
    {
        //var agents =  FindObjectsOfType<HumanoidMovingAgent>();
        //StartCoroutine(  waitAndFire(agents));
        GameObject basicRocketObj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicRocket);
        basicRocketObj.transform.position = this.transform.position + new Vector3(0, 0, 0);
        basicRocketObj.SetActive(true);
        BasicRocket rocket = basicRocketObj.GetComponent<BasicRocket>();
        rocket.fireRocketLocation(position);
        rocket.rocketScale = 0.4f;

    }



    IEnumerator waitAndFire(HumanoidMovingAgent[] targets)
    {
        foreach(HumanoidMovingAgent agent in targets)
        {
            if(agent.GetAgentData().m_agentFaction != AgentBasicData.AgentFaction.Player && GamePlayCam.IsVisibleToCamera(agent.transform) &&
                agent.IsFunctional())
            {
                GameObject basicRocketObj = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.BasicRocket);
                basicRocketObj.transform.position = this.transform.position + new Vector3(0, 0, 0);
                basicRocketObj.SetActive(true);
                BasicRocket rocket = basicRocketObj.GetComponent<BasicRocket>();
                rocket.fireFollowRocketAgent(agent.GetComponent<DamagableObject>());
                rocket.rocketScale = 0.4f;
                yield return new WaitForSeconds(0.1f);
            }

        }       
    }

    #endregion

    #region getters and setters

    public void setEnabled (bool enabled)
    {
        m_enabled = enabled;
    }

    public bool getEnabled()
    {
        return m_enabled;
    }

    // Set target Height depending on the target type.
    private Vector3 setTargetHeight(Vector3 position, string tag)
    {
        switch (tag)
        {
            case "Floor":
                return new Vector3(position.x, position.y, position.z);
            case "Enemy":
                return position;
        }
        return Vector3.zero;
    }

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_movingAgent = (HumanoidMovingAgent)agent;
    }

    private Vector3 getDirectionRelativeToCamera(Vector3 direction)
    {
        var camera = Camera.main;

        //camera forward and right vectors:
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return forward * direction.x - right * direction.z;
    }

    public override float getSkill()
    {
        throw new System.NotImplementedException();
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_movingAgent;
    }

    public override void OnAgentDestroy()
    {
        base.OnAgentDestroy();
    }

    public override void onAgentDisable()
    {
    }

    public override void onAgentEnable()
    {
    }

    public override void resetCharacher()
    {
        throw new System.NotImplementedException();
    }

    public override void setPosition(Vector3 postion)
    {
    }

    public void CancleInteraction()
    {
        if(m_movingAgent.isInteracting())
        {
            m_movingAgent.cancleInteraction();
        }
        else
        {
            Debug.LogError("There is no interaction to cancle");
        }
    }

    public void FindCoverPoints()
    {
        coverpoints = GameObject.FindObjectsOfType<CoverPoint>();
    }

    public override void OnDamage()
    {
        camplayer.DamageEffect(1);
    }

    #endregion
}
