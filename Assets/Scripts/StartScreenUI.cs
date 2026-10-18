
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class StartScreenUI
{
    private static readonly Color BackgroundColor = new Color32(11, 19, 38, 255);
    private static readonly Color Cyan = new Color32(53, 225, 247, 255);
    private static readonly Color Pink = new Color32(255, 65, 130, 255);
    private static readonly Color Muted = new Color32(163, 178, 202, 255);
    private static readonly Color HomeFill = new Color32(11, 19, 38, 225);

    public static GameObject CreateHomeButton(
        Transform parent,
        UnityEngine.Events.UnityAction onHome,
        string objectName,
        Vector2 size,
        Vector2 anchor,
        Vector2 anchoredPosition)
    {
        Transform existing = parent.Find(objectName);
        if (existing != null)
        {
            Button existingButton = existing.GetComponent<Button>();
            if (existingButton != null)
            {
                existingButton.onClick.RemoveListener(onHome);
                existingButton.onClick.AddListener(onHome);
            }

            return existing.gameObject;
        }

        GameObject buttonObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(Image),
            typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        Image image = buttonObject.GetComponent<Image>();
        image.color = HomeFill;
        image.raycastTarget = true;

        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = new Color(Cyan.r, Cyan.g, Cyan.b, 0.9f);
        outline.effectDistance = new Vector2(1.5f, 1.5f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onHome);
        ColorBlock colors = button.colors;
        colors.normalColor = HomeFill;
        colors.highlightedColor = new Color32(27, 61, 83, 255);
        colors.pressedColor = new Color32(24, 119, 141, 255);
        button.colors = colors;

        bool showLabel = size.x > 200f;
        CreateHouseIcon(
            buttonObject.transform,
            showLabel ? new Vector2(-84f, 0f) : Vector2.zero,
            showLabel ? 0.78f : 1.05f);

        if (showLabel)
        {
            TMP_Text label = AddText(
                buttonObject.transform,
                "HOME",
                30,
                Cyan,
                0.5f,
                0.5f,
                170f,
                size.y - 12f,
                FontStyles.Bold);
            label.rectTransform.anchoredPosition = new Vector2(40f, 0f);
            label.raycastTarget = false;
        }

        return buttonObject;
    }

    private static void CreateHouseIcon(
        Transform parent,
        Vector2 center,
        float scale)
    {
        AddHomeStroke(parent, "HomeIcon_RoofLeft",
            new Vector2(28f, 5f) * scale,
            center + new Vector2(-10f, 11f) * scale,
            42f);
        AddHomeStroke(parent, "HomeIcon_RoofRight",
            new Vector2(28f, 5f) * scale,
            center + new Vector2(10f, 11f) * scale,
            -42f);
        AddHomeStroke(parent, "HomeIcon_LeftWall",
            new Vector2(5f, 22f) * scale,
            center + new Vector2(-14f, -8f) * scale,
            0f);
        AddHomeStroke(parent, "HomeIcon_RightWall",
            new Vector2(5f, 22f) * scale,
            center + new Vector2(14f, -8f) * scale,
            0f);
        AddHomeStroke(parent, "HomeIcon_Base",
            new Vector2(33f, 5f) * scale,
            center + new Vector2(0f, -19f) * scale,
            0f);
    }

    private static void AddHomeStroke(
        Transform parent,
        string objectName,
        Vector2 size,
        Vector2 position,
        float rotation)
    {
        GameObject stroke = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(Image));
        stroke.transform.SetParent(parent, false);

        RectTransform rect = stroke.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        rect.localRotation = Quaternion.Euler(0f, 0f, rotation);

        Image image = stroke.GetComponent<Image>();
        image.color = Cyan;
        image.raycastTarget = false;
    }

    public static GameObject Create(
        Transform canvas,
        UnityEngine.Events.UnityAction onPlay)
    {
        GameObject root = new GameObject(
            "StartPanel",
            typeof(RectTransform),
            typeof(Image),
            typeof(CanvasGroup));

        root.transform.SetParent(canvas, false);

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        root.GetComponent<Image>().color = BackgroundColor;

        // Header
        AddText(root.transform, "ARCADE SURVIVAL",
            40, Muted, 0.5f, 0.87f, 1400, 70,
            FontStyles.Bold);

        // Main title
        AddText(root.transform, "LAST",
            112, Cyan, 0.5f, 0.775f, 1200, 140,
            FontStyles.Bold, Cyan);

        AddText(root.transform, "SECOND",
            112, Color.white, 0.5f, 0.665f, 1400, 140,
            FontStyles.Bold, Cyan);

        // Tagline
        AddText(root.transform, "ONE MOVE. ONE CHANCE.",
            42, Muted, 0.5f, 0.565f, 1450, 72,
            FontStyles.Normal);

        AddText(root.transform, "DODGE THE OBSTACLES",
            35, Cyan, 0.5f, 0.505f, 1400, 62,
            FontStyles.Bold);

        CreateLanePreview(root.transform);
        CreatePlayButton(root.transform, onPlay);

        AddText(root.transform, "TAP LEFT  ·  CENTER  ·  RIGHT",
            34, Muted, 0.5f, 0.065f, 1400, 62,
            FontStyles.Normal);

        return root;
    }

    private static void CreateLanePreview(Transform parent)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject lane = new GameObject(
                "LanePreview_" + i,
                typeof(RectTransform),
                typeof(Image));

            lane.transform.SetParent(parent, false);

            RectTransform rect = lane.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.365f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(132, 204);
            rect.anchoredPosition = new Vector2((i - 1) * 152, 0);

            lane.GetComponent<Image>().color = new Color32(14, 26, 47, 255);

            Outline outline = lane.AddComponent<Outline>();
            outline.effectColor = i == 1
                ? Cyan
                : new Color32(55, 72, 98, 255);
            outline.effectDistance = new Vector2(2, 2);

            if (i == 1)
            {
                AddText(lane.transform, "●",
                    58, Cyan, 0.5f, 0.18f, 90, 90,
                    FontStyles.Normal);
            }
            else
            {
                GameObject marker = new GameObject(
                    "LaneMarker",
                    typeof(RectTransform),
                    typeof(Image));

                marker.transform.SetParent(lane.transform, false);

                RectTransform markerRect = marker.GetComponent<RectTransform>();
                markerRect.anchorMin = markerRect.anchorMax = new Vector2(0.5f, 0.18f);
                markerRect.pivot = new Vector2(0.5f, 0.5f);
                markerRect.sizeDelta = new Vector2(38, 38);

                Image markerImage = marker.GetComponent<Image>();
                markerImage.color = Pink;
                markerImage.raycastTarget = false;
            }
        }
    }

    private static void CreatePlayButton(
        Transform parent,
        UnityEngine.Events.UnityAction onPlay)
    {
        GameObject buttonObject = new GameObject(
            "PlayButton",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button));

        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.09f, 0.125f);
        rect.anchorMax = new Vector2(0.91f, 0.235f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = buttonObject.GetComponent<Image>();
        image.color = Cyan;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onPlay);

        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color32(113, 239, 255, 255);
        colors.pressedColor = new Color32(26, 178, 205, 255);
        button.colors = colors;

        TMP_Text label = AddText(
            buttonObject.transform,
            "PLAY",
            52,
            new Color32(4, 28, 43, 255),
            0.5f,
            0.5f,
            1000,
            100,
            FontStyles.Bold);

        label.raycastTarget = false;
    }

    private static TMP_Text AddText(
        Transform parent,
        string value,
        float fontSize,
        Color color,
        float anchorX,
        float anchorY,
        float width,
        float height,
        FontStyles style,
        Color? glow = null)
    {
        GameObject textObject = new GameObject(
            "Text_" + value.Replace('\n', '_'),
            typeof(RectTransform),
            typeof(TextMeshProUGUI));

        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(anchorX, anchorY);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.raycastTarget = false;

        if (glow.HasValue)
        {
            Outline outline = textObject.AddComponent<Outline>();
            outline.effectColor = new Color(
                glow.Value.r, glow.Value.g, glow.Value.b, 0.35f);
            outline.effectDistance = new Vector2(1.5f, 1.5f);
        }

        return text;
    }
}
