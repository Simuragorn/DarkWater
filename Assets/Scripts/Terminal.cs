using System;
using TMPro;
using UnityEngine;

public class Terminal : PluggableObject
{
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
            if (userInput.Equals(TasksMenuName, StringComparison.InvariantCultureIgnoreCase))
            {
                terminalText.text = "No current tasks";
            }
            else if (userInput.Equals(BestiaryMenuName, StringComparison.InvariantCultureIgnoreCase))
            {
                terminalText.text = "List of beasts: ...";
            }
            else if (userInput.Equals(RadarMenuName, StringComparison.InvariantCultureIgnoreCase))
            {
                terminalText.text = "Trying to restart radar";
            }
            else if (userInput.Equals(InfoMenuName, StringComparison.InvariantCultureIgnoreCase))
            {
                terminalText.text = @"You are 100 meters below sea level.";
            }
            else if (userInput.Equals(MenuName, StringComparison.InvariantCultureIgnoreCase) || userInput.Equals(MenuQuickName, StringComparison.InvariantCultureIgnoreCase))
            {
                terminalText.text = @"
Menu:
- Tasks
- Bestiary
- Radar
- Info";
            }
            else
            {
                terminalText.text = "Unknown command";
            }
            terminalInput.text = string.Empty;
            terminalInput.ActivateInputField();
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
    }
}
