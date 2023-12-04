using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Rease : MonoBehaviour
{
    private Vector3 scale;
    private Camera Camera;

    void Start()
    {
        Camera = GetComponent<Camera>();
        scale = new Vector3(1, -1, 1);
    }

    void OnPreCull()
    {
        Camera.ResetWorldToCameraMatrix();
        Camera.ResetProjectionMatrix();
        Camera.projectionMatrix = Camera.projectionMatrix * Matrix4x4.Scale(scale);
        Debug.Log(scale);
    }

    void OnPreRender()
    {
        if (scale.x * scale.y * scale.z < 0)
        {
            GL.invertCulling = true;
        }
    }

    void OnPostRender()
    {
        GL.invertCulling = false;
    }

    public void SetScale(bool key)
    {
        if (key)
        {
            scale = new Vector3(1, -1, 1);
        }
        else if (!key)
        {
            scale = new Vector3(1, -1, 1);
        }
    }
}
