using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using JetBrains.Annotations;

public class ItemBox : NetworkBehaviour
{
	//Objectを持っておくList
	[SerializeField] List<GameObject> list_item;
	Item giveItem = null;
	void Start(){
		giveItem = RandomSetItem();
	}
	void OnTriggerEnter(Collider collision)
	{
		//衝突した相手にPlayerタグが付いているとき
		if(collision.gameObject.tag == "Player")
		{
			var pComp = collision.transform.GetComponent<CPlayer>();
			if(pComp.isHaveItem())pComp.SetItem(giveItem);
			NetworkServer.Destroy(this.gameObject);
		}
	}

	Item RandomSetItem(){
		int num = Random.Range(0, list_item.Count);
		Debug.Log(num);
		Debug.Log(list_item.Count - 1);
		Debug.Log("アイテム取得" + list_item[num].name);
		return list_item[num].GetComponent<Item>();
	}
}
