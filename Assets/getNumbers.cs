using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getNumbers : MonoBehaviour
{
    public List<GameObject> numbers;
    public Transform numbersContainer;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in numbersContainer) if (child.CompareTag("Numbers")) {
            numbers.Add(child.gameObject);
        }
        Debug.Log("foooo " + numbers[0].name);
    }


}
