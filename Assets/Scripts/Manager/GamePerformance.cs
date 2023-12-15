using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public partial class GameRuleManager
{
	GameObject resultStageObj = null;
	GameObject[] WinnerData;
	int winnernum = 0;
	bool isCompleteCountdown = false;
	[SerializeField] private TextMeshProUGUI countdownText;
	[SerializeField] int countdownTime;
	GameObject readyCanvasObj = null;
	float progressCountdownTime = 0;
	bool isStartCountdown = false;
	private RectTransform countdownTrans;
	Tweener countdownTwenner;

	// チュートリアル関連
	Image[] tutolialImage;
	RectTransform[] tutolialImageTrans;
	public int tutorialNum {get; private set;}= 0;
	bool isTutorialAnim = false;

	[ClientRpc]
	void RpcStartReadyPerformance()
	{
		isStartCountdown = true;
		SoundManager.instance.PlayAudio(SoundManager.AudioID.countdown);
		SoundManager.instance.LoopSettings(false);
	}

	void StartPerformance(){
		for(int i = 0; i < readyCanvasObj.transform.childCount; i++){
			var childObj = readyCanvasObj.transform.GetChild(i).gameObject;
			if(childObj.name == "TutorialPage1"){
				if(tutolialImage == null) tutolialImage = new Image[2];
				if(tutolialImageTrans == null) tutolialImageTrans = new RectTransform[2];
				tutolialImage[0] = childObj.GetComponent<Image>();
				tutolialImageTrans[0] = childObj.GetComponent<RectTransform>();
			}
			if(childObj.name == "TutorialPage2"){
				if(tutolialImage == null) tutolialImage = new Image[2];
				if(tutolialImageTrans == null) tutolialImageTrans = new RectTransform[2];
				tutolialImage[1] = childObj.GetComponent<Image>();
				tutolialImageTrans[1] = childObj.GetComponent<RectTransform>();
			}
			if(childObj.name == "CountdownText"){
				countdownTrans = childObj.GetComponent<RectTransform>();
			}
		}
	}

	void UpdateReadyPerformance()
	{
		if (isStartCountdown)
		{
			progressCountdownTime += Time.deltaTime;
			var time = countdownTime - progressCountdownTime;
			if (Mathf.Ceil(time) > 0)
			{
				countdownText.text = (Mathf.Ceil(time)).ToString();
			}
			else
			{
				isCompleteCountdown = true;
			}
		}
	}
	void StartResultPerforamce()
	{
		WinnerData = new GameObject[_playerData.Count - 1];
		isResultAnnounce = false;
		// 全てのGameObjectを取得する(非アクティブのGameObjectを含む)
		var obj = FindObjectsOfType<GameObject>(true); // 引数にtrueを渡して、非アクティブのGameObjectも含ませる
													   // 取得した全てのGameObjectの中から"Test"という名前のGameObjectを探す
		foreach (var go in obj)
		{
			if (go.name == "ResultStage")
			{
				resultStageObj = go;
				for (int i = 0; i < resultStageObj.transform.childCount; i++)
				{
					var child = resultStageObj.transform.GetChild(i).gameObject;
					if (child.name == "ResultCamera")
					{
						child.GetComponent<AudioListener>().enabled = true;
						fadeResult.SetCamera(child.GetComponent<Camera>());
						resultStageObj.SetActive(true);
						break;
					}
				}
				break;
			}
		}
		// フェードインが終わったら以下の関数を呼ぶ
		SetReaultPos();
	}

	void StartCountDown(){
		//DOVirtual.Float();
		float size = 10;
		if(countdownTwenner != null){
			countdownTwenner.Kill();
			Debug.Log("あり");
		}
		Debug.Log("更新" + countdownText.text);
		// 文字のスケール大きくする
		countdownTrans.localScale = new Vector3(size,size,size);
		countdownTwenner = countdownTrans.DOScale(5,1f);
	}

	void UpdateResultPerformance()
	{
		if (AnimationFinish)
			isResultAnnounce = true;

		UpdateResultManager();
	}

	void SetReaultPos()
	{
		for (int i = 0; i < _playerData.Count; i++)
		{
			// _playerData[i].transform.position = resultPos[i];
			_playerData[i].ResultData();
			// ここで立ちアニメーションに替える

			if (_playerData[i] != _orgaPlayer)
			{
				Debug.Log("勝者の名前" + _playerData[i].name);
				WinnerData[winnernum] = _playerData[i].gameObject;
				winnernum++;
			}
		}


		//リザルト用にアニメーションを割り振り
		for (int i = 1; i <= _playerData.Count - 1; i++)
		{
			ResultList.Add(i);
		}

		for (int i = 0; i < _playerData.Count; i++)
		{
			GameObject obj = Instantiate(Resultobj, Vector3.zero, Quaternion.identity);
			obj.transform.eulerAngles = new Vector3(0, 180, 0);
			_ResultObject.Add(obj);
			var script = _ResultObject[i].GetComponent<Result_Animation>();

			if (_playerData[i] == _orgaPlayer)
			{
				script.SetAnimation(0);
				LosePayer = i + 1;
			}
			else
			{
				int index = Random.Range(0, ResultList.Count);

				Debug.Log(index);
				int ransu = ResultList[index];
				Debug.Log(ransu);

				ResultList.RemoveAt(index);

				script.SetAnimation(ransu);
			}
			_ResultObject[i].transform.position = resultPos[i];
		}

		GameObject.Find("Pf_Ocean").gameObject.SetActive(false);

		// 最後にフェードアウトの命令入れる
		DOVirtual.DelayedCall(1f, () => fadeResult.StartFadeIn());
		DOVirtual.DelayedCall(1f + fadeResult.fadeInTime, () => ActiveText());
	}

	void ActiveText()
	{
		GameObject canvas = null;
		for (int i = 0; i < resultStageObj.transform.childCount; i++)
		{
			var childObj = resultStageObj.transform.GetChild(i);
			if (childObj.name == "Canvas")
			{
				canvas = childObj.gameObject;
				break;
			}
		}
		string textname = "Winner_";
		int num = 1;
		for (int i = 0; i < canvas.transform.childCount; i++)
		{
			var text = canvas.transform.GetChild(i);
			if (text.gameObject.name == textname + num.ToString())
			{
				Debug.Log("テキストあった");
				if (_playerData.Count - 1 > num - 1)
				{
					text.GetComponent<TextMeshProUGUI>().text = WinnerData[num - 1].ToString();
				}
				num += 1;
			}
		}


		//リザルト用
		ResultUpdateOn = true;
	}

		public void NextTutolialPage(){
		if(isTutorialAnim) return;
		if(!isCompleteFadeIn) return;
		if(tutorialNum + 1 == tutolialImage.Count()) return;
		tutorialNum++;
		isTutorialAnim = true;

		for(int i = 0; i < tutolialImage.Count(); i++){
			var pos = tutolialImageTrans[i].anchoredPosition.x - 1920;
			Debug.Log(pos);
			tutolialImageTrans[i].DOAnchorPosX(pos, 1.0f)
			.OnComplete(() => DOVirtual.DelayedCall(0.3f, () => isTutorialAnim = false));
		}
	}

	public void BackTutolialPage(){
		if(isTutorialAnim) return;
		if(tutorialNum == 0) return;
		tutorialNum--;
		isTutorialAnim = true;
		for(int i = 0; i < tutolialImage.Count(); i++){
			var pos = tutolialImageTrans[i].anchoredPosition.x + 1920;
			Debug.Log(pos);
			tutolialImageTrans[i].DOAnchorPosX(pos, 1.0f)
			.OnComplete(() => DOVirtual.DelayedCall(0.3f, () => isTutorialAnim = false));
		}
	}

	public void CloseTutorialImage(){
		for(int i = 0; i < tutolialImage.Count(); i++){
			var pos = tutolialImageTrans[i].anchoredPosition.y + 1080;
			tutolialImageTrans[i].DOAnchorPosY(pos, 1.0f).
			OnComplete(() => tutolialImage[i].gameObject.SetActive(false));
		}
	}
	public void CompleteFadeIn(){
		for(int i = 0; i < tutolialImage.Count(); i++){
			var pos = tutolialImageTrans[i].anchoredPosition.y - 1080;
			tutolialImageTrans[i].DOAnchorPosY(pos, 1.0f).
			OnComplete(() => isCompleteFadeIn = true);
		}
	}
}
