using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Result_Animation : MonoBehaviour
{
    //Animationcontroller
    private Animator Result_Animator;
    private bool AnimationSet = false;
    private int AnimationNumber = 0;
    // Update is called once per frame

    private void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject childObj = this.transform.GetChild(i).gameObject;
            Debug.Log(childObj.name);
            //アニメーションを代入
            if (childObj.name == "PenguinFBX")
            {
                Result_Animator = childObj.GetComponent<Animator>();
            }
        }


    }
    void Update()
    {
        if (!AnimationSet)
        {
            Result_Animator.SetInteger("Number", AnimationNumber);
            Result_Animator.SetFloat("MoveSpeed", 0.0f);
            AnimationSet = true;
        }
    }

    public void SetAnimation(int Number)
    {
        AnimationNumber = Number;
    }

    public void SetStart()
    {
        Result_Animator.SetFloat("MoveSpeed", 1.0f);
    }
    public bool GetGetCurrentAnimatorStateInfo()
    {
        return Result_Animator.GetCurrentAnimatorStateInfo(0).IsName("Finish");
    }
}
