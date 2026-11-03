using UnityEngine;
using TMPro;

public class player : MonoBehaviour
{
    [Header("Player")]
    public float moveSpeed = 5f;
    public Vector3 respawnPosition;

    [Header("UI")]
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;
    public GameObject gameClearPanel;

    [Header("Cars")]
    public CarMoveLeft[] leftCars;
    public CarMoveRight[] rightCars;

    private CharacterController controller;

    private int life = 3;
    private float timer = 60f;

    private bool isInvincible = false;
    private bool isGameOver = false;
    private bool isClear = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 플레이어 시작 위치 저장
        respawnPosition = transform.position;

        // 게임 시간 정상화
        Time.timeScale = 1f;

        // 게임오버 패널 숨기기
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 게임클리어 패널 숨기기
        if (gameClearPanel != null)
            gameClearPanel.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        // 게임오버 또는 클리어 상태면 움직이지 않음
        if (isGameOver || isClear)
            return;

        // 플레이어 이동
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(-x, 0f, -z);

        controller.Move(move * moveSpeed * Time.deltaTime);

        // 타이머 감소
        timer -= Time.deltaTime;

        if (timer < 0f)
            timer = 0f;

        timerText.text = "시간 : " + Mathf.Ceil(timer) + "초";

        // 시간이 끝나면 게임 클리어
        if (timer <= 0f)
            GameClear();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 게임오버 또는 클리어 상태면 충돌 처리 안 함
        if (isGameOver || isClear)
            return;

        // 차량에 충돌했고 무적 상태가 아닐 때
        if (other.CompareTag("Car") && !isInvincible)
        {
            // 목숨 감소
            life--;

            lifeText.text = "목숨 : " + life;

            Debug.Log("차량 충돌");

            // 충돌해도 플레이어 위치는 이동하지 않음
            isInvincible = true;

            // 기존 무적 해제 예약 취소
            CancelInvoke(nameof(ResetDamage));

            // 3초 후 무적 해제
            Invoke(nameof(ResetDamage), 3f);

            // 목숨이 0이면 게임오버
            if (life <= 0)
                GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;

        Debug.Log("게임 오버");

        // 게임오버 패널 표시
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // 게임 정지
        Time.timeScale = 0f;
    }

    void GameClear()
    {
        isClear = true;

        Debug.Log("게임 클리어");

        // 게임클리어 패널 표시
        if (gameClearPanel != null)
            gameClearPanel.SetActive(true);

        // 게임 정지
        Time.timeScale = 0f;
    }

    void ResetDamage()
    {
        isInvincible = false;
    }

    void UpdateUI()
    {
        lifeText.text = "목숨 : " + life;
        timerText.text = "시간 : " + Mathf.Ceil(timer) + "초";
    }

    public void RestartGame()
    {
        // 게임 시간 정상화
        Time.timeScale = 1f;

        // 무적 예약 취소
        CancelInvoke(nameof(ResetDamage));

        // 상태 초기화
        life = 3;
        timer = 60f;

        isInvincible = false;
        isGameOver = false;
        isClear = false;

        // 플레이어 시작 위치로 이동
        controller.enabled = false;
        transform.position = respawnPosition;
        controller.enabled = true;

        // 차량 위치 초기화
        foreach (CarMoveLeft car in leftCars)
        {
            if (car != null)
                car.ResetCarPosition();
        }

        foreach (CarMoveRight car in rightCars)
        {
            if (car != null)
                car.ResetCarPosition();
        }

        // UI 갱신
        UpdateUI();

        // 게임오버 패널 숨기기
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 게임클리어 패널 숨기기
        if (gameClearPanel != null)
            gameClearPanel.SetActive(false);

        Debug.Log("게임 다시 시작");
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");

        Application.Quit();
    }
}