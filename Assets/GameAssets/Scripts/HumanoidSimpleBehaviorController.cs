using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidSimpleBehaviorController : MonoBehaviour
{
    // Start is called before the first frame update
    public enum SimpleBehavior {StayCover,PeakandAimStay,StantAimStay,PeakAndAim,PeekAndShoot,StandAndShoot,Rockets}
    public SimpleBehavior Behavor;
    public GameObject Target;
    private HumanoidMovingAgent m_agent;
    private HealthBar m_hbar;

    private bool actionStarted = false;

    // aim variable
    private bool stayAimed = false;
    private HumanoidMovingAgent m_enemy;

    private Vector3 m_targetPosition;
    void Start()
    {
        m_agent = this.GetComponent<HumanoidMovingAgent>();
        m_hbar = this.GetComponentInChildren<HealthBar>();
        if(Target)
        {
            m_enemy = Target.GetComponent<HumanoidMovingAgent>();
        }
    }

    [ContextMenu("Do START_ACTION")]
    public void START_ACTION()
    {
        actionStarted = false;
        StartAction(0);
    }

    public void STOP_ACTION()
    {
        actionStarted = false;
    }

    public void StartAction(float timeForWait)
    {
        StartCoroutine(waitAndStart(timeForWait));
    }

    private IEnumerator waitAndStart(float time)
    {
        yield return new WaitForSeconds(time);
        startAction();
    }

    private void startAction()
    {
        actionStarted = true;
        switch(Behavor)
        {
            case SimpleBehavior.StayCover:
            staryCover();
            break;
            case SimpleBehavior.PeakandAimStay:
            AimStay(true);
            break;
            case SimpleBehavior.StantAimStay:
            AimStay(false);
            break;
            case SimpleBehavior.PeakAndAim:
            StartCoroutine(peakAndAim());
            break;
            case SimpleBehavior.PeekAndShoot:
            StartCoroutine(PeakAimShoot(true));
            break;
            case SimpleBehavior.StandAndShoot:
            StartCoroutine(PeakAimShoot(false));
            break;
            case SimpleBehavior.Rockets:
            break;
        }
    }


    public void Update()
    {
        if(m_enemy!=null)
        {
            m_targetPosition = m_enemy.getHeadTransfrom().position + Vector3.up *0.2f;
        }
        else
        {
            m_targetPosition = Target.transform.position;
        }

        if (m_hbar)
        {
            m_hbar.setHealthPercentage(m_agent.AgentData);
        }
        if(actionStarted && m_agent.IsFunctional())
        {

            switch(Behavor)
            {
                case SimpleBehavior.StayCover:
                break;
                case SimpleBehavior.PeakandAimStay:
                    m_agent.SET_TARGET(Target.transform.position);
                    aimWeapon();
                break;
                case SimpleBehavior.PeakAndAim:
                    m_agent.SET_TARGET(m_targetPosition);
                    aimWeapon();
                break;
                case SimpleBehavior.PeekAndShoot:
                    m_agent.SET_TARGET(m_targetPosition);
                    aimWeapon();
                break;
                case SimpleBehavior.StandAndShoot:
                    m_agent.SET_TARGET(m_targetPosition);
                    aimWeapon();
                break;
                case SimpleBehavior.StantAimStay:
                    m_agent.SET_TARGET(m_targetPosition);
                    aimWeapon();
                break;
                case SimpleBehavior.Rockets:
                    m_agent.SET_TARGET(m_targetPosition);
                    aimWeapon();
                break;
            } 
        }
        else
        {
            m_hbar.set_no_cover();
        }
      
    }

    private void aimWeapon()
    {
        if(stayAimed)
        {
            m_agent.aimWeapon();
        }
        else
        {
            m_agent.stopAiming();
        }
    }

    private void staryCover()
    {
        m_agent.togglePrimaryWeapon();
        _goHidden(true);
        m_hbar.set_full_cover();
    }

    private void AimStay(bool croushed)
    {
        _changeWeaponStatus(true);
        _goHidden(croushed);
        if(croushed)
            m_hbar.set_half_cover();     
        stayAimed = true;  
    }

    private IEnumerator peakAndAim()
    {
        _changeWeaponStatus(true);
        actionStarted = true;
        _goHidden(true);
        while(actionStarted)
        {
            
            stayAimed = false;
            m_hbar.set_full_cover();
            yield return new WaitForSeconds(1.5f);
            stayAimed = true;
            m_hbar.set_half_cover();
            yield return new WaitForSeconds(1.5f);
        }
        
    }
    private IEnumerator PeakAimShoot(bool crouched)
    {
        _changeWeaponStatus(true);
        actionStarted = true;

        if(crouched)
        {
            _goHidden(true);
        }
        else{
            _goHidden(false);
        }
        
        while(actionStarted)
        { 
            if(crouched)
            {
                stayAimed = false;
                m_hbar.set_full_cover();
                yield return new WaitForSeconds(1.5f);
            }             
            
            stayAimed = true;
            if(crouched)
            {
                m_hbar.set_half_cover();
            }

            for (int l =0 ; l<7;l++)
            {
            if(actionStarted)
            {
                yield return new WaitForSeconds(0.5f);
                m_agent.weaponFireForAI();
            }

            }

        }
        
    }

    
    private void _changeWeaponStatus(bool armed)
    {
        if(!m_agent.isArmed() & armed)
        {
            m_agent.togglepSecondaryWeapon();
        }
        else if (!armed && m_agent.isAimed())
        {
            m_agent.togglepSecondaryWeapon();
        }
    }

    private void _goHidden(bool hidden)
    {
        if(!m_agent.isHidden() && hidden)
        {
            m_agent.toggleHide();
        }
        else if(m_agent.isHidden() && !hidden)
        {
            m_agent.toggleHide();
        }
    }

}
