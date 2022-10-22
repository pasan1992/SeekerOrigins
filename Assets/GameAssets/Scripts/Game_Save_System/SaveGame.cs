using UnityEngine;

[System.Serializable]
public class SaveGame
{
    public AgentData agentData;

    public int curentScence;
    public int latestCheckPoint;

    public Vector3 playerPos;
    public Vector3 playerRotaion;

    public Vector3 df_l2_playerPos = new Vector3(-84, -5, -80);
    public float df_l2_playerDirection = 85f;
}
