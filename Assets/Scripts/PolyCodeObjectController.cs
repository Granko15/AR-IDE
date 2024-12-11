using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyCodeObjectController : MonoBehaviour
{
    [SerializeField] private GameObject polyCodeObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateObject()
    {
        polyCodeObject.transform.Rotate(0, 51, 0);
    }
}
