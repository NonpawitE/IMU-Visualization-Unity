using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SensorDataHandler : MonoBehaviour
{
    [Header("Arm Pivots")]
    [SerializeField] private GameObject _topArmPivot;
    [SerializeField] private GameObject _bottomArmPivot;

    [Header("Arm Angles")]
    [SerializeField] [Range(0, 360)] private int _topArmAngle;
    [SerializeField] [Range(0, 360)] private int _bottomArmAngle;

    [Header("Connectivities")]
    [SerializeField] private string _comPort;

    SerialPort serial;

    // Start is called before the first frame update
    void Start()
    {
        serial = new SerialPort(_comPort, 250400);
        serial.Open();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = serial.ReadLine();
        serial.BaseStream.Flush();

        string[] str_arr = buffer.Split(',');
        if (str_arr[0] != "" &&
            str_arr[1] != "" &&
            str_arr[2] != "" &&
            str_arr[3] != "" &&
            str_arr[4] != "" &&
            str_arr[5] != "")
        {
            _topArmPivot.transform.eulerAngles = new Vector3(int.Parse(str_arr[0]), int.Parse(str_arr[1]), int.Parse(str_arr[2]));
            _bottomArmPivot.transform.eulerAngles = new Vector3(int.Parse(str_arr[3]), int.Parse(str_arr[4]), int.Parse(str_arr[5]));
                
            serial.BaseStream.Flush();
        }
    }
}
