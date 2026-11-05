using UnityEngine;

public class CarMoveLeft : MonoBehaviour
{
    public float speed = 10f;
    public float resetX = -50f;
    public float endX = 50f;

    // 모든 자동차가 공유하는 속도 배율
    public static float speedMultiplier = 1f;

    private Vector3 startPosition;

    void Start()
    {
        // 게임 시작 시 자동차의 원래 위치 저장
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(
            Vector3.forward * speed * speedMultiplier * Time.deltaTime,
            Space.Self
        );

        if (transform.position.x > endX)
        {
            Vector3 pos = transform.position;
            pos.x = resetX;
            transform.position = pos;
        }
    }

    // 다시하기 버튼에서 호출할 초기화 함수
    public void ResetCarPosition()
    {
        transform.position = startPosition;
    }
}