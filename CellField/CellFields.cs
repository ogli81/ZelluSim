using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public class CellFields
    {

        /// <summary>
        /// Get the number of living neighbors (life value is true or > 0).
        /// </summary>
        public static int GetNumNeighbors(IBinaryCellField2D cells, int x, int y, bool wrap)
        {
            return wrap ? GetNumNeighborsWithWrap(cells, x, y) : GetNumNeighborsWithoutWrap(cells, x, y);
        }

        /// <summary>
        /// Get the number of living neighbors (life value is true or > 0).
        /// </summary>
        public static int GetNumNeighborsWithWrap(IBinaryCellField2D cells, int x, int y)
        {
            int n = 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.N) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.NE) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.E) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.SE) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.S) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.SW) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.W) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.NW) ? 1 : 0;
            return n;
        }

        /// <summary>
        /// Get the number of living neighbors (life value is true or > 0).
        /// </summary>
        public static int GetNumNeighborsWithoutWrap(IBinaryCellField2D cells, int x, int y, bool outsideVal = false)
        {
            int n = 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.N, outsideVal) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.NE, outsideVal) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.E, outsideVal) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.SE, outsideVal) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.S, outsideVal) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.SW, outsideVal) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.W, outsideVal) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.NW, outsideVal) ? 1 : 0;
            return n;
        }


        /// <summary>
        /// Get the number of living neighbors (life value is true or > 0).
        /// </summary>
        public static int GetNumNeighbors(IGenericCellField2D<byte> cells, int x, int y, bool wrap)
        {
            return wrap ? GetNumNeighborsWithWrap(cells, x, y) : GetNumNeighborsWithoutWrap(cells, x, y);
        }

        /// <summary>
        /// Get the number of living neighbors (life value is true or > 0).
        /// </summary>
        public static int GetNumNeighborsWithWrap(IGenericCellField2D<byte> cells, int x, int y)
        {
            int n = 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.N) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.NE) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.E) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.SE) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.S) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.SW) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.W) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.NW) > 0 ? 1 : 0;
            return n;
        }

        /// <summary>
        /// Get the number of living neighbors (life value is true or > 0).
        /// </summary>
        public static int GetNumNeighborsWithoutWrap(IGenericCellField2D<byte> cells, int x, int y)
        {
            int n = 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.N, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.NE, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.E, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.SE, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.S, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.SW, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.W, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.NW, 0) > 0 ? 1 : 0;
            return n;
        }
    }
}
