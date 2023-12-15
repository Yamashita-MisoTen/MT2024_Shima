using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;

public class GoldFishMove2 : NetworkBehaviour
{
	Tweener goldenFishTweener;
   // [SerializeField] public Vector3 zahyou01;
	[SerializeField, Tooltip("進む経路")] List<Vector3> route;
	float eventTime = 0;
	float addSpeed = 0;
	bool isPlayerCatch = false;	// プレイヤーに捕獲されたかどうか
	float requireTime = 0f;	// 経過時間
	CPlayer playerComp = null;

	// Start is called before the first frame update
	void Start()
	{
		float lookAhead = 0.8f; // 方向の更新の頻度
		// 経路を指定する
		goldenFishTweener = transform.DOLocalPath(route.ToArray(),30, PathType.CatmullRom, PathMode.Full3D, gizmoColor: Color.red)
		.OnComplete(() => Debug.Log("終わり"))
		.SetLookAt(lookAhead, Vector3.forward)
		.SetLoops(-1, LoopType.Restart)
		.SetEase(Ease.Linear);

		// 一定時間後に消えるようにする

		// DOVirtual.DelayedCall(time2, () => GoldenFish2.Kill());
	}

	void Update(){
		requireTime += Time.deltaTime;
		// イベントの時間が終了した場合それぞれ効果が変わる
		if(requireTime >= eventTime){
			if(isPlayerCatch){
				CatchByPlayer();
			}else{
				NotCatchByPlayer();
			}
		}
	}

	public void SetUpEventData(float time, float speed){
		eventTime = time;
		addSpeed = speed;
		// NetworkServer.Spawn(this.gameObject);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag != "Player") return;
		isPlayerCatch = true;
		playerComp = other.gameObject.transform.GetComponent<CPlayer>();
		// プレイヤーとあたったときのみ処理する
		playerComp.GetGoldenFish(addSpeed, this.gameObject);

		// 動きを止める
		goldenFishTweener.Kill();
	}
	private void CatchByPlayer(){
		// プレイヤーに飲み込む処理をさせる
		playerComp.FinishGoldenFishEvent(addSpeed);

		NetworkServer.Destroy(this.gameObject);
	}

	private void NotCatchByPlayer(){
		// そのまま削除
		NetworkServer.Destroy(this.gameObject);
	}
}
