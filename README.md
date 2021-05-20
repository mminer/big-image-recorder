# Big Image Recorder

This [Unity Recorder](https://docs.unity3d.com/Manual/com.unity.recorder.html)
plugin allows you to capture an image sequence at a higher resolution than the
maximum texture size. Want to render your scene at 100,000 x 100,000 pixels? You
got it friend.

It accomplishes this by dividing the camera's projection matrix and saving the
result as individual tile images for you to stitch together. At present this
stitching operation is left up to you (but see below for recommendations).


## Installing

Add the package to your project via the Unity Package Manager (UPM).

1. Open *Window > Package Manager*
2. If it's not already in your project, install Recorder from the Unity Registry
3. Click "+" in the top-left corner and choose "Add package from git URL..."
4. Enter https://github.com/mminer/big-image-recorder.git

You can also clone this repository and point UPM to your local copy.


## Using

1. Open *Window > General > Recorder > Recorder Window*
2. Click "Add Recorder" and choose "Big Image Sequence"
3. Enter the tag of your target camera (or keep the default to use your main camera)
4. Enter your desired output size, number of rows and columns, and start recording

### Image Stitching

The recorder plugin spits out multiple images per frame, one for each "tile". By
default these are named *image_FRAME_ROW-COLUMN.png*, e.g. *image_0003_1-1.png*.
One option to stitch these together into a final image is
[ImageMagick](https://imagemagick.org).

    montage -mode concatenate -tile 2x2 image_0000_*.png out.png
