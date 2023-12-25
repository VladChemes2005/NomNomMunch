using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BoolArrayLayout))]
public class BoolArrayLayoutPropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PrefixLabel(position, label);
		Rect newposition = position;
		newposition.y += 144f;
		SerializedProperty data = property.FindPropertyRelative("rows");

		if (data.arraySize != 7)
			data.arraySize = 7;

		for (int j = 0; j < 7; j++)
		{
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
			newposition.height = 18f;

			if (row.arraySize != 7)
				row.arraySize = 7;

			newposition.width = position.width / 7;
			for (int i = 0; i < 7; i++)
			{
				EditorGUI.PropertyField(newposition, row.GetArrayElementAtIndex(i), GUIContent.none);
				newposition.x += newposition.width;
			}

			newposition.x = position.x;
			newposition.y -= 18f;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 18f * 10;
}