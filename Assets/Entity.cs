using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatController))]
public class Entity : MonoBehaviour
{
    public GameObject entityRenderColliderAndTransformObject;

    protected StatController entityInfo;
    protected Collider2D entityCollider2D;
    protected SpriteRenderer entitySpriteRenderer;
    protected Transform entityTransform;

    public EventHandler OnDied;
    public EventHandler OnDone;

    protected void Initialize()
    {
        entityInfo = GetComponent<StatController>();
        entityCollider2D = entityRenderColliderAndTransformObject.GetComponent<Collider2D>();
        entityTransform = entityRenderColliderAndTransformObject.GetComponent<Transform>();
        entitySpriteRenderer = entityRenderColliderAndTransformObject.GetComponent<SpriteRenderer>();
    }

    public void DestroyEntity()
    {
        OnDied?.Invoke(this, new EventArgs());
        Destroy(gameObject);
    }

    public StatController GetStatController()
    {
        return entityInfo;
    }

    public Collider2D GetCollider2D()
    {
        return entityCollider2D;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return entitySpriteRenderer;
    }

    public Transform GetTransform()
    {
        return entityTransform;
    }

}
