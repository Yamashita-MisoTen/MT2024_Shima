using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using JetBrains.Annotations;

public class ItemBox : NetworkBehaviour
{
	//Objectを持っておくList
	[SerializeField] List<GameObject> list_item;


	/// <summary>
	/// </summary>
	/// <param name="collision collision"></param>param>
	void OnCollisionEnter(Collision collision)
	{
		//衝突した相手にPlayerタグが付いているとき
		if(collision.gameObject.tag == "Player")
		{
			collision.transform.GetComponent<CPlayer>().SetItem(RandomSetItem());
			Debug.Log("アイテム取得");
			Destroy(this.gameObject);
		}
	}

	Item RandomSetItem(){
		int num = Random.Range(0, list_item.Count - 1);
		return list_item[num].GetComponent<Item>();
	}
}
