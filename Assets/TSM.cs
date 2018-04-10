using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSM : MonoBehaviour {

    public int numberOfCities = 10;
    public float radius = 100.0f;
    public Transform nodePrefab;
    public LineRenderer linePrefab;
    
	// return the positive clockwise from one vector to another
	float PositiveAngle(Vector2 from, Vector2 to)
	{
		float angle = Vector2.SignedAngle(from, to);
		if (angle < 0.0f) {
			angle += 360.0f;
		}
		return angle;
	}

	List<Vector2> RandomCityPositions(int cities, float radius) {
		List<Vector2> nodes = new List<Vector2>();
		for (int i = 0; i < cities; ++i) {
			nodes.Add(Random.insideUnitCircle * radius);
		}
		return nodes;
	}

	List<Vector2> CreateRing(List<Vector2> nodes) {
		List<Vector2> ring = new List<Vector2>();
		var firstNode = nodes[0];
		ring.Add(firstNode);
		Vector2 currentDirection = Vector2.right;
		Vector2 currentNode = firstNode;
		bool skipFirst = true;
		while (nodes.Count > 0) {
			// find the next point in the ring
			float smallestAngle = Mathf.Infinity;
			float smallestDistance = Mathf.Infinity;
			Vector2 nextNode = currentNode;
			Vector2 nextNodeDirection = currentDirection;
			foreach (var node in nodes) {
				if (skipFirst) {
					skipFirst = false;
					continue;
				}
				Vector2 nodeDirection = node - currentNode;
				float distance = Vector2.Distance(node, currentNode);
				if (distance > 0.0f) {
					float angle = PositiveAngle(currentDirection, nodeDirection);
					if (angle < smallestAngle || (angle == smallestAngle && distance < smallestDistance)) {
						smallestAngle = angle;
						smallestDistance = distance;
						nextNode = node;
						nextNodeDirection = nodeDirection;
					}
				}
				else {
					// catch duplicate node
					smallestAngle = 0.0f;
					smallestDistance = distance;
					nextNode = node;
					nextNodeDirection = currentDirection;
					break;
				}
			}
			if (ring.Count > 1) {
				Debug.Assert(smallestAngle < Mathf.Infinity);
				Debug.Assert(smallestDistance < Mathf.Infinity);
				Debug.Assert(nextNode != currentNode);
				Debug.Assert(nextNodeDirection != Vector2.zero);
			}
			ring.Add(nextNode);
			nodes.Remove(nextNode);
			currentNode = nextNode;
			currentDirection = nextNodeDirection;
			if (currentNode == firstNode) {
				break;
			}
		}
		return ring;
	}

    void Start() {
		List<Vector2> nodes = RandomCityPositions (numberOfCities, radius);
		// order by y so that the highest point is first        
		nodes.Sort((a, b) => b.y.CompareTo(a.y));
		foreach (var node in nodes) {
			Instantiate(nodePrefab, node, Quaternion.identity);
		}
        // take the first point and create a ring starting at that point
        while (nodes.Count > 0) {
			Outline (CreateRing (nodes));
        }
    }

	void Outline(List<Vector2> ring)
	{
		var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
		line.positionCount = ring.Count;
		List<Vector3> positions = new List<Vector3>();
		foreach (var position in ring) {
			positions.Add(position);
		}
		line.SetPositions(positions.ToArray());
	}
}
