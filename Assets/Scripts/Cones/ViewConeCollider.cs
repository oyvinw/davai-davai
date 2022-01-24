using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewConeCollider : MonoBehaviour
{
    public ViewCone Viewer;
    private Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.cyan, Color.white, Color.yellow };
    private LayerMask mask;
    private int rays = 40;

    private void Start()
    {
        mask = new LayerMask();
        mask.value = ~((1 << Viewer.gameObject.layer) | (1 << LayerMask.NameToLayer("Mask")));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.Equals(Viewer.gameObject))
        {
            Viewer.CollidingObjects.Add(collision.gameObject, collision.gameObject.transform.position);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.gameObject.Equals(Viewer.gameObject))
        {
            int i = 0;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, colors[i]);
                i = (i + 1) % colors.Length;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.Equals(Viewer.gameObject))
        {
            List<Vector2> hits = new List<Vector2>();
            var circ = (CircleCollider2D)collision;
            var center = VectorExtension.From3D(collision.bounds.center);
            var radius = circ.radius;
            Vector2 mypos = Viewer.transform.position;
            float centerAngle = Vector2.SignedAngle(Vector2.right, center - mypos);
            Vector2 top = center + VectorExtension.RotationVector2(centerAngle + 90).normalized * new Vector2(radius, radius);
            float distance = (top - mypos).magnitude + 0.5f;
            float topAngle = Vector2.SignedAngle(center - mypos, top - mypos);
            //Vector2 bottom = center + VectorExtension.RotationVector2(centerAngle - 90).normalized * new Vector2(radius, radius);
            //float bottomAngle = Vector2.SignedAngle(bottom - mypos, center - mypos);
            float angleIncrease = topAngle * 2f  / rays;
            float angle = centerAngle + topAngle;
            for (int i = 0; i < rays; i++)
            {
                Vector3 vectorAngle = VectorExtension.RotationVector3(angle);
                RaycastHit2D hit = Physics2D.Raycast(mypos, vectorAngle, distance, mask);
                if (hit.collider != null && hit.collider.Equals(collision))
                    hits.Add(hit.point);
                angle -= angleIncrease;
            }
            if (hits.Count == 0) return;
            Viewer.CollidingObjects[collision.gameObject] = hits[hits.Count/2];
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.Equals(Viewer.gameObject))
            Viewer.CollidingObjects.Remove(collision.gameObject);
    }
}
