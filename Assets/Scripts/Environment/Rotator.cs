using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float xRotationValue;
    [SerializeField] private float yRotationValue;
    [SerializeField] private float zRotationValue;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Time.deltaTime * xRotationValue, Time.deltaTime * yRotationValue, Time.deltaTime * zRotationValue);
    }
}
