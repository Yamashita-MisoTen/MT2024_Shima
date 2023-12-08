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
	[SerializeField, Header("�q�b�g�X�g�b�v")] private float hitStopWaitTime = 1.0f;
	[SerializeField, Header("�q�b�g�X�g�b�v")] private float shakeTime = 0.3f;
	[SerializeField, Header("�q�b�g�X�g�b�v")] private float shakePower = 50.0f;
	[SerializeField, Header("�q�b�g�X�g�b�v")] private int shakeNum = 30;

	private GameObject targetImage;
	private RectTransform targetImageTrans;
	private Bloom bloom;
	private Camera m_camera;
	private Texture2D hitTex = null;

	void HitStopStart()
	{
		targetImage = GameObject.Find("HitStopImage");
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
		BloomSetting(true);
		CaptureScreenShot();
		ShowImage();
		ui.SetPlaneDistance(1);
		// �e�N�X�`��������
		targetImageTrans.DOShakeAnchorPos(shakeTime, shakePower, shakeNum, 1, false, true);
		// ����, ����, �U����, ��U��l, �X�i�b�v�t���O, �t�F�[�h�A�E�g
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
		// NGUI �� UITexture �ɕ\��
		RawImage target = targetImage.GetComponent<RawImage>();
		target.texture = hitTex;

		targetImage.SetActive(true);
	}

	private void BloomSetting(bool onoff)
	{
		if (volume.profile.TryGet<Bloom>(out bloom))
		{
			// �����Ńu���[���̐ݒ��������
			// �߂����ᖾ�邭�����肵����
			if(onoff){

			}else{

			}
		}
	}
}
