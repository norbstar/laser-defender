using System;
using System.Collections.Generic;

using UnityEngine;

public class MathFunctions
{
    public class PointData
    {
        public Vector3? Position { get; set; } = null;
        public Quaternion? Rotation { get; set; } = null;
        public Vector3? Scale { get; set; } = null;
    }

    // Resolve the circumferance of a circle based on it's radius
    public static float GetCircumferance(float radius)
    {
        return 2.0f * Mathf.PI * Mathf.Abs(radius);
    }

    // Calculate the relative angle between a Vector2 origin and target
    public static float GetAngle(Vector2 origin, Vector2 target, int offset = 0)
    {
        Vector2 direction = target - origin;
        float angleRadians = Mathf.Atan2(direction.y, direction.x);
        float angle = (angleRadians * Mathf.Rad2Deg) + offset;

        return angle;
    }

    // Calculate an angle from a interval count and interval
    public static float GetAngle(int intervalCount, int interval, int offset = 0)
    {
        return GetAngle((float) interval / intervalCount, offset);
    }

    // Calculate an angle from a fraction in the range 0 to 1
    public static float GetAngle(float fraction, int offset = 0)
    {
        fraction = Mathf.Clamp(fraction, 0.0f, 1.0f);
        float angle = (360.0f * fraction) + offset;

        return (angle == 360.0f) ? 0.0f : angle;
    }

    // Calculate the intersection angle between two right transforms
    public static float GetForwardIntersectionAngle(Transform fromTransform, Transform toTransform)
    {
        return Vector3.Angle(fromTransform.right, toTransform.right);
    }

    // Calculate the intersection angle between a right transform and the world right vector
    public static float GetIntersectionAngle(Transform transform)
    {
        return Vector3.Angle(transform.right, Vector3.right);
    }

    // Convert a relative angle to an true angle
    public static float TrueAngle(float angle)
    {
        if (angle < 0.0f)
        {
            angle = 360.0f + angle;
        }
        
        return angle;
    }

    // Modify a true angle
    public static float ModifyTrueAngle(float angle, float adjustByAngle)
    {
        float combinedAngle = angle + adjustByAngle;

        if (combinedAngle >= 360.0f)
        {
            return combinedAngle - 360.0f;
        }
        else if (combinedAngle < 0.0f)
        {
            return 360.0f + combinedAngle;
        }

        return combinedAngle;
    }

    // Convert an true angle to a relativistic angle
    public static float RelativisticAngle(float angle)
    {
        if (angle > 180.0f)
        {
            angle = -360.0f + angle;
        }

        return angle;
    }

    // Converts a specified angle into a Vector2
    public static Vector2 AngleToVector(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    // Align the Z rotation to that of the specified vector expressed as an origin and target
    public static Quaternion AlignZRotationToVector(Quaternion rotation, Vector2 origin, Vector2 target, int offset = 0)
    {
        float angle = GetAngle(origin, target, offset);
        return Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, angle);
    }

    // A variant of the angular approach that uses Quaternion.LookRotation
    public static Quaternion AlignZRotationToVector(Vector3 origin, Vector3 target)
    {
        Vector3 vector = (target - origin).normalized;
        return Quaternion.LookRotation(Vector3.forward, vector);
    }

    // Modify a relativistic angle
    //public static float ModifyRelativisticAngle(float angle, float adjustByAngle)
    //{

    //    return angle;
    //}

    // Convert a relative angle, centred about a Vector2 with
    // a given radius to a Vector2 representing a position
    public static Vector2 GetPosition(Vector2 point, float radius, float angle, float offset = 0.0f)
    {
        float radian = (angle + offset) * Mathf.PI / 180.0f;
        float x = Mathf.Cos(radian) * Mathf.Abs(radius) + point.x;
        float y = Mathf.Sin(radian) * Mathf.Abs(radius) + point.y;

        return new Vector2((float) Math.Round(x, 6), (float) Math.Round(y, 6));
    }

    // Convert a relative angle, centred about a Vector3 with
    // a given radius to a Vector3 representing a position
    public static Vector3 GetPosition(Vector3 point, float radius, float angle, float offset = 0.0f)
    {
        float radian = (angle + offset) * Mathf.PI / 180.0f;
        float x = Mathf.Cos(radian) * Mathf.Abs(radius) + point.x;
        float z = Mathf.Sin(radian) * Mathf.Abs(radius) + point.z;

        return new Vector3((float) Math.Round(x, 6), point.y, (float) Math.Round(z, 6));
    }

    // Clamp a Vector3 within a minimum and maximium range
    public static Vector3 ClampMagnitude(Vector3 vector, float max, float min)
    {
        double magnitude = vector.sqrMagnitude;

        if (magnitude > (double) max * (double) max)
        {
            return vector.normalized * max;
        }
        else if (magnitude < (double) min * (double) min)
        {
            return vector.normalized * min;
        }

        return vector;
    }

    // Calculates a set of point data comprising the positions centred about a Vector3 at a given radius
    // denoting equal intervals about a circle and their relative inward facing rotations
    public static IList<PointData> CalculateCircularPointData(Vector3 point, float radius, int pointCount, int rotation = 0, int offset = 0)
    {
        IList<PointData> collection = new List<PointData>();

        for (int itr = 0; itr < pointCount; itr++)
        {
            float angle = MathFunctions.GetAngle(pointCount, itr, offset);
            Vector3 position = GetPosition(point, radius, angle);

            float yRotation = 0.0f;

            if (rotation < 0)
            {
                yRotation = -angle * Mathf.Rad2Deg;
            }
            else if (rotation > 0)
            {
                yRotation = angle * Mathf.Rad2Deg;
            }

            collection.Add(new PointData
            {
                Position = new Vector3(position.x, point.y, position.z),
                Rotation = Quaternion.Euler(0.0f, yRotation, 0.0f)
            });
        }

        return collection;
    }

    // Calculates an X/Y velocity based on an initial projectory and velocity
    public static Vector2 CalculateVelocity(Vector2 trajectory, float velocity)
    {
        float radian = Mathf.Atan2(trajectory.y, trajectory.x) * Mathf.Rad2Deg;
        float xVel = velocity * Mathf.Cos(radian);
        float yVel = velocity * Mathf.Sin(radian);

        return new Vector2(xVel, yVel);
    }

    // Resolve the relative ratio representing the position of a value with a clamped range
    public static float GetRelativeRatio(float min, float max, float value)
    {
        value = Mathf.Clamp(value, min, max);
        float direction = max - min;
        return (value - min) / direction;
    }

    // Resolve the value of a ratio between min and max
    public static float GetValueInRange(float min, float max, float ratio)
    {
        float direction = max - min;
        return min + (ratio * direction);
    }
}