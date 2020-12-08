using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedShot : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    public float lifespan;

    // Update is called once per frame
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        Destroy(gameObject, lifespan);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")){
            if((other.transform.position.x - transform.position.x) > 0.1f && (other.transform.position.x - transform.position.x) < 0.6f && Mathf.Abs(other.transform.position.y - transform.position.y) < 0.4f && 
                other.GetComponent<PlayerRevised>().getRotateState() == 0 && other.GetComponent<PlayerRevised>().getHasRed())
            {
                other.GetComponent<PlayerRevised>().chargeEnergy();
            }
            else if ((other.transform.position.x - transform.position.x) < -0.1f && (other.transform.position.x - transform.position.x) > -0.6f && Mathf.Abs(other.transform.position.y - transform.position.y) < 0.4f && 
                other.GetComponent<PlayerRevised>().getRotateState() == 2 && other.GetComponent<PlayerRevised>().getHasRed())
            {
                other.GetComponent<PlayerRevised>().chargeEnergy();
            }
            else if ((other.transform.position.y - transform.position.y) > 0.1f &&  (other.transform.position.y - transform.position.y) < 0.6f && Mathf.Abs(other.transform.position.x - transform.position.x) < 0.4f && 
                other.GetComponent<PlayerRevised>().getRotateState() == 3 && other.GetComponent<PlayerRevised>().getHasRed())
            {
                other.GetComponent<PlayerRevised>().chargeEnergy();
            }
            else if ((other.transform.position.y - transform.position.y) < -0.1f && (other.transform.position.y - transform.position.y) > -0.6f && Mathf.Abs(other.transform.position.x - transform.position.x) < 0.4f && 
                other.GetComponent<PlayerRevised>().getRotateState() == 1 && other.GetComponent<PlayerRevised>().getHasRed())
            {
                other.GetComponent<PlayerRevised>().chargeEnergy();
            }
            else
            {
                other.GetComponent<PlayerRevised>().lives--;
                Debug.Log(other.GetComponent<PlayerRevised>().lives);
            }
        }
        Destroy(gameObject);
    }
}
