# Minesweeper

Joc de Minesweeper (Detectorul de mine) în C#, construit peste un schelet SDL2 (Silk.NET). Grilă de 20x18 celule, cu 35 de mine plasate aleator, flag-uire cu click dreapta, dezvăluire în lanț (flood fill) pentru celulele fără mine adiacente, și o serie de câștiguri consecutive (win streak) salvată local între sesiuni.

![Gameplay](gameplay.png)

---

## Funcționalități

- Grilă 20x18, cu 35 de mine plasate aleator la fiecare joc nou
- Click stânga — dezvăluie o celulă (dacă e goală, dezvăluie automat și celulele adiacente fără mine, în lanț)
- Click dreapta — pune/scoate un steag (flag) pe o celulă nedezvăluită
- Detectare automată a condiției de câștig (toate celulele fără mine dezvăluite) și de pierdere (ai dat click pe o mină)
- Buton de restart în bara de sus
- Serie de câștiguri consecutive (streak), salvată în `streak.txt` — se resetează la 0 dacă pierzi

---

## Tehnologii folosite

- .NET 10, C#
- Silk.NET (bindings SDL2) — `Silk.NET.SDL`, `Silk.NET.Windowing.Sdl`, `Silk.NET.Input.Sdl`
- SixLabors.ImageSharp — pentru încărcarea texturilor (`.bmp`) folosite pentru celule, steaguri, mine etc.

---

## Structura proiectului

```
Minesweeper-main/
├── Program.cs         # Punct de intrare: init SDL, bucla de joc, input, randare
├── Table.cs           # Logica jocului: grilă, plasare mine, flood fill, condiție de câștig
├── Cell.cs            # Model pentru o celulă (mină, dezvăluită, steag, nr. mine adiacente)
├── SdlContext.cs       # Încarcă biblioteca nativă SDL2 în funcție de OS/arhitectură
├── KeyCodes.cs         # Enum cu codurile de taste
├── MouseButton.cs      # Enum cu butoanele de mouse
├── assets/             # Texturi .bmp (celule, cifre, steag, mine, ecran de victorie etc.)
├── streak.txt          # Numărul curent de victorii consecutive (persistă între rulări)
├── TheAdventure.csproj
└── TheAdventure.sln
```

---

## Instalare și rulare locală

### Cerințe preliminare

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### 1. Clonează proiectul

```bash
git clone https://github.com/<user>/Minesweeper.git
cd Minesweeper-main
```

### 2. Restaurează pachetele NuGet

```bash
dotnet restore
```

Biblioteca nativă SDL2 vine inclusă automat prin pachetele Silk.NET, deci nu trebuie instalată separat.

### 3. Rulează jocul

```bash
dotnet run
```

Se deschide o fereastră de 800x800. Click stânga pentru a dezvălui o celulă, click dreapta pentru a pune un steag, și butonul din bara de sus pentru a începe un joc nou.

---

## Observații

- Codul caută texturile în folderul `Assets/` (cu literă mare), în timp ce folderul din proiect e `assets/` (literă mică) — pe sisteme sensibile la majuscule/minuscule (ex. Linux), asta poate face ca texturile să nu se încarce; verifică sau redenumește folderul dacă întâmpini probleme la rulare pe alt sistem decât Windows
- Metoda `FloodReveal` din `Table.cs` e marcată explicit ca fiind generată cu AI
