using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappenWhirloop : GameEvent
{
	[SerializeField] List<GameObject> whirloopList;
	protected override string eventName() => "渦潮発生";
	public override void StartEvent()
	{
		Debug.Log(eventName() + "どーーーーーーーーーーーーーーーーーｎ");
	}
}
