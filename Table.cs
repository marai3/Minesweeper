using System;
using System.Linq;

namespace TheAdventure;

public class Table
{
    public Cell[,] Cells {get;}
    public int Rows {get;}
    public int Columns {get;}

    public Table(int rows, int columns, int mineCount)
    {
        Rows = rows;
        Columns = columns;
        Cells = new Cell[columns, rows];
        InitializeBoard();
        PlaceMines(mineCount);
        CalculateAdjacents();
    }

    private void InitializeBoard()
    {
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                Cells[x, y] = new Cell();
            }
        }
    }
    public void PlaceMines(int mineCount)
    {
        var rand = new Random();
        int placed = 0;
        
        while (placed < mineCount)
        {
            int x = rand.Next(Columns);
            int y = rand.Next(Rows);
            
            if (!Cells[x, y].Mine)
            {
                Cells[x, y].Mine = true;
                placed++;
            }
        }
    }

    private void CalculateAdjacents()
    {
       for (int c = 0; c < Columns; c++)
        {
            for (int r = 0; r < Rows; r++)
            {
                if (Cells[c, r].Mine) continue;
                
                int count = 0;
                for (int dc = -1; dc <= 1; dc++)
                {
                    for (int dr = -1; dr <= 1; dr++)
                    {
                        int nc = c + dc;
                        int nr = r + dr;
                        
                        if (nc >= 0 && nc < Columns && nr >= 0 && nr < Rows && Cells[nc, nr].Mine)
                        {
                            count++;
                        }
                    }
                }
                Cells[c, r].NrMines = count;
            }
        }
    }
    public bool CheckWinCondition()
    {
        return Cells.Cast<Cell>().Where(c => !c.Mine).All(c => c.Revealed);
    }
//AI generated
    public void FloodReveal(int startC, int startR)
    {
        var queue = new Queue<(int c, int r)>();
        queue.Enqueue((startC, startR));

        while (queue.Count > 0)
        {
            var (c, r) = queue.Dequeue();
            if (c < 0 || c >= Columns || r < 0 || r >= Rows) continue;
            if (Cells[c, r].Revealed || Cells[c, r].Mine || Cells[c, r].Flag) continue;

            Cells[c, r].Revealed = true;

            if (Cells[c, r].NrMines == 0)
            {
                for (int dc = -1; dc <= 1; dc++)
                    for (int dr = -1; dr <= 1; dr++)
                        if (dc != 0 || dr != 0)
                            queue.Enqueue((c + dc, r + dr));
            }
        }
    }
//end of AI generated
    public void Reset(int mineCount)
    {
        for (int x = 0; x < Columns; x++)
            for (int y = 0; y < Rows; y++)
                Cells[x, y] = new Cell();
        PlaceMines(mineCount);
        CalculateAdjacents();
    }
}