using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] private Image m_HeartFill;
    [SerializeField] private CanvasGroup m_HeartGroup;
    [SerializeField] private TMP_Text m_ResultsText;

    void Awake()
    {
        m_ResultsText.text = "";
        m_HeartGroup.alpha = 0.0f;
    }

    void Start()
    {
        GameManager.instance.StopBGM();
        StartCoroutine(ShowResults());
    }

    IEnumerator ShowResults()
    {
        // Show heart
        m_HeartFill.fillAmount = Mathf.Clamp((float)GameManager.instance.m_HappyLevel / (float)10.0f, 0.0f, 1.0f);
        
        yield return new WaitForSeconds(0.5f);

        float t = 0.0f;
        float fadeDur = 2.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / fadeDur;
            m_HeartGroup.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Type results
        m_ResultsText.text = "";
        string sentence = GetResultText();
        float textSpeed = 0.88f;

        for(int i = 0; i < sentence.ToCharArray().Length + 1; i++)
        {
            m_ResultsText.text = sentence.Substring(0, i);
            m_ResultsText.text += "<color=#00000000>" + sentence.Substring(i) + "</color>";
            
            yield return new WaitForSeconds(Mathf.Clamp(1.0f - textSpeed, 0.01f, 1.0f));
        }
        yield return null;
    }

    private string GetResultText()
    {
        string results = null;
        if(m_HeartFill.fillAmount >= 1.0f)
        {
            // S
            results = "From the bottom of my heart, thank you for your service.\n\nYou truly are a good person.";
        }
        else if(m_HeartFill.fillAmount >= 0.8f)
        {
            // A
            results = "You've brought so many smiles. We're truly blessed to have you.\n\nThank you.";
        }
        else if(m_HeartFill.fillAmount >= 0.5f)
        {
            // B
            results = "You have done great work for those around you. You are a kind soul.\n\nThank you.";
        }
        else if(m_HeartFill.fillAmount >= 0.2f)
        {
            // C
            results = "Sometimes, even just trying is enough.\n\nThank you for your efforts.";
        }
        else if(m_HeartFill.fillAmount < 0.1f)
        {
            // D
            results = "Flowers grow even after the harshest of winters.\n\nMay your spring come soon.";
        }

        return results;
    }
}