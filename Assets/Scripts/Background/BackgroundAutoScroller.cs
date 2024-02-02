using UnityEngine;

public class BackgroundAutoScroller : MonoBehaviour
{
    [SerializeField] float scrollSpeed;

    float startX;
    float sizeX;
    float endX;

    private void Awake()
    {
        startX = transform.localPosition.x;
        sizeX = GetComponent<SpriteRenderer>().localBounds.size.x * transform.localScale.x;
        endX = startX - sizeX;
    }

    private void FixedUpdate()
    {
        transform.localPosition -= new Vector3(scrollSpeed, 0f, 0f);
        float posX = transform.localPosition.x;
        if (posX < endX)
        {
            transform.localPosition = new Vector3(posX + sizeX, 0f, 0f);
        }
    }
}
