using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PatroleMode
{
    None,
    BackAndForth,
    Circular
}

// This AI is more A than I
public class EnemyController : Entity
{
    public Vector2[] PatrolPoints;
    public PatroleMode PatroleMode;
    public float MoveSpeed = 1.0f;
    public Transform EnemyTransform;

    private Vector3[] PatrolePath;
    private int currentStep = -1;
    private Vector3 targetMovePoint;
    private Vector3 previousMovePoint;
    private float MovelerpSum = 0;
    private const float diagonalMultiplier = 0.707f;
    private float currentSpeed;
    private bool PatroleForth = true;
    public int priority;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {

        List<Vector3> tempPath = new List<Vector3>();
        if (PatrolPoints.Length > 1)
        {
            if (PatroleMode == PatroleMode.BackAndForth)
                tempPath.Add(PatrolPoints[0]);
            for (int i = 0; i < PatrolPoints.Length-1; i++)
            {
                tempPath.AddRange(AStar.ShortestPath(
                    PatrolPoints[i],
                    PatrolPoints[i + 1]
                ));
            }
            if (PatroleMode == PatroleMode.Circular)
            {
                tempPath.AddRange(AStar.ShortestPath(
                    PatrolPoints[PatrolPoints.Length-1],
                    PatrolPoints[0]
                ));
            }
            PatrolePath = tempPath.ToArray();
            targetMovePoint = PatrolPoints[0];
            previousMovePoint = EnemyTransform.position;
            currentSpeed = MoveSpeed;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Execute) return;
        if (PatroleMode != PatroleMode.None)
        {
            if (targetMovePoint == EnemyTransform.position)
            {
                if (PatroleMode == PatroleMode.Circular)
                {
                    currentStep = (currentStep + 1) % PatrolePath.Length;
                }
                else if (PatroleMode == PatroleMode.BackAndForth)
                {
                    if (PatroleForth)
                    {
                        currentStep++;
                        if (currentStep == PatrolePath.Length - 1)
                        {
                            PatroleForth = !PatroleForth;
                        }
                    }
                    else
                    {
                        currentStep--;
                        if (currentStep == 0)
                        {
                            PatroleForth = !PatroleForth;
                        }
                    }
                }
                previousMovePoint = targetMovePoint;
                targetMovePoint = PatrolePath[currentStep];
                MovelerpSum = 0;

                if (IsDiagonal(targetMovePoint - previousMovePoint))
                    currentSpeed = MoveSpeed * diagonalMultiplier;
                else
                    currentSpeed = MoveSpeed;
            }
            EnemyTransform.position = Vector3.Lerp(previousMovePoint, targetMovePoint, MovelerpSum * currentSpeed);
            MovelerpSum += Time.deltaTime;
            Vector2 direction = VectorExtension.From3D(targetMovePoint - EnemyTransform.position);
            if (direction.magnitude > 0.2)
            {
                if (direction.x < 0)
                    EnemyTransform.up = new Vector2(-direction.y, direction.x);
                else
                    EnemyTransform.right = direction;
            }
        }
    }

    private bool IsDiagonal(Vector3 vector)
    {
        return !((vector.x + vector.y == 1) || (vector.x + vector.y == -1));
    }
}
