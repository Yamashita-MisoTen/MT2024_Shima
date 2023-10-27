using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomPosition : MonoBehaviour
{
    //Object�������Ă���List
    [SerializeField]
    List<GameObject> list_item_;

    [SerializeField]
    int CreateNum = 3;

    [SerializeField]
    float CreateTime =0.0f;

    [SerializeField]
    [Tooltip("��������GameObject")]
    private GameObject createPrefab;
    [SerializeField]
    [Tooltip("��������͈�A")]
    private Transform rangeA;
    [SerializeField]
    [Tooltip("��������͈�B")]
    private Transform rangeB;

    
    

    //�o�ߎ���
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        //�C���X�^���X�쐬
        GameObject item_instance = Instantiate(createPrefab) as GameObject;

    }



    // Update is called once per frame
    void Update()
    {
        //�O�t���[������̎��Ԃ����Z���Ă���
        CreateTime = CreateTime+ Time.deltaTime; ;

        //�����_���ɐ��������悤�ɂ���
        if(CreateTime>5.0f)
        {
            for (int i = 0; i < CreateNum; i++)
            {
                //rangeA��rangeB��x���W�͈͓̔��Ń����_���Ȑ��l���쐬
                float x = Random.Range(rangeA.position.x, rangeB.position.x);
                //rangeA��rangeB��y���W�͈͓̔��Ń����_���Ȑ��l���쐬
                float y = Random.Range(rangeA.position.y, rangeB.position.y);
                //rangeA��rangeB�̂����W�͈͓̔��Ń����_���Ȑ��l���쐬
                float z = Random.Range(rangeA.position.z, rangeB.position.z);
                int Num =Random.Range(0, list_item_.Count);
                //GameObject����L�Ō��܂��������_���ȏꏊ�ɐ���
                Instantiate(list_item_[Num], new Vector3(x, y, z), createPrefab.transform.rotation);

            }
            //�o�ߎ��Ԃ����Z�b�g
            CreateTime = 0f;
        }
    }
}
