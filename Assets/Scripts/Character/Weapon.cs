using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public event Action<Collider2D> OnAttack;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnAttack != null)
        {
            OnAttack(collision);
        }
    }
}
