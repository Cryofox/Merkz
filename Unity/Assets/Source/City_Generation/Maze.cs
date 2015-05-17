using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Node
{
	public int x,y;
	public Node parent;
	public List<Node> children; //List of Potential next nodes


	public Node(int x, int y, Node parent)
	{
		children= new List<Node>();
		this.parent=parent;
		this.x=x;
		this.y=y;
	}
	public void AddChild(Node node)
	{
		children.Add(node);
	}

}
struct Point 
{
	public int x,y;
	public Point(int x, int y)
	{
		this.x=x;
		this.y=y;
	}
}

	enum direction { Left, Right, Up, Down, None };

public class Maze	
{
	//Legend
	//0 = Pathway
	//1 = Wall
	//2 = Room
	//3 = Door

	public int[][] blueprint;
	//These are used without CellSize consideration
	public int maxW;
	public int maxH;
	//These are including CellSize 
	public int realMaxW;
	public int realMaxH;
	//Room Controls

	//Dont Change this value
	int room_Depth =5;
	//////////////////////////

	//Use this to manipulate room sizes.
	int cellSize=4;

	//Create a Maze with the Following Parameters
	public Maze(int width, int height)
	{
		Random.seed=45;
		//Setup Maze Dimensions
		blueprint = new int[width*cellSize][];

		maxW= 	width;
		maxH=	height;

		realMaxH= maxH * cellSize;
		realMaxW= maxW * cellSize;	
			
		for(int i=0;i<width*cellSize;i++)
		{
			blueprint[i] = new int[height*cellSize];
			for(int j=0;j<height*cellSize;j++)
				blueprint[i][j]=1;
		}

		CreatePath();
	}


	void CreatePath()
	{
		//Method 1: Recursion/Subdivision
		// int depth=0;
		// int targetDepth=2;
		// Node root = new Node(0,0, maxW-1,maxH-1);

		// Subdivide(depth,targetDepth,root);

		//Method 2: DFS
		DFS();
		Create_Rooms(room_Depth);
	}

	Node root;
	void DFS()
	{
		//Choose Start
		int x = Random.Range(0, maxW);
		int y = Random.Range(0, maxH);

		List<Node> openCells= new List<Node>();
		openCells.Add( root=(new Node(x,y,null)));



		Node currentNode;
		AddToOpen(openCells,openCells[0]);
		while(openCells.Count>0)
		{
			currentNode = openCells[ openCells.Count-1 ];
			blueprint[currentNode.x][currentNode.y]=0;

			//Check 
			direction dir = GetNextPath(currentNode);
			Node newNode_1;
			Node newNode_2;
			switch (dir)
			{
				case direction.Left:
				//Set the Parent. Useful for backtracking.
					blueprint[(currentNode.x-1)][(currentNode.y)]=0;
					newNode_1 = new Node((currentNode.x-1), (currentNode.y),	currentNode);
					newNode_2 = new Node((currentNode.x-2), (currentNode.y), 	newNode_1);
					currentNode.AddChild(newNode_2);
					AddToOpen(openCells,newNode_2); //Adds to the End of the List					
				break;

				case direction.Right:
				//Set the Parent. Useful for backtracking.
					blueprint[(currentNode.x+1)][(currentNode.y)]=0;
					newNode_1 = new Node((currentNode.x+1), (currentNode.y),	currentNode);
					newNode_2 = new Node((currentNode.x+2), (currentNode.y), 	newNode_1);

					currentNode.AddChild(newNode_2);
					AddToOpen(openCells,newNode_2); //Adds to the End of the List						
				break;

				case direction.Up:
				//Set the Parent. Useful for backtracking.
					blueprint[(currentNode.x)][(currentNode.y+1)]=0;
					newNode_1 = new Node((currentNode.x), (currentNode.y+1),	currentNode);
					newNode_2 = new Node((currentNode.x), (currentNode.y+2), 	newNode_1);

					currentNode.AddChild(newNode_2);

					AddToOpen(openCells,newNode_2); //Adds to the End of the List	
						
				break;


				case direction.Down:
				//Set the Parent. Useful for backtracking.
					blueprint[(currentNode.x)][(currentNode.y-1)]=0;
					newNode_1 = new Node((currentNode.x), (currentNode.y-1),	currentNode);
					newNode_2 = new Node((currentNode.x), (currentNode.y-2),	newNode_1);

					currentNode.AddChild(newNode_2);
					AddToOpen(openCells,newNode_2); //Adds to the End of the List		
				break;
				//No Direction was found. BackTrack.
				default:
					// openCells.Add(currentNode.parent);
					//No Adjacent Cells here. Remove it from open list
					openCells.RemoveAt(openCells.Count-1);
				break;
			}
		}
	}

	void Create_Rooms(int depth)
	{
		//Grab all Children
		// starting at root
		List<Node> leaves = new List<Node>();
		List<Node> openList= new List<Node>();

		openList.Add(root);
		while(openList.Count>0)
		{
			if(openList[0].children.Count==0)
			{
				//A leaf has no children.
				leaves.Add(openList[0]);
			}
			//Add all the children of this node to the openlist
			for(int i=0;i< openList[0].children.Count;i++)
			{
				openList.Add(openList[0].children[i]);
			}
			//Remove the current node from open list
			openList.RemoveAt(0);
		}

		for(int i=0; i< leaves.Count;i++)
		{
		//	blueprint[leaves[i].x][leaves[i].y]=2;
			ExpandRoom(leaves[i],0,depth);
		}

		Room_Shapify();
	}

	//Given a leaf node it converts the walkable path into "rooms"
	//this is done untill the depth set is reached
	void ExpandRoom(Node currentNode, int curDepth, int targetDepth)
	{
		if(curDepth>=targetDepth)
			return;
		if(currentNode==null)
			return;

		blueprint[currentNode.x][currentNode.y]=2;
		ExpandRoom( currentNode.parent, (curDepth+1), targetDepth);
	}

	//Now we Try to Approximate roomShape by converting the weird
	//lines into containers
	void Room_Shapify()
	{
		List<Point> roomPoints = new List<Point>();
		//First get a list of all yellow points (this is needed so the new points dont get added to the equation)
		for(int y=0;y<maxH;y++)		
			for(int x=0;x<maxW;x++)
			{
				if(blueprint[x][y]==2)
					roomPoints.Add( new Point(x,y));
			}

		//Now for each point in the list, add it's neighbour points to yellow (regardless if a path)
		for(int i=0;i< roomPoints.Count;i++)
		{
			//up
			if(roomPoints[i].y+1< maxH)
				blueprint[ roomPoints[i].x][ roomPoints[i].y+1]=2;
			//down
			if(roomPoints[i].y-1>=0)
				blueprint[ roomPoints[i].x][ roomPoints[i].y-1]=2;

			//right
			if(roomPoints[i].x+1< maxH)
				blueprint[ roomPoints[i].x+1][ roomPoints[i].y]=2;
			//left
			if(roomPoints[i].x-1>=0)
				blueprint[ roomPoints[i].x-1][ roomPoints[i].y]=2;


		}


		Cleanup_Points();

	}

	void Cleanup_Points()
	{
		//If a yellow point exists where 3 of it's neighbours aren't also yellow, convert it blue
		for(int y=0;y<maxH;y++)		
			for(int x=0;x<maxW;x++)
			{
				if(blueprint[x][y]==2)
				{
					int count=0;
					if(x+1<maxW && blueprint[x+1][y]!=2)
						count++;
					if(x-1>=0 && blueprint[x-1][y]!=2)
						count++;
					if(y+1<maxH && blueprint[x][y+1]!=2)
						count++;
					if(y-1>=0 && blueprint[x][y-1]!=2)
						count++;

					if(count>=3)
						blueprint[x][y]=1;
				}
			}		
		//Zoomin 
		Rescale_Grid();
	}

	void Rescale_Grid()
	{
		//Easier to start from top Right (End) and work to the beginning.
		//This way we don't overwrite
		for(int y=maxH;y>0;y--)		
			for(int x=maxW;x>0;x--)
			{

				//Grab the value
				int val= blueprint[x-1][y-1];
				//Now Apply this to the location at * CellSize &
				if(x==0 && y==0)
					for(int i=0;i<cellSize;i++)
						for(int j=0;j<cellSize;j++)									
							blueprint[(cellSize-1)-i][(cellSize-1)-j]= val;

				else if(x==0)
					for(int i=0;i<cellSize;i++)
						for(int j=0;j<cellSize;j++)								
							blueprint[(cellSize-1)-i][y*cellSize-j-1]= val;

				else if(y==0)
					for(int i=0;i<cellSize;i++)
						for(int j=0;j<cellSize;j++)								
							blueprint[x*cellSize-i-1][(cellSize-1)-j]= val;

				else
					for(int i=0;i<cellSize;i++)
						for(int j=0;j<cellSize;j++)									
							blueprint[x*cellSize-i-1][y*cellSize-j-1]= val;
			}	


	}






	void AddToOpen(List<Node> openCells, Node curNode)
	{
		openCells.Add(curNode);
		blueprint[curNode.x][curNode.y]=0;
	}

	direction GetNextPath(Node curNode)
	{
		//Create a List of available Directions
		List<direction> paths = new List<direction>();

		if(curNode.x+2<maxW && blueprint[curNode.x+2][curNode.y]==1)
			paths.Add(direction.Right);
		if(curNode.x-2>=0 	&& blueprint[curNode.x-2][curNode.y]==1)
			paths.Add(direction.Left);
		if(curNode.y+2<maxH &&  blueprint[curNode.x][curNode.y+2]==1)
			paths.Add(direction.Up);		
		if(curNode.y-2>=0 	&& blueprint[curNode.x][curNode.y-2]==1)
			paths.Add(direction.Down);

		if(paths.Count>0)
			return paths[ Random.Range(0,paths.Count)];
		return direction.None;
	}


	//Modif = Spacing
	public void DebugDraw_Grid(float modif=1)
	{

		for(int x=0;x< 	realMaxW;x++)
		for(int y=0;y<	realMaxH;y++)
		{
			Color color;
			if(blueprint[x][y]==1)
				color= Color.red;
			else if(blueprint[x][y]==0)
				color= Color.blue;
			else if(blueprint[x][y]==2)
				color= Color.yellow;
			else
				color= Color.green;

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
