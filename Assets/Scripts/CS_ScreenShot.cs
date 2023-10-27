using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraScreenShotCapturer : MonoBehaviour
{
    [SerializeField] private Camera _camera;    //カメラ特定
    [SerializeField] private string path = "Assets/ScreenShot/";    //スクリーンショット保存先指定
    [SerializeField] private float timeOut;     //タイマー
    [SerializeField] private Volume volume;

    private GameObject targetImage;
    private float _time = 0;
    private bool hasCollided = false;
    private Bloom bloom;

    public void Start()
    {
        targetImage = GameObject.Find("RawImage");
        targetImage.SetActive(false);
    }

    public void Update()
    {
        if (hasCollided)
        {
            BloomOnOff(true);
            WaitForFlame();
            CaptureScreenShot(path + "ScreenShots.png");
            ShowImage();
        }

        if (Timer())
        {
            targetImage.SetActive(false);
            DeleteScreenShot();
        }
    }

    public void LateUpdate()
    {
        hasCollided = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        hasCollided = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        hasCollided = true;
    }

    IEnumerator WaitForFlame()
    {
        yield return null;
    }
    // カメラのスクリーンショットを保存する
    public void CaptureScreenShot(string filePath)
    {
        var rt = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 24);
        var prev = _camera.targetTexture;
        _camera.targetTexture = rt;
        _camera.Render();
        _camera.targetTexture = prev;
        RenderTexture.active = rt;

        var screenShot = new Texture2D(
            _camera.pixelWidth,
            _camera.pixelHeight,
            TextureFormat.RGB24,
            false);
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);
        screenShot.Apply();

        var bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);

        File.WriteAllBytes(filePath, bytes);

        BloomOnOff(false);
    }

    public void ShowImage()
    {
        if (!String.IsNullOrEmpty(path + "ScreenShots.png"))
        {
            targetImage.SetActive(true);

            byte[] image = File.ReadAllBytes(path + "ScreenShots.png");

            Texture2D tex = new Texture2D(0, 0);
            tex.LoadImage(image);

            RawImage target = targetImage.GetComponent<RawImage>();
            target.texture = tex;
        }
    }

    public void DeleteScreenShot()
    {
        AssetDatabase.DeleteAsset(path + "ScreenShots.png");
    }

    public bool Timer()
    {
        _time += Time.deltaTime;

        if (_time >= timeOut)
        {
            _time = 0;
            return true;
        }
        else
            return false;
    }

    public void BloomOnOff(bool onoff)
    {
        if (volume.profile.TryGet<Bloom>(out bloom))
        {
            bloom.active = onoff;
        }
    }
}