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
		if (Input.GetKey(KeyCode.W)) MoveX(0.1f);
		if (Input.GetKey(KeyCode.S)) MoveX(-0.1f);
	}

	[Command]
	void MoveX(float speed){
		Vector3 pos = this.transform.position;
		pos.x += speed;
		this.gameObject.transform.position = pos;
	}

	private void OnTriggerEnter(Collider other) {
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
