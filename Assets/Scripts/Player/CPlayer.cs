using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Mirror;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CPlayer : NetworkBehaviour
{
	[SerializeField] bool _isNowOrga = false;
	public bool testMove = true;
	// アイテム所持するように
	// CItem _HaveItemData;

	/// </summary>
	/// // Start is called before the first frame update
	void Start()
	{
		GameRuleManager.instance.AddPlayerData(this);
	}

	// Update is called once per frame
	void Update()
	{
		// 移動系適当に作ったから後から変更
		if(testMove){
			if (Input.GetKey(KeyCode.D)) MoveX(0.1f);
			if (Input.GetKey(KeyCode.A)) MoveX(-0.1f);
			if (Input.GetKey(KeyCode.W)) MoveZ(0.1f);
			if (Input.GetKey(KeyCode.S)) MoveZ(-0.1f);
		}
	}

	void MoveX(float speed){
		Vector3 pos = this.transform.position;
		pos.x += speed;
		this.gameObject.transform.position = pos;
	}
	void MoveZ(float speed){
		Vector3 pos = this.transform.position;
		pos.z += speed;
		this.gameObject.transform.position = pos;
	}
	private void OnCollisionEnter(Collision other) {
		Debug.Log("あたり");
		if(!other.gameObject.CompareTag("Player")) return;

		// 自分が鬼のときのみ通知をする
		if(_isNowOrga && GameRuleManager.instance.CheckOverCoolTime()){
			Debug.Log("当たり判定発生");
			GameRuleManager.instance.ChangeOrgaPlayer(other.gameObject.GetComponent<CPlayer>());
			_isNowOrga = false;
		}
	}

	public void ChangeToOrga(){
		_isNowOrga = true;
	}

}
