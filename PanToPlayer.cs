using System.Numerics;
using UnityEngine;

public class PanToPlayer : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCamera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void LateUpdate()
    {
        //Get camera position
        UnityEngine.Vector3 cameraPosition
            = _mainCamera.transform.position;
        
        //We only want to rotate on Y axis:
        cameraPosition.y
            = transform.position.y;
        //Make the sprite face the camera
        transform.LookAt(cameraPosition);
        //Rotate 180 on Y axis because of spriteRenderer works
        transform.Rotate(0f, 180f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
