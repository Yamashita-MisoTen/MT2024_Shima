using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class GameEvent : NetworkBehaviour
{
	protected virtual string eventName() => "name";
	protected virtual string eventExplanatory() => "説明文";
	[SerializeField] public virtual float eventTime() => 0f;
	// 継承する用のスタート関数
	public virtual void StartEvent() {}
	public virtual void FinishEvent() {}

	public string GetEventName(){
		return eventName();
	}

	public string GetExplanatory(){
		return eventExplanatory();
	}
}
