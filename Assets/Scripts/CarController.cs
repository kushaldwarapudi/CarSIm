using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public Text SpeedText;
    public Text GearText;
    [SerializeField] private float Wheelradius = 11.4f;
    [SerializeField] private float MotorTorque;
    [SerializeField] private float BrakeTorque;
    [SerializeField] private float H_Input;
    [SerializeField] private float V_Input;
    [SerializeField] private float MaxSteerAngle;
    [SerializeField] private float Steerangle;
    Rigidbody car;
    [SerializeField] private float GearRSpeed;
    [SerializeField] private float CurrentSpeed;
    public WheelCollider FDW, FPW, RDW, RPW;
    public Transform FDT, FPT, RDT, RPT;
    LogitechInput inputs;
    public int CurrentGear;
    public int[] Gearspeeds;
    public bool IgnitionOn;
    public Image Ignition;
    public AudioSource ads;
    public AudioClip StartCar;
    public AudioClip EngineSond;
    // Start is called before the first frame update
    void Start()
    {
        IgnitionOn = false;
        Ignition.enabled = false;
        inputs = GetComponent<LogitechInput>();
        car = GetComponent<Rigidbody>();
        car.centerOfMass += new Vector3(0, -0.9f, 0);
        ads = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        H_Input = Input.GetAxis("Horizontal");
        V_Input = Input.GetAxis("Vertical");
        if (IgnitionOn)
        {
            Ignition.enabled = true;
            if (CurrentGear != 0)
            {
                Accelerate();
               

            }
            else
            {
                FDW.motorTorque = 0;
                FPW.motorTorque = 0;
                ads.pitch = 1+inputs.GasInput;

            }
        }
        else
        {
            FDW.motorTorque = 0;
            FPW.motorTorque = 0;
            Ignition.enabled = false;
            ads.Stop();
            ads.loop = false;
        }
        
        if (inputs.BrakeInput > 0)
        {
            Brake();
        }
        else
        {
            FDW.brakeTorque = 0;
            FPW.brakeTorque = 0;
            RDW.brakeTorque = 0;
            RPW.brakeTorque = 0;
        }
        
        Steer();
        UpdateWheelPose(FDW, FDT);
        UpdateWheelPose(FPW, FPT);
        UpdateWheelPose(RDW, RDT);
        UpdateWheelPose(RPW, RPT);
        CurrentSpeed = Mathf.Round(car.velocity.magnitude * 3.6f);
        SpeedText.text = CurrentSpeed.ToString() + " " + " KMPH ";
        GearText.text = CurrentGear.ToString();
        if (CurrentGear == 0)
        {
            GearText.text = "N";
        }
        if (CurrentGear == -1)
        {
            GearText.text = "R";
            
        }
    }
    public void Accelerate()
    {
       
        
        if (CurrentGear > 0)
        {
            ads.pitch = 1 + (CurrentSpeed / Gearspeeds[CurrentGear - 1]);
           if(inputs.GasInput<=0 && inputs.ClutchInput > 0.1 && inputs.ClutchInput < 1)
            {
                
                if (CurrentGear == 1)
                {
                    float ClutchSpeed = Mathf.Round(Gearspeeds[CurrentGear - 1] - (inputs.ClutchInput * Gearspeeds[CurrentGear - 1]));
                    
                    if (CurrentSpeed <= ClutchSpeed / 2)
                    {
                        FDW.motorTorque = inputs.ClutchInput * (MotorTorque * 2.5f / 2);
                        FPW.motorTorque = inputs.ClutchInput * (MotorTorque * 2.5f / 2);
                        FDW.brakeTorque = 0;
                        FPW.brakeTorque = 0;
                        RDW.brakeTorque = 0;
                        RPW.brakeTorque = 0;
                    }
                    else if (inputs.ClutchInput == 0 || inputs.ClutchInput >= 1)
                    {
                        FDW.motorTorque = 0;
                        FPW.motorTorque = 0;
                    }
                    else
                    {
                        FDW.motorTorque = 0;
                        FPW.motorTorque = 0;
                    }
                }
                
                
            }else if(inputs.ClutchInput < 0.75 && inputs.GasInput > 0)
            {
                float gspeed = Mathf.Round(Gearspeeds[CurrentGear - 1] * inputs.GasInput);
                if (CurrentSpeed <= gspeed)
                {
                    FDW.motorTorque = inputs.GasInput * (MotorTorque * 2.5f / 2);
                    FPW.motorTorque = inputs.GasInput * (MotorTorque * 2.5f / 2);
                    FDW.brakeTorque = 0;
                    FPW.brakeTorque = 0;
                    RDW.brakeTorque = 0;
                    RPW.brakeTorque = 0;


                }
                else
                {
                    FDW.motorTorque = 0;
                    FPW.motorTorque = 0;
                }
            }
            else
            {
                FDW.motorTorque = 0;
                FPW.motorTorque = 0;
            }
            

           
        }
        else if (CurrentGear <= -1)
        {
            float rgspeed = Mathf.Round(GearRSpeed * inputs.GasInput);
            ads.pitch = 1 + (CurrentSpeed / GearRSpeed);
            if(inputs.ClutchInput<=0.75 && inputs.GasInput > 0)
            {
                if (CurrentSpeed <= rgspeed)
                {
                    FDW.motorTorque = -inputs.GasInput * (MotorTorque * 2.5f / 2);
                    FPW.motorTorque = -inputs.GasInput * (MotorTorque * 2.5f / 2);
                    FDW.brakeTorque = 0;
                    FPW.brakeTorque = 0;
                    RDW.brakeTorque = 0;
                    RPW.brakeTorque = 0;

                }
                else
                {
                    FDW.motorTorque = 0;
                    FPW.motorTorque = 0;
                }
            }
            else
            {
                FDW.motorTorque = 0;
                FPW.motorTorque = 0;
            }
            
        }
        
           
        
    }
    public void Steer()
    {
        if (inputs.SteerInput > 0)
        {
            FDW.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.78f / (Wheelradius + (1.515f / 2))) * inputs.SteerInput * 2;
            FPW.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.78f / (Wheelradius - (1.515f / 2))) * inputs.SteerInput * 2;
        }else if (inputs.SteerInput < 0)
        {
            FDW.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.78f / (Wheelradius - (1.515f / 2))) * inputs.SteerInput * 2;
            FPW.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.78f / (Wheelradius + (1.515f / 2))) * inputs.SteerInput * 2;
        }
        else
        {
            FDW.steerAngle = 0;
            FPW.steerAngle = 0;
        }
    }
    public void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;
        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }
    public void Brake()
    {
        FDW.motorTorque = 0;
        FPW.motorTorque = 0;
        FDW.brakeTorque = BrakeTorque;
        FPW.brakeTorque = BrakeTorque;
        RDW.brakeTorque = BrakeTorque/2;
        RPW.brakeTorque = BrakeTorque/2;
    }
    public IEnumerator PlayEngineSound()
    {
        ads.clip = StartCar;
        ads.Play();
        yield return new WaitForSeconds(ads.clip.length);
        ads.clip = EngineSond;
        ads.Play();
        ads.loop = true;
    }
}
