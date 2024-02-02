using UnityEngine;

public class MovableArea
{
    public const float MAX_X = 8f;
    public const float MIN_X = -8f;
    public const float CENTER_X = (MAX_X + MIN_X) / 2f;
    public const float MAX_Y = -2f;
    public const float MIN_Y = -5f;
    public const float WIDTH = MAX_X - MIN_X;

    public static Vector3 Adjust(Vector3 position, bool scrolling)
    {
        float max_x;
        if (scrolling)
        {
            max_x = CENTER_X;
        }
        else
        {
            max_x = MAX_X;
        }
        return new Vector3(Mathf.Min(max_x, Mathf.Max(position.x, MIN_X)), Mathf.Min(MAX_Y, Mathf.Max(position.y, MIN_Y)), position.z);
    }
}
