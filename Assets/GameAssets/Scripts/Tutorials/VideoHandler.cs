using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoHandler : MonoBehaviour
{
    [SerializeField] UICanvasHandler _uICanvasHandler;

    [SerializeField] List<VideoClip> _videoClip;
    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] RenderTexture _renderTexture;

    public void OnEnable()
    {
        SelectClip();
    }

    public void SelectClip()
    {
        _videoPlayer.clip = _videoClip[PlayerPrefs.GetInt("loadVideo",0)];
        _renderTexture.Release();
        _videoPlayer.Play();
    }

    public void PlayVideo()
    {
        _videoPlayer.Play();
    }

    public void StopVideo()
    {
        _videoPlayer.Stop();
    }
   public void PauseVideo()
    {
        _videoPlayer.Pause();
    }

    public void CloseInstantPLayer()
    {
        StopVideo();
        _uICanvasHandler.CloseInstantVideoPlayerHome();
    }
}
