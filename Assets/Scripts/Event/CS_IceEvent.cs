using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceEvent : GameEvent
{
    int edgeCnt = 4;
    int rndEdge;

    public float minSize = 3;
    public float maxSize = 5;
    public int count = 20;
    public GameObject Ice;
    GameObject[] Edges;
    protected override string eventName() => "ïXâÚî≠ê∂";

    public override void StartEvent()
    {
        Init();
        CreatIce();
    }

    void Init()
    {
        rndEdge = Random.Range(0, edgeCnt - 1);
        Edges = GameObject.FindGameObjectsWithTag("MapEdge");
    }

    void CreatIce()
    {
        Vector3 EdgePos = Edges[rndEdge].transform.position;
        Vector3 EdgeSize = Edges[rndEdge].transform.localScale;
        for (int i = 0; i < count; i++)
        {
            float rndScale = Random.Range(minSize, maxSize);
            if (EdgeSize.x > EdgeSize.z) 
            {
                GameObject Ins_Ice = (GameObject)Instantiate(Ice, new Vector3(Random.Range(-EdgeSize.x / 2, EdgeSize.x / 2), EdgePos.y, EdgePos.z), Quaternion.identity);
                Ins_Ice.transform.localScale = new Vector3(rndScale, rndScale, rndScale);
            }
            if (EdgeSize.x < EdgeSize.z)
            {
                GameObject Ins_Ice = (GameObject)Instantiate(Ice, new Vector3(EdgePos.x, EdgePos.y, Random.Range(-EdgeSize.z / 2, EdgeSize.z / 2)), Quaternion.identity);
                Ins_Ice.transform.localScale = new Vector3(rndScale, rndScale, rndScale);
            }
        }
    }
}
