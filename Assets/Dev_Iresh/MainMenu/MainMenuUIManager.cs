using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] GameObject _sceneLoader;
    //[SerializeField] GameObject _saveGameManager;

    [SerializeField] GameObject _newGameWarningPanel;
    [SerializeField] GameObject _continueBtn;
    public int checkPoint = -1;
    int _previousScence = -1;

    private void Start()
    {
        //_saveGameManager.GetComponent<SaveGameManager>().LoadGame();

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
            _previousScence = saveGame.curentScence;

            print("_previousScence "+ _previousScence + " CheckPoint " + checkPoint + " Load Success!");
        }

        if (_previousScence >= 0 && checkPoint >= 0)
        {
            print("TRUE");
            _continueBtn.SetActive(true);
        }
    }
    public void SetLevels()
    {

    }

    public void CheckPlayBtnState()
    {
        if (checkPoint == null)
        {
            PlayGame();
        }
        else
        {
            _newGameWarningPanel.SetActive(true);
        }
    }

    public void ContinueGame()
    {
        SceneLoad(_previousScence);
    }

    public void PlayGame()
    {
        if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 0)
        {
            SceneLoad(3);
        }
        else if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 1)
        {
            SceneLoad(5);
        }
        else if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 2)
        {
            SceneLoad(6);
        }
        else if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 3)
        {
            SceneLoad(7);
        }
    }

    void SceneLoad(int index)
    {
        PlayerPrefs.SetInt("LoadScene", index);
        SceneManager.LoadScene(8, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
