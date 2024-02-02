using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] BackgroundManualScroller[] manualScrollers;
    [SerializeField] float scrollSpeed;

    public void Move(float dx)
    {
        float speed = scrollSpeed * dx;
        foreach (var manualScroller in manualScrollers)
        {
            manualScroller.Scroll(speed);
        }
    }
}
