using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pf_GoldenFishEvent : GameEvent
{
	[SerializeField] private float addSpeed = 5f;
	[SerializeField] private GameObject GoldenFish;
	[SerializeField] private GameObject GoldenFish2;
	[SerializeField] private Vector3 zahyou1;
	[SerializeField] private Vector3 zahyou2;

	protected override string eventName() => "������";
	protected override string eventExplanatory() => "\"������\"���o���I�I�D�������I�I";
	[SerializeField] public override float eventTime() => 30f;

	public override void StartEvent()
	{
		var obj =  Instantiate(GoldenFish, zahyou1, Quaternion.identity);
		var obj2 = Instantiate(GoldenFish2, zahyou2, Quaternion.identity);
		// �֐��ĂԑO�ɃT�[�o�[�ɓo�^����
		NetworkServer.Spawn(obj);
		NetworkServer.Spawn(obj2);
		RpcSetUp(obj.GetComponent<GoldFishMove>(), obj2.GetComponent<GoldFishMove2>());
	}

	[ClientRpc]
	void RpcSetUp(GoldFishMove comp1, GoldFishMove2 comp2){
		comp1.SetUpEventData(eventTime(), addSpeed);
		comp2.SetUpEventData(eventTime(), addSpeed);
	}
}
