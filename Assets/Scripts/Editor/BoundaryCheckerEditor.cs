using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BoundaryChecker))]
public class BoundaryCheckerEditor : Editor
{
	/// <summary>
	/// The new path first node location
	/// </summary>
	private Vector3 newStartLoc = Vector3.zero;

	/// <summary>
	/// The new path end node location
	/// </summary>
	private Vector3 newEndLoc = Vector3.zero;

	/// <summary>
	/// Flag for delete confirmation checkbox
	/// </summary>
	private bool deletePathConfirm = false;

	/// <summary>
	/// The index of the selected main path node
	/// </summary>
	private int selectedMainPathIndex = 0;

	/// <summary>
	/// Flag for determining if should use a custom location for a new vector (true) or
	/// if the location should be halfway between the previous and next vectors (false)
	/// </summary>
	private bool useCustomLocation = false;

	/// <summary>
	/// The custom location to use if the useCustomLocation is set to true
	/// </summary>
	private Vector3 customLocation = Vector3.zero;

	/// <summary>
	/// The index of the selected fork start node
	/// </summary>
	private int selectedForkStartNodeIndex = 0;

	/// <summary>
	/// The index of the selected fork end node
	/// </summary>
	private int selectedForkEndNodeIndex = 1;

	/// <summary>
	/// The index of the selected fork
	/// </summary>
	private int selectedForkIndex = 0;

	/// <summary>
	/// The index of the selected fork node.
	/// </summary>
	private int selectedForkNodeIndex = 0;

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		// Get BoundaryChecker object
		BoundaryChecker checker = (BoundaryChecker)target;

		// Info about the object restricted by the path
        CustomEditorUtils.HeaderStart("Restricted Object");

		checker.RestrictedObject = (GameObject)EditorGUILayout.ObjectField("Restricted Object", checker.RestrictedObject, typeof(GameObject), true);
		checker.MaximumCheckDistance = EditorGUILayout.FloatField("Max Node Check Distance", checker.MaximumCheckDistance);
		checker.MaximumAllowableDistance = EditorGUILayout.FloatField("Max Distance From Path", checker.MaximumAllowableDistance);

        CustomEditorUtils.HeaderEnd();

        CustomEditorUtils.HeaderStart("Clear Path");

		deletePathConfirm = EditorGUILayout.ToggleLeft("Delete ENTIRE Path?", deletePathConfirm);
			
		// If user checked the confirm delete checkbox...
		if(deletePathConfirm)
		{
			// If user pressed delete button...
			if(GUILayout.Button("DELETE?"))
			{
				checker.DeleteEntirePath();
				deletePathConfirm = false;
			}
		}

        CustomEditorUtils.HeaderEnd();


		// Path editor
        CustomEditorUtils.HeaderStart("Path Editor");

		// If no path exists...
		if(checker.MainPathNodeCount == 0)
		{
			newStartLoc = EditorGUILayout.Vector3Field("Start Node Location", newStartLoc);
			newEndLoc = EditorGUILayout.Vector3Field("End Node Location", newEndLoc);

			// If button pressed to create path...
			if(GUILayout.Button("Start New Path"))
			{
				checker.StartMainPath(newStartLoc, newEndLoc);
			}
		}
		else
		{
			string[] nodeNames = new string[checker.MainPathNodeCount];

			// Get the GameObject names for every node
			for(int itr = 0; itr < checker.MainPathNodeCount; itr++)
			{
				nodeNames[itr] = checker.GetMainPathNode(itr).transform.name;
			}

			// Selected node in the list
			selectedMainPathIndex = EditorGUILayout.Popup("Node", selectedMainPathIndex, nodeNames);

			// Fix the index if it is out of bounds
			if(selectedMainPathIndex < 0)
				selectedMainPathIndex = 0;
			else if(selectedMainPathIndex >= checker.MainPathNodeCount)
				selectedMainPathIndex = checker.MainPathNodeCount - 1;

			EditorGUILayout.Separator();

			// Handle using custom node location
			useCustomLocation = EditorGUILayout.ToggleLeft("Use Custom Location", useCustomLocation);

			if(useCustomLocation)
			{
				customLocation = EditorGUILayout.Vector3Field("Custom Location", customLocation);
			}

			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();

			// If not the first node...
			GUI.enabled = selectedMainPathIndex != 0;
			if(GUILayout.Button("Create New Before"))
			{
				if(useCustomLocation)
				{
					checker.AddMainPathNode(selectedMainPathIndex - 1, selectedMainPathIndex, customLocation);
				}
				else
				{
					checker.AddMainPathNode(selectedMainPathIndex - 1, selectedMainPathIndex);
				}
			}
			GUI.enabled = true;

			// If not the last node...
			GUI.enabled = selectedMainPathIndex != checker.MainPathNodeCount - 1;
			if(GUILayout.Button("Create New After"))
			{
				if(useCustomLocation)
				{
					checker.AddMainPathNode(selectedMainPathIndex, selectedMainPathIndex + 1, customLocation);
				}
				else
				{
					checker.AddMainPathNode(selectedMainPathIndex, selectedMainPathIndex + 1);
				}

				// Select the newly added node
				selectedMainPathIndex++;
			}
			GUI.enabled = true;

			// If not the first or last node...
			GUI.enabled = selectedMainPathIndex != 0 && selectedMainPathIndex != checker.MainPathNodeCount - 1;
			if(GUILayout.Button("Delete Node"))
			{
				checker.DeleteMainPathNode(selectedMainPathIndex);
			}
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();

			// Allows for the selection of a node in the inspector
			if(GUILayout.Button("Select Node"))
			{
				Selection.activeGameObject = checker.GetMainPathNode(selectedMainPathIndex);
			}

			EditorGUILayout.Separator();

			// Creating forks in the main path
			string[] startNodeNames = new string[checker.MainPathNodeCount - 1];

			for(int itr = 0; itr < checker.MainPathNodeCount - 1; itr++)
			{
				startNodeNames[itr] = checker.GetMainPathNode(itr).transform.name;
			}

			selectedForkStartNodeIndex = EditorGUILayout.Popup("Start Node", selectedForkStartNodeIndex, startNodeNames);

			// Fix index if out of bounds
			if(selectedForkStartNodeIndex < 0)
				selectedForkStartNodeIndex = 0;
			else if(selectedForkStartNodeIndex >= checker.MainPathNodeCount - 1)
				selectedForkStartNodeIndex = checker.MainPathNodeCount - 2;

			string[] endNodeNames = new string[checker.MainPathNodeCount - (selectedForkStartNodeIndex + 1)];

			for(int itr = selectedForkStartNodeIndex + 1; itr < checker.MainPathNodeCount; itr++)
			{
				endNodeNames[itr - (selectedForkStartNodeIndex + 1)] = checker.GetMainPathNode(itr).transform.name;
			}

			selectedForkEndNodeIndex = EditorGUILayout.Popup("End Node", selectedForkEndNodeIndex, endNodeNames);

			// Fix index if it is out of bounds
			if(selectedForkEndNodeIndex < 0)
				selectedForkEndNodeIndex = 0;
			else if(selectedForkEndNodeIndex >= checker.MainPathNodeCount - (selectedForkStartNodeIndex + 1))
				selectedForkEndNodeIndex = checker.MainPathNodeCount - (selectedForkStartNodeIndex + 2);

			string[] forkNames = new string[checker.ForkPathCount + 1];

			// Get existing forks
			for(int itr = 0; itr < checker.ForkPathCount; itr++)
			{
				forkNames[itr] = checker.GetFork(itr).name;
			}

			forkNames[checker.ForkPathCount] = "NEW FORK";

			selectedForkIndex = EditorGUILayout.Popup("Fork", selectedForkIndex, forkNames);

			// Fix the index if it is out of bounds
			if(selectedForkIndex < 0)
				selectedForkIndex = 0;
			else if(selectedForkIndex >= checker.ForkPathCount + 1)
				selectedForkIndex = checker.ForkPathCount;

			EditorGUILayout.BeginHorizontal();

			// If not NEW FORK entry
			if(selectedForkIndex != checker.ForkPathCount)
			{
				if(GUILayout.Button("Edit Fork"))
				{
					checker.EditFork(selectedForkIndex, selectedForkStartNodeIndex, selectedForkEndNodeIndex + selectedForkStartNodeIndex + 1);
				}
			}
			else
			{
				if(GUILayout.Button("Add Fork"))
				{
					checker.AddFork(selectedForkStartNodeIndex, selectedForkEndNodeIndex + selectedForkStartNodeIndex + 1);
				}
			}

			// If not the NEW FORK entry
			GUI.enabled = selectedForkIndex != checker.ForkPathCount;
			if(GUILayout.Button("Delete Fork"))
			{
				checker.DeleteFork(selectedForkIndex);
			}
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();

			// If not the NEW FORK entry, show fork node editor
			if(selectedForkIndex != checker.ForkPathCount)
			{
				EditorGUILayout.Separator();

				string[] forkNodes = new string[checker.GetFork(selectedForkIndex).internalPathLength + 2];

				forkNodes[0] = checker.GetFork(selectedForkIndex).mainStartNode.transform.name;

				for(int itr = 1; itr < checker.GetFork(selectedForkIndex).internalPathLength + 1; itr++)
				{
					forkNodes[itr] = checker.GetFork(selectedForkIndex).path[itr - 1].transform.name;
				}

				forkNodes[checker.GetFork(selectedForkIndex).internalPathLength + 1] = checker.GetFork(selectedForkIndex).mainEndNode.transform.name;

				selectedForkNodeIndex = EditorGUILayout.Popup("Fork Nodes", selectedForkNodeIndex, forkNodes);

				if(selectedForkNodeIndex < 0)
					selectedForkNodeIndex = 0;
				else if(selectedForkNodeIndex >= checker.GetFork(selectedForkIndex).internalPathLength + 2)
					selectedForkNodeIndex = checker.GetFork(selectedForkIndex).internalPathLength;

				EditorGUILayout.BeginHorizontal();
						
				// If not the first node...
				GUI.enabled = selectedForkNodeIndex != 0;
				if(GUILayout.Button("Create New Before"))
				{
					checker.GetFork(selectedForkIndex).AddForkNode(selectedForkNodeIndex - 1, selectedForkNodeIndex);
				}
				GUI.enabled = true;
						
				// If not the last node...
				GUI.enabled = selectedForkNodeIndex != checker.GetFork(selectedForkIndex).internalPathLength + 1;
				if(GUILayout.Button("Create New After"))
				{
					checker.GetFork(selectedForkIndex).AddForkNode(selectedForkNodeIndex, selectedForkNodeIndex + 1);

					selectedForkNodeIndex++;
				}
				GUI.enabled = true;
						
				// If not the first or last node...
				GUI.enabled = selectedForkNodeIndex != 0 && selectedForkNodeIndex != checker.GetFork(selectedForkIndex).internalPathLength + 1;
				if(GUILayout.Button("Delete Node"))
				{
					checker.GetFork(selectedForkIndex).DeleteForkNode(selectedForkNodeIndex - 1);
				}
				GUI.enabled = true;
						
				EditorGUILayout.EndHorizontal();
			}

            CustomEditorUtils.HeaderEnd();
		}

		// Debugging info
		checker.ShowEditorLines = EditorGUILayout.ToggleLeft("Show Debug Lines", checker.ShowEditorLines);

		if(checker.ShowEditorLines)
		{
			checker.MainPathLineColor = EditorGUILayout.ColorField("Main Path Color", checker.MainPathLineColor);
			checker.ForkPathLineColor = EditorGUILayout.ColorField("Fork Path Color", checker.ForkPathLineColor);
		}
	}
}
