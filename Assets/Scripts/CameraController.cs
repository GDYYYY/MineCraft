using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivityX ;
    public float sensitivityY ;

    //上下最大视角(Y视角) 
    public float minY;
    public float maxY;
    private float rotationY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //根据鼠标移动的快慢(增量), 获得相机左右旋转的角度(处理X) 
        float rotationX = Input.GetAxis("Mouse X") * sensitivityX;
        //根据鼠标移动的快慢(增量), 获得相机上下旋转的角度(处理Y) 
        rotationY = -Input.GetAxis("Mouse Y") * sensitivityY;
        //角度限制. rotationY小于min,返回min. 大于max,返回max. 否则返回value  
        //rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        //设置相机角度
        transform.RotateAround(transform.position, Vector3.up, rotationX);
        Vector3 originPostion = transform.position;
        Quaternion originRotation = transform.rotation;
        transform.RotateAround(transform.position, Vector3.right, rotationY);
        if (transform.eulerAngles.x < minY || transform.eulerAngles.x > maxY)
        {
           // Debug.Log(transform.eulerAngles.x);
            transform.position = originPostion;
            transform.rotation = originRotation;
        }
        if (transform.localEulerAngles.z != 0)
        {
            float rotX = transform.localEulerAngles.x;
            float rotY = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3(rotX, rotY, 0);
        }

    }
    
}
