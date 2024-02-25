using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public bool tutorialButton;
    public TabGroup tabGroup;
    public Image background;
    [SerializeField] private bool backgroundAsOutline = false;

    [ShowIf("backgroundAsOutline")] [SerializeField]
    private GameObject backgroundOutline;

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
        if (backgroundAsOutline)
            backgroundOutline.SetActive(true);
    }

    public void EmulateOnClick()
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    public void DisableSelectionOutline()
    {
        if (backgroundAsOutline)
            backgroundOutline.SetActive(false);
    }

    void Start()
    {
        background = GetComponent<Image>();
    }
}