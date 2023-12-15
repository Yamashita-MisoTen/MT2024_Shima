using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class CPlayer : NetworkBehaviour
{
	// ** 黄金魚関連
	[SerializeField] GameObject mesh;
	bool isHaveGoldenFish = false;
	GameObject goldenFish;
	// ** 流氷イベント関連
	bool isConfusion = false;		// 混乱する
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

	// ** 黄金魚関連関数
	public void GetGoldenFish(float addSpeed, GameObject fish){
		isHaveGoldenFish = true;
		Velocity_Addition += addSpeed;
		goldenFish = new GameObject();
		goldenFish = fish;
		fish.transform.parent = mesh.transform;

		//プレイヤーのアニメーション設定
		SetGoldFishTrigger();

		// 顔に合うようにローカル座標・向き・大きさを変更する
		fish.transform.localPosition = new Vector3(0f,0f,0f);
		fish.transform.eulerAngles = new Vector3(90f,0f,-90f);
		fish.transform.localScale = new Vector3(2000f,2000f,2000f);
	}

	public void FinishGoldenFishEvent(float minusSpeed){
		isHaveGoldenFish = false;
		Velocity_Addition -= minusSpeed;
	}

	// ** 流氷関連関数
	public void HitDriftIce(float confusiontime){
		confusionTime = confusiontime;
	}

	// ** スピードアップイベント関数
	public void SetUpSpeedUpEvent(float addSpeed){
		Debug.Log("増えます");
		Velocity_Addition += addSpeed;
	}

	public void FinishSpeedUpEvent(float minusSpeed){
		Debug.Log("消えます");
		Velocity_Addition -= minusSpeed;
	}
}
