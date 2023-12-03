using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class SoundManager : NetworkBehaviour
{
	public static SoundManager instance;
	public enum AudioID{	// ここにレベルになるIDを追加していく
		decide = 0,	// 決定音
		select,		// 選択音
		playerMove,
		whistle,
		countdown,
	}

	[System.Serializable]
	private class AudioData{	// サウンドの実データ
		[SerializeField] public AudioClip audioClip;
		[SerializeField] public AudioID	audioID;
		[SerializeField] public float volume;
	}
	[SerializeField] private List<AudioData> audioDatas;
	[SerializeField] private AudioSource _source;
	void Awake(){
		if(instance == null){
			instance = this;
			DontDestroyOnLoad(gameObject);
		}else{
			Destroy(gameObject);
		}
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	/// <summary>
	/// IDのみを指定して音を再生する関数
	/// </summary>
	/// <param name="id"> 再生する音源のID </param>
	public void PlayAudio(AudioID id){
		_source.clip = audioDatas[(int)id].audioClip;
		_source.volume = audioDatas[(int)id].volume;
		_source.Play();
	}
	/// <summary>
	/// IDとSourceを指定して音を再生する関数
	/// </summary>
	/// <param name="id"> 再生する音源のID </param>
	/// <param name="source"> 再生するAudioSource </param>
	public void PlayAudio(AudioID id, AudioSource source){
		source.clip = audioDatas[(int)id].audioClip;
		source.volume = audioDatas[(int)id].volume;
		source.Play();
	}
		/// <summary>
	/// IDとSourceを指定して音を再生する関数
	/// </summary>
	/// <param name="id"> 再生する音源のID </param>
	/// <param name="source"> 再生するAudioSource </param>
	/// <param name="volume"> 再生する音源のVolume </param>
	public void PlayAudio(AudioID id, AudioSource source, float volume){
		source.clip = audioDatas[(int)id].audioClip;
		source.volume = volume;
		source.Play();
	}
	/// <summary>
	/// ボリュームの変更を行う
	/// </summary>
	/// <param name="volume"> 音の大きさ 0.0 ~ 1.0 </param>
	public void ChangeVolume(float volume){
		_source.volume = volume;
	}
	/// <summary>
	/// AudioSourceごとのボリュームの変更を行う
	/// </summary>
	/// <param name="volume"> 音の大きさ 0.0 ~ 1.0 </param>
	/// <param name="source"> 指定するAudioSoruce </param>
	public void ChangeVolume(float volume, AudioSource source){
		source.volume = volume;
	}
}
