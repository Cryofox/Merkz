using UnityEngine;
using System.Collections;

public class City : MonoBehaviour {
	Maze maze;
	// Use this for initialization
	void Start () {
		int dimensions= 51;
		maze = new Maze(dimensions,dimensions);
		DebugDraw_Grid(dimensions);
	}
	

	public void DebugDraw_Grid(int dimensions)
	{
		float modif=10;
		for(int x=0;x< 	dimensions;x++)
		for(int y=0;y<	dimensions;y++)
		{
			Color color;
			if(maze.blueprint[x][y]==1)
				color= Color.red;
			else if(maze.blueprint[x][y]==0)
				color= Color.blue;
			else if(maze.blueprint[x][y]==2)
				color= Color.yellow;
			else
				color= Color.green;

			if(color!=Color.red)
			{
				//Draw Top Line
				Debug.DrawLine(new Vector3((float)(x*modif)-0.25f*modif,0f,(float)(y*modif)+0.25f *modif), new Vector3((float)(x*modif)+0.25f*modif,0f,(float)(y*modif)+0.25f*modif ),color,20);
				//Draw Bot Line
				Debug.DrawLine(new Vector3((float)(x*modif)-0.25f*modif,0f,(float)(y*modif)-0.25f *modif), new Vector3((float)(x*modif)+0.25f*modif,0f,(float)(y*modif)-0.25f*modif ),color,20);

				//Draw Left Line
				Debug.DrawLine(new Vector3((float)(x*modif)-0.25f*modif,0f,(float)(y*modif)-0.25f*modif ), new Vector3((float)(x*modif)-0.25f*modif,0f,(float)(y*modif)+0.25f*modif ),color,20);
				//Draw Bot Line
				Debug.DrawLine(new Vector3((float)(x*modif)+0.25f*modif,0f,(float)(y*modif)-0.25f*modif ), new Vector3((float)(x*modif)+0.25f*modif,0f,(float)(y*modif)+0.25f*modif ),color,20);				
			}

		}
	}
}
