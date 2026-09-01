using System.Collections;
using UnityEngine;
using Item;
using TMPro;
using Guard;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI itemText;

    public GameObject gameOverPanel;
    public GameObject collectable;
    public GameObject nextDoor;

    public float timerInSecond = 120f;
    private int totalItem;
    private int itemCount = 0;

    public bool startOnAwake = true;
    public bool isPaused = false;

    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        totalItem = collectable.transform.childCount;

        itemText.text = $"{itemCount}/{totalItem}";

        if (startOnAwake)
            StartCoroutine(Countdown());
    }

    private void OnEnable()
    {
        ItemCollectible.OnInteractObject += AddItem;
        AIGuard.CatchingTrigger += GameOver;
        StartStage.OnStartStage += StartTime;

    }
    private void OnDisable()
    {
        ItemCollectible.OnInteractObject -= AddItem;
        AIGuard.CatchingTrigger -= GameOver;
        StartStage.OnStartStage -= StartTime;
    }

    public void StartTime()
    {
        //Debug.Log("Signal Emited");
        if (!startOnAwake)
        {
            //Debug.Log("Timer start");
            StartCoroutine(Countdown());
        }
    }

    void AddItem()
    {
        itemCount++;

        itemText.text = $"{itemCount}/{totalItem}";
    }

    IEnumerator Countdown()
    {
        float remainingTime = timerInSecond;

        while (remainingTime > 0)
        {
            if (isPaused)
            {
                Debug.Log("Paused");
                yield return new WaitUntil(() => !isPaused);
            }

            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            nextDoor.SetActive(itemCount >= totalItem);

            string minText = minutes > 9 ? $"{minutes}" : $"0{minutes}";
            string secText = seconds > 9 ? $"{seconds}" : $"0{seconds}";
            timerText.text = $"{minText}:{secText}";

            remainingTime -= Time.deltaTime;

            yield return null;
        }

        GameOver();
    }

    void GameOver()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        Destroy(player);
        gameOverPanel.SetActive(true);
    }
}
