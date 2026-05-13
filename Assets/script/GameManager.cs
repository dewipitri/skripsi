using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI itemText;

    public GameObject collectable;

    public float timerInSecond = 120f;
    private int totalItem;
    private int itemCount = 0;

    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        totalItem = collectable.transform.childCount;

        itemText.text = $"{itemCount}/{totalItem}";

        StartCoroutine(Countdown());
    }

    private void OnEnable()
    {
        Itempopup.OnInteractObject += AddItem;
    }

    private void OnDisable()
    {
        Itempopup.OnInteractObject -= AddItem;
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
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            timerText.text = $"{minutes}:{seconds}";

            remainingTime -= Time.deltaTime;

            yield return null;
        }
    }
}
