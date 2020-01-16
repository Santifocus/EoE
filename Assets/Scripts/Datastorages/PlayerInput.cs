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
                    ""processors"": """",
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
                    ""name"": ""UseItem"",
                    ""type"": ""Button"",
                    ""id"": ""e8d9a16f-3a49-442f-ab0c-dbcb89a3dbc8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MagicCast"",
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
                    ""processors"": ""InvertVector2(invertX=false,invertY=false),StickDeadzone(min=0.3,max=1)"",
                    ""groups"": """",
                    ""action"": ""PlayerMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c91643c6-ce9c-4b28-9bc7-beb1443298a2"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.3,max=1)"",
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
                    ""path"": ""<Gamepad>/leftTrigger"",
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
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Block"",
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
                    ""action"": ""MagicCast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a33f71bf-1ae9-497d-95af-01a19c82fa94"",
                    ""path"": ""<Gamepad>/dpad/up"",
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
                    ""path"": ""<Gamepad>/dpad/down"",
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
                    ""path"": ""<Gamepad>/dpad/right"",
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
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ItemScrollDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MenuInput"",
            ""id"": ""216672cd-8ca6-4e21-be29-8cbd352a2b8f"",
            ""actions"": [
                {
                    ""name"": ""Right"",
                    ""type"": ""Value"",
                    ""id"": ""064ad7d0-8b18-4a88-97d4-316de4cc46b4"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Left"",
                    ""type"": ""Value"",
                    ""id"": ""1a2b1097-93b8-423a-aa7a-85bf3f43c9ef"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Up"",
                    ""type"": ""Value"",
                    ""id"": ""ab9be2b8-2408-45a9-8a89-b75fd444b359"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Value"",
                    ""id"": ""b3e31f01-579b-403d-8882-e2e5c371d702"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Enter"",
                    ""type"": ""Value"",
                    ""id"": ""d71f096e-2858-47fb-b8e7-cd4b8792d127"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Value"",
                    ""id"": ""4938438f-107e-46a6-b5a0-ae57eaa6753d"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Value"",
                    ""id"": ""1a05c78e-d8e7-4c0f-94d2-2c02dbb83f71"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PlayerMenu"",
                    ""type"": ""Value"",
                    ""id"": ""318b1506-d1f1-4a08-9850-970c198fd8ee"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftPage"",
                    ""type"": ""Button"",
                    ""id"": ""6c38f06c-162d-4d97-a2ed-62e058906748"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightPage"",
                    ""type"": ""Button"",
                    ""id"": ""2fec7d5f-c0f8-46db-8af3-f50acd41c31c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""41349519-dffe-44eb-93c1-328fc00178a2"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc8bb6ba-cb1c-47f7-b380-71d22350e29b"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79e74484-0cf7-4249-a4f7-63ecee5eae3c"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09ca9158-c8b7-4b25-8a2e-033d5e21a238"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b4c94de-ec64-4796-9a36-62ae93e14055"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9355c44d-f0bf-4ad5-a3b4-0dd181715f51"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""61acd399-067c-4e51-887d-dd3f1c84f992"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2337a462-b4a7-420c-ba1d-b8c5856b7659"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b50d825-11da-454f-9cca-ce3983ea6d0e"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""25a2e37f-48ef-449d-a0fc-dde275dfcd83"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e3801c94-d11c-4a5e-bba5-10177b178a5d"",
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
                    ""id"": ""2c62c75c-d0e7-4dc1-b89b-1997bc433a84"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlayerMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""70dd2111-476b-4588-8b78-c5479e7c1e21"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftPage"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dc7ba5f0-401d-4a75-8926-ae71065427f8"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightPage"",
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
            m_GameInput_UseItem = m_GameInput.FindAction("UseItem", throwIfNotFound: true);
            m_GameInput_MagicCast = m_GameInput.FindAction("MagicCast", throwIfNotFound: true);
            m_GameInput_MagicScrollUp = m_GameInput.FindAction("MagicScrollUp", throwIfNotFound: true);
            m_GameInput_MagicScrollDown = m_GameInput.FindAction("MagicScrollDown", throwIfNotFound: true);
            m_GameInput_ItemScrollUp = m_GameInput.FindAction("ItemScrollUp", throwIfNotFound: true);
            m_GameInput_ItemScrollDown = m_GameInput.FindAction("ItemScrollDown", throwIfNotFound: true);
            // MenuInput
            m_MenuInput = asset.FindActionMap("MenuInput", throwIfNotFound: true);
            m_MenuInput_Right = m_MenuInput.FindAction("Right", throwIfNotFound: true);
            m_MenuInput_Left = m_MenuInput.FindAction("Left", throwIfNotFound: true);
            m_MenuInput_Up = m_MenuInput.FindAction("Up", throwIfNotFound: true);
            m_MenuInput_Down = m_MenuInput.FindAction("Down", throwIfNotFound: true);
            m_MenuInput_Enter = m_MenuInput.FindAction("Enter", throwIfNotFound: true);
            m_MenuInput_Back = m_MenuInput.FindAction("Back", throwIfNotFound: true);
            m_MenuInput_Pause = m_MenuInput.FindAction("Pause", throwIfNotFound: true);
            m_MenuInput_PlayerMenu = m_MenuInput.FindAction("PlayerMenu", throwIfNotFound: true);
            m_MenuInput_LeftPage = m_MenuInput.FindAction("LeftPage", throwIfNotFound: true);
            m_MenuInput_RightPage = m_MenuInput.FindAction("RightPage", throwIfNotFound: true);
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
        private readonly InputAction m_GameInput_UseItem;
        private readonly InputAction m_GameInput_MagicCast;
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
            public InputAction @UseItem => m_Wrapper.m_GameInput_UseItem;
            public InputAction @MagicCast => m_Wrapper.m_GameInput_MagicCast;
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
                    UseItem.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnUseItem;
                    UseItem.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnUseItem;
                    UseItem.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnUseItem;
                    MagicCast.started -= m_Wrapper.m_GameInputActionsCallbackInterface.OnMagicCast;
                    MagicCast.performed -= m_Wrapper.m_GameInputActionsCallbackInterface.OnMagicCast;
                    MagicCast.canceled -= m_Wrapper.m_GameInputActionsCallbackInterface.OnMagicCast;
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
                    UseItem.started += instance.OnUseItem;
                    UseItem.performed += instance.OnUseItem;
                    UseItem.canceled += instance.OnUseItem;
                    MagicCast.started += instance.OnMagicCast;
                    MagicCast.performed += instance.OnMagicCast;
                    MagicCast.canceled += instance.OnMagicCast;
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

        // MenuInput
        private readonly InputActionMap m_MenuInput;
        private IMenuInputActions m_MenuInputActionsCallbackInterface;
        private readonly InputAction m_MenuInput_Right;
        private readonly InputAction m_MenuInput_Left;
        private readonly InputAction m_MenuInput_Up;
        private readonly InputAction m_MenuInput_Down;
        private readonly InputAction m_MenuInput_Enter;
        private readonly InputAction m_MenuInput_Back;
        private readonly InputAction m_MenuInput_Pause;
        private readonly InputAction m_MenuInput_PlayerMenu;
        private readonly InputAction m_MenuInput_LeftPage;
        private readonly InputAction m_MenuInput_RightPage;
        public struct MenuInputActions
        {
            private PlayerInput m_Wrapper;
            public MenuInputActions(PlayerInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Right => m_Wrapper.m_MenuInput_Right;
            public InputAction @Left => m_Wrapper.m_MenuInput_Left;
            public InputAction @Up => m_Wrapper.m_MenuInput_Up;
            public InputAction @Down => m_Wrapper.m_MenuInput_Down;
            public InputAction @Enter => m_Wrapper.m_MenuInput_Enter;
            public InputAction @Back => m_Wrapper.m_MenuInput_Back;
            public InputAction @Pause => m_Wrapper.m_MenuInput_Pause;
            public InputAction @PlayerMenu => m_Wrapper.m_MenuInput_PlayerMenu;
            public InputAction @LeftPage => m_Wrapper.m_MenuInput_LeftPage;
            public InputAction @RightPage => m_Wrapper.m_MenuInput_RightPage;
            public InputActionMap Get() { return m_Wrapper.m_MenuInput; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MenuInputActions set) { return set.Get(); }
            public void SetCallbacks(IMenuInputActions instance)
            {
                if (m_Wrapper.m_MenuInputActionsCallbackInterface != null)
                {
                    Right.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnRight;
                    Right.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnRight;
                    Right.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnRight;
                    Left.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnLeft;
                    Left.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnLeft;
                    Left.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnLeft;
                    Up.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnUp;
                    Up.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnUp;
                    Up.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnUp;
                    Down.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnDown;
                    Down.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnDown;
                    Down.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnDown;
                    Enter.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnEnter;
                    Enter.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnEnter;
                    Enter.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnEnter;
                    Back.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnBack;
                    Back.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnBack;
                    Back.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnBack;
                    Pause.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnPause;
                    Pause.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnPause;
                    Pause.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnPause;
                    PlayerMenu.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnPlayerMenu;
                    PlayerMenu.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnPlayerMenu;
                    PlayerMenu.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnPlayerMenu;
                    LeftPage.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnLeftPage;
                    LeftPage.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnLeftPage;
                    LeftPage.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnLeftPage;
                    RightPage.started -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnRightPage;
                    RightPage.performed -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnRightPage;
                    RightPage.canceled -= m_Wrapper.m_MenuInputActionsCallbackInterface.OnRightPage;
                }
                m_Wrapper.m_MenuInputActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Right.started += instance.OnRight;
                    Right.performed += instance.OnRight;
                    Right.canceled += instance.OnRight;
                    Left.started += instance.OnLeft;
                    Left.performed += instance.OnLeft;
                    Left.canceled += instance.OnLeft;
                    Up.started += instance.OnUp;
                    Up.performed += instance.OnUp;
                    Up.canceled += instance.OnUp;
                    Down.started += instance.OnDown;
                    Down.performed += instance.OnDown;
                    Down.canceled += instance.OnDown;
                    Enter.started += instance.OnEnter;
                    Enter.performed += instance.OnEnter;
                    Enter.canceled += instance.OnEnter;
                    Back.started += instance.OnBack;
                    Back.performed += instance.OnBack;
                    Back.canceled += instance.OnBack;
                    Pause.started += instance.OnPause;
                    Pause.performed += instance.OnPause;
                    Pause.canceled += instance.OnPause;
                    PlayerMenu.started += instance.OnPlayerMenu;
                    PlayerMenu.performed += instance.OnPlayerMenu;
                    PlayerMenu.canceled += instance.OnPlayerMenu;
                    LeftPage.started += instance.OnLeftPage;
                    LeftPage.performed += instance.OnLeftPage;
                    LeftPage.canceled += instance.OnLeftPage;
                    RightPage.started += instance.OnRightPage;
                    RightPage.performed += instance.OnRightPage;
                    RightPage.canceled += instance.OnRightPage;
                }
            }
        }
        public MenuInputActions @MenuInput => new MenuInputActions(this);
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
            void OnUseItem(InputAction.CallbackContext context);
            void OnMagicCast(InputAction.CallbackContext context);
            void OnMagicScrollUp(InputAction.CallbackContext context);
            void OnMagicScrollDown(InputAction.CallbackContext context);
            void OnItemScrollUp(InputAction.CallbackContext context);
            void OnItemScrollDown(InputAction.CallbackContext context);
        }
        public interface IMenuInputActions
        {
            void OnRight(InputAction.CallbackContext context);
            void OnLeft(InputAction.CallbackContext context);
            void OnUp(InputAction.CallbackContext context);
            void OnDown(InputAction.CallbackContext context);
            void OnEnter(InputAction.CallbackContext context);
            void OnBack(InputAction.CallbackContext context);
            void OnPause(InputAction.CallbackContext context);
            void OnPlayerMenu(InputAction.CallbackContext context);
            void OnLeftPage(InputAction.CallbackContext context);
            void OnRightPage(InputAction.CallbackContext context);
        }
    }
}
