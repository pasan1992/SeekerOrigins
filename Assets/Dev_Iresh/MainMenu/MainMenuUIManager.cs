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
    int _previousScence = -1;

    private void Start()
    {
        var _previousScence = SaveData.GetLatestSceneID();

        if (_previousScence >= 0)
        {
            _continueBtn.SetActive(true);
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
        SceneLoad(_previousScence);
    }

    public void PlayGame()
    {
        if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 0)
        {
            SceneLoad(4);
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
