using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolMove : MonoBehaviour
{
    private bool isMouseDown;
    private Vector3 offset;
   // private Transform edge;
   // public Transform checkPoint;
    private float size;

    // Start is called before the first frame update
    void Start()
    {
       // size = checkPoint.position.x - transform.position.x;
        //checkPoint = GetComponentInChildren<Transform>();
        //edge = GameObject.Find("ToolLine").transform;
        //safe = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            
        }

        if (isMouseDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //鼠标射线 屏幕坐标转为世界坐标
            RaycastHit hit;//射线与游戏物体的碰撞检测
            if (Physics.Raycast(ray, out hit))
            {

                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.name = name;
                //cube.transform.localScale *= pos;
                //cube.GetComponent<Renderer>().material.mainTexture = tex;
                //Vector3 direction = hit.point - hit.transform.position;//AAAAAAAA
                //if (abs(direction.x) > abs(direction.y) && abs(direction.x) > abs(direction.z))
                //{
                //    if (direction.x > 0)
                //    {
                //        cube.transform.position = hit.transform.position + hit.transform.right * pos;
                //    }
                //    else if (direction.x < 0)
                //    {
                //        cube.transform.position = hit.transform.position - hit.transform.right * pos;
                //    }
                //}
                //if (abs(direction.y) > abs(direction.x) && abs(direction.y) > abs(direction.z))
                //{
                //    if (direction.y > 0)
                //    {
                //        cube.transform.position = hit.transform.position + hit.transform.up * pos;
                //    }
                //    else if (direction.y < 0)
                //    {
                //        cube.transform.position = hit.transform.position - hit.transform.up * pos;
                //    }
                //}
                //if (abs(direction.z) > abs(direction.y) && abs(direction.z) > abs(direction.x))
                //{
                //    if (direction.z > 0)
                //    {
                //        cube.transform.position = hit.transform.position + hit.transform.forward * pos;
                //    }
                //    else if (direction.z < 0)
                //    {
                //        cube.transform.position = hit.transform.position - hit.transform.forward * pos;
                //    }
                //}
            }
        }
    }

    void MoveTool()
    {
        offset = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset.z = 0;
        //if (offset.x+size < edge.position.x)
            transform.position = offset;
    }

}
