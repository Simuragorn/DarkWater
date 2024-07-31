using System;
using UnityEngine;

public class Terminal : PluggableObject
{
    [SerializeField] private Transform terminalPanel;

    private void Awake()
    {
        CloseTerminal();
    }

    void Update()
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
    }
}
