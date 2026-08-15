using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image completionIcon;
    [SerializeField] private Sprite completeIcon;
    [SerializeField] private Sprite incompleteIcon;
    [SerializeField] private Image background;
    [SerializeField] private Color completeColor = new Color(0.8f, 1f, 0.8f); //green
    [SerializeField] private Color incompleteColor;

    public void Bind(Objective objective, bool completed) 
    {
        title.text = objective.title;
        description.text = objective.description;
        completionIcon.sprite = completed ? completeIcon : incompleteIcon;
        background.color = completed ? completeColor : incompleteColor;
    }
}
