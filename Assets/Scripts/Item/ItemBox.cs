using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using JetBrains.Annotations;

public class ItemBox : NetworkBehaviour
{
	//Objectを持っておくList
	[SerializeField] List<GameObject> list_item;
	public Item giveItem;

	void Start(){
		int num = Random.Range(0, list_item.Count);
		giveItem = list_item[num].GetComponent<Item>();
	}
	void OnTriggerEnter(Collider collision)
	{
		if(!isServer) return;
		//衝突した相手にPlayerタグが付いているとき
		if(collision.gameObject.tag == "Player")
		{
			Debug.Log("ここで触ってる");
			var pComp = collision.transform.GetComponent<CPlayer>();
			if(pComp.isHaveItem())pComp.SetItem(giveItem);
			NetworkServer.Destroy(this.gameObject);
		}
	}

	[ClientRpc]
	public void RpcSetItemData(Item itemData){
		giveItem = itemData;
	}
}
