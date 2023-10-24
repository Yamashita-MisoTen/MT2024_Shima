using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.VFX;
using Unity.VisualScripting;

public class WhirloopBase : NetworkBehaviour
{
	[SerializeField] [Header("加速する速度(1fごとに加算していく距離)")]private float _accelerationSpeed = 0.1f;
	List<GameObject> otherObj = null;	// 複数対応用に配列にしておく
	[SerializeField]BoxCollider colliderObj = null;
	[SerializeField]VisualEffect whirloopFX = null;
	float whirloopLength = 1.0f;	// 渦潮の長さ
	float whirloopSize = 1.0f;		// 渦潮の大きさ(馬鹿でか渦潮作るならつかう)
	bool _isOnObject = false;	// 乗ってるかどうか
	// Start is called before the first frame update
	void Start()
	{
		otherObj = new List<GameObject>();
	}

	// Update is called once per frame
	void Update()
	{
		if(!_isOnObject) return;
		ForcingToMove();
		// transform.forwardで正面方向に生成できる
	}

	/// <summary>
	/// 渦潮を生成した時に呼ぶセットアップ関数
	/// </summary>
	/// <param name="length"> 渦潮の長さ </param>
	/// <param name="size">　渦潮の大きさ 基本は1.0f </param>
	public void SetUpWhrloop(float length, float size){
		colliderObj.size = new Vector3(size,size,length);
		colliderObj.center = new Vector3(0.0f,0.0f,length / 2.0f);

		// エフェクト起動
		if(whirloopFX == null) return;
		Vector3 fxpos = new Vector3(0.0f,0.0f,0.0f);
		for(int i = 0; i < (int)length + 1; i++){
			// エフェクト生成
			var obj = Instantiate(whirloopFX);
			// 生成したオブジェクトを自分の子供に変更する
			obj.gameObject.transform.parent = this.gameObject.transform;
			// 座標変更
			Debug.Log(fxpos);
			obj.transform.localPosition = fxpos;
			// 起動
			obj.SendEvent("OnStart");
			// 初期設定
			obj.SetFloat("RingRadius",1.0f);
			obj.SetFloat("RingSpeed",50.0f);
			// 次のエフェクト用に座標を＋する
			fxpos.z += 1.0f;
		}
	}

	void ForcingToMove(){
		// 乗ってるオブジェクトを終点まで運んでいく
		for(int i = 0; i < otherObj.Count; i++){
			var trans = otherObj[i].GetComponent<Transform>();
			// 向いてる方向に対して速度を増やす
			trans.position += this.transform.forward * _accelerationSpeed;
		}
	}

	// 当たったときにオブジェクトを指定する
	private void OnTriggerEnter(Collider other){
		otherObj.Add(other.gameObject);
		// プレイヤーの時は変更する
		if(other.gameObject.CompareTag("Player")){
			other.gameObject.GetComponent<CPlayer>().InWhirloopSetUp();
		}
		_isOnObject = true;	//　触れたオブジェクトがある場合にフラグをtrueに
	}

	private void OnTriggerExit(Collider other){
		// 出た時にオブジェクトを削除する
		for(int i = 0; i < otherObj.Count; i++){
			if(otherObj[i] == other.gameObject){
				if(other.gameObject.CompareTag("Player")){
					other.gameObject.GetComponent<CPlayer>().OutWhirloop();
				}
				otherObj.RemoveAt(i);
				// フラグを削除する
				if(otherObj.Count == 0) _isOnObject = false;
				continue;
			}
		}
	}

}
