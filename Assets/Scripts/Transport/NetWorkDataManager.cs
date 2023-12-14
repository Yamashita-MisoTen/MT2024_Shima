using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;
using Unity.VisualScripting;

public class NetWorkDataManager : NetworkBehaviour {
	public struct PlyaerDataInfo{
		public List<GameObject> ObjDatas;
		public List<CPlayer> CompDatas;
		public List<int> ConnId;
	}
	[SerializeField] PlyaerDataInfo playerInfo;
	[SerializeField] List<Vector3> lobyPlayerDataPos;
	// Start is called before the first frame update
	void Start()
	{
		DontDestroyOnLoad(this.gameObject);
		GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>().SetDataMgr(this);
	}
	public void PlayerDataInit(){
		Debug.Log("プレイヤーの位置初期化するで");
		if(playerInfo.ObjDatas == null) return;
		int Count = 0;
		foreach(CPlayer obj in playerInfo.CompDatas){
			obj.InitData();
			// ロビーでの座標を調整する
			Vector3 pos = new Vector3( -1.5f + Count, 0, 0);
			obj.transform.position = pos;
			Count++;
		}
	}

	// ** プレイヤーデータを格納する
	public void SetPlayerData(GameObject obj, CPlayer comp, int id){
		if(playerInfo.ObjDatas == null)playerInfo.ObjDatas = new List<GameObject>();
		playerInfo.ObjDatas.Add(obj);
		if(playerInfo.CompDatas == null)playerInfo.CompDatas = new List<CPlayer>();
		playerInfo.CompDatas.Add(comp);
		if(playerInfo.ConnId == null)playerInfo.ConnId = new List<int>();
		playerInfo.ConnId.Add(id);

		Debug.Log("プレイヤーを追加しました");
		Debug.Log("追加したプレイヤー" + obj.name);
		Debug.Log("現在のプレイヤー数" + playerInfo.ObjDatas.Count);
		Debug.Log("現在のプレイヤー数Comp" + playerInfo.CompDatas.Count);
		Debug.Log("現在のプレイヤー数id" + playerInfo.ConnId.Count);

		// ロビーでの座標を調整する
		if(lobyPlayerDataPos.Count == 0){
			int detail = playerInfo.ObjDatas.Count - 1;
			Vector3 pos = new Vector3( -1.5f + detail, 0, 0);
			obj.transform.position = pos;
		}else{
			obj.transform.position = lobyPlayerDataPos[playerInfo.ObjDatas.Count - 1];
		}

		// クライアントにデータを送る
		RpcSendPlayerData(playerInfo);
	}

	// ** データが必要ならデータを格納する
	public List<GameObject> GetPlayerData(List<GameObject> objects){
		if(isClient)CmdSetPlayerData();
		Debug.Log(playerInfo.ObjDatas);
		return playerInfo.ObjDatas;
	}
	public List<CPlayer> GetPlayerData(List<CPlayer> comps){
		if(isClient)CmdSetPlayerData();
		Debug.Log(playerInfo.CompDatas);
		return playerInfo.CompDatas;
	}
	public List<int> GetPlayerData(List<int> ids){
		if(isClient)CmdSetPlayerData();
		Debug.Log(playerInfo.ConnId);
		return playerInfo.ConnId;
	}

	// ** 削除命令が出ればデータを削除する
	public void DeleteObj(GameObject obj){
		int count = 0;
		foreach(GameObject p in playerInfo.ObjDatas){
			if(p == obj){
				Debug.Log("削除します : " + playerInfo.ObjDatas[count].name);
				Debug.Log("現在のプレイヤー数" + (playerInfo.ObjDatas.Count - 1));
				playerInfo.ObjDatas.RemoveAt(count);
				playerInfo.CompDatas.RemoveAt(count);
				playerInfo.ConnId.RemoveAt(count);
				return;
			}
			count += 1;
		}

		// 各クライアントにデータを送信
		if(NetworkClient.active) RpcSendPlayerData(playerInfo);
	}
	public void DeleteObj(CPlayer comp){
		int count = 0;
		foreach(CPlayer p in playerInfo.CompDatas){
			if(p == comp){
				Debug.Log("削除します : " + playerInfo.ObjDatas[count].name);
				Debug.Log("現在のプレイヤー数" + (playerInfo.ObjDatas.Count - 1));
				playerInfo.ObjDatas.RemoveAt(count);
				playerInfo.CompDatas.RemoveAt(count);
				playerInfo.ConnId.RemoveAt(count);
				return;
			}
			count += 1;
		}

		// 各クライアントにデータを送信
		if(NetworkClient.active) RpcSendPlayerData(playerInfo);
	}
	public void DeleteObj(int id){
		int count = 0;
		foreach(int p in playerInfo.ConnId){
			if(p == id){
				Debug.Log("削除します : " + playerInfo.ObjDatas[count].name);
				Debug.Log("現在のプレイヤー数" + (playerInfo.ObjDatas.Count - 1));
				playerInfo.ObjDatas.RemoveAt(count);
				playerInfo.CompDatas.RemoveAt(count);
				playerInfo.ConnId.RemoveAt(count);
				return;
			}
			count += 1;
		}

		// 各クライアントにデータを送信
		if(NetworkClient.active) RpcSendPlayerData(playerInfo);
	}

	public void DeleteAllObj(){
		playerInfo.ObjDatas.Clear();
		playerInfo.CompDatas.Clear();
		playerInfo.ConnId.Clear();
		// 各クライアントにデータを送信
		if(NetworkClient.active) RpcSendPlayerData(playerInfo);
	}

	// ** データが同一かどうかを確認する関数群
	public bool CheckIdentityPlayer(GameObject obj, CPlayer comp, int id){
		if(CheckIdentity(obj)) return false;
		if(CheckIdentity(comp)) return false;
		if(CheckIdentity(id)) return false;

		return true;
	}
	public bool CheckIdentity(GameObject obj){
		return playerInfo.ObjDatas.Contains(obj);
	}

	public bool CheckIdentity(CPlayer comp){
		return playerInfo.CompDatas.Contains(comp);
	}

	public bool CheckIdentity(int id){
		return playerInfo.ConnId.Contains(id);
	}


	[ClientRpc]
	void RpcSendPlayerData(PlyaerDataInfo players){
		playerInfo = players;
		Debug.Log("クライアントにデータを送信しました 人数 : " + playerInfo.ObjDatas.Count);

		foreach(GameObject p in playerInfo.ObjDatas){
			DontDestroyOnLoad(p);
		}
	}

	[Command]
	public void CmdSetPlayerData(){
		RpcSendPlayerData(playerInfo);
	}
}
