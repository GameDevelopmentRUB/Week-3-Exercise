using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    Vector3 direction;

    SpriteRenderer sr;
    Camera camMain;

    private void Awake() => sr = GetComponent<SpriteRenderer>();

    void Start()
    {
        // Cache camera, because otherwise it'll look for the tag 'MainCamera' every time
        camMain = Camera.main;

        // Time-related functions
        Invoke(nameof(SwitchDirection), 3f);
        InvokeRepeating(nameof(SwitchDirection), 0, 3f);
        StartCoroutine(SwitchDirectionCoroutine());

        // HSV = Hue (base color), Saturation (how much of that color), Value (black <-> white)
        sr.color = Random.ColorHSV(0f, 1f, 0.75f, 1f, 1f, 1f);   
    }

    void Update()
    {
        // Moves in 'direction'
        transform.position += speed * Time.deltaTime * direction;

        // Alternatively this would move into facing direction
        transform.position += speed * Time.deltaTime * transform.right;

        // This calculates the edges of the screen in relative coordinates ('Viewport')
        Vector2 bottomLeft = camMain.ViewportToWorldPoint(Vector3.zero);
        Vector2 topRight = camMain.ViewportToWorldPoint(Vector3.one);

        // If the object leaves the screen, destroy it
        if (transform.position.x < bottomLeft.x || transform.position.x > topRight.x ||
            transform.position.y < bottomLeft.y || transform.position.y > topRight.y)
            Destroy(gameObject);
    }

    // This is called when no camera is rendering the object - that includes the Scene camera
    void OnBecameInvisible() => Destroy(gameObject);

    void SwitchDirection()
    {
        // Generates a random direction with equal chance for every direction
        direction = Random.insideUnitCircle.normalized;

        // Flip sprite if moving to the left
        sr.flipY = direction.x < 0;

        // Atan2 calculates the angle (in radians) out of a directional vector 
        // Atan would only go from -90° to 90°, but Atan2 covers every direction from -180° to 180°
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Coroutines can be used for complex behaviour over time. 'yield return' suspends the method
    IEnumerator SwitchDirectionCoroutine()
    {
        while (true)
        {
            SwitchDirection();
            yield return new WaitForSeconds(1f);
            SwitchDirection();
            yield return new WaitForSeconds(3f);
            SwitchDirection();
            yield return new WaitForSeconds(0.5f);
        }
    }
}