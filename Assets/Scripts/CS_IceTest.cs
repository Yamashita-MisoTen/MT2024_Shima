using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_IceTest : MonoBehaviour
{
    
    float minSize = 2;
    float maxSize = 5;

    int rndEdge;
    public int count = 20;
    public int edgeCnt;
    public GameObject Ice;
    GameObject[] Edges;
    // Start is called before the first frame update
    void Start()
    {
        rndEdge = Random.Range(0, edgeCnt - 1);
        Edges = GameObject.FindGameObjectsWithTag("MapEdge");
        for (int i = 0; i < count; i++)
        {
            float rndScale = Random.Range(minSize, maxSize);
            Vector3 EdgePos = Edges[rndEdge].transform.position;
            GameObject Ins_Ice = (GameObject)Instantiate(Ice, new Vector3(EdgePos.x, EdgePos.y, EdgePos.z), Quaternion.identity);
            Ins_Ice.transform.localScale = new Vector3(rndScale, rndScale, rndScale);
        }
    }
}
