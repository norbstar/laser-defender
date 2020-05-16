using UnityEngine;

public class VectorFunctions
{
    // Construct a Vector2 from a Vector3, disguarding the Y value
    public static Vector2 ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    // Calcualate the difference between a pair of Vector2
    public static Vector2 Difference(Vector2 vectorA, Vector2 vectorB)
    {
        return new Vector2(vectorA.x - vectorB.x, vectorA.y - vectorB.y);
    }

    // Construct a Vector3 from a Vector2 and Y value aggregate
    public static Vector3 ToVector3(Vector2 vector, float z = 0.0f)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    // Calcualate the difference between a pair of Vector3
    public static Vector3 Difference(Vector3 vectorA, Vector3 vectorB)
    {
        return new Vector3(vectorA.x - vectorB.x, vectorA.y - vectorB.y, vectorA.z - vectorB.z);
    }
}