using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Sirenix.OdinInspector;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [ReadOnly] public Vector2 Pos;
    [ReadOnly] public bool Press;

    public Action<Vector2> OnPressStart;
    public Action<Vector2> OnPressEnd;
    public Action<Vector2> OnPosition;

    private InputAction _pressAction;
    private InputAction _positionAction;

    void Awake()
    {
        if (_playerInput == null)
            _playerInput = GetComponent<PlayerInput>();

        var actionMap = _playerInput.actions.FindActionMap("Game");
        _pressAction = actionMap.FindAction("Press");
        _positionAction = actionMap.FindAction("Position");
    }
    void Update()
    {
        if (Press)
        {
            Pos = _positionAction.ReadValue<Vector2>();
            OnPosition?.Invoke(Pos);
        }
    }


    void OnEnable()
    {
        _pressAction.performed += HandlePressStarted;
        _pressAction.canceled += HandlePressCanceled;
        _positionAction.performed += HandlePositionChanged;

        _pressAction.Enable();
        _positionAction.Enable();
    }

    void OnDisable()
    {
        _pressAction.performed -= HandlePressStarted;
        _pressAction.canceled -= HandlePressCanceled;
        _positionAction.performed -= HandlePositionChanged;

        _pressAction.Disable();
        _positionAction.Disable();
    }
    private void HandlePressStarted(InputAction.CallbackContext ctx)
    {
        Press = true;
        Pos = _positionAction.ReadValue<Vector2>();
        OnPressStart?.Invoke(Pos);
    }
    private void HandlePressCanceled(InputAction.CallbackContext ctx)
    {
        Press = false;
        Pos = _positionAction.ReadValue<Vector2>();
        OnPressEnd?.Invoke(Pos);
    }

    private void HandlePositionChanged(InputAction.CallbackContext ctx)
    {
        if (Press)
        {
            Pos = ctx.ReadValue<Vector2>();
        }
    }
}
