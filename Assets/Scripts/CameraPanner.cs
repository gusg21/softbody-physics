using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition; // you can set the Z on the same line
        mousePos.z = 0;
        Vector3 screenSize = Screen.safeArea.size;
        Vector3 movement = Vector3.zero;
        var paddingPixels = 70; // const
        var movementSpeed = 8f; // const

        if (mousePos.x < paddingPixels)
            movement.x = -1;
        if (mousePos.x > screenSize.x - paddingPixels)
            movement.x = 1;
        if (mousePos.y < paddingPixels)
            movement.y = -1;
        if (mousePos.y > screenSize.y - paddingPixels)
            movement.y = 1;

        // you should use brackets with your ifs

        transform.position += movement * movementSpeed * Time.deltaTime; // good and efficient
    }
}
