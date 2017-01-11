using UnityEngine;
using System.Collections;

public abstract class MapCellEdge : MonoBehaviour {

    public Cell cell, otherCell;
    public MapDirection dir;

    //Set cell, otherCell and dir of passage or wall, depending on which called
    public void Initialise(Cell cell, Cell otherCell, MapDirection dir)
    {
        this.cell = cell;
        this.otherCell = otherCell;
        this.dir = dir;
        cell.GetComponent<Cell>().SetEdge(dir, this);
        //Correct positioning and rotation relative to cell
        transform.parent = cell.transform;
        transform.localPosition = Vector3.zero;
        transform.rotation = dir.ToRotation();
        //Accidentally made cell prefab the wrong direction, oops
        //this felt like a quicker fix that I may change later
        if (this.cell.tag == "Floor")
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}