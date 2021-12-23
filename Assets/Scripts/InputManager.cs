using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool isInputEnabled;

    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float swipeThreshold = 20f;

    private static InputManager instance;
    public static InputManager Instance
    {
        get => instance;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(isInputEnabled == false)
        {
            return;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GridManager.Instance.TryRotate(RotateType.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GridManager.Instance.TryRotate(RotateType.Right);
        }
#else
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                CheckSwipe();
            }
        }
#endif
    }

    void CheckSwipe()
    {
        if (VerticalMove() > swipeThreshold && VerticalMove() > HorizontalMove())
        {
            if (fingerDown.y - fingerUp.y > 0)
            {
                OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)
            {
                OnSwipeDown();
            }
            fingerUp = fingerDown;
        }

        else if (HorizontalMove() > swipeThreshold && HorizontalMove() > VerticalMove())
        {
            if (fingerDown.x - fingerUp.x > 0)
            {
                OnSwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0)
            {
                OnSwipeLeft();
            }
            fingerUp = fingerDown;
        }
    }

    float VerticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float HorizontalMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    void OnSwipeUp()
    {
        GridManager.Instance.TryRotate(RotateType.Left);
    }

    void OnSwipeDown()
    {
        GridManager.Instance.TryRotate(RotateType.Right);
    }

    void OnSwipeLeft()
    {
        GridManager.Instance.TryRotate(RotateType.Left);
    }

    void OnSwipeRight()
    {
        GridManager.Instance.TryRotate(RotateType.Right);
    }
}

