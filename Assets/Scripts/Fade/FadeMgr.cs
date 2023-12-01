using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// ** フェードアウトしたらオブジェクト削除すするようにして
// タイトルシーンにおく→メインシーンでフェードインしたらリザルト用のフェードオブジェクト生成してこのオブジェクト削除
// っていう風にしたらタイトルシーンに戻ったとしても大丈夫な気がする

public class FadeMgr : NetworkBehaviour
{
	[SerializeField, Tooltip("フェードインの時の時間(秒)")] private float fadeInTime = 3f;
	[SerializeField, Tooltip("フェードアウトの時の時間(秒)")] private float fadeOutTime = 3f;
	void Start()
	{
		DontDestroyOnLoad(this);
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	void StartFadeIn(){

	}

	void StartFadeOut(){
		
	}
}
