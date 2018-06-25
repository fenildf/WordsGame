using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler {

    public bool selected { get; private set; } = false;
    public char letter { get; private set; }

    public Pointer pointer { get; private set; }

    public void Init(char letter, Pointer pointer) {
        this.letter = letter;
        this.pointer = pointer;
        transform.Find("Text").GetComponent<Text>().text = letter.ToString().ToUpper();
        GetComponent<Image>().color = GamingManager.Instance.ItemDefaultColor;
    }

    public void SetDefault() {
        selected = false;
        GetComponent<Image>().color = GamingManager.Instance.ItemDefaultColor;
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("按下" + letter);
        GamingManager.Instance.StartDrag(this);
        selected = true;
        GetComponent<Image>().color = GamingManager.Instance.ItemSelectedColor;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        
        if (!GamingManager.Instance.Draging) {
            return;
        }
        Debug.Log("经过 " + letter);
        if (selected) {
            GamingManager.Instance.EndDrag();
        }
        else {
            selected = true;
            GetComponent<Image>().color = GamingManager.Instance.ItemSelectedColor;
            GamingManager.Instance.CoverDarg(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!GamingManager.Instance.Draging) {
            return;
        }
        Debug.Log("抬起 " + letter);
        GamingManager.Instance.EndDrag();
    }
}
