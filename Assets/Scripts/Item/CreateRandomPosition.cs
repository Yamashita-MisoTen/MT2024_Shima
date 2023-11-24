using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class CreateRandomPosition : NetworkBehaviour
{
	[SerializeField] GameObject itemBox;
	[SerializeField]
	int CreateNum = 3;

	[SerializeField]
	float CreateTime =0.0f;

	[SerializeField]
	[Tooltip("��������͈�A")]
	private Vector3 rangeA;
	[SerializeField]
	[Tooltip("��������͈�B")]
	private Vector3 rangeB;

	//�o�ߎ���
	private float time;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//�O�t���[������̎��Ԃ����Z���Ă���
		CreateTime = CreateTime+ Time.deltaTime; ;

		//�����_���ɐ��������悤�ɂ���
		// if(CreateTime>5.0f)
		// {
		// 	CreateItemBox();
		// 	//�o�ߎ��Ԃ����Z�b�g
		// 	CreateTime = 0f;
		// }

		if(Input.GetKeyDown(KeyCode.U)){
			CreateItemBox();
		}
	}

	void CreateItemBox(){
		for (int i = 0; i < CreateNum; i++)
		{
			//rangeA��rangeB��x���W�͈͓̔��Ń����_���Ȑ��l���쐬
			float x = Random.Range(rangeA.x, rangeB.x);
			//rangeA��rangeB��y���W�͈͓̔��Ń����_���Ȑ��l���쐬
			float y = Random.Range(rangeA.y, rangeB.y);
			//rangeA��rangeB�̂����W�͈͓̔��Ń����_���Ȑ��l���쐬
			float z = Random.Range(rangeA.z, rangeB.z);
			//GameObject����L�Ō��܂��������_���ȏꏊ�ɐ���
			var obj = Instantiate(itemBox, new Vector3(x, y, z), itemBox.transform.rotation);
			NetworkServer.Spawn(obj);
		}
	}
}
