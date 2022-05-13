using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogitechInput : MonoBehaviour
{
    LogitechGSDK.LogiControllerPropertiesData properties;
    public float GasInput;
    public float BrakeInput;
    public float SteerInput;
    public float ClutchInput;
    public int GearInput;
    // Start is called before the first frame update
    void Start()
    {
        print("LogiSteeringIntialize:" + LogitechGSDK.LogiSteeringInitialize(false));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
            GearShifter();

        if (LogitechGSDK.LogiButtonTriggered(0, 2))
        {
            if (GetComponent<CarController>().IgnitionOn == false)
            {
                GetComponent<CarController>().IgnitionOn = true;
                if (GetComponent<CarController>().ads.isPlaying == false)
                {
                    GetComponent<CarController>().StartCoroutine(GetComponent<CarController>().PlayEngineSound());
                }
            }
            else
            {
                GetComponent<CarController>().IgnitionOn = false;
            }
            
        }
        if (LogitechGSDK.LogiButtonTriggered(0, 23))
        {

            Debug.Log("Working");

        }
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES L_Input;
            L_Input = LogitechGSDK.LogiGetStateUnity(0);
            //Steeering
            SteerInput = L_Input.lX / 32760f;
            //Accelerator
            if (L_Input.lY > 0)
            {
                GasInput = 0;
            }
            else if (L_Input.lY < 0)
            {
                GasInput = L_Input.lY / -32760f;
            }
            //Brake
            if (L_Input.lRz > 0)
            {
                BrakeInput = 0;
            }
            else if (L_Input.lRz < 0)
            {
                BrakeInput = L_Input.lRz / -32760f;
            }
            //Clutch
            if (L_Input.rglSlider[0] > 0)
            {
                ClutchInput = 0;
            }
            else if (L_Input.rglSlider[0] < 0)
            {
                ClutchInput = L_Input.rglSlider[0] / -32760f;
            }
        }
    }
    public void GearShifter()
    {
        if (ClutchInput > 0)
        {
            if (LogitechGSDK.LogiButtonIsPressed(0, 12))
            {
                GetComponent<CarController>().CurrentGear = 1;
            }
            if (LogitechGSDK.LogiButtonIsPressed(0, 13))
            {
                GetComponent<CarController>().CurrentGear = 2;
            }
            if (LogitechGSDK.LogiButtonIsPressed(0, 14))
            {
                GetComponent<CarController>().CurrentGear = 3;
            }
            if (LogitechGSDK.LogiButtonIsPressed(0, 15))
            {
                GetComponent<CarController>().CurrentGear = 4;
            }
            if (LogitechGSDK.LogiButtonIsPressed(0, 16))
            {
                GetComponent<CarController>().CurrentGear = 5;
            }
            if (LogitechGSDK.LogiButtonIsPressed(0, 17))
            {
                GetComponent<CarController>().CurrentGear = -1;
            }
        }
        for(int i = 12; i < 18; i++)
        {
            if(LogitechGSDK.LogiButtonReleased(0, i))
            {
                GetComponent<CarController>().CurrentGear = 0;
            }
        }
    }
}
