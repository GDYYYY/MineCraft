using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HairRoot : MonoBehaviour
{
    [Range(0,1)]
    public float mass;
    [Range(1,5)]
    public float length;
    [Range(5,20)]
    public int nodeNum = 5;
    [Range(5,20)]
    public int iterations = 5;
    [Range(0,1)]
    public float damping = 0;
    [Range(1,2)]
    public float curve = 2;
    public GameObject hairNode;
    private List<Vector3> lastNodes; //前一帧
    private List<Vector3> nodes; //当前帧
    // private List<GameObject> nodeObjects;

    private LineRenderer _lineRenderer;

    private Vector3 gravity = new Vector3(0, -98f, 0);

    public Transform headTransform;

    private Vector3 offsetPos;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 cur = transform.position;
        offsetPos = cur - headTransform.position;

        nodes = new List<Vector3>(nodeNum + 1);
        lastNodes = new List<Vector3>(nodeNum + 1);
        // nodeObjects = new List<GameObject>(nodeNum + 1);

        nodes.Add(cur);
        lastNodes.Add(cur);
        // nodeObjects.Add(this.gameObject);
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = nodeNum + 1;
        _lineRenderer.SetPosition(0, cur);

        //Vector3 cur = transform.position;
        float interval = length / nodeNum;
        for (int i = 0; i < nodeNum; i++)
        {
            cur.z += interval;

            // GameObject tmp = Instantiate(hairNode, cur, Quaternion.identity);
            // tmp.transform.parent = this.transform;

            nodes.Add(cur);
            lastNodes.Add(cur);
            // nodeObjects.Add(tmp);

            _lineRenderer.SetPosition(i + 1, cur);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dt = Time.deltaTime;
        for (int i = 1; i <= nodeNum; i++)
        {
            Vector3 p2 = Verlet(lastNodes[i], nodes[i], damping, mass * gravity, dt);
            lastNodes[i] = nodes[i];
            nodes[i] = p2;
        }

        float l = length / nodeNum;

        for (int j = 0; j < iterations; j++)
        {
            for (int i = 1; i <= nodeNum; i++)
            {
                lengthConstraint(nodes, i-1,i, l);
                collideSphere(headTransform, i);
                //Debug.Log(i.ToString()+' '+nodes[i]);
            }
            for (int i = 2; i <= nodeNum; i++)
            {
                lengthConstraint(nodes, i-2,i, curve*l);
                radiusConstraint(nodes, i-1, curve*l);
                // collideSphere(headTransform, i);
            }

            nodes[0] = transform.position; //offsetPos + headTransform.position;
        }

        for (int i = 0; i <= nodeNum; i++)
        {
            setPos(nodes[i], i);
        }
    }

    void collideSphere(Transform sphere, int index)
    {
        float r = sphere.localScale.x* 0.55f;
        Vector3 distV = nodes[index] - sphere.position;
        float dist=Mathf.Abs(distV.magnitude);
        // Debug.Log(index.ToString()+' '+dist);
        if (dist < r)
        {
            // Debug.Log(index.ToString()+' '+dist);
            nodes[index] = sphere.position + distV * r / dist;
        }
    }

    void radiusConstraint(List<Vector3> nodes, int i, float r)
    {
        Vector3 distV = nodes[i] -  nodes[0];
        float dist=Mathf.Abs(distV.magnitude);
        if (dist > r*i)
        {
            nodes[i] = nodes[i-1] + (nodes[i]-nodes[i-1]).normalized * r;
        }
    }
    void lengthConstraint(List<Vector3> nodes, int i0, int i1,float l)
    {
        Vector3 p0 = nodes[i0];
        Vector3 p1 = nodes[i1];
        Vector3 distV = p1 - p0;
        float dist = Mathf.Abs(distV.magnitude);
        //Debug.Log(dist);
        Vector3 tmp = (0.5f - l / (2 * dist)) * distV;
        //     nodes[i] = p1 - 2 * tmp;
        //     return;
        nodes[i0] = p0 + tmp;
        nodes[i1] = p1 - tmp;
        //Debug.Log("tmp: "+tmp);
    }

    Vector3 Verlet(Vector3 p0, Vector3 p1, float d, Vector3 a, float dt)
    {
        return p1 + d * (p0 - p1) + dt * dt * a;
    }

    void setPos(Vector3 pos, int i)
    {
        // nodeObjects[i].transform.position = pos;
        _lineRenderer.SetPosition(i, pos);
    }
    
    
    
}