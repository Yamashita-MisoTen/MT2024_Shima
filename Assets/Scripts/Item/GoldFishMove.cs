using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoldFishMove : MonoBehaviour
{
    Tweener GoldenFish;

    // Start is called before the first frame update
    void Start()
    {
        float lookAhead = 0.8f;
        GoldenFish = transform.DOLocalPath(new Vector3[] { new Vector3(-60f, 0f, -50f), new Vector3(60f, 0f, -50f), new Vector3(60f, 0f, 50f), new Vector3(-60f, 0f, 50) },
            30, PathType.CatmullRom, PathMode.Full3D, gizmoColor: Color.red).OnComplete(() => Debug.Log("èIÇÌÇË"))
        .SetLookAt(lookAhead, Vector3.forward)
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
