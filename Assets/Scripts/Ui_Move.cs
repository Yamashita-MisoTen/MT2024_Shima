using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ui_Move : MonoBehaviour
{
    enum number
    {
        HOST,
        GUEST,
    }
    private CustomNetworkManager CNetworkManager;
    [SerializeField] private GameObject Guest;
    [SerializeField] private GameObject Host;
    [SerializeField] private GameObject Address;
    [SerializeField] private GameObject Cursor;
    [SerializeField] private GameObject Connect;
    private TitleAnimation CTitleAnimation;

    private number Number;
    // Start is called before the first frame update
    void Start()
    {
        CNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        CTitleAnimation = GameObject.Find("TitleLogo").GetComponent<TitleAnimation>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Number == number.HOST)
        {
     Cursor.transform.localPosition = new Vector3(-850, -300, 0);
        }
        else if (Number == number.GUEST)
        {

            Cursor.transform.localPosition = new Vector3(150, -300, 0);
        }
    }

    private void OnCancel()
    {

    }

    private void OnDecision()
    {
        if (Number == number.HOST)
        {

            //ホスト
            CNetworkManager.StartHost();
            CTitleAnimation.ButtonPush();
            Guest.SetActive(false);
            Host.SetActive(false);
            Cursor.SetActive(false);
       
        }
        else if (Number == number.GUEST)
        {
            //ゲスト
            CTitleAnimation.ButtonPush();
            Host.SetActive(false);
            Guest.SetActive(false);
            Address.SetActive(true);
            Cursor.SetActive(false);
            Connect.SetActive(true);
        }
    }

    private void OnSelection(InputValue values)
    {
        var axis = values.Get<Vector2>();

        if (axis.x < 0.0f)
        {
            Number = number.HOST;
        }
        else if (axis.x > 0.0f)
        {
            Number = number.GUEST;
        }
    }
}
