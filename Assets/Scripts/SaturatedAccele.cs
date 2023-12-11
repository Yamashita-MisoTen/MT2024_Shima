using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SaturatedAccele : MonoBehaviour
{
	Material saturatedMt;
	Tweener tweener;
	[SerializeField, Tooltip("‰Á‘¬‰‰oˆê‰ñ‚ ‚½‚è‚ÌŠÔ")] float time = 0.01f;
	// Start is called before the first frame update
	void Start()
	{
		saturatedMt = this.GetComponent<Image>().material;
	}

	public void StartAccele(){
		tweener = DOVirtual.Float(0.5f,1f,time,
		onVirtualUpdate: (tweenValue) => {
				saturatedMt.SetFloat("_Ratio", tweenValue);
			})
		.SetLoops(-1, LoopType.Restart);
	}

	void SetImageActive(bool flg){
		this.gameObject.SetActive(flg);
		if(flg) StartAccele();
	}

	public void StopAccele(){
		tweener.Kill(true);
		this.gameObject.SetActive(false);
	}
}
