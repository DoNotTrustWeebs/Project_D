using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SmoothTurnProvider : LocomotionProvider
{
    public float turnSegment = 45;
    public float turnTime = 3;

    public InputHelpers.Button rightTurnButton = InputHelpers.Button.PrimaryAxis2DRight;
    public InputHelpers.Button leftTurnButton = InputHelpers.Button.PrimaryAxis2DLeft;

    public List<XRController> controllers = new List<XRController>();

    private float targetTurnAmount = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (CanBeginLocomotion())
            CheckForInput();
    }

    private void CheckForInput()
    {
        //every frame the program checks if the player wants to turn
        foreach(XRController controller in controllers)
        {
            targetTurnAmount = CheckForTurn(controller);

            if (targetTurnAmount != 0.0f)
                TrySmoothTurn();
        }
    }

    private void TrySmoothTurn()
    {
        //calls the turning method with values from another method
        StartCoroutine(TurnRoutine(targetTurnAmount));

        targetTurnAmount = 0.0f;
    }

    private IEnumerator TurnRoutine(float turnAmount)
    {
        //makes the player turn for a certain amount of time
        float previousTurnChange = 0.0f;
        float elapsedTime = 0.0f;

        BeginLocomotion();

        while(elapsedTime <= turnTime)
        {
            float blend = elapsedTime / turnTime;
            float turnChange = Mathf.Lerp(0, turnAmount, blend);

            float turnDifference = turnChange - previousTurnChange;
            system.xrOrigin.RotateAroundCameraUsingOriginUp(turnDifference);

            previousTurnChange = turnChange;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        EndLocomotion();
    }

    private float CheckForTurn(XRController controller)
    {
        //checks if the player wants to turn
        if(controller.inputDevice.IsPressed(rightTurnButton, out bool rightPress))
        {
            if (rightPress)
                return turnSegment;
        }
        if (controller.inputDevice.IsPressed(leftTurnButton, out bool leftPress))
        {
            if (leftPress)
                return -turnSegment;
        }
        return 0.0f;
    }
}
