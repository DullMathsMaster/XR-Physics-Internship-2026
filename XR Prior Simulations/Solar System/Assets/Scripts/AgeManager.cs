using UnityEngine;

public enum AgeGroup
{
    KS2,
    KS3,
    KS4,
    KS5
}

public class AgeManager : MonoBehaviour
{
    public static AgeManager Instance;

    public AgeGroup CurrentAge = AgeGroup.KS2;

    private void Awake()
    {
        Instance = this;
    }

    public void SetAge(int value)
    {
        CurrentAge = (AgeGroup)value;

        Debug.Log("Current age: " + CurrentAge);
    }
}