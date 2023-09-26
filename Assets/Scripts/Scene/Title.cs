using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class Title : NetworkBehaviour{
	// Start is called before the first frame update
	[SerializeField]NetworkManager manager;
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))ChangeSceneMainGame();
	}

	void ChangeSceneMainGame(){
		manager.ServerChangeScene("MainGame");
	}

	void OnServerAddPlayer(){
		
	}
}
