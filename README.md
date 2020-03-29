# gbdotnet

A Game Boy emulator written in .NET Core using [MonoGame](http://www.monogame.net/) as a frontend.

![Tetris](screenshots/tetris.png "Tetris")

## Running

```cmd
dotnet run rom.gb
```

Optional settings can be specified in `appsettings.json`.

## Controls

* Arrow keys = D-pad
* <kbd>X</kbd> = A
* <kbd>Z</kbd> = B
* <kbd>Enter</kbd> = Start
* <kbd>Right Shift</kbd> = Select

### Debug

* <kbd>Tab</kbd> = Speed toggle
* <kbd>Space</kbd> = Show screen layer
* <kbd>T</kbd> = Show tile layer
* <kbd>B</kbd> = Show background layer
* <kbd>W</kbd> = Show window layer
* <kbd>S</kbd> = Show sprite layer
* <kbd>F1</kbd> = Core dump
* <kbd>F2</kbd> = Restart
