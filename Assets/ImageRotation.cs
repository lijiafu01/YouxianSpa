using UnityEngine;
using UnityEngine.UI;

public class ImageRotation : MonoBehaviour
{
    public float rotationSpeed = 30f; // T?c ?? xoay

    private RectTransform imageRectTransform;

    private void Start()
    {
        // L?y th?nh ph?n RectTransform c?a h?nh ?nh
        imageRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Xoay h?nh ?nh theo th?i gian
        RotateImage();
    }

    private void RotateImage()
    {
        // Xoay h?nh ?nh theo t?c ?? v? th?i gian
        imageRectTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
