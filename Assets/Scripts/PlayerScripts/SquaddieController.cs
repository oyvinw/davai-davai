using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SquaddieController : Entity
{
    // Start is called before the first frame update
    private float diagonalAccountedSpeed;
    private float sumDelta;
    private const float diagonalMultiplier = 0.707f;

    public bool executeMode;
    public bool seeGhostMode;
    public bool done;
    public int priority;

    private Transform squaddieTransform;
    private Vector3? targetPos;
    private Vector3 previousPos;
    private int currentStep;
    private List<Vector3> _moveQueue;

    private List<Vector2> _rotationQueue;

    public GameObject ghost;
    public GameObject finalDestinationGhost;
    public GameObject squaddie;

    public GameObject squaddieSelection;
    private SpriteRenderer squaddieSelectionRenderer;
    private int currentGhostStep;
    private bool squaddieSelected = false;

    private Vector3? targetGhostPos;
    private Vector3 previousGhostPos;
    private SpriteRenderer ghostRenderer;
    private SpriteRenderer destinationGhostRenderer;
    private LineRenderer previewLineRenderer;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        squaddieTransform = squaddie.transform;
        ghostRenderer = ghost.GetComponent<SpriteRenderer>();
        destinationGhostRenderer = finalDestinationGhost.GetComponent<SpriteRenderer>();
        previewLineRenderer = GetComponentInChildren<LineRenderer>();
        squaddieSelectionRenderer = squaddieSelection.GetComponent<SpriteRenderer>();

        ResetState();
        _rotationQueue = new List<Vector2>();
    }

    public void ClearOrders()
    {
        _moveQueue = new List<Vector3>();
        _rotationQueue = new List<Vector2>();
        ResetGhosts();
    }

    public void HandleRotationInput()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = VectorExtension.From3D(mousePos - squaddieTransform.position);
        if (direction.x < 0)
            squaddieTransform.up = new Vector2(-direction.y, direction.x);
        else
            squaddieTransform.right = direction;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            AddRotationToQueue(direction, currentStep);
        }
    }

    public Transform GetSquaddieTransform()
    {
        return squaddieTransform;
    }

    public CircleCollider2D GetSquaddieCollider()
    {
        return squaddieTransform.GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State == GameState.Execute)
        {
            if (executeMode)
            {
                ExecuteMovement();
                ExecuteRotation();
            }
        }
        else if (GameManager.Instance.State == GameState.Planning)
        {
            if (seeGhostMode && _moveQueue.Count > 0)
            {
                ExecuteGhostMovement();
            }
        }
        if (GameManager.Instance.State == GameState.Planning ||
            GameManager.Instance.State == GameState.Execute)
        {
            if (squaddieSelected)
            {
                squaddieSelectionRenderer.transform.Rotate(0, 0, 360 * Time.deltaTime);
                squaddieSelectionRenderer.transform.position = GetSquaddieTransform().position;
            }
        }
    }

    private void ResetGhosts()
    {
        ghost.transform.position = squaddieTransform.position;
        targetGhostPos = squaddieTransform.position;
        finalDestinationGhost.transform.position = squaddieTransform.position;
        ghostRenderer.enabled = true;
        destinationGhostRenderer.enabled = true;
        seeGhostMode = true;
        previewLineRenderer.positionCount = 0;
        currentGhostStep = 0;
    }

    private void EnterBattlePhaseMode()
    {
        ghostRenderer.enabled = false;
        seeGhostMode = false;
        executeMode = true;
    }

    public void SelectUnit()
    {
        squaddieSelectionRenderer.enabled = true;
        squaddieSelected = true;

    }
    public void DeselectUnit()
    {
        squaddieSelectionRenderer.enabled = false;
        squaddieSelected = false;
    }

    public void ResetState()
    {
        _moveQueue = new List<Vector3>();
        currentStep = 0;
        done = false;
        ResetGhosts();
    }

    public void PlotPath(Vector3 point)
    {
        Vector3[] path = AStar.ShortestPath(finalDestinationGhost.transform.position, point);
        AddMoveQueue(new List<Vector3>(path));
    }

    public void AddMoveQueue(List<Vector3> moveQueue)
    {
        //ResetState();
        _moveQueue.AddRange(moveQueue);
        UpdatePreview();
    }

    public void SetRotationQueue(List<Vector2> rotationQueue)
    {
        _rotationQueue = rotationQueue;
    }

    public void AddRotationToQueue(Vector2 rotation, int index)
    {
        _rotationQueue[index] = rotation;
    }

    private void UpdatePreview()
    {
        UpdateDestinationGhost();
        UpdateLineRendererPreview();
    }

    private void UpdateDestinationGhost()
    {
        finalDestinationGhost.transform.position = _moveQueue.LastOrDefault();
    }

    public void UpdateLineRendererPreview()
    {
        int moveQueueCount = _moveQueue.Count + 1;

        Vector3[] pointsInLine = new Vector3[moveQueueCount];
        pointsInLine[0] = squaddieTransform.position;

        for (int i = 0; i < moveQueueCount - 1; i++)
        {
            pointsInLine[i + 1] = _moveQueue[i + currentStep];
        }

        previewLineRenderer.positionCount = moveQueueCount;
        previewLineRenderer.SetPositions(pointsInLine);
    }

    public void UpdateLineRendererExecute()
    {
        List<Vector3> pointList = new List<Vector3>();
        pointList.Add(squaddieTransform.position);
        pointList.AddRange(_moveQueue.GetRange(currentStep - 1, _moveQueue.Count - currentStep + 1));

        previewLineRenderer.positionCount = pointList.Count;
        previewLineRenderer.SetPositions(pointList.ToArray());
    }

    public void Execute()
    {
        if (executeMode)
        {
            Debug.Log("Already Executing order");
            return;
        }

        if (done)
        {
            Debug.Log("Order done. Press 'R' to reset for debug");
            return;
        }

        currentStep = 0;
        EnterBattlePhaseMode();
        ExecuteMovement();
        return;
    }

    private void ExecuteMovement()
    {
        if (_moveQueue.Count > 0)
        {
            if (!targetPos.HasValue || squaddieTransform.position == targetPos.Value && _moveQueue.Count > currentStep)
            {
                previousPos = targetPos ?? squaddieTransform.position;
                sumDelta = 0f;
                targetPos = _moveQueue[currentStep];

                if (IsDiagonal(_moveQueue[currentStep]))
                    diagonalAccountedSpeed = entityInfo.speed * diagonalMultiplier;
                else
                    diagonalAccountedSpeed = entityInfo.speed;

                currentStep++;
            }

            if (targetPos.HasValue)
            {
                Vector3 diff = targetPos.Value - squaddieTransform.position;
                RaycastHit2D hit = Physics2D.Raycast(
                    squaddieTransform.position + diff.normalized * 0.4f, diff,
                    0.5f,
                    1 << squaddieTransform.gameObject.layer);
                if (hit.collider == null ||
                    hit.collider.transform.parent.GetComponent<SquaddieController>().priority > priority)
                {
                    squaddieTransform.position = Vector3.Lerp(previousPos, targetPos.Value, sumDelta += Time.fixedDeltaTime * diagonalAccountedSpeed);
                    UpdateLineRendererExecute();
                }
            }

            if (currentStep == _moveQueue.Count && targetPos.HasValue && squaddieTransform.position == targetPos.Value)
            {
                executeMode = false;
                if (!done)
                {
                    done = true;
                    OnDone.Invoke(this, new EventArgs());
                }
                targetPos = null;
                return;
            }
        }
        else
        {
            executeMode = false;
            if (!done)
            {
                done = true;
                OnDone.Invoke(this, new EventArgs());
            }
            targetPos = null;
        }

    }

    private void ExecuteRotation()
    {
        /*
        Vector2 direction = _rotationQueue[currentStep];
        if (direction.x < 0)
            squaddieTransform.up = new Vector2(-direction.y, direction.x);
        else
            squaddieTransform.right = direction;
        */
    }

    private void ExecuteGhostMovement()
    {
        if (!targetGhostPos.HasValue || ghost.transform.position == targetGhostPos.Value && _moveQueue.Count > currentGhostStep)
        {
            previousGhostPos = targetGhostPos ?? squaddieTransform.position;
            sumDelta = 0f;
            targetGhostPos = _moveQueue[currentGhostStep];

            if (IsDiagonal(targetGhostPos.Value - previousGhostPos))
                diagonalAccountedSpeed = entityInfo.speed * diagonalMultiplier;
            else
                diagonalAccountedSpeed = entityInfo.speed;

            currentGhostStep++;
        }

        if (targetGhostPos.HasValue)
        {
            ghost.transform.position = Vector3.Lerp(previousGhostPos, targetGhostPos.Value, sumDelta += Time.fixedDeltaTime * diagonalAccountedSpeed);
        }

        if (currentGhostStep == _moveQueue.Count && targetGhostPos.HasValue && ghost.transform.position == targetGhostPos.Value)
        {
            ghost.transform.position = squaddieTransform.position;
            targetGhostPos = null;
            currentGhostStep = 0;
            return;
        }
    }

    //fix?
    private bool IsDiagonal(Vector3 vector)
    {
        return !((vector.x + vector.y == 1) || (vector.x + vector.y == -1));
    }
}
