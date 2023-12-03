using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Item_Whirloop : Item
{
	[SerializeField] private GameObject prefab;
	public override void UseEffect(Vector3 pos, Quaternion qt)
	{
		var tmp = new Vector3(0,0,3);
		var qtA = qt * tmp;
		Debug.Log(qtA);
		var Obj = Instantiate(prefab, pos + qtA, qt);
		NetworkServer.Spawn(Obj);
		Obj.GetComponent<WhirloopBase>().EventSetUpWhirloop();
	}
}
