using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EventUI : NetworkBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		// 必要タイミングで出せばいいので消しておく
		for(int i = 0; i < this.gameObject.transform.childCount; i++){
			this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public void SetCameraData(Camera camera){
		this.GetComponent<Canvas>().worldCamera = camera;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void StartUpEventUI(){
		for(int i = 0; i < this.gameObject.transform.childCount; i++){
			this.gameObject.transform.GetChild(i).gameObject.SetActive(true);
		}
	}
}
