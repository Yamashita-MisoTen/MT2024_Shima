using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Diagnostics;
using UnityEngine.VFX;
using Unity.VisualScripting;

public class WhirloopBase : NetworkBehaviour
{
	[SerializeField] [Header("加速する速度(1fごとに加算していく距離)")]private float _accelerationSpeed = 1.5f;
	List<GameObject> otherObj = null;	// 複数対応用に配列にしておく
	[SerializeField]VisualEffect whirloopFX = null;
	bool _isOnObject = false;	// 乗ってるかどうか
	// Start is called before the first frame update
	void Start()
	{
		otherObj = new List<GameObject>();
		if(whirloopFX == null) return;
		whirloopFX.SendEvent("OnStart");
	}

	// Update is called once per frame
	void Update()
	{
		if(!_isOnObject) return;
		ForcingToMove();
		// transform.forwardで正面方向に生成できる
	}

	void ForcingToMove(){
		// 乗ってるオブジェクトを強制移動していく
		for(int i = 0; i < otherObj.Count; i++){
			var trans = otherObj[i].GetComponent<Transform>();
			// 向いてる方向に対して速度を増やす
			trans.position += this.transform.forward * _accelerationSpeed;
		}
	}

	// 当たったときにオブジェクトを指定する
	private void OnTriggerEnter(Collider other){
		otherObj.Add(other.gameObject);
		_isOnObject = true;	//　触れたオブジェクトがある場合にフラグをtrueに
	}

	private void OnTriggerExit(Collider other){
		// 出た時にオブジェクトを削除する
		for(int i = 0; i < otherObj.Count; i++){
			if(otherObj[i] == other.gameObject){
				otherObj.RemoveAt(i);
				// フラグを削除する
				if(otherObj.Count == 0) _isOnObject = false;
				continue;
			}
		}
	}

}
