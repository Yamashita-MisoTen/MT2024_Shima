using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMove : MonoBehaviour
{
	float moveSpeed = 2.0f;
	float radian = 0.8f;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		var speed = moveSpeed * Time.time;

		var x = Mathf.Cos(speed) * radian;
		var z = Mathf.Sin(speed) * radian;

		var pos = new Vector3(0,0,0);
		pos.x = x;
		pos.z = z;
		this.gameObject.transform.position = pos;
	}
}
