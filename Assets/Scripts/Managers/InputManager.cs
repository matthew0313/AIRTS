using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;

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
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        return new Vector3(moveX, 0, moveZ).normalized;
    }
    public bool CamInput(out float rotateValue)
    {
        rotateValue = Input.GetAxis("Mouse X");
        return Input.GetMouseButton(1);
    }
}