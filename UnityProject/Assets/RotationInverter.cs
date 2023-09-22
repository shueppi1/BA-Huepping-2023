using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class RotationInverter : MonoBehaviour
{
    [SerializeField, Tooltip("The input action to read the rotation value of a tracked device. Must be a Quaternion control type.")]
    InputActionProperty m_RotationInput;
    /// <summary>
    /// The input action to read the rotation value of a tracked device.
    /// Must support reading a value of type <see cref="Quaternion"/>.
    /// </summary>
    /// <seealso cref="positionInput"/>
    public InputActionProperty rotationInput { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
