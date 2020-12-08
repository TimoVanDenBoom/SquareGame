using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCannon : MonoBehaviour
{
    public GameObject shot;
    public float shotTime;
    private float shotTimeOriginal;
    public Transform shotPoint;

    private void Start()
    {
        shotTimeOriginal = shotTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > shotTime)
        {
            shotTime = Time.time + shotTimeOriginal;
            Instantiate(shot, shotPoint.position, shotPoint.rotation);
            
        }
    }
}
