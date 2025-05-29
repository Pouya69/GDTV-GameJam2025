using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public InputAction GoBackAction;

    private void Start()
    {
        GoBackAction = InputSystem.actions.FindAction("Cancel");
        GoBackAction.Enable();
        GoBackAction.performed += GoBackAction_performed;
    }

    private void GoBackAction_performed(InputAction.CallbackContext obj)
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    private void OnDisable()
    {
        GoBackAction.Disable();
        GoBackAction.performed -= GoBackAction_performed;
    }
}
