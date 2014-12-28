using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[Serializable]
[ExecuteInEditMode]
public class BoundaryChecker : MonoBehaviour
{
	// Store rejection vector information
	private class Rejection
	{
		public Rejection(GameObject restrictedObject, GameObject node1, GameObject node2)
		{
			Vector3 nodalVector = node2.transform.position - node1.transform.position;
			Vector3 restrictedObjectVector = restrictedObject.transform.position - node1.transform.position;
			Vector3 projectionVector = Vector3.Project(restrictedObjectVector, nodalVector.normalized);
			
			// Rejection = orig. - projection of orig. onto normal
			Vector3 rejectionVector =  restrictedObjectVector - projectionVector;
			
			this.rejVector = rejectionVector;
			
			// Determine characteristics of the different vectors (separating these into 2 is for debugging purposes)
			if(Vector3.Dot(nodalVector.normalized, projectionVector.normalized) < 0)
			{
				this.undershoot = true;
			}
			else if(nodalVector.magnitude < Vector3.Distance(restrictedObject.transform.position - rejectionVector , node1.transform.position))
			{
				this.overshoot = true;
			}
		}

		public Vector3 rejVector;
		public bool undershoot;
		public bool overshoot;
	};

	// Store path fork information
	[Serializable]
	public class Fork
	{
		public string name;

		[SerializeField]
		[HideInInspector]
		public GameObject[] path;
		public int internalPathLength = 0;
		public GameObject mainStartNode;
		public GameObject mainEndNode;

		public void AddForkNode(int prevIndex, int nextIndex)
		{
			GameObject node = new GameObject(this.name + "-" + (prevIndex + 1).ToString());
            node.AddComponent<BoundaryNode>();

			Vector3 prevPosition = prevIndex == 0 ? mainStartNode.transform.position : path [prevIndex - 1].transform.position;
			Vector3 nextPosition = nextIndex == internalPathLength + 1 ? mainEndNode.transform.position : path [nextIndex - 1].transform.position;

			node.transform.position = (prevPosition + nextPosition) / 2.0f;
			node.transform.parent = this.mainStartNode.transform.parent;
			
			// Expand size by 1
			GameObject[] temp = path;
			
			path = new GameObject[internalPathLength + 1];
			
			// Copy over values after size increase and add new entry
			for(int itr = 0; itr < internalPathLength + 1; itr++)
			{
				// Uses prevIndex and not prevIndex - 1 because the start node is not included in the list
				if(itr < prevIndex)
				{
					this.path[itr] = temp[itr];
				}
				else if(itr == prevIndex)
				{
					this.path[itr] = node;
				}
				else if(itr > prevIndex)
				{
					this.path[itr] = temp[itr - 1];
					this.path[itr].transform.name = this.name + "-" + (itr + 1).ToString();
				}
			}
			
			// Increment count
			this.internalPathLength++;
		}

		public void DeleteForkNode(int index)
		{
			// New list for use after deletion
			GameObject[] reducedForkPath = new GameObject[this.internalPathLength - 1];
			
			int reducedPathItr = 0;
			for(int itr = 0; itr < this.internalPathLength; itr++)
			{
				// If value to delete...
				if(itr == index)
				{
					// Delete and do not copy
					if(Application.isPlaying)
					{
						Destroy(this.path[itr]);
					}
					else
					{
						DestroyImmediate(this.path[itr]);
					}
					
					continue;
				}
				// If not value to delete...
				else
				{
					// Copy to new list
					reducedForkPath[reducedPathItr] = this.path[itr];
					
					// Rename entries that are after the one deleted
					if(itr > index)
					{
						// Name is M# where # is the index assuming starting at 1
						reducedForkPath[reducedPathItr].transform.name = this.name + "-" + (reducedPathItr + 1).ToString();
					}
					
					reducedPathItr++;
				}
			}
			
			// Update list with temporary list
			this.path = reducedForkPath;
			
			// Reduce the number of entries
			internalPathLength--;
		}
	};

	/// <summary>
	/// Nodes that make up the boundary
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private GameObject[] mainPathNodes;

	/// <summary>
	/// The forks in the path
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private Fork[] forkPaths;
	
	/// <summary>
	/// The number of fork paths
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private int forkPathCount;

	[HideInInspector]
	[SerializeField]
	public int ForkPathCount
	{
		get
		{
			return forkPathCount;
		}

		set
		{
			forkPathCount = value;
		}
	}

	/// <summary>
	/// The number of path nodes
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private int mainPathNodeCount;

	/// <summary>
	/// The number of path nodes
	/// </summary>
	[HideInInspector]
	[SerializeField]
	public int MainPathNodeCount
	{
		get
		{
			return mainPathNodeCount;
		}
	}

	/// <summary>
	/// If true, will draw lines between nodes in the editor
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private bool showEditorLines = true;

	/// <summary>
	/// If true, will draw lines between nodes in the editor
	/// </summary>
	public bool ShowEditorLines
	{
		get
		{
			return showEditorLines;
		}

		set
		{
			if(showEditorLines != value)
			{
				showEditorLines = value;
			}
		}
	}

	/// <summary>
	/// The color of the editor line.
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private Color mainPathLineColor = Color.black;

	/// <summary>
	/// The color of the editor line.
	/// </summary>
	public Color MainPathLineColor
	{
		get
		{
			return mainPathLineColor;
		}

		set
		{
			if(value != mainPathLineColor)
			{
				mainPathLineColor = value;

				if(showEditorLines)
				{
					DrawLines();
				}
			}
		}
	}

	/// <summary>
	/// The color of the editor line.
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private Color forkPathLineColor = Color.cyan;
	
	/// <summary>
	/// The color of the editor line.
	/// </summary>
	public Color ForkPathLineColor
	{
		get
		{
			return forkPathLineColor;
		}
		
		set
		{
			if(value != forkPathLineColor)
			{
				forkPathLineColor = value;

			    if(showEditorLines)
			    {
					DrawLines();
				}
			}
		}
	}

	/// <summary>
	/// The maximum distance a node can be to test it
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private float maximumCheckDistance;

	/// <summary>
	/// The maximum distance a node can be to test it
	/// </summary>
	public float MaximumCheckDistance
	{
		get
		{
			return maximumCheckDistance;
		}

		set
		{
			maximumCheckDistance = value;
		}
	}

	/// <summary>
	/// The maximum distance the restrictedObject can be from the path
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private float maximumAllowableDistance;

	/// <summary>
	/// The maximum distance the restrictedObject can be from the path
	/// </summary>
	public float MaximumAllowableDistance
	{
		get
		{
			return maximumAllowableDistance;
		}

		set
		{
			maximumAllowableDistance = value;
		}
	}

	/// <summary>
	/// The object to be tested for being out of bounds
	/// </summary>
	[HideInInspector]
	[SerializeField]
	private GameObject restrictedObject;

	/// <summary>
	/// The object to be tested for being out of bounds
	/// </summary>
	public GameObject RestrictedObject
	{
		get
		{
			return restrictedObject;
		}
		
		set
		{
			restrictedObject = value;
		}
	}

	/// <summary>
	/// The warning label in the GUI
	/// </summary>
	public Text warningLabel;

	// Update is called once per frame
	private void Update ()
	{
		if(showEditorLines)
		{
			DrawLines();
		}

		if(restrictedObject != null)
		{
			// Nodes to test must be within some minimum distance to prevent extreme slow down on large levels
			List<GameObject> nodesToTest = new List<GameObject>();

			for(int itr = 0; itr < mainPathNodeCount; itr++)
			{
				if(Vector3.Distance(mainPathNodes[itr].transform.position, restrictedObject.transform.position) < maximumCheckDistance)
				{
					nodesToTest.Add(mainPathNodes[itr]);
				}
			}

			List<Rejection> rejections = new List<Rejection>();

			// Test main path nodal vectors (lines between nodes in the main path)
			for(int itr = 0; itr < nodesToTest.Count - 1; itr++)
			{
				rejections.Add(new Rejection(restrictedObject, nodesToTest[itr], nodesToTest[itr + 1]));
			}

			// Test fork path nodal vectors (lines between nodes in fork paths)
			for(int itr = 0; itr < forkPathCount; itr++)
			{
				if(forkPaths[itr].internalPathLength == 0)
				{
					rejections.Add(new Rejection(restrictedObject, forkPaths[itr].mainStartNode, forkPaths[itr].mainEndNode));
				}
				else
				{
					// First and last
					rejections.Add(new Rejection(restrictedObject, forkPaths[itr].mainStartNode, forkPaths[itr].path[0]));
					rejections.Add(new Rejection(restrictedObject, forkPaths[itr].path[forkPaths[itr].internalPathLength - 1], forkPaths[itr].mainEndNode));

					for(int forkPathItr = 0; forkPathItr < forkPaths[itr].internalPathLength - 1; forkPathItr++)
					{
						rejections.Add(new Rejection(restrictedObject, forkPaths[itr].path[forkPathItr], forkPaths[itr].path[forkPathItr + 1]));
					}
				}
			}

			// Worst case scenario
			Vector3 distVector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

			foreach(Rejection rej in rejections)
			{
				// If this is a normal vector (touches nodal vector, not over or undershooting)
				if(!rej.undershoot && !rej.overshoot)
				{
					// If this is a better vector...
					if(rej.rejVector.magnitude < distVector.magnitude)
					{
						distVector = rej.rejVector;
					}
				}
			}

			// Test distances to main path nodes themselves
				// This will never be better than a straight shot to a nodal vector
					// The hypotenuse (restrictedObject to node), will always be smaller than the legs of the right triangle (in this case, the rejection is one of the legs)
			foreach(GameObject node in nodesToTest)
			{
				// If distances to vectors beat best distance to a nodal vector or other nodes...
				if(Vector3.Distance(restrictedObject.transform.position, node.transform.position) < distVector.magnitude)
				{
					distVector = restrictedObject.transform.position - node.transform.position;
				}
			}

			// Test distances to fork path nodes themselves
			for(int itr = 0; itr < forkPathCount; itr++)
			{
				for(int forkPathItr = 0; forkPathItr < forkPaths[itr].internalPathLength; forkPathItr++)
				{
					// If distances to vectors beat best distance to a nodal vector or other nodes...
					if(Vector3.Distance(restrictedObject.transform.position, forkPaths[itr].path[forkPathItr].transform.position) < distVector.magnitude)
					{
						distVector = restrictedObject.transform.position - forkPaths[itr].path[forkPathItr].transform.position;
					}
				}
			}

			// If out of acceptable bounds...
			if(distVector.magnitude > maximumAllowableDistance)
			{
				if(warningLabel != null)
					warningLabel.enabled = true;

				// Draw the best case distance
				Debug.DrawLine(restrictedObject.transform.position, restrictedObject.transform.position - distVector, Color.red);
			}
			else
			{
				if(warningLabel != null)
					warningLabel.enabled = false;

				// Draw the best case distance
				Debug.DrawLine(restrictedObject.transform.position, restrictedObject.transform.position - distVector, Color.green);
			}
		}
	}

	private void DrawLines()
	{
		// Debugging purposes: draw a line between consecutive nodes
		for(int itr = 0; itr < mainPathNodeCount - 1; itr++)
		{
			try
			{
				Debug.DrawLine(mainPathNodes[itr].transform.position, mainPathNodes[itr + 1].transform.position, mainPathLineColor);
			}
			catch
			{
				Debug.LogError("Error: main path node at index " + itr.ToString() + " could not be found!");
			}
		}
		
		// Draw forks
		for(int itr = 0; itr < forkPathCount; itr++)
		{
			// If no internal nodes...
			if(forkPaths[itr].internalPathLength == 0)
			{
				Debug.DrawLine(forkPaths[itr].mainStartNode.transform.position, forkPaths[itr].mainEndNode.transform.position, forkPathLineColor);
			}
			else
			{
				// Draw first and last line
				Debug.DrawLine(forkPaths[itr].mainStartNode.transform.position, forkPaths[itr].path[0].transform.position, forkPathLineColor);
				Debug.DrawLine(forkPaths[itr].path[forkPaths[itr].internalPathLength - 1].transform.position, forkPaths[itr].mainEndNode.transform.position, forkPathLineColor);
				
				// Draw internal lines
				for(int forkPathItr = 0; forkPathItr < forkPaths[itr].internalPathLength - 1; forkPathItr++)
				{
					Debug.DrawLine(forkPaths[itr].path[forkPathItr].transform.position, forkPaths[itr].path[forkPathItr + 1].transform.position, forkPathLineColor);
				}
			}
		}
	}

	#region Entry Manipulation
	/// <summary>
	/// Gets the entry
	/// </summary>
	/// <returns>The entry</returns>
	/// <param name="index">The entry's index</param>
	public GameObject GetMainPathNode(int index)
	{
		if(index >= 0 && index < mainPathNodeCount)
		{
			return mainPathNodes[index];
		}
		else
		{
			Debug.LogError("Error: DialogController.GetEntry received an invalid index");
			
			return null;
		}
	}
	
	/// <summary>
	/// Moves the entry upward in the array
	/// </summary>
	/// <param name="entryName">Entry name to move</param>
	public void MoveMainPathEntryUp(int index)
	{
		// Swap upward
		GameObject temp = mainPathNodes [index - 1];
		mainPathNodes [index - 1] = mainPathNodes [index];
		mainPathNodes [index] = temp;
	}
	
	/// <summary>
	/// Moves the entry downward in the array
	/// </summary>
	/// <param name="entryName">Entry name to move</param>
	public void MoveMainPathEntryDown(int index)
	{
		// Swap downward
		GameObject temp = mainPathNodes [index + 1];
		mainPathNodes [index + 1] = mainPathNodes [index];
		mainPathNodes [index] = temp;
	}

	/// <summary>
	/// Adds a main path node
	/// </summary>
	/// <param name="prevIndex">Previous node index</param>
	/// <param name="nextIndex">Next node index</param>
	/// <param name="loc">Location of node</param>
	public void AddMainPathNode(int prevIndex, int nextIndex, Vector3 loc)
	{
		// If first node...
		if(prevIndex < 0 && nextIndex < 0)
		{
			// Create new entry
			GameObject node = new GameObject("M1");
            node.AddComponent<BoundaryNode>();
			node.transform.position = loc;
			node.transform.parent = this.transform;
			
			mainPathNodes = new GameObject[1];
			
			// Add new value
			mainPathNodes [mainPathNodeCount] = node;
			
			// Increment count
			mainPathNodeCount++;
		}
		// If last node...
		else if(prevIndex >= 0 && nextIndex < 0)
		{
			// Create new entry
			GameObject node = new GameObject("M2");
            node.AddComponent<BoundaryNode>();
			node.transform.position = loc;
			node.transform.parent = this.transform;
			
			// Expand size by 1
			GameObject[] temp = mainPathNodes;
			
			mainPathNodes = new GameObject[mainPathNodeCount + 1];
			
			// Copy over values after size increase
			for(int itr = 0; itr < mainPathNodeCount; itr++)
			{
				mainPathNodes[itr] = temp[itr];
			}
			
			// Add new value
			mainPathNodes [mainPathNodeCount] = node;
			
			// Increment count
			mainPathNodeCount++;
		}
		// If any node in between...
		else
		{
			if(prevIndex != nextIndex)
			{
				AddMainPathNode(prevIndex, nextIndex);
				mainPathNodes[prevIndex + 1].transform.position = loc;
			}
			else
			{
				Debug.LogError("Error: Boundary checker received invalid main point entry to add (with location)!");
			}
		}
	}

	/// <summary>
	/// Adds a main path node
	/// </summary>
	/// <param name="prevIndex">Previous node index</param>
	/// <param name="nextIndex">Next node index</param>
	public void AddMainPathNode(int prevIndex, int nextIndex)
	{
		if(prevIndex >= 0 && nextIndex >= 0 && prevIndex != nextIndex)
		{
			GameObject node = new GameObject("M" + (prevIndex + 2).ToString());
            node.AddComponent<BoundaryNode>();
			node.transform.position = (mainPathNodes[prevIndex].transform.position + mainPathNodes[nextIndex].transform.position) / 2.0f;
			node.transform.parent = this.transform;

			// Expand size by 1
			GameObject[] temp = mainPathNodes;
			
			mainPathNodes = new GameObject[mainPathNodeCount + 1];
			
			// Copy over values after size increase and add new entry
			for(int itr = 0; itr < mainPathNodeCount + 1; itr++)
			{
				if(itr < prevIndex + 1)
				{
					mainPathNodes[itr] = temp[itr];
				}
				else if(itr == prevIndex + 1)
				{
					mainPathNodes[itr] = node;
				}
				else if(itr > prevIndex + 1)
				{
					mainPathNodes[itr] = temp[itr - 1];
					mainPathNodes[itr].transform.name = "M" + (itr + 1).ToString();
				}
			}

			// Increment count
			mainPathNodeCount++;
		}
		else
		{
			Debug.LogError("Error: Boundary checker received invalid main point entry to add!");
		}
	}

	/// <summary>
	/// Adds a main path node.
	/// </summary>
	/// <param name="prevIndex">Previous node index</param>
	/// <param name="nextIndex">Next node index</param>
	public void StartMainPath(Vector3 newStartLoc, Vector3 newEndLoc)
	{
		AddMainPathNode (-1, -1, newStartLoc);
		AddMainPathNode (0, -1, newEndLoc);
	}
	
	/// <summary>
	/// Deletes the node at a given index
	/// </summary>
	/// <param name="index">Index</param>
	public void DeleteMainPathNode(int index)
	{
		// New list for use after deletion
		GameObject[] reducedMainPath = new GameObject[mainPathNodeCount - 1];
		
		int reducedPathItr = 0;
		for(int itr = 0; itr < mainPathNodeCount; itr++)
		{
			// If value to delete...
			if(itr == index)
			{
				// Delete and do not copy
				if(Application.isPlaying)
				{
					Destroy(mainPathNodes[itr]);
				}
				else
				{
					DestroyImmediate(mainPathNodes[itr]);
				}

				continue;
			}
			// If not value to delete...
			else
			{
				// Copy to new list
				reducedMainPath[reducedPathItr] = mainPathNodes[itr];

				// Rename entries that are after the one deleted
				if(itr > index)
				{
					// Name is M# where # is the index assuming starting at 1
					reducedMainPath[reducedPathItr].transform.name = "M" + (reducedPathItr + 1).ToString();
				}

				reducedPathItr++;
			}
		}
		
		// Update list with temporary list
		mainPathNodes = reducedMainPath;
		
		// Reduce the number of entries
		mainPathNodeCount--;
	}

	/// <summary>
	/// Adds a new fork
	/// </summary>
	/// <param name="startNodeIndex">Start node index</param>
	/// <param name="endNodeIndex">End node index</param>
	public void AddFork(int startNodeIndex, int endNodeIndex)
	{
		Fork fork = new Fork ();

		// Set up fork
		fork.mainStartNode = mainPathNodes [startNodeIndex];
		fork.mainEndNode = mainPathNodes [endNodeIndex];
		fork.name = "F" + (forkPathCount + 1).ToString ();

		// Expand array by 1
		Fork[] temp = forkPaths;
		forkPaths = new Fork[forkPathCount + 1];

		// Copy existing forks
		for(int itr = 0; itr < forkPathCount; itr++)
		{
			forkPaths[itr] = temp[itr];
		}

		// Insert new fork
		forkPaths[forkPathCount] = fork;

		forkPathCount++;
	}

	/// <summary>
	/// Gets the fork at a given index
	/// </summary>
	/// <returns>The fork.</returns>
	/// <param name="index">Index.</param>
	public Fork GetFork(int index)
	{
		// If valid index...
		if(index >= 0 && index < forkPathCount)
		{
			return forkPaths [index];
		}
		else
		{
			Debug.LogError("Error: received index is invalid");
			return new Fork();
		}
	}

	/// <summary>
	/// Edits the fork at a given index
	/// </summary>
	/// <param name="forkIndex">Fork index</param>
	/// <param name="startNodeIndex">New start node index.</param>
	/// <param name="endNodeIndex">New end node index.</param>
	public void EditFork(int forkIndex, int startNodeIndex, int endNodeIndex)
	{
		// Update values
		forkPaths[forkIndex].mainStartNode = mainPathNodes [startNodeIndex];
		forkPaths[forkIndex].mainEndNode = mainPathNodes [endNodeIndex];
	}

	/// <summary>
	/// Deletes the fork at a given index
	/// </summary>
	/// <param name="index">Index to delete fork from</param>
	public void DeleteFork(int index)
	{
		if(index >= 0 && index < forkPathCount)
		{
			Fork forkToDelete = forkPaths[index];

			// Delete internal path
			for(int itr = 0; itr < forkToDelete.internalPathLength; itr++)
			{
				if(Application.isPlaying)
				{
					Destroy(forkToDelete.path[itr]);
				}
				else
				{
					DestroyImmediate(forkToDelete.path[itr]);
				}
			}

			// Rename other forks
			for(int itr = index + 1; itr < forkPathCount; itr++)
			{
				forkPaths[itr].name = "F" + (itr).ToString();

				for(int forkItr = 0; forkItr < forkPaths[itr].internalPathLength; forkItr++)
				{
					forkPaths[itr].path[forkItr].transform.name = forkPaths[itr].name + "-" + (forkItr + 1).ToString();
				}
			}

			// Shift to the left
			Fork[] temp = forkPaths;
			forkPaths = new Fork[forkPathCount - 1];

			for(int itr = 0; itr < forkPathCount; itr++)
			{
				if(itr < index)
				{
					forkPaths[itr] = temp[itr];
				}
				else if(itr > index)
				{
					forkPaths[itr - 1] = temp[itr];
				}
			}

			forkPathCount--;
		}
		else
		{
			Debug.LogWarning("Warning: DeleteFork received an index that is out of range.");
		}
	}

	/// <summary>
	/// Deletes the entire path. Not to be used lightly
	/// </summary>
	public void DeleteEntirePath()
	{
		for(int itr = 0; itr < mainPathNodeCount; itr++)
		{
			// Deleting depends on if the game is running or not
			if(Application.isPlaying)
			{
				Destroy (mainPathNodes[itr]);
			}
			else
			{
				DestroyImmediate (mainPathNodes[itr]);
			}
		}

		// Reset
		mainPathNodeCount = 0;
		mainPathNodes = new GameObject[0];

		// Delete fork paths
		for(int itr = 0; itr < forkPathCount; itr++)
		{
			try
			{
				DeleteFork(itr);
			}
			catch {}
		}

		forkPaths = new Fork[0];
		forkPathCount = 0;
	}
	#endregion
}
