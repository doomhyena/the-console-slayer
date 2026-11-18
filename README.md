# The Console Slayer

Egyszerű, konzolon futó Doom-hangulatú lövöldözős játék C#-ban. A projekt
egyetemi, első féléves beadandóként készült, és a konzolos megjelenítés,
objektumorientált programozás, valamint egy valós időben frissülő játékmenet
alapjait mutatja be.

## A játékról

A játékos egy pályán mozog, felszerelést gyűjt, démonokat győz le, majd
megpróbálja elérni a kijáratot. A pálya ASCII-karakterekből épül fel, a
játékelemek pedig színekkel jelennek meg a konzolban.

### Célok

- érd el az `E` karakterrel jelölt kijáratot;
- győzd le a démonokat a lehető legmagasabb pontszámért;
- használd fel okosan a rendelkezésre álló lőszert és BFG-cellákat;
- kerüld el a mérgező hulladékot és a démonok támadásait.

## Vezérlés

| Billentyű | Művelet |
| --- | --- |
| `↑` `↓` `←` `→` | Mozgás |
| `A` | Lövés |
| `S` | BFG-támadás |
| `D` | Közvetlen interakció: ajtó nyitása/zárása vagy kijárat használata |
| `Esc` | Kilépés |

Az ammo, a BFG-cellák és a medikit automatikusan felvehető, amikor a játékos
az adott mezőre lép.

## Pályaelemek

| Karakter | Jelentés |
| --- | --- |
| `W` | Fal |
| `D` | Ajtó |
| `E` | Kijárat |
| `A` | Lőszer |
| `B` | BFG-cella |
| `M` | Medikit |
| `T` | Mérgező hulladék |
| `z` | Zombieman |
| `i` | Imp |
| `m` | Mancubus |
| `p` | Játékos kezdőpozíciója |
| `_` | Üres, járható mező |

A játékos látótávolsága korlátozott. A démonok a saját látó- és támadási
távolságuk alapján mozognak és támadnak, a mérgező hulladék pedig a játékost és
a démonokat is sebzi.

## Követelmények

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows-kompatibilis konzol

## Futtatás

1. Klónozd a repository-t:

   ```bash
   git clone https://github.com/doomhyena/the-console-slayer.git
   cd the-console-slayer
   ```

2. Indítsd el a projektet:

   ```bash
   dotnet run --project the_console_slayer/the_console_slayer.csproj
   ```

Build készítése:

```bash
dotnet build the_console_slayer/the_console_slayer.csproj
```

Érdemes olyan konzolablakot használni, amely legalább a pálya szélességénél és
a HUD számára fenntartott sornál magasabb. A játék befejezésekor a konzol
kiírja, hogy a játékos meghalt-e vagy teljesítette a szintet, valamint
megjeleníti a pontszámot.

## Projektstruktúra

```text
the-console-slayer/
├── the_console_slayer/
│   ├── Program.cs                 # A játék logikája és a pályaleírás
│   ├── the_console_slayer.csproj  # .NET projektbeállítások
│   └── the_console_slayer.slnx    # Solution fájl
├── LICENSE
└── README.md
```

## Megvalósítás

A játék egyetlen konzolos alkalmazásként működik. A főbb modellek:

- `Position` – koordináták és távolságszámítás;
- `ConsoleSprite` – karakter- és színmegjelenítés;
- `GameItem` – pályaelemek, felszerelések és ajtók;
- `Player` – életerő, lőszer, BFG-cellák és pontszám;
- `Demon` – ellenfelek eltérő életerővel, sebzéssel, sebességgel és
  látótávolsággal;
- `Game` – pályabetöltés, inputkezelés, játékmenet, démonfrissítés és
  renderelés.

A pálya a `Program.cs` fájlban található karaktertömbként, ezért új pályák
létrehozásához ezt a tömböt lehet módosítani.

## Licenc

A projekt az [MIT licenc](LICENSE) feltételei szerint használható.
