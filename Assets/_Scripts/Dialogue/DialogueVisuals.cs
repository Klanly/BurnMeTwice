using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVisuals : MonoBehaviour
{
    //GameObjects of the two models
    [SerializeField]
    GameObject _dialogueVisuals, _leftCharacter, _rightCharacter;

    [SerializeField]
    List<GameObject> _listLeftCharacters, _listRightCharacters;

    Animator _leftAnimator, _rightAnimator;

    string _nameLeftCharacter, _nameRightCharacter;

    public enum Character
    {
        Orobas,
        Gaspar,
        Débora,
        UlricoDeDüstenburg,
        Fausto,
        Mara,
        Tristán
    }
    public Character character;

    private void Start()
    {
        _leftAnimator = _leftCharacter.GetComponent<Animator>();
        _rightAnimator = _rightCharacter.GetComponent<Animator>();
    }

    public void LoadCharacters(string leftCharacter, string rightCharacter)
    {
        _nameLeftCharacter = leftCharacter;
        _nameRightCharacter = rightCharacter;

        switch (rightCharacter)
        {
            default:
                Debug.LogError("Incorrect character nomenclature");
                break;
            case "Claude":
                _listRightCharacters[0].SetActive(true);
                break;
            case "Gaspar":
                _listRightCharacters[1].SetActive(true);
                break;
            case "Dr. Fausto":
                _listRightCharacters[2].SetActive(true);
                break;
            case "Vanessa":
                _listRightCharacters[3].SetActive(true);
                break;
            case "Lois":
                _listRightCharacters[4].SetActive(true);
                break;
            case "Owain":
                _listRightCharacters[5].SetActive(true);
                break;
            case "Ramona":
                _listRightCharacters[6].SetActive(true);
                break;
            case "Rick":
                _listRightCharacters[7].SetActive(true);
                break;
            case "Humberto":
                _listRightCharacters[8].SetActive(true);
                break;
            case "Ernesto":
                _listRightCharacters[9].SetActive(true);
                break;
        }

        switch (leftCharacter)
        {
            default:
                _listLeftCharacters[0].SetActive(true);
                break;
            case "Vivianne":
                _listLeftCharacters[0].SetActive(true);
                break;
        }
    }

    public void LoadAnimations(string broadcaster, string animationTrigger)
    {
        if (broadcaster == _nameLeftCharacter)
        {
            _leftAnimator.SetTrigger(animationTrigger);
        }
        else if (broadcaster == _nameRightCharacter)
        {
            _rightAnimator.SetTrigger(animationTrigger);
        }
    }

    public void ClearCharacters()
    {
        foreach (GameObject g in _listLeftCharacters)
        {
            if (g.activeInHierarchy == true)
            {
                g.SetActive(false);
                break;
            }
        }

        foreach (GameObject g in _listRightCharacters)
        {
            if (g.activeInHierarchy == true)
            {
                g.SetActive(false);
                break;
            }
        }

        _nameLeftCharacter = "";
        _nameRightCharacter = "";
    }
}
