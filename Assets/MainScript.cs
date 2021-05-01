using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public int width;
    public int height;

    public Tile baseTile;

    private int[,] counts;

    private bool isRandom = true;
    private bool isSame = true;

    private bool isRunnning = false;
    private float timer = 0;
    private float diffAngle = 0;

    private int recordCount = 0;
    private string record = "Record\n You can copy them by press the button, then paste them in Excel to draw a graph.";

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("StartExperiment").GetComponent<Button>().enabled = true;
        GameObject.Find("ModeSelection").GetComponent<Dropdown>().enabled = true;
        GameObject.Find("ShowHeatmap").GetComponent<Toggle>().isOn = false;
        GameObject.Find("ShowHeatmap").GetComponent<Toggle>().enabled = false;
        GameObject.Find("EndExperimentTrue").GetComponent<Button>().enabled = false;
        GameObject.Find("EndExperimentFalse").GetComponent<Button>().enabled = false;
        counts = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                counts[i, j] = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Exit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Clear();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Copy(); 
        }
        GameObject.Find("Record").GetComponent<Text>().text = record;
        if (isRunnning)
        {
            timer += Time.deltaTime;
            int indexX = Mathf.Clamp((int)(Input.mousePosition.x / Screen.width * width), 0, width - 1);
            int indexY = Mathf.Clamp((int)(Input.mousePosition.y / Screen.height * height), 0, height - 1);
            AddCount(indexX, indexY, 2, 1);
            if (Input.GetKeyDown(KeyCode.S))
            {
                EndExperimentTrue();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                EndExperimentFalse();
            }
        }
        else
        {
            timer = 0;
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartExperiment();
            }
        }
    }

    public void AddCount(int x, int y, int weight, int range)
    {
        counts[x, y] += weight;
        for (int i = x - range; i <= x + range; i++)
        {
            if (i < 0 || i >= width)
            {
                continue;
            }
            for (int j = y - range; j <= y + range; j++)
            {
                if (j < 0 || j >= height)
                {
                    continue;
                }
                counts[i, j] += weight / 2;
            }
        }
    }

    public Tile FindCorrectTile(int count)
    {
        Debug.Log(count);
        if (count == 0)
        {
            return null;
        }
        Tile temp = Instantiate(baseTile);
        Color tempColor = temp.color;
        tempColor.r = Mathf.Min(tempColor.r + (float)count / 64, 1);
        tempColor.g = Mathf.Max(tempColor.g - (float)count / 64, 0);
        temp.color = tempColor;
        return temp;
    }

    private void SetTilemap()
    {
        Tilemap tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        tilemap.ClearAllTiles();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3Int pos = new Vector3Int(i - width / 2, j - height / 2, 0);
                tilemap.SetTile(pos, FindCorrectTile(counts[i, j]));
            }
        }
    }

    public void StartExperiment()
    {
        GameObject.Find("StartExperiment").GetComponent<Button>().enabled = false;
        GameObject.Find("ModeSelection").GetComponent<Dropdown>().enabled = false;
        GameObject.Find("ShowHeatmap").GetComponent<Toggle>().isOn = false;
        GameObject.Find("ShowHeatmap").GetComponent<Toggle>().enabled = false;
        GameObject.Find("EndExperimentTrue").GetComponent<Button>().enabled = true;
        GameObject.Find("EndExperimentFalse").GetComponent<Button>().enabled = true;

        GetComponent<ExpObjectScript>().ClearAllExpObjects();

        isSame = Random.Range(0, 2) == 0;
        if (isRandom)
        {
            diffAngle = GetComponent<ExpObjectScript>().CreateRandomObjects(isSame);
        }
        else
        {
            diffAngle = GetComponent<ExpObjectScript>().CreateDesignedObjects(isSame);
        }

        isRunnning = true;
    }

    public void EndExperiment()
    {
        GameObject.Find("StartExperiment").GetComponent<Button>().enabled = true;
        GameObject.Find("ModeSelection").GetComponent<Dropdown>().enabled = true;
        GameObject.Find("ShowHeatmap").GetComponent<Toggle>().isOn = true;
        GameObject.Find("ShowHeatmap").GetComponent<Toggle>().enabled = true;
        GameObject.Find("EndExperimentTrue").GetComponent<Button>().enabled = false;
        GameObject.Find("EndExperimentFalse").GetComponent<Button>().enabled = false;

        isRunnning = false;
        SetTilemap();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                counts[i, j] = 0;
            }
        }
    }

    // If player thinks two objects are same
    public void EndExperimentTrue()
    {
        if (isSame)
        {
            GameObject.Find("Result").GetComponent<Text>().text =
                "Correct!\nTime used: " + timer + " sec\nAngle: " + diffAngle + " degree";
            if (recordCount == 0)
            {
                record = "";
            }
            record += diffAngle + "\t" + timer + "\n";
            recordCount++;
        }
        else
        {
            GameObject.Find("Result").GetComponent<Text>().text =
                 "Wrong!\nTime used: " + timer + " sec\nAngle: " + diffAngle + " degree";
        }
        EndExperiment();
    }

    // If player thinks two objects are different
    public void EndExperimentFalse()
    {
        if (!isSame)
        {
            GameObject.Find("Result").GetComponent<Text>().text =
                "Correct!\nTime used: " + timer + " sec\nAngle: " + diffAngle + " degree";
            if (GameObject.Find("RecordDifferent").GetComponent<Toggle>().isOn)
            {
                if (recordCount == 0)
                {
                    record = "";
                }
                record += diffAngle + "\t" + timer + "\n";
                recordCount++;
            }
        }
        else
        {
            GameObject.Find("Result").GetComponent<Text>().text =
                 "Wrong!\nTime used: " + timer + " sec\nAngle: " + diffAngle + " degree";
        }
        EndExperiment();
    }

    public void SetMode(Dropdown dropdown)
    {
        isRandom = dropdown.value == 0;
    }

    public void ShowHeatmap()
    {
        if (GameObject.Find("Tilemap").layer == 5)
        {
            GameObject.Find("Tilemap").layer = 6;
        }
        else
        {
            GameObject.Find("Tilemap").layer = 5;
        }
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Clear()
    {
        recordCount = 0;
        record = "Record\n You can copy them by press the button, then paste them in Excel to draw a graph.";
    }

    public void Copy()
    {
        GameObject.Find("Result").GetComponent<Text>().text = "Record copied! You can paste them in Excel";
        GUIUtility.systemCopyBuffer = record;
    }
}
