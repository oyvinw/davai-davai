using UnityEngine;

public class VectorExtension
{
    public static Vector2 From3D(Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector3 From2D(Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static Vector3 RotationVector3(float degreeAngle)
    {
        return new Vector3(
            Mathf.Cos(degreeAngle * Mathf.Deg2Rad),
            Mathf.Sin(degreeAngle * Mathf.Deg2Rad)
        );
    }

    public static Vector2 RotationVector2(float degreeAngle)
    {
        return new Vector2(
            Mathf.Cos(degreeAngle * Mathf.Deg2Rad),
            Mathf.Sin(degreeAngle * Mathf.Deg2Rad)
        );
    }

    public static Vector3 RoundVector(Vector3 point)
    {
        return new Vector3(Mathf.Round(point.x), Mathf.Round(point.y), 0);
    }
}
