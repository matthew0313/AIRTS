using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Instance = Instantiate(Resources.Load<InputManager>("InputManager"));
        DontDestroyOnLoad(Instance.gameObject);
    }
    public Vector3 MoveInput()
    {
        if(EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.TryGetComponent<TMP_InputField>(out _)) return Vector3.zero;
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        return new Vector3(moveX, 0, moveZ).normalized;
    }
    public bool CamInput(out Vector2 rotateValue)
    {
        rotateValue = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        return Input.GetMouseButton(1);
    }
}