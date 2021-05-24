using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloth : MonoBehaviour
{
    public int m = 10;
    public int n = 10;
    public float length = 0.5f;
    public float kStruct = 300;
    public float kShear = 100;
    public float kBend = 50;
    public Vector3 gravity = new Vector3(0, -9.8f, 0);
    public float damping = 0.1f;

    public GameObject nodeGO;

    private float LStruct;
    private float LShear;
    private float LBend;

    private Vector2Int[] structDelta =
        {new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)};

    private Vector2Int[] shearDelta =
        {new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, -1)};

    private Vector2Int[] bendDelta =
        {new Vector2Int(2, 0), new Vector2Int(0, 2), new Vector2Int(-2, 0), new Vector2Int(0, -2)};

    private Vector3[] vel;
    private Vector3[] force;

    private GameObject[] nodes;
    private Vector3[] lastNodes;

    public int particleCoordinateToIdx(int i, int j)
    {
        return i * n + j;
    }

    public bool isValid(int i, int j)
    {
        return i >= 0 && j >= 0 && i < m && j < n;
    }

    // Start is called before the first frame update
    void Start()
    {
        LStruct = length;
        LShear = length * Mathf.Sqrt(2);
        LBend = 2 * length;

        nodes = new GameObject[m * n];
        vel = new Vector3[m * n];
        force = new Vector3[m * n];
        lastNodes = new Vector3[m * n];

        // Init
        for (int i = 0; i < m; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                nodes[i * n + j] = Instantiate(nodeGO, transform);
                nodes[i * n + j].transform.localPosition = new Vector3(i * length, 0, j * length);
                lastNodes[i * n + j] = nodes[i * n + j].transform.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // compute force
        for (int i = 0; i < m; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                int cur = particleCoordinateToIdx(i, j);
                Vector3 posA = nodes[cur].transform.position;
                Vector3 velA = vel[cur];

                Vector3 totalForce = new Vector3(0, 0, 0);
                // struct force here
                for (int q = 0; q < 12; q++)
                {
                    Vector2Int curNeighbour;
                    float k;
                    float l;
                    if (q < 4)
                    {
                        curNeighbour = structDelta[q];
                        k = kStruct;
                        l = LStruct;
                    }
                    else if (q < 8)
                    {
                        curNeighbour = shearDelta[q - 4];
                        k = kShear;
                        l = LShear;
                    }
                    else
                    {
                        curNeighbour = bendDelta[q - 8];
                        k = kBend;
                        l = LBend;
                    }

                    if (!isValid(i + curNeighbour.x, j + curNeighbour.y))
                    {
                        continue;
                    }

                    int curNeighbourIdx = particleCoordinateToIdx(i + (int) curNeighbour.x, j + (int) curNeighbour.y);
                    Vector3 posB = nodes[curNeighbourIdx].transform.position;
                    Vector3 velB = vel[curNeighbourIdx];
                    Vector3 distVectorAB = posA - posB;
                    float distAB = distVectorAB.magnitude;
                    Vector3 normalizedAB = distVectorAB.normalized;
                    totalForce += k * (l - distAB) * normalizedAB;
                    totalForce += damping * Vector3.Dot((velB - velA), (normalizedAB)) * normalizedAB;
                }

                force[cur] = gravity + totalForce;
            }
        }

        // constrain fixed particles
        force[0] = new Vector3(0, 0, 0);
        force[n - 1] = new Vector3(0, 0, 0);
        
        float deltaT = Time.deltaTime;
        deltaT = 0.016f;
        // update velocity and position of particles
        for (int i = 0; i < m * n; i++)
        {
            // vel[i] += force[i] * deltaT; 
            // nodes[i].transform.position += vel[i] * deltaT;
            Vector3 p2 = Verlet(lastNodes[i], nodes[i].transform.position, force[i], deltaT);
            vel[i] = (p2 - lastNodes[i]) / (2 * deltaT);
            lastNodes[i] = nodes[i].transform.position;
            nodes[i].transform.position = p2;
        }
        
    }

    Vector3 Verlet(Vector3 p0, Vector3 p1, Vector3 a, float dt)
    {
        return 2 * p1 - p0 + dt * dt * a;
    }
    
}