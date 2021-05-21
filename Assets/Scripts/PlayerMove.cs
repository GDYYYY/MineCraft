using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float jump = Input.GetAxis("Jump");

        // Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        Vector3 movement = new Vector3(moveHorizontal, jump, moveVertical);
        //Vector3 force = movement * speed * Time.deltaTime;
        
        //Debug.Log(movement+" "+force);
        //transform.position += force; // movement * speed * Time.deltaTime;
        transform.Translate(movement*speed*Time.deltaTime,Space.Self);
        
        // rb.AddForce(new Vector3(0,jump,0));
    }
}
