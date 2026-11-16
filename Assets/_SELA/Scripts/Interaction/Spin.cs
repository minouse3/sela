using UnityEngine;

// Untuk baling kipas (SM_Kipas_Baling). Coba sumbu X/Y/Z sampai baling berputar di bidangnya.
public class Spin : MonoBehaviour
{
    public Vector3 axis = Vector3.forward;
    public float degreesPerSecond = 720f;
    public bool running = true;

    void Update()
    {
        if (running) transform.Rotate(axis, degreesPerSecond * Time.deltaTime, Space.Self);
    }

    public void Toggle() { running = !running; }
}
