using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MainScene : NetworkBehaviour
{

	// Start is called before the first frame update
	void Awake()
	{
		GameRuleManager.instance.SendPlayerDataInfo();
		GameRuleManager.instance.ReadyGame();
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
