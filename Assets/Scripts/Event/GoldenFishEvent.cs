using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pf_GoldenFishEvent : GameEvent
{
	[SerializeField] private float eventTime = 30f;
	[SerializeField] private float addSpeed = 5f;
	[SerializeField] private GameObject GoldenFish;
	[SerializeField] private GameObject GoldenFish2;
	[SerializeField] private Vector3 zahyou1;
	[SerializeField] private Vector3 zahyou2;

	protected override string eventName() => "������";
    protected override string eventExplanatory() => "�G���A����\"������\"���o���I�I�D�������I�I";
    public override void StartEvent()
	{
		Debug.Log(eventName() + "ooooooooooo");

		var obj =  Instantiate(GoldenFish, zahyou1, Quaternion.identity);
		var obj2 = Instantiate(GoldenFish2, zahyou2, Quaternion.identity);
		// �֐��ĂԑO�ɃT�[�o�[�ɓo�^����
		NetworkServer.Spawn(obj);
		NetworkServer.Spawn(obj2);
		RpcSetUpGoldenFish(obj.GetComponent<GoldFishMove>());
		RpcSetUpGoldenFish(obj2.GetComponent<GoldFishMove2>());
	}

	[ClientRpc]
	private void RpcSetUpGoldenFish(GoldFishMove comp){
		comp.SetUpEventData(eventTime, addSpeed);
	}
	[ClientRpc]
	private void RpcSetUpGoldenFish(GoldFishMove2 comp){
		comp.SetUpEventData(eventTime, addSpeed);
	}
}
