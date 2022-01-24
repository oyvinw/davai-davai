using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float LifeTime = 2f;
    public int Damage = 1;
    public int IgnoreLayer;

    void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime < 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == IgnoreLayer) return;
        Entity e = collision.transform.parent?.GetComponent<Entity>();
        if (e != null)
            e.GetStatController().LoseHealth(Damage);
        Destroy(gameObject);
    }
}
