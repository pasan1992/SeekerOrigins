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

    bool _isOpenMenu = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePaused();
        }
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
    }

    public void LoadScene(int index)
    {
        if(index == -3){
            _sceneLoader.SetActive(true);

            if (_btnNo == -2)
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
