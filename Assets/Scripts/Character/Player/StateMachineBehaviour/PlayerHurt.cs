using UnityEngine;

public class PlayerHurt : StateMachineBehaviour
{
    private Player player;
   
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.gameObject.GetComponent<Player>();
        player.SetInvincible(true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("isAlive"))
        {
            player.SetInvincible(false);
        }
    }
}
