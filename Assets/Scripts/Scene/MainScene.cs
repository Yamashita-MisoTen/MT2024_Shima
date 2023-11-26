using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class MainScene : NetworkBehaviour
{
	[SerializeField] GameRuleManager mgr;
	// Start is called before the first frame update
	void Awake()
	{

	}

	void Start(){
		if(isServer){
			Debug.Log("生成");
			var obj = Instantiate(mgr);
			mgr.name = "Pf_GameRuleManager";
			NetworkServer.Spawn(obj.gameObject);
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
