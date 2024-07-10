using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VectorData
{
    public float x;
    public float y;
    public float z;

    public Vector3 GetData => new Vector3(x, y, z);
    public void SetData(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }
}
