using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameController gameController = (GameController)target;
        if(GUILayout.Button("Start Game")) {
            gameController.StartGame();
        }
    }
}
