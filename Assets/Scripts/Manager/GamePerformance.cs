using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine;

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

	void StartReadyPerformance(){
		// 3s後にカウントダウン開始
		DOVirtual.DelayedCall (1f, ()=> isStartCountdown = true, false);
	}

	void UpdateReadyPerformance(){
		if(isStartCountdown){
			progressCountdownTime += Time.deltaTime;
			var time = countdownTime - progressCountdownTime;
			if(Mathf.Ceil(time) > 0){
				countdownText.text = (Mathf.Ceil(time)).ToString();
			}else{
				countdownText.text = "Start!!";
				isCompleteCountdown = true;
				// 1s後に非表示に
				DOVirtual.DelayedCall (1f, ()=> readyCanvasObj.SetActive(false), false);
			}
		}
	}
	void StartResultPerforamce(){
		WinnerData = new GameObject[_playerData.Count - 1];
		isResultAnnounce = false;
		// 全てのGameObjectを取得する(非アクティブのGameObjectを含む)
		var obj = FindObjectsOfType<GameObject>(true); // 引数にtrueを渡して、非アクティブのGameObjectも含ませる
		// 取得した全てのGameObjectの中から"Test"という名前のGameObjectを探す
		foreach (var go in obj){
			if(go.name == "ResultStage"){
				resultStageObj = go;
				resultStageObj.SetActive(true);
				break;
			}
		}
		// フェードインが終わったら以下の関数を呼ぶ
		SetReaultPos();
	}

	void UpdateResultPerformance(){
		isResultAnnounce = true;
	}

	void SetReaultPos(){
		Vector3 respos = new Vector3(0,-499.5f,0);
		for(int i = 0; i < _playerData.Count; i++){
			_playerData[i].transform.position = respos;
			_playerData[i].ResultData();
			// ここで立ちアニメーションに替える
			respos.x += 2;

			if(_playerData[i] != _orgaPlayer){
				Debug.Log("勝者の名前" + _playerData[i].name);
				WinnerData[winnernum] = _playerData[i].gameObject;
				winnernum++;
			}
		}

		// 最後にフェードアウトの命令入れる
		ActiveText();
	}

	void ActiveText(){
		GameObject canvas = null;
		for(int i = 0; i < resultStageObj.transform.childCount; i++){
			var childObj = resultStageObj.transform.GetChild(i);
			if(childObj.name == "Canvas"){
				canvas = childObj.gameObject;
				break;
			}
		}
		string textname = "Winner_";
		int num = 1;
		for(int i = 0; i < canvas.transform.childCount; i++){
			var text = canvas.transform.GetChild(i);
			if(text.gameObject.name == textname + num.ToString()){
				Debug.Log("テキストあった");
				if(_playerData.Count - 1 > num - 1){
					text.GetComponent<TextMeshProUGUI>().text = WinnerData[num - 1].ToString();
				}
				num += 1;
			}
		}
	}
}
