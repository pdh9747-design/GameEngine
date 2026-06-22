using UnityEngine;
using TMPro;

public class player : MonoBehaviour
{
    public float moveSpeed = 5f;

    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI timerText;

    private int life = 3;
    private float timeLeft = 60f;

    private Vector3 startPos;

    void Start()
    {
        Time.timeScale = 1f;

        startPos = transform.position;

        lifeText.text = "목숨 : " + life;
        timerText.text = "시간 : 60초";
    }

    void Update()
    {
        // 플레이어 이동
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(-x, 0f, -z);

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        // 타이머
        timeLeft -= Time.deltaTime;

        if (timeLeft < 0)
            timeLeft = 0;

        timerText.text = "시간 : " + Mathf.Ceil(timeLeft) + "초";

        // 시간 종료
        if (timeLeft <= 0)
        {
            timerText.text = "클리어!";
            Time.timeScale = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            life--;

            if (life <= 0)
            {
                life = 0;
                lifeText.text = "게임 오버";

                Debug.Log("게임 오버");

                Time.timeScale = 0f;
                return;
            }

            lifeText.text = "목숨 : " + life;

          
        }
    }
}