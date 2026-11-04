using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    [Header("Player")]
    public float moveSpeed = 5f;
    public Vector3 respawnPosition;

    [Header("UI")]
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI timerText;
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public GameObject gameClearPanel;

    [Header("Cars")]
    public CarMoveLeft[] leftCars;
    public CarMoveRight[] rightCars;

    private CharacterController controller;

    private int life = 3;
    private float timer = 60f;

    private bool isStarted = false;
    private bool isInvincible = false;
    private bool isGameOver = false;
    private bool isClear = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 플레이어 시작 위치 저장
        respawnPosition = transform.position;

        // 게임 시작 전 시간 정지
        Time.timeScale = 0f;

        // 시작 화면 표시
        if (startPanel != null)
            startPanel.SetActive(true);

        // 게임오버 화면 숨기기
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 게임클리어 화면 숨기기
        if (gameClearPanel != null)
            gameClearPanel.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        // 시작 전, 게임오버, 클리어 상태에서는 진행하지 않음
        if (!isStarted || isGameOver || isClear)
            return;

        // 새 Input System 키보드 입력
        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                x = -1f;

            if (Keyboard.current.dKey.isPressed)
                x = 1f;

            if (Keyboard.current.wKey.isPressed)
                z = 1f;

            if (Keyboard.current.sKey.isPressed)
                z = -1f;
        }

        // 기존 이동 방향 유지
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
        if (!isStarted || isGameOver || isClear)
            return;

        // 차량에 충돌했고 무적 상태가 아닐 때
        if (other.CompareTag("Car") && !isInvincible)
        {
            // 목숨 감소
            life--;

            lifeText.text = "목숨 : " + life;

            Debug.Log("차량 충돌");

            // 충돌해도 플레이어 위치는 유지
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

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    void GameClear()
    {
        isClear = true;

        Debug.Log("게임 클리어");

        if (gameClearPanel != null)
            gameClearPanel.SetActive(true);

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

    // START 버튼
    public void StartGame()
    {
        isStarted = true;

        if (startPanel != null)
            startPanel.SetActive(false);

        Time.timeScale = 1f;

        Debug.Log("게임 시작");
    }

    // RESTART 버튼
    public void RestartGame()
    {
        Time.timeScale = 1f;

        CancelInvoke(nameof(ResetDamage));

        life = 3;
        timer = 60f;

        isStarted = true;
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

        UpdateUI();

        if (startPanel != null)
            startPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameClearPanel != null)
            gameClearPanel.SetActive(false);

        Debug.Log("게임 다시 시작");
    }

    // QUIT 버튼
    public void QuitGame()
    {
        Debug.Log("게임 종료");

        Application.Quit();
    }
}