using UnityEngine;

public class EnemyStan : StateMachineBehaviour
{
    private Transform tr;
    private EnemyBase enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;
        tr = obj.transform;
        enemy = obj.GetComponent<EnemyBase>();
        enemy.SetInvincible(true);
        enemy.SetStanRotate();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        tr.eulerAngles = Vector3.zero;
        enemy.SetInvincible(false);
        enemy.IsStan = false;
    }
}
