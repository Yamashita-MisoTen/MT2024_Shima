using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WhirloopBase : NetworkBehaviour
{
	[SerializeField] [Header("加速する速度(1fごとに加算していく距離)")]private float _accelerationSpeed = 0.5f;
	List<GameObject> otherObj = null;	// 複数対応用に配列にしておく
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		Debug.Log(transform.forward);
	}

	void ForcingToMove(){
		// 乗ってるオブジェクトを強制移動していく

	}

	// 当たったときにオブジェクトを指定する
	private void OnTriggerEnter(Collider other){
		otherObj.Add(other.gameObject);
	}

	private void OnTriggerExit(Collider other){
		// 出た時にオブジェクトを削除する
		for(int i = 0; i < otherObj.Count; i++){
			if(otherObj[i] == other.gameObject){
				otherObj.RemoveAt(i);
				continue;
			}
		}
	}

}
