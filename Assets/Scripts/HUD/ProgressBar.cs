using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ProgressBar
{
    private VisualElement fill;
    private Label label;

    public ProgressBar(VisualElement root, StyleColor color)
    {
        fill = root.Children().First().Children().First().Children().First();
        fill.style.backgroundColor = color;
        label = root.Q<Label>();
        label.style.color = new StyleColor(Color.white);
    }

    public void SetProgress(float min, float max, float value)
    {
        fill.style.width = Length.Percent(Mathf.Clamp01((value - min) / (max - min)) * 100);
        label.text = $"{value}/{max}";
    }
}