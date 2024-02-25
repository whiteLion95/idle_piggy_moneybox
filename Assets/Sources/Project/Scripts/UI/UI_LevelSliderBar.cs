using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LevelSliderBar : MonoBehaviour
{
    private SliderBar _sliderBar;
    private ExpController _expController;

    public void Init(ExpController expController)
    {
        _sliderBar = GetComponent<SliderBar>();
        _expController = expController;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
