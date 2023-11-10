using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ScreenShot : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Volume volume;
    [SerializeField] private string path = "Assets/ScreenShots/";
    [SerializeField] private string fileName = "ScreenShot.png";
    [SerializeField] private float timeOut = 3.0f;

    private GameObject targetImage;
    private Bloom bloom;
    private bool getHited = false;
    private float seconds = 0;

    private void Start()
    {
        targetImage = GameObject.Find("RawImage");
        targetImage.SetActive(false);
        BloomOnOff(false);
        seconds = 0;
    }

    private void Update()
    {
        if (getHited)
        {
            seconds += Time.deltaTime;
            if (seconds >= timeOut)
            {
                targetImage.SetActive(false);
                DeleteFile(path + fileName);
                seconds = 0;
                BloomOnOff(false);
                getHited = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (getHited == true) 
        {
            return;
        }
        getHited = true;
        BloomOnOff(true);
        StartCoroutine(DelayCoroutine(5));
        CaptureScreenShot(path + fileName);
        ShowImage();
    }

    // カメラのスクリーンショットを保存する
    private void CaptureScreenShot(string filePath)
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
    }

    public void ShowImage()
    {
        if (!String.IsNullOrEmpty(path + fileName))
        {
            byte[] image = File.ReadAllBytes(path + fileName);

            Texture2D tex = new Texture2D(0, 0);
            tex.LoadImage(image);

            // NGUI の UITexture に表示
            RawImage target = targetImage.GetComponent<RawImage>();
            target.texture = tex;

            targetImage.SetActive(true);
        }
    }

    private void BloomOnOff(bool onoff)
    {
        if (volume.profile.TryGet<Bloom>(out bloom))
        {
            bloom.active = onoff;
        }
    }

    private IEnumerator DelayCoroutine(int flame)
    {
        for (var i = 0; i < flame; i++)
        {
            yield return null;
        }
    }

    private void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }
}
