using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result_Spawn : MonoBehaviour
{
    [SerializeField, Header("リザルト用オブジェクト")] GameObject ResultObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetResultObject()
    {
        for(int a = 0;a < 4;a++)
        {
            GameObject obj = Instantiate(ResultObject, Vector3.zero, Quaternion.identity);
            
        }
        //Instantiate(,, Quaternion.identity);
    }
}
