using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Destroy : MonoBehaviour
{
    /// <summary>
    /// Õ“Ë‚µ‚½‚Æ‚«
    /// </summary>
    /// <param name="collision collision"></param>param>
    void OnCollisionEnter(Collision collision)
    {
        //Õ“Ë‚µ‚½‘Šè‚ÉPlayerƒ^ƒO‚ª•t‚¢‚Ä‚¢‚é‚Æ‚«
        if(collision.gameObject.tag == "Player")
        {
            //0.2•bŒã‚ÉÁ‚¦‚é
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
