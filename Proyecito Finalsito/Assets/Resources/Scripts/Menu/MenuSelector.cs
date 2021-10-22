using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class MenuSelector : MonoBehaviour
{
    [SerializeField] private List<Transform> buttonPosition = new List<Transform>();
    private int indx = 0;

    private InputMap input;

    [SerializeField] private UnityEvent OnSelectorChange;

    private MenuButton currentButton;

    private void Awake() {
        input = new InputMap();
    }

    private void OnEnable() {
        input.Menu.MoveButtonCursor.performed += OnMoveCursor;
        input.Menu.TriggerButton.performed += OnTriggerButton;

        input.Menu.Enable();
    }

    private void OnDisable() {
        input.Menu.MoveButtonCursor.performed -= OnMoveCursor;
        input.Menu.TriggerButton.performed -= OnTriggerButton;

        input.Menu.Disable();
    }

    /// <summary>
    /// Moves the button cursor depending on your input performed. <br/>
    /// The value is readed from a 1D Axis (float) from the InputMap asset 
    /// </summary>
    private void OnMoveCursor(InputAction.CallbackContext context)
    {
        // Read the input value that determines the selector direction
        var direction = context.ReadValue<float>();
        switch (direction) {
            case 1: IncrementIndex(); break;
            case -1: DecrementIndex(); break;
        }

        // Assign my new position
        transform.position = buttonPosition[indx].position;

        // Call my selector changed event
        OnSelectorChange?.Invoke();
        AudioManager.current.Play("UI", "Buttons", "ScrollButton");
    }

    private void IncrementIndex()
    {
        if (indx == buttonPosition.Count - 1)
            return;

        indx++;
    }

    private void DecrementIndex()
    {
        if (indx == 0)
            return;
        
        indx--;
    }

    /// <summary> Trigger the current button by calling it's event </summary>
    private void OnTriggerButton(InputAction.CallbackContext context) {
        currentButton.OnTrigger?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("MenuItem"))
            return;

        currentButton = other.GetComponent<MenuButton>();

        currentButton.OnHover?.Invoke();
    }
}
