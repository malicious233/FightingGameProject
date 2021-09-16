using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerManager playerManager;
    public StaticEnums.InputType inputType;

    float _horizontalInput;
    float _verticalInput;
    bool _downInput;
    Vector3 _inputDirection;
    bool _lightAttackPress;
    bool _heavyAttackPress;
    bool _jumpPress;

    public void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }
    public void Update()
    {
        switch (inputType)
        {
            case StaticEnums.InputType.None:
                break;
            case StaticEnums.InputType.WASD:
                InputType_WASD();
                break;
            case StaticEnums.InputType.Arrows:
                InputType_Arrows();
                break;
        }
        
    }


    private void InputType_WASD()
    {
        //Get Inputs
        _horizontalInput = Input.GetAxis("HorizontalWASD");
        _verticalInput = Input.GetAxis("VerticalWASD");
        _downInput = Input.GetKey(KeyCode.S);
        
        _inputDirection = new Vector3(_horizontalInput, _verticalInput, 0f);
        _inputDirection.Normalize();

        _jumpPress = Input.GetKeyDown(KeyCode.Space);

        _lightAttackPress = Input.GetKeyDown(KeyCode.J);
        _heavyAttackPress = Input.GetKeyDown(KeyCode.K);

        //Apply input to PlayerManager
        ApplyInputs();

    }

    private void InputType_Arrows()
    {
        _horizontalInput = Input.GetAxis("HorizontalArrows");
        _verticalInput = Input.GetAxis("VerticalArrows");
        _downInput = Input.GetKey(KeyCode.DownArrow);

        _jumpPress = Input.GetKeyDown(KeyCode.RightAlt);

        _lightAttackPress = Input.GetKeyDown(KeyCode.Comma);
        _heavyAttackPress = Input.GetKeyDown(KeyCode.Period);

        //Apply input to PlayerManager
        ApplyInputs();
    }
    
    private void ApplyInputs()
    {
        playerManager.horizontalInput = _horizontalInput;
        playerManager.lightAttackPress = _lightAttackPress;
        playerManager.heavyAttackPress = _heavyAttackPress;
        playerManager.jumpPress = _jumpPress;
        playerManager.downInput = _downInput;
    }

}
