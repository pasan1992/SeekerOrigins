using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentBasicData
{
    public enum AGENT_NATURE { DROID,DRONE,PLAYER,HUMANOID}

#region Static
    [Header("Agent Static Parameters")]
    [Tooltip("This governs non functional behavior of the unity Ex:- Post destory effects")]
    public AGENT_NATURE AgentNature; 
    public float Skill;
    public float VisualDistance = 12;
    public float VisualAngle = 90;

    [SerializeField] 
    private float maxHealth;
    public float MaxSheild;
    public float Regen;

    public enum AgentFaction { Player,Enemy3,Neutral,Enemy2,Enemy1, Allies };
    public AgentFaction m_agentFaction;
    public enum AgentBehaviorStatus {Suspicious,Neutral}

    private Transform m_agentTransfrom;
#endregion

[Space(10)]

#region Dynamic
      
    [Header("Agent Dynamic Parameters")]
    [SerializeField] 
    private float health;
    public float Sheild;

    //public float MaxSheild
    //{
    //    get => maxSheild;

    //    set
    //    {
    //        maxSheild = value;
    //        Debug.Log("Set");

    //        if (maxSheild < Sheild)
    //        {
    //            Sheild = maxSheild;
    //        }
    //    }
    //}
    //public float Sheild
    //{
    //    get => sheild;
    //    set
    //    {
    //        sheild = value;

    //        if (sheild > MaxSheild)
    //        {
    //            sheild = MaxSheild;
    //        }
    //    }
    //}

    // Getters and Setters
    public float MaxHealth 
    {
        get => maxHealth;

        set 
        {
            maxHealth = value;
            Debug.Log("Set");

            if(maxHealth < Health)
            {
                Health = maxHealth;
            }
        }
     }

    public float Health 
    {
        get => health; 
        set
        {
            health = value;

            if(health > MaxHealth)
            {
                health = MaxHealth;
            }
        }
    }

    public void setAgentTransfrom(Transform t)
    {
        m_agentTransfrom = t;
    }

    public Transform getAgentTransform()
    {
        return m_agentTransfrom;
    }





    #endregion
}
