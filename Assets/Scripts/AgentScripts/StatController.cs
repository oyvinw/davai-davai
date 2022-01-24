using System.Collections.Generic;
using UnityEngine;

public class StatController : MonoBehaviour
{
    //Stats
    public string unitName = "I have no name";
    public int maxHealth = 100;
    public int remainingHealth;
    public float fireRate = 1;
    public float bulletSpeed = 1;
    public int damage = 1;
    public float speed = 1;
    public float turnRate = 1;

    private Radio radio;
    public DamageStateController damageStateController;

    void Start()
    {
        AssignCoolName();
        radio = FindObjectOfType<Radio>();
        remainingHealth = maxHealth;
    }

    private void Die()
    {
        //Instantiate corpse before you destroy the object here;
        GetComponent<Entity>().DestroyEntity();

        if (CompareTag("Enemy"))
        {
            radio.ReportEnemyDead();
            return;
        }

        radio.ReportSquaddieDead();
    }

    public void LoseHealth(int damage)
    {
        remainingHealth -= damage;

        if (remainingHealth < maxHealth && remainingHealth > (maxHealth / 2))
        {
            damageStateController.setDamageSeverity(DamageState.minorDamage);
            if (CompareTag("Squaddie"))
            {
                radio.ReportTakingFire();
            }
        }
        if (remainingHealth < maxHealth && remainingHealth < (maxHealth / 2))
        {
            damageStateController.setDamageSeverity(DamageState.heavyDamage);
        }
        if (remainingHealth <= 0)
        {
            damageStateController.setDamageSeverity(DamageState.dead);
            Die();
        }
    }

    private void AssignCoolName()
    {
        NameGenerator nameGen = new NameGenerator();
        unitName = nameGen.GenerateCoolCopName((int)(gameObject.GetInstanceID() * Time.realtimeSinceStartup));

        if (CompareTag("Squaddie"))
        {
            GetComponentInParent<SquaddieController>().name = unitName;
        }
    }
}
