using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Item : NetworkBehaviour
{
    [SerializeField] public Texture2D itemTex;
    public virtual void UseEffect(Vector3 trans, Quaternion qt){}
}
