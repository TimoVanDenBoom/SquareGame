using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        position = transform.parent.transform.position - new Vector3(0, 0.5f, 0);
    }

    void Update()
    {
        transform.position = transform.parent.transform.position - new Vector3(0, 0.5f, 0);
    }

    void LateUpdate()
    {
        transform.position = transform.parent.transform.position - new Vector3(0, 0.5f, 0);
    }
}
