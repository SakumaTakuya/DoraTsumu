using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(Space))]
public class SpaceDrawer : PropertyDrawer
{    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height /= 5;
        var xmin = property.FindPropertyRelative("_xmin");
        var xmax = property.FindPropertyRelative("_xmax");
        var ymin = property.FindPropertyRelative("_ymin");
        var ymax = property.FindPropertyRelative("_ymax");

        xmax.floatValue = xmax.floatValue > xmin.floatValue ? xmax.floatValue : xmin.floatValue;
        ymax.floatValue = ymax.floatValue > ymin.floatValue ? ymax.floatValue : ymin.floatValue;

        EditorGUI.LabelField(position, label, GUIContent.none);
        position.y += position.height;
        position.width /= 4;
        var ypos = position;
            
        position.x += ypos.width + 3;
        position.width = position.width * 3 - 3;
        position.height = position.height * 3 - 3;
        EditorGUI.DrawRect(position,new Color(0.51f, 0.51f, 0.51f));
        position.x += 3;
        position.y += 1;
        position.width -= 4;
        position.height -= 4;
        EditorGUI.DrawRect(position,new Color(0.76f, 0.76f, 0.76f));

        var ylpos = ypos;
        ylpos.x += ypos.width + 6;
        ylpos.width = 30;
        GUI.Label(ylpos, "Y");
        
        EditorGUI.PropertyField(ypos, ymax, GUIContent.none);
        ypos.y += 2 * ypos.height - 3;
        EditorGUI.PropertyField(ypos, ymin, GUIContent.none);
        ypos.y += ypos.height + 3;
        var xpos = ypos;
        xpos.x += xpos.width - 12;
        EditorGUI.PropertyField(xpos, xmin, GUIContent.none);
        xpos.x += 2 * xpos.width + 12;
        EditorGUI.PropertyField(xpos, xmax, GUIContent.none);

        var xlpos = xpos;
        xlpos.y -= xpos.height + 3;
        xlpos.x += xpos.width - 12;
        xlpos.width = 30;
        GUI.Label(xlpos, "X");
    }
    
    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        return 5 * EditorGUIUtility.singleLineHeight + 3 * EditorGUIUtility.standardVerticalSpacing;
    }
}
#endif

[Serializable]
public struct Space
{
    [SerializeField]
    private float _xmin, _xmax, _ymin, _ymax;
    
    public float Xmin
    {
        get { return _xmin; }
        set { _xmin = value; }
    }

    public float Xmax
    {
        get { return _xmax; }
        set { _xmax = value > _xmin ? value : _xmin; }
    }

    public float Ymin
    {
        get { return _ymin; }
        set { _ymin = value; }
    }

    public float Ymax
    {
        get { return _ymax; }
        set { _ymax = value > _ymin ? value : _ymin; }
    }

    public Space(float xmin = 0f, float xmax = 10f, float ymin = 0f, float ymax = 10f)
    {
        _xmin = xmin;
        _xmax = xmax;
        _ymin = ymin;
        _ymax = ymax;
    }

    public Vector2 Random()
    {
        var pos = new Vector2(
            UnityEngine.Random.Range(_xmin, _xmax),
            UnityEngine.Random.Range(_ymin, _ymax)
        );
        return pos;
    }

    public static Space operator +(Space space, Vector2 vector2)
    {
        return new Space(
            space._xmin + vector2.x, space._xmax + vector2.x,
            space._ymin + vector2.y, space._ymax + vector2.y);
    }
    
    public static Space operator -(Space space, Vector2 vector2)
    {
        return new Space(
            space._xmin - vector2.x, space._xmax - vector2.x,
            space._ymin - vector2.y, space._ymax - vector2.y);
    }
}




