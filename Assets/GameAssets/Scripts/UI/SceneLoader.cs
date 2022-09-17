using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject _loadingScreen;
    [SerializeField] Slider _slider;
    [SerializeField] Text _percentageTxt;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
           Invoke("InitiateCall",4); 
        }
    }

    void InitiateCall()
    {
        LoadLevel(1);
    }

    public void LoadLevel(int sceneIndex)
    {
        //_loadingScreen.SetActive(true);
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
