using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.VFX;
using Unity.VisualScripting;
using DG.Tweening;
using UnityEngine.UIElements.Experimental;
using Unity.Burst.CompilerServices;

public class WhirloopBase : NetworkBehaviour
{
	enum WhirloopMove{
		Straight,
		Curve,			// deg　90度方向に進むとき
		InverseCurve	// deg-90度方向に進むとき
	}
	[Header("---- 調整項目 -----")]
	[SerializeField] [Tooltip("触れてからの停止時間(s)")] private float waitTime = 0.1f;
	[SerializeField] [Tooltip("使用回数")] private int maxUseNum = 1;	// 使用回数
	[SerializeField] [Tooltip("終了地点")] private Vector3 endPoint;
	[SerializeField] [Tooltip("通過地点")] private List<Vector3> checkPoint;
	[SerializeField] [Tooltip("出口方向軸")] private Vector3 exitForward;

	[Space]

	[Header("---- 必要コンポーネント類 -----")]
	[SerializeField] BoxCollider colliderObj = null;
	[SerializeField] VisualEffect whirloopFX = null;
	[SerializeField] VisualEffect whirloopShell = null;
	VisualEffect[] whirloopShellComp = null;


	List<GameObject> otherObj = null;	// 複数対応用に配列にしておく
	[SerializeField]List<GameObject> fxData = null;		// エフェクト用にデータを格納しておく
	[SerializeField]List<GameObject> fxShellData = null;		// エフェクト用にデータを格納しておく
	List<Vector3> wayPoint;
	float whirloopLength = 1.0f;	// 渦潮の長さ
	float whirloopSize = 1.0f;		// 渦潮の大きさ(馬鹿でか渦潮作るならつかう)
	float lowestSpeed = 5.0f;		// スピードの最低値
	int remainUseNum = 1;
	bool _isOnObject = false;	// 乗ってるかどうか
	bool isCompleteSetUp = false;
	bool isStraight = false;
	List<bool> isWaitFinish;
	// Start is called before the first frame update
	void Start()
	{
		remainUseNum = maxUseNum;
	}

	// Update is called once per frame
	void Update()
	{
		if(!_isOnObject) return;
		// transform.forwardで正面方向に生成できる
	}

	/// <summary>
	/// 渦潮を生成した時に呼ぶセットアップ関数
	/// </summary>
	/// <param name="length"> 渦潮の長さ </param>
	/// <param name="size">　渦潮の大きさ 基本は1.0f </param>
	public void SetUpWhrloop(float length, float size, Quaternion qt){
		if(isWaitFinish == null) isWaitFinish = new List<bool>();
		isWaitFinish.Add(false);

		// 必要情報を格納する
		whirloopLength = length;	// 長さ
		whirloopSize = size;		// 大きさ
		isStraight = true;			// 直線かどうかプレイヤーが作る場合は絶対に直線

		// 大きさと長さに当たり判定を大きくする
		colliderObj.size = new Vector3(whirloopSize,whirloopSize,whirloopLength);
		colliderObj.center = new Vector3(0.0f,0.0f,whirloopLength / 2.0f);
		endPoint = whirloopLength * this.transform.forward;
		Debug.Log("endPoint : "+endPoint);

		if(wayPoint == null) {
			// 通過地点をそれぞれ設定する
			wayPoint = new List<Vector3>();
			// 始まりの地点
			wayPoint.Add(new Vector3(0,0,0));
			// 脱出地点を少し奥側へ
			wayPoint.Add(endPoint);

			var qtA = qt * this.transform.rotation;
			// 回転に合わせて修正をかける
			for(int i = 0; i < wayPoint.Count; i++){
				wayPoint[i] = qtA * wayPoint[i];
				wayPoint[i] += this.transform.position;
			}
		}

		// エフェクト起動
		if(whirloopFX == null) return;
		Vector3 fxpos = new Vector3(0.0f,0.0f,0.0f);
		for(int i = 0; i < maxUseNum; i++){
			// エフェクト生成
			var obj = Instantiate(whirloopFX);
			// 生成したオブジェクトを自分の子供に変更する
			obj.gameObject.transform.parent = this.gameObject.transform;
			// 座標変更
			obj.transform.localPosition = fxpos;
			// 起動
			obj.SendEvent("OnStart");
			// 初期設定
			obj.SetFloat("RingRadius",1.0f);
			obj.SetFloat("RingSpeed",50.0f);
			// 次のエフェクト用に座標を＋する
			fxpos.z += 1.0f;

			// エフェクトのデータを配列に格納
			if(fxData == null) fxData = new List<GameObject>();
			fxData.Add(obj.gameObject);
		}

		// 外殻エフェクト生成
		whirloopShellComp = new VisualEffect[4];
		if(fxShellData == null) fxShellData = new List<GameObject>();
		for(int i = 0; i < 4; i++){
			var shell = Instantiate(whirloopShell,this.transform.position,Quaternion.identity);
			whirloopShellComp[i] = shell.GetComponent<VisualEffect>();
			whirloopShellComp[i].SetFloat("ForwardSpeed",4);
			whirloopShellComp[i].SetFloat("R",2);
			whirloopShellComp[i].SetFloat("EndPosZ",whirloopLength);
			Quaternion qtAngle = Quaternion.AngleAxis(90 * i, new Vector3(0,0,1));
			whirloopShellComp[i].gameObject.transform.rotation *= qtAngle;
			// 生成したオブジェクトを自分の子供に変更する
			shell.gameObject.transform.parent = this.gameObject.transform;

			fxShellData.Add(shell.gameObject);
		}

		// 向きの更新
		this.transform.rotation = qt * this.transform.rotation;

		// セットアップ完了
		isCompleteSetUp = true;
	}

	public void EventSetUpWhirloop(){
		if(isWaitFinish == null) isWaitFinish = new List<bool>();
		isWaitFinish.Add(false);

		Quaternion qtX = Quaternion.AngleAxis(this.transform.localEulerAngles.x, new Vector3(1,0,0));
		Quaternion qtY = Quaternion.AngleAxis(this.transform.localEulerAngles.y, new Vector3(0,1,0));
		Quaternion qtZ = Quaternion.AngleAxis(this.transform.localEulerAngles.z, new Vector3(0,0,1));
		Quaternion qt = qtX * qtY * qtZ;
		if(wayPoint == null) {
			// 通過地点をそれぞれ設定する
			wayPoint = new List<Vector3>();
			// 始まりの地点
			wayPoint.Add(new Vector3(0,0,0));
			for(int i = 0; i < checkPoint.Count; i++){
				wayPoint.Add(checkPoint[i]);
			}
			// 脱出地点を少し奥側へ
			wayPoint.Add(endPoint);
			// 回転に合わせて修正をかける
			for(int i = 0; i < wayPoint.Count; i++){
				Debug.Log("回転設定 :" + i);
				Debug.Log("前 :" + wayPoint[i]);
				wayPoint[i] = qt * wayPoint[i];
				wayPoint[i] += this.transform.position;
				Debug.Log("後 :" + wayPoint[i]);
			}
		}

		// チェックポイントが無い場合は直線になる
		if(checkPoint.Count == 0) isStraight = true;

		// セットアップ完了
		isCompleteSetUp = true;
	}

	// 当たったときにオブジェクトを指定する
	private void OnTriggerEnter(Collider other){
		// セットアップが終わってるかどうかを確認する
		if(!isCompleteSetUp) return;

		// プレイヤーのみを指定する
		if(!other.gameObject.CompareTag("Player")) return;
		// オブジェクトのデータのnullCheck
		if(otherObj == null) otherObj = new List<GameObject>();
		// 使用回数確認
		if(remainUseNum == otherObj.Count) return;
		Debug.Log("使用回数オーバーしてない");
		// 同一オブジェクト確認
		if(otherObj.Contains(other.gameObject)) return;
		Debug.Log("同一オブジェクトが入ってない");

		// 問題ないのでデータを格納する
		otherObj.Add(other.gameObject);

		// プレイヤーの時は変更する
		var playerComp = other.gameObject.GetComponent<CPlayer>();
		var speed = playerComp.GetNowSpeed();	// 自分の速度 + 追加分の速度を加算
		Debug.Log(speed);
		playerComp.InWhirloopSetUp();

		// スピードに補正
		if(speed < 5f) speed = lowestSpeed;

		ForcingToMove(otherObj[otherObj.Count - 1], speed, CheckNextPos(other.gameObject.transform.position));
		// 遅延後に処理
		//DOVirtual.DelayedCall(waitTime, () => ForcingToMove(otherObj[otherObj.Count - 1], moveTime, CheckNextPos(other.gameObject.transform.position)));
		_isOnObject = true;	//　触れたオブジェクトがある場合にフラグをtrueに
	}

	List<Vector3> CheckNextPos(Vector3 objpos){
		List<Vector3> result = new List<Vector3>();
		int startNum = 0;
		float nearDis = 0;

		// 距離を求めて一番近い点を算出する
		for(int i = 0; i < wayPoint.Count; i++){
			float distance = (objpos - wayPoint[i]).sqrMagnitude;
			if(i == 0){
				nearDis = distance;
				startNum = i;
			}else{
				if(nearDis > distance){
					nearDis = distance;
					startNum = i;
				}
			}
		}
		if(checkPoint.Count == 0){	// 直線の場合
			startNum = 1;	// 出口に向かう以外の選択肢がない
		}else{
			// U字S字の場合
			if(startNum != wayPoint.Count - 1){	// 出口以外は一番近い点の次の点に向かって進む
				startNum += 1;
			}
		}

		// 最終的な座標の更新
		for(int i = startNum; i < wayPoint.Count; i++){
			Debug.Log("A");
			result.Add(wayPoint[i]);
			Debug.Log("B");
		}
		Debug.Log("C");
		return result;
	}

	void ForcingToMove(GameObject obj, float time, List<Vector3> waypoint){
		// 乗ってるオブジェクトを終点まで運んでいく
		var trans = obj.GetComponent<Transform>();

		float velo = 5.0f;

		// 直線かどうかで動きを変える
		PathType type = PathType.Linear;
		if(isStraight) type = PathType.CatmullRom;
		// 実際に動かす
		obj.transform.DOLocalPath(waypoint.ToArray(), time, type, PathMode.Full3D, gizmoColor:Color.red)
			.SetLookAt(0.001f, Vector3.forward)
			.SetEase(Ease.InCirc)
			.SetSpeedBased(true)
			.OnComplete(() => CompleteMoveWhirloop(obj, velo));
	}

	private void CompleteMoveWhirloop(GameObject obj, float velo){
		// 出た時にオブジェクトを削除する
		for(int i = 0; i < otherObj.Count; i++){
			if(otherObj[i] == obj){
				if(obj.CompareTag("Player")){
					obj.GetComponent<CPlayer>().OutWhirloop(velo);
				}

				// エフェクト関連変更
				// 回数リング
				Destroy(fxData[i]);

				//

				Debug.Log(fxData.Count);
				otherObj.RemoveAt(i);
				Debug.Log(obj.transform.forward);
				// フラグを削除する
				if(otherObj.Count == 0) _isOnObject = false;
				continue;
			}
		}

		// 使用回数を減らす
		remainUseNum -= 1;
		Debug.Log(remainUseNum);
		// エフェクト削除

		// 使用回数がなくなった場合の処理
		if(remainUseNum == 0){
			NetworkServer.Destroy(this.gameObject);
		}
	}

	[Command]
	private void CmdDeleteFX(int i){
		RpcDeleteFX(i);
	}
	[ClientRpc]
	private void RpcDeleteFX(int i){
		Destroy(fxData[i]);
		fxData.RemoveAt(i);
	}
}
