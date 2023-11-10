using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EventMgr : NetworkBehaviour
{
	[SerializeField] List<GameEvent> eventKind;
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}
	public void RandomEventLottery(){
		if(eventKind == null) return;

		var eventnum = Random.Range(0, eventKind.Count);
		Debug.Log(eventKind[eventnum].GetEventName() + "発生");
		eventKind[eventnum].StartEvent();
	}
}
