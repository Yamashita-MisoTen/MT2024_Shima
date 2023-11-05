using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.VFX;
using Unity.VisualScripting;
using DG.Tweening;

public class WhirloopBase : NetworkBehaviour
{
	[Header("---- 調整項目 -----")]
	[SerializeField] [Tooltip("1f前に加速する速度")]private float accelerationSpeed = 0.1f;
	[SerializeField] [Tooltip("抜けるまでに必要な時間(s)")] private float moveTime = 1.0f;
	[SerializeField] [Tooltip("触れてからの停止時間(s)")] private float waitTime = 0.1f;
	[SerializeField] [Tooltip("使用回数")] private int maxUseNum = 1;	// 使用回数
	[SerializeField] [Tooltip("通過地点")] private Vector3 endPoint;
	[SerializeField] [Tooltip("通過地点")] private List<Vector3> checkPoint;

	[Space]

	[Header("---- 必要コンポーネント類 -----")]
	[SerializeField]BoxCollider colliderObj = null;
	[SerializeField]VisualEffect whirloopFX = null;
	List<GameObject> otherObj = null;	// 複数対応用に配列にしておく
	List<GameObject> fxData = null;		// エフェクト用にデータを格納しておく
	List<Vector3> wayPoint;
	float whirloopLength = 1.0f;	// 渦潮の長さ
	float whirloopSize = 1.0f;		// 渦潮の大きさ(馬鹿でか渦潮作るならつかう)
	int remainUseNum = 1;
	bool _isOnObject = false;	// 乗ってるかどうか
	// Start is called before the first frame update
	void Start()
	{
		otherObj = new List<GameObject>();	// オブジェクトのデータを格納する
		fxData = new List<GameObject>();	// エフェクトのデータを格納する
		remainUseNum = maxUseNum;
	}

	// Update is called once per frame
	void Update()
	{
		if(!_isOnObject) return;

		// ここで待ち時間入れる
		DOTween.Sequence()
		.SetDelay(3);

		for(int i = 0; i < otherObj.Count; i++){
			ForcingToMove(otherObj[i]);
		}
		// transform.forwardで正面方向に生成できる
	}

	/// <summary>
	/// 渦潮を生成した時に呼ぶセットアップ関数
	/// </summary>
	/// <param name="length"> 渦潮の長さ </param>
	/// <param name="size">　渦潮の大きさ 基本は1.0f </param>
	public void SetUpWhrloop(float length, float size){
		// 必要情報を格納する
		whirloopLength = length;	// 長さ
		whirloopSize = size;		// 大きさ　

		// 大きさと長さに当たり判定を大きくする
		colliderObj.size = new Vector3(whirloopSize,whirloopSize,whirloopLength);
		colliderObj.center = new Vector3(0.0f,0.0f,whirloopLength / 2.0f);

		// エフェクト起動
		if(whirloopFX == null) return;
		Vector3 fxpos = new Vector3(0.0f,0.0f,0.0f);
		for(int i = 0; i < maxUseNum; i++){
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

			Debug.Log("エフェクト確認");
			// エフェクトのデータを配列に格納
			if(fxData == null) fxData = new List<GameObject>();
			Debug.Log("エフェクト確認2");
			fxData.Add(obj.gameObject);
			Debug.Log("エフェクト確認3" + fxData.Count);
		}
	}

	void ForcingToMove(GameObject obj){
		// 乗ってるオブジェクトを終点まで運んでいく
		var trans = obj.GetComponent<Transform>();
		// 向いてる方向に対して速度を増やす
		trans.position += this.transform.forward * accelerationSpeed;
	}

	// 当たったときにオブジェクトを指定する
	private void OnTriggerEnter(Collider other){
		if(remainUseNum ==  otherObj.Count) return;
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
				Debug.Log(fxData.Count);
				Debug.Log(fxData[i]);
				fxData.RemoveAt(i);
				otherObj.RemoveAt(i);
				// フラグを削除する
				if(otherObj.Count == 0) _isOnObject = false;
				continue;
			}
		}

		// 使用回数を減らす
		remainUseNum -= 1;

		// 使用回数がなくなった場合の処理
		if(remainUseNum == 0){
			NetworkServer.Destroy(this.gameObject);
		}
	}

}
