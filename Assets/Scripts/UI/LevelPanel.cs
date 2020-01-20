using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    public GameObject grid;
    public GameObject btnPrefab;

    private void Awake()
    {
        foreach (var levelRef in GameManager.Instance.levels)
        {
            var btn = Instantiate(btnPrefab, grid.transform, true);
            var txt = btn.GetComponentInChildren<Text>();
            txt.text = levelRef.name;
            var b = btn.GetComponent<Button>();
            if (!levelRef.avalible)
            {
                b.interactable = false;
            }

            b.onClick.AddListener(() =>
            {
                StartCoroutine(GameManager.Instance.AsyncLoadScene(levelRef.assRef,
                    (si) =>
                    {
                        //SceneManager.SetActiveScene(si.Scene);
                        gameObject.Hide();
                    }));
            });
        }

        var rect = grid.GetComponent<RectTransform>();
        var size = GameManager.Instance.levels.Count * 1200;
        rect.sizeDelta = new Vector2(size, 930);
        rect.anchoredPosition = new Vector2(-(float) size / 2, 465);
    }
}