This program implements a photo-sorting application in which the user gets to move around and resize the images 
that are in some folder. 

It's an attempt to simulate multi-touch interaction on a mouse-based device, so you, the user get to

* right-click at a point on some image to create a little pink dot that you can (left)-drag around, which drags the photo
with it. That corresponds to single-touch interaction. Right-clicking again on a dot removes it. If you drag the contact off-screen, 
you may need to resize the screen to make it visible again so that you can keep moving it. 

* right click on two different points on some image (they have to be in the same image), after which you can left drag them one at a 
time to resize the image. 

The program begins with a file-chooser dialog that asks you to pick a folder in which your images are stored. The "Photos" folder 
of the source directory is a good one if you don't have another in mind. 


Program structure:
The PhotoDisplay is the Canvas on which the photos are shown, which maintains a list of photos. Clicking on this canvas in the right
place (i.e., on some photo) creates an Interactor, which is the thing that displays the dots, moves the photos, etc.; the clicked
point becomes a Contact, and right-clicking again (on the same photo) creates a second contact. (No more than two contacts 
are allowed).  

The Interactor does the main work of the program, determining how the Contacts (represented by the pink dots) have been moved,
and hence how the photo's display-transformation should be edited. 

A Contact is simply a glorified Dot: it's a draggable point on the screen. In response to a right-click, it tells the Interactor
to which it belongs that this contact should be removed. The left-click and drag operations simply move the Contact. 

