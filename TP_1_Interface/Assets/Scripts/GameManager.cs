using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string currentDisplay = "Start Menu";

    public GameObject startMenu;
    public GameObject instructionsMenu;

    private void Awake()
    {
        DisplayStartMenu();
    }

    private void Update()
    {
        CheckInputs();
    }

    private void CheckInputs()
    {
        if (currentDisplay == "Start Menu")
        {
            if (Input.GetKeyUp(KeyCode.E))
                Instructions();
        }
        else if (currentDisplay == "Instructions Menu")
        {
            if (Input.GetKeyUp(KeyCode.E))
                Play();
        }
        else if (currentDisplay == "Game")
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                Instructions();
        }
    }

    public void Instructions()
    {
        DisplayInstructionsMenu();
        currentDisplay = "Instructions Menu";
    }

    public void Play()
    {
        DisplayGame();
        currentDisplay = "Game";
    }

    public void Menu()
    {
        DisplayStartMenu();
        currentDisplay = "Start Menu";
    }

    private void DisplayStartMenu()
    {
        instructionsMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    private void DisplayInstructionsMenu()
    {
        startMenu.SetActive(false);
        instructionsMenu.SetActive(true);
    }

    private void DisplayGame()
    {
        instructionsMenu.SetActive(false);
        startMenu.SetActive(false);
    }
}
