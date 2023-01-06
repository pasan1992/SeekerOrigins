using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System;

public class SaveGameManager : MonoBehaviour
{
    [SerializeField] TMP_Text _Message;
    [SerializeField] List<GameObject> _checkpoints = new List<GameObject>();
    [SerializeField] GameObject _playerStart;
    public PlayerController m_playerController;

    public bool m_isMission = true;
    private int lvl_index = -1;


    private static SaveGameManager this_ins;

    public static SaveGameManager getInstance()
    {
        if(this_ins ==null)
        {
            this_ins = GameObject.FindObjectOfType<SaveGameManager>();
        }

        return this_ins;
    }

    [ContextMenu("ClearAll")]
    public void clearAllSaves()
    {
        PlayerPrefs.DeleteAll();
    }


    public void Start()
    {
        lvl_index = SceneManager.GetActiveScene().buildIndex;
        // If checkpoints are not available no need to activate this mechanisum
        if(m_isMission && _checkpoints.Count > 0)
        {
            StartLevel();
        }

    }

    public void StartLevel()
    {
            if(m_isMission & _checkpoints.Count >0)
            {
                var lvl_data = SaveData.LoadLevelData(lvl_index);
                if (lvl_data == null)
                {
                    Debug.Log("No Level Data Found");
                    return;
                }
                LoadGame(lvl_data);
            }
    }

    public void SaveLevel(int checkpointID)
    {
        var levelData = new SaveData.LevelData();
        levelData.scene_ID = SceneManager.GetActiveScene().buildIndex;
        levelData.currentCheckpoint = checkpointID;
        levelData.playerData.agentData = m_playerController.getICyberAgent().GetAgentData().getSaveAgentData();
        levelData.playerData.playerLookDirection = m_playerController.gameObject.transform.localEulerAngles;
        levelData.playerData.playerPosition = m_playerController.gameObject.transform.localPosition;
        SaveEquipmentBoxes(levelData);
        SaveData.SaveLevelData(SceneManager.GetActiveScene().buildIndex,levelData);
    }

    private void SaveEquipmentBoxes(SaveData.LevelData levelData)
    {
        var eboxes = FindObjectsOfType<AmmoPack>();
        foreach(var box in eboxes)
        {
            levelData.equipmentBoxes.Add(box.GetInstanceID().ToString(),box.properties.interactionEnabled.ToString());
        }
    }

    public void LoadEquipmentBoxes(SaveData.LevelData levelData)
    {

        var eboxes = FindObjectsOfType<AmmoPack>();
        var true_value = true.ToString();
        foreach(var box in eboxes)
        {
            if(levelData.equipmentBoxes.ContainsKey(box.GetInstanceID().ToString()))
            {
                box.properties.interactionEnabled = true;
                if(levelData.equipmentBoxes[box.GetInstanceID().ToString()] != true_value)
                {
                    box.properties.interactionEnabled = false;
                    box.OnInteractionStart();
                }
            }
            levelData.equipmentBoxes[box.GetInstanceID().ToString()] =box.properties.interactionEnabled.ToString();
        }
    }

    public void ResetLevel()
    {
        SaveData.RemoveSaveData(lvl_index);
    }

    private void LoadGame(SaveData.LevelData lvlData)
    {
        m_playerController.getICyberAgent().SetAgentData(lvlData.playerData.agentData);
        m_playerController.GetComponent<NavMeshAgent>().enabled = false;
        m_playerController.transform.localPosition = lvlData.playerData.playerPosition;
        m_playerController.transform.localRotation = Quaternion.Euler(lvlData.playerData.playerLookDirection);
        m_playerController.GetComponent<NavMeshAgent>().enabled = true;
        LoadEquipmentBoxes(lvlData);
        ActivateCheckpont(lvlData.currentCheckpoint);
    }

    private void ActivateCheckpont(int val)
    {
        foreach(var cp in _checkpoints)
        {
            cp.SetActive(false);
        }
        _checkpoints[val].SetActive(true);
    }



    public void EnableNavMesh()
    {
    }
}
