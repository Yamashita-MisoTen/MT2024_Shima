using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRandomPosition : MonoBehaviour
{
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
        
    }

    // Update is called once per frame
    void Update()
    {
        //前フレームからの時間を加算していく
        time = time+ Time.deltaTime; ;

        //約一秒おきにランダムに生成されるようにする
        if(time>5.0f)
        {
            //rangeAとrangeBのx座標の範囲内でランダムな数値を作成
            float x = Random.Range(rangeA.position.x, rangeB.position.x);
            //rangeAとrangeBのy座標の範囲内でランダムな数値を作成
            float y = Random.Range(rangeA.position.y, rangeB.position.y);
            //rangeAとrangeBのｚ座標の範囲内でランダムな数値を作成
            float z = Random.Range(rangeA.position.z, rangeB.position.z);

            //GameObjectを上記で決まったランダムな場所に生成
            Instantiate(createPrefab, new Vector3(x, y, z), createPrefab.transform.rotation);

            //経過時間をリセット
            time = 0f;
        }
    }
}
