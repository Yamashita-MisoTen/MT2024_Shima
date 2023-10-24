using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Destroy : MonoBehaviour
{
    /// <summary>
    /// 衝突したとき
    /// </summary>
    /// <param name="collision collision"></param>param>
    void OnCollisionEnter(Collision collision)
    {
        //衝突した相手にPlayerタグが付いているとき
        if(collision.gameObject.tag == "Item")
        {
            //消える
            Destroy(collision.gameObject);
            Debug.Log("アイテム取得");
        }
    }

    public virtual void UseEffect()
    {

    }

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
