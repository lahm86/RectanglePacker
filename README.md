# RectanglePacker
RectanglePacker provides a solution to the bin-packing problem. Given a finite number of bins, and a collections of items to add to them, RectanglePacker attempts to fill the bins using as little space as possible. The solution is currently targeted at rectangles and squares only.

# Usage
This application uses a MIT license as described in the LICENSE file. Follow the steps below to download and use the application.

_Prerequisites_
* Windows 7 SP1, Windows 8.1, Windows 10
* .NET Framework 4.7.2

_Install Steps_
* Download the latest release from https://github.com/lahm86/RectanglePacker/releases
* Extract the zip file to any location on your PC.

### Library
To use RectanglePacker in your own solution, after making a reference to RectanglePacker.dll:

* Implement `IRectangle` in your solution to use as the basis for the rectangles you wish to pack. Alternatively, simply use or extend `DefaultRectangle`.
* Implement `ITile` in your solution to use as the actual bins into which rectangles will be packed. Again, you can use or extend `DefaultTile`.
* `AbstractPacker` is the basis for the main packing algorithm. Extend this or use `DefaultPacker` if you are using `DefaultRectangle` and `DefaultTile`.

#### Packing Options
The following options can be configured to change the packing behaviour.

* `PackingFillMode`
  * `Horizontal` - rectangles will be added to tiles from left to right, top to bottom.
  * `Vertical` - rectangles will be added to tiles from top to bottom, left to right.
* `PackingGroupMode`
  * `None` - no grouping will be performed.
  * `Squares` - all squares will be grouped together and packed first.
* `PackingOrderMode` - rectangles will be sorted by comparing one of the following properties.
  * `Width`
  * `Height`
  * `Area`
  * `Perimiter`
* `PackingOrder` - works in combination with `PackingOrderMode`.
  * `Ascending`
  * `Descending`
* `PackingStartMethod`
  * `FirstTile` - packing will begin on the first tile.
  * `EndTile` - packing will begin on the last occupied tile - this and the following option are useful for speed if the tiles have pre-filled content.
  * `NewEndTile` - if there is available space for a new tile, this will be added and packing will commence here.

## User Interface
The UI provides a visual representation of how packing is carried out, and allows you to test the best combination of options for your particular problem. Use the File menu to:
* **Generate Rectangles** - you can specify a number of rectangles to generate automatically.
* **Import Rectangles** - choose a JSON file that contains an array of [Width, Height] values. See SampleData\SampleSizes.json in the provided solution.
* **Import Images** - choose a list of image files. The UI solution contains an extension of `DefaultRectangle` to support images.

Following is a packing demo.

![UI Packing Demo](https://github.com/lahm86/RectanglePacker/blob/main/Resources/PackingDemo.gif)
