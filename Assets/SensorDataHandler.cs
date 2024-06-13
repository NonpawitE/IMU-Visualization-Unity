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
    [SerializeField] Quaternion offsetRotation;

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

        setOffsetButton.onClick.AddListener(CaptureOffset);
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion currentReadings = GetSensorReadings();

        Quaternion offset = Quaternion.Inverse(offsetRotation) * currentReadings;

        _armPivot.transform.rotation = offset;
    }

    private void OnApplicationQuit()
    {
        if (!string.IsNullOrEmpty(_comPort))
        {
            serial.Close();
        }
    }

    void CaptureOffset()
    {
        offsetRotation = GetSensorReadings();
    }

    Quaternion GetSensorReadings()
    {
        if (!string.IsNullOrEmpty(_comPort))
        {
            string buffer = serial.ReadLine();
            serial.BaseStream.Flush();

            string[] str_arr = buffer.Split(',');
            if (str_arr[0] != "" &&
                str_arr[1] != "" &&
                str_arr[2] != "" &&
                str_arr[3] != "")
            {
                float w = float.Parse(str_arr[0]);
                float x = float.Parse(str_arr[1]);
                float y = float.Parse(str_arr[2]);
                float z = float.Parse(str_arr[3]);

                serial.BaseStream.Flush();

                return new Quaternion(x, -z, y, w);
            }
        }

        return new Quaternion(0, 0, 0, 0);
    }
}
