using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Destroy : MonoBehaviour
{
    /// <summary>
    /// �Փ˂����Ƃ�
    /// </summary>
    /// <param name="collision collision"></param>param>
    void OnCollisionEnter(Collision collision)
    {
        //�Փ˂��������Player�^�O���t���Ă���Ƃ�
        if(collision.gameObject.tag == "Item")
        {
            //������
            Destroy(collision.gameObject);
            Debug.Log("�A�C�e���擾");
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
