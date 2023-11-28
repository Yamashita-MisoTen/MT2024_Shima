using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class TitleAnimation : MonoBehaviour
{
    string networkaddress;
    [SerializeField]GameObject inputField;
    TMP_InputField comp;

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

    public void SetNetworkAddress(){
        comp = inputField.GetComponent<TMP_InputField>();
        networkaddress = comp.text;
    }
    public void SendNetworkAddresstoMgr(){
        var netmgrobj = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        netmgrobj.networkAddress = networkaddress;
    }

    public void OnCancel(){
        Debug.Log("あ");
    }

    public void OnDecision(){
        Debug.Log("i");
    }

    public void OnSelection(InputValue value){
        Debug.Log("u");
    }
}
