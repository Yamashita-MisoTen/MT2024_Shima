using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomPosition : MonoBehaviour
{
    //Objectを持っておくList
    [SerializeField]
    List<GameObject> list_item_;

    [SerializeField]
    int CreateNum = 3;

    [SerializeField]
    float CreateTime =0.0f;

    [SerializeField]
    [Tooltip("生成するGameObject")]
    private GameObject createPrefab;
    [SerializeField]
    [Tooltip("生成する範囲A")]
    private Transform rangeA;
    [SerializeField]
    [Tooltip("生成する範囲B")]
    private Transform rangeB;

    
    

    //経過時間
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        //インスタンス作成
        GameObject item_instance = Instantiate(createPrefab) as GameObject;

    }



    // Update is called once per frame
    void Update()
    {
        //前フレームからの時間を加算していく
        CreateTime = CreateTime+ Time.deltaTime; ;

        //ランダムに生成されるようにする
        if(CreateTime>5.0f)
        {
            for (int i = 0; i < CreateNum; i++)
            {
                //rangeAとrangeBのx座標の範囲内でランダムな数値を作成
                float x = Random.Range(rangeA.position.x, rangeB.position.x);
                //rangeAとrangeBのy座標の範囲内でランダムな数値を作成
                float y = Random.Range(rangeA.position.y, rangeB.position.y);
                //rangeAとrangeBのｚ座標の範囲内でランダムな数値を作成
                float z = Random.Range(rangeA.position.z, rangeB.position.z);
                int Num =Random.Range(0, list_item_.Count);
                //GameObjectを上記で決まったランダムな場所に生成
                Instantiate(list_item_[Num], new Vector3(x, y, z), createPrefab.transform.rotation);

            }
            //経過時間をリセット
            CreateTime = 0f;
        }
    }
}
