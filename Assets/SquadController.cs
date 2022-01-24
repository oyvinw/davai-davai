using System;
using System.Collections.Generic;
using UnityEngine;

public class SquadController : MonoBehaviour
{
    // Start is called before the first frame update
    SquaddieController[] squaddies;
    bool[] squaddieDead;

    int squaddieCount;
    public List<string> FallenBrothers = new List<string>();
    public List<string> Survivors = new List<string>();

    void Start()
    {
        squaddies = GetComponentsInChildren<SquaddieController>();
        squaddieCount = squaddies.Length;
        squaddieDead = new bool[squaddieCount];
    }

    private void SquaddieDied(object sender, EventArgs e)
    {
        SquaddieController squaddie = sender as SquaddieController;
        if (squaddie == null) return;
        FallenBrothers.Add(squaddie.name);
        Survivors.Remove(squaddie.name);
        SquaddieDiedOrDone(sender, e);
    }

    private void SquaddieDone(object sender, EventArgs e)
    {
        SquaddieController squaddie = sender as SquaddieController;
        if (squaddie == null) return;
        SquaddieDiedOrDone(sender, e);
    }

    private void SquaddieDiedOrDone(object sender, EventArgs e)
    {
        SquaddieController squaddie = sender as SquaddieController;
        if (squaddie == null) return;
        squaddieDead[squaddie.priority] = true;
        bool gameOver = true;
        for (int i = 0; i < squaddieCount; i++)
            gameOver &= squaddieDead[i];
        if (gameOver)
            GameManager.Instance.State = GameState.Lose;
    }

    public void ExecuteAllSquaddieOrders()
    {
        for (int i = 0; i < squaddieCount; i++)
        {
            Survivors.Add(squaddies[i].name);
            squaddies[i].OnDied += SquaddieDied;
            squaddies[i].OnDone += SquaddieDone;
            squaddies[i].priority = i;
            squaddies[i].Execute();
        }
    }

    public void ResetAllSquaddies()
    {
        for (int i = 0; i < squaddieCount; i++)
        {
            squaddies[i].ResetState();
        }
    }

    public void ClearAllSquaddieOrders()
    {
        for (int i = 0; i < squaddieCount; i++)
        {
            squaddies[i].ClearOrders();
        }
    }
}
