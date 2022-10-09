using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    [SerializeField] TMP_Text _Messgae;
    [SerializeField] GameObject _player;
    public int checkPoint;

    public void SaveGame(int val)
    {
        checkPoint = val;
        SaveGame saveGame = CreateSaveGameObject();

        SurrogateSelector surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.SurrogateSelector = surrogateSelector;

        FileStream file = File.Create(Application.streamingAssetsPath + "/gameSave.save");

        binaryFormatter.Serialize(file, saveGame);
        file.Close();
        _Messgae.text = "CheckPoint " + checkPoint + "  Saved!";
        print("CheckPoint " + checkPoint + "  Saved!");
    }

    SaveGame CreateSaveGameObject()
    {
        SaveGame saveGame = new SaveGame();
        saveGame.latestCheckPoint = checkPoint;
        saveGame.playerPos = _player.transform.position;
        saveGame.playerRotaion = _player.transform.rotation.eulerAngles;

        return saveGame;
    }

    public void ResetGame(int val)
    {
        checkPoint = val;
        SaveGame saveGame = ResetSaveGameObject();

        SurrogateSelector surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.SurrogateSelector = surrogateSelector;

        FileStream file = File.Create(Application.streamingAssetsPath + "/gameSave.save");

        binaryFormatter.Serialize(file, saveGame);
        file.Close();

        _Messgae.text = "CheckPoint Reset Success!";

        print("CheckPoint Reset Success!");

    }

    SaveGame ResetSaveGameObject()
    {
        SaveGame saveGame = new SaveGame();
        saveGame.latestCheckPoint = 0;
        saveGame.playerPos = saveGame.df_l2_playerPos;
        saveGame.playerRotaion = new Vector3(0, saveGame.df_l2_playerDirection, 0);

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

            FileStream file = File.Open(Application.streamingAssetsPath + "/gameSave.save", FileMode.Open);
            SaveGame saveGame = (SaveGame)binaryFormatter.Deserialize(file);
            file.Close();

            checkPoint = saveGame.latestCheckPoint;
            _player.transform.position = saveGame.playerPos;
            _player.transform.rotation = Quaternion.Euler(saveGame.playerRotaion);

            //_player.transform.position = saveGame.playerPos;
            //_player.GetComponent<AgentData>().grenade.count = 20;
            _Messgae.text = "CheckPoint " + checkPoint + " Load Success!";

            print("CheckPoint " + checkPoint + " Load Success!");

        }
    }
}