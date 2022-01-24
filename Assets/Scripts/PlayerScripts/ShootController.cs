using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public Rigidbody2D projectile;
    private Entity unitEntity;
    private StatController unitInfo;
    private ViewCone viewCone;
    private MakeShootSound shootSound;
    private Radio radio;
    private float time;

    private bool madeContact;

    private void Start()
    {
        viewCone = GetComponentInChildren<ViewCone>();
        unitEntity = GetComponent<Entity>();
        unitInfo = unitEntity.GetStatController();
        radio = FindObjectOfType<Radio>();
        shootSound = GetComponent<MakeShootSound>();
        madeContact = false;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State != GameState.Execute) return;
        Vector2? target = AssignClosestTarget();
        if (target.HasValue)
        {
            if (!madeContact)
            {
                radio.EnemySpotted();
            }

            time += Time.deltaTime * unitInfo.fireRate;
            if(time >= 1)
            {
                time = 0;
                Shoot(target.Value);
            }
        }
    }

    public void Shoot(Vector2 target)
    {
        var myPos = unitEntity.GetTransform().position;
        Vector3 targetPos = target;

        //Bullet logic
        Vector3 tanr = myPos - targetPos;
        Vector3 norm = tanr.normalized;
        norm.Scale(new Vector3(1.5f, 1.5f, 1.5f));
        tanr += norm;
        Rigidbody2D bulletClone = Instantiate(projectile, myPos, Quaternion.Euler(0, 0, Mathf.Rad2Deg*Mathf.Atan2(tanr.y, tanr.x))) as Rigidbody2D;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        var bulletCollider = bulletClone.GetComponent<Collider2D>();
        foreach(var collider in colliders)
        {
            Physics2D.IgnoreCollision(bulletCollider, collider);
        }

        var proj = bulletClone.GetComponent<Projectile>();
        proj.Damage = unitInfo.damage;
        proj.IgnoreLayer = gameObject.GetComponent<Entity>().entityRenderColliderAndTransformObject.layer;

        Vector3 bulletDirection = (targetPos - bulletClone.transform.position).normalized * unitInfo.bulletSpeed;
        bulletClone.AddForce(VectorExtension.From3D(bulletDirection));

        shootSound.BangBang();
    }

    private Vector2? AssignClosestTarget()
    {
        Dictionary<GameObject, Vector2> unitsInVision = viewCone.CollidingObjects;
        if (unitsInVision.Count == 0)
        {
            return null;
        }

        float minDist = float.PositiveInfinity;
        Vector2? closest = null;
        foreach (var unit in unitsInVision)
        {
            if (unit.Key.CompareTag("Projectile") || unit.Key.CompareTag(unitEntity.tag))
                continue;

            float dist = Vector3.Distance(unitEntity.GetTransform().position, unit.Value);
            if (dist < minDist)
            {
                minDist = dist;
                closest = unit.Value;
            }
        }

        return closest;
    }
}
