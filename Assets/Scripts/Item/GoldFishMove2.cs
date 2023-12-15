using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;

public class GoldFishMove2 : NetworkBehaviour
{
	Tweener goldenFishTweener;
   // [SerializeField] public Vector3 zahyou01;
	[SerializeField, Tooltip("�i�ތo�H")] List<Vector3> route;
	float eventTime = 0;
	float addSpeed = 0;
	bool isPlayerCatch = false;	// �v���C���[�ɕߊl���ꂽ���ǂ���
	float requireTime = 0f;	// �o�ߎ���
	CPlayer playerComp = null;

	// Start is called before the first frame update
	void Start()
	{
		float lookAhead = 0.8f; // �����̍X�V�̕p�x
		// �o�H���w�肷��
		goldenFishTweener = transform.DOLocalPath(route.ToArray(),30, PathType.CatmullRom, PathMode.Full3D, gizmoColor: Color.red)
		.OnComplete(() => Debug.Log("�I���"))
		.SetLookAt(lookAhead, Vector3.forward)
		.SetLoops(-1, LoopType.Restart)
		.SetEase(Ease.Linear);

		// ��莞�Ԍ�ɏ�����悤�ɂ���

		// DOVirtual.DelayedCall(time2, () => GoldenFish2.Kill());
	}

	void Update(){
		requireTime += Time.deltaTime;
		// �C�x���g�̎��Ԃ��I�������ꍇ���ꂼ����ʂ��ς��
		if(requireTime >= eventTime){
			if(isPlayerCatch){
				CatchByPlayer();
			}else{
				NotCatchByPlayer();
			}
		}
	}

	public void SetUpEventData(float time, float speed){
		eventTime = time;
		addSpeed = speed;
		// NetworkServer.Spawn(this.gameObject);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag != "Player") return;
		isPlayerCatch = true;
		playerComp = other.gameObject.transform.GetComponent<CPlayer>();
		// �v���C���[�Ƃ��������Ƃ��̂ݏ�������
		playerComp.GetGoldenFish(addSpeed, this.gameObject);

		// �������~�߂�
		goldenFishTweener.Kill();
	}
	private void CatchByPlayer(){
		// �v���C���[�Ɉ��ݍ��ޏ�����������
		playerComp.FinishGoldenFishEvent(addSpeed);

		NetworkServer.Destroy(this.gameObject);
	}

	private void NotCatchByPlayer(){
		// ���̂܂܍폜
		NetworkServer.Destroy(this.gameObject);
	}
}
