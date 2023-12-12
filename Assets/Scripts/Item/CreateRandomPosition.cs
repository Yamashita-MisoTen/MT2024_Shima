using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CreateRandomPosition : NetworkBehaviour
{
	[SerializeField] GameObject itemBox;
	[SerializeField]
	int CreateNum = 3;

	[SerializeField]
	float CreateTime =0.0f;

	[SerializeField]
	[Tooltip("生成する範囲A")]
	private Vector3 rangeA;
	[SerializeField]
	[Tooltip("生成する範囲B")]
	private Vector3 rangeB;

	//経過時間
	public bool isCreateItemBox = true;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if(!isServer) return;
		if(!isCreateItemBox) return;
		//前フレームからの時間を加算していく
		CreateTime = CreateTime + Time.deltaTime;

		//ランダムに生成されるようにする
		if(CreateTime > 5.0f)
		{
			CreateItemBox();
			//経過時間をリセット
			CreateTime = 0f;
		}
	}

	void CreateItemBox(){
		for (int i = 0; i < CreateNum; i++)
		{
			//rangeAとrangeBのx座標の範囲内でランダムな数値を作成
			float x = Random.Range(rangeA.x, rangeB.x);
			//rangeAとrangeBのy座標の範囲内でランダムな数値を作成
			float y = Random.Range(rangeA.y, rangeB.y);
			//rangeAとrangeBのｚ座標の範囲内でランダムな数値を作成
			float z = Random.Range(rangeA.z, rangeB.z);
			//GameObjectを上記で決まったランダムな場所に生成
			var obj = Instantiate(itemBox, new Vector3(x, y, z), itemBox.transform.rotation);
			var comp = obj.GetComponent<ItemBox>();
			NetworkServer.Spawn(obj);

			DOVirtual.DelayedCall(0.01f, () => comp.RpcSetItemData(comp.giveItem));
		}
	}
}
