using UnityEngine;
using System.Collections;

public abstract class MapCellEdge : MonoBehaviour {

    public Cell cell, otherCell;
    public MapDirection dir;

    public void Initialise(Cell cell, Cell otherCell, MapDirection dir)
    {
        this.cell = cell;
        this.otherCell = otherCell;
        this.dir = dir;
        cell.GetComponent<Cell>().SetEdge(dir, this);
        transform.parent = cell.transform;
        transform.localPosition = Vector3.zero;
        transform.rotation = dir.ToRotation();
        if (this.cell.tag == "Floor")
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}