using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class HappenWhirloop : GameEvent
{
	[SerializeField] float eventTime = 30f;
	[SerializeField] List<GameObject> whirloopList;
	[SerializeField] int createWhirloopNum = 10;
	[SerializeField] Vector2 mapSize;
	List<GameObject> whirloopObjList;
	float requireTime = 0f;
	protected override string eventName() => "水流発生";
	protected override string eventExplanatory() => "エリア内に水流が大量発生中！！";
	public override void StartEvent()
	{
		whirloopObjList = new List<GameObject>();

		// 分割数を設定する
		var x = mapSize.x / 20;
		var y = mapSize.y / 20;

		for(int i = 0; i < createWhirloopNum; i++){
			// 番号指定
			int num = Random.Range(0, whirloopList.Count);
			// エリア設定
			Vector3 position = new Vector3(0,0,0);
			Vector2 face;

			// 範囲を設定
			face = new Vector2(Random.Range(1,(int)x), Random.Range(1,(int)y));

			Vector2 vertex1 = new Vector2(20 * face.x, 20 * face.y);
			Vector2 vertex2 = new Vector2(20 * (face.x + 1), 20 * (face.y + 1));
			position.x = Mathf.Lerp(vertex1.x, vertex2.x, 0.5f) - mapSize.x / 2;
			position.z = Mathf.Lerp(vertex1.y, vertex2.y, 0.5f) - mapSize.y/ 2;
			// 向き設定
			Quaternion rotation = Quaternion.identity;
			int angle = Random.Range(0,361);
			rotation = Quaternion.AngleAxis(angle, Vector3.up);
			// オブジェクトの生成
			var obj = Instantiate(whirloopList[num], position, rotation);
			whirloopObjList.Add(obj);
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
			//NetworkServer.Destroy(whirloopObjList[i]);
		}
	}
}
