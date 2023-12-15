using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class Ui_Move : MonoBehaviour
{
	enum number
	{
		HOST,
		GUEST,
	}
	enum number2
	{
		INPUT,
		CONNECT,
	}
	private CustomNetworkManager CNetworkManager;
	[SerializeField] private GameObject Guest;
	[SerializeField] private GameObject Host;
	[SerializeField] private GameObject Address;
	[SerializeField] private GameObject Cursor;
	[SerializeField] private GameObject Connect;
	[SerializeField] TextMeshProUGUI conectText;
	private TitleAnimation CTitleAnimation;
	int progressNum = 0;
	bool isInputNow = false;
	public bool isConnectServer = false;
	bool isConnectingNow = false;
	float requireTime = 0f;
	int updateNum = 0;
	int conNum = 0;

	[SerializeField] List<GameObject> dammyObj;
	[SerializeField] List<GameObject> titlePlayerDammy;
	[SerializeField] GameObject soundPf;
	[SerializeField] GameObject BGMPf;
	Title titleObj;

	private number Number;
	private number2 Number2;
	// Start is called before the first frame update
	void Start()
	{
		CNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
		CTitleAnimation = GameObject.Find("TitleLogo").GetComponent<TitleAnimation>();

		var soundObj = GameObject.Find("Pf_SoundManager 1");
		if(soundObj == null){
			Instantiate(soundPf);
		}
		var BsoundObj = GameObject.Find("Pf_BGMSoundManager 1");
		if(BsoundObj == null){
			Instantiate(BGMPf);
		}
		if(NetworkClient.active){
			AllUIunActive();
		}

		for(int i = 0 ; i < titlePlayerDammy.Count; i++){
			titlePlayerDammy[i].GetComponent<MeshRenderer>().material.SetColor("_BaseColor",CNetworkManager.GetPlayerColor(i));
		}

		for(int i = 0 ; i < dammyObj.Count; i++){
			dammyObj[i].SetActive(false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		switch (progressNum)
		{
			case 0:
				Progression0();
				break;
			case 1:
				Progression1();
				break;
		}
	}

	private void Progression0()
	{
		if(!NetworkClient.active){
			conNum = 0;
			ActiveDammy(0);
			if (Number == number.HOST)
			{
				Cursor.transform.localPosition = new Vector3(-850, -300, 0);
			}
			else if (Number == number.GUEST)
			{

				Cursor.transform.localPosition = new Vector3(150, -300, 0);
			}
		}else{
			if(CNetworkManager.connectPlayerCount != conNum){
				if(titleObj == null)titleObj = GameObject.Find("TitleScene").GetComponent<Title>();
				titleObj.ConnectUpdate(CNetworkManager.connectPlayerCount);
				ActiveDammy(CNetworkManager.connectPlayerCount);
			}
		}
	}

	private void Progression1()
	{
		if(!NetworkClient.active){
			conNum = 0;
			ActiveDammy(0);
			if(isConnectingNow){
				isConnectingNow = false;
				Address.SetActive(true);
				Connect.SetActive(true);
				Cursor.SetActive(true);
				conectText.gameObject.SetActive(false);
			}
			switch (Number2)
			{
				case number2.INPUT:
					Cursor.transform.localPosition = new Vector3(-400, -200, 0);
					break;
				case number2.CONNECT:
					Cursor.transform.localPosition = new Vector3(-400, -440, 0);
					break;
			}
		}else{
			if(!NetworkClient.isConnected){
				ConnectingText();
			}else{
				if(titleObj == null)titleObj = GameObject.Find("TitleScene").GetComponent<Title>();
				if(titleObj.connectNum != conNum){
					ActiveDammy(titleObj.connectNum);
				}
			}
		}
	}

	private void OnCancel()
	{
		SoundManager.instance.PlayAudio(SoundManager.AudioID.cancel);
		SoundManager.instance.LoopSettings(false);
		switch (progressNum)
		{
			case 0:
				CancelProgression0();
				break;
			case 1:
				CancelProgression1();
				break;
		}
	}

	void CancelProgression0(){
		if (Number == number.HOST)
		{
			CNetworkManager.OnStopHost();
		}
	}
	void CancelProgression1(){
		if(!NetworkClient.active){
			progressNum = 0;
			Host.SetActive(true);
			Guest.SetActive(true);
			Address.SetActive(false);
			Connect.SetActive(false);
		}else{
			CNetworkManager.StopClient();
		}
	}

	private void OnDecision()
	{
		SoundManager.instance.PlayAudio(SoundManager.AudioID.decide);
		SoundManager.instance.LoopSettings(false);
		switch (progressNum)
		{
			case 0:
				DecisionProgression0();
				break;
			case 1:
				DecisionProgression1();
				break;
		}
	}

	private void DecisionProgression0(){
		if(!NetworkClient.active){
			if (Number == number.HOST)
			{
				Debug.Log("a");
				//ホスト
				CNetworkManager.StartHost();
				CTitleAnimation.ButtonPush();
				Guest.SetActive(false);
				Host.SetActive(false);
				Cursor.SetActive(false);
			}
			else if (Number == number.GUEST)
			{
				//ゲスト
				CTitleAnimation.ButtonPush();
				Host.SetActive(false);
				Guest.SetActive(false);
				Address.SetActive(true);
				Connect.SetActive(true);
				Number2 = number2.INPUT;
				progressNum = 1;
			}
		}else{
			if(titleObj == null){
				titleObj = GameObject.Find("TitleScene").GetComponent<Title>();
			}
			titleObj.StartGame();
		}
	}
	private void DecisionProgression1(){
		switch (Number2)
		{
			case number2.INPUT:
				if(isInputNow){
					Debug.Log("i");
					//Address.GetComponent<InputField>().Com();
				}else{
					Debug.Log("a");
					Address.GetComponent<TMP_InputField>().Select();
				}
				isInputNow = !isInputNow;
				break;
			case number2.CONNECT:
				CNetworkManager.StartClient();
				isConnectingNow = true;
				Address.SetActive(false);
				Connect.SetActive(false);
				Cursor.SetActive(false);
				conectText.gameObject.SetActive(true);
				break;
		}
	}

	private void OnSelection(InputValue values)
	{
		var axis = values.Get<Vector2>();
		SoundManager.instance.PlayAudio(SoundManager.AudioID.cursorMove);
		SoundManager.instance.LoopSettings(false);
		switch (progressNum)
		{
			case 0:
				SelectProgression0(axis.x);
				break;
			case 1:
				SelectProgression1(axis.y);
				break;
		}
	}

	private void SelectProgression0(float x){
		if (x < 0.0f)
		{
			Number = number.HOST;
		}
		else if (x > 0.0f)
		{
			Number = number.GUEST;
		}
	}
	private void SelectProgression1(float y){
		if (y < 0.0f)
		{
			Number2 = number2.CONNECT;
		}
		else if (y > 0.0f)
		{
			Number2 = number2.INPUT;
		}
	}

	private void ConnectingText(){
		requireTime += Time.deltaTime;
		if(requireTime >= 1){
			requireTime = 0;
			updateNum++;
			if(updateNum - 1 == 3){
				conectText.text = "セツゾクチュウ";
				updateNum = 0;
			}else{
				conectText.text = conectText.text + ".";
			}
		}
	}

	private void AllUIunActive(){
		Host.SetActive(false);
		Guest.SetActive(false);
		Address.SetActive(false);
		Connect.SetActive(false);
		Cursor.SetActive(false);
	}

	public void ActiveDammy(int number){
		if(number > 3) return;
		for(int i = 0 ;i < dammyObj.Count;i++){
			dammyObj[i].SetActive(false);
		}
		for(int i = 0 ;i < number;i++){
			dammyObj[i].SetActive(true);
		}

		conNum = number;
	}
}
