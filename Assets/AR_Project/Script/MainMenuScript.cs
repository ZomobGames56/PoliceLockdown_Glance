using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class MainMenuScript : MonoBehaviour
{
    private int price = 0;
    private int carInt = 0 , coin = 0;
    private static bool gameLoaded = false;
    private float maxHealth = 250, maxFuel = 150, maxSpeed = 100, maxHandling = 50;
    
    public GameData gameData;
    public SaveStateScript saveStateScript;
    public GameObject exitPanel, pauseScreen , infinitePauseScreen;
    
    [SerializeField] private GameObject infiniteCanvas;
    [SerializeField] private GameObject vehicleHandler;
    [SerializeField] private Button selectButton , buyButton;
    [SerializeField] private TextMeshProUGUI coinText , priceText;
    [SerializeField] private List<GameObject> carPrefab = new List<GameObject>();
    [SerializeField] private List<DetailScript> details = new List<DetailScript>();
    private void Start()
    {
        AudioManager.Instance.PlaySound();
        pauseScreen.SetActive(false);
        infinitePauseScreen.SetActive(false);
        coin = saveStateScript.GetCoin();
        carInt = saveStateScript.GetCarValue();
        infiniteCanvas.SetActive(false);
        Application.targetFrameRate = 144;
        foreach(GameObject car in carPrefab)
        {
            car.GetComponent<CarScript>().SetInteraction(false);
        }
        UpdatePrice();
        SetCar(carInt);
    }
    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            ExitButton();
        }
    }
    public void NextCar()
    {
        vehicleHandler.SetActive(false);
        vehicleHandler.SetActive(true);
        if (carInt < carPrefab.Count-1)
        {
            carInt++;
        }
        else
        {
            carInt = 0;
        }
        SetCar(carInt);
    }
    public void PreviousCar()
    {
        vehicleHandler.SetActive(false);
        vehicleHandler.SetActive(true);
        if(carInt > 0)
        {
            carInt--;
        }
        else
        {
            carInt = carPrefab.Count - 1;
        }
        SetCar(carInt);

    }
    public void StartLevel()
    {
        gameData.coin = coin;
        gameData.carValue = carInt;
        gameData.highScore = saveStateScript.GetHighScore();
        if (gameLoaded == false)
        {
            Application.ExternalCall("GameStart");
            gameLoaded = true;
        }
        else
        {
            Application.ExternalCall("replayGame");
        }
        infiniteCanvas.SetActive(true);
    }
    public void SelectCar()
    {
        saveStateScript.SetCarValue(carInt);
        saveStateScript.SaveState();
        selectButton.interactable = false;
    }
    private void SetCar(int carValue)
    {
        if (saveStateScript.GetCarValue() == carValue)
        {
            selectButton.interactable = false;
        }
        else
        {
            selectButton.interactable = true;
        }
        foreach (GameObject car in carPrefab)
        {
            car.SetActive(false);
        }
        carPrefab[carValue].SetActive(true);
        SetDetails(carPrefab[carValue].GetComponent<CarScript>().GetCarSO());
    }
    public void replayGameEvent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void SetDetails(CarObject car_SO)
    {
        if(car_SO.isAvailable)
        {
            selectButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            priceText.text = car_SO.Price.ToString();
            buyButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
            if (carPrefab[carInt].GetComponent<CarScript>().GetCarSO().Price <= coin)
            {
                buyButton.interactable = true;
            }
            else
            {
                buyButton.interactable = false;
            }
        }
        details[0].SetDetails("Health", car_SO.Health.ToString(), car_SO.Health / maxHealth);
        details[1].SetDetails("Fuel", car_SO.FuelCapacity.ToString(), car_SO.FuelCapacity / maxFuel);
        details[2].SetDetails("Speed", car_SO.Speed.ToString(), car_SO.Speed / maxSpeed);
        details[3].SetDetails("Handling", car_SO.Handling.ToString(), car_SO.Handling / maxHandling);
    }
    public void BuyCar()
    {
        if(carPrefab[carInt].GetComponent<CarScript>().GetCarSO().Price <= coin)
        {
            coin -= carPrefab[carInt].GetComponent<CarScript>().GetCarSO().Price;
            carPrefab[carInt].GetComponent<CarScript>().GetCarSO().isAvailable = true;
            UpdatePrice();
            SetDetails(carPrefab[carInt].GetComponent<CarScript>().GetCarSO());
            SetCar(carInt);
        }
        saveStateScript.SaveState();
    }
    private void UpdatePrice()
    {
        coinText.text = coin.ToString();
        saveStateScript.SetCoin(coin);
        saveStateScript.SaveState();
    }
    public void ExitButton()
    {
        exitPanel.SetActive(true);
    }
    public void PauseGame()
    {
        pauseScreen.SetActive(true);
        infinitePauseScreen.SetActive(true);
        AudioManager.Instance.StopAudio();
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        pauseScreen.SetActive(false);
        infinitePauseScreen.SetActive(false);
        AudioManager.Instance.PlaySound();
        Time.timeScale = 1;
    }
}
