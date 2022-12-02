using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoHandler : MonoBehaviour
{
    [SerializeField] List<VideoClip> _videoClip;
    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] RenderTexture _renderTexture;

    public void SelectClip(int clipNo)
    {
        _videoPlayer.clip = _videoClip[clipNo];
        _renderTexture.Release();
        _videoPlayer.Play();
    }

    public void PlayVideo()
    {
        _videoPlayer.Play();
    }

    public void PauseVideo()
    {
        _videoPlayer.Pause();
    }

    public void StopVideo()
    {
        _videoPlayer.Stop();
    }

    public void ClosePLayer()
    {
        StopVideo();
    }
}
