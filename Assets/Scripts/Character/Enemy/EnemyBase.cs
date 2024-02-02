using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected int atk;
    [SerializeField] protected int spd;
    [SerializeField] protected int hp;
    [SerializeField] protected int jmp;
    [SerializeField] protected string displayName;
    [SerializeField] protected GameObject hitEffect;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float recoveryTime;
    [SerializeField] protected float attackInterval;
    [SerializeField] protected float gravity;
    [SerializeField] protected float knockbackDistance;
    [SerializeField] protected int enabledHitOrderRange;

    protected int restHp;
    protected Animator animator;
    protected float _recoveryTime;
    private BoxCollider2D collider2d;

    public event Action<GameObject> OnDie;
    public event Action<EnemyBase> OnDamage;

    public Player Target { get; set; }
    public bool IsAlive
    {
        get => animator.GetBool("alive");
        set => animator.SetBool("alive", value);
    }

    public int MaxHp { get => hp; }

    public int RestHp { get => restHp; }

    public abstract int SortingOrder { get; }

    public string DisplayName { get => displayName; }


    protected Vector3 StanPos
    {
        get;
        private set;
    }

    public bool IsStan
    {
        get => animator.GetBool("stan");
        set => animator.SetBool("stan", value);
    }

    public bool IsDamageFromBack
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        restHp = hp;
        IsAlive = true;
        IsStan = false;
        collider2d = GetComponent<BoxCollider2D>();
        AwakeImpl();
    }

    protected abstract void AwakeImpl();

    private void Update()
    {
        if (!IsAlive)
        {
            return;
        }
        if (!IsStan && _recoveryTime > 0)
        {
            _recoveryTime -= Time.deltaTime;
            return;
        }
        if (Target == null || !Target.IsAlive)
        {
            return;
        }
        UpdateImpl();
    }

    protected abstract void UpdateImpl();

    protected bool TargetWithinAttackArea()
    {
        if (Target == null)
        {
            return false;
        }
        if(!Target.IsAlive)
        {
            return false;
        }
        if (GetTargetDistance() > attackRange)
        {
            return false;
        }
        int order = Target.SortingOrder - SortingOrder;
        if (order < -enabledHitOrderRange || order > enabledHitOrderRange)
        {
            return false;
        }
        return true;
    }

    protected float GetTargetDistance()
    {
        if(Target != null)
        {
            return (Target.Position - transform.position).magnitude;
        }
        else
        {
            return 0f;
        }
    }

    protected Vector3 GetTargetDirection()
    {
        if (Target != null)
        {
            return (Target.Position - transform.position).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public abstract void ShowAttackCollider();

    protected virtual void Attack(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        int layer = obj.layer;
        if (layer == LayerMask.NameToLayer("Player"))
        {
            // 攻撃モーションが長いと多段ヒットするので当たったら消す
            HideAttackCollider();
            Player p = obj.GetComponent<Player>();
            p.Damage(atk);
        }
    }

    public abstract void HideAttackCollider();

    public virtual void Damage(int attack, bool stan)
    {
        restHp -= attack;
        _recoveryTime = recoveryTime;
        animator.SetTrigger("hurt");
        animator.SetFloat("speed", 0f);
        IsDamageFromBack = CheckDamageFromBack();
        OnDamage(this);
        if (restHp <= 0)
        {
            IsAlive = false;
            SetInvincible(true);
        }
        else
        {
            IsStan = stan;
            if (stan)
            {
                StanPos = transform.position;
            }
            else
            {
                StartCoroutine(ShowHitEffect());
            }
        }
    }

    protected abstract bool CheckDamageFromBack();

    public abstract void SetStanRotate();


    public void SetInvincible(bool isInvincible)
    {
        collider2d.enabled = !isInvincible;
    }

    private IEnumerator ShowHitEffect()
    {
        GameObject obj = Instantiate(hitEffect, gameObject.transform.position, hitEffect.transform.rotation);
        yield return new WaitForSeconds(0.3f);
        Destroy(obj);
    }

    public void Victory()
    {
        Target = null;
        animator.SetFloat("speed", 0f);
        animator.SetBool("victory", true);
    }

    public void Die()
    {
        OnDie(gameObject);
        Destroy(gameObject);
    }
}
