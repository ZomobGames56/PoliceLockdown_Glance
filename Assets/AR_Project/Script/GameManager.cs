using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    private int carInt;
    private bool gameRunning;
    private float _highScore;
    private int coin , score;
    private GameObject player;
    private bool lifeUsed = false;
    public static bool gameStarted = false;
    private Vector3 offset = new Vector3(0,30,-45);

    public GameData gameData;
    public AIManager aiManager;
    public SaveStateScript saveStateScript;
    
    [SerializeField] private GameObject Camera;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private CarFadeRayCast raycastScript;
    [SerializeField] private Slider fuelSlider, healthSlider;
    [SerializeField] private TextMeshProUGUI scoreText, timer;
    [SerializeField] private List<GameObject> cars = new List<GameObject>();
    [SerializeField] private GameObject inGameUI, gameOverUI , gamePauseUI , resumeButton;
    private void Start()
    {
        AudioManager.Instance.PlaySound();
        if(gameStarted ==false)
        {
            gameStarted = true;
        }
        carInt = gameData.carValue;
        coin = gameData.coin;
        print("Data Given to Game Manager" + carInt + "Car Int" + coin);
        Time.timeScale = 1;
        player = Instantiate(cars[carInt]);
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
        player.GetComponent<CarScript>().SetGameManager(this);
        player.GetComponent<CarScript>().enabled = false;
        Camera.transform.SetParent(player.transform);
        Camera.transform.localPosition = offset;
        Camera.transform.LookAt(player.transform);
        aiManager.GetTarget(player);
        UpdateUI();
        gameRunning = false;
        aiManager.enabled = false;
        scoreText.gameObject.SetActive(false);
        raycastScript.SetObject(player);
        SetUI(inGameUI);
        StartCoroutine(DelayFunction(3f, StartGame , timer));
    }

    private void StartGame()
    {
        player.GetComponent<CarScript>().enabled = true;
        player.GetComponent<CarScript>().SetInteraction(true);
        scoreText.gameObject.SetActive(true);
        gameRunning = true;
        aiManager.enabled = true;
        SetUI(inGameUI);
    }
    private void Update()
    {
        if (gameRunning)
        {
            _highScore += Time.deltaTime;
            score = (int)_highScore;
            scoreText.text = score.ToString();
        }

    }
    public void GameRestart()
    {
        int i = Random.Range(1, 7);
        SceneManager.LoadScene(i);
    }
    public void CoinPicked(int amount)
    {
        coin += amount;
        UpdateUI();
        SaveData();
    }
    public void SetSliders(float fuelValue , float healthValue)
    {
        fuelSlider.value = fuelValue;
        healthSlider.value = healthValue;
    }
    private void UpdateUI()
    {
        coinText.text = coin.ToString();
    }
    public void GameResume()
    {

        resumeButton.SetActive(false);
        Vector3 pos = new Vector3(Random.Range(-100,100) , 0 , Random.Range(-100, 100));
        if(CheckPosition(pos))
        {
            player.transform.position = pos;
            SetUI(inGameUI);
            player.GetComponent<CarScript>().Start();
            StartCoroutine(DelayFunction(3f, StartGame, timer));
        }
        else
        {
            GameResume();
        }
    }
    public void GameOver()
    {
        aiManager.enabled = false;
        gameRunning = false;
        player.GetComponent<CarScript>().enabled = false;
        scoreText.gameObject.SetActive(false);
        if(lifeUsed)
        {
            if(saveStateScript.savedData.highScore < score)
            {
                saveStateScript.savedData.highScore = score;
                saveStateScript.SaveState();
            }
            int highScore =  saveStateScript.savedData.highScore;
            AudioManager.Instance.StopAudio();
            Application.ExternalCall("GameEnd", score, highScore);
        }
        else
        {
            lifeUsed = true;
            SetUI(gameOverUI);
            if (saveStateScript.savedData.highScore < score)
            {
                saveStateScript.savedData.highScore = score;
                saveStateScript.SaveState();
            }
            int highScore =  saveStateScript.savedData.highScore;
            AudioManager.Instance.StopAudio();
            Application.ExternalCall("GameLifeEnd" , score, highScore);
        }
    }
    private void SetUI(GameObject go)
    {
        inGameUI.SetActive(false);
        gameOverUI.SetActive(false);
        gamePauseUI.SetActive(false);
        go.SetActive(true);
    }
    IEnumerator DelayFunction(float delay, System.Action functionToCall , TextMeshProUGUI timerText = null)
    {
        if(timerText != null)
        {
            while(delay > 0)
            {
                timerText.gameObject.SetActive(true);
                timerText.text = delay.ToString();
                delay--;
                yield return new WaitForSeconds(1);
                timerText.gameObject.SetActive(false);
            }
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }
        functionToCall();
    }
    private bool CheckPosition(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + new Vector3(0, 100, 0), Vector3.down, out hit))
        {
            if (hit.collider.tag == "Ground")
            {

                Collider[] hitResults = Physics.OverlapBox(hit.point + new Vector3(0, 2, 0), new Vector3(6, 2, 10));
                foreach (Collider collider in hitResults)
                {
                    if (collider.tag == "Building")
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }
    public void ShowRewardedAd()
    {
        Application.ExternalCall("rewardEvent");
    }
    public void AdFailed()
    {
        LeaderBoard();
    }
    public void AdSuccess()
    {
        ResumePressed();
        GameResume();
    }
    public void CompleteReplay()
    {
        GameRestart();
    }
    //GLANCE METHODS
    public void GamePause()
    {
        gamePauseUI.SetActive(true);
        Time.timeScale = 0;
        AudioManager.Instance.StopAudio();
    }
    public void GameExit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        Application.ExternalCall("GameEndEmpty");
    }
    public void ResumePressed()
    {
        Time.timeScale = 1;
        //SetUI(inGameUI);
        gamePauseUI.SetActive(false);
        AudioManager.Instance.PlaySound();
    }
    public void replayGameEvent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void SaveData()
    {
        saveStateScript.SetCoin(coin);
        saveStateScript.SaveState();
    }
    public void LeaderBoard()
    {
        if (saveStateScript.savedData.highScore < score)
        {
            saveStateScript.SetHighScore(score);
            saveStateScript.SaveState();
        }
        int highScore = saveStateScript.GetHighScore();
        print(highScore + "highScore and score" + score);
        Application.ExternalCall("GameEnd", score, highScore);
    }
}
