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
		obj.GetComponent<GoldFishMove>().SetUpEventData(eventTime, addSpeed);
		var obj2 = Instantiate(GoldenFish2, zahyou2, Quaternion.identity);
		obj2.GetComponent<GoldFishMove2>().SetUpEventData(eventTime, addSpeed);

		NetworkServer.Spawn(obj);
		NetworkServer.Spawn(obj2);
	}
}
