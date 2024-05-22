using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class SensorDataHandler : MonoBehaviour
{
    [Header("Arm Objects")]
    [SerializeField] private GameObject _armPivot;
    [SerializeField] [Range(-180, 180)] private float _armX;
    [SerializeField] [Range(-180, 180)] private float _armY;
    [SerializeField] [Range(-180, 180)] private float _armZ;

    [Header("Arm Offsets")]
    [SerializeField] [Range(-180, 180)] private float _offsetX;
    [SerializeField] [Range(-180, 180)] private float _offsetY;
    [SerializeField] [Range(-180, 180)] private float _offsetZ;

    [SerializeField] private Button setOffsetButton;

    [Header("Connectivities")]
    [SerializeField] private string _comPort;

    SerialPort serial;

    // Start is called before the first frame update
    void Start()
    {
        if (!string.IsNullOrEmpty(_comPort))
        {
            try
            {
                serial = new SerialPort(_comPort, 250400);
                serial.Open();
                Debug.Log("Serial port opened: " + _comPort);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to open serial port: " + e.Message);
            }
        } else
        {
            Debug.LogWarning("Port name is empty, not opening serial port.");
        }

        setOffsetButton.onClick.AddListener(SetOffset);
    }

    // Update is called once per frame
    void Update()
    {
        if (!string.IsNullOrEmpty(_comPort))
        {
            string buffer = serial.ReadLine();
            serial.BaseStream.Flush();

            string[] str_arr = buffer.Split(',');
            if (str_arr[0] != "" &&
                str_arr[1] != "" &&
                str_arr[2] != "")
            {
                float x = float.Parse(str_arr[0]);
                float y = float.Parse(str_arr[1]);
                float z = float.Parse(str_arr[2]);

                _armPivot.transform.eulerAngles = new Vector3(
                    ClampAngle(x, _offsetX),    
                    ClampAngle(y, _offsetY),    
                    ClampAngle(z, _offsetZ)
                );

                serial.BaseStream.Flush();
            }
        } else
        {
            _armPivot.transform.eulerAngles = new Vector3(
                ClampAngle(_armX, _offsetX),
                ClampAngle(_armY, _offsetY),
                ClampAngle(_armZ, _offsetZ)
            );
        }
    }

    float ClampAngle(float angle, float offset)
    {
        float adjudstedAngle = angle + offset;

        if (adjudstedAngle > 180.0f)
        {
            adjudstedAngle -= 360.0f;
        } else if (adjudstedAngle < -180.0f)
        {
            adjudstedAngle += 360.0f;
        }

        return adjudstedAngle;
    }

    void SetOffset()
    {
        Vector3 temp = _armPivot.transform.eulerAngles;
        _offsetX = temp.x;
        _offsetY = temp.y;
        _offsetZ = temp.z;
    }
}
