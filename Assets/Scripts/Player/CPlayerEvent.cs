using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class CPlayer : NetworkBehaviour
{
	// ** �������֘A
	[SerializeField] GameObject mesh;
	bool isHaveGoldenFish = false;
	GameObject goldenFish;
	// ** ���X�C�x���g�֘A
	bool isConfusion = false;		// ��������
	float requireConfusionTime = 0f;
	float confusionTime = 0f;
	// Start is called before the first frame update
	void EventStart()
	{

	}

	// Update is called once per frame
	void EventUpdate()
	{
		// if(){

		// }
	}

	// ** �������֘A�֐�
	public void GetGoldenFish(float addSpeed, GameObject fish){
		isHaveGoldenFish = true;
		Velocity_Addition += addSpeed;
		goldenFish = new GameObject();
		goldenFish = fish;
		fish.transform.parent = mesh.transform;

		//�v���C���[�̃A�j���[�V�����ݒ�
		SetGoldFishTrigger();

		// ��ɍ����悤�Ƀ��[�J�����W�E�����E�傫����ύX����
		fish.transform.localPosition = new Vector3(0f,0f,0f);
		fish.transform.eulerAngles = new Vector3(90f,0f,-90f);
		fish.transform.localScale = new Vector3(2000f,2000f,2000f);
	}

	public void FinishGoldenFishEvent(float minusSpeed){
		isHaveGoldenFish = false;
		Velocity_Addition -= minusSpeed;
	}

	// ** ���X�֘A�֐�
	public void HitDriftIce(float confusiontime){
		confusionTime = confusiontime;
	}

	// ** �X�s�[�h�A�b�v�C�x���g�֐�
	public void SetUpSpeedUpEvent(float addSpeed){
		Debug.Log("�����܂�");
		Velocity_Addition += addSpeed;
	}

	public void FinishSpeedUpEvent(float minusSpeed){
		Debug.Log("�����܂�");
		Velocity_Addition -= minusSpeed;
	}
}
