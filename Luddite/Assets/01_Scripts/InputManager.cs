using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public InputSystem_Actions InputSystem;

    [Space]
    [Header("Player Input")]
    [SerializeField] public Vector2 MoveInput;
    [SerializeField] public Vector2 CameraInput;
    [SerializeField] public bool AttackInput;
    [SerializeField] public bool InteractInput;
    [SerializeField] public bool CrouchInput;
    [SerializeField] public bool JumpInput;
    [SerializeField] public bool NextInput;
    [SerializeField] public bool PreviousInput;
    [SerializeField] public bool SprintInput;
    [SerializeField] public bool EscapeInput;
    [SerializeField] public bool InventoryInput;
    [SerializeField] public bool StatusInput;

    [Space]
    [Header("UI Input")]
    [SerializeField] public Vector2 NavigateInput;
    [SerializeField] public Vector2 MouseTraceInput;
    [SerializeField] public bool SubmitInput;
    [SerializeField] public bool CancelInput;
    [SerializeField] public Vector2 PointInput;
    [SerializeField] public bool LeftClickInput;
    [SerializeField] public bool RightClickInput;
    [SerializeField] public bool MiddleClickInput;
    [SerializeField] public Vector2 ScrollWheelInput;
    [SerializeField] public bool ContinueInput;




    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }

        //InputBinding();
    }

    private void Start()
    {

        InputBinding();
        PlayerInputBind();
    }

    private void InputBinding()
    {
        if (InputSystem == null) InputSystem = new InputSystem_Actions();

        InputSystem.Player.Move.performed += context => MoveInput = context.ReadValue<Vector2>();
        InputSystem.Player.Move.canceled += context => MoveInput = context.ReadValue<Vector2>();
        InputSystem.Player.Look.performed += context => CameraInput = context.ReadValue<Vector2>();
        // InputSystem.Player.Look.canceled += context => CameraInput = context.ReadValue<Vector2>();

        InputSystem.Player.Attack.started += context => AttackInput = true;
        InputSystem.Player.Attack.canceled += context => AttackInput = false;
        // InputSystem.Player.Attack.canceled += context => AttackInput = context.ReadValue<float>();
        InputSystem.Player.Interact.performed += context => InteractInput = true;
        InputSystem.Player.Interact.canceled += context => InteractInput = false;
        //InputSystem.Player.Cancel.performed += context => CrouchInput = true;
        //InputSystem.Player.Cancel.canceled += context => CrouchInput = false;
        InputSystem.Player.Jump.performed += context => JumpInput = true;
        InputSystem.Player.Next.performed += context => NextInput = true;
        InputSystem.Player.Next.canceled += context => NextInput = false;
        InputSystem.Player.Previous.performed += context => PreviousInput = true;
        InputSystem.Player.Previous.canceled += context => PreviousInput = false;
        InputSystem.Player.Sprint.performed += context => SprintInput = true;

        //InputSystem.Player.Pause.started += context => EscapeInput = true;
        //InputSystem.Player.Pause.canceled += context => EscapeInput = false;
        //InputSystem.Player.Inventory.started += context => InventoryInput = true;
        //InputSystem.Player.Inventory.canceled += context => InventoryInput = false;
        //InputSystem.Player.Status.started += context => StatusInput = true;
        //InputSystem.Player.Status.canceled += context => StatusInput = false;

        //[SerializeField] public Vector2 NavigateInput;
        //[SerializeField] public bool SubmitInput;
        //[SerializeField] public bool CancelInput;
        //[SerializeField] public Vector2 PointInput;
        //[SerializeField] public bool LeftClickInput;
        //[SerializeField] public bool RightClickInput;
        //[SerializeField] public bool MiddleClickInput;
        //[SerializeField] public Vector2 ScrollWheelInput;

        InputSystem.UI.Navigate.performed += context => NavigateInput = context.ReadValue<Vector2>();
        InputSystem.UI.Navigate.canceled += context => NavigateInput = context.ReadValue<Vector2>();
        //InputSystem.UI.MouseTrace.performed += context => MouseTraceInput = context.ReadValue<Vector2>();
        //InputSystem.UI.MouseTrace.canceled += context => MouseTraceInput = context.ReadValue<Vector2>();
        InputSystem.UI.Submit.started += context => SubmitInput = true;
        InputSystem.UI.Submit.canceled += context => SubmitInput = false;
        InputSystem.UI.Cancel.started += context => CancelInput = true;
        InputSystem.UI.Cancel.canceled += context => CancelInput = false;
        InputSystem.UI.Point.performed += context => PointInput = context.ReadValue<Vector2>();
        InputSystem.UI.Click.started += context => LeftClickInput = true;
        InputSystem.UI.Click.canceled += context => LeftClickInput = false;
        InputSystem.UI.RightClick.started += context => RightClickInput = true;
        InputSystem.UI.RightClick.canceled += context => RightClickInput = false;
        InputSystem.UI.MiddleClick.started += context => MiddleClickInput = true;
        InputSystem.UI.MiddleClick.canceled += context => MiddleClickInput = false;
        InputSystem.UI.ScrollWheel.performed += context => ScrollWheelInput = context.ReadValue<Vector2>();
        InputSystem.UI.ScrollWheel.canceled += context => ScrollWheelInput = context.ReadValue<Vector2>();
        //InputSystem.UI.Continue.started += context => ContinueInput = true;
        //InputSystem.UI.Continue.canceled += context => ContinueInput = false;



    }

    public void PlayerInputBind()
    {
        Cursor.lockState = CursorLockMode.Confined;
        InputSystem.UI.Disable();
        InputSystem.Player.Enable();
    }

    public void UIInputBind()
    {
        Cursor.lockState = CursorLockMode.None;
        InputSystem.Player.Disable();
        InputSystem.UI.Enable();

    }

    //private void Navigate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    //{
    //    throw new System.NotImplementedException();
    //}
}
