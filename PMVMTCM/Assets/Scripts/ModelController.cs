using UnityEngine;

public class ModelController : MonoBehaviour
{
    public static ModelController Instance;
    public GameObject model3D; // Asigna el modelo 3D en el inspector
    private float scaleFactor = 1.0f;
    private float rotationSpeed = 50.0f;
    private Vector2 initialTouchPos;
    private Vector2 secondTouchPos;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                float deltaY = touch.position.y - initialTouchPos.y;
                float scaleValue = deltaY / Screen.height;
                ScaleModel(scaleValue);
                initialTouchPos = touch.position;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                secondTouchPos = touch1.position;
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float deltaX = touch1.position.x - secondTouchPos.x;
                float rotationValue = deltaX / Screen.width;
                RotateModel(rotationValue);
                secondTouchPos = touch1.position;
            }
        }
    }

    void ScaleModel(float value)
    {
        scaleFactor += value;
        scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 30.0f); // Limitar el factor de escala
        model3D.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    void RotateModel(float value)
    {
        model3D.transform.Rotate(Vector3.forward, value * rotationSpeed);
    }
}
