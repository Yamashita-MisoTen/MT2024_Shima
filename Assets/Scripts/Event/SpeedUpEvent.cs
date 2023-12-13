using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpeedUpEvent : GameEvent
{
	[SerializeField] float eventTime = 30f;
	[SerializeField] float AddSpeedVelocity = 30f;
	protected override string eventName() => "スピードアップ";
	protected override string eventExplanatory() => "説明文";
	List<CPlayer> playerData;
	float requireTime = 0f;
	// 継承する用のスタート関数
	public override void StartEvent() {
		// GameObject型の配列cubesに、"box"タグのついたオブジェクトをすべて格納
		GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
		playerData = new List<CPlayer>();
		foreach(GameObject obj in player){
			var p = obj.GetComponent<CPlayer>();
			playerData.Add(p);
		}
		if(isServer) RpcSetUpSpeed(playerData, AddSpeedVelocity);
	}

	void Update(){
		requireTime += Time.deltaTime;
		if(requireTime >= eventTime){
			// 終了処理
			if(isServer) {
				Debug.Log(playerData[0].name);
				Debug.Log("終了");
				RpcFinishSpeed(playerData, AddSpeedVelocity);
				requireTime = 0f;
				DOVirtual.DelayedCall(0.1f ,() => NetworkServer.Destroy(this.gameObject));
			}
		}
	}

	[ClientRpc]
	private void RpcSetUpSpeed(List<CPlayer> data, float velocity){
		if(playerData == null) playerData = new List<CPlayer>();
		playerData = data;
		foreach(CPlayer p in playerData){
			p.SetUpSpeedUpEvent(velocity);
		}
	}
	[ClientRpc]
	private void RpcFinishSpeed(List<CPlayer> data, float velocity){
		if(playerData == null) playerData = new List<CPlayer>();
		playerData = data;
		foreach(CPlayer p in playerData){
			p.FinishSpeedUpEvent(velocity);
		}
	}
}
