using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public const int OUTER_TYPE_BARREL = 0x1;
    public const int OUTER_TYPE_BOX = 0x2;
    public const int ITEM_TYPE_FULL_RCV = 0x1;
    public const int ITEM_TYPE_LITTLE_RCV = 0x2;

    [SerializeField] GameObject barrel;
    [SerializeField] GameObject box;
    [SerializeField] GameObject fullRecovery;
    [SerializeField] GameObject littleRecovery;

    [SerializeField] float stageWidth;
    [SerializeField] int numItems;

    private List<GameObject> items;

    private void Awake()
    {
        items = new List<GameObject>();
    }

    public void Spawn()
    {
        int itemDistance = (int)(stageWidth / (numItems + 1));
        for (int i = 0; i < numItems; i++)
        {
            int y = UnityEngine.Random.Range((int)MovableArea.MIN_Y, (int)MovableArea.MAX_Y - 1);
            int x = itemDistance * (i + 1);
            GameObject item = SpawnItem();
            item.transform.position = new Vector3(x, y + 0.5f, 0);
            item.GetComponent<SpriteRenderer>().sortingOrder = OrderManager.CalcOrderInLayer(item.transform.position.y);
            items.Add(item);
            OuterItem outer = item.GetComponent<OuterItem>();
            outer.OnSpawnInnerItem += SpawnInnerItem;
        }
    }

    public GameObject SpawnItem()
    {
        int outer = UnityEngine.Random.Range(OUTER_TYPE_BARREL, OUTER_TYPE_BOX + 1);
        int inner = UnityEngine.Random.Range(ITEM_TYPE_FULL_RCV, ITEM_TYPE_LITTLE_RCV + 1);
        GameObject outerPrefab = null;
        switch (outer)
        {
            case OUTER_TYPE_BARREL:
                outerPrefab = barrel;
                break;
            case OUTER_TYPE_BOX:
                outerPrefab = box;
                break;
        }
        GameObject innerPrefab = null;
        switch (inner)
        {
            case ITEM_TYPE_FULL_RCV:
                innerPrefab = fullRecovery;
                break;
            case ITEM_TYPE_LITTLE_RCV:
                innerPrefab = littleRecovery;
                break;
        }
        if (outerPrefab != null && innerPrefab != null)
        {
            GameObject obj = Instantiate(outerPrefab);
            OuterItem item = obj.GetComponent<OuterItem>();
            item.SetInner(innerPrefab);
            return obj;
        }
        throw new NullReferenceException(string.Format("Outer: {0}, Inner: {1}", outer, inner));
    }

    private void SpawnInnerItem(GameObject outerItem)
    {
        OuterItem outer = outerItem.GetComponent<OuterItem>();
        GameObject innerItem = Instantiate(outer.InnerPrefab, outer.transform.position, outer.InnerPrefab.transform.rotation);
        items.Remove(outerItem);
        items.Add(innerItem);
    }

    public void Move(float dx)
    {
        items.ForEach(item => item.transform.localPosition -= new Vector3(dx, 0f, 0f));
    }

    public bool GetItem(Vector3 position, out GameObject item)
    {
        foreach (GameObject itm in items)
        {
            RecoveryItem r = itm.GetComponent<RecoveryItem>();
            if (r == null)
            {
                continue;
            }
            if (Mathf.Abs(r.transform.position.x - position.x) < 0.5f && Mathf.Abs(r.transform.position.y - position.y) < 0.5f)
            {
                items.Remove(itm);
                item = itm;
                return true;
            }
        }
        item = null;
        return false;
    }
}
