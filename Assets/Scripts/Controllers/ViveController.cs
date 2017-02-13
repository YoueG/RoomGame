﻿using UnityEngine;

public class ViveController : PlayerController
{
    [SerializeField, Range(0,1)]
    float m_detectionRadius = 0.4f;

    SteamVR_TrackedController m_controllerLeft;     // ID = 0
    SteamVR_TrackedController m_controllerRight;    // ID = 1

    TeleportVive m_teleporter;

    void Start()
    {
        m_controllerLeft = transform.FindChild("Controller (left)").GetComponent<SteamVR_TrackedController>();
        m_controllerRight = transform.FindChild("Controller (right)").GetComponent<SteamVR_TrackedController>();

        m_teleporter = GetComponentInChildren<TeleportVive>();
        m_teleporter.SetActiveController(m_controllerLeft.GetComponent<SteamVR_TrackedObject>());

        m_controllerLeft.PadClicked += onPadClickedLeft;            // TP init
        m_controllerLeft.PadUnclicked += onPadUnClickedLeft;        // TP launch

        m_controllerRight.PadClicked += onPadClickedRight;          // Take obj
        m_controllerRight.PadUnclicked += onPadUnClickedRight;      // Drop obj

        m_controllerLeft.TriggerClicked += onTriggerClicked;        // interract
        m_controllerRight.TriggerClicked += onTriggerClicked;

        m_controllerLeft.MenuButtonClicked += onMenuClickLeft;      // Main menu
        m_controllerRight.MenuButtonClicked += onMenuClickRight;    // Inventory menu
    }
    
    void onPadClickedLeft(object sender, ClickedEventArgs e)
    {
        m_teleporter.InitTeleport();
    }
    void onPadUnClickedLeft(object sender, ClickedEventArgs e)
    {
        m_teleporter.Teleport();
    }

    void onPadClickedRight(object sender, ClickedEventArgs e)
    {
        if (Target is MovableObject)
            HoldState = true;
        else if (Target is DrawableObject)
            DrawState = true;
    }
    void onPadUnClickedRight(object sender, ClickedEventArgs e)
    {
        HoldState = false;
        DrawState = false;
    }
    
    void onTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (Target is ActivableObject)
            activate();
    }

    void onMenuClickLeft(object sender, ClickedEventArgs e)
    {
        
    }
    void onMenuClickRight(object sender, ClickedEventArgs e)
    {

    }

    public override void drawObject()
    {
        ((DrawableObject)Target).draw(m_controllerRight.transform.position);
    }

    public override Vector3 getControllerPos()
    {
        return m_controllerRight.transform.position;
    }

    public override void pickFromInventoryCallBack()
    {
    }

    public override void rotateObject()
    {
    }

    public override void updateTarget()
    {
        Collider[] nearObjects = Physics.OverlapSphere(m_holdPoint.transform.position, m_detectionRadius);

        float[] nearObjectsDistance = new float[nearObjects.Length];
        for (int objID = 0; objID < nearObjectsDistance.Length; objID++)
            nearObjectsDistance[objID] = Vector3.Distance(m_holdPoint.transform.position, nearObjects[objID].transform.position);

        System.Array.Sort(nearObjectsDistance, nearObjects);

        foreach (var item in nearObjects)
        {
            Target = item.GetComponentInParent<InteractiveObject>();

            if (Target != null)
                break;
        }
    }

    private bool m_isInventoryOpen = false;
    protected override void eventHandler()
    {

    }
}
