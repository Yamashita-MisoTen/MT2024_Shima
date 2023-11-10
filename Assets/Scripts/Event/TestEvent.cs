using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvent : GameEvent
{
	protected override string eventName() => "ゴールデンマグロ";
	public override void StartEvent()
	{
		Debug.Log(eventName() + "どーーーーーーーーーーーーーーーーーｎ");
	}
}
