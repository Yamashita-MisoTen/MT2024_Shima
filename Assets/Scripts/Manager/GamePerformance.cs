using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine;
using Mirror;

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

	[ClientRpc]
	void RpcStartReadyPerformance(){
		isStartCountdown = true;
		SoundManager.instance.PlayAudio(SoundManager.AudioID.countdown);
		SoundManager.instance.LoopSettings(false);
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

	void UpdateResultPerformance()
	{
		isResultAnnounce = true;
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
			}
			else
			{
				int index = Random.Range(0, ResultList.Count);

				Debug.Log(index);
				int ransu = ResultList[index];
				Debug.Log(ransu);

				ResultList.RemoveAt(index);

				script.SetAnimation(index);
			}
			_ResultObject[i].transform.position = resultPos[i];
		}

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
	}
}
