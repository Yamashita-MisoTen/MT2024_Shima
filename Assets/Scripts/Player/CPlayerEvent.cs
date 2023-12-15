using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class CPlayer : NetworkBehaviour
{
	// ** �������֘A
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
	public void GetGoldenFish(float addSpeed){
		isHaveGoldenFish = true;
		Velocity_Addition += addSpeed;
		SetGoldFishTrigger();
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
