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
	[SerializeField, Header("ヒットストップ")] private float hitStopWaitTime = 1.0f;
	[SerializeField, Header("ヒットストップ")] private float shakeTime = 0.3f;
	[SerializeField, Header("ヒットストップ")] private float shakePower = 50.0f;
	[SerializeField, Header("ヒットストップ")] private int shakeNum = 30;

	private GameObject targetImage;
	private RawImage targetRawImageComp;
	private RectTransform targetImageTrans;
	private Bloom bloom;
	private Camera m_camera;
	private Texture2D hitTex = null;

	void HitStopStart()
	{
		targetImage = GameObject.Find("HitStopImage");
		targetRawImageComp = targetImage.GetComponent<RawImage>();
		targetImageTrans = targetImage.GetComponent<RectTransform>();
		targetImage.SetActive(false);
	}

	void HitStopUpdate()
	{
		if(Input.GetKeyDown(KeyCode.T)){
			HitStopPerformance();
		}
	}

	private void HitStopPerformance(){
		Debug.Log("あ");
		BloomSetting(true);
		CaptureScreenShot();
		ShowImage();
		// テクスチャ動かす
		targetImageTrans.DOShakeAnchorPos(shakeTime, shakePower, shakeNum, 1, false, true);
		// 時間, 強さ, 振動数, 手振れ値, スナップフラグ, フェードアウト
		DOVirtual.DelayedCall(hitStopWaitTime, () =>
		{;
			BloomSetting(false);
			targetImage.SetActive(false);
			ui.SetPlaneDistance(2);
			hitTex = null;
		});
	}

	private void CaptureScreenShot()
	{
		targetRawImageComp.enabled = false;
		var cam = GetRenderCamera();
		var rt = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);
		var prev = cam.targetTexture;
		cam.targetTexture = rt;
		cam.Render();
		cam.targetTexture = prev;
		RenderTexture.active = rt;

		hitTex = new Texture2D(
			cam.pixelWidth,
			cam.pixelHeight,
			TextureFormat.RGB24,
			false);
		hitTex.ReadPixels(new Rect(0, 0, hitTex.width, hitTex.height), 0, 0);
		hitTex.Apply();
	}

	public void ShowImage()
	{
		targetRawImageComp.enabled = true;
		// NGUI の UITexture に表示
		targetRawImageComp.texture = hitTex;

		targetImage.SetActive(true);
	}

	private void BloomSetting(bool onoff)
	{
		if (volume.profile.TryGet<Bloom>(out bloom))
		{
			// ここでブルームの設定をいじる
			// めっちゃ明るくしたりしたい
			if(onoff){
				// bloom.intensity = ;
			}else{

			}
		}
	}
}
