The Testbed2DApp is slightly different from what's described in the book. In particular, the creation of the stripe 
image has been somewhat simplified: there's now an image-constructor that takes an n x k x 4 array of pixels and
uses it to build an RGBA image, so we use that rather than the linear-array-of-gray-values described iin the book. 

There's also a second piece of sample text, showing that failure to set the yUp flag to "false" leads to upside down text. 

