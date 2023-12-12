using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoldFishMove2 : MonoBehaviour
{
    Tweener GoldenFish2;
   // [SerializeField] public Vector3 zahyou01;
    [SerializeField] public float time2;

    // Start is called before the first frame update
    void Start()
    {
        float lookAhead = 0.8f;
      GoldenFish2   =  transform.DOLocalPath(new Vector3[] { new Vector3(60f, 0f, -50f), new Vector3(-60f, 0f, -50f), new Vector3(-60f, 0f, 50f), new Vector3(60f, 0f, 50) },
          30, PathType.CatmullRom, PathMode.Full3D, gizmoColor: Color.red).OnComplete(() => Debug.Log("I‚í‚è"))
      .SetLookAt(lookAhead, Vector3.forward)
      .SetLoops(-1, LoopType.Restart)
      .SetEase(Ease.Linear);
      // DOVirtual.DelayedCall(time2, () => GoldenFish2.Kill());
    }

    // Update is called once per frame
    void Update()
    {


        


    }



 
}
