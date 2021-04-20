using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTool : MonoBehaviour
{
    public float distance;

    public float h;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeTool(GameObject tool)
    {
        //float h =-0.5f;
        GameObject newItem= Instantiate(tool,  transform.position+transform.forward*distance+transform.up*h, Quaternion.identity);
        //gameObject.SetActive(false);
      
    }
}
