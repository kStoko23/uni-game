using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private List<Mole> moles;
   
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject settingsButtonPause;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject backButtonBombAndTime;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject scoresMenu;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject pauseBackground;
    [SerializeField] private GameObject pauseHeader;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject outOfTimeText;
    [SerializeField] private GameObject bombText;
    [SerializeField] private GameObject timeHeader;
    [SerializeField] private GameObject scoreHeader;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private GameObject volumeSlider;
    [SerializeField] private GameObject musicSlider;
    [SerializeField] private GameObject volumeSliderHeader;
    [SerializeField] private GameObject musicSliderHeader;
    [SerializeField] private GameObject musicSource;
    [SerializeField] private GameObject soundEffectSource;
    [SerializeField] private GameObject bombSoundSource;
    [SerializeField] private GameObject outOfTimeSoundSource;
    [SerializeField] private GameObject backButtonSettings;
    [SerializeField] private GameObject backButtonPause;
    [SerializeField] private TMPro.TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject Moles;
    
    private float startingTime = 30f;
    
    private float timeRemaining;
    private HashSet<Mole> currentMoles = new HashSet<Mole>();
    private int score;
    private bool playing = false;
    private const int MaxScores = 5;
    private string scoreKey = "Score_";

    private void SaveScore(int score)
    {        
        List<int> scores = new List<int>();
        for (int i = 0; i < MaxScores; i++)
        {
            scores.Add(PlayerPrefs.GetInt(scoreKey + i, 0));
        }
        
        scores.Add(score);
       
        scores.Sort((a, b) => b.CompareTo(a)); 
        if (scores.Count > MaxScores)
        {
            scores = scores.GetRange(0, MaxScores);
        }
       
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetInt(scoreKey + i, scores[i]);
        }

        PlayerPrefs.Save();
    }

    private List<int> LoadScores()
    {
        List<int> scores = new List<int>();
        for (int i = 0; i < MaxScores; i++)
        {
            scores.Add(PlayerPrefs.GetInt(scoreKey + i, 0));
        }
        return scores;
    }

    private void UpdateHighScoreUI()
    {
        List<int> scores = LoadScores();
        highScoreText.text = "";
        for (int i = 0; i < scores.Count; i++)
        {
            highScoreText.text += (i + 1).ToString() + ". " + scores[i].ToString() + "\n";
        }
    }

    private void Start()
    {
        UpdateHighScoreUI();
    }
            
    public void StartGame() {    
    Moles.SetActive(true);
    playButton.SetActive(false);
    mainMenu.SetActive(false);
    outOfTimeText.SetActive(false);
    bombText.SetActive(false);
    gameUI.SetActive(true);
    pauseBackground.SetActive(false);
    scoreHeader.SetActive(true);
    timeHeader.SetActive(true);
    timeText.gameObject.SetActive(true);
    scoreText.gameObject.SetActive(true);
    pauseButton.SetActive(true);
   
    musicSource.GetComponent<AudioSource>().Play();
    
    for (int i = 0; i < moles.Count; i++) {
      moles[i].Hide();
      moles[i].SetIndex(i);
    }
    
    currentMoles.Clear();
   
    timeRemaining = startingTime;
    score = 0;
    scoreText.text = "0";
    playing = true;
  }

  public void GameOver(int type) {
    pauseBackground.SetActive(true);
    pauseButton.SetActive(false);
    backButtonBombAndTime.SetActive(true);
    
    musicSource.GetComponent<AudioSource>().Stop();
    
    if (type == 0) {
      outOfTimeText.SetActive(true);
       outOfTimeSoundSource.GetComponent<AudioSource>().Play();
    } else {
      bombText.SetActive(true);
      bombSoundSource.GetComponent<AudioSource>().Play();
    }
    
    foreach (Mole mole in moles) {
      mole.StopGame();
    }   
        
    playing = false;
    playButton.SetActive(true);

    SaveScore(score);        
    }
 
  void Update() {
    if (playing) {      
      timeRemaining -= Time.deltaTime;
      if (timeRemaining <= 0) {
        timeRemaining = 0;
        GameOver(0);
      }
      timeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";     
      if (currentMoles.Count <= (score / 10)) {        
        int index = Random.Range(0, moles.Count);        
        if (!currentMoles.Contains(moles[index])) {
          currentMoles.Add(moles[index]);
          moles[index].Activate(score / 10);
        }
      }
    }
  }

  public void AddScore(int moleIndex) {    
    score += 1;
    scoreText.text = $"{score}";    
    timeRemaining += 1;    
    soundEffectSource.GetComponent<AudioSource>().Play();   
    currentMoles.Remove(moles[moleIndex]);
  }

  public void Missed(int moleIndex, bool isMole) {
    if (isMole) {     
      timeRemaining -= 2;
    }    
    currentMoles.Remove(moles[moleIndex]);
  }

    public void PauseGame()
    {        
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);
        pauseBackground.SetActive(true);
        pauseHeader.SetActive(true);
        settingsButtonPause.SetActive(true);
        backButton.SetActive(true);
        volumeSlider.SetActive(true);
        musicSlider.SetActive(true);
        volumeSliderHeader.SetActive(true);
        musicSliderHeader.SetActive(true);
        musicSource.GetComponent<AudioSource>().Stop();
       
        foreach (Mole mole in moles)
        {
            mole.StopGame();
        }
       
        playing = false;
    }

    public void ResumeGame()
    {       
        pauseButton.SetActive(true);
        resumeButton.SetActive(false);
        pauseBackground.SetActive(false);
        pauseHeader.SetActive(false);
        settingsButtonPause.SetActive(false);
        backButton.SetActive(false);

        musicSource.GetComponent<AudioSource>().Play();

        foreach (Mole mole in currentMoles)
        {
            mole.ResumeGame();
        }
        
        playing = true;
    }

    public void SettingsMenuPause()
    {
        pauseButton.SetActive(false);
        resumeButton.SetActive(false);
        pauseBackground.SetActive(false);
        pauseHeader.SetActive(false);
        settingsButtonPause.SetActive(false);
        backButton.SetActive(false);
        scoreHeader.SetActive(false);
        timeHeader.SetActive(false);
        timeText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        settingsMenu.SetActive(true);
        backButtonSettings.SetActive(false);
        backButtonPause.SetActive(true);
    }

    public void BackToMenu()
    {        
        pauseButton.SetActive(false);
        resumeButton.SetActive(false);
        pauseBackground.SetActive(false);
        pauseHeader.SetActive(false);
        settingsButtonPause.SetActive(false);
        backButton.SetActive(false);
        scoreHeader.SetActive(false);
        timeHeader.SetActive(false);
        timeText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        backButtonBombAndTime.SetActive(false);
        outOfTimeText.SetActive(false);
        bombText.SetActive(false);
        UpdateHighScoreUI();
        musicSource.GetComponent<AudioSource>().Stop();
       
        foreach (Mole mole in moles)
        {
            mole.StopGame();
        }
        
        playing = false;
        playButton.SetActive(true);
        mainMenu.SetActive(true);
    }

    public void BackToPauseFromSettings()
    {
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);
        pauseBackground.SetActive(true);
        pauseHeader.SetActive(true);
        settingsButtonPause.SetActive(true);
        backButton.SetActive(true);
        scoreHeader.SetActive(true);
        timeHeader.SetActive(true);
        timeText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        settingsMenu.SetActive(false);
        backButtonSettings.SetActive(true);
        backButtonPause.SetActive(false);
    }

    public void SettingsMenu()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ScoresMenu()
    {        
        mainMenu.SetActive(false);
        scoresMenu.SetActive(true);
    }

    public void BackToMenuFromScores()
    {
        scoresMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void BackToMenuFromSettings()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void VolumeChange()
    {
       soundEffectSource.GetComponent<AudioSource>().volume = volumeSlider.GetComponent<UnityEngine.UI.Slider>().value;
       bombSoundSource.GetComponent<AudioSource>().volume = volumeSlider.GetComponent<UnityEngine.UI.Slider>().value;
       outOfTimeSoundSource.GetComponent<AudioSource>().volume = volumeSlider.GetComponent<UnityEngine.UI.Slider>().value;
    }

    public void MusicChange()
    {
        musicSource.GetComponent<AudioSource>().volume = musicSlider.GetComponent<UnityEngine.UI.Slider>().value;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
