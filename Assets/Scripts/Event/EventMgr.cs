using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;

public class EventMgr : NetworkBehaviour
{
	[SerializeField] List<GameEvent> eventKind;
	[SerializeField] GameObject eventUIObj;
	[SerializeField] EventUI eventUI;
	GameEvent eventObj;
	float eventTime = 0f;
	float requireEventTime = 0f;
	bool isEventNow = false;
	void Start(){
		eventUI = Instantiate(eventUIObj).GetComponent<EventUI>();
	}
	void Update(){
		if(!isServer) return;
		if(!isEventNow) return;

		requireEventTime += Time.deltaTime;
		if(requireEventTime >= eventTime){
			eventObj.FinishEvent();
			requireEventTime = 0f;
			eventUI.StopEventBeltUI();
			eventObj =null;
		}
		var ratio = requireEventTime / eventTime;
		RpcSetRatio(ratio);
	}

	[ClientRpc]
	void RpcSetRatio(float ratio){
		eventUI.SetRatio(ratio);
	}

	public void SetUpUI(GameObject playerObj){
		for(int i = 0 ; i < playerObj.transform.childCount; i++){
			var obj = playerObj.transform.GetChild(i);
			if(obj.name == "PlayerCamera"){
				if(eventUI == null) eventUI = eventUIObj.GetComponent<EventUI>();
				eventUI.SetCameraData(obj.GetComponent<Camera>());
			}
		}
	}

	public void RandomEventLottery(){
		if(eventKind == null) return;

		var eventnum = Random.Range(0, eventKind.Count);
		Debug.Log(eventKind[eventnum].GetEventName() + "発生");
		if(isServer) {
			eventObj = Instantiate(eventKind[eventnum]);
			NetworkServer.Spawn(eventObj.gameObject);
			eventObj.StartEvent();
			eventTime = eventObj.eventTime();
			isEventNow = true;
		}

		StartPerformance(eventKind[eventnum].GetEventName(), eventKind[eventnum].GetExplanatory());
	}

	void StartPerformance(string name, string explanatory){
		eventUI.StartUpEventUI(name, explanatory);
		DOVirtual.DelayedCall(3f,() => eventUI.StopEventUI());
	}
}
