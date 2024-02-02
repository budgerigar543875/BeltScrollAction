using System;
using System.Collections;
using UnityEngine;

public class OuterItem : MonoBehaviour
{
    [SerializeField] int itemIndex;
    [SerializeField] GameObject innerPrefab;

    public event Action<GameObject> OnSpawnInnerItem;

    public GameObject InnerPrefab => innerPrefab;

    public int ItemIndex => itemIndex;

    public void SetIndex(int itemIndex)
    {
        this.itemIndex = itemIndex;
    }

    public void SetInner(GameObject innerPrefab)
    {
        this.innerPrefab = innerPrefab;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            StartCoroutine(DestroyObject());
        }
    }

    private IEnumerator DestroyObject()
    {
        GetComponent<Collider2D>().enabled = false;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 5; i++)
        {
            renderer.enabled = !renderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        if (OnSpawnInnerItem != null)
        {
            OnSpawnInnerItem(gameObject);
        }
        Destroy(gameObject);
    }
}
