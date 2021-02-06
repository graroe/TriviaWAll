using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UIModule;


public class WallState : MonoBehaviour
{
    //Config Params
    [SerializeField] List<Tile> tileOrder;
    [SerializeField] Vector2[] gridPositions;
    [SerializeField] Color[] selectionColours;
    [SerializeField] List<Tile> chosenTiles;
    [SerializeField] Life[] lives;
    [SerializeField] GameObject instructionText;
    [SerializeField] GameObject[] answersText;

    //state variables
    int solvedRows;
    int totalTiles;
    int livesRemaining;
    int answersRevealed;
    bool livesActive = false;
    bool answersActive = false;
    String[] wallText;


    //cached references
    Timer timer;
    FileReader fileReader;

    // Start is called before the first frame update
    void Start()
    {
        solvedRows = 0;
        answersRevealed = 0;
        fileReader = FindObjectOfType<FileReader>();

        VerifyAndSetTotalTilesVariable();
        InitializeBoard();
        setSelectionColour();

        timer = FindObjectOfType<Timer>();

    }

    private void Update()
    {
        if (answersActive && Input.GetKeyDown(KeyCode.A))
        {
            RevealNextAnswer();
        }
    }

    private void VerifyAndSetTotalTilesVariable()
    {
        if (gridPositions.Length == tileOrder.Count())
        {
            totalTiles = gridPositions.Length;
        }
        else
        {
            Debug.LogError("Error: grid positions and tile order should both equal the total number of tiles");
        }
    }

    private void InitializeBoard()
    {
        //Assigns text
        wallText = fileReader.GetWallTextArray();
        for (int i = 0; i < totalTiles; i++)
        {
            Tile currentTile = tileOrder.ElementAt(i);
            currentTile.SetClueText(wallText[i]);
            if (i % 4 == 0)
            {
                currentTile.SetAnswer(wallText[i / 4 + totalTiles + 1]);
            }
        }

        //shuffles list
        tileOrder = tileOrder.OrderBy(i => Guid.NewGuid()).ToList();

        //instantly sends tiles to their new shuffled positions
        for (int i = 0; i < totalTiles; i++)
        {
            Tile currentTile = tileOrder.ElementAt(i);
            currentTile.GetComponent<RectTransform>().anchoredPosition = gridPositions[i];
        }

    }

    private void setSelectionColour()
    {
        GameObject[] activeOnStates = GameObject.FindGameObjectsWithTag("ActiveOnState");
        for (int i = 0; i < activeOnStates.Length; i++)
        {
            activeOnStates[i].GetComponent<UnityEngine.UI.Image>().color = selectionColours[solvedRows];
        }
    }

    

    private void EvaluateTileSelection()
    {
        if (GroupTagsMatch(chosenTiles))
        {
            GroupFound(chosenTiles);
        }
        else
        {
            ResetChosenTiles();
            if (livesActive) { loseOneLife(); }
        }
    }

    private bool GroupTagsMatch(List<Tile> chosenTiles)
    {
        Tile referenceTile = chosenTiles.ElementAt(0);
        string referenceTag = referenceTile.getTag();
        for (int i = 1; i < chosenTiles.Count(); i++)
        {
            if (!chosenTiles.ElementAt(i).getTag().Equals(referenceTag))
            {
                return false;
            }
        }
        return true;
    }

    private void GroupFound(List<Tile> chosenTiles)
    {
        RearrangeBoard(chosenTiles);
        chosenTiles.Clear();
        solvedRows++;
        setSelectionColour();

        if (solvedRows == 2)
        {
            activateLives();
        }

        if (solvedRows == 3)
        {
            FinishWall();
        }
    }

    private void ResetChosenTiles()
    {
        chosenTiles.Clear();
        for (int i = solvedRows * 4; i < tileOrder.Count(); i++)
        {
            tileOrder.ElementAt(i).ToggleOff();
        }

    }
    private void RearrangeBoard(List<Tile> chosenTiles)
    {
        foreach (Tile t in chosenTiles)
        {
            tileOrder.Remove(t);
            tileOrder.Insert(solvedRows * 4, t);
            t.DisableChildTags(t.gameObject.transform);
            t.tag = "Solved";
            t.Deactivate();
            NewTilePositionsAnimation();
        }
    }
   private void NewTilePositionsAnimation()
    {
        for (int i = 0; i < totalTiles; i++)
        {
            Tile currentTile = tileOrder.ElementAt(i);
            currentTile.MoveTo(gridPositions[i]);
        }
    }    
    private void FinishWall()
    {
        for (int i = totalTiles - 4; i < totalTiles; i++)
        {
            timer.FreezeTimer();
            Tile currentTile = tileOrder.ElementAt(i);
            currentTile.ToggleOn();
            currentTile.Deactivate();
        }
        StartCoroutine("ActivateAnswerPrompt");
       
    }

    private void activateLives()
    {
        livesActive = true;
        livesRemaining = lives.Length;
        foreach (Life l in lives)
        {
            l.fadeIn();
        }
    }

    private void loseOneLife()
    {
        livesRemaining--;
        Life currentLife = lives[livesRemaining];
        currentLife.fadeout();
        if (livesRemaining <= 0) { FreezeWall(); }
    }

    IEnumerator ActivateAnswerPrompt()
    {
        yield return new WaitForSeconds(1.5f);
        instructionText.GetComponent<TextMeshProUGUI>().text = "Press A to see the next Answer";
        answersActive = true;
    }

    private void RevealNextAnswer()
    {
        for (int i = answersRevealed * 4; i < answersRevealed * 4 + 4; i++)
        {
            Tile currentTile = tileOrder.ElementAt(i);
            if (currentTile.GetAnswer() != null)
            {
                answersText[answersRevealed].GetComponent<TextMeshProUGUI>().text = currentTile.GetAnswer();
                break;
            }
        }
        answersRevealed++;
    }
   
    public void AddTileToChosen(Tile tile)
    {
        chosenTiles.Add(tile);
        if (chosenTiles.Count() == 4)
        {
            EvaluateTileSelection();
        }
    }
    
    public void RemoveTileFromChosen(Tile tile)
    {
        chosenTiles.Remove(tile);
    }

    public void FreezeWall()
    {
        timer.FreezeTimer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        instructionText.GetComponent<TextMeshProUGUI>().text = "Wall Frozen! Press R to reveal solution or ESC to try again";
    }

    public void ResolveUnfinishedWall()
    {
        for (int i = solvedRows * 4; i < totalTiles; i++)
        {
            tileOrder.ElementAt(i).ToggleOff();
        }

        for (int i = solvedRows*4; i< totalTiles -4; i++)
        {
            Tile currentTile = tileOrder.ElementAt(i);
            String currentTag = currentTile.tag;
            if (currentTag != "Solved")
            {
                GameObject[] currentGroup = GameObject.FindGameObjectsWithTag(currentTag);
                foreach (GameObject groupedTile in currentGroup)
                {
                    Tile tile = groupedTile.GetComponent<Tile>();
                    tile.ToggleOn();
                    tile.Deactivate();
                }
            }
        }
    }

}