using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class BGMSoundManager : NetworkBehaviour
{
	public static BGMSoundManager instance;
	public enum AudioID{	// ここにレベルになるIDを追加していく
		Title = 0,
		GameBGM,
		GameBGMHighTemp,
		Result,
	}

	[System.Serializable]
	private class AudioData{	// サウンドの実データ
		[SerializeField] public AudioClip audioClip;
		[SerializeField] public AudioID	audioID;
	}
	[SerializeField] private List<AudioData> audioDatas;
	[SerializeField] private AudioSource[] _source;
	float requireTime = 0f;
	float targetTime = 0f;
	bool isBlendNow = false;
	int mainSorceNum = 0;
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
		if(!isBlendNow) return;

		int subsorcenum = 0;
		if(mainSorceNum == 0) subsorcenum = 1;

		requireTime += Time.deltaTime;
		Debug.Log(requireTime);
		float ratio = requireTime / targetTime;
		Debug.Log(ratio);

		if(ratio >= 1.0f){	// 目標時刻を過ぎた場合の処理
			ratio = 1.0f;		// 割合を1.0に
		}
		// 割合からそれぞれのボリュームの大きさを設定していく
		_source[mainSorceNum].volume = (1f - ratio) / 100;
		_source[subsorcenum].volume = ratio / 100;

		if(ratio >= 1.0f){
			requireTime = 0f;	// それぞれ時間を初期化
			targetTime = 0f;
			isBlendNow = false;	// ブレンドのフラグを切る
			mainSorceNum = subsorcenum;
		}
	}

	/// <summary>
	/// IDのみを指定して音を再生する関数
	/// </summary>
	/// <param name="id"> 再生する音源のID </param>
	public void PlayAudio(AudioID id){
		_source[mainSorceNum].clip = audioDatas[(int)id].audioClip;
		_source[mainSorceNum].Play();
	}
	/// <summary>
	/// IDとSourceを指定して音を再生する関数
	/// </summary>
	/// <param name="id"> 再生する音源のID </param>
	/// <param name="source"> 再生するAudioSource </param>
	public void PlayAudio(AudioID id, AudioSource source){
		source.clip = audioDatas[(int)id].audioClip;
		source.Play();
	}

	public void StopAudio(){
		_source[mainSorceNum].Stop();
	}
	/// <summary>
	/// ボリュームの変更を行う
	/// </summary>
	/// <param name="volume"> 音の大きさ 0.0 ~ 1.0 </param>
	public void ChangeVolume(float volume){
		_source[mainSorceNum].volume = volume;
	}
	/// <summary>
	/// AudioSourceごとのボリュームの変更を行う
	/// </summary>
	/// <param name="volume"> 音の大きさ 0.0 ~ 1.0 </param>
	/// <param name="source"> 指定するAudioSoruce </param>
	public void ChangeVolume(float volume, AudioSource source){
		source.volume = volume;
	}
	/// <summary>
	/// ブレンドを行いながら音を切り替える
	/// </summary>
	/// <param name="id"> 再生する音源のID </param>
	/// <param name="blendTime"> ブレンドするのに必要な時間 </param>
	public void SetNextBGM(AudioID id ,float blendTime){
		int num = 0;
		if(mainSorceNum == 0) num = 1;
		_source[num].clip = audioDatas[(int)id].audioClip;
		_source[num].volume = 0f;
		targetTime = blendTime;
		isBlendNow = true;
	}
}
