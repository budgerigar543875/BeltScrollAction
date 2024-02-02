using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInformation: MonoBehaviour
{
    [SerializeField] HpGauge hpGauge;
    [SerializeField] Text nameText;

    Coroutine coroutine = null;

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetHp(int max, int rest)
    {
        hpGauge.SetMaxHp(max);
        hpGauge.SetRestHp(rest);
    }

    public void DisplayInformation()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(displayInformation());
    }

    private IEnumerator displayInformation()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
