using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Mirror;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public partial class CPlayer : NetworkBehaviour
{
	[SerializeField] bool _isNowOrga = false;
	[Header("渦潮のプレハブ")]
	[SerializeField] GameObject _WhirloopPrefab;
	public bool isCanMove = false;
	[SerializeField] float _rotAngle;
	[SerializeField] PlayerUI ui;
	[Header("鬼のオーラエフェクト")]
	[SerializeField]VisualEffect orgaFX = null;

	GameRuleManager mgr;
	// アイテム所持するように
	// CItem _HaveItemData;

	/// </summary>
	/// // Start is called before the first frame update
	void Start()
	{
		CPlayerMoveStart();
		mgr = GameObject.Find("Pf_GameRuleManager").GetComponent<GameRuleManager>();
	}

	// Update is called once per frame
	void Update()
	{
		// 今のシーンを確認してから入力機構切りたい
		if(isCanMove){
			CplayerMoveUpdate();	// 移動系の更新
		}
	}

	public void DataSetUPforMainScene(){
		// メインシーンでのセットアップで使用する
		this.GetComponent<PlayerUI>().MainSceneUICanvas();
		this.GetComponent<PlayerCamera>().MainSceneCamera();

		// 入力系をつける
		if(isLocalPlayer){
			var inputComp = this.gameObject.GetComponent<PlayerInput>();
			inputComp.enabled = true;
		}
	}

	private void OnCollisionEnter(Collision other) {
		// Debug.Log("あたり");
		// if(!other.gameObject.CompareTag("Player")) return;

		// // 自分が鬼のときのみ通知をする
		// Debug.Log("いまは" + mgr.CheckOverCoolTime());
		// if(_isNowOrga && mgr.CheckOverCoolTime()){
		// 	Debug.Log("当たり判定発生");
		// 	CmdChangeOrga(other.gameObject);
		// }
	}

	private void OnTriggerEnter(Collider other){
		Debug.Log("あたり");
		if(!other.gameObject.CompareTag("Player")) return;

		// 自分が鬼のときのみ通知をする
		Debug.Log("いまは" + mgr.CheckOverCoolTime());
		if(_isNowOrga && mgr.CheckOverCoolTime()){
			Debug.Log("当たり判定発生");
			CmdChangeOrga(other.gameObject);
		}
	}

	[Command]
	void CmdChangeOrga(GameObject otherObj){
		mgr.CmdChangeOrgaPlayer(otherObj.GetComponent<CPlayer>());
		_isNowOrga = false;
		ui.ChangeOrgaPlayer(_isNowOrga);
	}

	public void ChangeToOrga(){
		Debug.Log("鬼になったで");
		_isNowOrga = true;
		ui.ChangeOrgaPlayer(_isNowOrga);
	}

	private void CreateWhirloop(float size){
		Debug.Log("うずしおせいせい");
		// 渦潮を生成する座標を自分の現在の座標を格納する
		Vector3 whirloopPosition = this.transform.position;
		// 回転角をクオータニオンに変換
		Quaternion qtAngle = Quaternion.AngleAxis(_rotAngle, this.transform.up);
		// オブジェクトを生成する
		var obj = Instantiate(_WhirloopPrefab, whirloopPosition, Quaternion.identity);
		// 大きさの更新
		obj.transform.localScale = new Vector3(size,size,size);
		// 向きの更新
		obj.transform.rotation = qtAngle * obj.transform.rotation;
	}

}
