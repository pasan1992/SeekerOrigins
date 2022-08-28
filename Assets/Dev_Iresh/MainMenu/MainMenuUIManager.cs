using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public DontDistroyManager dontDistroyManager;
    public AudioManager audioManager;

    [SerializeField] GameObject _splashScreen, _loading, _manu;

    #region Option Feild
        [SerializeField] Slider _musicSlider, _sfxSlider;
        [SerializeField] TMP_Dropdown _resulotionDropdown, _qualityDropdown;
        [SerializeField] Toggle _fullScreenToggle;

        Resolution[] _resolutions;

        const string MIXER_MUSIC = "MusicVolume";
        const string MIXER_SFX = "SFXVolume";
    #endregion

    #region Level Feild
        [SerializeField] List<Sprite> _bgList;

        [SerializeField] Image _levelBackground;
        [SerializeField] TMP_Text _levelMainTitle;
        [SerializeField] TMP_Text _levelSubTitle;
        [SerializeField] TMP_Text _levelDescription;

        [SerializeField] GameObject _levelIcon;
        [SerializeField] Image _levelIconImage;
        [SerializeField] TMP_Text _levelMIconTitle;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(OpenSplash());

        dontDistroyManager = GameObject.FindGameObjectWithTag("DontDistroyManager").GetComponent<DontDistroyManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        if (SceneManager.GetActiveScene().name.ToString() == GameEnums.Scence.Main_Menu.ToString())
        {
            if (!dontDistroyManager.isGameRunning)
            {
                SetSettings();
                dontDistroyManager.isGameRunning = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OpenSplash()
    {
        yield return new WaitForSeconds(5);
        _splashScreen.SetActive(false);
        _manu.SetActive(true);

        //_loading.SetActive(true);
        //StartCoroutine(CloseLoading());
    }

    //IEnumerator CloseLoading()
    //{
    //    yield return new WaitForSeconds(3);
    //    _loading.SetActive(false);
    //    _manu.SetActive(true);
    //}

    public void SetLevels()
    {

    }

    public void PlayGame()
    {
        SceneManager.LoadScene(GameEnums.Scence.Mission_00_ep1.ToString());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetSettings()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        _fullScreenToggle.isOn = GetFullScreen();
        GetResolution();
        GetQuality();
    }

    public void SetMusicVolume(float volume)
    {
        //audioManager.audioMixer.SetFloat(MIXER_MUSIC, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
     
    public void SetSfxVolume(float volume)
    {
        //audioManager.audioMixer.SetFloat(MIXER_SFX, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        if (isFullScreen)
        {
            PlayerPrefs.SetInt("isFullScreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("isFullScreen", 0);
        }
    }
    public bool GetFullScreen()
    {
        int isFullScreen = PlayerPrefs.GetInt("isFullScreen", 1);
        return isFullScreen == 1 ? true : false;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, GetFullScreen());
    }

    public void GetResolution()
    {
        _resolutions = Screen.resolutions;
        _resulotionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height + " @ " + _resolutions[i].refreshRate + "hz";
            options.Add(option);

            if (GetFullScreen())
            {
                if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            else
            {
                if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }
        }
        _resulotionDropdown.AddOptions(options);
        _resulotionDropdown.value = currentResolutionIndex;
        _resulotionDropdown.RefreshShownValue();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void GetQuality()
    {
        int value = QualitySettings.GetQualityLevel();
        _qualityDropdown.value = value;
    }
}
