using System.Collections;
using UnityEngine;

public class Boss : EnemyBase
{
    [SerializeField] Weapon frontAttack;
    [SerializeField] Weapon backAttack;

    private SpriteRenderer spriteRenderer;
    private bool attackable;
    private float knockbackY;

    public override int SortingOrder => spriteRenderer.sortingOrder;

    protected override void AwakeImpl()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackable = true;
        frontAttack.OnAttack += Attack;
        backAttack.OnAttack += Attack;
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
                transform.position = transform.position + dir * speed;
                spriteRenderer.sortingOrder = OrderManager.CalcOrderInLayer(transform.position.y);
                float scaleX = 1;
                if (dir.x < 0f)
                {
                    scaleX = -1;
                }
                transform.localScale = new Vector3(scaleX, 1, 1);
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
        backAttack.gameObject.SetActive(true);
    }

    public override void HideAttackCollider()
    {
        backAttack.gameObject.SetActive(false);
        frontAttack.gameObject.SetActive(false);
    }

    public void WeaponDirectionChange()
    {
        backAttack.gameObject.SetActive(false);
        frontAttack.gameObject.SetActive(true);
    }

    protected override bool CheckDamageFromBack()
    {
        return Target.transform.localScale.x * transform.localScale.x > 0;
    }

    public override void SetStanRotate()
    {
        int dir;
        if (transform.localScale.x > 0)
        {
            dir = IsDamageFromBack ? -1 : 1;
        }
        else
        {
            dir = IsDamageFromBack ? 1 : -1;
        }
        transform.eulerAngles = new Vector3(0f, 0f, dir * 90f);
    }
}
