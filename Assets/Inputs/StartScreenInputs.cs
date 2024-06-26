//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.1
//     from Assets/GameManager/StartInputs.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @StartScreenInputs: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @StartScreenInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""StartInputs"",
    ""maps"": [
        {
            ""name"": ""SettingsScreen"",
            ""id"": ""9498d387-33a9-4e72-a830-d6f68ff08546"",
            ""actions"": [
                {
                    ""name"": ""SettingsScreen_Close"",
                    ""type"": ""Button"",
                    ""id"": ""657a44ce-5f3c-4046-8c95-1c9eca2269cc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""30cc4392-ba7d-4c45-8bf6-2a3bb002a8ea"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SettingsScreen_Close"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""267dfc3a-a1fd-46bd-b8d3-a90bced74568"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SettingsScreen_Close"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""StartScreen"",
            ""id"": ""8cb6f90d-1738-4ac8-b8ab-f5ca8bd7ac11"",
            ""actions"": [
                {
                    ""name"": ""Start_OpenSettings"",
                    ""type"": ""Button"",
                    ""id"": ""fd9be499-5f0d-4916-8836-75b09e16fb85"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e4be7e3b-899a-4e1b-9290-6e0617cde104"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start_OpenSettings"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // SettingsScreen
        m_SettingsScreen = asset.FindActionMap("SettingsScreen", throwIfNotFound: true);
        m_SettingsScreen_SettingsScreen_Close = m_SettingsScreen.FindAction("SettingsScreen_Close", throwIfNotFound: true);
        // StartScreen
        m_StartScreen = asset.FindActionMap("StartScreen", throwIfNotFound: true);
        m_StartScreen_Start_OpenSettings = m_StartScreen.FindAction("Start_OpenSettings", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // SettingsScreen
    private readonly InputActionMap m_SettingsScreen;
    private List<ISettingsScreenActions> m_SettingsScreenActionsCallbackInterfaces = new List<ISettingsScreenActions>();
    private readonly InputAction m_SettingsScreen_SettingsScreen_Close;
    public struct SettingsScreenActions
    {
        private @StartScreenInputs m_Wrapper;
        public SettingsScreenActions(@StartScreenInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @SettingsScreen_Close => m_Wrapper.m_SettingsScreen_SettingsScreen_Close;
        public InputActionMap Get() { return m_Wrapper.m_SettingsScreen; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SettingsScreenActions set) { return set.Get(); }
        public void AddCallbacks(ISettingsScreenActions instance)
        {
            if (instance == null || m_Wrapper.m_SettingsScreenActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_SettingsScreenActionsCallbackInterfaces.Add(instance);
            @SettingsScreen_Close.started += instance.OnSettingsScreen_Close;
            @SettingsScreen_Close.performed += instance.OnSettingsScreen_Close;
            @SettingsScreen_Close.canceled += instance.OnSettingsScreen_Close;
        }

        private void UnregisterCallbacks(ISettingsScreenActions instance)
        {
            @SettingsScreen_Close.started -= instance.OnSettingsScreen_Close;
            @SettingsScreen_Close.performed -= instance.OnSettingsScreen_Close;
            @SettingsScreen_Close.canceled -= instance.OnSettingsScreen_Close;
        }

        public void RemoveCallbacks(ISettingsScreenActions instance)
        {
            if (m_Wrapper.m_SettingsScreenActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ISettingsScreenActions instance)
        {
            foreach (var item in m_Wrapper.m_SettingsScreenActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_SettingsScreenActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public SettingsScreenActions @SettingsScreen => new SettingsScreenActions(this);

    // StartScreen
    private readonly InputActionMap m_StartScreen;
    private List<IStartScreenActions> m_StartScreenActionsCallbackInterfaces = new List<IStartScreenActions>();
    private readonly InputAction m_StartScreen_Start_OpenSettings;
    public struct StartScreenActions
    {
        private @StartScreenInputs m_Wrapper;
        public StartScreenActions(@StartScreenInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Start_OpenSettings => m_Wrapper.m_StartScreen_Start_OpenSettings;
        public InputActionMap Get() { return m_Wrapper.m_StartScreen; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(StartScreenActions set) { return set.Get(); }
        public void AddCallbacks(IStartScreenActions instance)
        {
            if (instance == null || m_Wrapper.m_StartScreenActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_StartScreenActionsCallbackInterfaces.Add(instance);
            @Start_OpenSettings.started += instance.OnStart_OpenSettings;
            @Start_OpenSettings.performed += instance.OnStart_OpenSettings;
            @Start_OpenSettings.canceled += instance.OnStart_OpenSettings;
        }

        private void UnregisterCallbacks(IStartScreenActions instance)
        {
            @Start_OpenSettings.started -= instance.OnStart_OpenSettings;
            @Start_OpenSettings.performed -= instance.OnStart_OpenSettings;
            @Start_OpenSettings.canceled -= instance.OnStart_OpenSettings;
        }

        public void RemoveCallbacks(IStartScreenActions instance)
        {
            if (m_Wrapper.m_StartScreenActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IStartScreenActions instance)
        {
            foreach (var item in m_Wrapper.m_StartScreenActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_StartScreenActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public StartScreenActions @StartScreen => new StartScreenActions(this);
    public interface ISettingsScreenActions
    {
        void OnSettingsScreen_Close(InputAction.CallbackContext context);
    }
    public interface IStartScreenActions
    {
        void OnStart_OpenSettings(InputAction.CallbackContext context);
    }
}
