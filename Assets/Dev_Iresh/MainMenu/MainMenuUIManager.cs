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
        _sceneLoader.SetActive(true);

        _sceneLoader.GetComponent<SceneLoader>().LoadLevel(_previousScence);
    }

    public void PlayGame()
    {

        _sceneLoader.SetActive(true);
        //SceneManager.LoadScene(GameEnums.Scence.Mission_00_ep1.ToString());

        //if (true)
        //{
        //    GameEnums.Scence.Mission_00_ep1.ToString());
        //}
        if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 0)
        {
            //SceneManager.LoadScene(0);
            _sceneLoader.GetComponent<SceneLoader>().LoadLevel(3);
            //SceneManager.LoadScene(GameEnums.Scence.Mission_00_ep1.ToString());
        }
        else if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 1)
        {
            _sceneLoader.GetComponent<SceneLoader>().LoadLevel(5);

            //SceneManager.LoadScene(GameEnums.Scence.Mission_02.ToString());
        }
        else if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 2)
        {
            _sceneLoader.GetComponent<SceneLoader>().LoadLevel(6);

            //SceneManager.LoadScene(GameEnums.Scence.Mission_01.ToString());
        }
        else if (transform.GetComponent<UILevelsHandler>()._currantLevelID == 3)
        {
            _sceneLoader.GetComponent<SceneLoader>().LoadLevel(7);

            //SceneManager.LoadScene(GameEnums.Scence.Mission_01.ToString());
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //public void SetSettings()
    //{
    //    _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
    //    _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
    //    _fullScreenToggle.isOn = GetFullScreen();
    //    GetResolution();
    //    GetQuality();
    //}

    //public void SetMusicVolume(float volume)
    //{
    //    //audioManager.audioMixer.SetFloat(MIXER_MUSIC, Mathf.Log10(volume) * 20);
    //    PlayerPrefs.SetFloat("MusicVolume", volume);
    //}
     
    //public void SetSfxVolume(float volume)
    //{
    //    //audioManager.audioMixer.SetFloat(MIXER_SFX, Mathf.Log10(volume) * 20);
    //    PlayerPrefs.SetFloat("SFXVolume", volume);
    //}

    //public void SetFullScreen(bool isFullScreen)
    //{
    //    Screen.fullScreen = isFullScreen;

    //    if (isFullScreen)
    //    {
    //        PlayerPrefs.SetInt("isFullScreen", 1);
    //    }
    //    else
    //    {
    //        PlayerPrefs.SetInt("isFullScreen", 0);
    //    }
    //}
    //public bool GetFullScreen()
    //{
    //    int isFullScreen = PlayerPrefs.GetInt("isFullScreen", 1);
    //    return isFullScreen == 1 ? true : false;
    //}

    //public void SetResolution(int resolutionIndex)
    //{
    //    Resolution resolution = _resolutions[resolutionIndex];
    //    Screen.SetResolution(resolution.width, resolution.height, GetFullScreen());
    //}

    //public void GetResolution()
    //{
    //    _resolutions = Screen.resolutions;
    //    _resulotionDropdown.ClearOptions();
    //    List<string> options = new List<string>();

    //    int currentResolutionIndex = 0;

    //    for (int i = 0; i < _resolutions.Length; i++)
    //    {
    //        string option = _resolutions[i].width + " x " + _resolutions[i].height + " @ " + _resolutions[i].refreshRate + "hz";
    //        options.Add(option);

    //        if (GetFullScreen())
    //        {
    //            if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
    //            {
    //                currentResolutionIndex = i;
    //            }
    //        }
    //        else
    //        {
    //            if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
    //            {
    //                currentResolutionIndex = i;
    //            }
    //        }
    //    }
    //    _resulotionDropdown.AddOptions(options);
    //    _resulotionDropdown.value = currentResolutionIndex;
    //    _resulotionDropdown.RefreshShownValue();
    //}

    //public void SetQuality(int qualityIndex)
    //{
    //    QualitySettings.SetQualityLevel(qualityIndex);
    //}

    //public void GetQuality()
    //{
    //    int value = QualitySettings.GetQualityLevel();
    //    _qualityDropdown.value = value;
    //}
}
