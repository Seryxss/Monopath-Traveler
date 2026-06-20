using UnityEngine;

public class Slime : EnemyBase
{
    [SerializeField] private AudioClip _someSound;

    // private void Start()
    // {
    //     AudioSystem.Instance.PlaySound(_someSound);
    // }

    //public override void ExecuteMove()
    //{
    //    //Perform specific warrior animation, damage, move etc
    //    base.ExecuteMove();//To clean up the move
    //}
}
