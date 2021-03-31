using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesMesh;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private MapTreadmill mapTreadmill;
    [SerializeField] private PlayerController player;
    [Space]
    [SerializeField] private GameObject gameOverPanel;
    private void Start()
    {
        gameOverPanel.SetActive(false);

        speedSlider.minValue = mapTreadmill.SpeedMinValue;
        speedSlider.maxValue = mapTreadmill.SpeedMaxValue;
        speedSlider.value = speedSlider.minValue;

        mapTreadmill.SetSpeed(speedSlider.value);
        speedSlider.onValueChanged.AddListener(SliderValueChangedHandler);

        UpdateLivesCounter();
        player.onTakingDamage += UpdateLivesCounter;
        player.onGameOver += GameOverHandler;
    }

    private void SliderValueChangedHandler(float sliderValue)
    {
        mapTreadmill.SetSpeed(sliderValue);
    }

    private void UpdateLivesCounter()
    {
        livesMesh.text = "Lives: " + player.Lives.ToString();
    }

    private void GameOverHandler()
    {
        gameOverPanel.SetActive(true);
        player.onTakingDamage -= UpdateLivesCounter;
        player.onGameOver -= GameOverHandler;
    }
}