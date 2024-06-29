using UnityEngine;
using TMPro;

public class movesoham : MonoBehaviour
{
    public progressBar pb;

    enum JointType
    {
        Knee,
        Elbow,
        Wrist,
        Ankle
    }

    [SerializeField] JointType jointType;
    [SerializeField] int kneeDeviation;
    [SerializeField] int elbowDeviation;
    [SerializeField] int wristDeviationX;
    [SerializeField] int wristDeviationY;

    public string[] data;
    public GameObject jointObject;
    public TMP_Text angleText;

    private Quaternion initialRotation;
    private Quaternion currentRotation;
    public int max;
    public int min =0;

    public int angle;
    void Start()
    {
        if (data.Length < 4) return;

        UpdateRotations();
        initialRotation = Quaternion.Inverse(currentRotation);
    }

    void FixedUpdate()
    {
        if (data.Length < 4) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetInitialRotation();
        }

        UpdateRotations();
        ApplyRotation();
        UpdateAngleDisplay();
    }

    void UpdateRotations()
    {
        float x = float.Parse(data[0]);
        float y = float.Parse(data[1]);
        float z = float.Parse(data[2]);
        float w = float.Parse(data[3]);

        currentRotation = GetRotationForJoint(x, y, z, w);
    }

    Quaternion GetRotationForJoint(float x, float y, float z, float w)
    {
        switch (jointType)
        {
            case JointType.Knee:
            case JointType.Elbow:
                return new Quaternion(z, y, -x, w);
            case JointType.Ankle:
                return new Quaternion(-y, -x, z, w);
            case JointType.Wrist:
            default:
                return new Quaternion(x, y, -z, w);
        }
    }

    void ApplyRotation()
    {
        Quaternion rotationDelta = currentRotation * initialRotation;
        jointObject.transform.localRotation = rotationDelta;
    }

    void UpdateAngleDisplay()
    {
        angle = CalculateAngle();
        max = Mathf.Max(max, Mathf.Abs(angle));

        if (jointType == JointType.Knee || jointType == JointType.Elbow)
        {
            angleText.text = (-angle).ToString();
            pb.health = -angle;
        }
        else
        {
            int angleY = CalculateAngleY();
            angleText.text = $"ANGLE X: {Mathf.Abs(angle)}\nANGLE Y: {angleY}";
        }
    }

    int CalculateAngle()
    {
        float rawAngle = -2 * (Mathf.Rad2Deg * Mathf.Acos(jointObject.transform.localRotation.x));
        return ((int)rawAngle - kneeDeviation) * -1;
    }

    int CalculateAngleY()
    {
        float rawAngle = -2 * (Mathf.Rad2Deg * Mathf.Acos(jointObject.transform.localRotation.z));
        return Mathf.Abs(((int)rawAngle * -1) - wristDeviationX);
    }

    void ResetInitialRotation()
    {
        initialRotation = Quaternion.Inverse(currentRotation);
    }

    public void ResetMaxAngle()
    {
     max = 0;
    }
}