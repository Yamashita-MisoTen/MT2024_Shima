using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pf_GoldenFishEvent : GameEvent
{
	[SerializeField] private GameObject GoldenFish;
	[SerializeField] private GameObject GoldenFish2;
	[SerializeField] private Vector3 zahyou1;
	[SerializeField] private Vector3 zahyou2;

	protected override string eventName() => "â©ã‡ãõî≠ê∂";
	public override void StartEvent()
	{
		Debug.Log(eventName() + "ooooooooooo");

		var obj =  Instantiate(GoldenFish, zahyou1, Quaternion.identity);
		var obj2 = Instantiate(GoldenFish2, zahyou2, Quaternion.identity);

		NetworkServer.Spawn(obj);
		NetworkServer.Spawn(obj2);
	}
}
