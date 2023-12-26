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
    [SerializeField] GameObject _endlessBtn;
    int _previousScence = -1;

    private void Start()
    {
        //var _previousScence = SaveData.GetLatestSceneID();
        var latest_scene = PlayerPrefs.GetInt("LatestLevel",-1);
        var isEndlessActiveted = PlayerPrefs.GetInt("isEndlessActiveted", 0);

        if (latest_scene >= 0)
        {
            _continueBtn.SetActive(true);
        }
        else
        {
            _continueBtn.SetActive(false);
        }
        if (isEndlessActiveted == 1)
        {
            _endlessBtn.SetActive(true);
        }
    }

    public void CheckPlayBtnState()
    {
        if (_previousScence == -1)
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
        var latest_scene = PlayerPrefs.GetInt("LatestLevel",-1);
        if (latest_scene >= 0)
        {
            SceneLoad(latest_scene);
        }
    }

    public void PlayEndless()
    {
        SceneLoad(8);
    }

    public void PlayGame()
    {
        if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 0)
        {
            SceneLoad(9);
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
            SceneLoad(8);
        }
    }

    void SceneLoad(int index)
    {
        PlayerPrefs.SetInt("LoadScene", index);
        SceneManager.LoadScene(1, LoadSceneMode.Single); //LOAD
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
