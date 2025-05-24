using System.Collections;
using UnityEngine;

public class Puzzle_Elevator_Anchor : MonoBehaviour
{
    public Puzzle_Elevator LinkedElevator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool CanChangeState = true;
    public float CooldownTimer = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!CanChangeState || !collision.collider.gameObject.TryGetComponent<PhysicsObjectBasic>(out _)) return;
        LinkedElevator.ChangeState();
        StartCoroutine(StateChangeCooldownTimer());
    }

    private IEnumerator StateChangeCooldownTimer()
    {
        CanChangeState = false;
        yield return new WaitForSeconds(CooldownTimer);
        CanChangeState = true;
    }
}
