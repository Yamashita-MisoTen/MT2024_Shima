using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanWave : MonoBehaviour
{
	[SerializeField] Vector2 _waveSpeed;
	Material _material;
	//string _texName = "_MainTex";
	// Start is called before the first frame update
	void Start()
	{
		_material = this.GetComponent<MeshRenderer>().material;
	}

	// Update is called once per frame
	void Update()
	{
		_material.mainTextureOffset += _waveSpeed;
	}
}
