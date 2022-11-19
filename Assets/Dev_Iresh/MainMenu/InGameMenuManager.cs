using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuManager : MonoBehaviour
{
    [SerializeField] GameObject _sceneLoader;
    [SerializeField] GameObject _menu;
    [SerializeField] int _btnNo = 0;

    [SerializeField] Text _message;


    bool _isOpenMenu = false;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    GamePaused();
        //}
    }

    public void GamePaused()
    {
        if (!_isOpenMenu)
        {
            Time.timeScale = 0;
            _menu.SetActive(true);
            _isOpenMenu = true;
        }
        else
        {
            Time.timeScale = 1;
            _menu.SetActive(false);
            _isOpenMenu = false;
        }
    }

    public void btnNoChange(int btnNo)
    {
        _btnNo = btnNo;

        if (btnNo == 1 || btnNo == 2)
        {
            _message.text = "Do you want to quit from the mission?";
        }
        else if (btnNo == -2)
        {
            _message.text = "Do you want to reset of the current mission?";
        }
        else if (btnNo == -3)
        {
            _message.text = "Do you want to start from the previous checkpoint?";
        }
    }

    public void LoadScene(int index)
    {
        Time.timeScale = 1;

        if (index == -3){
            _sceneLoader.SetActive(true);

            if (_btnNo == -2)
            {
                //TODO: set checkpoint to 0
                _sceneLoader.GetComponent<SceneLoader>().LoadLevel(SceneManager.GetActiveScene().buildIndex);
            }
            else if (_btnNo == -3)
            {
                _sceneLoader.GetComponent<SceneLoader>().LoadLevel(SceneManager.GetActiveScene().buildIndex);
            }
            else
            _sceneLoader.GetComponent<SceneLoader>().LoadLevel(_btnNo);
        }
        else
        {
            _sceneLoader.SetActive(true);
            _sceneLoader.GetComponent<SceneLoader>().LoadLevel(index);
        }
    }
}
