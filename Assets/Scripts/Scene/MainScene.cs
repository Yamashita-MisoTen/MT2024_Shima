using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MainScene : NetworkBehaviour
{

	// Start is called before the first frame update
	void Awake()
	{
		GameRuleManager mgr = GameObject.Find("Pf_GameRuleManager").GetComponent<GameRuleManager>();
		mgr.ReadyGame();
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
