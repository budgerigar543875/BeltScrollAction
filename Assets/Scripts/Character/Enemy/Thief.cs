using System.Collections;
using UnityEngine;

public class Thief : EnemyBase
{
    [SerializeField] Weapon weapon;

    Vector3 scale;
    protected SpriteRenderer[] spriteRenderers;
    protected bool attackable;
    private BoxCollider2D weaponCollider;
    private float knockbackY;

    public override int SortingOrder => spriteRenderers[0].sortingOrder;

    protected override void AwakeImpl()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        attackable = true;
        scale = transform.localScale;
        weaponCollider = weapon.gameObject.GetComponent<BoxCollider2D>();
        weaponCollider.enabled = false;
        weapon.OnAttack += Attack;
        knockbackY = knockbackDistance;
    }


    protected override void UpdateImpl()
    {
        if (IsStan)
        {
            if (transform.position.y >= StanPos.y)
            {
                knockbackY -= gravity * Time.deltaTime;
                // Player‚ÌŒü‚«‚É”ò‚Ô‚æ‚¤‚É‚·‚éiŽ©•ª‚ÌŒü‚«‚ðŠî€‚É‚·‚é‚ÆŒã‚ë‚©‚ç‰£‚ç‚ê‚Ä‚¢‚é‚Æ‚«‚É‹t‚É”ò‚Ôj
                int dir = Target.transform.localScale.x > 0 ? 1 : -1;
                transform.position += new Vector3(dir * knockbackDistance, knockbackY, 0f) * Time.deltaTime;
                float y = transform.position.y - StanPos.y;
                if (y < 0f)
                {
                    knockbackY = knockbackDistance;
                    transform.position += new Vector3(0f, y, 0f);
                }
            }
            return;
        }
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            if (TargetWithinAttackArea())
            {
                if (attackable)
                {
                    attackable = false;
                    animator.SetFloat("speed", 0f);
                    animator.SetTrigger("attack");
                    StartCoroutine(AttackInterval());
                }
            }
            else
            {
                float speed = spd * Time.deltaTime;
                animator.SetFloat("speed", speed);
                Vector3 dir = GetTargetDirection();
                transform.position += dir * speed;
                int order = OrderManager.CalcOrderInLayer(transform.position.y);
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.sortingOrder = order;
                }
                float scaleX = dir.x > 0 ? 1f : -1f;
                transform.localScale = new Vector3(scaleX * scale.x, scale.y, scale.z);
            }
        }
    }

    protected IEnumerator AttackInterval()
    {
        yield return new WaitForSeconds(attackInterval);
        attackable = true;
    }

    public override void ShowAttackCollider()
    {
        weaponCollider.enabled = true;
    }

    public override void HideAttackCollider()
    {
        weaponCollider.enabled = false;
    }

    protected override bool CheckDamageFromBack()
    {
        return Target.transform.localScale.x * transform.localScale.x < 0;
    }

    public override void SetStanRotate()
    {
        if (IsDamageFromBack)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }
}
