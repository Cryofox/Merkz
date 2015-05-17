using UnityEngine;
using System.Collections;
using System;
public class City : MonoBehaviour {
	Maze maze;
//Now that The blueprint is set, we can spawn the objects we need where we need them
enum city_Object { None, Door, OCorWall,CorWall, TriWall, Wall, StreetLight};

	// Use this for initialization
	int gridSpacing=2;
	void Start () 
	{
		int dimensions= 100;
		int roomCellSize= 10;


		//Create the Maze + Room Grid Points
		maze = new Maze(dimensions,dimensions);
		Populate_City();


		//maze.DebugDraw_Grid(gridSpacing);
	}
	


	int door_Proximity=5; // Range at which doors can't be placed due to being too close to one another

	city_Object[][] object_Grid;
	void Populate_City()
	{
		//Populate City with Doors. Doors can only be placed on Yellow Cells, and must only be placed at locations 
		//where there is a blue on one side and yellow square opposing.

		object_Grid = new city_Object[maze.realMaxW][];
		//Setup Grid according to the Maze Specs
		for(int i=0;i<maze.realMaxW;i++)
		{
			object_Grid[i] = new city_Object[maze.realMaxH];
			for(int j=0;j<maze.realMaxH;j++)
				object_Grid[i][j]=city_Object.None;
		}

		//Object Grid Placeing Doors.
		Place_Doors();
		Place_Walls();


		Place_Tiles(); //Call this one Last
	}

	void Place_Doors()
	{
		//Door PlaceMent------------------------
		//The 1 Buffer is it makes no sense for units to use doors to OutofBounds
		for(int y=1;y<maze.realMaxH-1; y++)		
		for(int x=1;x<maze.realMaxW-1; x++)
		{
			//Room = 2
			if(maze.blueprint[x][y]==2)
			{
				//Check  <- Door ->	
				//Check if paths exist on both sides, and no corners
				if((maze.blueprint[x-1][y]==2 && maze.blueprint[x+1][y]==0) &&
				  (maze.blueprint[x][y-1]==2 && maze.blueprint[x][y+1]==2))
				{
					//Here we know the Room is on left so check for Right corners
					if(maze.blueprint[x+1][y+1]!=2 && maze.blueprint[x+1][y-1]!=2)
						if(!isWithinProximity(city_Object.Door,door_Proximity,x,y))
							if(UnityEngine.Random.Range(0,100)<=25)
								Spawn_Object(city_Object.Door, new Vector3(x,0,y),Quaternion.Euler(0,90,0));					
				}
				else if(( maze.blueprint[x+1][y]==2 && maze.blueprint[x-1][y]==0) &&
						(maze.blueprint[x][y-1]==2 && maze.blueprint[x][y+1]==2))
				{
					if(maze.blueprint[x-1][y+1]!=2 && maze.blueprint[x-1][y-1]!=2)
						if(!isWithinProximity(city_Object.Door,door_Proximity,x,y))
							if(UnityEngine.Random.Range(0,100)<=25)
								Spawn_Object(city_Object.Door, new Vector3(x,0,y),Quaternion.Euler(0,-90,0));		
				}

				//Check  v Door ^	
				else if((maze.blueprint[x][y+1]==0 && maze.blueprint[x][y-1]==2) &&
						(maze.blueprint[x-1][y]==2 && maze.blueprint[x+1][y]==2))
				{
					if(maze.blueprint[x-1][y+1]!=2 && maze.blueprint[x+1][y+1]!=2)
						if(!isWithinProximity(city_Object.Door,door_Proximity,x,y))
							if(UnityEngine.Random.Range(0,100)<=25)
								Spawn_Object(city_Object.Door, new Vector3(x,0,y),Quaternion.Euler(0,0,0));			
				}
				else if((maze.blueprint[x][y-1]==0 && maze.blueprint[x][y+1]==2) &&
						(maze.blueprint[x-1][y]==2 && maze.blueprint[x+1][y]==2))
				{
					if(maze.blueprint[x-1][y-1]!=2 && maze.blueprint[x+1][y-1]!=2)
						if(!isWithinProximity(city_Object.Door,door_Proximity,x,y))
							if(UnityEngine.Random.Range(0,100)<=25)
								Spawn_Object(city_Object.Door, new Vector3(x,0,y),Quaternion.Euler(0,180,0));				
				}
			}
		}
		//=======================================	
	}

	void Place_Walls()
	{
		for(int y=0;y<maze.realMaxH; y++)		
		for(int x=0;x<maze.realMaxW; x++)
		{
			//Room = 2 no object placed == None
			if(maze.blueprint[x][y]==2 && object_Grid[x][y]== city_Object.None)
			{
				//Readability. Attempt so long as it's withing bounds.
				try{

				//2 Side Opening
				//Corner Right Side opening. Down Left U
					if((maze.blueprint[x+1][y]!=2) &&
					  (maze.blueprint[x][y+1]!=2) &&
					  (maze.blueprint[x-1][y]==2) &&
					  (maze.blueprint[x][y-1]==2))
						Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,0,0));

					else if((maze.blueprint[x-1][y]!=2) &&
					  (maze.blueprint[x][y+1]!=2) &&
					  (maze.blueprint[x+1][y]==2) &&
					  (maze.blueprint[x][y-1]==2))
						Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,-90,0));

					else if((maze.blueprint[x][y-1]!=2) &&
					  (maze.blueprint[x+1][y]!=2) &&
					  (maze.blueprint[x][y+1]==2) &&
					  (maze.blueprint[x-1][y]==2))
						Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,90,0));

					else if((maze.blueprint[x][y-1]!=2) &&
					  (maze.blueprint[x-1][y]!=2) &&
					  (maze.blueprint[x][y+1]==2) &&
					  (maze.blueprint[x+1][y]==2))
						Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,180,0));
				//Standard Wall
				//Its a Wall...but what rotation?
					else if((maze.blueprint[x+1][y]!=2) &&
					  (maze.blueprint[x-1][y]==2) &&
					  (maze.blueprint[x][y+1]==2) &&
					  (maze.blueprint[x][y-1]==2))
						Spawn_Object(city_Object.Wall, new Vector3(x,0,y), Quaternion.Euler(-90,90,0));

					else if((maze.blueprint[x-1][y]!=2) &&
					  (maze.blueprint[x+1][y]==2) &&
					  (maze.blueprint[x][y+1]==2) &&
					  (maze.blueprint[x][y-1]==2))
						Spawn_Object(city_Object.Wall, new Vector3(x,0,y), Quaternion.Euler(-90,-90,0));

					else if((maze.blueprint[x][y+1]!=2) &&
					  (maze.blueprint[x+1][y]==2) &&
					  (maze.blueprint[x-1][y]==2) &&
					  (maze.blueprint[x][y-1]==2))
						Spawn_Object(city_Object.Wall, new Vector3(x,0,y), Quaternion.Euler(-90,0,0));

					else if((maze.blueprint[x][y-1]!=2) &&
					  (maze.blueprint[x+1][y]==2) &&
					  (maze.blueprint[x-1][y]==2) &&
					  (maze.blueprint[x][y+1]==2))
						Spawn_Object(city_Object.Wall, new Vector3(x,0,y), Quaternion.Euler(-90,180,0));

					//Finally if none of the above check if its an outer Corner (Cornerner that is nested in 2 walls)
					// -+   < Outer Coern
					//  |

					//If all around are +, excluding 1 Corner
					else if(
					  (maze.blueprint[x][y-1]==2) &&
					  (maze.blueprint[x+1][y]==2) &&
					  (maze.blueprint[x-1][y]==2) &&
					  (maze.blueprint[x][y+1]==2) &&
					  (maze.blueprint[x+1][y+1]!=2)) 
						Spawn_Object(city_Object.OCorWall, new Vector3(x,0,y), Quaternion.Euler(-90,0,0));

					else if(
					  (maze.blueprint[x][y-1]==2) &&
					  (maze.blueprint[x+1][y]==2) &&
					  (maze.blueprint[x-1][y]==2) &&
					  (maze.blueprint[x][y+1]==2) &&
					  (maze.blueprint[x-1][y-1]!=2)) 
						Spawn_Object(city_Object.OCorWall, new Vector3(x,0,y), Quaternion.Euler(-90,180,0));

					else if(
					  (maze.blueprint[x][y-1]==2) &&
					  (maze.blueprint[x+1][y]==2) &&
					  (maze.blueprint[x-1][y]==2) &&
					  (maze.blueprint[x][y+1]==2) &&
					  (maze.blueprint[x-1][y+1]!=2)) 
						Spawn_Object(city_Object.OCorWall, new Vector3(x,0,y), Quaternion.Euler(-90,-90,0));

					else if(
					  (maze.blueprint[x][y-1]==2) &&
					  (maze.blueprint[x+1][y]==2) &&
					  (maze.blueprint[x-1][y]==2) &&
					  (maze.blueprint[x][y+1]==2) &&
					  (maze.blueprint[x+1][y-1]!=2)) 
						Spawn_Object(city_Object.OCorWall, new Vector3(x,0,y), Quaternion.Euler(-90,90,0));

				}
				catch (Exception e)
				{
					// //Here we can only have Corner Pieces
					if(x+1==maze.realMaxW && y+1==maze.realMaxH)
					{
						Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,0,0));						
					}	
					else if(x-1<0 && y-1<0)
					{
						Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,180,0));						
					}	
					//Top Left Corner
					else if(x-1<0  && y+1==maze.realMaxH)
					{
						Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,-90,0));	
					}
					//Bot Right Corner
					else if(x+1==maze.realMaxW  && y-1<0)
					{
						Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,90,0));						
					}					
					// //Since the error occured here we now construct the walls that lie on the border
					

					//Place Walls where the RightSide is Open
					else if(x+1>=maze.realMaxW)
					{
						//At this node we ignore the right Locations
						if((maze.blueprint[x-1][y]==2) &&
						  (maze.blueprint[x][y+1]==2) &&
						  (maze.blueprint[x][y-1]==2))
							Spawn_Object(city_Object.Wall, new Vector3(x,0,y), Quaternion.Euler(-90,90,0));
						//CornerWall
						else if((maze.blueprint[x][y-1]!=2) &&
						  (maze.blueprint[x][y+1]==2) &&
						  (maze.blueprint[x-1][y]==2))
							Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,90,0));
						else if((maze.blueprint[x][y+1]!=2) &&
						  (maze.blueprint[x-1][y]==2) &&
						  (maze.blueprint[x][y-1]==2))
							Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,0,0));
					}
					else if(x-1<0)
					{
					//Wall
						if(
						  (maze.blueprint[x+1][y]==2) &&
						  (maze.blueprint[x][y+1]==2) &&
						  (maze.blueprint[x][y-1]==2))
							Spawn_Object(city_Object.Wall, new Vector3(x,0,y), Quaternion.Euler(-90,-90,0));
					//CornerWall
						else if((maze.blueprint[x][y-1]!=2) &&
						  (maze.blueprint[x][y+1]==2) &&
						  (maze.blueprint[x+1][y]==2))
							Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,180,0));
						else if((maze.blueprint[x][y+1]!=2) &&
						  (maze.blueprint[x][y-1]==2) &&
						  (maze.blueprint[x+1][y]==2))
							Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,-90,0));
					}

					// //Now for the Y out of Bounds
					else if(y+1>=maze.realMaxW)
					{
					//Wall
						if(
						  (maze.blueprint[x+1][y]==2) &&
						  (maze.blueprint[x-1][y]==2) &&
						  (maze.blueprint[x][y-1]==2))
							Spawn_Object(city_Object.Wall, new Vector3(x,0,y), Quaternion.Euler(-90,0,0));
					//CornerWall
						else if	((maze.blueprint[x+1][y]==2) &&
						  (maze.blueprint[x-1][y]!=2) &&
						  (maze.blueprint[x][y-1]==2))
							Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,-90,0));
						else if	((maze.blueprint[x-1][y]==2) &&
						  (maze.blueprint[x+1][y]!=2) &&
						  (maze.blueprint[x][y-1]==2))
							Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,0,0));
					}
					else if(y-1<0)
					{
					//Wall
						if(
						  (maze.blueprint[x+1][y]==2) &&
						  (maze.blueprint[x-1][y]==2) &&
						  (maze.blueprint[x][y+1]==2))
							Spawn_Object(city_Object.Wall, new Vector3(x,0,y), Quaternion.Euler(-90,180,0));
					//CornerWall
						else if	((maze.blueprint[x+1][y]==2) &&
						  (maze.blueprint[x-1][y]!=2) &&
						  (maze.blueprint[x][y+1]==2))
							Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,180,0));
						else if	((maze.blueprint[x-1][y]==2) &&
						  (maze.blueprint[x+1][y]!=2) &&
						  (maze.blueprint[x][y+1]==2))
							Spawn_Object(city_Object.CorWall, new Vector3(x,0,y), Quaternion.Euler(-90,90,0));
					}



				}
			}	
		}		
	}

	void Place_Tiles()
	{
		for(int y=0;y<maze.realMaxH; y++)		
		for(int x=0;x<maze.realMaxW; x++)
		{
			GameObject go = Resources.Load<GameObject>("Prefabs/Tile");
			go = GameObject.Instantiate(go, new Vector3(x,0,y)*gridSpacing, Quaternion.Euler(-90,0,0)) as GameObject;


			//Poll object, if the enum == Null set material to grass.
			//Otherwise set material to Concrete
			//Grass		
			if(object_Grid[x][y]==city_Object.None)
			{
				go.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/PH_Grass");
			}
			//Concrete
			else
			{
				go.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/PH_Concrete");
			}
		}
	}



	bool isWithinProximity(city_Object city_obj, int proximity, int xLoc, int yLoc)
	{
		for(int y=yLoc-proximity; y< yLoc+proximity;y++)		
			for(int x=xLoc-proximity; x< xLoc+proximity;x++)
			{
				if(
				   (x>-1 && x< maze.realMaxW) &&
				   (y>-1 && y< maze.realMaxH))
				{
					if(object_Grid[x][y]==city_obj)
						return true;
				}
			}
		return false;
	}


	void Spawn_Object(city_Object city_obj, Vector3 location, Quaternion rotation)
	{
		object_Grid[(int)location.x][(int)location.z]= city_obj;

		//Modify Vector here to be at correct location
		location*=gridSpacing;

		//Spawn the needed obj according to the Switch Statement
		switch(city_obj)
		{
			case city_Object.Door:
				GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Door"), location, rotation);
			break;
			case city_Object.CorWall:
				GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Wall_Corner"), location, rotation);
			break;
			case city_Object.OCorWall:
				GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Wall_OuterCorner"), location, rotation);
			break;

			case city_Object.Wall:
				GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Wall"), location, rotation);
			break;

		}
	}


}
