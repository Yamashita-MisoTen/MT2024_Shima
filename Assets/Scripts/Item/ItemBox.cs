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
	GameObject item;
	CPlayer playerComp;
	void Start(){
		int num = Random.Range(0, list_item.Count);
		if(isServer){
			item = Instantiate(list_item[num],transform.position,transform.rotation);
			NetworkServer.Spawn(item);
			giveItem = item.GetComponent<Item>();
		}
	}

	private void OnTriggerEnter(Collider other){
		//衝突した相手にPlayerタグが付いているとき
		if(other.gameObject.tag == "Player")
		{
			Debug.Log("ここで触ってる");
			var pComp = other.transform.GetComponent<CPlayer>();
			if(!pComp.isLocalPlayer) return;
			if(pComp.isHaveItem()){
				playerComp = pComp;
				playerComp.SetItemData(giveItem);
				NetworkServer.Destroy(this.gameObject);
			}
		}
	}

	[ClientRpc]
	public void RpcSetItemData(Item itemData){
		giveItem = itemData;
	}
}
