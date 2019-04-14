using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform character;

    public float heigth = 25;
    public float distance = 20;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update() {
        transform.position = new Vector3(character.position.x, character.position.y + heigth, character.position.z + distance);
        transform.LookAt(character);
	}
}
