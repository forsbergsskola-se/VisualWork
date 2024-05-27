using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Collections;
using Grids;
using UnityEngine;
using Grid = Grids.Grid;

public class PlayerController : MonoBehaviour
{
    public GameObject gold;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Grid grid = FindObjectOfType<Grid>();
            GridCell start = grid.GetCellForPosition(transform.position);
            GridCell end = grid.GetCellForPosition(gold.transform.position);
            var path = FindPath_AStar(grid, start, end);
            foreach (var node in path)
            {
                node.spriteRenderer.color = Color.green;
            }

            StartCoroutine(Co_WalkPath(path));
        }
    }

    IEnumerator Co_WalkPath(IEnumerable<GridCell> path)
    {
        foreach (var cell in path)
        {
            while (Vector2.Distance(transform.position, cell.transform.position) > 0.001f)
            {
                Vector3 targetPosition = Vector2.MoveTowards(transform.position, cell.transform.position, Time.deltaTime);
                targetPosition.z = transform.position.z;
                transform.position = targetPosition;
                yield return null;
            }
        }
    }

    // Update is called once per frame
    static IEnumerable<GridCell> FindPath_DepthFirst(Grid grid, GridCell start, GridCell end)
    {
        Stack<GridCell> path = new Stack<GridCell>();
        HashSet<GridCell> visited = new HashSet<GridCell>();
        path.Push(start);
        visited.Add(start);

        while (path.Count > 0)
        {
            bool foundNextNode = false;
            foreach (var neighbor in grid.GetWalkableNeighborsForCell(path.Peek()))
            {
                if (visited.Contains(neighbor)) continue;
                path.Push(neighbor);
                visited.Add(neighbor);
                neighbor.spriteRenderer.color = Color.cyan;
                if (neighbor == end) return path.Reverse(); // <<<<<<<<<<
                foundNextNode = true;
                break;
            }

            if (!foundNextNode)
                path.Pop();
        }

        return null;
    }
    
    static IEnumerable<GridCell> FindPath_BreadthFirst(Grid grid, GridCell start, GridCell end)
    {
        Queue<GridCell> todo = new();                         // STACK -> QUEUE
        HashSet<GridCell> visited = new();
        todo.Enqueue(start);                                  // SAME, BUT DIFFERENT
        visited.Add(start);
        Dictionary<GridCell, GridCell> previous = new();      // NEW, TRACK PREVIOUS NEED

        while (todo.Count > 0)                                 // SAME, BUT DIFFERENT
        {
            //bool foundNextNode = false;
            var current = todo.Dequeue();               // PEEK -> DEQUEUE, SEPARATE VARIABLE
            foreach (var neighbor in grid.GetWalkableNeighborsForCell(current)) // USE VARIABLE
            {
                if (visited.Contains(neighbor)) continue;
                todo.Enqueue(neighbor);                        // SAME, BUT DIFFERENT
                previous[neighbor] = current;                  // NEW: KEEP TRACK OF WHERE WE CAME FROM
                visited.Add(neighbor);
                neighbor.spriteRenderer.color = Color.cyan;
                if (neighbor == end) 
                    return TracePath(neighbor, previous).Reverse(); // NEW: BUILD PATH
                //foundNextNode = true;
                //break;
            }

            //if (!foundNextNode)
            //    path.Pop();
        }

        return null;
    }
    
    // CLONE OF BREADTH-FIRST-SEARCH
    static IEnumerable<GridCell> FindPath_Dijkstra(Grid grid, GridCell start, GridCell end)
    {
        PriorityQueue<GridCell> todo = new(); // QUEUE -> PRIORITY_QUEUE
        todo.Enqueue(start, 0); // START = 0 COSTS
        Dictionary<GridCell, int> costs = new(); // HASHSET -> DICTIONARY
        costs[start] = 0; // START = 0 COSTS
        Dictionary<GridCell, GridCell> previous = new();

        while (todo.Count > 0)
        {
            var current = todo.Dequeue();
            if (current == end) // IF THE END NODE GOT OUT OF THE QUEUE WITH HIGHEST PRIORITY
                return TracePath(current, previous).Reverse(); // IT MEANS WE FOUND THE FASTEST PATH
            
            foreach (var neighbor in grid.GetWalkableNeighborsForCell(current))
            {
                int newNeighborCosts = costs[current] + neighbor.Costs; // CALCULATE NEW PATH COSTS
                if (costs.TryGetValue(neighbor, out int neighborCosts) && // CHECK IF THE NODE HAD COSTS
                    neighborCosts <= newNeighborCosts) continue; // AND IF THE COSTS WERE MORE EFFICIENT
                                                                 // THAN NEW COSTS THEN SKIP THIS NODE
                                                                 
                todo.Enqueue(neighbor, newNeighborCosts);        // PROVIDE THE NEW PATH COSTS
                previous[neighbor] = current;
                costs[neighbor] = newNeighborCosts;
                
                neighbor.spriteRenderer.ShiftBrightness(0.4f); // INSTEAD OF COLOR = CYAN
                
                //if (neighbor == end)  moved to start
                //    return TracePath(neighbor, previous).Reverse();
            }
        }

        return null;
    }

    static int GetEstimatedCosts(GridCell from, GridCell to)
    {
        // Pessimistic Heuristic: Vector3.SqrMagnitude(from.transform.position - to.transform.position);
        // Optimistic  Heuristic: return 0;
        return Mathf.RoundToInt(Vector3.Distance(from.transform.position, to.transform.position));
    }
    
    static IEnumerable<GridCell> FindPath_BestFirst(Grid grid, GridCell start, GridCell end)
    {
        PriorityQueue<GridCell> todo = new();
        todo.Enqueue(start, 0);
        Dictionary<GridCell, int> costs = new();
        costs[start] = GetEstimatedCosts(start, end); // START = Estimated Distance Costs
        Dictionary<GridCell, GridCell> previous = new();

        while (todo.Count > 0)
        {
            var current = todo.Dequeue();
            if (current == end)
                return TracePath(current, previous).Reverse();
            
            foreach (var neighbor in grid.GetWalkableNeighborsForCell(current))
            {
                int newNeighborCosts = costs[current] + neighbor.Costs;
                if (costs.TryGetValue(neighbor, out int neighborCosts) &&
                    neighborCosts <= newNeighborCosts) continue;
                                                                 
                todo.Enqueue(neighbor, GetEstimatedCosts(neighbor, end)); // Get Estimate
                previous[neighbor] = current;
                costs[neighbor] = newNeighborCosts;
                
                neighbor.spriteRenderer.ShiftBrightness(0.4f);
            }
        }

        return null;
    }
    
    static IEnumerable<GridCell> FindPath_AStar(Grid grid, GridCell start, GridCell end)
    {
        PriorityQueue<GridCell> todo = new();
        todo.Enqueue(start, 0);
        Dictionary<GridCell, int> costs = new();
        costs[start] = 0 + GetEstimatedCosts(start, end); // Costs To Here + Estimated Rem. Costs
        Dictionary<GridCell, GridCell> previous = new();

        while (todo.Count > 0)
        {
            var current = todo.Dequeue();
            if (current == end)
                return TracePath(current, previous).Reverse();
            
            foreach (var neighbor in grid.GetWalkableNeighborsForCell(current))
            {
                int newNeighborCosts = costs[current] + neighbor.Costs;
                if (costs.TryGetValue(neighbor, out int neighborCosts) &&
                    neighborCosts <= newNeighborCosts) continue;
                                                                    
                todo.Enqueue(neighbor, newNeighborCosts + GetEstimatedCosts(neighbor, end));  // Costs To Here + Estimated Rem. Costs
                previous[neighbor] = current;
                costs[neighbor] = newNeighborCosts;
                
                neighbor.spriteRenderer.ShiftBrightness(0.4f);
            }
        }

        return null;
    }

    private static IEnumerable<GridCell> TracePath(GridCell neighbor, Dictionary<GridCell, GridCell> previous)
    {
        while (true)
        {
            yield return neighbor;
            if (!previous.TryGetValue(neighbor, out neighbor))
                yield break;
        }
    }
}

public static class SpriteRendererExtensions
{
    public static void ShiftHue(this SpriteRenderer spriteRenderer, float hue)
    {
        Color.RGBToHSV(spriteRenderer.color, out var h, out var s, out var v);
        spriteRenderer.color = Color.HSVToRGB((h + hue)%1, s, v);
    }
    
    public static void ShiftBrightness(this SpriteRenderer spriteRenderer, float brightness)
    {
        Color.RGBToHSV(spriteRenderer.color, out var h, out var s, out var v);
        if (v > 0.5f)
        {
            spriteRenderer.color = Color.HSVToRGB(h, s, (v-brightness)%1f);
        }
        else
        {
            spriteRenderer.color = Color.HSVToRGB(h, s, (v+brightness)%1f);
        }
    }
}
