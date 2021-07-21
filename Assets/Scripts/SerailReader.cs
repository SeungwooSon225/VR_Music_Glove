using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;



public class SerailReader : MonoBehaviour
{
    public Text[] leftFingerText = new Text[5];

    public float[] leftFingerValues = new float[5];
    public GameObject[] leftThumb = new GameObject[3];
    public GameObject[] leftIndex = new GameObject[3];
    public GameObject[] leftMiddle = new GameObject[3];
    public GameObject[] leftRing = new GameObject[3];
    public GameObject[] leftLittle = new GameObject[3];

    private SerialPort stream = new SerialPort("COM3", 115200);
    private int firstByte;
    private int secondByte;
    private int value;
    private float filterSensitivity = 0.1f;
    private float time;
    private float[] leftFingerMax = new float[5];
    private float[] leftFingerMin = new float[5];
    private float[] leftFingerAngles = new float[5];


    // Start is called before the first frame update
    void Start()
    {
        stream.Open();  // Open the Serial Stream.
        Debug.Log("Open");

        for (int i = 0; i < 5; i++)
        {
            firstByte = stream.ReadByte();
            secondByte = stream.ReadByte();

            value = firstByte | (secondByte << 8);

            leftFingerValues[i] = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            firstByte = stream.ReadByte();
            secondByte = stream.ReadByte();

            value = firstByte | (secondByte << 8);

            leftFingerValues[i] = filterSensitivity * (float)value + (1-filterSensitivity) * (float)leftFingerValues[i];
        }

        CalculateFingerAnger();
        UpdateFingerAnger();

        for (int i = 0; i < 5; i++)
        {
            leftFingerText[i].text = "V: " + leftFingerValues[i].ToString("F1") + "A: " + leftFingerAngles[i].ToString("F1");
        }
    }

    void OnDisable()
    {
        stream.Close();
        Debug.Log("Close");
    }

    public void LMCalibrationClick()
    {
        Debug.Log("LMC Click!");

        for (int i = 0; i < 5; i++)
        {
            leftFingerMax[i] = leftFingerValues[i];
            Debug.Log(i + " : " + leftFingerMax[i]);
        }
    }

    public void LmCalibrationClick()
    {
        Debug.Log("LmC Click!");

        for (int i = 0; i < 5; i++)
        {
            leftFingerMin[i] = leftFingerValues[i];
            Debug.Log(i + " : " + leftFingerMin[i]);
        }
    }

    private void CalculateFingerAnger()
    {
        float priviousAngle = leftFingerAngles[1];

        for (int i = 0; i < 5; i++)
        {
            leftFingerAngles[i] = 90  - (leftFingerValues[i] - leftFingerMin[i]) / (leftFingerMax[i] - leftFingerMin[i]) * 90;

            if (leftFingerAngles[i] < 0)
            {
                leftFingerAngles[i] = 0;
            }
            else if (leftFingerAngles[i] > 90)
            {
                leftFingerAngles[i] = 90;
            }
        }

        //leftIndex[0].transform.Rotate(new Vector3(priviousAngle - leftFingerAngles[1], 0, 0), Space.Self);
    }

    private void UpdateFingerAnger()
    {
        for (int i=0; i<3; i++)
        {
            leftIndex[i].transform.localRotation = Quaternion.Euler(new Vector3(-leftFingerAngles[1], 0, 0));
            leftMiddle[i].transform.localRotation = Quaternion.Euler(new Vector3(-leftFingerAngles[2], 0, 0));
            leftRing[i].transform.localRotation = Quaternion.Euler(new Vector3(-leftFingerAngles[3], 0, 0));
            leftLittle[i].transform.localRotation = Quaternion.Euler(new Vector3(-leftFingerAngles[4], 0, 0));
        }

        for (int i = 1; i < 3; i++)
        {
            leftThumb[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, leftFingerAngles[0]));
        }
    }
}
