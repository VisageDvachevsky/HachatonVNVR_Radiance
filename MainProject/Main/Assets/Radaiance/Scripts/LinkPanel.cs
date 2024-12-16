using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPanel : MonoBehaviour
{
    public PanelSpawner panelSpawner;
    void Start()
    {
        panelSpawner = GameObject.FindGameObjectWithTag("Manager").GetComponent<PanelSpawner>();
    }

    public void GrabOn()
    {
        panelSpawner.GrabPanel();
    }

    public void GrabOff()
    {
        panelSpawner.UngrabPanel();
    }
}
