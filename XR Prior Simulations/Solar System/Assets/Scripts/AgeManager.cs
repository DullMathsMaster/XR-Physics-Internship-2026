using UnityEngine;

public enum AgeGroup
{
    KS2,
    KS3,
    KS4,
    KS5,
}

public class AgeManager : MonoBehaviour
{
    public static AgeManager Instance;

    public AgeGroup CurrentAge = AgeGroup.Age7to11;

    private void Awake()
    {
        Instance = this;
    }
}