using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private string currentDisplay = "Start Menu";

    public GameObject startMenu;
    public GameObject instructionsMenu;
    public GameObject endMenu;

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
        else if (currentDisplay == "End Menu")
        {
            if (Input.GetKeyUp(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            if (Input.GetKeyUp(KeyCode.Q))
                Application.Quit();
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

    public void End()
    {
        DisplayEndMenu();
        currentDisplay = "End Menu";
    }

    private void DisplayEndMenu()
    {
        instructionsMenu.SetActive(false);
        startMenu.SetActive(false);
        endMenu.SetActive(true); ;
    }

    private void DisplayStartMenu()
    {
        instructionsMenu.SetActive(false);
        endMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    private void DisplayInstructionsMenu()
    {
        startMenu.SetActive(false);
        endMenu.SetActive(false);
        instructionsMenu.SetActive(true);
    }

    private void DisplayGame()
    {
        instructionsMenu.SetActive(false);
        endMenu.SetActive(false);
        startMenu.SetActive(false);
    }
}
