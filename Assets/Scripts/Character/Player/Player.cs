using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int attack;
    [SerializeField] int speed;
    [SerializeField] int specialAttackMoveSpeed;
    [SerializeField] int maxHp;
    [SerializeField] int jump;
    [SerializeField] float gravity;
    [SerializeField] Weapon nomalAttackArea;
    [SerializeField] Weapon specialAttackArea;
    [SerializeField] Weapon jumpAttackArea;
    [SerializeField] PlayerInformation playerInformation;
    [SerializeField] AudioClip slashAudio;
    [SerializeField] AudioClip fireballAudio;
    [SerializeField] GameObject fireball;
    [SerializeField] int comboCountTimeLimitMilliSec;
    [SerializeField, Range(0, 5)] float reviveInvincibleTimeSec;
    [SerializeField] AudioClip specialAttackAudio;

    public const int COMBO_COUNT_STAN = 3;

    private GameManager gameManager;
    private int rest;
    private int restHp;
    private Vector3 groundPos;
    private bool isJumping;
    private Vector3 jumpDir;
    private float jumpPower;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Coroutine flashing;
    private bool isHit;
    private int comboCount;
    private AudioSource audioSource;
    private SpriteRenderer fireballSpriteRenderer;
    private DateTime lastAttackTime;
    private bool isScrolling;
    private BoxCollider2D collider2d;
    public bool isSpecialAttack;

    private enum AttackType
    {
        Normal = 0,
        Special = 1,
        Jump = 2,
    }

    public event Action OnDie;
    public event Action<Vector3> OnMove;

    public int MaxHp => maxHp;

    public bool IsAlive => animator.GetBool("isAlive");

    public int SortingOrder => spriteRenderer.sortingOrder;

    public Vector3 Position => groundPos;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rest = gameManager.Rest;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        fireballSpriteRenderer = fireball.GetComponent<SpriteRenderer>();
        lastAttackTime = DateTime.Now;
        isScrolling = false;
        collider2d = GetComponent<BoxCollider2D>();
        isSpecialAttack = false;
    }

    private void Start()
    {
        restHp = maxHp;
        groundPos = transform.position;
        nomalAttackArea.OnAttack += AttackImpl;
        specialAttackArea.OnAttack += AttackImpl;
        jumpAttackArea.OnAttack += AttackImpl;
        spriteRenderer.sortingOrder = OrderManager.CalcOrderInLayer(transform.position.y);
        playerInformation.SetMaxHp(MaxHp);
        playerInformation.SetRestHp(MaxHp);
        playerInformation.UpdateRest(rest);
        isHit = false;
        comboCount = 0;
        animator.SetBool("isAlive", true);
    }

    public void Move(Vector3 direction)
    {
        Vector3 move;
        if (isJumping)
        {
            move = Move_Jump(jumpDir);
        }
        else if (isSpecialAttack)
        {
            move = Move_SpecialAttack(direction);
        }
        else
        {
            move = Move_Walk(direction);
            animator.SetFloat("speed", Mathf.Abs(move.magnitude));
        }
        // 動いたらコンボ終了
        if (move != Vector3.zero)
        {
            comboCount = 0;
        }
        spriteRenderer.sortingOrder = OrderManager.CalcOrderInLayer(groundPos.y);
        if(OnMove != null)
        {
            OnMove(move);
        }
    }

    public void SetScrolling(bool isScrolling)
    {
        this.isScrolling = isScrolling;
    }

    private Vector3 Move_Walk(Vector3 direction)
    {
        if (restHp <= 0)
        {
            return Vector3.zero;
        }
        if (direction == Vector3.zero)
        {
            return Vector3.zero;
        }
        if (IsHurting())
        {
            return Vector3.zero;
        }
        if (IsAttacking())
        {
            return Vector3.zero;
        }
        if(IsGettingItem())
        {
            return Vector3.zero;
        }
        if (direction.x * transform.localScale.x < 0f)
        {
            Vector3 playerDir = transform.localScale;
            playerDir.x *= -1f;
            transform.localScale = playerDir;
        }
        Vector3 move = speed * direction * Time.deltaTime;
        groundPos = CalcGroundPosition(move);
        transform.position = groundPos;
        return move;
    }

    private Vector3 CalcGroundPosition(Vector3 move)
    {
        Vector3 pos = MovableArea.Adjust(groundPos + move, isScrolling);
        if (isScrolling && pos.x == MovableArea.CENTER_X)
        {
            // 画面スクロール発生時に画面の右側にいる
            if (groundPos.x > MovableArea.CENTER_X)
            {
                if (move.x < 0f)
                {
                    // 左向きで左に移動
                    pos.x = groundPos.x + move.x;
                }
                else if (move.x <= groundPos.x)
                {
                    // 右向きで左に移動
                    pos.x = groundPos.x - move.x;
                }
            }
        }
        return pos;
    }

    private Vector3 Move_Jump(Vector3 direction)
    {
        if (restHp <= 0 || IsHurting())
        {
            // ダメージを受けたら落下する
            direction = Vector3.zero;
        }
        Vector3 move = speed / 2 * direction * Time.deltaTime;
        groundPos = CalcGroundPosition(move);
        transform.position = new Vector3(groundPos.x, transform.position.y, groundPos.z);
        jumpPower -= gravity * Time.deltaTime;
        transform.position += new Vector3(0f, jumpPower * Time.deltaTime, 0f);
        if (jumpPower < 0)
        {
            float diffY = transform.localPosition.y - groundPos.y;
            if (diffY <= 0f)
            {
                transform.localPosition += new Vector3(0f, -diffY, 0f);
                isJumping = false;
                animator.SetBool("jump", false);
            }
        }
        return move;
    }

    private Vector3 Move_SpecialAttack(Vector3 direction)
    {
        Vector3 move = specialAttackMoveSpeed * direction * Time.deltaTime;
        groundPos = CalcGroundPosition(move);
        transform.position = groundPos;
        return move;
    }

    public void UpdatePosition(Vector3 distance)
    {
        groundPos += distance;
        groundPos = MovableArea.Adjust(groundPos, isScrolling);
        transform.position = groundPos;
    }

    public void Attack()
    {
        if (IsAttacking())
        {
            return;
        }
        if (IsHurting())
        {
            return;
        }
        DateTime now = DateTime.Now;
        // 一定時間攻撃が無ければコンボをリセット
        if(now.Subtract(lastAttackTime).TotalMilliseconds > comboCountTimeLimitMilliSec)
        {
            comboCount = 0;
        }
        lastAttackTime = now;
        animator.SetFloat("speed", 0f);
        animator.SetTrigger("attack");
        if (!isJumping)
        {
            isHit = false;
            animator.SetInteger("attackType", (int)AttackType.Normal);
            animator.SetInteger("comboCount", comboCount);
            if(comboCount == COMBO_COUNT_STAN)
            {
                comboCount = 0;
            }
        }
        else
        {
            animator.SetInteger("attackType", (int)AttackType.Jump);
        }
    }

    public void SpecialAttack()
    {
        if (IsAttacking())
        {
            return;
        }
        if (IsHurting())
        {
            return;
        }
        if (restHp <= 10)
        {
            return;
        }
        animator.SetFloat("speed", 0f);
        animator.SetTrigger("attack");
        animator.SetInteger("attackType", (int)AttackType.Special);
        comboCount = 0;
        // 体力を消費して発動
        ChangeHp(-10);
    }

    private bool IsAttacking()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName("normalAttack")
            || info.IsName("strongAttack")
            || info.IsName("finishAttack")
            || info.IsName("specialAttack")
            || info.IsName("jumpAttack");
    }

    private bool IsGettingItem()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName("crouch");
    }

    public void ShowNomalAttackCollider()
    {
        nomalAttackArea.gameObject.SetActive(true);
    }

    private void AttackImpl(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        int layer = obj.layer;
        if (layer == LayerMask.NameToLayer("Enemy"))
        {
            isHit = true;
            EnemyBase e = obj.GetComponent<EnemyBase>();
            e.Damage(attack, animator.GetInteger("comboCount") == COMBO_COUNT_STAN);
        }
    }

    public void UpdateHitCount()
    {
        if (isHit)
        {
            comboCount++;
        }
        else
        {
            comboCount = 0;
        }
    }

    public void HideNomalAttackCollider()
    {
        nomalAttackArea.gameObject.SetActive(false);
    }

    public void ShowSpecialAttackCollider()
    {
        specialAttackArea.gameObject.SetActive(true);
    }

    public void HideSpecialAttackCollider()
    {
        specialAttackArea.gameObject.SetActive(false);
    }

    public void ShowJumpAttackCollider()
    {
        jumpAttackArea.gameObject.SetActive(true);
    }

    public void HideJumpAttackCollider()
    {
        jumpAttackArea.gameObject.SetActive(false);
    }

    public void ShowFireball()
    {
        fireball.SetActive(true);
        fireballSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder;
    }

    public void HideFireball()
    {
        fireball.SetActive(false);
    }

    public void GetItem(GameObject item)
    {
        animator.SetTrigger("crouch");
        Recovery(item.GetComponent<RecoveryItem>().RecoveryRate);
        Destroy(item);
    }

    private void Recovery(float rate)
    {
        ChangeHp((int)(maxHp * rate));
    }

    private void ChangeHp(int change)
    {
        restHp += change;
        restHp = Math.Min(restHp, maxHp);
        playerInformation.SetRestHp(restHp);
    }

    public void Jump(Vector3 dir)
    {
        if (isJumping)
        {
            return;
        }
        isJumping = true;
        jumpDir = dir;
        // 上下方向の成分は無視したベクトルにする
        if (jumpDir.x != 0f)
        {
            jumpDir.x = dir.x > 0f ? 1f : -1f;
        }
        jumpDir.y = 0f;
        jumpPower = jump;
        animator.SetBool("jump", true);
    }

    private bool IsHurting()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("hurt");
    }

    public void Damage(int attack)
    {
        comboCount = 0;
        animator.SetTrigger("hurt");
        ChangeHp(-attack);
        if (IsAlive && restHp <= 0)
        {
            animator.SetBool("isAlive", false);
            rest--;
            playerInformation.UpdateRest(rest);
            gameManager.Rest = rest;
            if (rest > 0)
            {
                StartCoroutine(RevivePlayer());
            }
            else
            {
                Defeat();
                if (OnDie != null)
                {
                    OnDie();
                }
            }
        }
    }

    public void Defeat()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        SetInvincible(true);
        StartFlashing();
        yield return new WaitForSeconds(1f);
        StopFlashing();
        Destroy(gameObject);
    }

    private IEnumerator RevivePlayer()
    {
        SetInvincible(true);
        while (IsHurting())
        {
            yield return new WaitForSeconds(0.1f);
        }
        StartFlashing();
        yield return new WaitForSeconds(1f);
        restHp = maxHp;
        playerInformation.SetRestHp(MaxHp);
        yield return new WaitForSeconds(reviveInvincibleTimeSec);
        StopFlashing();
        SetInvincible(false);
        animator.SetBool("isAlive", true);
    }

    private void StartFlashing()
    {
        if (flashing != null)
        {
            return;
        }
        flashing = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        while (true)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void StopFlashing()
    {
        if (flashing == null)
        {
            return;
        }
        StopCoroutine(flashing);
        flashing = null;
        spriteRenderer.enabled = true;
    }

    public void PlaySlashAudio()
    {
        audioSource.clip = slashAudio;
        audioSource.Play();
    }

    public void PlayFireballAudio()
    {
        audioSource.clip = fireballAudio;
        audioSource.Play();
    }

    public void PlaySpecialAttackAudio()
    {
        audioSource.clip = specialAttackAudio;
        audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    public void SetInvincible(bool isInvincible)
    {
        collider2d.enabled = !isInvincible;
    }

    public void SetSpecialAttack(bool isSpecialAttack)
    {
        this.isSpecialAttack = isSpecialAttack;
    }
}
