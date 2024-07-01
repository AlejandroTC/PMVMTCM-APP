using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeGesture : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 currentPosition;
    private Vector2 endTouchPosition;
    private bool stopTouch = false;
    private bool swiping = false; // Bandera para detectar si se está realizando un swipe

    public float swipeRange = 50f;  // Distancia mínima para considerar un deslizamiento
    public float tapRange = 10f;    // Distancia máxima para considerar un toque como tap
    public int sceneIndex = 0;

    void Update()
    {
        Swipe();
    }

    void Swipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    swiping = true; // Comienza el swipe
                    break;

                case TouchPhase.Moved:
                    currentPosition = touch.position;
                    Vector2 distance = currentPosition - startTouchPosition;

                    if (!stopTouch && swiping)
                    {
                        if (distance.x < -swipeRange)
                        {
                            Debug.Log("Left Swipe");
                            // Acción para swipe a la izquierda (puede ser regresar a una escena anterior)
                            PreviousScene();
                            stopTouch = true;
                        }
                        else if (distance.x > swipeRange)
                        {
                            Debug.Log("Right Swipe");
                            // Acción para swipe a la derecha
                            stopTouch = true;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    stopTouch = false;
                    endTouchPosition = touch.position;
                    Vector2 endDistance = endTouchPosition - startTouchPosition;

                    if (Mathf.Abs(endDistance.x) < tapRange && Mathf.Abs(endDistance.y) < tapRange)
                    {
                        Debug.Log("Tap");
                        // Acción para tap
                    }

                    swiping = false; // Finaliza el swipe
                    break;
            }
        }
    }

    void PreviousScene()
    {
        // Verifica si hay una escena anterior en la pila del SceneManager
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogWarning("No previous scene to load.");
        }
    }
}
