using UnityEngine;
using System.Collections;

public class colorpicker : MonoBehaviour {

	public Texture2D colors;
	public Material[] changedMaterials;

	int x,y;

	Rect textureRect=new Rect(-300, Screen.height-180, 250, 150);
	float rectX = -300f;

	bool paint;

	void Update () {

		rectX=50;

		textureRect.x=rectX;
		textureRect.y= Screen.height-180;

		if(paint){
			
			x=(int)(Input.mousePosition.x-textureRect.x);
			y=(int)(textureRect.y-((Screen.height-Input.mousePosition.y)-130));

			foreach(Material mat in changedMaterials)
				mat.SetColor("_Color",colors.GetPixel(x,y));
			
			if(Input.GetMouseButtonDown(0))
				paint=false;
		}
	}

	void OnGUI() {
		
		if(GUI.Button(new Rect(textureRect.x, textureRect.y-40, 100, 30), "chose color"))
			paint=true;

			GUI.DrawTexture(textureRect, colors);

	}
		
}
