using System;
using UnityEngine;

public class Puzzle_Elevator : MonoBehaviour
{
    public enum CalculationProccess
    {
        Mass,
        Anchor
    }
    public CalculationProccess ElevatorWork;
    public bool State;
    public float Side_1_middle_y;
    public float Side_1_Top_y;
    public float Side_1_Bottom_y;
    public float Side_2_middle_y;
    public float Side_2_Top_y;
    public float Side_2_Bottom_y;
    [SerializeReference] public Puzzle_Elevator_Side Side_1;
    [SerializeReference] public Puzzle_Elevator_Side Side_2;
    public float SidesMovementSpeed = 0.5f;
    // Update is called once per frame
    private void Update()
    {
        float Delta = SidesMovementSpeed * Time.deltaTime;
        Vector3 Side_1_Loc = Side_1.movingObject.transform.position;
        Vector3 Side_2_Loc = Side_2.movingObject.transform.position;
        switch(ElevatorWork)
        {
            case CalculationProccess.Mass:
                float DifferenceFromSide_1 = Side_1.MassOfObjects - Side_2.MassOfObjects;
                bool IsEqual = Mathf.Abs(DifferenceFromSide_1) <= 2;
                if (IsEqual)
                {
                    Side_1_Loc.y = Mathf.MoveTowards(Side_1_Loc.y, Side_1_middle_y, Delta);
                    Side_2_Loc.y = Mathf.MoveTowards(Side_2_Loc.y, Side_2_middle_y, Delta);
                }
                else
                {
                    Side_1_Loc.y = Mathf.MoveTowards(Side_1_Loc.y, DifferenceFromSide_1 < 0 ? Side_1_Top_y : Side_1_Bottom_y, Delta);
                    Side_2_Loc.y = Mathf.MoveTowards(Side_2_Loc.y, DifferenceFromSide_1 > 0 ? Side_2_Top_y : Side_2_Bottom_y, Delta);
                }
                break;
            case CalculationProccess.Anchor:
                Side_1_Loc.y = Mathf.MoveTowards(Side_1_Loc.y, !State ? Side_1_Top_y : Side_1_Bottom_y, Delta);
                Side_2_Loc.y = Mathf.MoveTowards(Side_2_Loc.y, State ? Side_2_Top_y : Side_2_Bottom_y, Delta);
                break;
        }
        
        Side_1.movingObject.transform.position = Side_1_Loc;
        Side_2.movingObject.transform.position = Side_2_Loc;
    }
}
