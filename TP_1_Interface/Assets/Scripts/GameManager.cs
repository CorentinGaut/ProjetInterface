using System;
using System.Collections;
using System.Collections.Generic;
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

    public void Play()
    {
        DisplayInstructionsMenu();
        currentDisplay = "Instructions Menu";
    }

    public void Continue()
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
