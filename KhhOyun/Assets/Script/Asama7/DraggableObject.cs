using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class DraggableObject : MonoBehaviour
{
    [Header("Hedef Ayarlarý")]
    [SerializeField] private string targetTag = "Basket";
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 startPosition;

    private bool isOverTarget = false;
    private GameObject currentTarget;

    void Start()
    {
        startPosition = transform.position;
    }

    
    void Update()
    {
        if (Pointer.current == null) return;
        
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = transform.position.z;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if(hit.collider!=null&& hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - worldPosition;
            }
        }

        if (isDragging)
        {
            transform.position = new Vector3(worldPosition.x + offset.x, worldPosition.y + offset.y, startPosition.z);
        }

        if (Pointer.current.press.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            if (isOverTarget && currentTarget != null)
            {
                currentTarget.SendMessage("ObjectDropped", gameObject, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                transform.position = startPosition;
            }
        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            isOverTarget = true;
            currentTarget = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag)) 
        {
            isOverTarget = false;
            currentTarget = null;
        }
    }
    public void ResetPosition()
    {
        transform.position = startPosition;
    }
}
