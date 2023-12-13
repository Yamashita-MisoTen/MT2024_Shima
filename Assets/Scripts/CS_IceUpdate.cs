using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CS_IceUpdate : NetworkBehaviour
{
    Vector3 direction;
    bool hited = false;
    float Seconds = 0;

    public float speed;
    public float exSeconds; //ë∂ç›éûä‘
    // Start is called before the first frame update
    void Start()
    {
        SetDirection();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        Seconds += Time.deltaTime;
        speed = speed - 0.005f;

        if (speed < 0)
        {
            speed = 0;
        }

        if (hited == true || Seconds > exSeconds)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

    void SetDirection()
    {
        if (transform.position.x > 0 && transform.position.z > 0)
        {
            direction = new Vector3(Random.Range(-1.0f, 0.0f), 0, Random.Range(-1.0f, 0.0f));
        }
        if (transform.position.x > 0 && transform.position.z < 0)
        {
            direction = new Vector3(Random.Range(-1.0f, 0.0f), 0, Random.Range(0.0f, 1.0f));
        }
        if (transform.position.x < 0 && transform.position.z > 0)
        {
            direction = new Vector3(Random.Range(0.0f, 1.0f), 0, Random.Range(-1.0f, 0.0f));
        }
        if (transform.position.x < 0 && transform.position.z < 0)
        {
            direction = new Vector3(Random.Range(0.0f, 1.0f), 0, Random.Range(0.0f, 1.0f));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            hited = true;
        }
    }
}
