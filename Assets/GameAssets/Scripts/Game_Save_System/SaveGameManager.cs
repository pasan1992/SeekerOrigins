using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    //[SerializeField] GameObject _playerStrat;
    [SerializeField] GameObject _player;
    int _checkPoint = 0;

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void SaveGame(int val)
    {
        _checkPoint = val;
        SaveGame saveGame = CreateSaveGameObject();

        SurrogateSelector surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.SurrogateSelector = surrogateSelector;

        FileStream file = File.Create(Application.streamingAssetsPath + "/gameSave.save");

        binaryFormatter.Serialize(file, saveGame);
        file.Close();

        print("Game Saved!");
    }

    SaveGame CreateSaveGameObject()
    {
        SaveGame saveGame = new SaveGame();
        saveGame.latestCheckPoint = _checkPoint;
        saveGame.playerPos= _player.transform.position;
        saveGame.playerRotaion = _player.transform.rotation.eulerAngles;

        return saveGame;
    }

    public void ResetGame(int val)
    {
        _checkPoint = val;
        SaveGame saveGame = ResetSaveGameObject();

        SurrogateSelector surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.SurrogateSelector = surrogateSelector;

        FileStream file = File.Create(Application.streamingAssetsPath + "/gameSave.save");

        binaryFormatter.Serialize(file, saveGame);
        file.Close();

        print("Game Reset Done!");

    }

    SaveGame ResetSaveGameObject()
    {
        SaveGame saveGame = new SaveGame();
        saveGame.latestCheckPoint = 0;
        saveGame.playerPos = saveGame.df_l2_playerPos;
        saveGame.playerRotaion = new Vector3(0, saveGame.df_l2_playerDirection,0);

        return saveGame;
    }

    public void LoadGame()
    {
        if (File.Exists(Application.streamingAssetsPath + "/gameSave.save"))
        {
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.SurrogateSelector = surrogateSelector;

            FileStream file = File.Open(Application.streamingAssetsPath + "/gameSave.save" , FileMode.Open);
            SaveGame saveGame = (SaveGame)binaryFormatter.Deserialize(file);
            file.Close();

            _checkPoint = saveGame.latestCheckPoint;
            _player.transform.position = saveGame.playerPos;
            _player.transform.rotation = Quaternion.Euler(saveGame.playerRotaion);

            //_player.transform.position = saveGame.playerPos;
            //_player.GetComponent<AgentData>().grenade.count = 20;
            print("Game Load Success!");

        }
    }

    //public void ResetGame()
    //{
    //    if (File.Exists(Application.persistentDataPath + "/gameSave.save"))
    //    {
    //        SurrogateSelector surrogateSelector = new SurrogateSelector();
    //        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());

    //        BinaryFormatter binaryFormatter = new BinaryFormatter();

    //        binaryFormatter.SurrogateSelector = surrogateSelector;

    //        FileStream file = File.Open(Application.persistentDataPath + "/gameSave.save", FileMode.Open);
    //        SaveGame saveGame = (SaveGame)binaryFormatter.Deserialize(file);
    //        //file.Close();

    //        saveGame.latestCheckPoint = 0;
    //        saveGame.playerPos = Vector3.zero;

    //        binaryFormatter.Serialize(file, saveGame);
    //        file.Close();
    //        //_player.GetComponent<AgentData>().grenade.count = 20;
    //    }
    //}
}
