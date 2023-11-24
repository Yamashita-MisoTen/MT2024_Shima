using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U_Whirloop : Item
{
	[SerializeField] private GameObject prefab;
	public override void UseEffect(Transform trans)
	{
		Debug.Log("あああ");
	}
}
