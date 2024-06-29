using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class movesoham : MonoBehaviour{
    public progressBar pb;
    public bool canmove;
    enum joint
    {
        knee,
        elbow,
        wrist,
        ankle
    }

    enum angle
    {
        x,
        y,
        xy
    }

    public int max, min = 0;

    [SerializeField]
    angle ang = new angle();

    [SerializeField]
    joint js = new joint();

    [SerializeField]
    int kneedeviation;

    [SerializeField]
    int elbowdeviation;

    [SerializeField]
    int wristdeviationx;

    [SerializeField]
    int wristdeviationy;

    float gyrox, gyroy, accx, accy, accz;
    float gyrox2, gyroy2, accx2, accy2, accz2;
    float errgx, errgy, errax, erray, erraz;

    public string[] data;

    [SerializeField]
    TMP_Text angText;

    public int gg;

    int sample_size = 500;
    int scale = 10;
    bool cal = false;
    float alpha = 0.91f;

    public GameObject leg;
    public Quaternion invQT;
    public Quaternion rawQT;
    public Quaternion rawQT2;

    void Start()
    {
        while (data.Length < 8) { }

        invQT = Quaternion.identity;

        UpdateSensorData();

        rawQT = new Quaternion(accx, gyroy, -gyrox, accy);
        rawQT2 = new Quaternion(accx2, gyroy2, -gyrox2, accy2);

        invQT = Quaternion.Inverse(rawQT);
    }

    void UpdateSensorData()
    {
        gyrox = float.Parse(data[0]);
        gyroy = float.Parse(data[1]);
        accx = float.Parse(data[2]);
        accy = float.Parse(data[3]);

        gyrox2 = float.Parse(data[4]);
        gyroy2 = float.Parse(data[5]);
        accx2 = float.Parse(data[6]);
        accy2 = float.Parse(data[7]);
    }

    void calibration()
    {
        UpdateSensorData();

        rawQT = new Quaternion(accx, gyroy, -gyrox, accy);
        rawQT2 = new Quaternion(accx2, gyroy2, -gyrox2, accy2);

        invQT = Quaternion.Inverse(rawQT);
        cal = false;
        resetminmax();
    }

    public void cancal()
    {
        cal = true;
    }

    void FixedUpdate()
    {
        if (data.Length >= 8)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                invQT = Quaternion.Inverse(rawQT);
            }

            if (cal)
            {
                calibration();
            }

            UpdateSensorData();

            rawQT = new Quaternion(accx, gyroy, -gyrox, accy);
            rawQT2 = new Quaternion(accx2, gyroy2, -gyrox2, accy2);

            if (ShouldMove())
            {
                Quaternion ang = rawQT * invQT;
                leg.transform.localRotation = ang;

                gg = (((int)(-2 * (Mathf.Rad2Deg * Mathf.Acos(leg.transform.localRotation.x)))) * -1 - kneedeviation) * -1;

                if (Mathf.Abs(gg) > Mathf.Abs(max))
                {
                    max = gg;
                }

                if (js == joint.knee || js == joint.elbow)
                {
                    angText.text = "" + -gg;
                }
                else
                {
                    angText.text = "ANGLEX: " + Mathf.Abs(gg) + "\nANGLEY: " + Mathf.Abs((int)(-2 * (Mathf.Rad2Deg * Mathf.Acos(leg.transform.localRotation.z))) * -1 - wristdeviationx);
                }

                pb.health = -gg;
            }
        }
    }

        bool ShouldMove()
    {
        float dotProduct = Quaternion.Dot(rawQT, rawQT2);
        bool yay = dotProduct < 0.95f;
         Debug.Log("Dot Product: " +yay);
        return yay; // Adjust this threshold as needed to cancel out whole limb movement
    }

    public void resetminmax()
    {
        max = 0;
        min = 0;
    }

    float AdjustAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }
}

public class KalmanFilter
{
    private float Xk; // State estimate
    private float Pk; // Estimate error covariance
    private float Q = 0.01f; // Process noise covariance
    private float R = 0.1f; // Measurement noise covariance

    public void SetState(float initialState)
    {
        Xk = initialState;
        Pk = 1.0f; // Initial covariance estimation
    }

    public float PredictAndUpdate(float measurement)
    {
        // Prediction
        float Xk_ = Xk;
        float Pk_ = Pk + Q;

        // Kalman gain
        float K = Pk_ / (Pk_ + R);

        // Update state
        Xk = Xk_ + K * (measurement - Xk_);
        Pk = (1 - K) * Pk_;

        return Xk;
    }
}