using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject _loadingScreen;
    [SerializeField] Slider _slider;
    [SerializeField] Text _percentageTxt;
    [SerializeField] GameObject _videoPlayer;
    [SerializeField] GameObject _sceneLoader;
    [SerializeField] GameObject _bGMusic;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) 
        {
            Invoke("CloseSplash", 10);
        }
        else
        {
            Invoke("SceneLoad", 3); //LOAD
        }
    }
    void CloseSplash()
    {
        _videoPlayer.SetActive(false);
        _sceneLoader.SetActive(true);
        _bGMusic.SetActive(true);
        Invoke("InitiateCall", 5);
    }

    void InitiateCall()
    {
        LoadLevel(2);
    }

    void SceneLoad()
    {
        LoadLevel(PlayerPrefs.GetInt("LoadScene"));
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            _slider.value = progress;
            _percentageTxt.text = Mathf.Round(progress * 100) + "%";
            yield return null;
        }
    }
}
