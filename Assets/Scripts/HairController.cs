using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HairController : MonoBehaviour
{
    public GameObject hair;
    [Range(0,50)]
    public int hairNum;
    [Range(0,1)]
    public float mass;
    [Range(1,5)]
    public float length;
    [Range(0,1)]
    public float damping = 0;
    [Range(1,20)]//5,20
    public int nodeNum = 15;
    [Range(1,2)]
    public float curve;
    private List<GameObject> hairObjects;
    // Start is called before the first frame update
    void Start()
    {
        hairObjects = new List<GameObject>(hairNum);
        
        hair.GetComponent<HairRoot>().length = length;
        hair.GetComponent<HairRoot>().mass = mass;
        hair.GetComponent<HairRoot>().nodeNum = nodeNum;
        hair.GetComponent<HairRoot>().damping = damping;
        hair.GetComponent<HairRoot>().curve = curve;
        hair.GetComponent<HairRoot>().headTransform = transform;
        
        GameManager.changeText(0,length);
        GameManager.changeText(1,hairNum);
        GameManager.changeText(2,mass);
        GameManager.changeText(3,damping);
        GameManager.changeText(4,curve);
        
        for (int i = 0; i < hairNum; i++)
        {
            GameObject tmp= Instantiate(hair, randomPos(), Quaternion.identity);
            tmp.transform.parent = transform;
            hairObjects.Add(tmp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 randomPos()
    {
        float r = transform.localScale.x *0.5f;
        r = 0.5f;
        float alpha = Random.Range(0, Mathf.PI);//-GetComponentInParent<Transform>().localEulerAngles.y/180*Mathf.PI;
        // Debug.Log(alpha);
        float phi = Random.Range(0, Mathf.PI / 2);
        // alpha = 0;
        // phi = Mathf.PI / 2;
        float right = r * Mathf.Cos(alpha)*Mathf.Cos(phi);
        float up = r * Mathf.Sin(phi);
        float forward = -r * Mathf.Sin(alpha)*Mathf.Cos(phi);
        float x = transform.right.x*right+transform.up.x*up+transform.forward.x*forward;
        float y = transform.right.y*right+transform.up.y*up+transform.forward.y*forward;
        float z = transform.right.z * right + transform.up.z * up + transform.forward.z * forward;
        // Debug.Log(new Vector3(x, y, z));
        return transform.position + new Vector3(x, y, z);
    }

    public void changeLength(Slider l)
    {
        length = l.value;
        GameManager.changeText(0,length);
        hair.GetComponent<HairRoot>().length = length;
       
        for (int i = 0; i < hairNum; i++)
        {
            hairObjects[i].GetComponent<HairRoot>().length = length;
        }
    }
    public void changeMass(Slider m)
    {
        mass = m.value;
        GameManager.changeText(2,mass);
        hair.GetComponent<HairRoot>().mass = mass;
        for (int i = 0; i < hairNum; i++)
        {
            hairObjects[i].GetComponent<HairRoot>().mass = mass;
        }
    }
    public void changeDamp(Slider d)
    {
        damping = d.value;
        GameManager.changeText(3,damping);
        hair.GetComponent<HairRoot>().damping = damping;
        for (int i = 0; i < hairNum; i++)
        {
            hairObjects[i].GetComponent<HairRoot>().damping = damping;
        }
    }
    public void changeNum(Slider n)
    {
        int curNum = (int) n.value;
        GameManager.changeText(1,curNum);
        Debug.Log("num: "+curNum);
        int diff = curNum - hairNum;
        if (diff>0)
        {
            for (int i = 0; i < diff; i++)
            {
               GameObject tmp = Instantiate(hair, randomPos(), Quaternion.identity);
               tmp.transform.parent = transform;
               hairObjects.Add(tmp);
            }
        }
        else
        {
            for (int i = curNum; i < hairNum; i++)
            {
                Destroy(hairObjects[i]);
                hairObjects.Remove(hairObjects[i]);//remove!!
            }
        }
        hairNum =curNum;
    }
    
    public void changeCurve(Slider m)
    {
        curve = m.value;
        GameManager.changeText(4,curve);
        hair.GetComponent<HairRoot>().curve = curve;
        for (int i = 0; i < hairNum; i++)
        {
            hairObjects[i].GetComponent<HairRoot>().curve = curve;
        }
    }

}
