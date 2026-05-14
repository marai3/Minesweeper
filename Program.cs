using System.Diagnostics;
using Silk.NET.SDL;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TheAdventure;

public static class Program
{
    static int _winStreak = 0;
    static string _savePath = "streak.txt";
    public static void Main()
    {
        var sdl = new Sdl(new SdlContext());
        UInt64 framesRenderedCounter = 0;
       if (File.Exists(_savePath)) int.TryParse(File.ReadAllText(_savePath), out _winStreak);
        var table = new Table(18, 20, 35);
        int cellSize = 40;
        int offsetY = 60;      
        bool gameWon = false;
        bool gameOver = false;


        ReadOnlySpan<byte> keyboardState;
        unsafe
        {
            keyboardState = new(sdl.GetKeyboardState(null), (int)KeyCode.Count);
        }

        Span<byte> mouseButtonStates = stackalloc byte[(int)MouseButton.Count];

        var ev = new Event();

        var sdlInitResult = sdl.Init(Sdl.InitVideo | Sdl.InitAudio | Sdl.InitEvents | Sdl.InitTimer | Sdl.InitGamecontroller |
                                     Sdl.InitJoystick);
        if (sdlInitResult < 0)
        {
            throw new InvalidOperationException("Failed to initialize SDL.");
        }

        IntPtr window;
        unsafe
        {
            window = (IntPtr)sdl.CreateWindow(
                "The Adventure", Sdl.WindowposUndefined, Sdl.WindowposUndefined, 800, 800,
                (uint)WindowFlags.Resizable | (uint)WindowFlags.AllowHighdpi
            );

            if (window == IntPtr.Zero)
            {
                var ex = sdl.GetErrorAsException();
                if (ex != null)
                {
                    throw ex;
                }

                throw new Exception("Failed to create window.");
            }
        }
 
        IntPtr renderer;
        unsafe
        {
            renderer = (IntPtr)sdl.CreateRenderer((Window*)window, -1, (uint)RendererFlags.Accelerated);
            sdl.RenderSetVSync((Renderer*)renderer, 1);
        }

        if (renderer == IntPtr.Zero)
        {
            var ex = sdl.GetErrorAsException();
            if (ex != null)
            {
                throw ex;
            }

            throw new Exception("Failed to create renderer.");
        }
        var textures = new Dictionary<string, IntPtr>();
        unsafe IntPtr LoadTexture(string path)
        {
            try
            {
                using (var fStream = new FileStream(path, FileMode.Open))
                {
                    var image = Image.Load<Rgba32>(fStream);
                    var width = image.Width;
                    var height = image.Height;
                    var imageRAWData = new byte[width * height * 4];
                    image.CopyPixelDataTo(imageRAWData.AsSpan());
                    fixed (byte* data = imageRAWData)
                    {
                        var imageSurface = sdl.CreateRGBSurfaceWithFormatFrom(
                            data, 
                            width, 
                            height, 
                            8, 
                            width * 4, 
                            (uint)PixelFormatEnum.Rgba32);
                            
                        if (imageSurface == null) 
                        {
                            Console.WriteLine($"[Avertisment] Nu s-a putut crea suprafata pentru {path}");
                            return IntPtr.Zero;
                        }
                        var texture = sdl.CreateTextureFromSurface((Renderer*)renderer, imageSurface);
                        sdl.FreeSurface(imageSurface);
                        return (IntPtr)texture;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Nu am gasit fisierul: {path}");
                return IntPtr.Zero;
            }
        }
        textures["hidden"] = LoadTexture("Assets/hidden.bmp");
        textures["revealed"] = LoadTexture("Assets/revealed.bmp");
        textures["bomb"] = LoadTexture("Assets/bomb.bmp");
        textures["bomb_explode"] = LoadTexture("Assets/bomb_explode.bmp");
        textures["flag"] = LoadTexture("Assets/flag.bmp");
        textures["resume"] = LoadTexture("Assets/resume.bmp");
        textures["win"] = LoadTexture("Assets/win.bmp");

        for (int i = 1; i <= 8; i++)
            textures[i.ToString()] = LoadTexture($"Assets/{i}.bmp");

        bool quit = false;
        while (!quit)
        {
            while (sdl.PollEvent(ref ev) != 0)
            {
                if (ev.Type == (uint)EventType.Quit)
                {
                    quit = true;
                    break;
                }

                switch (ev.Type)
                {
                    case (uint)EventType.Windowevent:
                    {
                        switch (ev.Window.Event)
                        {
                            case (byte)WindowEventID.Shown:
                            case (byte)WindowEventID.Exposed:
                            {
                                break;
                            }
                            case (byte)WindowEventID.Hidden:
                            {
                                break;
                            }
                            case (byte)WindowEventID.Moved:
                            {
                                break;
                            }
                            case (byte)WindowEventID.SizeChanged:
                            {
                                break;
                            }
                            case (byte)WindowEventID.Minimized:
                            case (byte)WindowEventID.Maximized:
                            case (byte)WindowEventID.Restored:
                                break;
                            case (byte)WindowEventID.Enter:
                            {
                                break;
                            }
                            case (byte)WindowEventID.Leave:
                            {
                                break;
                            }
                            case (byte)WindowEventID.FocusGained:
                            {
                                break;
                            }
                            case (byte)WindowEventID.FocusLost:
                            {
                                break;
                            }
                            case (byte)WindowEventID.Close:
                            {
                                break;
                            }
                            case (byte)WindowEventID.TakeFocus:
                            {
                                unsafe
                                {
                                    sdl.SetWindowInputFocus(sdl.GetWindowFromID(ev.Window.WindowID));
                                }

                                break;
                            }
                        }

                        break;
                    }

                    case (uint)EventType.Fingermotion:
                    {
                        break;
                    }

                    case (uint)EventType.Mousemotion:
                    {
                        
                        break;
                    }

                    case (uint)EventType.Fingerdown:
                    {
                        break;
                    }
                    case (uint)EventType.Mousebuttondown:
                    {
                    if (ev.Type == (uint)EventType.Mousebuttondown )
                    {
                        int mx = ev.Button.X;
                        int my = ev.Button.Y;
                        if (mx >= 300 && mx <= 500 && my >= 10 && my <= 50)
                        {
                            table = new Table(18, 20, 35); 
                            gameOver = false;
                            gameWon = false;
                            continue; 
                        }
                    }
                    if(!gameOver && !gameWon)
                    {
                        int c = ev.Button.X / cellSize;
                        int r = (ev.Button.Y - offsetY) / cellSize;

                        if (c >= 0 && c < table.Columns && r >= 0 && r < table.Rows)
                        {
                            if (ev.Button.Button == (byte)MouseButton.Primary)
                            {
                                if (table.Cells[c, r].Mine) 
                                { 
                                    gameOver = true; 
                                    _winStreak = 0;
                                    File.WriteAllText(_savePath, "0"); 
                                }
                                else 
                                {
                                    table.FloodReveal(c, r);
                                    if (table.CheckWinCondition()) 
                                    { 
                                        gameWon = true; 
                                        _winStreak++;
                                        File.WriteAllText(_savePath, _winStreak.ToString());
                                    }
                                }
                            }
                            else if (ev.Button.Button == (byte)MouseButton.Secondary)
                            {
                                table.Cells[c, r].Flag = !table.Cells[c, r].Flag;
                            }
                        }
                    }
                        break;
                    }
                  
                        

                    case (uint)EventType.Fingerup:
                    {
                        mouseButtonStates[(byte)MouseButton.Primary] = 0;
                        break;
                    }

                    case (uint)EventType.Mousebuttonup:
                    {
                        mouseButtonStates[ev.Button.Button] = 0;
                        break;
                    }

                    case (uint)EventType.Mousewheel:
                    {
                        break;
                    }

                    case (uint)EventType.Keyup:
                    {
                        break;
                    }

                    case (uint)EventType.Keydown:
                    {
                        Console.WriteLine($"Key down: {(KeyCode)ev.Key.Keysym.Scancode}");
                        break;
                    }
                }
            }


            // game.render(renderer, RenderEvent{ elapsed, framesRenderedCounter++ });
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
            ++framesRenderedCounter;
            
        }

        unsafe
        {   
            foreach (var tex in textures.Values)
            {
                if (tex != IntPtr.Zero) 
                {
                    sdl.DestroyTexture((Texture*)tex);
                }
            }
            sdl.DestroyRenderer((Renderer*)renderer);
            sdl.DestroyWindow((Window*)window);
        }

        sdl.Quit();
    }
}
