using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public partial class GameRuleManager
{
    enum ResultWave
    {
        ICE_CUT,
        PENLOSE,
        PENLOSEUPDATE,
        PENWIN
    }
    private ResultWave Wave = ResultWave.ICE_CUT;
    private Animator Iceanimator;
    private GameObject IceStage;
    private bool ResultUpdateOn = false;
    private bool AnimationFinish = false;
    // Update is called once per frame

    void UpdateResultManager()
    {
        if (ResultUpdateOn)
        {
            switch (Wave)
            {
                //リザルトのアニメーション
                case ResultWave.ICE_CUT:

                    if (IceStage == null)
                    {
                        IceStage = GameObject.Find("Ice" + LosePayer);
                    }
                    else if (Iceanimator == null)
                    {
                        Iceanimator = IceStage.GetComponent<Animator>();
                        DOVirtual.DelayedCall(1.5f, () => Iceanimator.SetTrigger("SetCut"));

                        for (int i = 0; i < _playerData.Count; i++)
                        {
                            if (_playerData[i] == _orgaPlayer)
                            {
                                _ResultObject[i].GetComponent<Result_Animation>().SetStart();
                            }
                            if (_playerData[i] != _orgaPlayer)
                            {
                                _ResultObject[i].GetComponent<Result_Animation>().SetStart();
                            }
                        }
                    }
                    if (Iceanimator != null)
                    {
                        if (Iceanimator.GetCurrentAnimatorStateInfo(0).IsName("Finish"))
                        {
                            IceStage.SetActive(false);
                        }
                        for (int i = 0; i < _playerData.Count; i++)
                        {
                            if (_playerData[i] == _orgaPlayer)
                            {
                                if (_ResultObject[i].GetComponent<Result_Animation>().GetGetCurrentAnimatorStateInfo())
                                {
                                    Wave = ResultWave.PENWIN;
                                    _ResultObject[i].SetActive(false);
                                    AnimationFinish = true;
                                }
                            }
                        }
                    }
                    break;

                    /*   case ResultWave.PENWIN:
                           for (int i = 0; i < _playerData.Count; i++)
                           {
                               if (_playerData[i] != _orgaPlayer)
                               {
                                   _ResultObject[i].GetComponent<Result_Animation>().SetStart();
                               }
                           }
                           AnimationFinish = true;
                           break;*/
            }
        }
    }
}
