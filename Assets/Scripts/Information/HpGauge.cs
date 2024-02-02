using UnityEngine;
using UnityEngine.UI;

public class HpGauge: MonoBehaviour
{
    [SerializeField] Image gauge1;
    [SerializeField] Image gauge2;

    int maxHp;

    public void SetMaxHp(int maxHp)
    {
        if (maxHp <= 0)
        {
            return;
        }
        this.maxHp = maxHp;
    }

    public void SetRestHp(int hp)
    {
        float hpPercent = (float)hp / maxHp;
        Vector2 size = gauge1.rectTransform.sizeDelta;
        gauge2.rectTransform.sizeDelta = new Vector2(size.x * hpPercent, size.y);
    }
}
