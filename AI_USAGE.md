Tool-uri utilizate: Gemini 1.5 pro și Claude Sonnet 4.6
Am folosit gemini la debug deoarece aveam butonul de resume dar cand era apasat nu dadea resume la joc.

Am folosit Gemini pentru a scrie logica de FloodReveal - de a afisa celulele adiacente goale

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

Am folosit Claude pentru a refactoriza codul pentru randare:
            unsafe
            {
                var r = (Renderer*)renderer;
                sdl.SetRenderDrawColor(r, 0, 0, 0, 255);
                sdl.RenderClear(r);
                for (int x = 0; x < table.Columns; x++)
                {
                    for (int y = 0; y < table.Rows; y++)
                    {
                        var cell = table.Cells[x, y];
                        string texKey = "hidden"; 

                        if (cell.Revealed)
                        {
                            if (cell.Mine) 
                                texKey = "bomb_explode"; 
                            else if (cell.NrMines > 0) 
                                texKey = cell.NrMines.ToString(); 
                            else 
                                texKey = "revealed"; 
                        }
                        else if (cell.Flag)
                        {
                            texKey = "flag";
                        }
                        else if (gameOver && cell.Mine)
                        {
                            texKey = "bomb";
                        }
                        var rect = new Silk.NET.Maths.Rectangle<int>(x * cellSize,y * cellSize + offsetY, cellSize, cellSize);
                        if (textures.ContainsKey(texKey) && textures[texKey] != IntPtr.Zero)
                        {
                            sdl.RenderCopy(r, (Texture*)textures[texKey], null, ref rect);
                        }
                    }
                }
                sdl.SetRenderDrawColor(r, 50, 50, 50, 255);
                var bar = new Silk.NET.Maths.Rectangle<int>(0, 0, 800, offsetY);
                sdl.RenderFillRect(r, ref bar);

                if (textures.ContainsKey("resume") && textures["resume"] != IntPtr.Zero)
                {
                    var btn = new Silk.NET.Maths.Rectangle<int>(300, 10, 200, 40);
                    sdl.RenderCopy(r, (Texture*)textures["resume"], null, ref btn);
                }

                if (gameWon)
                {
                    if (textures.ContainsKey("win") && textures["win"] != IntPtr.Zero)
                    {
                        var winBox = new Silk.NET.Maths.Rectangle<int>(200, 350, 400, 100);
                        sdl.RenderCopy(r, (Texture*)textures["win"], null, ref winBox);
                    }
                }
                string streakStr = _winStreak.ToString();
                for (int i = 0; i < streakStr.Length; i++)
                {
                    string digit = streakStr[i].ToString();
                    var streakRect = new Silk.NET.Maths.Rectangle<int>(750 + (i * 15), 15, 15, 25);
                    if (textures.ContainsKey(digit))
                    {
                        sdl.RenderCopy(r, (Texture*)textures[digit], null, ref streakRect);
                    }
                }
                   sdl.RenderPresent(r);
                System.Threading.Thread.Sleep(13);
            }
          
            
        

