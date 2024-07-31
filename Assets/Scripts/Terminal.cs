using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Terminal : PluggableObject
{
    [SerializeField] private float printingTextDelay = 1.5f;
    [SerializeField] private Radar radar;
    [SerializeField] private Transform terminalPanel;
    [SerializeField] private TextMeshProUGUI terminalText;
    [SerializeField] private TMP_InputField terminalInput;
    private bool isActive => terminalPanel.gameObject.activeInHierarchy;

    private const string TasksMenuName = "Tasks";
    private const string BestiaryMenuName = "Bestiary";
    private const string RadarMenuName = "Radar";
    private const string InfoMenuName = "Info";
    private const string MenuName = "Menu";
    private const string MenuQuickName = "Q";

    private const string MenuText = @"
Menu:
- Tasks
- Bestiary
- Radar
- Info";

    private Coroutine currentPrintingCoroutine;

    private void Awake()
    {
        CloseTerminal();
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        HandleClosing();
        HandleMenuChanging();
    }

    private void HandleMenuChanging()
    {
        string userInput = terminalInput.text;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                terminalInput.ActivateInputField();
                return;
            }
            userInput = userInput.Trim();
            if (TasksMenuName.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase))
            {
                PrintText("No current tasks available.");
            }
            else if (BestiaryMenuName.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase))
            {
                PrintText("List of beasts: ...");
            }
            else if (RadarMenuName.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase))
            {
                bool isSuccess = radar.TryStartScanning();
                if (isSuccess)
                {
                    PrintText("Radar has restared.");
                }
                else
                {
                    PrintText("Radar is reloading.");
                }
            }
            else if (InfoMenuName.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase))
            {
                PrintText(@"You are 100 meters below sea level.");
            }
            else if (MenuName.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase) || userInput.Equals(MenuQuickName, StringComparison.InvariantCultureIgnoreCase))
            {
                PrintText(MenuText);
            }
            else
            {
                PrintText("Unknown command.");
            }
            terminalInput.text = string.Empty;
            terminalInput.ActivateInputField();
        }
    }

    private void PrintText(string text)
    {
        if (currentPrintingCoroutine != null)
        {
            StopCoroutine(currentPrintingCoroutine);
        }
        currentPrintingCoroutine = StartCoroutine(PrintingCoroutine(text));
    }

    private IEnumerator PrintingCoroutine(string text)
    {
        terminalText.text = string.Empty;
        float letterDelay = printingTextDelay / Mathf.Max(text.Length, 1);
        foreach (char letter in text)
        {
            yield return new WaitForSeconds(letterDelay);
            terminalText.text += letter;
        }
    }

    private void HandleClosing()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTerminal();
        }
    }

    protected override void Slot_OnPowerDisconnected(object sender, EventArgs e)
    {
        base.Slot_OnPowerDisconnected(sender, e);
        CloseTerminal();
    }

    public bool TryOpenTerminal()
    {
        if (hasPowerSupply)
        {
            OpenTerminal();
            return true;
        }
        return false;
    }

    public void CloseTerminal()
    {
        terminalPanel.gameObject.SetActive(false);
    }

    private void OpenTerminal()
    {
        terminalPanel.gameObject.SetActive(true);
        terminalInput.ActivateInputField();
        PrintText(MenuText);
    }
}
