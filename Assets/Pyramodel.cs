using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramodel : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    UnityEngine.Color[] colors = new UnityEngine.Color[]
    {
        UnityEngine.Color.red,
        UnityEngine.Color.blue,
        UnityEngine.Color.green,
        UnityEngine.Color.yellow
    };
    public void SetColor(int color)
    {
        GetComponentInChildren<MeshRenderer>().material.color = colors[color];
    }

    public void SetSize(int size)
    {
        transform.localScale = Vector3.one * .5f * size;
    }
}
