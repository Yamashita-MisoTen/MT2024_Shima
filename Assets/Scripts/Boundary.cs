using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    [ColorUsage(false, true), SerializeField] private Color _hdrColor;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", _hdrColor);
    }
}
