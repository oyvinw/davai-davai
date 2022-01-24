using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private SquaddieController selectedUnit = null;
    private Radio radio;
    private GameManager gm;
    public Text nameDisplay;
    public Text title;
    public GameObject WinScreen;
    public GameObject LoseScreen;

    private void Start()
    {
        gm = GameManager.Instance;
        radio = FindObjectOfType<Radio>();
    }

    void Update()
    {
        //Unitselection and pathing
        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.Instance.State == GameState.Planning || GameManager.Instance.State == GameState.Execute)
            {
                Vector2 point = VectorExtension.From3D(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                SquaddieController[] selectable = FindObjectsOfType<SquaddieController>();
                for (int i = 0; i < selectable.Length; i++)
                {
                    var squaddie = selectable[i];
                    Collider2D collider = squaddie.GetSquaddieCollider();
                    if (collider.OverlapPoint(point))
                    {
                        SelectNewUnit(squaddie);

                        if (Random.value > 0.6)
                            radio.ReportSelected();

                        return;
                    }
                }
                if (Input.GetKey(KeyCode.LeftShift) && GameManager.Instance.State == GameState.Planning)
                {
                    selectedUnit.PlotPath(SnapToTile(Camera.main.ScreenToWorldPoint(Input.mousePosition)));

                    if (Random.value > 0.6)
                        radio.ReportAffirmative();
                }
                else
                {
                    DeselectUnit();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.LoadNextLevel();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gm.Execute(); 
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (selectedUnit != null)
                selectedUnit.ClearOrders();
            else
                gm.Clear();
        }

        if (Input.GetKey(KeyCode.Mouse1) && selectedUnit != null)
        {
            selectedUnit.HandleRotationInput();
        }
    }

    private Vector3 SnapToTile(Vector3 point)
    {
        return new Vector3(Mathf.Round(point.x), Mathf.Round(point.y), 0);
    }

    private void SelectNewUnit(SquaddieController squaddie)
    {
        if (selectedUnit != null)
        {
            selectedUnit.DeselectUnit();
        }
        selectedUnit = squaddie;
        selectedUnit.SelectUnit();

        UpdateNameDisplay();
    }

    private void UpdateNameDisplay()
    {
        if (selectedUnit != null)
        {
            nameDisplay.text = selectedUnit.GetStatController().unitName;
            return;
        }
        nameDisplay.text = "NONE";
    }

    public void SetTitleText(string text)
    {
        title.text = text;
    }

    public void Won(bool lastLevel)
    {
        WinScreen.SetActive(true);
        title.text = "THREAT NEUTRALIZED";
        Text winText = WinScreen.GetComponentInChildren<Text>();
        var squad = GameManager.Instance.squad;
        string t = $"CASUALTIES:\n\t{string.Join("\n\t", squad.FallenBrothers)}\n\nSURVIVORS:\n\t{string.Join("\n\t", squad.Survivors)}";
        if (lastLevel)
        {
            t += "\n\nTHANK YOU FOR PLAYING!";
            Button nextLvlBtn = WinScreen.GetComponentInChildren<Button>();
            nextLvlBtn?.gameObject.SetActive(false);
        }
        winText.text = t;
    }

    public void Lost()
    {
        LoseScreen.SetActive(true);
        title.text = "MISSION FAILED";
        Text loseText = LoseScreen.GetComponentInChildren<Text>();
        var squad = GameManager.Instance.squad;
        string t = $"MISSING IN ACTION:\n\t{string.Join("\n\t", squad.FallenBrothers)}\n\nSURVIVORS:\n\t{string.Join("\n\t", squad.Survivors)}";
        loseText.text = t;
    }

    private void DeselectUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.DeselectUnit();
            selectedUnit = null;
        }

        UpdateNameDisplay();
    }
}
