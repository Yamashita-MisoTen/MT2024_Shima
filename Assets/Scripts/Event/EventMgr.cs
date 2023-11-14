using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EventMgr : NetworkBehaviour
{
	[SerializeField] List<GameEvent> eventKind;
	// Start is called before the first frame update

	public void RandomEventLottery(){
		if(eventKind == null) return;

		StartPerformance();

		var eventnum = Random.Range(0, eventKind.Count);
		Debug.Log(eventKind[eventnum].GetEventName() + "発生");
		eventKind[eventnum].StartEvent();
	}

	void StartPerformance(){

	}
}
