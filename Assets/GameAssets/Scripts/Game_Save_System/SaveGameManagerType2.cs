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

public class SaveGameManagerType2 : MonoBehaviour
{
    [SerializeField] TMP_Text _Message;
    [SerializeField] GameObject _playerStart;
    [SerializeField] NavMeshAgent _player;
    [SerializeField] List<GameObject> _checkpoints;
    [SerializeField] bool isTesting = false;


    public int activeCheckPoint;

    
    //int _previousScence;

    SaveGame CreateSaveGameObject()
    {
        SaveGame saveGame = new SaveGame();
        saveGame.curentScence = SceneManager.GetActiveScene().buildIndex;
        saveGame.curentCheckPoint = activeCheckPoint;
        saveGame.curentDirecton = _checkpoints[activeCheckPoint].transform.rotation.eulerAngles;

        //saveGame.playerPos = _player.transform.position;
        //saveGame.playerRotaion = _player.transform.rotation.eulerAngles;

        return saveGame;
    }
    private void Start()
    {
        _player.enabled = false;

        if (!File.Exists(Application.streamingAssetsPath + "/gameSave.save"))
        {
            SaveGame(0);
        }
    }

    public void SaveGame(int val)
    {
        activeCheckPoint = val;
        SaveGame saveGame = CreateSaveGameObject();

        SurrogateSelector surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.SurrogateSelector = surrogateSelector;

        FileStream file = File.Create(Application.streamingAssetsPath + "/gameSave.save");

        binaryFormatter.Serialize(file, saveGame);
        file.Close();
        _Message.text = "CheckPoint " + activeCheckPoint + " is saved!";
        print("CheckPoint " + activeCheckPoint + " is saved!");
    }

    public void ResetGame(int val)
    {
        activeCheckPoint = val;
        SaveGame saveGame = ResetSaveGameObject();

        SurrogateSelector surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.SurrogateSelector = surrogateSelector;

        FileStream file = File.Create(Application.streamingAssetsPath + "/gameSave.save");

        binaryFormatter.Serialize(file, saveGame);
        file.Close();

        _Message.text = "SGM CheckPoint Reset Success!";

        print("SGM RESET CheckPoint Reset Success!");

    }

    SaveGame ResetSaveGameObject()
    {
        SaveGame saveGame = new SaveGame();
        saveGame.curentScence = SceneManager.GetActiveScene().buildIndex;
        saveGame.curentCheckPoint = 0;
        saveGame.curentDirecton = _checkpoints[0].transform.rotation.eulerAngles;

        //saveGame.latestCheckPoint = 0;
        //saveGame.playerPos = saveGame.df_l2_playerPos;
        //saveGame.playerRotaion = new Vector3(0, saveGame.df_l2_playerDirection, 0);

        return saveGame;
    }

    public void LoadGame()
    {
        if (!File.Exists(Application.streamingAssetsPath + "/gameSave.save"))
        {
            SaveGame(0);
        }

        SurrogateSelector surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        binaryFormatter.SurrogateSelector = surrogateSelector;

        FileStream file = File.Open(Application.streamingAssetsPath + "/gameSave.save", FileMode.Open);
        SaveGame saveGame = (SaveGame)binaryFormatter.Deserialize(file);
        file.Close();

        if (!isTesting)
        {
            activeCheckPoint = saveGame.curentCheckPoint;
        }

        _playerStart.transform.localPosition = _checkpoints[activeCheckPoint].transform.position;
        _playerStart.transform.rotation = _checkpoints[activeCheckPoint].transform.rotation;
        //_player.transform.position = saveGame.playerPos;
        //_player.transform.rotation = Quaternion.Euler(saveGame.playerRotaion);

        //_player.transform.position = saveGame.playerPos;
        //_player.GetComponent<AgentData>().grenade.count = 20;
        _Message.text = "CheckPoint " + activeCheckPoint + " is loaded";

        int intVal = saveGame.curentScence - 4;
        print("Season 01 | Episode 0" + intVal + " | CheckPoint " + activeCheckPoint + " is loaded");

    }

    public void ActivateCheckpont(int val)
    {
        _checkpoints[val].SetActive(true);
    }

    public void EnableNavMesh()
    {
        _player.enabled = true;
    }
}
