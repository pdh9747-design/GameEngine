using UnityEngine;
using TMPro;

public class player : MonoBehaviour
{
    public float moveSpeed = 5f;

    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI timerText;

    private CharacterController controller;

    private int life = 3;
    private float timer = 60f;

    private bool isInvincible = false;

    public Vector3 respawnPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        respawnPosition = transform.position;

        lifeText.text = "목숨 : " + life;
        timerText.text = "시간 : " + Mathf.Ceil(timer) + "초";
    }

    void Update()
    {
        // 이동
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(-x, 0f, -z);

        controller.Move(move * moveSpeed * Time.deltaTime);

        // 타이머
        timer -= Time.deltaTime;

        if (timer < 0)
            timer = 0;

        timerText.text = "시간 : " + Mathf.Ceil(timer) + "초";

        // 클리어
        if (timer <= 0)
        {
            Debug.Log("게임 클리어");
            Time.timeScale = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car") && !isInvincible)
        {
            life--;

            lifeText.text = "목숨 : " + life;

            Debug.Log("차량 충돌");

            // 시작 위치로 이동
            transform.position = respawnPosition;

            isInvincible = true;

            // 3초 무적
            Invoke(nameof(ResetDamage), 3f);

            if (life <= 0)
            {
                Debug.Log("게임 오버");
                Time.timeScale = 0f;
            }
        }
    }

    void ResetDamage()
    {
        isInvincible = false;
    }
}