using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;

public class HappenWhirloop : GameEvent
{
	[SerializeField] float eventTime = 30f;
	[SerializeField] List<GameObject> whirloopList;
	[SerializeField] int createWhirloopNum = 10;
	List<GameObject> whirloopObjList;
	float requireTime = 0f;
	protected override string eventName() => "水流発生";
	protected override string eventExplanatory() => "エリア内に水流が大量発生中！！";
	public override void StartEvent()
	{
		whirloopObjList = new List<GameObject>();

		for(int i = 0; i < createWhirloopNum; i++){
			// 番号指定
			int num = Random.Range(0, whirloopList.Count);
			// エリア設定
			Vector3 position = new Vector3(0,0,0);
			// 向き設定
			Quaternion rotation = Quaternion.identity;
			int angle = Random.Range(0,361);
			rotation = Quaternion.AngleAxis(angle, Vector3.up);
			// オブジェクトの生成
			var obj = Instantiate(whirloopList[num], position, rotation);
			NetworkServer.Spawn(obj);
		}
	}

	void Update(){
		requireTime += Time.deltaTime;
		if(requireTime >= eventTime){
			FinishEvent();
		}
	}

	void FinishEvent(){
		for(int i = 0; i < whirloopObjList.Count; i++){
			if(whirloopObjList[i] == null) continue;
			NetworkServer.Destroy(whirloopObjList[i]);
		}
	}
}
