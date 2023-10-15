using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SoundManager : NetworkBehaviour
{
	private enum AudioID{	// ここにレベルになるIDを追加していく
		decide = 0,	// 決定音
		select,		// 選択音
	}

	[System.Serializable]
	private class AudioData{	// サウンドの実データ
		[SerializeField] public AudioClip audioClip;
		[SerializeField] public AudioID	audioID;
	}
	[SerializeField] private List<AudioData> audioDatas;
	[SerializeField] private AudioSource _source;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//if(Input.GetKeyDown(KeyCode.Space)) PlayAudio(AudioID.decide);
	}

	/// <summary>
	/// IDのみを指定して音を再生する関数
	/// </summary>
	/// <param name="id"> 再生する音源のID </param>
	void PlayAudio(AudioID id){
		_source.clip = audioDatas[(int)id].audioClip;
		_source.Play();
	}
	/// <summary>
	/// IDとSourceを指定して音を再生する関数
	/// </summary>
	/// <param name="id"> 再生する音源のID </param>
	/// <param name="source"> 再生するAudioSource </param>
	void PlayAudio(AudioID id, AudioSource source){
		source.clip = audioDatas[(int)id].audioClip;
		source.Play();
	}
	/// <summary>
	/// ボリュームの変更を行う
	/// </summary>
	/// <param name="volume"> 音の大きさ 0.0 ~ 1.0 </param>
	void ChangeVolume(float volume){
		_source.volume = volume;
	}
	/// <summary>
	/// AudioSourceごとのボリュームの変更を行う
	/// </summary>
	/// <param name="volume"> 音の大きさ 0.0 ~ 1.0 </param>
	/// <param name="source"> 指定するAudioSoruce </param>
	void ChangeVolume(float volume, AudioSource source){
		source.volume = volume;
	}
}
