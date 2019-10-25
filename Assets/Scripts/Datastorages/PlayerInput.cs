// GENERATED AUTOMATICALLY FROM 'Assets/Settings/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace EoE.Controlls
{
    public class PlayerInput : IInputActionCollection, IDisposable
    {
        private InputActionAsset asset;
        public PlayerInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""GameInput"",
            ""id"": ""86df98da-af13-484a-b9c9-5b20d459d5c2"",
            ""actions"": [
                {
                    ""name"": ""PlayerMove"",
                    ""type"": ""Value"",
                    ""id"": ""94e8b9f1-57f0-469a-aefe-2a877f3e6fce"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraMove"",
                    ""type"": ""Value"",
                    ""id"": ""c3976377-b761-48a1-8e8f-fea9f1eb9c8b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""StickDeadzone(min=0.3,max=1)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""0d2e0652-1a00-4a7f-b41b-3e8b3bb02c25"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dodge"",
                    ""type"": ""Button"",
                    ""id"": ""3482a3a9-ec6b-47ea-a741-9a5e00d5cda6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NormalAttack"",
                    ""type"": ""Button"",
                    ""id"": ""8dcad126-ae7e-4776-b73d-d73200fa8d8f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HeavyAttack"",
                    ""type"": ""Button"",
                    ""id"": ""8784c058-5deb-4b39-ae00-bf4aa6c9731e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""5c92c7c4-4933-483f-94f7-090ea0557dd9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ResetCamera"",
                    ""type"": ""Button"",
                    ""id"": ""d379406d-45e6-45cf-ad88-8c35a34d1ff9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""2cd7d6ec-5b58-4584-b672-fcb7a166168c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Block"",
                    ""type"": ""Button"",
                    ""id"": ""fdac22e1-d615-4a09-8a63-836ce3e19d36"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""8cea9377-d5ee-40ee-b827-b7d0d516ccae"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UseItem"",
                    ""type"": ""Button"",
                    ""id"": ""e8d9a16f-3a49-442f-ab0c-dbcb89a3dbc8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PhysicalMagicSwap"",
                    ""type"": ""Button"",
                    ""id"": ""e8dd7e70-0b0e-40f5-99bc-83374ae0d9b5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MagicScrollUp"",
                    ""type"": ""Button"",
                    ""id"": ""044f7c5e-9158-4d68-95a5-e667b97ffec4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MagicScrollDown"",
                    ""type"": ""Button"",
                    ""id"": ""af06ea1e-14b9-41bf-aeef-358803166a13"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ItemScrollUp"",
                    ""type"": ""Button"",
                    ""id"": ""d8c9a449-2a9b-4382-a878-df7952ec5140"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ItemScrollDown"",
                    ""type"": ""Button"",
                    ""id"": ""c1895aef-3fa9-41d1-b91d-3c5a00d05347"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""cc43d195-2008-4e25-9e1e-901dbe73120b"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false),StickDeadzone(min=0.3,max=1)"",
                    ""groups"": """",
                    ""action"": ""PlayerMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""6f06002d-3cb8-4038-a8a0-f2deea9c342b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""706fec50-4bd2-4209-b4ab-7a953b650651"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f97d6ab8-9060-4e57-a030-82240767c9a2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2ea9ce6d-d66e-479b-bb84-62e6faff6afc"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d7d7fdce-3174-47d7-833f-9279d0dd8cd4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c91643c6-ce9c-4b28-9bc7-beb1443298a2"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6ded021c-cfe3-48bc-8a22-f1d4b8de3400"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eecfe70b-5e41-4ebd-9ab9-553938db5bfb"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""17f48b0a-e4d4-42e1-8e44-81ce09d04395"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NormalAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1bd5e962-3507-4bb5-b553-24068850b2c6"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HeavyAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d08335c-0a74-49b0-a381-36f77b727e2d"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8ee8ec51-d7c7-45b5-b7d6-7ade6f305e56"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""39503915-8bcf-4240-932a-ec370410919c"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0237551f-f436-4926-b639-e052fc91493a"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c7d2d2f1-363b-4265-aec1-cae8a2bcc883"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""413f925e-a271-4c0c-b230-f356e10c65c4"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da23e232-45fe-4215-b96f-60b80437682f"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PhysicalMagicSwap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a33f71bf-1ae9-497d-95af-01a19c82fa94"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MagicScrollUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4897149c-a1de-472c-99de-abf3e56fa5bb"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MagicScrollDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e29be8ff-5488-4cbf-85fe-5d5b3e76fbef"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ItemScrollUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0dd94895-bef2-49e8-a56a-7dc080111be3"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ItemScrollDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // GameInput
            m_GameInput = asset.FindActionMap("GameInput", throwIfNotFound: true);
            m_GameInput_PlayerMove = m_GameInput.FindAction("PlayerMove", throwIfNotFound: true);
            m_GameInput_CameraMove = m_GameInput.FindAction("CameraMove", throwIfNotFound: true);
            m_GameInput_Jump = m_GameInput.FindAction("Jump", throwIfNotFound: true);
            m_GameInput_Dodge = m_GameInput.FindAction("Dodge", throwIfNotFound: true);
            m_GameInput_NormalAttack = m_GameInput.FindAction("NormalAttack", throwIfNotFound: true);
            m_GameInput_HeavyAttack = m_GameInput.FindAction("HeavyAttack", throwIfNotFound: true);
            m_GameInput_Run = m_GameInput.FindAction("Run", throwIfNotFound: true);
            m_GameInput_ResetCamera = m_GameInput.FindAction("ResetCamera", throwIfNotFound: true);
            m_GameInput_Aim = m_GameInput.FindAction("Aim", throwIfNotFound: true);
            m_GameInput_Block = m_GameInput.FindAction("Block", throwIfNotFound: true);
            m_GameInput_Pause = m_GameInput.FindAction("Pause", throwIfNotFound: true);
            m_GameInput_UseItem = m_GameInput.FindAction("UseItem", throwIfNotFound: true);
            m_GameInput_PhysicalMagicSwap = m_GameInput.FindAction("PhysicalMagicSwap", throwIfNotFound: true);
            m_GameInput_MagicScrollUp = m_GameInput.FindAction("MagicScrollUp", throwIfNotFound: true);
            m_GameInput_MagicScrollDown = m_GameInput.FindAction("MagicScrollDown", throwIfNotFound: true);
            m_GameInput_ItemScrollUp = m_GameInput.FindAction("ItemScrollUp", throwIfNotFound: true);
            m_GameInput_ItemScrollDown = m_GameInput.FindAction("ItemScrollDown", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // GameInput
        private readonly InputActionMap m_GameInput;
        private IGameInputActions m_GameInputActionsCallbackInterface;
        private readonly InputAction m_GameInput_PlayerMove;
        private readonly InputAction m_GameInput_CameraMove;
        private readonly InputAction m_GameInput_Jump;
        private readonly InputAction m_GameInput_Dodge;
        private readonly InputAction m_GameInput_NormalAttack;
        private readonly InputAction m_GameInput_HeavyAttack;
        private readonly InputAction m_GameInput_Run;
        private readonly InputAction m_GameInput_ResetCamera;
        private readonly InputAction m_GameInput_Aim;
        private readonly InputAction m_GameInput_Block;
        private readonly InputAction m_GameInput_Pause;
        private readonly InputAction m_GameInput_UseItem;
        private readonly InputAction m_GameInput_PhysicalMagicSwap;
        private readonly InputAction m_GameInput_MagicScrollUp;
        private readonly InputAction m_GameInput_MagicScrollDown;
        private readonly InputAction m_GameInput_ItemScrollUp;
        private readonly InputAction m_GameInput_ItemScrollDown;
        public struct GameInputActions
        {
            private PlayerInput m_Wrapper;
            public GameInputActions(PlayerInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @PlayerMove => m_Wrapper.m_GameInput_PlayerMove;
            public InputAction @CameraMove => m_Wrapper.m_GameInput_CameraMove;
            public InputAction @Jump => m_Wrapper.m_GameInput_Jump;
            public InputAction @Dodge => m_Wrapper.m_GameInput_Dodge;
            public InputAction @NormalAttack => m_Wrapper.m_GameInput_NormalAttack;
            public InputAction @HeavyAttack => m_Wrapper.m_GameInput_HeavyAttack;
            public InputAction @Run => m_Wrapper.m_GameInput_Run;
            public InputAction @ResetCamera => m_Wrapper.m_GameInput_ResetCamera;
            public InputAction @Aim => m_Wrapper.m_GameInput_Aim;
            public InputAction @Block => m_Wrapper.m_GameInput_Block;
            public InputAction @Pause => m_Wrapper.m_GameInput_Pause;
            public InputAction @UseItem => m_Wrapper.m_GameInput_UseItem;
            public InputAction @PhysicalMagicSwap => m_Wrapper.m_GameInput_PhysicalMagicSwap;
            public InputAction @MagicScrollUp => m_Wrapper.m_GameInput_MagicScrollUp;
            public InputAction @MagicScrollDown => m_Wrapper.m_GameInput_MagicScrollDown;
            public InputAction @ItemScrollUp => m_Wrapper.m_GameInput_ItemScrollUp;
            public InputAction @ItemScrollDown => m_Wrapper.m_GameInput_ItemScrollDown;
            public InputActionMap Get() { return m_Wrapper.m_GameInput; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameInputActions set) { return set.Get(); }
            public void SetCallbacks(IGameInputActions instance)
            {
                if (m_Wrapper.m_GameInputActionsCallbackInterface != null)
                {
                    PlayerMove.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnPlayerMove;
                    PlayerMove.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnPlayerMove;
                    PlayerMove.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnPlayerMove;
                    CameraMove.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnCameraMove;
                    CameraMove.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnCameraMove;
                    CameraMove.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnCameraMove;
                    Jump.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnJump;
                    Jump.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnJump;
                    Jump.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnJump;
                    Dodge.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnDodge;
                    Dodge.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnDodge;
                    Dodge.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnDodge;
                    NormalAttack.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnNormalAttack;
                    NormalAttack.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnNormalAttack;
                    NormalAttack.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnNormalAttack;
                    HeavyAttack.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnHeavyAttack;
                    HeavyAttack.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnHeavyAttack;
                    HeavyAttack.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnHeavyAttack;
                    Run.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnRun;
                    Run.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnRun;
                    Run.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnRun;
                    ResetCamera.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnResetCamera;
                    ResetCamera.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnResetCamera;
                    ResetCamera.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnResetCamera;
                    Aim.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnAim;
                    Aim.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnAim;
                    Aim.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnAim;
                    Block.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnBlock;
                    Block.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnBlock;
                    Block.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnBlock;
                    Pause.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnPause;
                    Pause.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnPause;
                    Pause.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnPause;
                    UseItem.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnUseItem;
                    UseItem.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnUseItem;
                    UseItem.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnUseItem;
                    PhysicalMagicSwap.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnPhysicalMagicSwap;
                    PhysicalMagicSwap.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnPhysicalMagicSwap;
                    PhysicalMagicSwap.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnPhysicalMagicSwap;
                    MagicScrollUp.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnMagicScrollUp;
                    MagicScrollUp.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnMagicScrollUp;
                    MagicScrollUp.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnMagicScrollUp;
                    MagicScrollDown.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnMagicScrollDown;
                    MagicScrollDown.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnMagicScrollDown;
                    MagicScrollDown.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnMagicScrollDown;
                    ItemScrollUp.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnItemScrollUp;
                    ItemScrollUp.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnItemScrollUp;
                    ItemScrollUp.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnItemScrollUp;
                    ItemScrollDown.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnItemScrollDown;
                    ItemScrollDown.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnItemScrollDown;
                    ItemScrollDown.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnItemScrollDown;
                }
                m_Wrapper.m_GameInputActionsCallbackInterface = instance;
                if (instance != null)
                {
                    PlayerMove.started += instance.OnPlayerMove;
                    PlayerMove.performed += instance.OnPlayerMove;
                    PlayerMove.canceled += instance.OnPlayerMove;
                    CameraMove.started += instance.OnCameraMove;
                    CameraMove.performed += instance.OnCameraMove;
                    CameraMove.canceled += instance.OnCameraMove;
                    Jump.started += instance.OnJump;
                    Jump.performed += instance.OnJump;
                    Jump.canceled += instance.OnJump;
                    Dodge.started += instance.OnDodge;
                    Dodge.performed += instance.OnDodge;
                    Dodge.canceled += instance.OnDodge;
                    NormalAttack.started += instance.OnNormalAttack;
                    NormalAttack.performed += instance.OnNormalAttack;
                    NormalAttack.canceled += instance.OnNormalAttack;
                    HeavyAttack.started += instance.OnHeavyAttack;
                    HeavyAttack.performed += instance.OnHeavyAttack;
                    HeavyAttack.canceled += instance.OnHeavyAttack;
                    Run.started += instance.OnRun;
                    Run.performed += instance.OnRun;
                    Run.canceled += instance.OnRun;
                    ResetCamera.started += instance.OnResetCamera;
                    ResetCamera.performed += instance.OnResetCamera;
                    ResetCamera.canceled += instance.OnResetCamera;
                    Aim.started += instance.OnAim;
                    Aim.performed += instance.OnAim;
                    Aim.canceled += instance.OnAim;
                    Block.started += instance.OnBlock;
                    Block.performed += instance.OnBlock;
                    Block.canceled += instance.OnBlock;
                    Pause.started += instance.OnPause;
                    Pause.performed += instance.OnPause;
                    Pause.canceled += instance.OnPause;
                    UseItem.started += instance.OnUseItem;
                    UseItem.performed += instance.OnUseItem;
                    UseItem.canceled += instance.OnUseItem;
                    PhysicalMagicSwap.started += instance.OnPhysicalMagicSwap;
                    PhysicalMagicSwap.performed += instance.OnPhysicalMagicSwap;
                    PhysicalMagicSwap.canceled += instance.OnPhysicalMagicSwap;
                    MagicScrollUp.started += instance.OnMagicScrollUp;
                    MagicScrollUp.performed += instance.OnMagicScrollUp;
                    MagicScrollUp.canceled += instance.OnMagicScrollUp;
                    MagicScrollDown.started += instance.OnMagicScrollDown;
                    MagicScrollDown.performed += instance.OnMagicScrollDown;
                    MagicScrollDown.canceled += instance.OnMagicScrollDown;
                    ItemScrollUp.started += instance.OnItemScrollUp;
                    ItemScrollUp.performed += instance.OnItemScrollUp;
                    ItemScrollUp.canceled += instance.OnItemScrollUp;
                    ItemScrollDown.started += instance.OnItemScrollDown;
                    ItemScrollDown.performed += instance.OnItemScrollDown;
                    ItemScrollDown.canceled += instance.OnItemScrollDown;
                }
            }
        }
        public GameInputActions @GameInput => new GameInputActions(this);
        public interface IGameInputActions
        {
            void OnPlayerMove(InputAction.CallbackContext context);
            void OnCameraMove(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnDodge(InputAction.CallbackContext context);
            void OnNormalAttack(InputAction.CallbackContext context);
            void OnHeavyAttack(InputAction.CallbackContext context);
            void OnRun(InputAction.CallbackContext context);
            void OnResetCamera(InputAction.CallbackContext context);
            void OnAim(InputAction.CallbackContext context);
            void OnBlock(InputAction.CallbackContext context);
            void OnPause(InputAction.CallbackContext context);
            void OnUseItem(InputAction.CallbackContext context);
            void OnPhysicalMagicSwap(InputAction.CallbackContext context);
            void OnMagicScrollUp(InputAction.CallbackContext context);
            void OnMagicScrollDown(InputAction.CallbackContext context);
            void OnItemScrollUp(InputAction.CallbackContext context);
            void OnItemScrollDown(InputAction.CallbackContext context);
        }
    }
}
