using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleAnimation : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    public void ButtonPush()
    {
        transform.DOLocalMoveY(300f, 1f);
    }
        
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
