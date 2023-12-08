using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;
using UnityEngine.UI;

public partial class CPlayer
{
    [SerializeField] private Volume volume;
    [SerializeField] private string path = "Assets/ScreenShots/";
    [SerializeField] private string fileName = "ScreenShot.png";

    private GameObject targetImage;
    private Bloom bloom;
    private Camera m_camera;
    private bool getHited = false;

    void ScreenShotStart()
    {
        volume = GetComponent<Volume>();
        targetImage = GameObject.Find("RawImage");
        targetImage.SetActive(false);
        BloomOnOff(false);
        m_camera = renderCamera;
    }

    void ScreenShotUpdate()
    {
        if(getHited)
        {
            BloomOnOff(false);
            DOVirtual.DelayedCall(3.0f, () =>
            {;
                targetImage.SetActive(false);
                DeleteFile(path + fileName);
                getHited = false;
            });
        }
    }

    //“¯‚¶ŠÖ”Žg‚Á‚Ä‚©‚çA‚±‚±‚ðC³‘Ò‚¿
    //private void OnCollisionEnter(Collision other)
    //{
    //    if (getHited == true || !other.gameObject.CompareTag("Player") || !_isNowOrga) 
    //    {
    //        return;
    //    }
    //    getHited = true;
    //    BloomOnOff(true);
    //    DOVirtual.DelayedCall(0.1f, () =>
    //    {
    //        CaptureScreenShot(path + fileName);
    //    });
    //    ShowImage();
    //}

    private void CaptureScreenShot(string filePath)
    {
        var rt = new RenderTexture(m_camera.pixelWidth, m_camera.pixelHeight, 24);
        var prev = m_camera.targetTexture;
        m_camera.targetTexture = rt;
        m_camera.Render();
        m_camera.targetTexture = prev;
        RenderTexture.active = rt;

        var screenShot = new Texture2D(
            m_camera.pixelWidth,
            m_camera.pixelHeight,
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

            // NGUI ‚Ì UITexture ‚É•\Ž¦
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

    private void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }
}
