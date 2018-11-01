using UnityEngine;
using System.Collections;

public class FlyCamera : MonoBehaviour
{
    /*
    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    Converted to C# 27-02-13 - no credit wanted.
    Simple flycam I made, since I couldn't find any others made public.  
    Made simple to use (drag and drop, done) for regular keyboard layout  
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height
    */

    public float MainSpeed = 100.0f; //regular speed
    public float ShiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    public float MaxShift = 1000.0f; //Maximum speed when holdin gshift
    public float CameraSensitivity = 0.25f; //How sensitive it with mouse
    private Vector3 _lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float _totalRun = 1.0f;

    void Update()
    {
        _lastMouse = Input.mousePosition - _lastMouse;
        _lastMouse = new Vector3(-_lastMouse.y * CameraSensitivity, _lastMouse.x * CameraSensitivity, 0);
        _lastMouse = new Vector3(transform.eulerAngles.x + _lastMouse.x, transform.eulerAngles.y + _lastMouse.y, 0);
        transform.eulerAngles = _lastMouse;
        _lastMouse = Input.mousePosition;
        //Mouse  camera angle done.  

        //Keyboard commands
        Vector3 keysPressed = GetBaseInput();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _totalRun += Time.deltaTime;
            keysPressed = keysPressed * _totalRun * ShiftAdd;
            keysPressed.x = Mathf.Clamp(keysPressed.x, -MaxShift, MaxShift);
            keysPressed.y = Mathf.Clamp(keysPressed.y, -MaxShift, MaxShift);
            keysPressed.z = Mathf.Clamp(keysPressed.z, -MaxShift, MaxShift);
        }
        else
        {
            _totalRun = Mathf.Clamp(_totalRun * 0.5f, 1f, 1000f);
            keysPressed = keysPressed * MainSpeed;
        }

        keysPressed = keysPressed * Time.deltaTime;
        Vector3 newPosition = transform.position;

        //If player wants to move on X and Z axis only
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(keysPressed);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else
        {
            transform.Translate(keysPressed);
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 keysPressed = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            keysPressed += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            keysPressed += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            keysPressed += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            keysPressed += new Vector3(1, 0, 0);
        }
        return keysPressed;
    }
}