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
    public TextMeshProUGUI speedUpText;

    public GameObject startPanel;
    public GameObject gameOverPanel;
    public GameObject gameClearPanel;

    [Header("Cars")]
    public CarMoveLeft[] leftCars;
    public CarMoveRight[] rightCars;

    [Header("Hit Effect")]
    public GameObject hitEffectPrefab;

    [Header("Car Difficulty")]
    public float speedIncreaseInterval = 15f;
    public float speedIncreaseAmount = 0.2f;
    public float maxSpeedMultiplier = 2f;

    private CharacterController controller;
    private Renderer[] playerRenderers;

    private int life = 3;
    private float timer = 60f;
    private float speedIncreaseTimer = 0f;

    private bool isStarted = false;
    private bool isInvincible = false;
    private bool isGameOver = false;
    private bool isClear = false;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerRenderers = GetComponentsInChildren<Renderer>();

        respawnPosition = transform.position;

        // 자동차 속도 초기화
        CarMoveLeft.speedMultiplier = 1f;
        CarMoveRight.speedMultiplier = 1f;

        Time.timeScale = 0f;

        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(false);

        // 속도 증가 문구 숨기기
        if (speedUpText != null)
        {
            speedUpText.gameObject.SetActive(false);
        }

        UpdateUI();
    }


    void Update()
    {
        if (!isStarted || isGameOver || isClear)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            float x = 0f;
            float z = 0f;

            if (keyboard.aKey.isPressed)
            {
                x = -1f;
            }

            if (keyboard.dKey.isPressed)
            {
                x = 1f;
            }

            if (keyboard.wKey.isPressed)
            {
                z = 1f;
            }

            if (keyboard.sKey.isPressed)
            {
                z = -1f;
            }

            Vector3 move = new Vector3(-x, 0f, -z);

            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        // 자동차 속도 증가 타이머
        speedIncreaseTimer += Time.deltaTime;

        if (speedIncreaseTimer >= speedIncreaseInterval)
        {
            speedIncreaseTimer = 0f;

            // 실제로 속도가 증가하는 경우에만 문구 표시
            bool canIncreaseSpeed =
                CarMoveLeft.speedMultiplier < maxSpeedMultiplier ||
                CarMoveRight.speedMultiplier < maxSpeedMultiplier;

            CarMoveLeft.speedMultiplier = Mathf.Min(
                CarMoveLeft.speedMultiplier + speedIncreaseAmount,
                maxSpeedMultiplier
            );

            CarMoveRight.speedMultiplier = Mathf.Min(
                CarMoveRight.speedMultiplier + speedIncreaseAmount,
                maxSpeedMultiplier
            );

            if (canIncreaseSpeed)
            {
                ShowSpeedUpText();
            }
        }

        // 게임 타이머
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = 0f;
            GameClear();
        }

        UpdateUI();
    }


    // 속도 증가 문구 표시
    void ShowSpeedUpText()
    {
        if (speedUpText == null)
        {
            return;
        }

        speedUpText.gameObject.SetActive(true);

        // 문구가 이미 표시 중이면 숨기는 시간 갱신
        CancelInvoke(nameof(HideSpeedUpText));
        Invoke(nameof(HideSpeedUpText), 2f);
    }


    // 속도 증가 문구 숨기기
    void HideSpeedUpText()
    {
        if (speedUpText != null)
        {
            speedUpText.gameObject.SetActive(false);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (!isStarted || isGameOver || isClear)
        {
            return;
        }

        if (other.CompareTag("Car") && !isInvincible)
        {
            // 충돌 이펙트 생성
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(
                    hitEffectPrefab,
                    transform.position + Vector3.up,
                    Quaternion.identity
                );

                // 파티클 재생이 끝나면 이펙트 오브젝트 삭제
                ParticleSystem ps =
                    effect.GetComponentInChildren<ParticleSystem>();

                if (ps != null)
                {
                    var main = ps.main;

                    Destroy(
                        effect,
                        main.duration + main.startLifetime.constantMax
                    );
                }
                else
                {
                    Destroy(effect, 3f);
                }
            }

            // 목숨 감소
            life--;

            UpdateUI();

            Debug.Log("목숨 감소! 남은 목숨: " + life);

            // 무적 상태 시작
            isInvincible = true;

            CancelInvoke(nameof(ResetDamage));
            CancelInvoke(nameof(BlinkPlayer));

            InvokeRepeating(nameof(BlinkPlayer), 0f, 0.2f);
            Invoke(nameof(ResetDamage), 3f);

            if (life <= 0)
            {
                GameOver();
            }
        }
    }


    // 플레이어 깜빡임
    void BlinkPlayer()
    {
        foreach (Renderer playerRenderer in playerRenderers)
        {
            playerRenderer.enabled = !playerRenderer.enabled;
        }
    }


    // 무적 상태 종료
    void ResetDamage()
    {
        isInvincible = false;

        CancelInvoke(nameof(BlinkPlayer));

        foreach (Renderer playerRenderer in playerRenderers)
        {
            playerRenderer.enabled = true;
        }
    }


    // 게임 오버
    void GameOver()
    {
        isGameOver = true;

        HideSpeedUpText();

        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }


    // 게임 클리어
    void GameClear()
    {
        isClear = true;

        HideSpeedUpText();

        gameClearPanel.SetActive(true);

        Time.timeScale = 0f;
    }


    // UI 업데이트
    void UpdateUI()
    {
        lifeText.text = "Life : " + life;
        timerText.text = "Time : " + Mathf.CeilToInt(timer);
    }


    // 게임 시작
    public void StartGame()
    {
        isStarted = true;

        Time.timeScale = 1f;

        startPanel.SetActive(false);
    }


    // 게임 재시작
    public void RestartGame()
    {
        Time.timeScale = 1f;

        CancelInvoke(nameof(ResetDamage));
        CancelInvoke(nameof(BlinkPlayer));
        CancelInvoke(nameof(HideSpeedUpText));

        isStarted = true;
        isInvincible = false;
        isGameOver = false;
        isClear = false;

        life = 3;
        timer = 60f;

        // 자동차 속도 초기화
        speedIncreaseTimer = 0f;

        CarMoveLeft.speedMultiplier = 1f;
        CarMoveRight.speedMultiplier = 1f;

        // 속도 증가 문구 숨기기
        HideSpeedUpText();

        // 플레이어 위치 초기화
        controller.enabled = false;
        transform.position = respawnPosition;
        controller.enabled = true;

        // 플레이어 렌더러 다시 표시
        foreach (Renderer playerRenderer in playerRenderers)
        {
            playerRenderer.enabled = true;
        }

        // 자동차 위치 초기화
        foreach (CarMoveLeft car in leftCars)
        {
            car.ResetCarPosition();
        }

        foreach (CarMoveRight car in rightCars)
        {
            car.ResetCarPosition();
        }

        // UI 초기화
        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(false);

        UpdateUI();
    }


    // 게임 종료
    public void QuitGame()
    {
        Application.Quit();
    }
}