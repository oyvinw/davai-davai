using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCone : MonoBehaviour
{
    [SerializeField]
    private LayerMask LayerMask;
    [SerializeField]
    private Material Material;
    public float FieldOfView = 90f;
    public int RayCount = 2;
    public float ViewDistance = 4;
    private Mesh mesh;
    private MeshFilter meshFilter;
    private GameObject FovObject;
    private PolygonCollider2D Collider;
    //private MeshCollider Collider;
    public Dictionary<GameObject, Vector2> CollidingObjects = new Dictionary<GameObject, Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        FovObject = new GameObject();
        FovObject.name = $"{name}-FOV";
        FovObject.AddComponent<ViewConeCollider>().Viewer = this;
        FovObject.layer = LayerMask.NameToLayer("Mask");

        meshFilter = FovObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        var meshRenderer = FovObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Material;
        Collider = FovObject.AddComponent<PolygonCollider2D>();
        //Collider.sharedMesh = mesh;
        Collider.enabled = true;
        Collider.isTrigger = true;
        //Collider.convex = true;
    }

    // Update is called once per frame
    void Update()
    {

        float angleIncrease = FieldOfView / RayCount;
        Vector2[] verticies2d = new Vector2[RayCount + 1 + 1];
        Vector3[] verticies = new Vector3[RayCount + 1 + 1]; // 1 for index 0 and 1 for last index
        Vector2[] uv = new Vector2[verticies.Length];
        int[] triangles = new int[RayCount * 3];

        Vector3 origin = transform.position;
        verticies[0] = origin;
        verticies2d[0] = origin;

        float parentAngle = transform.rotation.eulerAngles.z;
        float angle = parentAngle + FieldOfView / 2f;
        for (int i = 0; i <= RayCount; i++)
        {
            Vector3 vectorAngle = VectorExtension.RotationVector3(angle);
            RaycastHit2D rch = Physics2D.Raycast(origin, vectorAngle, ViewDistance, LayerMask);
            if (rch.collider == null)
            {
                verticies[i + 1] = origin + (vectorAngle) * ViewDistance;
            }
            else
            {
                verticies[i + 1] = rch.point;
            }
            verticies2d[i + 1] = verticies[i + 1];

            if (i > 0)
            {
                triangles[(i - 1) * 3 + 0] = 0;
                triangles[(i - 1) * 3 + 1] = i;
                triangles[(i - 1) * 3 + 2] = i + 1;
            }
            angle -= angleIncrease;
        }

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
        Collider.SetPath(0, verticies2d);
    }

    private void OnDestroy()
    {
        Destroy(FovObject);
    }
}
