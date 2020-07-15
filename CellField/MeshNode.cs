using System;
using System.Runtime.CompilerServices;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public class MeshNode<T>
    {
        //state:

        public T Value;
        public readonly MeshNode<T>[] neighbors;
        

        //c'tors:

        public MeshNode()
        {
            neighbors = new MeshNode<T>[8];
        }

        public MeshNode(T value) : this()
        {
            this.Value = value;
        }

        
        //public methods:
        
        public MeshNode<T> GetNeighbor(Direction dir)
        {
            switch (dir)
            {
                case Direction.N: return neighbors[0];
                case Direction.NE: return neighbors[1];
                case Direction.E: return neighbors[2];
                case Direction.SE: return neighbors[3];
                case Direction.S: return neighbors[4];
                case Direction.SW: return neighbors[5];
                case Direction.W: return neighbors[6];
                case Direction.NW: return neighbors[7];
                default: throw new ArgumentException($"unknown direction: {dir}");
            }
        }

        public void SetNeighbor(Direction dir, MeshNode<T> neighbor)
        {
            switch (dir)
            {
                case Direction.N: neighbors[0] = neighbor; return; //is it faster than break; ?
                case Direction.NE: neighbors[1] = neighbor; return;
                case Direction.E: neighbors[2] = neighbor; return;
                case Direction.SE: neighbors[3] = neighbor; return;
                case Direction.S: neighbors[4] = neighbor; return;
                case Direction.SW: neighbors[5] = neighbor; return;
                case Direction.W: neighbors[6] = neighbor; return;
                case Direction.NW: neighbors[7] = neighbor; return;
                default: throw new ArgumentException($"unknown direction: {dir}");
            }
        }
    }
}
