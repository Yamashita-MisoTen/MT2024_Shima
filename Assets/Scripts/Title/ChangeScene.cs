using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeScene : MonoBehaviour
{
    public GameObject button;
    public GameObject TitleLogo;

    // Start is called before the first frame update
    void Start()
    {
        button.SetActive(false);

        TitleLogo = GameObject.Find("Text");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "button")
        {
            button.SetActive(true);
        }
    }

    public void BallUp()
    {
        TitleLogo.transform.position = new Vector3(0, 5, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
}
