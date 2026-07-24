using UnityEngine;

public class BicycleIdleMotion : MonoBehaviour
{
    [Header("Hareket Edilecek UI Objesi")]
    public RectTransform target;

    [Header("Yukarý Aþaðý Hareket")]
    [SerializeField]
    private float verticalAmount = 5f;

    [SerializeField]
    private float verticalSpeed = 2.5f;

    [Header("Hafif Ýleri Geri Hareket")]
    [SerializeField]
    private float horizontalAmount = 2f;

    [Header("Hafif Eðilme")]
    [SerializeField]
    private float rotationAmount = 1.2f;

    private Vector2 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        if (target == null)
        {
            target = GetComponent<RectTransform>();
        }
    }

    private void OnEnable()
    {
        if (target == null)
        {
            return;
        }

        startPosition = target.anchoredPosition;
        startRotation = target.localRotation;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        float wave = Mathf.Sin(
            Time.time * verticalSpeed
        );

        float secondWave = Mathf.Sin(
            Time.time * verticalSpeed * 0.65f
        );

        Vector2 movement = new Vector2(
            secondWave * horizontalAmount,
            wave * verticalAmount
        );

        target.anchoredPosition =
            startPosition + movement;

        target.localRotation =
            startRotation *
            Quaternion.Euler(
                0f,
                0f,
                secondWave * rotationAmount
            );
    }

    private void OnDisable()
    {
        if (target == null)
        {
            return;
        }

        target.anchoredPosition = startPosition;
        target.localRotation = startRotation;
    }
}