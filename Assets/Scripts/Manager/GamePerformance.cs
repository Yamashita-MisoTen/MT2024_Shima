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
		isStartCountdown = true;
		SoundManager.instance.PlayAudio(SoundManager.AudioID.countdown);
	}

	void UpdateReadyPerformance(){
		if(isStartCountdown){
			progressCountdownTime += Time.deltaTime;
			var time = countdownTime - progressCountdownTime;
			if(Mathf.Ceil(time) > 0){
				countdownText.text = (Mathf.Ceil(time)).ToString();
			}else{
				isCompleteCountdown = true;
			}
		}
	}
	void StartResultPerforamce(){
		WinnerData = new GameObject[_playerData.Count - 1];
		isResultAnnounce = false;
		// �S�Ă�GameObject���擾����(��A�N�e�B�u��GameObject���܂�)
		var obj = FindObjectsOfType<GameObject>(true); // ������true��n���āA��A�N�e�B�u��GameObject���܂܂���
		// �擾�����S�Ă�GameObject�̒�����"Test"�Ƃ������O��GameObject��T��
		foreach (var go in obj){
			if(go.name == "ResultStage"){
				resultStageObj = go;
				for(int i = 0; i < resultStageObj.transform.childCount; i++){
					var child = resultStageObj.transform.GetChild(i).gameObject;
					if(child.name == "ResultCamera"){
						fadeResult.SetCamera(child.GetComponent<Camera>());
						resultStageObj.SetActive(true);
						break;
					}
				}
				break;
			}
		}
		// �t�F�[�h�C�����I�������ȉ��̊֐����Ă�
		SetReaultPos();
	}

	void UpdateResultPerformance(){
		isResultAnnounce = true;
	}

	void SetReaultPos(){
		for(int i = 0; i < _playerData.Count; i++){
			_playerData[i].transform.position = resultPos[i];
			_playerData[i].ResultData();
			// �����ŗ����A�j���[�V�����ɑւ���

			if(_playerData[i] != _orgaPlayer){
				Debug.Log("���҂̖��O" + _playerData[i].name);
				WinnerData[winnernum] = _playerData[i].gameObject;
				winnernum++;
			}
		}

		// �Ō�Ƀt�F�[�h�A�E�g�̖��ߓ����
		DOVirtual.DelayedCall(1f, () => fadeResult.StartFadeIn());
		DOVirtual.DelayedCall(1f + fadeResult.fadeInTime, () => ActiveText());
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
				Debug.Log("�e�L�X�g������");
				if(_playerData.Count - 1 > num - 1){
					text.GetComponent<TextMeshProUGUI>().text = WinnerData[num - 1].ToString();
				}
				num += 1;
			}
		}
	}
}
