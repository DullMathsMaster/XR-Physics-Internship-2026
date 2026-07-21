using UnityEngine;
using UnityEngine.UI;

public class CharizardController : MonoBehaviour
{
    [SerializeField] private float speed = 0.2f;
    [SerializeField] private FixedJoystick fixedJoystick;
    [SerializeField] private Animator animator;

    [SerializeField] private Slider speedSlider;

    [SerializeField] private Slider scaleSlider;

    private Vector3 originalScale;

    private Rigidbody rigidBody;
    private bool isFlying;

    private void Start()
    {
        originalScale = transform.localScale;


        // Find the sliders by tag
        if (speedSlider == null)
        {
            speedSlider = GameObject.FindWithTag("SpeedSlider").GetComponent<Slider>();
        }

        if (scaleSlider == null)
        {
            scaleSlider = GameObject.FindWithTag("ScaleSlider").GetComponent<Slider>();
        }
        

        // Set the slider for the speed
        speedSlider.minValue = 0.1f;
        speedSlider.maxValue = 5f;
        speedSlider.value = speed;

        speedSlider.onValueChanged.AddListener(SetSpeed);
        

        // Set the slider for the scale
        scaleSlider.minValue = 0.1f;
        scaleSlider.maxValue = 2f;
        scaleSlider.value = 0.27f;

        scaleSlider.onValueChanged.AddListener(SetScale);
    }

    private void OnDestroy()
    {
        if (speedSlider != null)
        {
            speedSlider.onValueChanged.RemoveListener(SetSpeed);
        }

        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.RemoveListener(SetScale);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetScale(float scaleMultiplier)
    {
        transform.localScale = originalScale * scaleMultiplier;
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        if (fixedJoystick == null)
        {
            fixedJoystick = FindFirstObjectByType<FixedJoystick>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void FixedUpdate()
    {
        float xVal = fixedJoystick.Horizontal;
        float yVal = fixedJoystick.Vertical;

        Vector3 movement = new Vector3(xVal, 0f, yVal);
        rigidBody.linearVelocity = movement * speed;

        bool isMoving = movement.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            float angle = Mathf.Atan2(xVal, yVal) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                angle,
                transform.eulerAngles.z
            );
        }

        if (isFlying != isMoving)
        {
            animator.SetBool("flyParam", isMoving);
            isFlying = isMoving;
        }
    }
}