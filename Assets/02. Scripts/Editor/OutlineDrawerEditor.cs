using UnityEditor;

[CustomEditor(typeof(OutlineDrawer))]
public class OutlineDrawerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Material 말고 OriginMesh만 넣을 수 있게 .
    }
}