using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    [SerializeField] HpGauge hpGauge;
    [SerializeField] Text restText;

    public void SetMaxHp(int maxHp)
    {
        hpGauge.SetMaxHp(maxHp);
    }

    public void SetRestHp(int hp)
    {
        hpGauge.SetRestHp(hp);
    }

    public void UpdateRest(int rest)
    {
        restText.text = string.Format("Å~{0}", rest);
    }
}
