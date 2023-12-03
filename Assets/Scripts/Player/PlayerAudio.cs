using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAudio : NetworkBehaviour
{
	// Start is called before the first frame update
	AudioSource audioSource ;
	AudioListener audioListener;
	void Start()
	{
		for(int i = 0; i < this.gameObject.transform.childCount; i++){
			var child = this.gameObject.transform.GetChild(i).gameObject;
			if(child.gameObject.name == "PlayerAudio"){
				audioSource = child.GetComponent<AudioSource>();
				audioSource.loop = true;
				audioListener = child.GetComponent<AudioListener>();
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void SetUpAudio(){
		audioListener.enabled = false;
		if(isLocalPlayer){
			audioListener.enabled = true;
		}
	}

	public AudioSource GetAudioSource(){
		return audioSource;
	}
}
