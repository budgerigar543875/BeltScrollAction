using UnityEngine;

public class BackgroundManualScroller : MonoBehaviour
{
    [SerializeField] float speedCoefficient;

    float startX;
    float sizeX;
    float endX;

    private void Awake()
    {
        startX = transform.localPosition.x;
        sizeX = GetComponent<SpriteRenderer>().localBounds.size.x * transform.localScale.x;
        endX = startX - sizeX;
    }

    public void Scroll(float x)
    {
        transform.localPosition -= new Vector3(speedCoefficient * x, 0f, 0f);
        float posX = transform.localPosition.x;
        if (posX < endX)
        {
            transform.localPosition = new Vector3(posX + sizeX, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
