using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        //Vector3 force = movement * speed * Time.deltaTime;
        //GetComponent<Rigidbody>().AddForce(force);
        //Debug.Log(movement+" "+force);
        //transform.position += force; // movement * speed * Time.deltaTime;
        transform.Translate(movement*speed*Time.deltaTime,Space.Self);
    }
}
