using UnityEngine;
using Whisper.Samples;

public class WizardListener : MonoBehaviour
{

    public MyMicrophone myMicrophone;

    public float force = 300f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myMicrophone.onAdiosDetected += OnAdiosDetected;
    }

    private void OnAdiosDetected()
    {
        GetComponent<Rigidbody>()?.AddForce(Vector3.forward * force);
    }
}
