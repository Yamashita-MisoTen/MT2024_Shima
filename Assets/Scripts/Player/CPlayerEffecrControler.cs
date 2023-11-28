using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.VFX;
using DG.Tweening;

public partial class CPlayer : NetworkBehaviour
{
	[Header("エフェクトのデータ")]
	[SerializeField][Header("鬼のオーラエフェクト")]VisualEffect orgaFX = null;
	[SerializeField][Header("水しぶきエフェクト")]VisualEffect swimSplashFX = null;
	[SerializeField][Header("波紋エフェクト")]VisualEffect swimRippleFX = null;
	[Space]
	bool isSwim = true;
	float requireSplashFXTime = 0;
	float requireRippleFXTime = 0;
	[Header("調整項目")]
	[SerializeField][Header("エフェクトの生成タイミング")]float HappenSplashFXTime = 0.1f;
	[SerializeField][Header("エフェクトの生成タイミング")]float HappenRippleFXTime = 0.1f;
	[SerializeField][Header("生成の位置の補正値")] float posCorrection = 1.2f;
	void ParticleStart(){

	}
	void ParticleUpdate(){
		// エフェクト各種の発生時間を計測
		requireSplashFXTime += Time.deltaTime;
		requireRippleFXTime += Time.deltaTime;
		// 発生するかどうかのフラグ
		bool happenSplashFX = false;
		bool happenRippleFX = false;

		if(HappenSplashFXTime < requireSplashFXTime){
			happenSplashFX = true;
			requireSplashFXTime = 0;
		}

		if(HappenRippleFXTime < requireRippleFXTime){
			happenRippleFX = true;
			requireRippleFXTime = 0;
		}

		// エフェクト生成
		if(isSwim){
			// 水しぶき
			if(happenSplashFX){
				var pos = this.transform.position;
				var obj = Instantiate(swimSplashFX.gameObject, pos, Quaternion.identity);
				obj.gameObject.transform.parent = this.gameObject.transform;
				var comp = obj.GetComponent<VisualEffect>();
				// 計測時間後にオブジェクトを削除予定
				DOVirtual.DelayedCall(1.1f, () =>DeleteEffect(obj));
			}

			// 波紋
			if(happenRippleFX){
				var pos = this.transform.position + (this.transform.forward * posCorrection);
				var obj = Instantiate(swimRippleFX.gameObject, pos, Quaternion.identity);
				var comp = obj.GetComponent<VisualEffect>();
				comp.SetFloat("Random",Random.Range(0,100));
				// 計測時間後にオブジェクトを削除予定
				DOVirtual.DelayedCall(1.1f, () =>DeleteEffect(obj));
			}
		}
	}

	void ParticleStartUpSwitch(VisualEffect fx, bool isactive){
		fx.gameObject.SetActive(isactive);
	}

	void DeleteEffect(GameObject obj){
		Destroy(obj.gameObject);
	}
}
