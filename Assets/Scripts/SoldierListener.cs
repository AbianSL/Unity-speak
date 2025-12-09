using UnityEngine;
using Whisper.Samples;

public class SoldierListener : MonoBehaviour
{

    public MyMicrophone myMicrophone;

    public float force = 300f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myMicrophone.onHolaDetected += OnHolaDetected;
    }

    private void OnHolaDetected()
    {
        GetComponent<Rigidbody>()?.AddForce(Vector3.up * force);
    }
}
