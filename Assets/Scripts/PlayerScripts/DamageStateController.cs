using UnityEngine;

public class DamageStateController : MonoBehaviour
{
    public Sprite minorDamage;
    public Sprite heavyDamage;
    public GameObject corpse;
    private SpriteRenderer damageRenderer;

    void Start()
    {
        damageRenderer = GetComponent<SpriteRenderer>();
    }

    public void setDamageSeverity(DamageState damageState)
    {
        Debug.Log("Hit");
        switch (damageState)
        {
            case DamageState.minorDamage:
                damageRenderer.sprite = minorDamage;
                break;
            case DamageState.heavyDamage:
                damageRenderer.sprite = heavyDamage;
                break;
            case DamageState.dead:
                Instantiate(corpse, transform, true);
                break;
            default:
                damageRenderer.sprite = null;
                break;
        }
    }
}

public enum DamageState
{
    minorDamage,
    heavyDamage,
    dead
}