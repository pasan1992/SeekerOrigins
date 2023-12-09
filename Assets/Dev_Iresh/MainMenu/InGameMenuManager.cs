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

    public void GamePaused()
    {
        if (!_isOpenMenu)
        {
            Time.timeScale = 0;
            _isOpenMenu = true;
        }
        else
        {
            Time.timeScale = 1;
            _isOpenMenu = false;
        }
    }

    public void btnNoChange(int btnNo)
    {
        Debug.Log(btnNo);
        _btnNo = btnNo;

        if (btnNo == 2 || btnNo == 3)
        {
            _message.text = "Do you want to quit from the mission?";
        }
        else if (btnNo == -2 )
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
        print("LoadScene");
        Time.timeScale = 1;

        if(_btnNo == -2)
        {
                SaveGameManager.getInstance().ResetLevel();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
                return;
        }

        else if(_btnNo == -3)
        {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
                return;
        }

        //else if (_btnNo == 2 || _btnNo == 3)
        //{
        //    print("Application.Quit");

        //    Application.Quit();
        //}
        if (index == -3)
        {
            if (_btnNo == -2) // to Reset
            {
                SaveGameManager.getInstance().ResetLevel();
                PlayerPrefs.SetInt("LoadScene", SceneManager.GetActiveScene().buildIndex);
            }
            else if (_btnNo == -3) // to Reset checkpoint to 0
            {
                //TODO: set checkpoint to 0
                PlayerPrefs.SetInt("LoadScene", SceneManager.GetActiveScene().buildIndex);
            }
            else if (_btnNo == 2) // to MainMenu
            {
                PlayerPrefs.SetInt("LoadScene", _btnNo);
            }
            else if ( _btnNo == 3) //to TUtorials
            {
                PlayerPrefs.SetInt("LoadScene", _btnNo);
            }
            else
            {
                PlayerPrefs.SetInt("LoadScene", _btnNo);
            }
        }
        else
        {
            PlayerPrefs.SetInt("LoadScene", index);
        }

        SceneManager.LoadScene(1, LoadSceneMode.Single); //LOAD

        //SceneManager.LoadScene(index, LoadSceneMode.Single); //LOAD


        //SceneManager.UnloadSceneAsync(PlayerPrefs.GetInt("CurrentScene"));
        //SceneManager.LoadScene(0, LoadSceneMode.Additive);
        //
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Initial Scene"));
        //print("ADDITIVE");

        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //SceneManager.UnloadSceneAsync(PlayerPrefs.GetInt("CurrentScene"));
        //SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("EP01_A"));
        //print(SceneManager.sceneCount);
        //// Get count of loaded Scenes and last index
        //var lastSceneIndex = SceneManager.sceneCount - 1;

        //// Get last Scene by index in all loaded Scenes
        //var lastLoadedScene = SceneManager.GetSceneAt(lastSceneIndex);

        // Unload Scene
        //SceneManager.UnloadSceneAsync(lastLoadedScene);
        //SceneManager.UnloadSceneAsync(0);

        //Scene gameScene = SceneManager.GetSceneByName("Initial Scene");
        //if (gameScene.isLoaded)
        //{
        //    StartCoroutine(SetActive(gameScene));
        //    var sceneObjs = gameScene.GetRootGameObjects();
        //    print(gameScene.GetRootGameObjects().Length);
        //}


        //foreach (var root in gameScene.GetRootGameObjects())
        //{
        //    print("WORKS");
        //}

        //var sceneObjs = gameScene.GetRootGameObjects();
        //print(gameScene.GetRootGameObjects().Length);
        //print(gameScene.GetRootGameObjects()[3].name);

        //_sceneLoader = sceneObjs[3];

        //if (index == -3)
        //{
        //    if (_btnNo == -2)
        //    {
        //        //TODO: set checkpoint to 0
        //        _sceneLoader.GetComponent<SceneLoader>().LoadLevel(SceneManager.GetActiveScene().buildIndex);
        //    }
        //    else if (_btnNo == -3)
        //    {
        //        _sceneLoader.GetComponent<SceneLoader>().LoadLevel(SceneManager.GetActiveScene().buildIndex);
        //    }
        //    else
        //        _sceneLoader.GetComponent<SceneLoader>().LoadLevel(_btnNo);
        //}
        //else
        //{
        //    _sceneLoader.SetActive(true);
        //    _sceneLoader.GetComponent<SceneLoader>().LoadLevel(index);
        //}
        //

        //if (index == -3)
        //{


        //    _sceneLoader.SetActive(true);

        //    if (_btnNo == -2)
        //    {
        //        //TODO: set checkpoint to 0
        //        _sceneLoader.GetComponent<SceneLoader>().LoadLevel(SceneManager.GetActiveScene().buildIndex);
        //    }
        //    else if (_btnNo == -3)
        //    {
        //        _sceneLoader.GetComponent<SceneLoader>().LoadLevel(SceneManager.GetActiveScene().buildIndex);
        //    }
        //    else
        //        _sceneLoader.GetComponent<SceneLoader>().LoadLevel(_btnNo);
        //}
        //else
        //{
        //    _sceneLoader.SetActive(true);
        //    _sceneLoader.GetComponent<SceneLoader>().LoadLevel(index);
        //}
    }

    public IEnumerator SetActive(Scene scene)
    {
        int i = 0;
        while (i == 0)
        {
            i++;
            yield return null;
        }
        SceneManager.SetActiveScene(scene);
        yield break;
    }
}
