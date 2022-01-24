using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHiveMind : MonoBehaviour
{
    EnemyController[] enemies;
    int enemyCount;
    bool[] enemyDead;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GetComponentsInChildren<EnemyController>();
        enemyCount = enemies.Length;
        enemyDead = new bool[enemyCount];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EnemyDied(object sender, EventArgs eventArgs)
    {
        EnemyController enemy = sender as EnemyController;
        if (enemy == null) return;
        enemyDead[enemy.priority] = true;
        bool gameOver = true; // Game over from the perspective of the hive mind
        for (int i = 0; i < enemyCount; i++)
            gameOver &= enemyDead[i];
        if (gameOver)
            GameManager.Instance.State = GameState.Win;
    }

    public void ExecuteAllEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            enemies[i].OnDied += EnemyDied;
            enemies[i].OnDone += EnemyDied;
            enemies[i].priority = i;
        }
    }
}
