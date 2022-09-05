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

    bool _isPEnable = true;

    private void Update()
    {
        if (_isPEnable)
        {
            if (Input.GetKey(KeyCode.P) || Input.GetKey(KeyCode.Escape))
            {
                _isPEnable = false;
                Time.timeScale = 0;

                _menu.SetActive(true);
            }
        }
    }

    public void btnNoChange(int btnNo)
    {
        _btnNo = btnNo;
    }

    public void LoadScene(int index)
    {
        Time.timeScale = 1;

        if (index == -1) {
            _isPEnable = true;
        }

        else if(index == -3){
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
