using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CPlayer : NetworkBehaviour
{
	bool _isNowOrga = false;
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

	// 鬼の場合のみ起動して相手を鬼に変更する
	void ChangeOrgaPlayer(){
		//if()
		_isNowOrga = false;
	}
}
