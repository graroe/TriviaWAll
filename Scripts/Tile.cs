using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    //config params
    [SerializeField] float moveSpeed = 0.5f;
    string answer;

    //State params
    Vector2 movementDestination;
    bool moving = false;

    //Cached references
    WallState wallState;

    void Start()
    {
        wallState = FindObjectOfType<WallState>();
        answer = null;
    }

    void Update()
    {
        Move();
    }
    public void ToggleOff()
    {
        GetComponent<Toggle>().isOn = false;
    }

    public void ToggleOn()
    {
        GetComponent<Toggle>().isOn = true;
    }
    public void SelectThisTile()
    {
        if (GetComponent<Toggle>().isOn) { wallState.AddTileToChosen(this); }
        else { wallState.RemoveTileFromChosen(this); }

    }

    public string getTag()
    {
        return tag;
    }

    public void Deactivate()
    {
        GetComponent<Toggle>().interactable = false;
    }
    public void SetClueText(string clue)
    {
        Text clueTextComponent = transform.GetComponentInChildren<Text>();
        clueTextComponent.text = clue;
    }

    public void SetAnswer(string answer)
    {
        this.answer = answer;
    }

    public string GetAnswer()
    {
        return this.answer;
    }

    public void DisableChildTags(Transform parent)
    {
        for (int x = 0; x < parent.childCount; x++)
        {
            Transform child = parent.GetChild(x);
            child.tag = "Untagged";
            DisableChildTags(child);
        }
    }
    public void MoveTo(Vector2 targetPos)
    {
        movementDestination = targetPos;
        moving = true;
    }
    public void Move()
    {
       if (moving)
        {
            var movementThisFrame = moveSpeed * Time.deltaTime;
            
            GetComponent<RectTransform>().anchoredPosition = 
            Vector2.MoveTowards(GetComponent<RectTransform>().anchoredPosition, 
            movementDestination, movementThisFrame);
            
            if (GetComponent<RectTransform>().anchoredPosition == movementDestination)
            { moving = false; }
       }
    }

}
