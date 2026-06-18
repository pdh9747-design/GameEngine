using UnityEngine;

public class CarMoveLeft : MonoBehaviour
{
    public float speed = 10f;
    public float resetX = -50f;
    public float endX = 50f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        if (transform.position.x > endX)
        {
            Vector3 pos = transform.position;
            pos.x = resetX;
            transform.position = pos;
        }
    }
}